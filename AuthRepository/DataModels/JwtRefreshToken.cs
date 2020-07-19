using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MiniGuids;

namespace AuthRepository.DataModels
{
    public class JwtRefreshToken : IComparable<JwtRefreshToken>
    {
        [Required, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public MiniGuid UserId { get; set; }

        //[NotMapped]
        //private int? _hash;
        //[Required]
        //public int HashedKey
        //{
        //    get
        //    {
        //        _hash ??= Math.Abs((UserUuid + RefreshTokenString).GetHashCode() / 2);
        //        return _hash.Value;
        //    }
        //    private set => _hash = value;
        //}

        [Required, MinLength(512), MaxLength(512), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string RefreshTokenString { get; set; }

        [Required]
        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
        
        [Required]
        public DateTime ExpiresAt { get; set; }
        
        public virtual AuthUser AuthUser { get; set; }

        public int CompareTo(JwtRefreshToken other)
        {
            if (UserId != other.UserId)
            {
                return string.Compare(UserId.ToString(), other.UserId.ToString(), StringComparison.Ordinal);
            }
            else
            {
                return RefreshTokenString != other.RefreshTokenString ? string.Compare(RefreshTokenString, other.RefreshTokenString, StringComparison.Ordinal) : 0;
            }
        }
    }
}
