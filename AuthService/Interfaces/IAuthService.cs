using System;
using System.Collections.Generic;
using System.Text;
using AuthDtos.Request;

namespace AuthService.Interfaces
{
    public interface IAuthService
    {
        bool Auth(JwtAuthRequest request);
    }
}
