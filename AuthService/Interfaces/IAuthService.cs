using System;
using System.Threading.Tasks;
using AuthDtos.Request;
using AuthDtos.Response;
using MiniGuids;

namespace AuthService.Interfaces
{
    public interface IAuthService
    {
        Task<JwtAuthResponse> Auth(JwtAuthRequest request);
        Task<JwtAuthResponse> RefreshTokenAuth(AuthRefreshTokenRequest request);
        Task<JwtAuthResponse> CreateAuth(string username, string password);

        Task<bool> RemoveRefreshTokens(string userId);

        Task<bool> RemoveExpiredRefreshTokens();
    }
}
