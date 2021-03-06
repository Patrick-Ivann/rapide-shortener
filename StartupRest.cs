﻿using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using rapide_shortener_service.Model;
namespace rapide_shortener_service
{
    public class StartupRest
    {
        public StartupRest(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<URLDatabaseSettings>(
               Configuration.GetSection(nameof(URLDatabaseSettings)));

            services.AddSingleton<IURLDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<URLDatabaseSettings>>().Value);

            services.AddSingleton<Services.ShortenerService>();
            services.AddApiVersioning();
            services.AddControllers().AddNewtonsoftJson(options => options.UseMemberCasing());

            services.AddHealthChecks();
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {

                    Version = "v1",
                    Title = "Rapide-shortener",
                    Description = "Rest api Rapide-shortener",
                    Contact = new OpenApiContact
                    {
                        Name = "Origo patrick-ivann",
                        Email = string.Empty,
                    },

                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger(c =>
                {
                    c.SerializeAsV2 = true;
                });

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {

                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
                endpoints.MapSwagger();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }
    }
}
