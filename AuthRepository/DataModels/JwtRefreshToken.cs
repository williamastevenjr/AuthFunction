using System;
using System.ComponentModel.DataAnnotations;
using MiniGuids;

namespace AuthRepository.DataModels
{
    public class JwtRefreshToken
    {
        [Required, MinLength(512), MaxLength(512)]
        public string RefreshTokenString { get; set; }

        [Required]
        public MiniGuid UserId { get; set; }

        [Required]
        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
        
        [Required]
        public DateTime ExpiresAt { get; set; }
        
        public virtual AuthUser AuthUser { get; set; }
    }
}
