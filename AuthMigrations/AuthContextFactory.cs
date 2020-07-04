using AuthRepository.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AuthMigrations
{
    public class AuthContextFactory : IDesignTimeDbContextFactory<AuthDbContext>
    {
        public AuthDbContext CreateDbContext(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            var builder = new DbContextOptionsBuilder<AuthDbContext>();
            builder.UseMySQL(configuration.GetConnectionString("AuthDb"), b=>b.MigrationsAssembly("AuthMigrations"));

            return new AuthDbContext(builder.Options);
        }
    }
}
