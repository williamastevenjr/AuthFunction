using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MiniGuids;

namespace AuthRepository.DataModels
{
    public class JwtRefreshToken
    {
        [Required, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string UserId { get; set; }
        
        [Required, MinLength(512), MaxLength(512), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string RefreshTokenString { get; set; }

        [Required]
        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
        
        [Required]
        public DateTime ExpiresAt { get; set; }
        
        public virtual AuthUser AuthUser { get; set; }

    }
}
