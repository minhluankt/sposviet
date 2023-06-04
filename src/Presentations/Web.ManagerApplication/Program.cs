using Application.Constants;
using Application.Extensions;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Extensions.Logging;
using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using DeviceDetectorNET.Parser.Device;
using Domain.Entities;
using FluentValidation.AspNetCore;
using Hangfire;
using Infrastructure.Infrastructure.Extensions;
using Infrastructure.Infrastructure.HubS;
using Domain.Identity;
using Infrastructure.Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Globalization;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using SystemVariable;
using Web.ManagerApplication;
using Web.ManagerApplication.Abstractions;
using Web.ManagerApplication.Extensions;
using Web.ManagerApplication.Permission;
using Web.ManagerApplication.Service;
using WebEssentials.AspNetCore.Pwa;

using static System.Net.Mime.MediaTypeNames;

var builder = WebApplication.CreateBuilder(args);

var mvcBuilder = builder.Services.AddRazorPages();
if (builder.Environment.IsDevelopment())
{
    mvcBuilder.AddRazorRuntimeCompilation();
}
var connectionString = builder.Configuration.GetConnectionString("WebManagerApplicationContextConnection");

//builder.Services.AddDbContext<WebManagerApplicationContext>(options =>
//    options.UseSqlServer(connectionString));
//builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<WebManagerApplicationContext>();


/// <summary>
/// service
/// </summary>
/// 

// Add services to the container.


//builder.Services.AddIdentityManually();


builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTransient<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddScoped<IViewRenderService, ViewRenderService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddApplicationLayer();
builder.Services.AddRepositories();
builder.Services.AddMultiLingualSupport(); //song ngữ



builder.Services.AddControllersWithViews().AddFluentValidation(fv =>
{
    fv.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
#pragma warning disable CS0618 // Type or member is obsolete
    fv.RunDefaultMvcValidationAfterFluentValidationExecutes = true;
#pragma warning restore CS0618 // Type or member is obsolete
});
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddMemoryCache();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddProgressiveWebApp(new PwaOptions { RegisterServiceWorker = true });
builder.Services.AddNotyf(o =>
{
    o.DurationInSeconds = 10;
    o.IsDismissable = true;
    o.HasRippleEffect = true;
    o.Position = NotyfPosition.TopRight;
});
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "en-US", "fr" };
    options.SetDefaultCulture(supportedCultures[0])
        .AddSupportedCultures(supportedCultures)
        .AddSupportedUICultures(supportedCultures);
}); 
//builder.Services.AddSignalR()
//    .AddJsonProtocol(options => {
//        options.PayloadSerializerOptions.PropertyNamingPolicy = null;
//      //  options.
//    });

builder.Services.AddSignalR(hubOptions =>
{
    hubOptions.EnableDetailedErrors = true;
    hubOptions.ClientTimeoutInterval = TimeSpan.FromHours(15);
    hubOptions.KeepAliveInterval = TimeSpan.FromMilliseconds(5);

}).AddJsonProtocol(options =>
{
    options.PayloadSerializerOptions.PropertyNamingPolicy = null;
    //  options.
});
//builder.Services.AddBreadcrumbs(GetType().Assembly, options =>
//{
//    options.TagName = "nav";
//    options.TagClasses = "";
//    options.OlClasses = "breadcrumb";
//    options.LiClasses = "breadcrumb-item";
//    options.ActiveLiClasses = "breadcrumb-item active";
//    //options.SeparatorElement = "<li class=\"separator\">/</li>";
//});

builder.Services.Configure<IdentityOptions>(options =>
{
    // Thiết lập về Password
    options.Password.RequireDigit = false; // Không bắt phải có số
    options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
    //options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
    options.Password.RequireUppercase = false; // Không bắt buộc chữ in
    options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
    //options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

    // Cấu hình Lockout - khóa user
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(100); // Khóa 1 phút
    options.Lockout.MaxFailedAccessAttempts = 5; // Thất bại 5 lầ thì khóa
    options.Lockout.AllowedForNewUsers = true;

    // Cấu hình về User.
    options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true; // Email là duy nhất

    // Cấu hình đăng nhập.
    //   options.SignIn.RequireConfirmedEmail = true; // Cấu hình xác thực địa chỉ email (email phải tồn tại)
    // options.SignIn.RequireConfirmedPhoneNumber = false; // Xác thực số điện thoại

});





