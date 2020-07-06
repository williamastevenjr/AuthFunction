using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

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

        [JsonProperty("access_token")]
        public string Token { get; set; }

        [JsonProperty("token_type")] 
        public string TokenType => "bearer";
        
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("expires_in")] 
        public int ExpiresIn => 60 * 60 * 24;

        [JsonIgnore]
        public DateTime RefreshTokenExpiration { get; set; }

        [JsonProperty("refresh_token_expiration")]
        public int RefreshTokenExpires => (int) ((DateTimeOffset) RefreshTokenExpiration).ToUnixTimeSeconds();
    }
}
