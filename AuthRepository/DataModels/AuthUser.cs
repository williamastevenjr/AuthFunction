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
        public Guid AuthUserGuid { get; set; }

        [Required, MaxLength(30)]
        public string Username { get; set; }

        [Required, MaxLength(264)]
        public byte[] Salt { get; set; }

        [Required, MaxLength(264)]
        public byte[] PasswordHash { get; set; }
    }
}
