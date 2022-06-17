using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Optimization;

namespace CMMC
{
 // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
 // visit http://go.microsoft.com/?LinkId=9394801
 public class MvcApplication : System.Web.HttpApplication
 {
  protected void Application_PreSendRequestHeaders()
  {
   base.Response.Headers.Remove("Server");
   base.Response.Headers.Remove("X-AspNet-Version");
   base.Response.Headers.Remove("X-AspNetMvc-Version");
  }

  protected static void Session_End(object Sender, EventArgs e)
  {
   string a = "";
  }

  protected void Application_EndRequest()
  {
   ///////////////////////
   //// UNCOMMENT IN PROD //
   ///////////////////////
   ////foreach (var cookie in Request.Cookies.AllKeys)
   ////{
   //// Response.Cookies.Get(cookie).Secure = true;
   ////}   
  }

  protected void Application_BeginRequest(Object source, EventArgs e)
  {
    HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");
   if (SystemCore.ApplicationURL == null)
   {
    string strURL = "";
    HttpApplication app = (HttpApplication)source;
    HttpContext context = app.Context;
    strURL += app.Context.Request.Url.Scheme;
    strURL += "://";
    strURL += app.Context.Request.Url.Authority;
    strURL += app.Context.Request.ApplicationPath;
    SystemCore.ApplicationURL = strURL;
   }
  }

  protected void Application_Error(object source, EventArgs e)
  {
   string a = "";
  }
  protected void Application_Start()
  {
   SystemCore.SecurityKey = "5329b65f5b773130e1f6b864d72dd231";
   RouteTable.Routes.MapHubs();
   AreaRegistration.RegisterAllAreas();
   WebApiConfig.Register(GlobalConfiguration.Configuration);
   FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
   RouteConfig.RegisterRoutes(RouteTable.Routes);
   BundleConfig.RegisterBundles(BundleTable.Bundles);
  }
 }
}