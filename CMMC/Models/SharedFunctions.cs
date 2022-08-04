using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CTBC;

namespace CMMC.Models
{
    public class SharedFunctions : IDisposable
    {
        public void Dispose() { GC.SuppressFinalize(this); }

        public struct Details
        {
            public SettingsStruct Settings { get; set; }
            public FileAndBatch FileAndBatch { get; set; } // File and Batch Program Settings
            public ATMSystemConnection ATMSystemConnection { get; set; }
            public ADBTransfer ADBTransferFile { get; set; }
            public FTPConSettings FTPConSettings { get; set; }
            public NotificationSettings NotificationSettings { get; set; }
            public ODSConnectionSetting ODSConnectionSetting { get; set; }
        }

        public enum Status
        {
            ForApproval = 1,
            Approved = 2,
            Disapproved = 3,
            Cancelled = 4
        }

        public static DateTime ServerDate
        {
            get { return DateTime.Today; }
        }

        public struct SettingsStruct
        {
            public string SQLServer { get; set; }
            public string SQLUserID { get; set; }
            public string SQLPassword { get; set; }
            public string SQLDatabase { get; set; }
            public string SecurityKey { get; set; } //
            public string UserID { get; set; }//
            public string Schedule { get; set; }//
            public string Fee { get; set; }//
            public string INVTYPE_MOTHER { get; set; }
            public string INVTYPE_CHILD { get; set; }
        }

        public struct OracleStruct
        {
            public string OracleServerIP { get; set; }
            public string OracleUserID { get; set; }
            public string OraclePassword { get; set; }
            public string OracleServicename { get; set; }
            public string OracleDB { get; set; }
            public int OraclePort { get; set; }
        }

        public struct FileAndBatch
        {
            public string FileName { get; set; }
            public string FileLoadPath { get; set; }
            public string FMTFilesPath { get; set; }
            public string InvvPath { get; set; }
            public string AcctPath { get; set; }
            public string CustPath { get; set; }
            public string ADBCasaPath { get; set; }
            public string BackUp { get; set; }
            public string BackUpPath { get; set; }
            public string BulkInsert { get; set; }
            public string DumpinInvvPath { get; set; }
            public string ProgramName { get; set; }
            public string FNSAdbFile { get; set; }
            public string ChargingFilePath { get; set; }
            public string UpdateAcct { get; set; }
            public string AcctOnePath { get; set; }
            public string UpdateCust { get; set; }
            public string CustOnePath { get; set; }
            public string MaintenanceSubject { get; set; }
            public string MaintenanceRecipient { get; set; }
        }

        public struct ATMSystemConnection
        {
            public string PowercardServer { get; set; }
            public string PowercardPort { get; set; }
            public string PowercardServiceName { get; set; }
            public string PowercardUserID { get; set; }
            public string PowercardPassword { get; set; }
        }

        public struct ADBTransfer
        {
            public string ADBDatabase { get; set; }
            public string DBID { get; set; }
            public string JobName { get; set; }
            public string ADBPassword { get; set; }
            public string ADBServer { get; set; }
        }

        public struct FTPConSettings
        {
            public string FTPIPAddress { get; set; }
            public string FTPTimeout { get; set; }
            public string FTPDestination { get; set; }
            public string FTPUserID { get; set; }
            public string FTPPassword { get; set; }
            public string FTPDestinationFileName { get; set; }
        }

        public struct NotificationSettings
        {
            public string SMPTPServer { get; set; }
            public string Sender { get; set; }
            public string SubjectError { get; set; }
            public string SubjectSuccess { get; set; }
            public string BodyError { get; set; }
            public string BodySuccess { get; set; }
            public string LogFilePath { get; set; }
            public string Email { get; set; }
            public string AccountEmailSubject { get; set; }
            public string AccountEmailRecipient { get; set; }
        }
        public struct ODSConnectionSetting
        {
            public string ODSServerIP { get; set; }
            public string ODSUserID { get; set; }
            public string ODSPassword { get; set; }
            public string ODSPort { get; set; }
            public string ODSServiceName { get; set; }
            public string ODSDatabase { get; set; }
        }

