using System;
using System.Text.Json.Serialization;

namespace AuthDtos.Response
{
    public class JwtAuthResponse
    {
        public JwtAuthResponse(string token, string refreshToken, DateTime refreshTokenExpiration)
        {
            Token = token;
            RefreshToken = refreshToken;
            RefreshTokenExpiration = refreshTokenExpiration;
        }

        public string Token { get; set; }

        public string TokenType => "bearer";
        
        public string RefreshToken { get; set; }

        public int ExpiresIn => 60 * 60 * 24;

        [JsonIgnore]
        public DateTime RefreshTokenExpiration { get; set; }

        public int RefreshTokenExpires => (int) ((DateTimeOffset) RefreshTokenExpiration).ToUnixTimeSeconds();
    }
}
