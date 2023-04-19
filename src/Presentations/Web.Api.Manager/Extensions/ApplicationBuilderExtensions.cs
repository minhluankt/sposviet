using Microsoft.AspNetCore.Builder;
namespace Web.Api.Manager.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void ConfigureSwagger(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Web.Api.Manager");
                options.RoutePrefix = "swagger";
                options.DisplayRequestDuration();
            });
        }
    }
}
