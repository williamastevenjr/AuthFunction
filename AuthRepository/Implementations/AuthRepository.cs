using System;
using System.Collections.Generic;
using System.Text;
using AuthDtos.Request;
using AuthRepository.Interfaces;

namespace AuthRepository.Implementations
{
    public class AuthRepository: IAuthRepository
    {
        public bool Auth(JwtAuthRequest request)
        {
            return true;
        }
    }
}
