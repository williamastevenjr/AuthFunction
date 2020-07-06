using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Newtonsoft.Json;

namespace AuthDtos.Request
{
    public class AuthRefreshTokenRequest
    {
        [Required, JsonProperty("grant_type")]
        public string GrantType { get; set; }

        [Required, JsonProperty("client_id")]
        public Guid UserGuid { get; set; }

        [Required, JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
    }
}
