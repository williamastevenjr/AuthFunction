using System;
using System.ComponentModel.DataAnnotations;
using JwtAuth;
using MiniGuids;
using Newtonsoft.Json;

namespace AuthDtos.Request
{
    public class AuthRefreshTokenRequest
    {
        [Required, JsonProperty("grant_type")]
        public string GrantType { get; set; }

        [Required, JsonProperty("client_id"), JsonConverter(typeof(JsonMiniGuidConverter))]
        public MiniGuid ClientId { get; set; }

        [JsonIgnore] 
        public string UserId => ClientId.ToString();

        [Required, JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
    }
}
