using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthRepository.DataModels
{
    public class AuthUser
    {
        [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }
        
        [Required, MaxLength(30)]
        public string Username { get; set; }

        [Required]
        public byte AuthRoleId { get; set; }

        [Required, MinLength(264), MaxLength(264)]
        public byte[] Salt { get; set; }

        [Required, MinLength(264), MaxLength(264)]
        public byte[] PasswordHash { get; set; }
        
        public virtual ICollection<JwtRefreshToken> RefreshTokens { get; set; }
        public virtual AuthRole AuthRole { get; set; }
    }
}