// add service in Configuration

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.ConfigsAppsettings(builder.Configuration);
builder.Services.AddSharedInfrastructure(builder.Configuration);



// add login 2 loại
builder.Services.AddAuthentication(CookieAuthenticationCustomer.AuthenticationScheme)
            .AddCookie(CookieAuthenticationCustomer.AuthenticationScheme, options =>
            {
                options.LoginPath = new PathString("/Account/Login");
                options.LogoutPath = new PathString("/Account/Logout");
                options.AccessDeniedPath = new PathString("/Account/AccessDenied");
                options.Cookie.HttpOnly = true;
                // options.ReturnUrlParameter = CookieAuthenticationCustomer.ReturnUrlParameter;
                options.ExpireTimeSpan = TimeSpan.FromDays(300);
                options.SlidingExpiration = true;
            });
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)  // login lần 2 theo bên trên
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
     {
         options.AccessDeniedPath = new PathString("/Identity/Account/AccessDenied");
         //options.Cookie.Name = CookieAuthenticationDefaults.AuthenticationScheme;
         options.Cookie.HttpOnly = true;
         options.ExpireTimeSpan = TimeSpan.FromDays(2);
         options.LoginPath = new PathString("/Identity/Account/Login");
         options.LogoutPath = new PathString("/Identity/Account/Logout");
         options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
         options.SlidingExpiration = true;
     });

builder.Services.AddAuthentication()
            .AddGoogle(googleOptions =>
            {
                //  googleOptions.SignInScheme = GoogleDefaults.AuthenticationScheme;
                // Đọc thông tin Authentication:Google từ appsettings.json
                IConfigurationSection googleAuthNSection = builder.Configuration.GetSection("Authentication:Google");

                // Thiết lập ClientID và ClientSecret để truy cập API google
                googleOptions.ClientId = googleAuthNSection["ClientId"];
                googleOptions.ClientSecret = googleAuthNSection["ClientSecret"];
                googleOptions.Scope.Add("email");
                // Cấu hình Url callback lại từ Google (không thiết lập thì mặc định là /signin-google)
                //googleOptions.CallbackPath = "/ExternalLoginCallback";

            })
            .AddFacebook(facebookOptions =>
            {
                // Đọc cấu hình
                IConfigurationSection facebookAuthNSection = builder.Configuration.GetSection("Authentication:Facebook");
                facebookOptions.AppId = facebookAuthNSection["AppId"];
                facebookOptions.AppSecret = facebookAuthNSection["AppSecret"];
                // Thiết lập đường dẫn Facebook chuyển hướng đến
                facebookOptions.Scope.Add("public_profile");
                facebookOptions.Scope.Add("email");
                // facebookOptions.SignInScheme = FacebookDefaults.AuthenticationScheme;

                facebookOptions.Fields.Add("email");
                facebookOptions.Fields.Add("picture");
                facebookOptions.Fields.Add("gender");
                facebookOptions.Fields.Add("name");
                facebookOptions.Fields.Add("id");
                facebookOptions.Fields.Add("birthday");
                //  facebookOptions.CallbackPath = "/ExternalLoginCallback";
                facebookOptions.AccessDeniedPath = "/AccessDeniedPathInfo";
                facebookOptions.SaveTokens = true;


            });
// serilog
builder.Host.UseSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline.
var lifetime = app.Services.GetService<Microsoft.AspNetCore.Hosting.IApplicationLifetime>();
var distCache = app.Services.GetService<IDistributedCache>();


