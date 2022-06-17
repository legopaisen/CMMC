using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CTBC;

namespace CMMC.Controllers
{
 [Authorize]
 [OutputCache(Duration = 0, NoStore = true, VaryByParam = "*")]
 public class HomeController : Controller
 {
  public ActionResult Index()
  {
   if (!Request.IsAuthenticated || Session["UserID"] == null)
   {
    return RedirectToAction("SignOut", "Login");
   }
   else
   {
    if (Session["Access_Code"].ToString().ToInt() == 1)
    {
     return RedirectToAction("Index", "Access");
    }
    else if (Session["Access_Code"].ToString().ToInt() == 6)
    {
     return RedirectToAction("Index", "SystemSettings");
    }
    else if (Session["Access_Code"].ToString().ToInt() == 12)
    {
     return RedirectToAction("Index", "RegularBatchRun");
    } 
   }
   ViewBag.TerminationList = new CMMC.Models.CMSCode().ListForTermination();
   ViewBag.BranchList = new Models.Branches().GetBranchesName();
   return View();
  }
  public ActionResult EditRequest(int CMSCode)
  {
   int intCMSCode = new Models.RequestList().GetCMSCodebyRequestCode(CMSCode);
   ViewBag.BranchList = new Models.Branches().GetBranchesName().Where(x => x.IsActive);
   ViewBag.MaxTransaction = new Models.ServiceOptions().GetFreeTransaction();
   ViewBag.ClientType = new Models.CMSCode().GetTagging();
   ViewBag.ServiceList = new Models.ServiceOptions().ServiceName().Where(x => x.IsActive);
   ViewBag.ApproverList = new SYS_MATRIX.Models.SYS_USERS().GetApproverTypeList(1);
   ViewBag.RequestDetails = new Models.RequestList().GetRequestForEdit(CMSCode);
   ViewBag.ServiceNotAvailed = new Models.ServiceOptions().GetServiceNameForRequest(intCMSCode).Where(x => x.IsActive);
   ViewBag.RMList = new Models.RelationshipManager().GetList();
   return View(new Models.Enrollment().GetEnrollmentDetails(intCMSCode));
  }

  public JsonResult LoadPendingRequest()
  {
   return Json(new Models.Enrollment().GetPendingRequest(), JsonRequestBehavior.AllowGet);
  }

  public JsonResult ApproveRequestByRequestCode(string pRequestCode, bool pIsApproved, string pApprover, bool pIsForApprover, bool pIsStatus, string pMPersonId, string pSPersonId)
  {
   return Json(new Models.RequestList().ProcessRequest(pRequestCode.ToInt(), pIsApproved, pApprover, pIsForApprover, pIsStatus, pMPersonId, pSPersonId), JsonRequestBehavior.DenyGet);
  }

  public JsonResult ApproveAllRequestByRequestCode(List<string> pRequestCodeList, string pMPersonId, bool forApproved)
  {
   return Json(new CMMC.Models.RequestList().ProcessRequestList(pRequestCodeList, true, pMPersonId, false, forApproved, pMPersonId), JsonRequestBehavior.DenyGet);
  }

  public JsonResult RejectRequestByRequestCode(int pRequestCode, bool pIsNewCMSCode, string pApprover, bool pIsForApprover, bool pIsStatus, string pMPersonId, string pSPersonId)
  {
   return Json(new Models.RequestList().RejectRequest(pRequestCode, pIsNewCMSCode, pApprover, pIsForApprover, pIsStatus, pMPersonId, pSPersonId), JsonRequestBehavior.DenyGet);
  }

  public JsonResult RejectAllRequestByRequestCode(List<Models.ParamData> pRequestCodeList, bool pIsForApprover, bool pIsStatus, string pMPersonId)
  {
   return Json(new Models.RequestList().RejectRequestList(pRequestCodeList, pMPersonId, pIsForApprover, pIsStatus, pMPersonId), JsonRequestBehavior.DenyGet);
  }

  public JsonResult CancelRequestByRequestCode(int pRequestCode, bool pIsNewCMSCode)
  {
   return Json(new Models.RequestList().CancelRequest(pRequestCode, pIsNewCMSCode), JsonRequestBehavior.DenyGet);
  }

  public JsonResult CancelAllRequestByRequestCode(List<Models.ParamData> pRequestCodeList)
  {
   return Json(new Models.RequestList().CancelAllRequest(pRequestCodeList), JsonRequestBehavior.DenyGet);
  }

  public JsonResult CancelRequestByRequestListCode(string pRequestListCode)
  {
   return Json(new Models.RequestList().CancelRequestByRequestListCode(pRequestListCode), JsonRequestBehavior.DenyGet);
  }

  public JsonResult LoadDetailsByRequestListCode(int pRequestListCode)
  {
   return Json(new Models.RequestList().GetRequestByRequestListCode(pRequestListCode), JsonRequestBehavior.AllowGet);
  }

  public JsonResult GetCMSCode(int pRequestCode)
  {
   return Json(new Models.RequestList().GetCMSCodebyRequestCode(pRequestCode), JsonRequestBehavior.AllowGet);
  }

  public JsonResult LoadBranchName()
  {
   return Json(new Models.Branches().GetBranchesName(), JsonRequestBehavior.AllowGet);
  }

  public JsonResult LoadRequestListCodeDetails()
  {
   return Json(new Models.RequestList().RequestListCodeDetails(), JsonRequestBehavior.AllowGet);
  }

  public JsonResult LoadRequestListDetails()
  {
   return Json(new Models.RequestList().RequestListDetails(), JsonRequestBehavior.AllowGet);
  }

  public ActionResult EditRequestByRequestCode(int CMSCode)
  {
   return View(new Models.RequestList().GetRequestForEdit(CMSCode));
  }

  public JsonResult LoadServices()
  {
   return Json(new Models.ServiceOptions().ServiceName(), JsonRequestBehavior.AllowGet);
  }

  public JsonResult GetNewValues(List<Models.Enrollment.RequestListDetails> ReqDet)
  {
   return Json(new Models.RequestList().UpdateEditofRequest(ReqDet), JsonRequestBehavior.AllowGet);
  }

  public JsonResult GetTypeUser(string pNetworkID)
  {
   return Json(new SYS_MATRIX.Models.SYS_USERS().GetTypeUser(pNetworkID), JsonRequestBehavior.AllowGet);
  }

  public JsonResult UpdateRequest(List<Models.Enrollment.RequestListDetails> ReqDet, Models.Enrollment.CmsCodeDetails pdetails, string pRequestCode, string pMUserID, string pSUserID)
  {
   new Models.RequestList().UpdateEditofRequest(ReqDet);
   new Models.RequestList().UpdateServiceRequest(pRequestCode.ToInt(), pdetails);
   return Json(new Models.RequestList().SendEmailRequestNotification(true, false, pMUserID, pSUserID, pRequestCode.ToInt()), JsonRequestBehavior.DenyGet);
  }

  public JsonResult SendAccountMaintainedStatus(List<string> pCMSCodes)
  {
   return Json(new Models.RequestList().SendEmailForAccountMaintenanceStatus(pCMSCodes), JsonRequestBehavior.AllowGet);
  }


 }
}
