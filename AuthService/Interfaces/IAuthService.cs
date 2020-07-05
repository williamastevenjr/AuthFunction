using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AuthDtos.Request;

namespace AuthService.Interfaces
{
    public interface IAuthService
    {
        Task<string> Auth(JwtAuthRequest request);
        Task<Guid> CreateAuth(string username, string password);
    }
}
