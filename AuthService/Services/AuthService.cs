using System;
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

        public bool Auth(JwtAuthRequest request)
        {
            _authRepository.Auth(request);
            return true;
        }
    }
}
