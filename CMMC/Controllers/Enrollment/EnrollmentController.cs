using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMMC.Models;
using CTBC;
using System.Net;
using System.Data.SqlClient;
using CTBC.Excel;
using System.Data;
using System.IO;
using Newtonsoft.Json;

namespace CMMC.Controllers.Enrollment
{
    [UserAuthorization]
    [OutputCache(Duration = 0, NoStore = true, VaryByParam = "*")] //
    public class EnrollmentController : Controller
    {
        CMMC.Models.Enrollment enrollment = new Models.Enrollment();
        //
        // GET: /Enrollment/

        //MAIN
        public ActionResult Index()
        {
            //string userModules = JsonConvert.SerializeObject(Session["ModuleList"]);
            //var modules = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SYS_MATRIX.Models.SYS_ACCESS_MODS_MODEL>>(userModules);
            //string a = Request.RequestContext.RouteData.Values["controller"].ToString();

            //ViewBag.Permission = modules.Single(x => x.ModuleCode.Equals(a)).Permission;
            ViewBag.ApproverList = new SYS_MATRIX.Models.SYS_USERS().GetApproverTypeList(1);
            ViewBag.TerminationList = new CMMC.Models.CMSCode().ListForTermination();
            return View();
        }

        public JsonResult GetUnmanagedCMSCodeList()
        {
            return Json(new Models.CMSCode().GetUnmanagedAccounts(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCMSCodeList()
        {
            return Json(new Models.CMSCode().GetListPending(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCMSCodeListApproved()
        {
            return Json(new Models.CMSCode().GetListApproved(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadAvailedServices(Models.ServiceOptions.AvailedDetails pAvailedDetails)
        {
            return Json(new Models.ServiceOptions().GetAvailmentList(pAvailedDetails.CMSCode), JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadNotAddedAccount(Models.AccountsMasterList.Details details)
        {
            return Json(new Models.AccountsMasterList().GetAcccountNotAddedList(details), JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadNotAddedRelatedAccount(Models.AccountsMasterList.Details details)
        {
            return Json(new Models.AccountsMasterList().GetAcccountNotAddedRelatedList(details), JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadServiceName(Models.ServiceOptions.AvailedDetails pDetails)
        {
            return Json(new Models.ServiceOptions().GetServiceName(pDetails).Where(x => x.IsActive), JsonRequestBehavior.AllowGet);
        }

        //public JsonResult LoadAccountNumber(Models.AccountInformation.Details pDetails)
        //{
        // return Json(new Models.AccountInformation().GetNumber(pDetails), JsonRequestBehavior.AllowGet);
        //}

        public JsonResult LoadFreeTransaction()
        {
            return Json(new Models.ServiceOptions().GetFreeTransaction(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadCMSTagging()
        {
            return Json(new Models.CMSCode().GetTagging(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateCmsCode(string pCMSCode)
        {
            ViewBag.ApproverList = new SYS_MATRIX.Models.SYS_USERS().GetApproverTypeList(1);
            ViewBag.RMList = new Models.RelationshipManager().GetList();
            return View();
        }

        private IPAddress LocalIPAddress()
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                return null;
            }
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            return host.AddressList.FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
        }
        //[HttpGet]
        //public JsonResult GetMotherAccount()
        //{

        //    return Json();
        //}


        [HttpPost]
        public JsonResult SaveNewRecord(Models.Enrollment.CmsCodeDetails pDetails, Models.Enrollment.RequestListDetails pRequestListDetails)
        {
            int intCMSCode = 0;
            int intRequestCode = 0;
            int intServiceOptionID = 0;
            Models.CMSCode.Details c_details = new CMSCode.Details();
            Models.Enrollment.RequestListDetails reqlist_details = new Models.Enrollment.RequestListDetails();
            Models.Enrollment.RequestsDetails req_details = new Models.Enrollment.RequestsDetails();

            Models.CMSCode cmscode = new CMSCode();
            c_details.CMSCode = //pDetails.CMSCode;
            c_details.BranchCode = pDetails.GeneralDetails.BranchCode;
            c_details.BranchName = pDetails.GeneralDetails.BranchName;
            c_details.Description = pDetails.GeneralDetails.Description;
            c_details.IsActive = pDetails.GeneralDetails.IsActive;
            c_details.Status = pDetails.GeneralDetails.Status;
            c_details.CreatedBy = pDetails.GeneralDetails.CreatedBy;
            c_details.CreatedOn = pDetails.GeneralDetails.CreatedOn;
            c_details.Tagging = pDetails.GeneralDetails.Tagging;
            c_details.BasePenalty = pDetails.GeneralDetails.BasePenalty;
            c_details.PenaltyFee = pDetails.GeneralDetails.PenaltyFee;
            c_details.IsAutoDebit = pDetails.GeneralDetails.IsAutoDebit;
            c_details.MaxFreeTransaction = pDetails.GeneralDetails.MaxFreeTransaction;
            c_details.MaxWithdrawalPaidByEmployer = pDetails.GeneralDetails.MaxWithdrawalPaidByEmployer;
            c_details.WithdrawalFeePerTransaction = pDetails.GeneralDetails.WithdrawalFeePerTransaction;
            intCMSCode = cmscode.Insert(c_details);
            c_details.CMSCode = intCMSCode;
            req_details.IPAddress = LocalIPAddress().ToString();
            req_details.CreatedBy = pRequestListDetails.RequestsDetails.CreatedBy;
            req_details.CreatedOn = pRequestListDetails.RequestsDetails.CreatedOn;
            req_details.ModifiedBy = pRequestListDetails.RequestsDetails.ModifiedBy;
            req_details.ModifiedOn = pRequestListDetails.RequestsDetails.ModifiedOn;
            req_details.RequestCode = pRequestListDetails.RequestsDetails.RequestCode;
            req_details.AssignedApprover = pRequestListDetails.RequestsDetails.AssignedApprover;
            pRequestListDetails.AffectedTable = "CMSCodes";
            pRequestListDetails.Remarks = "Request to add CMSCode";
            pRequestListDetails.RequestsDetails = req_details;
            pRequestListDetails.RequestCode = new Models.RequestList().InsertRequest(pRequestListDetails.RequestsDetails);
            new Models.RequestList().InsertRequestList(c_details, pRequestListDetails, true, intCMSCode);
            if (intCMSCode > 0)
            {
                if (pDetails.AvailedDetailsList != null)
                {
                    //Services
                    Models.ServiceOptions.AvailedDetails a_details = new ServiceOptions.AvailedDetails();
                    Models.ServiceOptions ser = new ServiceOptions();
                    foreach (var item in pDetails.AvailedDetailsList)
                    {
                        a_details.EnrolledBy = item.EnrolledBy;
                        a_details.EnrolledOn = item.EnrolledOn;
                        a_details.ModifiedBy = item.ModifiedBy;
                        a_details.ModifiedOn = item.ModifiedOn;
                        a_details.ServiceID = item.ServiceID;
                        a_details.RMID = item.RMID;
                        a_details.ServiceName = item.ServiceName;
                        a_details.Status = item.Status;
                        a_details.CMSCode = intCMSCode;
                        a_details.MotherRequiredADB = item.MotherRequiredADB;
                        a_details.SubRequiredADB = item.SubRequiredADB;
                        a_details.MinNumberEmployee = item.MinNumberEmployee;
                        pRequestListDetails.AffectedTable = "ServicesAvailment";
                        pRequestListDetails.Remarks = "Request to add Services " + item.ServiceName;
                        new Models.RequestList().InsertRequestList(a_details, pRequestListDetails, true, intCMSCode);
                    }
                }
            }
            return Json(new Models.RequestList().SendEmailRequestNotification(true, true, pRequestListDetails.RequestsDetails.AssignedApprover, pRequestListDetails.RequestsDetails.CreatedBy, pRequestListDetails.RequestCode), JsonRequestBehavior.DenyGet);
        }



        public ActionResult EditCmsCode(int CMSCode)
        {
            //string userModules = JsonConvert.SerializeObject(Session["ModuleList"]);
            //var modules = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SYS_MATRIX.Models.SYS_ACCESS_MODS_MODEL>>(userModules);
            //string a = Request.RequestContext.RouteData.Values["action"].ToString();
            //ViewBag.Permission = modules.Single(x => x.ModuleCode.Equals(a)).Permission;
            ViewBag.ApproverList = new SYS_MATRIX.Models.SYS_USERS().GetApproverTypeList(1);
            ViewBag.RMList = new Models.RelationshipManager().GetList();
            Models.Enrollment.CmsCodeDetails details = new Models.Enrollment().GetEnrollmentDetails(CMSCode);
            return View(details);
        }
        //> Save changes
        [HttpPost]
        public JsonResult SaveCmsDetails(Models.Enrollment.CmsCodeDetails pDetails, Models.Enrollment.RequestListDetails pRequestListDetails)
        {
            int intRequestCode = 0;
            Models.Enrollment.RequestsDetails req_details = new Models.Enrollment.RequestsDetails();
            req_details.IPAddress = LocalIPAddress().ToString();
            req_details.CreatedBy = pRequestListDetails.RequestsDetails.CreatedBy;
            req_details.CreatedOn = pRequestListDetails.RequestsDetails.CreatedOn;
            req_details.ModifiedBy = pRequestListDetails.RequestsDetails.ModifiedBy;
            req_details.ModifiedOn = pRequestListDetails.RequestsDetails.ModifiedOn;
            req_details.AssignedApprover = pRequestListDetails.RequestsDetails.AssignedApprover;
            pRequestListDetails.RequestsDetails = req_details;
            intRequestCode = new Models.Enrollment().SaveUpdateRequest(pDetails, pRequestListDetails);
            return Json(new CMMC.Models.RequestList().SendEmailRequestNotification(true, false, pRequestListDetails.RequestsDetails.AssignedApprover,
             pRequestListDetails.RequestsDetails.CreatedBy, intRequestCode), JsonRequestBehavior.DenyGet);
        }

        public JsonResult ApprovedCMSCode(Models.CMSCode.Details details)
        {
            return Json(new Models.CMSCode().ApproveCMSCode(details), JsonRequestBehavior.DenyGet);
        }

        public JsonResult CancelandRemove(Models.Enrollment.CmsCodeDetails details)
        {
            return Json(new Models.Enrollment().CancelandRemove(details), JsonRequestBehavior.DenyGet);
        }

        //DELETE
        public ActionResult DeleteCMSCode(CMMC.Models.CMSCode.Details details)
        {
            //int cmscode = new CMMC.Models.CMSCode().Delete(details);
            if (new CMMC.Models.CMSCode().Delete(details) > 0)
            {
                return RedirectToAction("Index", "Enrollment");
            }
            else
            {
                return View("Index");
            }
        }

        public JsonResult LoadAccountNumber(CMMC.Models.AccountsMasterList.Details details)
        {
            return Json(new Models.AccountsMasterList().GetAccountNoAddedList(details), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewDetails(int CMSCode)
        {
            ViewBag.TerminationList = new CMMC.Models.CMSCode().ListForTermination();
            Models.Enrollment.CmsCodeDetails details = new Models.Enrollment().GetEnrollmentDetails(CMSCode);
            return View(details);
        }

        public bool ReturnStringvalue(string pAccount)
        {
            return Misc.IsChinatrustAccount(pAccount);
        }

        public JsonResult LoadInvestmentType()
        {
            return Json(new Models.AccountInformation().getInvestmentType(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadBranchName()
        {
            return Json(new Models.Branches().GetBranchesName().Where(x => x.IsActive), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Excel(List<Models.ChildAccounts.DataForExcel> values, int pCMSCode)
        {
            var FileName = "Child Accounts.xlsx";
            var FullWithFileName = Path.Combine(GetFilePath(), FileName);
            FileInfo newFile = new FileInfo(FullWithFileName);
            try
            {
                using (ExcelPackage pck = new ExcelPackage(newFile))
                {
                    var ws = pck.Workbook.Worksheets.FirstOrDefault(x => x.Name == "Child Accounts for CMSCode" + pCMSCode);
                    if (ws == null)
                    {
                        ws = pck.Workbook.Worksheets.Add("Child Accounts for CMSCode" + pCMSCode);
                    }
                    ws.Cells["A1"].LoadFromCollection(values, true);
                    ws.Cells.AutoFitColumns();
                    int totalCols = ws.Dimension.End.Column;
                    var headerCells = ws.Cells[1, 1, 1, totalCols];
                    var headerFont = headerCells.Style.Font;
                    headerFont.Bold = true;

                    pck.Save();
                    System.Diagnostics.Process.Start(FullWithFileName);
                }

            }
            catch (Exception e)
            {
                CTBC.Logs.Write("Excel", e.Message, "Enrollment Controller");
            }

            return Json(GetFilePath(), JsonRequestBehavior.DenyGet);
        }

        public string GetFilePath()
        {
            return HttpContext.Request.PhysicalApplicationPath;
        }

        public JsonResult CheckIfServiceIsForRequest(int pCMSCode, int pServiceID)
        {
            return Json(new Models.RequestList().IsServiceAlreadyPendingforRequest(pCMSCode, pServiceID), JsonRequestBehavior.AllowGet);
        }

        public JsonResult InsertForTerminationRequest(int pCMSCode, Models.Enrollment.RequestListDetails pReqList)
        {
            Models.Enrollment.RequestsDetails req = new Models.Enrollment.RequestsDetails();
            req.AssignedApprover = pReqList.RequestsDetails.AssignedApprover;
            req.CreatedBy = pReqList.RequestsDetails.CreatedBy;
            req.CreatedOn = pReqList.RequestsDetails.CreatedOn;
            req.IPAddress = LocalIPAddress().ToString();
            req.RequestCode = new Models.RequestList().InsertRequest(req);
            pReqList.RequestsDetails = req;
            return Json(new Models.RequestList().InsertForTerminationRequest(pCMSCode, pReqList), JsonRequestBehavior.DenyGet);
        }

        public bool CheckCMSName(string sCompanyName) //AFM 20220502 check similar name
        {
            Models.CMSCode.Details detail = new Models.CMSCode.Details();
            Models.CMSCode model = new Models.CMSCode();
            detail = model.GetCMSCodeDetails(0, sCompanyName);

            if (!string.IsNullOrEmpty(detail.Description))
                return true;
            else
                return false;
        }
        public bool CheckExistingCMS(string sAcctNo)
        {
            Models.AccountInformation.Details detail = new Models.AccountInformation.Details();
            Models.AccountInformation model = new Models.AccountInformation();
            detail = model.Fill(0, sAcctNo);

            if (!string.IsNullOrEmpty(detail.AccountNumber))
                return true;
            else
                return false;
        }

        public JsonResult SendEmailUnmanagedAccountsList(List<int> pCMSCode, List<string> pBranchName)
        {
            return Json(new CMMC.Models.RequestList().SendEmailForUnmanagedAccount(pCMSCode, pBranchName), JsonRequestBehavior.DenyGet);
        }

        public JsonResult GetCIFDetails(string sCIFNo)
        {
            return Json(new CMMC.Models.CIFDetails().Fill(sCIFNo), JsonRequestBehavior.AllowGet);
        }
    }
}
