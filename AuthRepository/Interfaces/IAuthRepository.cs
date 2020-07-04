using System;
using System.Collections.Generic;
using System.Text;
using AuthDtos.Request;

namespace AuthRepository.Interfaces
{
    public interface IAuthRepository
    {
        bool Auth(JwtAuthRequest request);
    }
}
