using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MySql.Data.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

//using Microsoft.EntityFrameworkCore;
//using MySqlConnector;
using Public_API.Controllers;
using Public_API.Models.Account;
using Microsoft.AspNetCore.Authentication.Cookies;
using static Org.BouncyCastle.Math.EC.ECCurve;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Public_API.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;

namespace Public_API.Configuration
{
    public static class Configruation
    {
        public static void RegisterServices(this WebApplicationBuilder builder)
        {

            // Add configuration to the builder
            //builder.Configuration.AddJsonFile("appsettings.json");

            //.UseSerilog((context, configuration) =>
            // {
            //     configuration
            //         .MinimumLevel.Debug()
            //         .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            //         .MinimumLevel.Override("System", LogEventLevel.Warning)
            //         .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
            //         .Enrich.FromLogContext()
            //         .WriteTo.Console(
            //             outputTemplate:
            //             "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}",
            //             theme: AnsiConsoleTheme.Literate);
            // });


            builder.Services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddConsole();
            });

            var serviceProvider = builder.Services.BuildServiceProvider();
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();



            var env = builder.Environment.EnvironmentName;
            //var env = builder.Configuration.GetValue<string>(WebHostDefaults.EnvironmentKey);
            logger.LogInformation($"Environment to use is: {env}");

            builder.Configuration
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            ;

            builder.Services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            });


            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                var connectionstring = Environment.GetEnvironmentVariable("MYSQL")!;
                logger.LogInformation("Reading environment variable");
                logger.LogInformation(connectionstring);

                if (string.IsNullOrEmpty(connectionstring))
                {
                    connectionstring = builder.Configuration.GetConnectionString("DefaultConnection")!;
                    logger.LogInformation($"Connection string from json to use: {connectionstring}!");
                }

                else
                {
                    logger.LogInformation($"Connection string from Environment to use: {connectionstring}!");
                }
                options.UseMySql(connectionstring, ServerVersion.AutoDetect(connectionstring));
            });

            builder.Services.AddScoped<IRoleStore<IdentityRole>, ApplicationRoleStore>();
            builder.Services.AddScoped<IRoleClaimStore<IdentityRole>, ApplicationRoleStore>();
            builder.Services.AddScoped<IUserStore<ApplicationUser>, ApplicationUserStore>();

            //builder.Services.AddScoped(RoleManager<>);

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Configure identity options if needed
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 6;
                //options.Password.RequireLowercase = true;
                //options.Password.RequireUppercase = true;
                //options.Password.RequireNonAlphanumeric = true;
            })
                //.AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            //builder.Services.AddIdentityServer()
            //    .AddInMemoryApiResources(Config.GetApiResources())
            //    .AddInMemoryClients(Config.GetClients())
            //    .AddInMemoryIdentityResources(Config.GetIdentityResources())
            //    .AddTestUsers(Config.GetTestUsers()) // You can replace this with your user store
            //    .AddAspNetIdentity<ApplicationUser>();

            //builder.Services.AddDeveloperSigningCredential();

            AddAuthentication(builder);

            builder.Services.AddCors(options =>
            {

                var origins = builder.Configuration.GetSection("Cors:Origin").Get<string[]>();

                options.AddPolicy("Application", builder =>
                {
                    builder.WithOrigins(origins!)
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            builder.Services.AddControllers();

            builder.WebHost.UseKestrel(options =>
             {
                 options.Limits.MaxRequestBodySize = 52428800; //50MB
             });

            builder.Services.AddScoped<ApplicationJwtHandler>();
            builder.Services.AddScoped<UserManager<ApplicationUser>, ApplicationUserManager>();
            builder.Services.AddScoped<ArtService>();
        }

        private static  bool CustomLifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters)
        {
            // Check if the token is expired (expires must be greater than the current UTC time)
            //return expires.HasValue && expires.Value > DateTime.UtcNow;
            if (expires.HasValue)
            {
                var utcNow = DateTime.UtcNow;
                var expirationTimeUtc = expires.Value.ToUniversalTime(); // Convert expires to UTC
                return expirationTimeUtc > utcNow;
            }

            return false;
        }
        private static void AddAuthentication(WebApplicationBuilder builder)
        {
            var _configuration = builder.Configuration;
            var key = Encoding.UTF8.GetBytes(_configuration["JWTSettings:SecretKey"]!);
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; // Set to true in production
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["JWTSettings:Issuer"],
                    ValidateAudience = false,
                    ValidAudience = _configuration["JWTSettings:Audience"],
                    ValidateLifetime = true,
                    LifetimeValidator = CustomLifetimeValidator,
                };
            });
        }
    }
}
