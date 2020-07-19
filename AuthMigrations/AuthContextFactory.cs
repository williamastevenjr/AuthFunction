using System;
using AuthRepository.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Thinktecture;
using Ws.EfCore.Extensions.AppSettings;
using Ws.EfCore.Extensions.TypeMappingExtensions;

namespace AuthMigrations
{
    public class AuthContextFactory : IDesignTimeDbContextFactory<AuthDbContext>
    {
        public AuthDbContext CreateDbContext(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            var mySqlAppSettings = new MySqlAppSettings(configuration.GetSection("MySql"));

            var builder = new DbContextOptionsBuilder<AuthDbContext>();
            builder.UseMySql(configuration.GetConnectionString("AuthDb"), b=>b.MigrationsAssembly("AuthMigrations")
                    .ServerVersion(new Version(mySqlAppSettings.Major, mySqlAppSettings.Minor, mySqlAppSettings.Build), ServerType.MySql));
                //.AddRelationalTypeMappingSourcePlugin<MiniGuidTypeMappingPlugin>();

            return new AuthDbContext(builder.Options);
        }
    }
}
