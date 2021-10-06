using System;
using System.Reflection;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using rapide_shortener_service.Model;
using rapide_shortener_service.Services;
using Prometheus;
namespace rapide_shortener_service
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
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

            services.AddSingleton<Controller.ShortenerGrpcController>();
            services.AddSingleton<Services.ShortenerService>();
            services.AddGrpc();
            services.AddGrpcReflection();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            Console.WriteLine($"URLDatabaseSettings:ConnectionString : {Configuration["URLDatabaseSettings:ConnectionString"]}");
            foreach (DictionaryEntry e in System.Environment.GetEnvironmentVariables())
            {
                Console.WriteLine(e.Key + ":" + e.Value);
            }

            app.UseRouting();
            app.UseGrpcMetrics();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<Controller.ShortenerGrpcController>();
                endpoints.MapGrpcReflectionService();
            });
        }
    }
}
