using InfoInkasService.Core.DataModels;
using InfoInkasService.Core.Interfaces;
using InfoInkasService.InfoInkasServiceAPI.Models.Services;
using InfoInkasService.OracleDB;
using InfoInkasServiceAPI.Models.Configuration;
using InfoInkasServiceAPI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.Elasticsearch;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace InfoInkasService.InfoInkasServiceAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

            services.AddSingleton<IBotService, BotService>();
            services.AddSingleton<ISpamControlService, SpamControlService>();
            services.AddSingleton<ILogger>(CreateLoggerConfiguration());
            services.AddScoped<IEmailService, EmailService>();
#if DEBUG
            services.AddScoped<IDBControl, /*DBImmitation*/OracleControl>();
            services.AddScoped<IUpdateService, /*FakeUpdate*/UpdateService>();
#else
            services.AddScoped<IDBControl, OracleControl>();
            services.AddScoped<IUpdateService, UpdateService>();
#endif
            //services.Configure<BotConfiguration>(Configuration);
            services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));
            services.Configure<OraConfig>(Configuration.GetSection("OraConfig"));
            //services.AddDbContext<OracleDbContext>(options =>
            //     options.UseOracle(Configuration.GetConnectionString("Zukus")));
            services
            .AddControllers()
            .AddNewtonsoftJson();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IBotService botService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private Logger CreateLoggerConfiguration()
        {
#if DEBUG
            return new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();
#else
            return new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(Configuration["ElasticConfiguration:Uri"]))
                    {
                        AutoRegisterTemplate = true,
                        IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name.ToLower()}-{DateTime.UtcNow:yyyy-MM}"
                    })
                    .Enrich.WithProperty("Environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"))
                    .ReadFrom.Configuration(Configuration).CreateLogger();
#endif
        }
    }
}