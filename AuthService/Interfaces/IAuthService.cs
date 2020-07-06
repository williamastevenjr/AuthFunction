using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AuthDtos.Request;
using AuthDtos.Response;

namespace AuthService.Interfaces
{
    public interface IAuthService
    {
        Task<JwtAuthResponse> Auth(JwtAuthRequest request);
        Task<JwtAuthResponse> RefreshTokenAuth(AuthRefreshTokenRequest request);
        Task<JwtAuthResponse> CreateAuth(string username, string password);

        Task<bool> RemoveRefreshTokens(Guid userGuid);

        Task<bool> RemoveExpiredRefreshTokens();
    }
}
