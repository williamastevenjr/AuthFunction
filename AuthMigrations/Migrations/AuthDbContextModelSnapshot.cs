﻿// <auto-generated />
using System;
using AuthRepository.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AuthMigrations.Migrations
{
    [DbContext(typeof(AuthDbContext))]
    partial class AuthDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("AuthRepository.DataModels.AuthRole", b =>
                {
                    b.Property<byte>("Id")
                        .HasColumnType("tinyint unsigned");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(10) CHARACTER SET utf8mb4")
                        .HasMaxLength(10);

                    b.HasKey("Id");

                    b.ToTable("AuthRole");
                });

            modelBuilder.Entity("AuthRepository.DataModels.AuthUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("char(26)");

                    b.Property<byte>("AuthRoleId")
                        .HasColumnType("tinyint unsigned");

                    b.Property<byte[]>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("varbinary(264)")
                        .HasMaxLength(264);

                    b.Property<byte[]>("Salt")
                        .IsRequired()
                        .HasColumnType("varbinary(264)")
                        .HasMaxLength(264);

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("varchar(30) CHARACTER SET utf8mb4")
                        .HasMaxLength(30);

                    b.HasKey("Id");

                    b.HasIndex("AuthRoleId");

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("AuthUsers");
                });

            modelBuilder.Entity("AuthRepository.DataModels.JwtRefreshToken", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("char(26)");

                    b.Property<string>("RefreshTokenString")
                        .HasColumnType("varchar(512) CHARACTER SET utf8mb4")
                        .HasMaxLength(512);

                    b.Property<DateTime>("ExpiresAt")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("IssuedAt")
                        .HasColumnType("datetime(6)");

                    b.HasKey("UserId", "RefreshTokenString");

                    b.HasIndex("ExpiresAt");

                    b.ToTable("RefreshTokens");
                });

            modelBuilder.Entity("AuthRepository.DataModels.AuthUser", b =>
                {
                    b.HasOne("AuthRepository.DataModels.AuthRole", "AuthRole")
                        .WithMany()
                        .HasForeignKey("AuthRoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AuthRepository.DataModels.JwtRefreshToken", b =>
                {
                    b.HasOne("AuthRepository.DataModels.AuthUser", "AuthUser")
                        .WithMany("RefreshTokens")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
