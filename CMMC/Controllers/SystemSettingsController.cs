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
    public class SystemSettingsController : Controller
    {
        public ActionResult Index()
        {
            Models.SharedFunctions.FileAndBatch filebatch = new Models.SharedFunctions().GetFileAndBatch();
            Models.SharedFunctions.ATMSystemConnection atmsystemconnection = new Models.SharedFunctions().GetATMDatabaseSettings();
            Models.SharedFunctions.FTPConSettings ftpconsettings = new Models.SharedFunctions().GetFTPConSettings();
            Models.SharedFunctions.NotificationSettings notificationsettings = new SharedFunctions().GetNoticationSettings();
            Models.SharedFunctions.ADBTransfer adbFile = new Models.SharedFunctions().GetADBTransferFile();
            Models.SharedFunctions.SettingsStruct conn = new Models.SharedFunctions().Connection();
            Models.SharedFunctions.Details details = new Models.SharedFunctions.Details();
            Models.SharedFunctions.ODSConnectionSetting odsconnectionsetting = new SharedFunctions().GetODSConnection();
            details.ATMSystemConnection = atmsystemconnection;
            details.FileAndBatch = filebatch;
            details.NotificationSettings = notificationsettings;
            details.ADBTransferFile = adbFile;
            details.FTPConSettings = ftpconsettings;
            details.ODSConnectionSetting = odsconnectionsetting;

            details.Settings = conn;

            return View(details);
        }

        [HttpPost]
        public JsonResult SaveADB(Models.SharedFunctions.ADBTransfer adbfiles)
        {
            new Models.SharedFunctions().SaveADBFile(adbfiles);
            using (CMMC.Models.AuditTrail audit = new Models.AuditTrail())
            {
                audit.Insert(new CMMC.Models.AuditTrailModel()
                {
                    UserID = Session["UserID"] == null ? "" : Session["UserID"].ToString()
                 ,
                    Module = "SystemSettings"
                 ,
                    NewValues = "ADB DTS Connection Settings saved successfully."
                 ,
                    IPAddress = SystemCore.GetIPAddress()
                });
            }
            return Json("success", JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult SaveNotificationSettings(Models.SharedFunctions.NotificationSettings notif)
        {
            new Models.SharedFunctions().SaveNotificationSettings(notif);
            using (CMMC.Models.AuditTrail audit = new Models.AuditTrail())
            {
                audit.Insert(new CMMC.Models.AuditTrailModel()
                {
                    UserID = Session["UserID"] == null ? "" : Session["UserID"].ToString()
                 ,
                    Module = "SystemSettings"
                 ,
                    NewValues = "Notification Settings saved successfully."
                 ,
                    IPAddress = SystemCore.GetIPAddress()
                });
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveFileAndBatch(Models.SharedFunctions.FileAndBatch fileandBatch)
        {
            new Models.SharedFunctions().SaveFileandBatch(fileandBatch);
            using (CMMC.Models.AuditTrail audit = new Models.AuditTrail())
            {
                audit.Insert(new CMMC.Models.AuditTrailModel()
                {
                    UserID = Session["UserID"] == null ? "" : Session["UserID"].ToString()
                 ,
                    Module = "SystemSettings"
                 ,
                    NewValues = "File and Batch Program Settings saved successfully."
                 ,
                    IPAddress = SystemCore.GetIPAddress()
                });
            }
            return Json("success", JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult SaveATMDatabaseSettings(Models.SharedFunctions.ATMSystemConnection atmcon)
        {
            new Models.SharedFunctions().SaveATMDatabaseConnection(atmcon);
            using (CMMC.Models.AuditTrail audit = new Models.AuditTrail())
            {
                audit.Insert(new CMMC.Models.AuditTrailModel()
                {
                    UserID = Session["UserID"] == null ? "" : Session["UserID"].ToString()
                 ,
                    Module = "SystemSettings"
                 ,
                    NewValues = "ATM System (Powercard) Database Connection Settings saved successfully."
                 ,
                    IPAddress = SystemCore.GetIPAddress()
                });
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveFTPConnectionSetting(Models.SharedFunctions.FTPConSettings ftp)
        {
            new Models.SharedFunctions().SaveFTPConSettings(ftp);
            using (CMMC.Models.AuditTrail audit = new Models.AuditTrail())
            {
                audit.Insert(new CMMC.Models.AuditTrailModel()
                {
                    UserID = Session["UserID"] == null ? "" : Session["UserID"].ToString()
                 ,
                    Module = "SystemSettings"
                 ,
                    NewValues = "FTP Connection Settings saved successfully."
                 ,
                    IPAddress = SystemCore.GetIPAddress()
                });
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveConnectionSetting(Models.SharedFunctions.SettingsStruct sett)
        {
            new Models.SharedFunctions().SaveConnectionSetting(sett);
            using (CMMC.Models.AuditTrail audit = new Models.AuditTrail())
            {
                audit.Insert(new CMMC.Models.AuditTrailModel()
                {
                    UserID = Session["UserID"] == null ? "" : Session["UserID"].ToString()
                 ,
                    Module = "SystemSettings"
                 ,
                    NewValues = "Connection Setting saved successfully"
                 ,
                    IPAddress = SystemCore.GetIPAddress()
                });
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
    }
}
