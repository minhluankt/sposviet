using ApiHttpClient.Extensions;
using Application.Extensions;
using AspNetCoreHero.Extensions.Logging;
using Infrastructure.Infrastructure.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using SystemVariable;
using Web.Api.Manager;
using Web.Api.Manager.Extensions;
using Web.Api.Manager.Filter;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApplicationLayer();
builder.Services.AddContextInfrastructure(builder.Configuration);
builder.Services.AddPersistenceContexts(builder.Configuration);
builder.Services.ConfigsAppsettings(builder.Configuration);
builder.Services.AddSharedInfrastructure(builder.Configuration);
builder.Services.AddRepositories();
builder.Services.AddEssentials();

//builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(ServiceExceptionFilters));
});
builder.Services.AddMvc(o =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    o.Filters.Add(new AuthorizeFilter(policy));
});
builder.Host.UseSerilog();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
   ///  app.UseSwagger();
     // app.UseSwaggerUI();
}
app.UseApiResponseAndExceptionWrapper();
app.ConfigureSwagger();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
    var logger = loggerFactory.CreateLogger("app");
    try
    {
        // var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        // var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        logger.LogInformation("Application Starting Api");
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
app.UseHttpsRedirection();
//app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseRouting();
app.UseAuthentication(); // phải có 2 dòng này mới authen dc, dù lấy token dc nhưng k xác thực dc
app.UseAuthorization();

app.MapControllers();

app.Run();
