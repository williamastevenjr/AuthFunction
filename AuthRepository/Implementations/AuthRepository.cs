using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AuthDtos.Request;
using AuthRepository.Context;
using AuthRepository.DataModels;
using AuthRepository.Interfaces;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Microsoft.EntityFrameworkCore;

namespace AuthRepository.Implementations
{
    public class AuthRepository: IAuthRepository
    {
        private readonly Func<AuthDbContext> _contextFactory;

        public AuthRepository(Func<AuthDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<string> Auth(JwtAuthRequest request)
        {
            await using var context = _contextFactory.Invoke();
            var userResult = await context.AuthUsers.Where(x => x.Username.Equals(request.Username))
                .Select(x=> new
                {
                    salt = x.Salt,
                    hash = x.PasswordHash,
                    guid = x.AuthUserGuid
                })
                .FirstOrDefaultAsync();
            string result = null;
            if (userResult != null)
            {
                if (Validate(request.Password, userResult.salt, userResult.hash))
                {
                    result = GenerateJwt(userResult.guid);
                }
                else
                {
                    throw new Exception("fuck off");
                }
            }
            return result;
        }

        public async Task<Guid> CreateAuth(string username, string password)
        {
            await using var context = _contextFactory.Invoke();
            var taken = (await context.AuthUsers.CountAsync(x => x.Username.Equals(username))) != 0;
            if (!taken)
            {
                var saltAndHash = SaltAndHash(password);
                var account = new AuthUser
                {
                    Username = username,
                    AuthUserGuid = Guid.NewGuid(),
                    Salt = saltAndHash.salt,
                    PasswordHash = saltAndHash.hash
                };
                await context.AuthUsers.AddAsync(account);
                await context.SaveChangesAsync();
                return account.AuthUserGuid;
            }
            throw new Exception("todo");
        }

        public bool Validate(string password, byte[] salt, byte[] hash)
        {
            using RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
            int iterations = 10000;
            Rfc2898DeriveBytes derived = new Rfc2898DeriveBytes(password, salt, iterations);
            return derived.GetBytes(256).SequenceEqual(hash);
        }

        public (byte[] salt, byte[] hash) SaltAndHash(string password)
        {
            var salt = new byte[256];
            using RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
            rngCsp.GetBytes(salt);
            int iterations = 10000;
            Rfc2898DeriveBytes derived = new Rfc2898DeriveBytes(password, salt, iterations);
            return (salt, derived.GetBytes(256));
        }

        public string GenerateJwt(Guid userGuid)
        {
            var payload = new Dictionary<string, object>
            {
                { "iss", "dummythiccapi.auth" },
                { "sub", userGuid.ToString() },
                { "aud", "dummythiccapi.*" },
                { "exp", (int)DateTime.UtcNow.AddDays(1).Subtract(new DateTime(1970, 1, 1)).TotalSeconds },
                { "iat", (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds },
                { "jti", "dummythiccapi.auth" },
                { "rol", "user" }
            };

            const string secret = "SmokinessPatienceOpulentlyMannedMothproofTreeBufferHuntsman";

            IJwtAlgorithm algorithm = new HMACSHA256Algorithm(); // symmetric
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

            var token = encoder.Encode(payload, secret);
            return token;
        }
    }
}
