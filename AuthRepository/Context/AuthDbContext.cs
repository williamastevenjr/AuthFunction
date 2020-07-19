using System;
using AuthRepository.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MiniGuids;

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
                .HasForeignKey(x => x.AuthRoleId);

            ////AuthUserRoles
            //modelBuilder.Entity<AuthUserRole>()
            //    .HasKey(x => new {x.AuthUserId, x.AuthRoleId});
            //modelBuilder.Entity<AuthUserRole>()
            //    .HasOne(x => x.AuthRole)
            //    .WithMany()
            //    .HasForeignKey(x => x.AuthRoleId);

            //JwtRefreshToken
            modelBuilder.Entity<JwtRefreshToken>()
                .HasKey(x => new {x.UserId, x.RefreshTokenString});
            modelBuilder.Entity<JwtRefreshToken>()
                .HasIndex(x => x.ExpiresAt);
        }

    }
}
