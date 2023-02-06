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
    public class ParameterMaintenanceController : Controller
    {
        //
        // GET: /ParameterMaintenance/
        public ActionResult Index()
        {
          return View(new Branches().GetList());
        }

        public ActionResult AddBranches()
        {
         return View();
        }

        public JsonResult LoadBranchesFromODS()
        {
            SystemCore.SystemResponse response = new SystemCore.SystemResponse();
            
            return Json(response);
        }
        public ActionResult Holidays()
        {
         return View(new YearHolidays().GetList());
        }

        public JsonResult LoadCluster()
        {
         return Json(new Models.Branches().GetCluster(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadCategory()
        {
         return Json(new Models.ServiceTypes().GetCategory(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewBranches(string CMSCode)
        {
         Branches.Details details = new Branches().Fill(CMSCode);
         return View(details);
        }

        public ActionResult UpdateBranches(string CMSCode)
        {
         ViewBag.Cluster = new Branches().GetCluster();
         Branches.Details details = new Branches().Fill(CMSCode);
         return View(details);
        }

        [HttpPost]
        public JsonResult BranchInsert(Branches.Details model)
        {
         SystemCore.SystemResponse response = new SystemCore.SystemResponse();
         model.CreatedOn = DateTime.Now;
         model.CreatedBy = Session["UserID"].ToString();
         using (Branches branch = new Branches())
         {
          int intReturn = branch.Insert(model);
          if (intReturn > 0)
          {
           using (AuditTrail audit = new AuditTrail())
           {
            audit.Insert(new AuditTrailModel()
            {
             UserID = Session["UserID"].ToString()
             ,Module = "SystemParameter"
             ,NewValues = "Added New Branch Code: " + model.BranchCode + "\n" + "Branch Name: " + model.BranchName + "\n" + "Created On: " + model.CreatedOn + "\n" + "Created By: " + model.CreatedBy
             ,IPAddress = SystemCore.GetIPAddress()
            });
           }
           response.ResponseStatus = SystemCore.ResponseStatus.SUCCESS;
           response.Description = "New Branch Added";
          }
          else if (branch.IsExist(model.BranchCode))
          {
           response.ResponseStatus = SystemCore.ResponseStatus.FAILED;
           response.Description = "Branch already exist.";
          }
          else
          {
           response.ResponseStatus = SystemCore.ResponseStatus.FAILED;
           response.Description = "Unable to add branch";
          }
         }
         return Json(response, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult BranchSaveChanges(Branches.Details model)
        {
         SystemCore.SystemResponse response = new SystemCore.SystemResponse();
         model.ModifiedBy = Session["UserID"].ToString(); 
      
         using (Branches group = new Branches())
         {
          Branches.Details oldmodel = new Branches().Fill(model.BranchCode);
          if (group.Update(model) > 0)
          {
           using (AuditTrail audit = new AuditTrail())
           {
            model.CreatedOn = oldmodel.CreatedOn;
            model.ModifiedOn = DateTime.Now;
            model.CreatedBy = Session["UserID"].ToString();
            string[] arrDifferences = SystemCore.ToStringDifferences(new SystemCore().GetDiffirence(oldmodel, model)).Split(new char[] { '|' });
            audit.Insert(new AuditTrailModel()
            {
             UserID = Session["UserID"].ToString()
             ,Module = "SystemParameter"
             ,OldValues = "Edit Branch [" + model.BranchName.ToString() + "]" +"\n" + "Changes:" + arrDifferences[0].ToString()
             ,NewValues = arrDifferences[1].ToString()
             ,IPAddress = SystemCore.GetIPAddress()
            });
           }
           response.ResponseStatus = SystemCore.ResponseStatus.SUCCESS;
           response.Description = "Branch Successfully updated";
          }
          else
          {
           response.ResponseStatus = SystemCore.ResponseStatus.FAILED;
           response.Description = "Unable to update group details";
          }
         }
         return Json(response, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult BranchProcessAction(List<int> IDs, bool pIsActivate)
        {
         SystemCore.SystemResponse response = new SystemCore.SystemResponse();
         int intReturn = new Branches().BranchProcessAction(IDs, pIsActivate, Session["UserID"].ToString());
         string strIDs = string.Join("\n", IDs.ToArray());
         if (intReturn > 0)
         {
          using (AuditTrail audit = new AuditTrail())
          {
           audit.Insert(new AuditTrailModel()
           {
            UserID = Session["UserID"].ToString()
            ,Module = "SystemParameter"
            ,NewValues = pIsActivate ? "Activate Branch:" + strIDs : "Deactivate Branch:" + strIDs
            ,IPAddress = SystemCore.GetIPAddress()
           });
          }
          response.ResponseStatus = SystemCore.ResponseStatus.SUCCESS;
          response.Description = "Selected item(s) has been " + (pIsActivate ? "activated" : "deactivated") + ".";
         }
         else
         {
          response.ResponseStatus = SystemCore.ResponseStatus.FAILED;
          response.Description = "Unable to " + (pIsActivate ? "activate" : "deactivate") + " selected item(s).";
         }
         return Json(response, JsonRequestBehavior.DenyGet);
        }

        public ActionResult Services()
        {
         return View(new ServiceTypes().GetList());
        }

        //public JsonResult Search()
        //{
        // SystemCore.TableSearchResponse response = new SystemCore.TableSearchResponse();
        // SystemCore.Search search = new SystemCore.Search();
        // int intItemCount = 0;

        // search.Limit = Request["limit"] == null ? 10 : Request["limit"].ToString().ToInt();
        // search.OrderBy = Request["order"] == null ? "ASC" : Request["order"].ToString();
        // search.PageNumber = Request["offset"] == null ? 0 : Request["offset"].ToString().ToInt() <= 0 ? 0 : Request["offset"].ToString().ToInt() / search.Limit.Value;
        // search.SearchString = Request["search"] == null ? "" : Request["search"].ToString();

        // response.searchrows = new ServiceTypes().GetList(out intItemCount, search).Cast<object>().ToList();
        // response.searchtotal = intItemCount;
        // return Json(response, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult AddServices()
        {
         return View();
        }

        public ActionResult ViewServices(string CMSCode)
        {
         ServiceTypes.Details details = new ServiceTypes().Fill(CMSCode);
         return View(details);
        }

        public ActionResult UpdateServices(string CMSCode)
        {
         ServiceTypes.Details details = new ServiceTypes().Fill(CMSCode);
         ViewBag.ServiceCategory = new ServiceTypes().GetCategory(); //
         return View(details);
        }

        [HttpPost]
        public JsonResult ServiceSaveChanges(ServiceTypes.Details model)
        {
         SystemCore.SystemResponse response = new SystemCore.SystemResponse();
         model.ModifiedBy = Session["UserID"].ToString();
         using (ServiceTypes group = new ServiceTypes())
         {
          ServiceTypes.Details oldmodel = new ServiceTypes().Fill(model.ServiceID);
          if (group.Update(model) > 0)
          {
           using (AuditTrail audit = new AuditTrail())
           {
            model.CreatedOn = oldmodel.CreatedOn;
            model.ModifiedOn = DateTime.Now;
            string[] arrDifferences = SystemCore.ToStringDifferences(new SystemCore().GetDiffirence(oldmodel, model)).Split(new char[] { '|' });
            audit.Insert(new AuditTrailModel()
            {
             UserID = Session["UserID"].ToString()
             ,Module = "SystemParameter"
             ,OldValues = "Edit Service [" + model.ServiceName.ToString() + "]" + "Changes: " + arrDifferences[0].ToString()
             ,NewValues = arrDifferences[1].ToString()
             ,IPAddress = SystemCore.GetIPAddress()
            });
           }
           response.ResponseStatus = SystemCore.ResponseStatus.SUCCESS;
           response.Description = "Service Successfully updated";
          }
          else
          {
           response.ResponseStatus = SystemCore.ResponseStatus.FAILED;
           response.Description = "Unable to update group details";
          }
         }
         return Json(response, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult ServiceInsert(ServiceTypes.Details model)
        {
         SystemCore.SystemResponse response = new SystemCore.SystemResponse();
         model.CreatedOn = DateTime.Now;
         model.CreatedBy = Session["UserID"].ToString();
         using (ServiceTypes services = new ServiceTypes())
         {
          int intReturn = services.Insert(model);
          if (intReturn > 0)
          {
           using (AuditTrail audit = new AuditTrail())
           {
            audit.Insert(new AuditTrailModel()
            {
             UserID = Session["UserID"].ToString()
             ,Module = "SystemParameter"
             ,NewValues = "Added New Service Type: " + model.ServiceName
             ,IPAddress = SystemCore.GetIPAddress()
            });
           }
           response.ResponseStatus = SystemCore.ResponseStatus.SUCCESS;
           response.Description = "New Service Type Added";
          }
          else if (services.IsExist(model.ServiceName))
          {
           response.ResponseStatus = SystemCore.ResponseStatus.FAILED;
           response.Description = "Service Type already exist.";
          }
          else
          {
           response.ResponseStatus = SystemCore.ResponseStatus.FAILED;
           response.Description = "Unable to add Service Type";
          }
         }
         return Json(response, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult ServicesProcessAction(List<int> IDs, bool pIsActivate)
        {
         SystemCore.SystemResponse response = new SystemCore.SystemResponse();
         int intReturn = new ServiceTypes().ServicesProcessAction(IDs, pIsActivate, Session["UserID"].ToString());
         string strIDs = string.Join("\n", IDs.ToArray());
         if (intReturn > 0)
         {
          using (AuditTrail audit = new AuditTrail())
          {
           audit.Insert(new AuditTrailModel()
           {
            UserID = Session["UserID"].ToString()
            ,Module = "SystemParameter"
            ,NewValues = pIsActivate ? "Activate Services: " + strIDs : "Deactivate Services: " + strIDs
            ,IPAddress = SystemCore.GetIPAddress()
           });
          }
          response.ResponseStatus = SystemCore.ResponseStatus.SUCCESS;
          response.Description = "Selected item(s) has been " + (pIsActivate ? "activated" : "deactivated") + ".";
         }
         else
         {
          response.ResponseStatus = SystemCore.ResponseStatus.FAILED;
          response.Description = "Unable to " + (pIsActivate ? "activate" : "deactivate") + " selected item(s).";
         }
         return Json(response, JsonRequestBehavior.DenyGet);
        }
        
        public ActionResult ProductType()
        {
         return View(new ProductTypes().GetList());
        }

        public ActionResult ViewProductType(string CMSCode)
        {
         ProductTypes.ProductDetails details = new ProductTypes().Fill(CMSCode);
         return View(details);
        }

        public ActionResult UpdateProduct(string CMSCode)
        {
         ProductTypes.ProductDetails details = new ProductTypes().Fill(CMSCode);
         return View(details);
        }

        public ActionResult AddProduct()
        {
         return View();
        }

        [HttpPost]
        public JsonResult ProductInsert(ProductTypes.ProductDetails model)
        {
         SystemCore.SystemResponse response = new SystemCore.SystemResponse();
         model.CreatedOn = DateTime.Now;
         model.CreatedBy = Session["UserID"].ToString();
         using (ProductTypes productType = new ProductTypes())
         {
          int intReturn = productType.Insert(model);
          if (intReturn > 0)
          {
           using (AuditTrail audit = new AuditTrail())
           {
            audit.Insert(new AuditTrailModel()
            {
             UserID = Session["UserID"].ToString()
             ,Module = "SystemParameter"
             ,NewValues = "Added New Product Code: " + model.ProductCode + " Product Description: " + model.ProductDescription
             ,IPAddress = SystemCore.GetIPAddress()
            });
           }
           response.ResponseStatus = SystemCore.ResponseStatus.SUCCESS;
           response.Description = "New Product Code Added";
          }
          else if (productType.IsExist(model.ProductCode))
          {
           response.ResponseStatus = SystemCore.ResponseStatus.FAILED;
           response.Description = "Product Code already exist.";
          }
          else
          {
           response.ResponseStatus = SystemCore.ResponseStatus.FAILED;
           response.Description = "Unable to add Product Code";
          }
         }
         return Json(response, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult ProductSaveChanges(ProductTypes.ProductDetails model)
        {
         SystemCore.SystemResponse response = new SystemCore.SystemResponse();
         model.ModifiedBy = Session["UserID"].ToString();
         using (ProductTypes group = new ProductTypes())
         {
         ProductTypes.ProductDetails oldmodel = new ProductTypes().Fill(model.ProductCode);
          if (group.Update(model) > 0)
          {
           using (AuditTrail audit = new AuditTrail())
           {
            model.CreatedOn = oldmodel.CreatedOn;
            model.ModifiedOn = DateTime.Now;
            string[] arrDifferences = SystemCore.ToStringDifferences(new SystemCore().GetDiffirence(oldmodel, model)).Split(new char[] { '|' });
            audit.Insert(new AuditTrailModel()
            {
             UserID = Session["UserID"].ToString()
             ,Module = "SystemParameter"
             ,OldValues = "Edit Product Type [" + model.ProductDescription.ToString() + "]" + "Changes:" + arrDifferences[0].ToString()
             ,NewValues = arrDifferences[1].ToString()
             ,IPAddress = SystemCore.GetIPAddress()
            });
           }
           response.ResponseStatus = SystemCore.ResponseStatus.SUCCESS;
           response.Description = "Product Successfully updated";
          }
          else
          {
           response.ResponseStatus = SystemCore.ResponseStatus.FAILED;
           response.Description = "Unable to update Product details";
          }
         }
         return Json(response, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult DeleteProduct(List<string> pIDs)
        {
         JsonResult jsonReturn = new JsonResult();
         foreach (var item in pIDs)
         {
          jsonReturn = Json(new Models.ProductTypes().Delete(item.ToString()));
          using (AuditTrail audit = new AuditTrail())
          {
           audit.Insert(new AuditTrailModel()
           {
            UserID = Session["UserID"].ToString()
            ,Module = "SystemParameter"
            ,NewValues = "The Product Code " + item.ToString() + " is already deleted." 
            ,IPAddress = SystemCore.GetIPAddress()
           });
          }
         }
         return Json(jsonReturn, JsonRequestBehavior.DenyGet);
        }

        public ActionResult InvestmentType()
        {
         return View(new InvestmentTypes().GetList());
        }

        public ActionResult AddInvestment()
        {
         return View();
        }

        public ActionResult UpdateInvestment(string CMSCode)
        {
         InvestmentTypes.InvestmentTypeDetails details = new InvestmentTypes().Fill(CMSCode);
         return View(details);
        }

        public ActionResult ViewInvestment(string CMSCode)
        {
         InvestmentTypes.InvestmentTypeDetails details = new InvestmentTypes().Fill(CMSCode);
         return View(details);
        }

        [HttpPost]
        public JsonResult InvestmentInsert(InvestmentTypes.InvestmentTypeDetails model)
        {
         SystemCore.SystemResponse response = new SystemCore.SystemResponse();
         model.CreatedOn = DateTime.Now;
         model.CreatedBy = Session["UserID"].ToString();
         using (InvestmentTypes investmentType = new InvestmentTypes())
         {
          int intReturn = investmentType.Insert(model);
          if (intReturn > 0)
          {
           using (AuditTrail audit = new AuditTrail())
           {
            audit.Insert(new AuditTrailModel()
            {
             UserID = Session["UserID"].ToString()
             ,Module = "SystemParameter"
             ,NewValues = "Added New Investment Code: " + model.InvestmentCode + " Investment Description: " + model.InvestmentDescription
             ,IPAddress = SystemCore.GetIPAddress()
            });
           }
           response.ResponseStatus = SystemCore.ResponseStatus.SUCCESS;
           response.Description = "New Investment Type Added";
          }
          else if (investmentType.IsExist(model.InvestmentCode))
          {
           response.ResponseStatus = SystemCore.ResponseStatus.FAILED;
           response.Description = "Investment Type already exist.";
          }
          else
          {
           response.ResponseStatus = SystemCore.ResponseStatus.FAILED;
           response.Description = "Unable to add Investment Type";
          }
         }
         return Json(response, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult InvestmentSaveChanges(InvestmentTypes.InvestmentTypeDetails model)
        {
         SystemCore.SystemResponse response = new SystemCore.SystemResponse();
         model.ModifiedBy = Session["UserID"].ToString();
         using (InvestmentTypes group = new InvestmentTypes())
         {
          ServiceTypes.Details oldmodel = new ServiceTypes().Fill(model.InvestmentCode);
          if (group.Update(model) > 0)
          {
           using (AuditTrail audit = new AuditTrail())
           {
            model.CreatedOn = oldmodel.CreatedOn;
            model.ModifiedOn = DateTime.Now;
            string[] arrDifferences = SystemCore.ToStringDifferences(new SystemCore().GetDiffirence(oldmodel, model)).Split(new char[] { '|' });
            audit.Insert(new AuditTrailModel()
            {
             UserID = Session["UserID"].ToString()
             ,Module = "SystemParameter"
             ,OldValues = "Edit Investment [" + model.InvestmentDescription.ToString() + "]" + "Changes:" + arrDifferences[0].ToString()
             ,NewValues = arrDifferences[1].ToString()
             ,IPAddress = SystemCore.GetIPAddress()
            });
           }
           response.ResponseStatus = SystemCore.ResponseStatus.SUCCESS;
           response.Description = "Investment Type Successfully updated";
          }
          else
          {
           response.ResponseStatus = SystemCore.ResponseStatus.FAILED;
           response.Description = "Unable to update Investment Type details";
          }
         }
         return Json(response, JsonRequestBehavior.DenyGet);
        }


        [HttpPost]
        public JsonResult InvestmentProcessAction(List<string> IDs, bool pIsActivate)
        {
         SystemCore.SystemResponse response = new SystemCore.SystemResponse();
         int intReturn = new InvestmentTypes().InvestmentProcessAction(IDs, pIsActivate, Session["UserID"].ToString());
         string strIDs = string.Join("\n", IDs.ToArray());
         if (intReturn > 0)
         {
          using (AuditTrail audit = new AuditTrail())
          {
           audit.Insert(new AuditTrailModel()
           {
            UserID = Session["UserID"].ToString()
            ,Module = "SystemParameter"
            ,NewValues = pIsActivate ? "Activate Investment: " + strIDs : "Deactivate Investment: " + strIDs
            ,IPAddress = SystemCore.GetIPAddress()
           });
          }
          response.ResponseStatus = SystemCore.ResponseStatus.SUCCESS;
          response.Description = "Selected item(s) has been " + (pIsActivate ? "activated" : "deactivated") + ".";
         }
         else
         {
          response.ResponseStatus = SystemCore.ResponseStatus.FAILED;
          response.Description = "Unable to " + (pIsActivate ? "activate" : "deactivate") + " selected item(s).";
         }
         return Json(response, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult DeleteBranch(List<string> pIDs)
        {
       
         JsonResult jsonReturn = new JsonResult();
         foreach (var item in pIDs)
         {
          jsonReturn = Json(new Models.Branches().Delete(item.ToString()));
          using (AuditTrail audit = new AuditTrail())
          {
           audit.Insert(new AuditTrailModel()
           {
            UserID = Session["UserID"].ToString()
            ,Module = "SystemParameter"
            ,NewValues = "The Branch Code " + item.ToString() + " is already deleted."
            ,IPAddress = SystemCore.GetIPAddress()
           });
          }
         }
         return Json(jsonReturn, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult DeleteInvestment(List<string> pIDs)
        {
         JsonResult jsonReturn = new JsonResult();
         foreach (var item in pIDs)
         {
          jsonReturn = Json(new Models.InvestmentTypes().Delete(item.ToString()));
          using (AuditTrail audit = new AuditTrail())
          {
           audit.Insert(new AuditTrailModel()
           {
            UserID = Session["UserID"].ToString()
            ,Module = "SystemParameter"
            ,NewValues = "The Investment Code " + item.ToString() + " is already deleted."
            ,IPAddress = SystemCore.GetIPAddress()
           });
          }
         }
         return Json(jsonReturn, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult DeleteServices (List<string> pIDs)
        {
         JsonResult jsonReturn = new JsonResult();
         foreach (var item in pIDs)
         {
           jsonReturn = Json(new Models.ServiceTypes().Delete(item.ToString()));
        using (AuditTrail audit = new AuditTrail())
        {
         audit.Insert(new AuditTrailModel()
           {
            UserID = Session["UserID"].ToString()
            ,Module = "SystemParameter"
            ,NewValues = "The Service Code " + item.ToString() + " is already deleted."
            ,IPAddress = SystemCore.GetIPAddress()
           });
          }
         }
         return Json(jsonReturn, JsonRequestBehavior.DenyGet);
        }
    }
}