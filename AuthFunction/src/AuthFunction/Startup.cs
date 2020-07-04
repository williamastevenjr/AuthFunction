﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace AuthFunction
{
    public class Startup
    {
        public const string AppS3BucketKey = "AppS3Bucket";

        private const string SwaggerDocumentVersionName = "v1";
        private static string SwaggerDocumentServiceName => $"Auth API({SwaggerDocumentVersionName})";


        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static IConfiguration Configuration { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

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
