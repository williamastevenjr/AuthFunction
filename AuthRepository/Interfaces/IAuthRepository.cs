using System;
using System.Threading.Tasks;
using AuthDtos.Request;
using AuthDtos.Response;
using MiniGuids;

namespace AuthRepository.Interfaces
{
    public interface IAuthRepository
    {
        Task<JwtAuthResponse> CreateAuth(string username, string password);
        Task<JwtAuthResponse> Auth(JwtAuthRequest request);
        Task<JwtAuthResponse> RefreshTokenAuth(AuthRefreshTokenRequest refreshTokenRequest);
        Task<bool> RemoveRefreshTokens(MiniGuid userId);

        Task<bool> RemoveExpiredRefreshTokens();
    }
}
