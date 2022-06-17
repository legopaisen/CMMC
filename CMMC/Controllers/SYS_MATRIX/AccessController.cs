using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using SYS_MATRIX.Models;
using System.Data;
using System.Web.Script.Serialization;
using CMMC.Models;

namespace CMMC.Controllers
{
 [UserAuthorization]
 [OutputCache(Duration = 0, NoStore = true, VaryByParam = "*")] //
 public class AccessController : Controller
 {
  //
  // GET: /Access/
  public ActionResult Index()
  {
   return View(new SYS_ACCESS().GetList());
  }
  
  public ActionResult AccessAddNew()
  {
   ViewBag.GroupList = new SYS_GROUP().GetList();
   ViewBag.UserTypeList = new SYS_ACCESS().GetUserType();
   List<SYS_MODULES_MODEL> modulelist = new SYS_MODULES().GetList();  
   ViewBag.ModuleList = modulelist;
   List<SYS_ACCESS_MODS_MODEL> permissionlist = new SYS_ACCESS_MODS().GetPermissionList();
   ViewBag.PermissionList = permissionlist;
   return View();
  }

  public ActionResult AccessEdit(int CMSCode)
  {
   ViewBag.GroupList = new SYS_GROUP().GetList();
   List<SYS_MODULES_MODEL> modulelist = null;  //new SYS_MODULES().GetList();
   List<SYS_MODULES_MODEL> assignedmodules = new List<SYS_MODULES_MODEL>();
   List<SYS_MODULES_MODEL> modules = new SYS_MODULES().GetList();
   ViewBag.UserTypeList = new SYS_ACCESS().GetUserType();
   List<SYS_ACCESS_MODS_MODEL> permissionlist = new SYS_ACCESS_MODS().GetPermissionList();
   ViewBag.PermissionList = permissionlist;

   List<SYS_ACCESS_MODS_MODEL> accessassignedmod = new SYS_ACCESS_MODS().GetList(CMSCode);
   
   SYS_ACCESS_MODEL access = new SYS_ACCESS().Fill(CMSCode);
   string[] mods = (from n in access.ModuleList select n.ModuleCode).ToArray();

   modulelist = modules.Where(x =>
    !mods.Contains(x.Module_Code)
    ).ToList();

   ViewBag.AccessAssignedMod = accessassignedmod.Where(x => 
    mods.Contains(x.ModuleCode)
    ).ToList();


   var assmod = modules.Where(x =>
    mods.Contains(x.Module_Code)).ToList();
    //.Where(y => 
    //accessassignedmod.Contains(y.Module_Code)
    //).ToList();

   ViewBag.AssignedModules = (from a in assmod
                              join b in accessassignedmod
                              on a.Module_Code
                              equals b.ModuleCode
                              select new {
                               a.Module_Code, a.ModuleName, 
                               a.ViewIndex, b.Permission, b.PermissionText                              
                              }).ToList();

   ViewBag.ModuleList = modulelist;
   return View(access);
  }

  public ActionResult AccessView(int CMSCode)
  {
   ViewBag.Groups = new SYS_GROUP().GetList();
   SYS_ACCESS_MODEL access = new SYS_ACCESS().Fill(CMSCode);
   ViewBag.UserTypeList = new SYS_ACCESS().GetUserType();
   return View(access);
  }


  [HttpPost]  
  public JsonResult AccessInsert(SYS_ACCESS_MODEL Model)
  {
   SystemCore.SystemResponse response = new SystemCore.SystemResponse();
   using (SYS_ACCESS access = new SYS_ACCESS())
   {
    Model.CreatedBy = Session["UserID"].ToString();
    int intReturn = access.Insert(Model);
    if (intReturn > 0)
    {
     using (AuditTrail Audit = new AuditTrail())
     {
      Audit.Insert(new AuditTrailModel()
      {
       UserID = Session["UserID"].ToString()
       ,Module = "Access"
       ,NewValues = "Added New Access: " + Model.AccessName + "\n" + ", Access Code: " + intReturn.ToString()
       ,IPAddress = SystemCore.GetIPAddress()
      });
     }
     response.ResponseStatus = SystemCore.ResponseStatus.SUCCESS;
     response.Description = "New Access has been successfully added.";
    }
    else
    {
     response.ResponseStatus = SystemCore.ResponseStatus.FAILED;
     response.Description = "Unable to save new access.";
    }
   }
   return Json(response, JsonRequestBehavior.DenyGet);
  }

