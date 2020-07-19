using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MiniGuids;

namespace AuthRepository.DataModels
{
    public class AuthUser
    {
        [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public MiniGuid Id { get; set; }
        
        [Required, MaxLength(30)]
        public string Username { get; set; }

        [Required, MinLength(264), MaxLength(264)]
        public byte[] Salt { get; set; }

        [Required, MinLength(264), MaxLength(264)]
        public byte[] PasswordHash { get; set; }

        [Required]
        public byte AuthRoleId { get; set; } 

        public virtual IList<JwtRefreshToken> RefreshTokens { get; set; }
        public virtual AuthRole AuthRole { get; set; }
    }
}