        //ODS Connection string
        public static string ODSConnectionString
        {
            get
            {
                SharedFunctions.ODSConnectionSetting ODSSetting = new SharedFunctions.ODSConnectionSetting();
                using (CTBC.Utility.Registry registry = new CTBC.Utility.Registry(CTBC.Utility.Registry.RegistryType.LOCAL_MACHINE, @"SOFTWARE\CTBC\CMMC"))
                {
                    CTBC.Cryptography.AES crypto = new CTBC.Cryptography.AES(registry.Read("SECURITY_Key"));
                    ODSSetting.ODSDatabase = registry.Read("ODS_DATABASE");
                    //ODSSetting.ODSPassword = crypto.Decrypt(registry.Read("ODS_PASSWORD"));
                    //ODSSetting.ODSUserID = crypto.Decrypt(registry.Read("ODS_USERID"));
                    ODSSetting.ODSPassword = registry.Read("ODS_PASSWORD");
                    ODSSetting.ODSUserID = registry.Read("ODS_USERID");
                    ODSSetting.ODSServerIP = registry.Read("ODS_SERVER");
                    ODSSetting.ODSPort = registry.Read("ODS_PORT");
                    ODSSetting.ODSServiceName = registry.Read("ODS_SERVICENAME");
                }
                return CTBC.SQL.ConnectionStringBuilder.Build(CTBC.SQLDatabases.MS_SQL, ODSSetting.ODSDatabase, ODSSetting.ODSPassword, ODSSetting.ODSUserID, ODSSetting.ODSServerIP, ODSSetting.ODSPort.ToInt());

            }
        }

        //ODS Connection Setting
        public ODSConnectionSetting GetODSConnection()
        {
            SharedFunctions.ODSConnectionSetting ODSSetting = new SharedFunctions.ODSConnectionSetting();
            using (CTBC.Utility.Registry registry = new CTBC.Utility.Registry(CTBC.Utility.Registry.RegistryType.LOCAL_MACHINE, @"SOFTWARE\CTBC\CMMC"))
            {
                CTBC.Cryptography.AES crypto = new CTBC.Cryptography.AES(registry.Read("SECURITY_Key"));
                ODSSetting.ODSDatabase = registry.Read("ODS_DATABASE");
                //ODSSetting.ODSPassword = crypto.Decrypt(registry.Read("ODS_PASSWORD"));
                //ODSSetting.ODSUserID = crypto.Decrypt(registry.Read("ODS_USERID"));
                ODSSetting.ODSPassword = registry.Read("ODS_PASSWORD");
                ODSSetting.ODSUserID = registry.Read("ODS_USERID");
                ODSSetting.ODSServerIP = registry.Read("ODS_SERVER");
                ODSSetting.ODSPort = registry.Read("ODS_PORT");
                ODSSetting.ODSServiceName = registry.Read("ODS_SERVICENAME");
            }
            return ODSSetting;
        }

        //Connection String Database
        public static string Connectionstring
        {
            get
            {
                SharedFunctions.SettingsStruct settings = new SharedFunctions.SettingsStruct();
                using (CTBC.Utility.Registry registry = new CTBC.Utility.Registry(CTBC.Utility.Registry.RegistryType.LOCAL_MACHINE, @"SOFTWARE\CTBC\CMMC"))
                {
                    CTBC.Cryptography.AES crypto = new CTBC.Cryptography.AES(registry.Read("SECURITY_Key"));
                    settings.SQLDatabase = registry.Read("SOURCE_DATABASE");
                    settings.SQLPassword = crypto.Decrypt(registry.Read("SOURCE_PASSWORD")); //password
                    settings.SQLUserID = crypto.Decrypt(registry.Read("SOURCE_DBID")); //cmp_user
                    settings.SQLServer = registry.Read("SOURCE_SERVER");
                }
                return CTBC.SQL.ConnectionStringBuilder.Build(CTBC.SQLDatabases.MS_SQL, settings.SQLDatabase, settings.SQLPassword, settings.SQLUserID, settings.SQLServer);
            }
        }

