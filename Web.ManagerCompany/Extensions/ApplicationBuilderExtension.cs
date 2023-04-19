using Microsoft.Extensions.Options;
namespace Web.ManagerCompany.Extensions
{
    public static class ApplicationBuilderExtension
    {
        public static void UseMultiLingualFeature(this IApplicationBuilder app)
        {
            app.UseRequestLocalization(app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);
        }
    }
}
