using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMMC.Controllers
{
 public class ErrorController : Controller
 {
  public ActionResult Unauthorized()
  {
   return View();
  }

  public ActionResult AccessDenied()
  {
   return View();
  }

  public ActionResult SystemError()
  {
   return View();
  }
  public ActionResult PageNotFoundError()
  {
   return View();
  }

 }
}
