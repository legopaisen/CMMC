using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMMC.Models;
using CTBC;


namespace CMMC.Controllers
{
  [OutputCache(Duration = 0, NoStore = true, VaryByParam = "*")]
    public class FAQController : Controller
    {
        //
        // GET: /FAQ/

        public ActionResult Index()
        {
         List<FAQModel> list = new FAQ().GetList();

         string a = list.XMLSerialize();

         return View(list);
        }

    }
}