        //Connection String Oracle
        public static string OracleConnection
        {
            get
            {
                SharedFunctions.OracleStruct oracle = new OracleStruct();
                using (CTBC.Utility.Registry registry = new CTBC.Utility.Registry(CTBC.Utility.Registry.RegistryType.LOCAL_MACHINE, @"SOFTWARE\CTBC\CMMC"))
                {
                    //BANCS
                    oracle.OracleServerIP = registry.Read("ORACLE_SERVERIP");
                    oracle.OracleServicename = registry.Read("ORACLE_SERVICENAME");
                    oracle.OraclePort = registry.Read("ORACLE_PORT").ToInt();
                    oracle.OracleUserID = registry.Read("ORACLE_USERID");
                    oracle.OraclePassword = registry.Read("ORACLE_PASSWORD");
                    oracle.OracleDB = registry.Read("ORACLE_DB");
                }
                return CTBC.SQL.ConnectionStringBuilder.Build(CTBC.SQLDatabases.Oracle, oracle.OracleServerIP, oracle.OraclePort, "P012BAND", oracle.OracleUserID, oracle.OraclePassword);
            }
        }

        //Get file and batch
        public FileAndBatch GetFileAndBatch()
        {
            FileAndBatch fileandbatch = new FileAndBatch();
            using (CTBC.Utility.Registry registry = new CTBC.Utility.Registry(CTBC.Utility.Registry.RegistryType.LOCAL_MACHINE, @"SOFTWARE\CTBC\CMMC"))
            {
                fileandbatch.FileName = registry.Read("SOURCE_INVVFILENAME");
                fileandbatch.FileLoadPath = registry.Read("SOURCE_FILELOADPATH");
                fileandbatch.FMTFilesPath = registry.Read("SOURCE_FMTFilesPath");
                fileandbatch.InvvPath = registry.Read("SOURCE_INVVPath");
                fileandbatch.AcctPath = registry.Read("SOURCE_ACCTPath");
                fileandbatch.CustPath = registry.Read("SOURCE_CUSTPath");
                fileandbatch.ADBCasaPath = registry.Read("SOURCE_ADBCASAPath");
                fileandbatch.BackUp = registry.Read("SOURCE_BACKUP");
                fileandbatch.BackUpPath = registry.Read("SOURCE_BACKUPPATH");
                fileandbatch.BulkInsert = registry.Read("SOURCE_BULKINSERTFOLDER");
                fileandbatch.DumpinInvvPath = registry.Read("SOURCE_DUMPINVVPATH");
                fileandbatch.ProgramName = registry.Read("SOURCE_PROGRAMNAME");
                fileandbatch.FNSAdbFile = registry.Read("SOURCE_FNSADBFILE");
                fileandbatch.ChargingFilePath = registry.Read("TARGET_CHARGINGFILEPATH");
                fileandbatch.UpdateAcct = registry.Read("MONTHLYUPDATE_UPDATEACCT");
                fileandbatch.AcctOnePath = registry.Read("MONTHLYUPDATE_ACCTONEPATH");
                fileandbatch.UpdateCust = registry.Read("MONTHLYUPDATE_UPDATECUST");
                fileandbatch.CustOnePath = registry.Read("MONTHLYUPDATE_CUSTONEPATH");
                fileandbatch.MaintenanceSubject = registry.Read("MAINTENANCEEMAIL_SUBJECT");
                fileandbatch.MaintenanceRecipient = registry.Read("MAINTENANCEEMAIL_RECIPIENTS");
            }
            return fileandbatch;
        }

        //Get ADB Transfer File
        public ADBTransfer GetADBTransferFile()
        {
            ADBTransfer adbtransfer = new ADBTransfer();
            using (CTBC.Utility.Registry registry = new CTBC.Utility.Registry(CTBC.Utility.Registry.RegistryType.LOCAL_MACHINE, @"SOFTWARE\CTBC\CMMC"))
            {
                CTBC.Cryptography.AES crypto = new CTBC.Cryptography.AES(registry.Read("SECURITY_Key"));

                adbtransfer.ADBDatabase = registry.Read("ADBTRANSFER_DATABASE");
                adbtransfer.DBID = crypto.Decrypt(registry.Read("ADBTRANSFER_DBID"));
                adbtransfer.JobName = registry.Read("ADBTRANSFER_JOBNAME");
                adbtransfer.ADBPassword = crypto.Decrypt(registry.Read("ADBTRANSFER_PASSWORD"));
                adbtransfer.ADBServer = registry.Read("ADBTRANSFER_SERVER");
            }
            return adbtransfer;
        }

