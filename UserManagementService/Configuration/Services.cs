using Microsoft.AspNetCore.Identity;
using UserManagementService.Models.Account;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UserManagementService.JwtFeatures;
using UserManagementService.Security;
using M2Store.Common.Extensions;
using UserManagementService.Repositories;
using UserManagementService.Services;

namespace UserManagementService.Configuration
{
    public static class Configruation
    {
        public static WebApplicationBuilder RegisterLocalServices(this WebApplicationBuilder builder)
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
            // Add session services

            builder.Services.AddDistributedMemoryCache();
            builder.Services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
                //options.HttpOnly = true;
            });
            builder.Services.AddSession(options =>
            {
                options.Cookie.Name = "M2Store"; // Set the name of the session cookie
                options.Cookie.Path = "/"; // Set the path for which the session cookie is valid
                options.Cookie.HttpOnly = true; // Set to true to prevent client-side JavaScript from accessing the cookie
                options.Cookie.SameSite = SameSiteMode.Lax; // Set the SameSite attribute of the cookie
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // Set the Secure attribute of the cookie
                options.Cookie.IsEssential = true; // Set to true if the session cookie is essential for your application's functionality

                options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout (optional)
            });

            builder.Services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddConsole();
            });


            builder.RegisterMySql<ApplicationDbContext, Program>();

            //builder.Services.AddScoped<IRoleStore<IdentityRole>, ApplicationRoleStore>();
            //builder.Services.AddScoped<IUserStore<ApplicationUser>, ApplicationUserStore>();
            //builder.Services.AddScoped<IRoleClaimStore<IdentityRole>, ApplicationRoleStore>();
            //builder.Services.AddScoped(RoleManager<>);

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Configure identity options if needed
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.DefaultLockoutTimeSpan = new TimeSpan(0, 1, 0); //  TimeSpan.FromMinutes(5); // 30 minutes lockout duration
                options.Lockout.MaxFailedAccessAttempts = 3; // Maximum failed access attempts before lockout
                options.SignIn.RequireConfirmedEmail = true;

                options.Tokens.EmailConfirmationTokenProvider = "CustomEmailConfirmation";
                //options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;

            })
             .AddEntityFrameworkStores<ApplicationDbContext>()
             .AddDefaultTokenProviders()
             .AddTokenProvider<CustomEmailConfirmationTokenProvider<ApplicationUser>>("CustomEmailConfirmation");

            builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromHours(6);
            });

            builder.Services.Configure<CustomEmailConfirmationTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromDays(3);
            });

            //.AddTokenProvider("M2Store");//, typeof(DataProtectorTokenProvider<User>)


            //services.AddIdentity<User, UserRole>()
            //.AddEntityFrameworkStores<ApplicationDbContext>()
            //.AddDefaultTokenProviders();



            //builder.Services.AddIdentityServer();
            //    .AddInMemoryApiResources(Config.GetApiResources())
            //    .AddInMemoryClients(Config.GetClients())
            //    .AddInMemoryIdentityResources(Config.GetIdentityResources())
            //    .AddTestUsers(Config.GetTestUsers()) // You can replace this with your user store
            //.AddAspNetIdentity<ApplicationUser>();

            //builder.Services.AddDeveloperSigningCredential();

            AddAuthentication(builder);
            
            builder.RegisterMassTransitWithRabbitMQ();
            builder.Services.AddScoped<ApplicationJwtHandler>();
            builder.Services.AddScoped<UserManager<ApplicationUser>, ApplicationUserManager>();
            builder.Services.AddScoped<JwtService>();
            builder.Services.AddScoped<IEmailRepository, EmailService>();

            return builder;

        }

        private static bool CustomLifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters)
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
            var jwtSettings = builder.Configuration.GetSection(nameof(JWTSettings)).Get<JWTSettings>();
            var key = Encoding.UTF8.GetBytes(jwtSettings!.SecretKey!);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddGoogle(options =>
            {
                options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;// "866120810220-af0aelcr6euuh4tuem5df0637qr3rrn5.apps.googleusercontent.com";
                options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!; //  "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";
                options.SignInScheme = IdentityConstants.ExternalScheme;
            });
            //.AddJwtBearer(options =>
            //{
            //    options.RequireHttpsMetadata = false; // Set to true in production
            //    options.SaveToken = true;
            //    options.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateIssuerSigningKey = false,
            //        IssuerSigningKey = new SymmetricSecurityKey(key),
            //        ValidateIssuer = false,
            //        ValidIssuer = jwtSettings.Issuer, // _configuration["JWTSettings:Issuer"],
            //        ValidateAudience = false,
            //        ValidAudience = jwtSettings.Audience, //  _configuration["JWTSettings:Audience"],
            //        ValidateLifetime = false,
            //        LifetimeValidator = CustomLifetimeValidator,
            //    };
            //});
        }
    }
}