  [HttpPost]
  public JsonResult DeleteAccess(List<string> pIDs)
  {
   JsonResult jsonReturn = new JsonResult();
   foreach (var item in pIDs)
   {
    jsonReturn = Json(new SYS_ACCESS().Delete(item.ToString()));
    using (AuditTrail audit = new AuditTrail())
    {
     audit.Insert(new AuditTrailModel()
     {
      UserID = Session["UserID"].ToString()
      ,Module = "Access"
      ,NewValues = "The Access Code " + item.ToString() + " is deleted. "
      ,IPAddress = SystemCore.GetIPAddress()
     });
    }
   }
   return Json(jsonReturn, JsonRequestBehavior.DenyGet);
  }

  [HttpPost]
  public JsonResult AccessSaveChanges(SYS_ACCESS_MODEL Model)
  {
   SystemCore.SystemResponse response = new SystemCore.SystemResponse();
   using (SYS_ACCESS access = new SYS_ACCESS())
   {
    Model.ModifiedBy = Session["UserID"].ToString();
    Model.ModifiedOn = DateTime.Now;
    SYS_ACCESS_MODEL oldModel = new SYS_ACCESS().Fill(Model.AccessCode);
    string[] arrDifferences = SystemCore.ToStringDifferences(new SystemCore().GetDiffirence(oldModel, Model)).Split(new char[] { '|' });
    if (access.Update(Model) > 0)
    {
     using (AuditTrail audit = new AuditTrail())
     {
      Model.ModifiedOn = DateTime.Now;
      Model.CreatedOn = oldModel.CreatedOn;
      audit.Insert(new AuditTrailModel()
      {
       UserID = Session["UserID"].ToString()
       ,Module = "Access"
       ,OldValues = "Edited Access Name[" + Model.AccessName.ToString() + "]," + "Changes:" + arrDifferences[0].ToString()
       ,NewValues = arrDifferences[1].ToString()
       ,IPAddress = SystemCore.GetIPAddress()
      });
     }
     response.ResponseStatus = SystemCore.ResponseStatus.SUCCESS;
     response.Description = "Access changes has been successfully saved.";
    }
    else
    {
     response.ResponseStatus = SystemCore.ResponseStatus.FAILED;
     response.Description = "Unable to save access changes.";
    }
   }

   return Json(response, JsonRequestBehavior.DenyGet);
  }

  [HttpPost]
  public JsonResult AccessProcessAction(List<int> IDs, bool pIsActivate)
  {
   SystemCore.SystemResponse response = new SystemCore.SystemResponse();
   int intReturn = new SYS_ACCESS().ProcessAction(IDs, pIsActivate, Session["UserID"].ToString());
   string strIDS = string.Join("\n", IDs.ToArray());
   if (intReturn > 0)
   {
    using (AuditTrail Audit = new AuditTrail())
    {
     Audit.Insert(new AuditTrailModel()
     {
      UserID = Session["UserID"].ToString()
      ,Module = "Access"
      ,NewValues = pIsActivate ? "Activated Access IDs: " + strIDS : "Deactivated Access: " + strIDS
      ,IPAddress = SystemCore.GetIPAddress()
     });
    }
    response.ResponseStatus = SystemCore.ResponseStatus.SUCCESS;
    response.Description = intReturn.ToString() + " has been " + (pIsActivate ? "activated" : "deactivated") + ".";
   }
   else
   {
    response.ResponseStatus = SystemCore.ResponseStatus.FAILED;
    response.Description = "Unable to " + (pIsActivate ? "activate" : "deactivate") + " selected item(s).";
   }
   return Json(response, JsonRequestBehavior.DenyGet); 
  }

 
  ///////////
  // USERS //
  ///////////
  public ActionResult Users()
  {
   ViewBag.AccessList = new SYS_ACCESS().GetList();
   return View(new SYS_USERS().GetList());
  }

