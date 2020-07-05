using System;
using System.Threading.Tasks;
using AuthDtos.Request;
using AuthRepository.Interfaces;
using AuthService.Interfaces;

namespace AuthService.Services
{
    public class AuthService : IAuthService
    {

        private readonly IAuthRepository _authRepository;

        public AuthService(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        public async Task<string> Auth(JwtAuthRequest request)
        {
            var result = await _authRepository.Auth(request);
            return result;
        }

        public async Task<Guid> CreateAuth(string username, string password)
        {
            return await _authRepository.CreateAuth(username, password);
        }
    }
}