        //GET FTP
        public FTPConSettings GetFTPConSettings()
        {
            FTPConSettings ftpsettings = new FTPConSettings();
            using (CTBC.Utility.Registry registry = new CTBC.Utility.Registry(CTBC.Utility.Registry.RegistryType.LOCAL_MACHINE, @"SOFTWARE\CTBC\CMMC"))
            {
                CTBC.Cryptography.AES crypto = new CTBC.Cryptography.AES(registry.Read("SECURITY_Key"));
                ftpsettings.FTPIPAddress = registry.Read("FTP_IPADDRESS");
                ftpsettings.FTPTimeout = registry.Read("FTP_TIMEOUT");
                ftpsettings.FTPDestination = registry.Read("FTP_DESTINATION");
                ftpsettings.FTPUserID = crypto.Decrypt(registry.Read("FTP_USERID"));
                ftpsettings.FTPPassword = crypto.Decrypt(registry.Read("FTP_PASSWORD"));
                ftpsettings.FTPDestinationFileName = registry.Read("FTP_DESTINATIONFILENAME");
            }
            return ftpsettings;
        }

        //GET Notification Settings
        public NotificationSettings GetNoticationSettings()
        {
            NotificationSettings notificationsettings = new NotificationSettings();
            using (CTBC.Utility.Registry registry = new CTBC.Utility.Registry(CTBC.Utility.Registry.RegistryType.LOCAL_MACHINE, @"SOFTWARE\CTBC\CMMC"))
            {
                notificationsettings.SMPTPServer = registry.Read("NOTIFICATION_SMTPSERVER");
                notificationsettings.Sender = registry.Read("NOTIFICATION_SENDER");
                notificationsettings.SubjectError = registry.Read("NOTIFICATION_SUBJECTERROR");
                notificationsettings.SubjectSuccess = registry.Read("NOTIFICATION_SUBJECTSUCCESS");
                notificationsettings.BodyError = registry.Read("NOTIFICATION_BODYERROR");
                notificationsettings.BodySuccess = registry.Read("NOTIFICATION_BODYSUCCESS");
                notificationsettings.LogFilePath = registry.Read("NOTIFICATION_LOGFILEPATH");
                notificationsettings.Email = registry.Read("DEFAULTVALUES_Email");
                notificationsettings.AccountEmailRecipient = registry.Read("ACCOUNTUPDATEEMAIL_RECIPIENTS");
                notificationsettings.AccountEmailSubject = registry.Read("ACCOUNTUPDATEEMAIL_SUBJECT");
            }
            return notificationsettings;
        }

        //Get Destination File
        public ATMSystemConnection GetATMDatabaseSettings()
        {
            ATMSystemConnection atmsystemconnection = new ATMSystemConnection();
            using (CTBC.Utility.Registry registry = new CTBC.Utility.Registry(CTBC.Utility.Registry.RegistryType.LOCAL_MACHINE, @"SOFTWARE\CTBC\CMMC"))
            {
                atmsystemconnection.PowercardServer = registry.Read("POWERCARD_SERVER");
                atmsystemconnection.PowercardPort = registry.Read("POWERCARD_PORT");
                atmsystemconnection.PowercardServiceName = registry.Read("POWERCARD_SERVICENAME");
                atmsystemconnection.PowercardUserID = registry.Read("POWERCARD_USERID");
                atmsystemconnection.PowercardPassword = registry.Read("POWERCARD_PASSWORD");
            }
            return atmsystemconnection;
        }

