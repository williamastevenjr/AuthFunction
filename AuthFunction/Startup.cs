using System;
using AuthRepository.Context;
using AuthRepository.Interfaces;
using AuthService.Interfaces;
using EFCore.DbContextFactory.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Ws.EfCore.Extensions.TypeMappingExtensions;
using Ws.JwtAuth.Extensions;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Thinktecture;
using Ws.EfCore.Extensions.AppSettings;

namespace AuthFunction
{
    public class Startup
    {
        public const string AppS3BucketKey = "AppS3Bucket";

        private const string SwaggerDocumentVersionName = "v1";
        private static string SwaggerDocumentServiceName => $"Auth API({SwaggerDocumentVersionName})";
        private readonly MySqlAppSettings _mySqlAppSettings;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _mySqlAppSettings = new MySqlAppSettings(configuration.GetSection("MySql"));
        }

        public static IConfiguration Configuration { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddNewtonsoftJson();

            // Add S3 to the ASP.NET Core dependency injection framework.
            services.AddAWSService<Amazon.S3.IAmazonS3>();

            //swagger
            //services.AddSwaggerGen(c =>
            //c.SwaggerDoc("v1", new OpenApiInfo { Title = "Auth Functions", Version = "v1" }));
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(
                      SwaggerDocumentVersionName,
                            new OpenApiInfo
                            {
                                Title = SwaggerDocumentServiceName,
                                Version = $"{SwaggerDocumentVersionName}"
                            });
            });

            //auth
            services.ConfigureJwtAuth(Configuration);

            //service
            services.AddTransient<IAuthService, AuthService.Services.AuthService>();

            //repo
            services.AddTransient<IAuthRepository, AuthRepository.Implementations.AuthRepository>();
            services.AddDbContext<AuthDbContext>(builder =>
                builder.UseMySql(Configuration.GetConnectionString("AuthDb"), mySqlOptions => mySqlOptions
                    // replace with your Server Version and Type
                    .ServerVersion(
                        new Version(_mySqlAppSettings.Major, _mySqlAppSettings.Minor, _mySqlAppSettings.Build),
                        ServerType.MySql)));
                    //.AddRelationalTypeMappingSourcePlugin<MiniGuidTypeMappingPlugin>());
            services.AddDbContextFactory<AuthDbContext>(builder=> 
                builder.UseMySql(Configuration.GetConnectionString("AuthDb"), mySqlOptions => mySqlOptions
                        // replace with your Server Version and Type
                        .ServerVersion(new Version(_mySqlAppSettings.Major, _mySqlAppSettings.Minor, _mySqlAppSettings.Build), ServerType.MySql)));
                    //.AddRelationalTypeMappingSourcePlugin<MiniGuidTypeMappingPlugin>());

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // global cors policy
            //app.UseCors(x => x
            //    .AllowAnyOrigin()
            //    .AllowAnyMethod()
            //    .AllowAnyHeader());

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //use swagger
            var API_PREFIX = "test";//todo
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"{(env.IsDevelopment() ? "" : API_PREFIX)}/swagger/{SwaggerDocumentVersionName}/swagger.json", SwaggerDocumentServiceName);
                //c.SwaggerEndpoint(Configuration.GetSection("Swagger")["url"] + "/swagger/v1/swagger.json", "AWS Serverless Asp.Net Core Web API");
                c.RoutePrefix = "swagger/ui";
            });

        }
    }
}
