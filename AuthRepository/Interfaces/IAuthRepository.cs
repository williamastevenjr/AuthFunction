using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AuthDtos.Request;
using AuthDtos.Response;

namespace AuthRepository.Interfaces
{
    public interface IAuthRepository
    {
        Task<JwtAuthResponse> CreateAuth(string username, string password);
        Task<JwtAuthResponse> Auth(JwtAuthRequest request);
        Task<JwtAuthResponse> RefreshTokenAuth(AuthRefreshTokenRequest refreshTokenRequest);
        Task<bool> RemoveRefreshTokens(Guid userGuid);

        Task<bool> RemoveExpiredRefreshTokens();
    }
}
