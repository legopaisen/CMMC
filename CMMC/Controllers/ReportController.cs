using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMMC.Models;

namespace CMMC.Controllers
{
    [UserAuthorization]
    [OutputCache(Duration = 0, NoStore = true, VaryByParam = "*")]
    public class ReportController : Controller
    {
        //
        // GET: /Report/

        public ActionResult Index()
        {
            ViewBag.ModuleList = new SYS_MATRIX.Models.SYS_MODULES().GetList();
            return View();
        }

        public ActionResult ViewAuditTrailFilter(string pModule)
        {
            ViewBag.title = Request["Title"];
            ViewBag.Modules = pModule;
            return PartialView();
        }

        public ActionResult ViewTransactionalReports(string pModule)
        {
            ViewBag.title = Request["Title"];
            ViewBag.Modules = pModule;
            return PartialView();
        }

        public ActionResult ViewADBReport()
        {
            ViewBag.BranchName = new Branches().GetBranchesName();
            return PartialView();
        }

        public ActionResult ViewNoLinkedAccountsReport()
        {
            ViewBag.BranchName = new Branches().GetBranchesName();
            return PartialView();
        }
        public ActionResult ViewClientAccountsTypeReport()
        {
            return PartialView();
        }
        public ActionResult ViewTotalADBRequirementReport()
        {
            ViewBag.BranchName = new Branches().GetBranchesName();
            return PartialView();
        }
        public ActionResult ViewOrphanAccounts()
        {
            ViewBag.BranchName = new Branches().GetBranchesName();
            return PartialView();
        }
        public ActionResult ViewAlienSubAccounts()
        {
            ViewBag.BranchName = new Branches().GetBranchesName();
            return PartialView();
        }
        public ActionResult ViewForeignCIFNumber()
        {
            ViewBag.BranchName = new Branches().GetBranchesName();
            return PartialView();
        }

        public ActionResult ViewApprove()
        {
            ViewBag.ApproverList = new SYS_MATRIX.Models.SYS_USERS().GetApproverDetails();
            return PartialView();
        }

        public ActionResult ViewDisapproved()
        {
            ViewBag.ApproverList = new SYS_MATRIX.Models.SYS_USERS().GetApproverDetails();
            return PartialView();
        }

        public ActionResult ViewEnrollment()
        {
            ViewBag.MakerList = new SYS_MATRIX.Models.SYS_USERS().GetMakerDetails();
            return PartialView();
        }

        public JsonResult GetDisapprove()
        {
            return Json(new SYS_MATRIX.Models.SYS_USERS().GetApproverDetails(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStatus()
        {
            return Json(new SharedFunctions.Status(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetApprover()
        {
            return Json(new SYS_MATRIX.Models.SYS_USERS().GetApproverDetails(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMaker()
        {
            return Json(new SYS_MATRIX.Models.SYS_USERS().GetMakerDetails(), JsonRequestBehavior.AllowGet);
        }

    }
}

