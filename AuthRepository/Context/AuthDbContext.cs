using AuthRepository.DataModels;
using Microsoft.EntityFrameworkCore;

namespace AuthRepository.Context
{
    public class AuthDbContext : DbContext
    {
        public DbSet<AuthUser> AuthUsers { get; set; }
        public DbSet<JwtRefreshToken> RefreshTokens { get; set; }

        public AuthDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //AuthUser
            modelBuilder.Entity<AuthUser>()
                .HasIndex(x => x.Username)
                .IsUnique();
            modelBuilder.Entity<AuthUser>()
                .HasMany(x => x.RefreshTokens)
                .WithOne(x => x.AuthUser)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AuthUser>()
                .HasOne(x => x.AuthRole)
                .WithMany()
                .HasForeignKey(x => x.AuthRoleId)
                .OnDelete(DeleteBehavior.Restrict);

            ////AuthUserRoles
            //modelBuilder.Entity<AuthUserRole>()
            //    .HasKey(x => new {x.AuthUserId, x.AuthRoleId});
            //modelBuilder.Entity<AuthUserRole>()
            //    .HasOne(x => x.AuthRole)
            //    .WithMany()
            //    .HasForeignKey(x => x.AuthRoleId);

            //JwtRefreshToken
            modelBuilder.Entity<JwtRefreshToken>()
                .HasKey(x => new {x.UserId, x.RefreshTokenString});//));//{x.UserUuid, x.RefreshTokenString});
            modelBuilder.Entity<JwtRefreshToken>()
                .HasIndex(x => x.ExpiresAt);
        }

    }

    //public sealed class Comp : IComparable<Comp>{
    //    public MiniGuid UserUuid { get; set; }
    //    private string RefreshTokenString { get; set; }
    //    public Comp(MiniGuid mg, string str)
    //    {
    //        UserUuid = mg;
    //        RefreshTokenString = str;
    //    }
        
    //    public int CompareTo(Comp other)
    //    {
    //        if (UserUuid != other.UserUuid)
    //        {
    //            return String.Compare(UserUuid.ToString(), other.UserUuid.ToString(), StringComparison.Ordinal);
    //        }
    //        else
    //        {
    //            if (RefreshTokenString != other.RefreshTokenString)
    //            {
    //                return String.Compare(RefreshTokenString, other.RefreshTokenString, StringComparison.Ordinal);
    //            }
    //            else
    //            {
    //                return 0;
    //            }
    //        }
    //    }
    //}
}
