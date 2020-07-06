using System;
using System.Threading.Tasks;
using AuthDtos.Request;
using AuthDtos.Response;
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

        public async Task<JwtAuthResponse> Auth(JwtAuthRequest request)
        {
            var result = await _authRepository.Auth(request);
            return result;
        }

        public async Task<JwtAuthResponse> RefreshTokenAuth(AuthRefreshTokenRequest request)
        {
            var result = await _authRepository.RefreshTokenAuth(request);
            return result;
        }

        public async Task<JwtAuthResponse> CreateAuth(string username, string password)
        {
            var result = await _authRepository.CreateAuth(username, password);
            return result;
        }
    }
}
