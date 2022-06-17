using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CMMC
{
 public class RouteConfig
 {
  public static void RegisterRoutes(RouteCollection routes)
  {
   routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

   routes.MapRoute(
       name: "Default",
       url: "{controller}/{action}/{CMSCode}",
       defaults: new { controller = "Home", action = "Index", CMSCode = UrlParameter.Optional}
   );
  }
 }
}