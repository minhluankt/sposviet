namespace Web.ManagerCompany.Extensions
{
    public static class RoutingConfig
    {
        public static void Include(IApplicationBuilder app)
        {
            app.UseMvc(routes =>
            {

                //routes.MapRoute(
                //  name: "quan-ly-don-hang",
                //  template: "quan-ly-don-hang",
                //  defaults: new { controller = "Account", action = "OrderMy" },
                //    //new[] { "duocphamngochiep.Controllers" }
                //    new { Controller = "Account", Action = "OrderMy" }
                //  );





                routes.MapRoute("Default", "{controller = Home}/{action = Index}/{id?}");


            }
            );
        }
    }
}