        //Get Connection SQL Server Setting
        public SettingsStruct Connection()
        {
            SettingsStruct sett = new SettingsStruct();
            using (CTBC.Utility.Registry registry = new CTBC.Utility.Registry(CTBC.Utility.Registry.RegistryType.LOCAL_MACHINE, @"SOFTWARE\CTBC\CMMC"))
            {
                CTBC.Cryptography.AES crypto = new CTBC.Cryptography.AES(registry.Read("SECURITY_Key"));
                sett.SQLDatabase = registry.Read("SOURCE_DATABASE");
                sett.SQLPassword = crypto.Decrypt(registry.Read("SOURCE_PASSWORD"));
                sett.SQLServer = registry.Read("SOURCE_SERVER");
                sett.SQLUserID = crypto.Decrypt(registry.Read("SOURCE_DBID"));
                sett.SecurityKey = registry.Read("SECURITY_Key");
                sett.UserID = registry.Read("USER_USERID");
                sett.Schedule = registry.Read("SCHEDULE_EVERYDAY");
                sett.Fee = registry.Read("DEFAULTVALUES_WithdrawalFee");
                sett.INVTYPE_MOTHER = registry.Read("INVTYPE_MOTHER");
                sett.INVTYPE_CHILD = registry.Read("INVTYPE_CHILD");
            }
            return sett;
        }

        // Save the Source File
        public void SaveFileandBatch(FileAndBatch fileandBatch)
        {
            using (CTBC.Utility.Registry registry = new CTBC.Utility.Registry(CTBC.Utility.Registry.RegistryType.LOCAL_MACHINE, @"SOFTWARE\CTBC\CMMC"))
            {
                registry.Write("SOURCE_INVVFILENAME", fileandBatch.FileName);
                registry.Write("SOURCE_FILELOADPATH", fileandBatch.FileLoadPath);
                registry.Write("SOURCE_FMTFilesPath", fileandBatch.FMTFilesPath);
                registry.Write("SOURCE_INVVPath", fileandBatch.InvvPath);
                registry.Write("SOURCE_ACCTPath", fileandBatch.AcctPath);
                registry.Write("SOURCE_CUSTPath", fileandBatch.CustPath);
                registry.Write("SOURCE_ADBCASAPath", fileandBatch.ADBCasaPath);
                registry.Write("SOURCE_BACKUP", fileandBatch.BackUp);
                registry.Write("SOURCE_BACKUPPATH", fileandBatch.BackUpPath);
                registry.Write("SOURCE_BULKINSERTFOLDER", fileandBatch.BulkInsert);
                registry.Write("SOURCE_DUMPINVVPATH", fileandBatch.DumpinInvvPath);
                registry.Write("SOURCE_PROGRAMNAME", fileandBatch.ProgramName);
                registry.Write("SOURCE_FNSADBFILE", fileandBatch.FNSAdbFile);
                registry.Write("TARGET_CHARGINGFILEPATH", fileandBatch.ChargingFilePath);
                registry.Write("MONTHLYUPDATE_UPDATEACCT", fileandBatch.UpdateAcct);
                registry.Write("MONTHLYUPDATE_ACCTONEPATH", fileandBatch.AcctOnePath);
                registry.Write("MONTHLYUPDATE_UPDATECUST", fileandBatch.UpdateCust);
                registry.Write("MONTHLYUPDATE_CUSTONEPATH", fileandBatch.CustOnePath);
                registry.Write("MAINTENANCEEMAIL_SUBJECT", fileandBatch.MaintenanceSubject);
                registry.Write("MAINTENANCEEMAIL_RECIPIENTS", fileandBatch.MaintenanceRecipient);
            }
        }

        //Save Destination File
        public void SaveATMDatabaseConnection(ATMSystemConnection atmcon)
        {
            using (CTBC.Utility.Registry registry = new CTBC.Utility.Registry(CTBC.Utility.Registry.RegistryType.LOCAL_MACHINE, @"SOFTWARE\CTBC\CMMC"))
            {
                registry.Write("POWERCARD_SERVER", atmcon.PowercardServer);
                registry.Write("POWERCARD_PORT", atmcon.PowercardPort);
                registry.Write("POWERCARD_SERVICENAME", atmcon.PowercardServiceName);
                registry.Write("POWERCARD_USERID", atmcon.PowercardUserID);
                registry.Write("POWERCARD_PASSWORD", atmcon.PowercardPassword);
            }
        }

