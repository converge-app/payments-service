using System;
using Application.Database;
using Application.Helpers;
using Application.Repositories;
using Application.Services;
using Application.Utility.ClientLibrary;
using Application.Utility.Database;
using Application.Utility.Middleware;
using Application.Utility.Models;
using Application.Utility.Startup;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Prometheus;

namespace Application
{
    public class Startup
    {
        public Startup(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            Configuration = configuration;
            Logging.CreateLogger();
        }

        private readonly ILoggerFactory _loggerFactory;

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Set compability mode for mvc
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });

            services.AddSingleton<IDatabaseSettings, DatabaseSettings>();
            services.AddTransient<IDatabaseContext, DatabaseContext>();
            services.AddMongoDb();
            services.AddAutoMapper(typeof(AutoMapperProfile));

            services.AddHttpClient();
            services.AddTransient<IClient, Client>();

            services.AddMultipleDomainSupport();

            var appSettings = Configuration.GetSection("AppSettings").Get<AppSettings>();
            services.AddTokenValidation(appSettings.Secret);
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IAccountsRepository, AccountsRepository>();
            services.AddScoped<IPaymentservice, Paymentservice>();
            services.AddTracing(options =>
            {
                options.JaegerAgentHost = Environment.GetEnvironmentVariable("JAEGER_AGENT_HOST");
                options.ServiceName = "payments-service";
                options.LoggerFactory = _loggerFactory;
            });

            services.AddApiDocumentation("Payments");

            services.AddHealthChecks();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
            app.UseHttpsRedirection();

            loggerFactory.AddLogging();

            app.UseMultipleDomainSupport();
            app.UseHealthChecks("/api/health");
            app.UseMetricServer();
            app.UseRequestMiddleware();

            app.UseAuthentication();
            app.UseApiDocumentation("Payments");

            app.UseMvc();
        }
    }
}