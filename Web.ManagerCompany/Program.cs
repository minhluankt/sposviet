using Application.Extensions;
using AspNetCoreHero.Extensions.Logging;
using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using FluentValidation.AspNetCore;
using Hangfire;
using Infrastructure.Infrastructure.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;
using System.Text;
using SystemVariable;
using Web.ManagerCompany;
using Web.ManagerCompany.Abstractions;
using Web.ManagerCompany.Extensions;
using Web.ManagerCompany.Permission;
using Web.ManagerCompany.Service;
using WebEssentials.AspNetCore.Pwa;

var builder = WebApplication.CreateBuilder(args);

var mvcBuilder = builder.Services.AddRazorPages();
if (builder.Environment.IsDevelopment())
{
    mvcBuilder.AddRazorRuntimeCompilation();
}



builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddApplicationLayer();
builder.Services.AddHttpContextAccessor();

builder.Services.AddRepositories();
builder.Services.AddMultiLingualSupport(); //song ngữ
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.ConfigsAppsettings(builder.Configuration);
builder.Services.AddSharedInfrastructure(builder.Configuration);



builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTransient<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddScoped<IViewRenderService, ViewRenderService>();

builder.Services.AddControllersWithViews().AddFluentValidation(fv =>
{
    fv.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
#pragma warning disable CS0618 // Type or member is obsolete
    fv.RunDefaultMvcValidationAfterFluentValidationExecutes = true;
#pragma warning restore CS0618 // Type or member is obsolete
});
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddDistributedMemoryCache();
builder.Services.AddProgressiveWebApp(new PwaOptions { RegisterServiceWorker = true });
builder.Services.AddNotyf(o =>
{
    o.DurationInSeconds = 10;
    o.IsDismissable = true;
    o.HasRippleEffect = true;
    o.Position = NotyfPosition.TopRight;
});

builder.Services.Configure<IdentityOptions>(options =>
{
    // Thiết lập về Password
    options.Password.RequireDigit = false; // Không bắt phải có số
    options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
    options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
    options.Password.RequireUppercase = false; // Không bắt buộc chữ in
    options.Password.RequiredLength = 5; // Số ký tự tối thiểu của password
    options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

    // Cấu hình Lockout - khóa user
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(100); // Khóa 1 phút
    options.Lockout.MaxFailedAccessAttempts = 5; // Thất bại 5 lầ thì khóa
    options.Lockout.AllowedForNewUsers = true;

    // Cấu hình về User.
    options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._";
    // options.User.RequireUniqueEmail = true; // Email là duy nhất

    // Cấu hình đăng nhập.
    //   options.SignIn.RequireConfirmedEmail = true; // Cấu hình xác thực địa chỉ email (email phải tồn tại)
    // options.SignIn.RequireConfirmedPhoneNumber = false; // Xác thực số điện thoại

});


// Add services to the container.
builder.Services.AddAuthentication()  // login lần 2 theo bên trên
    .AddCookie(options =>
    {
        options.AccessDeniedPath = new PathString("/identity/account/accessdenied");
        //options.cookie.name = cookieauthenticationdefaults.authenticationscheme;
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(6);
        // options.loginpath = new pathstring("/identity/account/login");
        //  options.logoutpath = new pathstring("/identity/account/logout");
        options.SlidingExpiration = true;
    });
builder.Host.UseSerilog();

var app = builder.Build();
// Configure the HTTP request pipeline.
var lifetime = app.Services.GetService<Microsoft.AspNetCore.Hosting.IApplicationLifetime>();
var distCache = app.Services.GetService<IDistributedCache>();

var distCacheOptions = new DistributedCacheEntryOptions()
                      .SetSlidingExpiration(TimeSpan.FromDays(100));

// Session State distributed cache configuration.
lifetime.ApplicationStarted.Register(() =>
{
    var currentTimeUTC = DateTime.Now.ToString();
    byte[] encodedCurrentTimeUTC = Encoding.UTF8.GetBytes(currentTimeUTC);
    distCache.Set("cachedTimeUTC", encodedCurrentTimeUTC, distCacheOptions);
});

// cấu hình Mediator, log
BuildMediator();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
    var logger = loggerFactory.CreateLogger("app");
    try
    {

        //await Infrastructure.Infrastructure.Identity.Seeds.DefaultBasicUser.SeedAsync(userManager, roleManager);
        logger.LogInformation("Application Starting");
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "An error occurred seeding the DB");
    }
}
static IMediator BuildMediator()
{
    var services = new ServiceCollection();

    services.AddLogging(options => options.AddConsole());

    services.AddMediatR(typeof(Program));
    services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

    var provider = services.BuildServiceProvider();

    return provider.GetRequiredService<IMediator>();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseNotyf();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCookiePolicy();
app.UseMultiLingualFeature();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseHangfireDashboard("/HangfireDashboard");
app.UseEndpoints(endpoints =>
  {
      endpoints.MapRazorPages();
      endpoints.MapControllerRoute(
  name: "default",
  pattern: "{controller=Home}/{action=Index}/{id?}");


  });
app.Run();
