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
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AuthRepository.Implementations
{
    public class AuthRepository: IAuthRepository
    {
        private readonly Func<AuthDbContext> _contextFactory;

        public AuthRepository(Func<AuthDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<JwtAuthResponse> Auth(JwtAuthRequest request)
        {
            await using var context = _contextFactory.Invoke();
            var userResult = await context.AuthUsers.Where(x => x.Username.Equals(request.Username))
                .Select(x=> new
                {
                    salt = x.Salt,
                    hash = x.PasswordHash,
                    guid = x.Guid
                })
                .FirstOrDefaultAsync();
            JwtAuthResponse response = null;
            if (userResult != null)
            {
                if (Validate(request.Password, userResult.salt, userResult.hash))
                {
                    var jwt = GenerateJwt(userResult.guid);
                    var refreshToken = GenerateRefreshToken(userResult.guid);
                    if (!string.IsNullOrWhiteSpace(refreshToken) && !string.IsNullOrWhiteSpace(jwt))
                    {
                        var expiration = DateTime.UtcNow.AddDays(31);
                        var refreshTokenStore = new JwtRefreshToken
                        {
                            UserGuid = userResult.guid,
                            ExpiresAt = expiration,
                            RefreshTokenString = refreshToken
                        };
                        await context.RefreshTokens.AddAsync(refreshTokenStore);
                        await context.SaveChangesAsync();

                        response = new JwtAuthResponse(jwt, refreshToken, expiration);
                    }
                }
                else
                {
                    throw new Exception("fuck off");
                }
            }
            
            return response;
        }

        public async Task<JwtAuthResponse> RefreshTokenAuth(AuthRefreshTokenRequest refreshTokenRequest)
        {
            await using var context = _contextFactory.Invoke();
            var refresh = await context.RefreshTokens.FirstOrDefaultAsync(x =>
                x.UserGuid.Equals(refreshTokenRequest.UserGuid) &&
                x.RefreshTokenString.Equals(refreshTokenRequest.RefreshToken));
            JwtAuthResponse response = null;
            if (refresh != null)
            {
                var jwt = GenerateJwt(refreshTokenRequest.UserGuid);
                var refreshToken = GenerateRefreshToken(refreshTokenRequest.UserGuid);
                if (!string.IsNullOrWhiteSpace(refreshToken) && !string.IsNullOrWhiteSpace(jwt))
                {
                    var expiration = DateTime.UtcNow.AddDays(31);
                    var refreshTokenStore = new JwtRefreshToken
                    {
                        UserGuid = refreshTokenRequest.UserGuid,
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
                    Guid = Guid.NewGuid(),
                    Salt = saltAndHash.salt,
                    PasswordHash = saltAndHash.hash
                };
                await context.AuthUsers.AddAsync(account);
                var jwt = GenerateJwt(account.Guid);
                var refreshToken = GenerateRefreshToken(account.Guid);
                var expiration = DateTime.UtcNow.AddDays(31);
                var refreshTokenStore = new JwtRefreshToken
                {
                    UserGuid = account.Guid,
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
            var refreshTokens = await context.RefreshTokens.Where(x => x.UserGuid.Equals(userGuid))
                .ToListAsync();
            context.RefreshTokens.RemoveRange(refreshTokens);
            await context.SaveChangesAsync();
            return true;
        }

        private string GenerateRefreshToken(Guid userGuid)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()_-;:'<>,.|+=/";
            StringBuilder res = new StringBuilder();
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] uintBuffer = new byte[sizeof(uint)];
                var length = 512;
                while (length-- > 0)
                {
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    res.Append(valid[(int)(num % (uint)valid.Length)]);
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
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("SmokinessPatienceOpulentlyMannedMothproofTreeBufferHuntsman");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = "dummythiccapi",
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userGuid.ToString()),
                    new Claim(ClaimTypes.Role, "User")
                }),
                Audience = "dummythiccapi",
                IssuedAt = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(1),

                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);

            //var payload = new Dictionary<string, object>
            //{
            //    { "iss", "dummythiccapi.auth" },
            //    { "sub", userGuid.ToString() },
            //    { "aud", "dummythiccapi.*" },
            //    { "exp", (int)((DateTimeOffset)DateTime.UtcNow.AddDays(1)).ToUnixTimeSeconds() },
            //    { "iat", (int)((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds() },
            //    { "jti", Guid.NewGuid().ToString() },
            //    { "rol", "user" }
            //};

            //const string secret = "SmokinessPatienceOpulentlyMannedMothproofTreeBufferHuntsman";

            //IJwtAlgorithm algorithm = new HMACSHA256Algorithm(); // symmetric
            //IJsonSerializer serializer = new JsonNetSerializer();
            //IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            //IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

            //var token = encoder.Encode(payload, secret);
            //return token;
        }

        
    }
}
