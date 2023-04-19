namespace Web.ManagerApplication.Extensions
{
    public static class RoutingConfig
    {
        public static void Include(IApplicationBuilder app)
        {
            app.UseMvc(routes =>
            {

                routes.MapRoute(
                  name: "quan-ly-don-hang",
                  template: "quan-ly-don-hang",
                  defaults: new { controller = "Account", action = "OrderMy" },
                    //new[] { "duocphamngochiep.Controllers" }
                    new { Controller = "Account", Action = "OrderMy" }
                  );

                routes.MapRoute(
                 name: "theo-doi-don-hang",
                 template: "theo-doi-don-hang",
                 defaults: new { controller = "OrderCustomer", action = "Tracking" },
                   new { Controller = "OrderCustomer", Action = "Tracking" }
                 );

                routes.MapRoute(
                name: "tracking",
                template: "tracking",
                defaults: new { controller = "OrderCustomer", action = "TrackingPublish" },
                  new { Controller = "OrderCustomer", Action = "TrackingPublish" }
                );


                routes.MapRoute(
                 name: "tai-khoan-cua-toi",
                 template: "tai-khoan-cua-toi",
                 defaults: new { controller = "Account", action = "Profile" },
                   //new[] { "duocphamngochiep.Controllers" }
                   new { Controller = "Account", Action = "Profile" }
                 );


                routes.MapRoute(
               name: "thong-bao-cua-toi",
               template: "thong-bao-cua-toi",
               defaults: new { controller = "Account", action = "Notifmy" },
                 //new[] { "duocphamngochiep.Controllers" }
                 new { Controller = "Account", Action = "Notifmy" }
               );


                routes.MapRoute(
               name: "doi-mat-khau",
               template: "doi-mat-khau",
               defaults: new { controller = "Account", action = "ChangePass" },
                 //new[] { "duocphamngochiep.Controllers" }
                 new { Controller = "Account", Action = "ChangePass" }
               );

                routes.MapRoute(
              name: "SaleRetail",
              template: "ban-le",
              defaults: new { controller = "SaleRetail", action = "Index", areas = "Selling" },
                //new[] { "duocphamngochiep.Controllers" }
                new { Controller = "SaleRetail", Action = "Index", Areas = "Selling" }
              );



                routes.MapRoute(
                  name: "slug",
                  template: "{slug}",
                  defaults: new { controller = "Site", action = "index" },
                    //new[] { "duocphamngochiep.Controllers" }
                    new { Controller = "Site", Action = "Index" }
                  );





                routes.MapRoute("Default", "{controller = Home}/{action = Index}/{id?}");


            }
            );
        }
    }
}
