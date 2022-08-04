using Newtonsoft.Json;
using SYS_MATRIX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Data.SqlClient;
using CTBC.Network;


namespace CMMC.Controllers
{
    [OutputCache(Duration = 0, NoStore = true, VaryByParam = "*")]
    public class LoginController : Controller
    {
        //
        // GET: /Login/

        public ActionResult Index()
        {
            if (new SYS_USERS().GetCount().Count == 0)//CHECK IF WE HAVE USERS IN THE SYS_USERS TABLE. REDIRECT TO /Access/Users/New
            {
                Session["UserID"] = "SYSTEM";
                Session["IdleMinutes"] = 2880;
                Session["GroupName"] = "Unspecified";
                Session["FullName"] = "Unspecified";
                Session["UserTypeText"] = "Unspecified";
                return RedirectToAction("UserAddNew", "Access");
            }

            if (Request.IsAuthenticated && Session["UserID"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                FormsAuthentication.SignOut();
            }

            ViewBag.CryptoHashCode = new SystemCore().CreateSessionHash();
            ViewBag.Response = TempData["Error"];
            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult SignOn()
        {
            if (ModelState.IsValid)
            {
            }
            SystemCore.SystemResponse response = new SystemCore.SystemResponse();
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                string strNetworkID = new SystemCore().DecryptStringAES(Request["hdnUserIDEncrypted"].ToString(), Request["HashCode"].ToString());
                string strPassword = new SystemCore().DecryptStringAES(Request["hdnPasswordEncrypted"].ToString(), Request["HashCode"].ToString());

                //List<Models.SystemParametersModel> param = new Models.SystemParameters().GetList("LDAP");
                CTBC.Cryptography.AES crypto = new CTBC.Cryptography.AES(SystemCore.SecurityKey);

                string strLDAPPath = "LDAP://chinatrust.com.ph"; //param.Single(x => x.ParameterName.Equals("LDAPPath")).ParameterValue;
                string strLDAPUsername = "57kfZnu8PbRQDLMcXz+EJg=="; // param.Single(x => x.ParameterName.Equals("UserID")).ParameterValue;
                string strLDAPPassword = "PrMYssXe9/K59cERN3IMeA=="; // param.Single(x => x.ParameterName.Equals("Password")).ParameterValue;

                if (new SYS_USERS().GetList().Where(x => x.UserID.Equals(strNetworkID) && x.IsActive).ToList().Count > 0)
                {
                    ActiveDirectory ad = new ActiveDirectory(strLDAPPath, crypto.Decrypt(strLDAPUsername), crypto.Decrypt(strLDAPPassword));
                    if (ad.ErrorException == null)
                    {
                        ActiveDirectory.UserDetails details = ad.GetUserDetailsSingle(strNetworkID);
                        if (!details.IsLockout && !details.IsAccountExpired && !details.IsAccountDisabled)
                        {
                            if (!CTBC.Network.Credential.Logon(strNetworkID, "CTCBPH_GL2", strPassword))
                            {
                                response.ResponseStatus = SystemCore.ResponseStatus.FAILED;
                                response.Description = "Authentication Failed.";
                            }
                            else
                            {
                                SYS_USERS_MODEL user = new SYS_USERS().Fill(strNetworkID);
                                Session["UserID"] = strNetworkID;
                                Session["Fullname"] = user.FullName;
                                Session["Email"] = user.Email;
                                Session["Access_Code"] = user.Access_Code;
                                Session["GroupCode"] = user.GroupCode;
                                Session["DepartmentCode"] = user.DepartmentCode;
                                Session["Unit_Code"] = user.UnitCode;
                                Session["IsGroupHead"] = user.IsGroupHead;
                                Session["IsDepartmentHead"] = user.IsDepartmentHead;
                                Session["IsUnitHead"] = user.IsUnitHead;
                                Session["UserType"] = user.UserType;
                                var modulelist = JsonConvert.SerializeObject(new SYS_ACCESS_MODS().GetList(user.Access_Code));
                                Session["ModuleList"] = new JavaScriptSerializer().Deserialize(modulelist.ToString(), null);
                                new SYS_USERS().UpdateLastLogOn(strNetworkID, user.Access_Code);
                                FormsAuthentication.Authenticate(strNetworkID, strPassword);
                                FormsAuthentication.RedirectFromLoginPage(strNetworkID, false);
                                response.ResponseStatus = SystemCore.ResponseStatus.SUCCESS;
                                response.Description = "Log-in Successfully";
                            }
                        }
                        else
                        {
                            // if user account is locked expired or disabled
                            response.ResponseStatus = SystemCore.ResponseStatus.FAILED;
                            response.Description = "Network ID is locked out. Please contact System Administrator.";
                        }
                    }
                    else
                    {//if active directory error
                        response.ResponseStatus = SystemCore.ResponseStatus.FAILED;
                        response.Description = "LDAP Error: " + ad.ErrorException.Message.Replace("\r\n", "");
                    }
                }//if user is not on cmmc user list
                else
                {
                    response.ResponseStatus = SystemCore.ResponseStatus.FAILED;
                    response.Description = "User ID is not enrolled in the system.";
                }

                using (CMMC.Models.AuditTrail audit = new Models.AuditTrail())
                {
                    audit.Insert(new CMMC.Models.AuditTrailModel()
                    {
                        UserID = strNetworkID
                     ,
                        Module = "LogIn"
                     ,
                        NewValues = response.Description
                     ,
                        IPAddress = SystemCore.GetIPAddress()
                    });
                }
                TempData["Error"] = response;
                return RedirectToAction("Index", "Login");
            }
        }

        [Authorize]
        public ActionResult SignOut()
        {
            using (CMMC.Models.AuditTrail audit = new Models.AuditTrail())
            {
                audit.Insert(new CMMC.Models.AuditTrailModel()
                {
                    UserID = Session["UserID"] == null ? "" : Session["UserID"].ToString()
                 ,
                    Module = "LogIn"
                 ,
                    NewValues = "Signed out"
                 ,
                    IPAddress = SystemCore.GetIPAddress()
                });
            }
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Index", "Login");
        }

        [Authorize]
        public JsonResult SessionTimeOut()
        {
            using (CMMC.Models.AuditTrail audit = new Models.AuditTrail())
            {
                audit.Insert(new CMMC.Models.AuditTrailModel()
                {
                    UserID = Session["UserID"] == null ? "" : Session["UserID"].ToString()
                 ,
                    Module = "LogIn"
                 ,
                    NewValues = "Session timeout due to inactivity."
                 ,
                    IPAddress = SystemCore.GetIPAddress()
                });
            }
            Session.Abandon();
            return Json("0", JsonRequestBehavior.DenyGet);
        }

        [Authorize]
        public JsonResult ForceLogout()
        {
            using (CMMC.Models.AuditTrail audit = new Models.AuditTrail())
            {
                audit.Insert(new CMMC.Models.AuditTrailModel()
                {
                    UserID = Session["UserID"] == null ? "" : Session["UserID"].ToString()
                 ,
                    Module = "LogIn"
                 ,
                    NewValues = "Force logout due to multiple login session."
                 ,
                    IPAddress = SystemCore.GetIPAddress()
                });
            }
            Session.Abandon();
            return Json("0", JsonRequestBehavior.DenyGet);
        }
    }
}
