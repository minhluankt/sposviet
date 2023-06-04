using Application.DTOs.Settings;
using Application.Interfaces.Shared;
using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.SqlServer;
using Infrastructure.Infrastructure.DbContexts;
using Domain.Identity;
using Infrastructure.Shared.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Text;
using Web.ManagerCompany.Service;

namespace Web.ManagerCompany.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static void AddMultiLingualSupport(this IServiceCollection services)
        {



            services.AddMvc().AddMvcOptions(Mvcoption =>
            {
                Mvcoption.EnableEndpointRouting = false;
            });

            // services.AddPaging();
            services.AddRouting(o => o.LowercaseUrls = true);
            services.AddHttpContextAccessor();
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var cultures = new List<CultureInfo> {
        new CultureInfo("en"),
         new CultureInfo("vi")
                };
                options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("vi");
                options.SupportedCultures = cultures;
                options.SupportedUICultures = cultures;
                options.SetDefaultCulture("vi");
            });

        }

        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {


            services.AddPersistenceContexts(configuration);
            //  services.AddAuthenticationScheme(configuration); // tắt login
        }

        private static void AddAuthenticationScheme(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMvc(o =>
            {
                //Add Authentication to all Controllers by default.
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                o.Filters.Add(new AuthorizeFilter(policy));

                // o.Conventions.Add(new MyAuthorizeFiltersControllerConvention());
            });
        }


        private static void AddPersistenceContexts(this IServiceCollection services, IConfiguration configuration)
        {


            var connetname = configuration.GetConnectionString("ApplicationConnection");
            services.AddDbContext<IdentityContext>(options => options.UseSqlServer(connetname));
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("IdentityConnection")));

            services.AddHangfire(x => x
           .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
           .UseSimpleAssemblyNameTypeSerializer()
           .UseRecommendedSerializerSettings()
           //.UseDefaultTypeSerializer()
           .UseMemoryStorage()
           .UseSqlServerStorage(connetname, new SqlServerStorageOptions
           {
               CommandBatchMaxTimeout = TimeSpan.FromMinutes(60),
               SlidingInvisibilityTimeout = TimeSpan.FromMinutes(60),
               QueuePollInterval = TimeSpan.Zero,
               UseRecommendedIsolationLevel = true,
               DisableGlobalLocks = true
           }));
            services.AddHangfireServer();

            //thêm identity
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                //options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedAccount = true;
                options.Password.RequireNonAlphanumeric = false;
            }).AddEntityFrameworkStores<IdentityContext>().AddDefaultUI().AddDefaultTokenProviders();
            // cái này cần
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                ValidIssuer = configuration["JWTSettings:Issuer"],
                ValidAudience = configuration["JWTSettings:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWTSettings:Key"]))
            };

            services.AddSingleton(tokenValidationParameters);
        }

        public static void AddSharedInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            //services.Configure<JiraSetting>(configuration.GetSection("JiraSetting"));

            services.Configure<CacheSettings>(configuration.GetSection("CacheSettings"));
            services.AddTransient<IDateTimeService, SystemDateTimeService>();
            services.AddTransient<IMailService, SMTPMailService>();
            services.AddTransient<IAuthenticatedUserService, AuthenticatedUserService>();
        }


    }
}
