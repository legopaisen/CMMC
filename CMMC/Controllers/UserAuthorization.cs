using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CTBC;

public class UserAuthorization : AuthorizeAttribute
{
 public override void OnAuthorization(AuthorizationContext filterContext)
 {
  if (filterContext.HttpContext.Request.IsAuthenticated && filterContext.HttpContext.Session["UserID"] != null)
  {
   string userModules = JsonConvert.SerializeObject(HttpContext.Current.Session["ModuleList"]);
   var modules = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SYS_MATRIX.Models.SYS_ACCESS_MODS_MODEL>>(userModules);
   
   if (modules.Where(x => filterContext.RouteData.Values.Values.Where(y => y.Equals(x.ModuleCode)).ToList().Count > 0).ToList().Count == 0 )
   {
    filterContext.Result = new RedirectToRouteResult(new
    RouteValueDictionary(new { controller = "Error", action = "AccessDenied" }));
    base.OnAuthorization(filterContext);
   }
  }
  else
  {
   if (filterContext.HttpContext.Session["UserID"] != null)
   {
    if (filterContext.HttpContext.Session["UserID"].Equals("SYSTEM"))
    {
     //DO NOTHING. 
    }
    else
    {
     filterContext.Result = new RedirectResult("~/User/SignOut");
     base.OnAuthorization(filterContext);
    }
   }
   else
   {
    filterContext.Result = new RedirectResult("~/User/SignOut");
    base.OnAuthorization(filterContext);
   }
  } 
 }
}