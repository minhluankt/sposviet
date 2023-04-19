using Application.DTOs.Settings;
using Application.Interfaces;
using Application.Interfaces.Shared;
using Infrastructure.Infrastructure.DbContexts;
using Infrastructure.Infrastructure.Identity.Models;
using Infrastructure.Infrastructure.Identity.Services;
using AspNetCoreHero.Results;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Infrastructure.Shared.Services;
using Application.Hepers;
using SystemVariable;
using Application.Interfaces.Repositories;
using Infrastructure.Infrastructure.Repositories;
using Infrastructure.Infrastructure.CacheRepositories;
using Application.Interfaces.CacheRepositories;
using Yanga.Module.EntityFrameworkCore.AuditTrail;
using Web.Api.Manager.Services;

namespace Web.Api.Manager.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddSharedInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IDateTimeService, SystemDateTimeService>();
            services.AddTransient<IMailService, SMTPMailService>();
            services.AddTransient<IAuthenticatedUserService, AuthenticatedUserService>();
        }

        public static void AddEssentials(this IServiceCollection services)
        {
            services.RegisterSwagger();
            services.AddVersioning();
        }

        private static void RegisterSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                //c.IncludeXmlComments(string.Format(@"{0}\Web.Api.Manager.xml", System.AppDomain.CurrentDomain.BaseDirectory));
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Web.Api.Manager",
                    //License = new OpenApiLicense()
                    //{
                    //    Name = "MIT License",
                    //    Url = new Uri("https://opensource.org/licenses/MIT")
                    //}
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Description = "Input your Bearer token in this format - Bearer {your token here} to access this API",
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer",
                            },
                            Scheme = "Bearer",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        }, new List<string>()
                    },
                });
            });
        }

        private static void AddVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;
            });
        }

        public static void AddContextInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
          var connetname = configuration.GetConnectionString("ApplicationConnection");
            services.AddDbContext<IdentityContext>(options => options.UseSqlServer(connetname));
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("IdentityConnection")));



            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.Password.RequireNonAlphanumeric = false;
            }).AddEntityFrameworkStores<IdentityContext>().AddDefaultUI().AddDefaultTokenProviders();

            //#region Services
            //services.AddTransient(typeof(IRepositoryAsync<>), typeof(RepositoryAsync<>));
            //services.AddTransient(typeof(IRepositoryCacheAsync<>), typeof(RepositoryCacheAsync<>));
            //services.AddTransient<AuditableContext, ApplicationDbContext>();
            //services.AddTransient<IIdentityService, IdentityService>();



            //services.AddTransient<IEmailHistoryRepository<Domain.Entities.Mailhistory>, EmailHistoryRepository>();
            //services.AddTransient<IEmailRepository<Domain.Entities.MailSettings>, EmailRepository>();
            //services.AddTransient<IDbFactory, DbFactory>();
            //services.AddTransient<IDapperRepository, DapperBaseRepository>();
            //services.AddTransient<ILogRepository, LogRepository>();
            //services.AddTransient<IUnitOfWork, UnitOfWork>();
            //#endregion Services
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
           
            services.Configure<JWTSettings>(configuration.GetSection("JWTSettings"));
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(o =>
                {
                    o.RequireHttpsMetadata = false;
                    o.SaveToken = false;
                    o.TokenValidationParameters = tokenValidationParameters;
                    o.Events = new JwtBearerEvents()
                    {
                        OnAuthenticationFailed = c =>
                        {
                            c.NoResult();
                            c.Response.StatusCode = 500;
                            c.Response.ContentType = "text/plain";
                            return c.Response.WriteAsync(c.Exception.ToString());
                        },
                        OnChallenge = context =>
                        {
                            context.HandleResponse();
                            context.Response.StatusCode = 401;
                            context.Response.ContentType = "application/json";
                            var result = JsonConvert.SerializeObject(Result.Fail("You are not Authorized"));
                            return context.Response.WriteAsync(result);
                        },
                        OnForbidden = context =>
                        {
                            context.Response.StatusCode = 403;
                            context.Response.ContentType = "application/json";
                            var result = JsonConvert.SerializeObject(Result.Fail("Bạn không có quyền truy cập"));
                            return context.Response.WriteAsync(result);
                        },
                    };
                });
            services.AddSingleton(tokenValidationParameters);
        }
    }
}