  public ActionResult UserView(string CMSCode)
  {
   SYS_USERS_MODEL details = new SYS_USERS().Fill(CMSCode);
   ViewBag.AccessList = new SYS_ACCESS().GetList();
  
   return View(details);
  }

  public ActionResult UserEdit(string CMSCode)
  {
   SYS_USERS_MODEL details = new SYS_USERS().Fill(CMSCode);

   ViewBag.AccessList = new SYS_ACCESS().GetList();
 
   return View(details);

  }

  public ActionResult UserAddNew(SYS_USERS_MODEL model)
  {
   SYS_USERS_MODEL details = new SYS_USERS_MODEL();
   ViewBag.AccessList = new SYS_ACCESS().GetList();
   return View(details);
  }

  public JsonResult SearchEmployee(string pEmployeeID)
  {
   //var data = new JavaScriptSerializer().Deserialize(new SystemCore().GetCrossDomainResponse("https://cmpapp02.chinatrust.com.ph:447/Employee/Search/?pEmployeeID=" + pEmployeeID), null);
   CTBC.Network.ActiveDirectory ad = new CTBC.Network.ActiveDirectory("LDAP://chinatrust.com.ph", "Z_ESSDEV", "Welcome1");   
   //var data = new JavaScriptSerializer().Deserialize(ad.GetUserDetailsSingle(pEmployeeID).ToString(),null);
   var data = new JavaScriptSerializer().Serialize(ad.GetUserDetailsSingle(pEmployeeID));
   var data2 = new JavaScriptSerializer().Deserialize(data, null);
   return Json(data2, JsonRequestBehavior.AllowGet);
  }

  [HttpPost]
  public JsonResult UserProcessAction(List<string> IDs, bool pIsActivate)
  {
   SystemCore.SystemResponse response = new SystemCore.SystemResponse();
   int intReturn = new SYS_USERS().UserProcessAction(IDs, pIsActivate, Session["UserID"].ToString());
   string strIDs = string.Join("\n", IDs.ToArray());
   if (intReturn > 0)
   {
    using (AuditTrail audit = new AuditTrail())
    {
     audit.Insert(new AuditTrailModel()
     {
      UserID = Session["UserID"].ToString()
      ,Module = "Access"
      ,NewValues = pIsActivate ? "Activated Access User/s: " + strIDs : "Deactivate Access User/s: " + strIDs
      ,IPAddress = SystemCore.GetIPAddress()
     });
    }
    response.ResponseStatus = SystemCore.ResponseStatus.SUCCESS;
    response.Description = intReturn.ToString() + " has been " + (pIsActivate ? "activated" : "deactivated") + ".";
   }
   else
   {
    response.ResponseStatus = SystemCore.ResponseStatus.FAILED;
    response.Description = "Unable to " + (pIsActivate ? "activate" : "deactivate") + " selected item(s).";
   }
   return Json(response, JsonRequestBehavior.DenyGet);
  }

  [HttpPost]
  public JsonResult DeleteUser(List<string> pIDs)
  {
   JsonResult jsonReturn = new JsonResult();
   foreach (var item in pIDs)
   {
    jsonReturn = Json(new SYS_USERS().Delete(item.ToString()));
    using (AuditTrail audit = new AuditTrail())
    {
     audit.Insert(new AuditTrailModel()
     {
      UserID = Session["UserID"].ToString()
      ,Module = "Access"
      ,NewValues = "The User ID: " + item.ToString() + " is already deleted."
      ,IPAddress = SystemCore.GetIPAddress()
     });
    }
   }
   return Json(jsonReturn, JsonRequestBehavior.DenyGet);
  }

