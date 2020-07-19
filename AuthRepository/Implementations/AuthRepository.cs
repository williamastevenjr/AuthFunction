using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AuthDtos.Request;
using AuthDtos.Response;
using AuthRepository.Context;
using AuthRepository.DataModels;
using AuthRepository.Interfaces;
using JwtAuth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MiniGuids;

namespace AuthRepository.Implementations
{
    public class AuthRepository: IAuthRepository
    {
        private readonly Func<AuthDbContext> _contextFactory;
        private readonly IConfigurationSection _jwtConfiguration;
        private readonly string _validRefreshCharSet;

        public AuthRepository(Func<AuthDbContext> contextFactory, IConfiguration configuration)
        {
            _contextFactory = contextFactory;
            _validRefreshCharSet =
                configuration.GetSection("JwtIssuerOptions").GetSection("RefreshToken")["RefreshTokenCharSet"] ??
                throw new Exception("missing valid refresh token char set");
            _jwtConfiguration = configuration.GetSection("JwtIssuerOptions") ?? throw new Exception("MissingJwtIssuerOptions");
        }

        public async Task<JwtAuthResponse> Auth(JwtAuthRequest request)
        {
            await using var context = _contextFactory.Invoke();
            var userResult = await context.AuthUsers.Where(x => x.Username.Equals(request.AccountName))
                .Select(x=> new
                {
                    salt = x.Salt,
                    hash = x.PasswordHash,
                    guid = x.Id
                })
                .FirstOrDefaultAsync();

            JwtAuthResponse response = null;
            if (userResult != null)
            {
                if (Validate(request.Password, userResult.salt, userResult.hash))
                {
                    var jwt = GenerateJwt(userResult.guid);
                    var refreshToken = GenerateRefreshToken();
                    if (!string.IsNullOrWhiteSpace(refreshToken) && !string.IsNullOrWhiteSpace(jwt))
                    {
                        var expiration = DateTime.UtcNow.AddDays(31);
                        var refreshTokenStore = new JwtRefreshToken
                        {
                            UserId = userResult.guid,
                            ExpiresAt = expiration,
                            RefreshTokenString = refreshToken
                        };
                        await context.RefreshTokens.AddAsync(refreshTokenStore);
                        await context.SaveChangesAsync();

                        response = new JwtAuthResponse(jwt, refreshToken, expiration);
                    }
                }
            }
            
            return response;
        }

        public async Task<JwtAuthResponse> RefreshTokenAuth(AuthRefreshTokenRequest refreshTokenRequest)
        {
            await using var context = _contextFactory.Invoke();
            var refresh = await context.RefreshTokens.FirstOrDefaultAsync(x =>
                x.UserId.Equals(refreshTokenRequest.UserGuid) &&
                x.RefreshTokenString.Equals(refreshTokenRequest.RefreshToken));
            JwtAuthResponse response = null;
            if (refresh != null)
            {
                var jwt = GenerateJwt(refreshTokenRequest.UserGuid);
                var refreshToken = GenerateRefreshToken();
                if (!string.IsNullOrWhiteSpace(refreshToken) && !string.IsNullOrWhiteSpace(jwt))
                {
                    var expiration = DateTime.UtcNow.AddDays(31);
                    var refreshTokenStore = new JwtRefreshToken
                    {
                        UserId = refreshTokenRequest.UserGuid,
                        ExpiresAt = expiration,
                        RefreshTokenString = refreshToken
                    };
                    await context.RefreshTokens.AddAsync(refreshTokenStore);
                    context.RefreshTokens.Remove(refresh);
                    await context.SaveChangesAsync();

                    response = new JwtAuthResponse(jwt, refreshToken, expiration);
                }
            }

            return response;
        }

        public async Task<bool> RemoveExpiredRefreshTokens()
        {
            await using var context = _contextFactory.Invoke();
            var expiredTokens = await context.RefreshTokens.Where(x => x.ExpiresAt <= DateTime.UtcNow).ToListAsync();
            if (expiredTokens.Count > 0)
            {
                context.RefreshTokens.RemoveRange(expiredTokens);
                await context.SaveChangesAsync();
            }

            return true;
        }

        public async Task<JwtAuthResponse> CreateAuth(string username, string password)
        {
            await using var context = _contextFactory.Invoke();
            var taken = (await context.AuthUsers.CountAsync(x => x.Username.Equals(username))) != 0;
            if (!taken)
            {
                var saltAndHash = SaltAndHash(password);
                var account = new AuthUser
                {
                    Username = username,
                    Id = MiniGuid.NewGuid(),
                    Salt = saltAndHash.salt,
                    PasswordHash = saltAndHash.hash,
                    AuthRoleId = (byte)JwtRole.User
                };
                await context.AuthUsers.AddAsync(account);
                var jwt = GenerateJwt(account.Id);
                var refreshToken = GenerateRefreshToken();
                var expiration = DateTime.UtcNow.AddDays(31);
                var refreshTokenStore = new JwtRefreshToken
                {
                    UserId = account.Id,
                    ExpiresAt = expiration,
                    RefreshTokenString = refreshToken
                };
                await context.RefreshTokens.AddAsync(refreshTokenStore);
                await context.SaveChangesAsync();
                var response = new JwtAuthResponse(jwt, refreshToken, expiration);
                return response;
            }
            throw new Exception("todo");
        }

        public async Task<bool> RemoveRefreshTokens(Guid userGuid)
        {
            await using var context = _contextFactory.Invoke();
            var refreshTokens = await context.RefreshTokens.Where(x => x.UserId.Equals(userGuid))
                .ToListAsync();
            context.RefreshTokens.RemoveRange(refreshTokens);
            await context.SaveChangesAsync();
            return true;
        }

        private string GenerateRefreshToken()
        {
            StringBuilder res = new StringBuilder();
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] uintBuffer = new byte[sizeof(uint)];
                var length = 512;
                while (length-- > 0)
                {
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    res.Append(_validRefreshCharSet[(int)(num % (uint)_validRefreshCharSet.Length)]);
                }
            }
            return res.ToString();
        }

        private bool Validate(string password, byte[] salt, byte[] hash)
        {
            using RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
            int iterations = 10000;
            Rfc2898DeriveBytes derived = new Rfc2898DeriveBytes(password, salt, iterations);
            return derived.GetBytes(256).SequenceEqual(hash);
        }
        
        private (byte[] salt, byte[] hash) SaltAndHash(string password)
        {
            var salt = new byte[256];
            using RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
            rngCsp.GetBytes(salt);
            int iterations = 10000;
            Rfc2898DeriveBytes derived = new Rfc2898DeriveBytes(password, salt, iterations);
            return (salt, derived.GetBytes(256));
        }

        private string GenerateJwt(Guid userGuid)
        {
            // generate token that is valid for 1 day
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("SmokinessPatienceOpulentlyMannedMothproofTreeBufferHuntsman");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _jwtConfiguration["JwtIssuer"],
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("sub", MiniGuid.NewGuid()),
                    new Claim(ClaimTypes.Role, "User")
                }),
                Audience = _jwtConfiguration["JwtIssuer"],
                IssuedAt = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(int.Parse(_jwtConfiguration["JwtExpireDays"])),

                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);

        }
    }
}
