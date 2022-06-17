using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMMC.Controllers
{
  [UserAuthorization]
  [OutputCache(Duration = 0, NoStore = true, VaryByParam = "*")]
    public class UserSettingsController : Controller
    {
        //
        // GET: /UserSettings/

        public ActionResult Index()
        {
            return View();
        }

    }
}
