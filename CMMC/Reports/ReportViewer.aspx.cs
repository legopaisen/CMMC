using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Reporting.WebForms;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using CTBC;
using System.Web.Security;
using CMMC.Models;

namespace CMMC.Reports
{
    public partial class ReportViewer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Request.IsAuthenticated)
                {
                    if (Request.QueryString.Count > 0)
                    {
                        if (Request.QueryString["ReportName"] != null)
                        {
                            string strReportName = Request.QueryString["ReportName"].ToString();

                            if (strReportName.Equals("AuditTrail"))
                            {
                                string strModule = Request.QueryString["Module"] != null ? Request.QueryString["Module"].ToString() : "";
                                DateTime? dtStartDate = Request.QueryString["StartDate"].ToString().ToDateTimeParse();
                                DateTime? dtEndDate = Request.QueryString["EndDate"].ToString().ToDateTimeParse();
                                rptReportViewer.LocalReport.DataSources.Add(new ReportDataSource("CMMC", this.GetAuditTrail(strModule, dtStartDate, dtEndDate)));
                                rptReportViewer.LocalReport.ReportPath = "Reports/Report/rptAuditTrail.rdl";
                                rptReportViewer.LocalReport.Refresh();
                                rptReportViewer.LocalReport.DisplayName = strModule + " Report";
                            }
                            else if (strReportName.Equals("User"))
                            {
                                string strModule = Request.QueryString["Module"] != null ? Request.QueryString["Module"].ToString() : "";
                                rptReportViewer.LocalReport.DataSources.Add(new ReportDataSource("CMMC", this.GetUser()));
                                rptReportViewer.LocalReport.ReportPath = "Reports/Report/rptUserList.rdl";
                                rptReportViewer.LocalReport.Refresh();
                                rptReportViewer.LocalReport.DisplayName = "User List Report";
                            }
                            else if (strReportName.Equals("ADBPenaltyReport"))
                            {
                                string strBranch = Request.QueryString["Branch"] != null ? Request.QueryString["Branch"] : "";
                                DateTime? dtStartDate = Request.QueryString["StartDate"].ToString().ToDateTimeParse();
                                DateTime? dtEndDate = Request.QueryString["EndDate"].ToString().ToDateTimeParse();
                                rptReportViewer.LocalReport.ReportPath = "Reports/Report/rptADBReport.rdl";
                                rptReportViewer.LocalReport.DisplayName = "Monthly ADB Penalty Report";
                                rptReportViewer.LocalReport.DataSources.Add(new ReportDataSource("CMMC", this.GetADBReport(strBranch, dtStartDate, dtEndDate)));
                                rptReportViewer.LocalReport.Refresh();
                            }
                            else if (strReportName.Equals("Approver"))
                            {
                                string strApprover = Request.QueryString["Approver"] != null ? Request.QueryString["Approver"] : "";
                                DateTime? dtStartDate = Request.QueryString["StartDate"].ToString().ToDateTimeParse();
                                DateTime? dtEndDate = Request.QueryString["EndDate"].ToString().ToDateTimeParse();
                                string strStatus = Request.QueryString["Status"] != null ? Request.QueryString["Status"] : "";
                                rptReportViewer.LocalReport.ReportPath = "Reports/Report/rptApprover.rdl";
                                rptReportViewer.LocalReport.DisplayName = "Approval Tracking";
                                rptReportViewer.LocalReport.DataSources.Add(new ReportDataSource("CMMC", this.GetApprove(strApprover, strStatus, dtStartDate, dtEndDate)));
                                rptReportViewer.LocalReport.Refresh();
                            }
                            else if (strReportName.Equals("Disapproved"))
                            {
                                string strApprover = Request.QueryString["Disapprover"] != null ? Request.QueryString["Disapprover"] : "";
                                DateTime? dtStartDate = Request.QueryString["StartDate"].ToString().ToDateTimeParse();
                                DateTime? dtEndDate = Request.QueryString["EndDate"].ToString().ToDateTimeParse();
                                rptReportViewer.LocalReport.ReportPath = "Reports/Report/rptDisapproved.rdl";
                                rptReportViewer.LocalReport.DisplayName = "Approval Tracking - Disapproved";
                                rptReportViewer.LocalReport.DataSources.Add(new ReportDataSource("CMMC", this.GetDisapproved(strApprover, dtStartDate, dtEndDate)));
                                rptReportViewer.LocalReport.Refresh();
                            }
                            else if (strReportName.Equals("CMSEnrollment"))
                            {
                                DateTime? dtStartDate = Request.QueryString["StartDate"].ToString().ToDateTimeParse();
                                DateTime? dtEndDate = Request.QueryString["EndDate"].ToString().ToDateTimeParse();
                                string CreatedBy = Request.QueryString["CreatedBy"] != null ? Request.QueryString["CreatedBy"] : "";
                                rptReportViewer.LocalReport.ReportPath = "Reports/Report/rptEnrollment.rdl";
                                rptReportViewer.LocalReport.DisplayName = "Enrollment";
                                rptReportViewer.LocalReport.DataSources.Add(new ReportDataSource("CMMC", this.GetEnrollment(CreatedBy, dtStartDate, dtEndDate)));
                                rptReportViewer.LocalReport.Refresh();
                            }
                            else if (strReportName.Equals("ATMWithdrawals"))
                            {
                                DateTime? dtStartDate = Request.QueryString["StartDate"].ToString().ToDateTimeParse();
                                DateTime? dtEndDate = Request.QueryString["EndDate"].ToString().ToDateTimeParse();
                                string strBranch = Request.QueryString["Branch"] != null ? Request.QueryString["Branch"] : "";
                                rptReportViewer.LocalReport.ReportPath = "Reports/Report/rptATMWithdrawals.rdlc";
                                rptReportViewer.LocalReport.DisplayName = "ATM Withdrawals";
                                rptReportViewer.LocalReport.DataSources.Add(new ReportDataSource("RptATMWithdrawalsDtSet", this.GetATMWithdrawals(strBranch, dtStartDate, dtEndDate)));
                                rptReportViewer.LocalReport.Refresh();
                            }
                            else if (strReportName.Equals("MTDADBReport"))
                            {
                                DateTime? dtStartDate = Request.QueryString["StartDate"].ToString().ToDateTimeParse();
                                DateTime? dtEndDate = Request.QueryString["EndDate"].ToString().ToDateTimeParse();
                                string CreatedBy = Request.QueryString["CreatedBy"] != null ? Request.QueryString["CreatedBy"] : "";
                                rptReportViewer.LocalReport.ReportPath = "Reports/Report/rptMTDADB.rdlc";
                                rptReportViewer.LocalReport.DisplayName = "MTD ADB";
                                rptReportViewer.LocalReport.DataSources.Add(new ReportDataSource("RptMTDADBDtSet", this.GetEnrollment(CreatedBy, dtStartDate, dtEndDate)));
                                rptReportViewer.LocalReport.Refresh();
                            }
                            else if (strReportName.Equals("ADBPerformanceReport"))
                            {
                                DateTime? dtStartDate = Request.QueryString["StartDate"].ToString().ToDateTimeParse();
                                DateTime? dtEndDate = Request.QueryString["EndDate"].ToString().ToDateTimeParse();
                                string CreatedBy = Request.QueryString["CreatedBy"] != null ? Request.QueryString["CreatedBy"] : "";
                                rptReportViewer.LocalReport.ReportPath = "Reports/Report/rptADBPerformanceReport.rdlc";
                                rptReportViewer.LocalReport.DisplayName = "ADB Performance";
                                rptReportViewer.LocalReport.DataSources.Add(new ReportDataSource("RptADBPerformanceDtSet", this.GetEnrollment(CreatedBy, dtStartDate, dtEndDate)));
                                rptReportViewer.LocalReport.Refresh();
                            }
                            else if (strReportName.Equals("MonthlyADBPenFee"))
                            {
                                DateTime? dtStartDate = Request.QueryString["StartDate"].ToString().ToDateTimeParse();
                                DateTime? dtEndDate = Request.QueryString["EndDate"].ToString().ToDateTimeParse();
                                string CreatedBy = Request.QueryString["CreatedBy"] != null ? Request.QueryString["CreatedBy"] : "";
                                rptReportViewer.LocalReport.ReportPath = "Reports/Report/rptMonthlyADBPenFee.rdlc";
                                rptReportViewer.LocalReport.DisplayName = "Monthly ADB Penalty Fee";
                                rptReportViewer.LocalReport.DataSources.Add(new ReportDataSource("RptMonthlyADBPenFeeDtSet", this.GetEnrollment(CreatedBy, dtStartDate, dtEndDate)));
                                rptReportViewer.LocalReport.Refresh();
                            }
                            else if (strReportName.Equals("ForeignCIFNumber"))
                            {
                                string strBranch = Request.QueryString["Branch"] != null ? Request.QueryString["Branch"] : "";
                                DateTime? dtStartDate = Request.QueryString["StartDate"].ToString().ToDateTimeParse();
                                DateTime? dtEndDate = Request.QueryString["EndDate"].ToString().ToDateTimeParse();
                                rptReportViewer.LocalReport.ReportPath = "Reports/Report/rptForeignCIFNumber.rdlc";
                                rptReportViewer.LocalReport.DisplayName = "Foreign CIF Number";
                                rptReportViewer.LocalReport.DataSources.Add(new ReportDataSource("RptForeignCIFNumberDtSet", this.GetForeignCIFNumberList(strBranch, dtStartDate, dtEndDate)));
                                rptReportViewer.LocalReport.Refresh();
                            }
                            else if (strReportName.Equals("OrphanAccounts"))
                            {
                                string strBranch = Request.QueryString["Branch"] != null ? Request.QueryString["Branch"] : "";
                                DateTime? dtStartDate = Request.QueryString["StartDate"].ToString().ToDateTimeParse();
                                DateTime? dtEndDate = Request.QueryString["EndDate"].ToString().ToDateTimeParse();
                                rptReportViewer.LocalReport.ReportPath = "Reports/Report/rptOrphanAccounts.rdlc";
                                rptReportViewer.LocalReport.DisplayName = "Orphan Accounts Exception";
                                rptReportViewer.LocalReport.DataSources.Add(new ReportDataSource("RptOrphanAccountsDtSet", this.GetOrphanAccounts(strBranch, dtStartDate, dtEndDate)));
                                rptReportViewer.LocalReport.Refresh();
                            }
                            else if (strReportName.Equals("AlienSubAccounts"))
                            {
                                string strBranch = Request.QueryString["Branch"] != null ? Request.QueryString["Branch"] : "";
                                DateTime? dtStartDate = Request.QueryString["StartDate"].ToString().ToDateTimeParse();
                                DateTime? dtEndDate = Request.QueryString["EndDate"].ToString().ToDateTimeParse();
                                rptReportViewer.LocalReport.ReportPath = "Reports/Report/rptAlienSubAccounts.rdlc";
                                rptReportViewer.LocalReport.DisplayName = "Alien Sub Accounts Exception";
                                rptReportViewer.LocalReport.DataSources.Add(new ReportDataSource("RptAlienSubAccountsDtSet", this.GetAlienSubAccounts(strBranch, dtStartDate, dtEndDate)));
                                rptReportViewer.LocalReport.Refresh();
                            }
                            else if (strReportName.Equals("CMSAccounts"))
                            {
                                DateTime? dtStartDate = Request.QueryString["StartDate"].ToString().ToDateTimeParse();
                                DateTime? dtEndDate = Request.QueryString["EndDate"].ToString().ToDateTimeParse();
                                string CreatedBy = Request.QueryString["CreatedBy"] != null ? Request.QueryString["CreatedBy"] : "";
                                rptReportViewer.LocalReport.ReportPath = "Reports/Report/rptCMSAccounts.rdlc";
                                rptReportViewer.LocalReport.DisplayName = "CMS Accounts";
                                rptReportViewer.LocalReport.DataSources.Add(new ReportDataSource("RptCMSAccountsDtSet", this.GetEnrollment(CreatedBy, dtStartDate, dtEndDate)));
                                rptReportViewer.LocalReport.Refresh();
                            }
                            else if (strReportName.Equals("SubAccountsCount"))
                            {
                                DateTime? dtStartDate = Request.QueryString["StartDate"].ToString().ToDateTimeParse();
                                DateTime? dtEndDate = Request.QueryString["EndDate"].ToString().ToDateTimeParse();
                                string CreatedBy = Request.QueryString["CreatedBy"] != null ? Request.QueryString["CreatedBy"] : "";
                                rptReportViewer.LocalReport.ReportPath = "Reports/Report/rptSubAccountsCount.rdlc";
                                rptReportViewer.LocalReport.DisplayName = "Sub Accounts Count";
                                rptReportViewer.LocalReport.DataSources.Add(new ReportDataSource("RptSubAccountsCountDtSet", this.GetEnrollment(CreatedBy, dtStartDate, dtEndDate)));
                                rptReportViewer.LocalReport.Refresh();
                            }
                            else if (strReportName.Equals("CustomerProfile"))
                            {
                                DateTime? dtStartDate = Request.QueryString["StartDate"].ToString().ToDateTimeParse();
                                DateTime? dtEndDate = Request.QueryString["EndDate"].ToString().ToDateTimeParse();
                                string CreatedBy = Request.QueryString["CreatedBy"] != null ? Request.QueryString["CreatedBy"] : "";
                                rptReportViewer.LocalReport.ReportPath = "Reports/Report/rptCustomerProfile.rdlc";
                                rptReportViewer.LocalReport.DisplayName = "Customer Profile";
                                rptReportViewer.LocalReport.DataSources.Add(new ReportDataSource("RptCustomerProfileDtSet", this.GetEnrollment(CreatedBy, dtStartDate, dtEndDate)));
                                rptReportViewer.LocalReport.Refresh();
                            }
                            else if (strReportName.Equals("CMSDeals"))
                            {
                                DateTime? dtStartDate = Request.QueryString["StartDate"].ToString().ToDateTimeParse();
                                DateTime? dtEndDate = Request.QueryString["EndDate"].ToString().ToDateTimeParse();
                                string CreatedBy = Request.QueryString["CreatedBy"] != null ? Request.QueryString["CreatedBy"] : "";
                                rptReportViewer.LocalReport.ReportPath = "Reports/Report/rptCMSDeals.rdlc";
                                rptReportViewer.LocalReport.DisplayName = "CMS Deals";
                                rptReportViewer.LocalReport.DataSources.Add(new ReportDataSource("RptCMSDealsDtSet", this.GetEnrollment(CreatedBy, dtStartDate, dtEndDate)));
                                rptReportViewer.LocalReport.Refresh();
                            }
                            else if (strReportName.Equals("ClosureCharges"))
                            {
                                DateTime? dtStartDate = Request.QueryString["StartDate"].ToString().ToDateTimeParse();
                                DateTime? dtEndDate = Request.QueryString["EndDate"].ToString().ToDateTimeParse();
                                string CreatedBy = Request.QueryString["CreatedBy"] != null ? Request.QueryString["CreatedBy"] : "";
                                rptReportViewer.LocalReport.ReportPath = "Reports/Report/rptClosureCharges.rdlc";
                                rptReportViewer.LocalReport.DisplayName = "Closure/Accumulated Charges";
                                rptReportViewer.LocalReport.DataSources.Add(new ReportDataSource("RptClosureChargesDtSet", this.GetEnrollment(CreatedBy, dtStartDate, dtEndDate)));
                                rptReportViewer.LocalReport.Refresh();
                            }
                            else if (strReportName.Equals("TotalADBRequirement"))
                            {
                                string strBranch = Request.QueryString["Branch"] != null ? Request.QueryString["Branch"] : "";
                                DateTime? dtStartDate = Request.QueryString["StartDate"].ToString().ToDateTimeParse();
                                DateTime? dtEndDate = Request.QueryString["EndDate"].ToString().ToDateTimeParse();
                                rptReportViewer.LocalReport.ReportPath = "Reports/Report/rptTotalADBRequirement.rdlc";
                                rptReportViewer.LocalReport.DisplayName = "Total ADB Requirement";
                                rptReportViewer.LocalReport.DataSources.Add(new ReportDataSource("RptTotalADBRequirementDtSet", this.GetTotalADBRequirement(strBranch, dtStartDate, dtEndDate)));
                                rptReportViewer.LocalReport.Refresh();
                            }
                            else if (strReportName.Equals("UncollectedCMSADBPenFee"))
                            {
                                DateTime? dtStartDate = Request.QueryString["StartDate"].ToString().ToDateTimeParse();
                                DateTime? dtEndDate = Request.QueryString["EndDate"].ToString().ToDateTimeParse();
                                string CreatedBy = Request.QueryString["CreatedBy"] != null ? Request.QueryString["CreatedBy"] : "";
                                rptReportViewer.LocalReport.ReportPath = "Reports/Report/rptUncollectedCMSADBPenFee.rdlc";
                                rptReportViewer.LocalReport.DisplayName = "Uncollected CMS ADB Penalty Fee";
                                rptReportViewer.LocalReport.DataSources.Add(new ReportDataSource("RptUncollectedCMSADBPenFeeDtSet", this.GetEnrollment(CreatedBy, dtStartDate, dtEndDate)));
                                rptReportViewer.LocalReport.Refresh();
                            }
                            else if (strReportName.Equals("InsufficientFundCMSADBPenFee"))
                            {
                                DateTime? dtStartDate = Request.QueryString["StartDate"].ToString().ToDateTimeParse();
                                DateTime? dtEndDate = Request.QueryString["EndDate"].ToString().ToDateTimeParse();
                                string CreatedBy = Request.QueryString["CreatedBy"] != null ? Request.QueryString["CreatedBy"] : "";
                                rptReportViewer.LocalReport.ReportPath = "Reports/Report/rptInsufficientFundCMSADBPenFee.rdlc";
                                rptReportViewer.LocalReport.DisplayName = "Insufficient Fund CMS ADB Penalty Fee";
                                rptReportViewer.LocalReport.DataSources.Add(new ReportDataSource("RptInsufficientFundCMSADBPenFeeDtSet", this.GetEnrollment(CreatedBy, dtStartDate, dtEndDate)));
                                rptReportViewer.LocalReport.Refresh();
                            }
                            else if (strReportName.Equals("NoLinkedAccounts"))
                            {
                                string strBranch = Request.QueryString["Branch"] != null ? Request.QueryString["Branch"] : "";
                                DateTime? dtStartDate = Request.QueryString["StartDate"].ToString().ToDateTimeParse();
                                DateTime? dtEndDate = Request.QueryString["EndDate"].ToString().ToDateTimeParse();
                                rptReportViewer.LocalReport.ReportPath = "Reports/Report/rptNoLinkedAccounts.rdlc";
                                rptReportViewer.LocalReport.DisplayName = "No Linked Accounts";
                                rptReportViewer.LocalReport.DataSources.Add(new ReportDataSource("RptNoLinkedAccountsDtSet", this.GetNoLinkedAccounts(strBranch, dtStartDate, dtEndDate)));
                                rptReportViewer.LocalReport.Refresh();
                            }
                            else if (strReportName.Equals("ClientAccountsType"))
                            {
                                string strClientType = Request.QueryString["ClientType"] != null ? Request.QueryString["ClientType"] : "";
                                DateTime? dtStartDate = Request.QueryString["StartDate"].ToString().ToDateTimeParse();
                                DateTime? dtEndDate = Request.QueryString["EndDate"].ToString().ToDateTimeParse();
                                rptReportViewer.LocalReport.ReportPath = "Reports/Report/rptClientAccountsType.rdlc";
                                rptReportViewer.LocalReport.DisplayName = "Client-Accounts Type";
                                rptReportViewer.LocalReport.DataSources.Add(new ReportDataSource("RptClientAccountsTypeDtSet", this.GetClientAccountsType(strClientType, dtStartDate, dtEndDate)));
                                rptReportViewer.LocalReport.Refresh();
                            }
                            else
                            {
                                Response.Redirect("/Home/Index");
                            }
                        }
                    }
                }
                else
                {
                    Response.Redirect("~/User/Login", true);
                }
            }
        }

        private DataTable GetAuditTrail(string pModule, DateTime? pStartDate = null, DateTime? pEndDate = null)
        {
            DataTable tblReturn = new DataTable("CMMC");
            List<Models.AuditTrailModel> audit = new Models.AuditTrail().GetAuditTrail(pModule, pStartDate, pEndDate);
            tblReturn = audit.ToDataTable();
            tblReturn.TableName = "CMMC";
            return tblReturn;
        }

        private DataTable GetATMWithdrawals(string pBranch, DateTime? pStartDate = null, DateTime? pEndDate = null)
        {
            DataTable tblReturn = new DataTable("CMMC");
            tblReturn = new Models.Reports.RptATMWithdrawals().GetATMWithdrawalsList(pBranch, pStartDate, pEndDate).ToDataTable();
            tblReturn.TableName = "CMMC";
            return tblReturn; 
        }
        private DataTable GetEnrollment(string pCreatedBy, DateTime? pStartDate = null, DateTime? pEndDate = null)
        {
            DataTable tblReturn = new DataTable("CMMC");
            tblReturn = new Models.RequestList().GetRequestListEnrollment(pCreatedBy, pStartDate, pEndDate);
            tblReturn.TableName = "CMMC";
            return tblReturn;
        }

        private DataTable GetADBReport(string pBranch, DateTime? pStartDate = null, DateTime? pEndDate = null)
        {
            DataTable tblReturn = new DataTable("CMMC");
            tblReturn = new Branches().GetADBReport(pBranch, pStartDate, pEndDate);//.ToDataTable();
            tblReturn.TableName = "CMMC";
            return tblReturn;
        }
        private DataTable GetNoLinkedAccounts(string pBranch, DateTime? pStartDate = null, DateTime? pEndDate = null)
        {
            DataTable tblReturn = new DataTable("CMMC");
            tblReturn = new Models.Reports.RptNoLinkedAccounts().GetNoLinkedAccountsList(pBranch, pStartDate, pEndDate).ToDataTable();
            tblReturn.TableName = "CMMC";
            return tblReturn;
        }
        private DataTable GetTotalADBRequirement(string pBranch, DateTime? pStartDate = null, DateTime? pEndDate = null)
        {
            DataTable tblReturn = new DataTable("CMMC");
            tblReturn = new Models.Reports.RptTotADBRequirement().GetTotADBRequirementList(pBranch, pStartDate, pEndDate).ToDataTable();
            tblReturn.TableName = "CMMC";
            return tblReturn;
        }
        private DataTable GetOrphanAccounts(string pBranch, DateTime? pStartDate = null, DateTime? pEndDate = null)
        {
            DataTable tblReturn = new DataTable("CMMC");
            tblReturn = new Models.Reports.RptOrphanAccounts().GetOrphanAccountsList(pBranch, pStartDate, pEndDate).ToDataTable();
            tblReturn.TableName = "CMMC";
            return tblReturn;
        }
        private DataTable GetAlienSubAccounts(string pBranch, DateTime? pStartDate = null, DateTime? pEndDate = null)
        {
            DataTable tblReturn = new DataTable("CMMC");
            tblReturn = new Models.Reports.RptAlienSubAccounts().GetAlienSubAccountsList(pBranch, pStartDate, pEndDate).ToDataTable();
            tblReturn.TableName = "CMMC";
            return tblReturn;
        }

        private DataTable GetClientAccountsType(string pClientsAccountType, DateTime? pStartDate = null, DateTime? pEndDate = null)
        {
            DataTable tblReturn = new DataTable("CMMC");
            tblReturn = new Models.Reports.RptClientAccountsType().GetClientAccountsList(pClientsAccountType, pStartDate, pEndDate).ToDataTable();
            tblReturn.TableName = "CMMC";
            return tblReturn;
        }
        private DataTable GetForeignCIFNumberList(string pBranch, DateTime? pStartDate = null, DateTime? pEndDate = null)
        {
            DataTable tblReturn = new DataTable("CMMC");
            tblReturn = new Models.Reports.RptForeignCIFNumber().GetForeignCIFNumberList(pBranch, pStartDate, pEndDate).ToDataTable();
            tblReturn.TableName = "CMMC";
            return tblReturn;
        }

        private DataTable GetApprove(string pApprover, string pstatus, DateTime? pStartDate = null, DateTime? pEndDate = null)
        {
            DataTable tblReturn = new DataTable("CMMC");
            tblReturn = new RequestList().GetApprove(pApprover, pstatus, pStartDate, pEndDate);//.ToDataTable();
            tblReturn.TableName = "CMMC";
            return tblReturn;
        }

        private DataTable GetDisapproved(string pApprover, DateTime? pStartDate = null, DateTime? pEndDate = null)
        {
            DataTable tblReturn = new DataTable("CMMC");
            tblReturn = new RequestList().GetDisapprove(pApprover, pStartDate, pEndDate);
            tblReturn.TableName = "CMMC";
            return tblReturn;
        }

        private DataTable GetUser()
        {
            DataTable tblReturn = new DataTable("CMMC");
            List<Models.UserListModel> user = new Models.AuditTrail().GetUserList();
            tblReturn = user.ToDataTable();
            tblReturn.TableName = "CMMC";
            return tblReturn;
        }
    }
}
