using System;
using System.Collections.Generic;
using System.Text;
using AuthRepository.DataModels;
using Microsoft.EntityFrameworkCore;
using JetBrains.Annotations;

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
            modelBuilder.Entity<AuthUser>()
                .HasIndex(x => x.Username)
                .IsUnique()
                .HasFilter(null);

            modelBuilder.Entity<JwtRefreshToken>()
                .HasKey(x => new {x.UserGuid, x.RefreshTokenString});
            modelBuilder.Entity<JwtRefreshToken>()
                .HasOne(x => x.AuthUser)
                .WithMany(x => x.RefreshTokens)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<JwtRefreshToken>()
                .HasIndex(x => x.ExpiresAt);
            modelBuilder.Entity<JwtRefreshToken>()
                .HasIndex(x => x.IssuedAt);

        }
    }
}