  [HttpPost]
  public JsonResult UserInsert(SYS_USERS_MODEL model)
  {
   SystemCore.SystemResponse response = new SystemCore.SystemResponse();
   model.CreatedBy = Session["UserID"].ToString();
   using (SYS_USERS users = new SYS_USERS())
   {
    //int intReturn = users.Insert(model);
    if (users.Insert(model) > 0)
    {
     using (AuditTrail audit = new AuditTrail())
     {
      audit.Insert(new AuditTrailModel()
      {
       UserID = Session["UserID"].ToString()
       ,Module = "Access"
       ,NewValues = "Added New User: " + model.FullName + "User ID:" + model.UserID
       ,IPAddress = SystemCore.GetIPAddress()
      });
     }
     response.ResponseStatus = SystemCore.ResponseStatus.SUCCESS;
     response.Description = "User Successfully Added";
    }
    else if (users.IsExist(model.UserID, model.Access_Code))
    {
     response.ResponseStatus = SystemCore.ResponseStatus.FAILED;
     response.Description = "User already exist.";
    }
    else
    {
     response.ResponseStatus = SystemCore.ResponseStatus.FAILED;
     response.Description = "Unable to save new user";
    }
   }
   return Json(response, JsonRequestBehavior.DenyGet);
  }

  [HttpPost]
  public ActionResult UserSaveChanges(SYS_USERS_MODEL model)
  {
  SystemCore.SystemResponse response = new SystemCore.SystemResponse();
   using (SYS_USERS access = new SYS_USERS())
   {
    model.ModifiedBy = Session["UserID"].ToString();
    model.ModifiedOn = DateTime.Now;
    SYS_USERS_MODEL oldModel = new SYS_USERS().Fill(model.UserID);
    string[] arrDifferences = SystemCore.ToStringDifferences(new SystemCore().GetDiffirence(oldModel, model)).Split(new char[] { '|' });
    if (access.Update(model) > 0)
    {
     using (AuditTrail audit = new AuditTrail())
     {
      model.ModifiedOn = DateTime.Now;
     model.CreatedOn = oldModel.CreatedOn;
      audit.Insert(new AuditTrailModel()
      {
       UserID = Session["UserID"].ToString()
       ,Module = "Access"
       ,OldValues = "User ID: " + model.UserID + "\n" + "Edited User Name[" + model.FullName.ToString() + "]" + "Changes:" + arrDifferences[0].ToString()
       ,NewValues = arrDifferences[1].ToString()
       ,IPAddress = SystemCore.GetIPAddress()
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
    return Json(response, JsonRequestBehavior.DenyGet);
  }

  [HttpPost]
  public ActionResult UserDelete()
  {
   return View();
  }


  ////////////
  // GROUPS //
  ////////////
  public ActionResult Groups()
  {
   return View(new SYS_GROUP().GetList());
  }

  public ActionResult GroupAddNew()
  {
   return View();
  }

  public ActionResult GroupView(int CMSCode)
  {
   return View(new SYS_GROUP().Fill(CMSCode));
  }

  public ActionResult GroupEdit(int CMSCode)
  {
   return View(new SYS_GROUP().Fill(CMSCode));
  }

  [HttpPost]
  public JsonResult DeleteGroup(string pIDs)
  {
   JsonResult jsonReturn = new JsonResult();
   foreach (var item in pIDs)
   {
    jsonReturn = Json(new SYS_GROUP().Delete(item.ToString()));
    using (AuditTrail audit = new AuditTrail())
    {
     audit.Insert(new AuditTrailModel()
     {
      UserID = Session["UserID"].ToString()
      ,Module = "Access"
      ,NewValues = "The Group Code " + item.ToString() + " is deleted. "
      ,IPAddress = SystemCore.GetIPAddress()
     });
    }
   }
   return Json(jsonReturn, JsonRequestBehavior.DenyGet);
  }

  [HttpPost]
  public JsonResult GroupInsert(SYS_GROUP_MODEL model)
  {
   SystemCore.SystemResponse response = new SystemCore.SystemResponse();
   model.CreatedBy = Session["UserID"].ToString();
   using (SYS_GROUP group = new SYS_GROUP())
   {
    int intReturn = group.Insert(model);
    if (intReturn > 0)
    {
     using (AuditTrail audit = new AuditTrail())
     {
      audit.Insert(new AuditTrailModel()
      {
       UserID = Session["UserID"].ToString()
       ,Module = "Access"
       ,NewValues = "Added New Group, Code: " + intReturn.ToString() + "\n" + " Access Group Name: " + model.Group_Name
       ,IPAddress = SystemCore.GetIPAddress()
      });
     }
     response.ResponseStatus = SystemCore.ResponseStatus.SUCCESS;
     response.Description = "Group Successfully added";
    }
    else
    {
     response.ResponseStatus = SystemCore.ResponseStatus.FAILED;
     response.Description = "Unable to add group";
    }
   }
   return Json(response, JsonRequestBehavior.DenyGet);
  }

  [HttpPost]
  public JsonResult GroupSaveChanges(SYS_GROUP_MODEL model)
  {
   SystemCore.SystemResponse response = new SystemCore.SystemResponse();
   model.ModifiedBy = Session["UserID"].ToString();
   using (SYS_GROUP group = new SYS_GROUP())
   {
    SYS_GROUP_MODEL oldmodel = new SYS_GROUP().Fill(model.Group_Code);
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
       ,Module = "Access"
       ,OldValues = "Edit Access Group [" + model.Group_Name.ToString() + "]" + "Changes:" + arrDifferences[0].ToString()
       ,NewValues = arrDifferences[1].ToString()
       ,IPAddress = SystemCore.GetIPAddress()
      });
     }
     response.ResponseStatus = SystemCore.ResponseStatus.SUCCESS;
     response.Description = "Group Successfully updated";
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
  public JsonResult GroupProcessAction(List<int> IDs, bool pIsActivate)
  {
   SystemCore.SystemResponse response = new SystemCore.SystemResponse();
   int intReturn = new SYS_GROUP().GroupProcessAction(IDs, pIsActivate, Session["UserID"].ToString());
   string strIDs = string.Join("\n", IDs.ToArray());
   if (intReturn > 0)
   {
    using (AuditTrail audit = new AuditTrail())
    {
     audit.Insert(new AuditTrailModel()
     {
      UserID = Session["UserID"].ToString()
      ,Module = "Access"
      ,NewValues = pIsActivate ? "Activate Access Group/s: " + strIDs : "Deactivate Access Group/s: " + strIDs
      ,IPAddress = SystemCore.GetIPAddress()
     });
    }
    response.ResponseStatus = SystemCore.ResponseStatus.SUCCESS;
    response.Description = intReturn.ToString() + " has been " + (pIsActivate ? "activated" : "deactivated") + ".";
   }
   else
   {
    response.ResponseStatus = SystemCore.ResponseStatus.FAILED;
    response.Description = "Unable to " + (pIsActivate ? "activate" : "deactivate") + " selected item(s).";
   }
   return Json(response, JsonRequestBehavior.DenyGet);
  }

  /////////////
  // MODULES //
  /////////////
  public ActionResult Modules()
  {
   return View();
  }

  [HttpPost]
  public ActionResult ModuleAddNew()
  {
   return View();
  }

  [HttpPost]
  public ActionResult ModuleEdit()
  {
   return View();
  }

  [HttpPost]
  public ActionResult ModuleDelete()
  {
   return View();
  }
 }
}
