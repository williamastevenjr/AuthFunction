using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AuthDtos.Request;

namespace AuthRepository.Interfaces
{
    public interface IAuthRepository
    {
        Task<Guid> CreateAuth(string username, string password);
        Task<string> Auth(JwtAuthRequest request);
    }
}