        //Save ADB File
        public void SaveADBFile(ADBTransfer adb)
        {
            using (CTBC.Utility.Registry registry = new CTBC.Utility.Registry(CTBC.Utility.Registry.RegistryType.LOCAL_MACHINE, @"SOFTWARE\CTBC\CMMC"))
            {
                CTBC.Cryptography.AES crypto = new CTBC.Cryptography.AES(registry.Read("SECURITY_Key"));

                registry.Write("ADBTRANSFER_DATABASE", adb.ADBDatabase);
                registry.Write("ADBTRANSFER_DBID", crypto.Encrypt(adb.DBID));
                registry.Write("ADBTRANSFER_JOBNAME", adb.JobName);
                registry.Write("ADBTRANSFER_PASSWORD", crypto.Encrypt(adb.ADBPassword));
                registry.Write("ADBTRANSFER_SERVER", adb.ADBServer);
            }
        }

        //Save Notification Settings
        public void SaveNotificationSettings(NotificationSettings notif)
        {
            using (CTBC.Utility.Registry registry = new CTBC.Utility.Registry(CTBC.Utility.Registry.RegistryType.LOCAL_MACHINE, @"SOFTWARE\CTBC\CMMC"))
            {
                registry.Write("NOTIFICATION_SMTPSERVER", notif.SMPTPServer);
                registry.Write("NOTIFICATION_SENDER", notif.Sender);
                registry.Write("NOTIFICATION_SUBJECTERROR", notif.SubjectError);
                registry.Write("NOTIFICATION_SUBJECTSUCCESS", notif.SubjectSuccess);
                registry.Write("NOTIFICATION_BODYERROR", notif.BodyError);
                registry.Write("NOTIFICATION_BODYSUCCESS", notif.BodySuccess);
                registry.Write("NOTIFICATION_LOGFILEPATH", notif.LogFilePath);
                registry.Write("DEFAULTVALUES_Email", notif.Email);
                registry.Write("ACCOUNTUPDATEEMAIL_SUBJECT", notif.AccountEmailSubject);
                registry.Write("ACCOUNTUPDATEEMAIL_RECIPIENTS", notif.AccountEmailRecipient);
            }
        }

        //Save FTP Connection Settings
        public void SaveFTPConSettings(FTPConSettings ftp)
        {
            using (CTBC.Utility.Registry registry = new CTBC.Utility.Registry(CTBC.Utility.Registry.RegistryType.LOCAL_MACHINE, @"SOFTWARE\CTBC\CMMC"))
            {
                CTBC.Cryptography.AES crypto = new CTBC.Cryptography.AES(registry.Read("SECURITY_Key"));
                registry.Write("FTP_IPADDRESS", ftp.FTPIPAddress);
                registry.Write("FTP_TIMEOUT", ftp.FTPTimeout);
                registry.Write("FTP_DESTINATION", ftp.FTPDestination);
                registry.Write("FTP_USERID", crypto.Encrypt(ftp.FTPUserID));
                registry.Write("FTP_PASSWORD", crypto.Encrypt(ftp.FTPPassword));
                registry.Write("FTP_DESTINATIONFILENAME", ftp.FTPDestinationFileName);
            }
        }

        //Save Connection Settings
        public void SaveConnectionSetting(SettingsStruct sett)
        {
            using (CTBC.Utility.Registry registry = new CTBC.Utility.Registry(CTBC.Utility.Registry.RegistryType.LOCAL_MACHINE, @"SOFTWARE\CTBC\CMMC"))
            {
                CTBC.Cryptography.AES crypto = new CTBC.Cryptography.AES(registry.Read("SECURITY_Key"));
                registry.Write("SOURCE_SERVER", sett.SQLServer);
                registry.Write("SOURCE_DATABASE", sett.SQLDatabase);
                registry.Write("SOURCE_DBID", crypto.Encrypt(sett.SQLUserID)); //
                registry.Write("SOURCE_PASSWORD", crypto.Encrypt(sett.SQLPassword)); //
                registry.Write("SECURITY_Key", sett.SecurityKey);
                registry.Write("USER_USERID", sett.UserID);
                registry.Write("SCHEDULE_EVERYDAY", sett.Schedule);
                registry.Write("DEFAULTVALUES_WithdrawalFee", sett.Fee);
                registry.Write("INVTYPE_MOTHER", sett.INVTYPE_MOTHER);
                registry.Write("INVTYPE_CHILD", sett.INVTYPE_CHILD);
            }
        }
    }
}