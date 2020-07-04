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

        public AuthDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuthUser>()
                .HasIndex(x => x.Username)
                .IsUnique()
                .HasFilter(null);
        }
    }
}
