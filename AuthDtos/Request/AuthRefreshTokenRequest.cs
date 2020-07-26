using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using JwtAuth;
using MiniGuids;

namespace AuthDtos.Request
{
    public class AuthRefreshTokenRequest
    {
        [Required]
        public string GrantType { get; set; }

        [Required, JsonConverter(typeof(JsonMiniGuidConverter))]
        public MiniGuid UserId { get; set; }
        
        [Required]
        public string RefreshToken { get; set; }
    }
}
