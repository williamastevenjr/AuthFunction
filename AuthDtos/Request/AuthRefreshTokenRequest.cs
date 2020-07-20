using System;
using System.ComponentModel.DataAnnotations;
using JwtAuth;
using MiniGuids;
using Newtonsoft.Json;

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
