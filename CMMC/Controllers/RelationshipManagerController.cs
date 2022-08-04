using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SYS_MATRIX;
using CTBC;
using CMMC.Models;

namespace CMMC.Controllers
{
    [UserAuthorization]
    [OutputCache(Duration = 0, NoStore = true, VaryByParam = "*")]

    public class RelationshipManagerController : Controller
    {
        //
        // GET: /RelationshipManager/

        public ActionResult Index()
        {
            ViewBag.GetRMList = new Models.RelationshipManager().GetList();
            ViewBag.BranchesList = new Models.Branches().GetBranchesName();
            return View();
        }

        public ActionResult AddRM()
        {
            ViewBag.BrancheList = new Models.Branches().GetBranchesName().Where(x => x.IsActive);
            return View();
        }

        public ActionResult EditRM(string CMSCode)
        {
            ViewBag.BrancheList = new Models.Branches().GetBranchesName().Where(x => x.IsActive);
            CMMC.Models.RelationshipManagerModel details = new Models.RelationshipManager().Fill(CMSCode);
            return View(details);
        }

        [HttpPost]
        public JsonResult SaveNewRecord(Models.RelationshipManagerModel pDetails)
        {
            int intReturn = 0;
            SystemCore.SystemResponse response = new SystemCore.SystemResponse();
            using (Models.RelationshipManager RM = new Models.RelationshipManager())
            {
                pDetails.AddedBy = Session["UserID"].ToString();
                intReturn = RM.Insert(pDetails);

                if (intReturn != 2)
                {
                    using (AuditTrail Audit = new AuditTrail())
                    {
                        Audit.Insert(new AuditTrailModel()
                        {
                            UserID = Session["UserID"].ToString()
                         ,
                            Module = "RelationshipManager"
                         ,
                            NewValues = "Added New Relationship Manager: " + pDetails.RMFullName + " \nRelationship Manager ID: " + pDetails.ID + " \nBranch Assigned: " + pDetails.BranchAssigned
                         ,
                            IPAddress = SystemCore.GetIPAddress()
                        });
                    }
                    response.ResponseStatus = SystemCore.ResponseStatus.SUCCESS;
                    response.Description = "New  has been successfully added.";
                }
                //else if (RM.IsExist(pDetails))
                //{
                // response.ResponseStatus = SystemCore.ResponseStatus.FAILED;
                // response.Description = "Relationship Manager already exist.";
                //}
                else
                {
                    response.ResponseStatus = SystemCore.ResponseStatus.FAILED;
                    response.Description = "Unable to save new access.";
                }
            }
            //return Json(new Models.RelationshipManager().Insert(pDetails), JsonRequestBehavior.DenyGet);
            return Json(intReturn, JsonRequestBehavior.DenyGet); // AFM 20220804 - Fixed inconsistent return value
        }

        [HttpPost]
        public JsonResult SaveUpdate(Models.RelationshipManagerModel pDetails)
        {
            int intReturn = 0;
            SystemCore.SystemResponse response = new SystemCore.SystemResponse();
            using (RelationshipManager RM = new RelationshipManager())
            {
                pDetails.ModifiedBy = Session["UserID"].ToString();
                pDetails.ModifiedDate = DateTime.Now;
                RelationshipManagerModel oldModel = new RelationshipManager().Fill((pDetails.ID).ToString());

                string[] arrDifferences = SystemCore.ToStringDifferences(new SystemCore().GetDiffirence(oldModel, pDetails)).Split(new char[] { '|' });
                intReturn = RM.Update(pDetails);
                if (intReturn > 0)
                {
                    using (AuditTrail audit = new AuditTrail())
                    {
                        pDetails.ModifiedDate = DateTime.Now;

                        audit.Insert(new AuditTrailModel()
                        {
                            UserID = Session["UserID"].ToString()
                         ,
                            Module = "RelationshipManager"
                         ,
                            OldValues = "Edited User Name[" + pDetails.RMFullName + "], \nChanges: " + arrDifferences[0].ToString()
                         ,
                            NewValues = arrDifferences[1].ToString()
                         ,
                            IPAddress = SystemCore.GetIPAddress()
                        });
                    }
                    response.ResponseStatus = SystemCore.ResponseStatus.SUCCESS;
                    response.Description = "User Successfully updated";
                }
                else
                {
                    response.ResponseStatus = SystemCore.ResponseStatus.FAILED;
                    response.Description = "Unable to update";
                }
            }
            //return Json(new Models.RelationshipManager().Update(pDetails), JsonRequestBehavior.DenyGet); // AFM 20220804 - Fixed return value inconsistency
            return Json(intReturn, JsonRequestBehavior.DenyGet);

        }

        [HttpPost]
        public JsonResult DeleteRM(List<string> pIDs)
        {
            JsonResult jsonReturn = new JsonResult();
            foreach (var item in pIDs)
            {
                jsonReturn = Json(new Models.RelationshipManager().Delete(item.ToString()));
                using (AuditTrail audit = new AuditTrail())
                {
                    audit.Insert(new AuditTrailModel()
                    {
                        UserID = Session["UserID"].ToString()
                      ,
                        Module = "RelationshipManager"
                      ,
                        NewValues = "The Relationship Manager " + item.ToString() + " is already deleted."
                      ,
                        IPAddress = SystemCore.GetIPAddress()
                    });
                }
            }
            return Json(jsonReturn, JsonRequestBehavior.DenyGet);
        }
    }
}
