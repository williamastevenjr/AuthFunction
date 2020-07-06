using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AuthRepository.DataModels
{
    public class AuthUser
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Guid { get; set; }
        
        [Required, MaxLength(30)]
        public string Username { get; set; }

        [Required, MinLength(264), MaxLength(264)]
        public byte[] Salt { get; set; }

        [Required, MinLength(264), MaxLength(264)]
        public byte[] PasswordHash { get; set; }

        public virtual IList<JwtRefreshToken> RefreshTokens { get; set; }
    }
}