//var backgroundJobs = app.Services.GetService<IBackgroundJobClient>();

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
    var recurringJobManager = scope.ServiceProvider.GetService<IRecurringJobManager>();
    var serviceProvider = scope.ServiceProvider.GetService<IServiceProvider>();

    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
    var logger = loggerFactory.CreateLogger("app");
    try
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();

        await Infrastructure.Infrastructure.Identity.Seeds.DefaultRoles.SeedAsync(userManager, roleManager);
        await Infrastructure.Infrastructure.Identity.Seeds.DefaultSuperAdminUser.SeedAsync(userManager, roleManager);
        //await Infrastructure.Infrastructure.Identity.Seeds.DefaultBasicUser.SeedAsync(userManager, roleManager);
        logger.LogInformation("Application Starting");
        recurringJobManager.AddOrUpdate("serilog", () => serviceProvider.GetService<ILogRepository>().JobDeleteSerilog(SystemVariableHelper.folderLog), Cron.Daily(23), TimeZoneInfo.Local);
        recurringJobManager.AddOrUpdate("clearAuditLog", () => serviceProvider.GetService<ILogRepository>().DeleteAuditLogAsync(), Cron.Daily(00,13), TimeZoneInfo.Local);
        recurringJobManager.AddOrUpdate("deleteinvoice", () => serviceProvider.GetService<IInvoicePepository<Invoice>>().JobDeleteInvoiceAsync(), Cron.Daily(24), TimeZoneInfo.Local);

    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "An error occurred seeding the DB");
    }
}



//

static IMediator BuildMediator()
{
    var services = new ServiceCollection();

    services.AddLogging(options => options.AddConsole());

    services.AddMediatR(typeof(Program));
    services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

    var provider = services.BuildServiceProvider();

    return provider.GetRequiredService<IMediator>();
}




if (!app.Environment.IsDevelopment())
{

    // app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseExceptionHandler(exceptionHandlerApp =>
    {
        exceptionHandlerApp.Run(async context =>
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            // using static System.Net.Mime.MediaTypeNames;
            context.Response.ContentType = Text.Plain;

            await context.Response.WriteAsync("An exception was thrown.");

            var exceptionHandlerPathFeature =
                context.Features.Get<IExceptionHandlerPathFeature>();

            if (exceptionHandlerPathFeature?.Error is FileNotFoundException)
            {
                await context.Response.WriteAsync(" The file was not found.");
            }

            if (exceptionHandlerPathFeature?.Path == "/")
            {
                await context.Response.WriteAsync(" Page: Home.");
            }
        });
    });
    app.UseHsts();
}
app.Use(async (ctx, next) =>


{
    var principla = new ClaimsPrincipal();
    var result1 = await ctx.AuthenticateAsync(CookieAuthenticationCustomer.AuthenticationScheme);
    if (result1?.Principal != null)
    {
        principla.AddIdentities(result1.Principal.Identities);
    }

    var result2 = await ctx.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    if (result2?.Principal != null)
    {
        principla.AddIdentities(result2.Principal.Identities);
    }

    ctx.User = principla;

    await next();
});





app.UseNotyf();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseMultiLingualFeature();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseHangfireDashboard("/HangfireDashboard", new DashboardOptions
{
    Authorization = new[] { new HangfireAuthorizationFilter() }
});

app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();// có cái này mới trỏ đúng view
    endpoints.MapControllers();

    endpoints.MapHub<SignalRHub>("/Signal", options =>
    {
        options.Transports =
            HttpTransportType.WebSockets |
            HttpTransportType.LongPolling;
        options.ApplicationMaxBufferSize = 1048576;//1MB
    });
    endpoints.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );
    // phải để dòng này lên trên vì nếu không sẽ không ăn link view asp-area="Admin"
    endpoints.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


    // endpoints.MapHangfireDashboard();


});

// app.UseSqlTableDependency<SubscribeProductTableDependency>(builder.Configuration);sử dụng realtime DB
RoutingConfig.Include(app);
app.Run();
