using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AuthRepository.DataModels
{
    public class JwtRefreshToken
    {
        [Key, ForeignKey(nameof(AuthUser))]
        public Guid UserGuid { get; set; }

        [Required]
        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
        
        [Required]
        public DateTime ExpiresAt { get; set; }

        [Key, Required, MinLength(512), MaxLength(512)]
        public string RefreshTokenString { get; set; }

        public virtual AuthUser AuthUser { get; set; }
    }
}
