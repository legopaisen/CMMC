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
                                string CreatedBy = Request.QueryString["CreatedBy"] != null ? Request.QueryString["CreatedBy"] : "";
                                rptReportViewer.LocalReport.ReportPath = "Reports/Report/rptATMWithdrawals.rdl";
                                rptReportViewer.LocalReport.DisplayName = "ATM Withdrawals";
                                rptReportViewer.LocalReport.DataSources.Add(new ReportDataSource("CMMC", this.GetEnrollment(CreatedBy, dtStartDate, dtEndDate)));
                                rptReportViewer.LocalReport.Refresh();
                            }
                            else if (strReportName.Equals("MTDADBReport"))
                            {
                                DateTime? dtStartDate = Request.QueryString["StartDate"].ToString().ToDateTimeParse();
                                DateTime? dtEndDate = Request.QueryString["EndDate"].ToString().ToDateTimeParse();
                                string CreatedBy = Request.QueryString["CreatedBy"] != null ? Request.QueryString["CreatedBy"] : "";
                                rptReportViewer.LocalReport.ReportPath = "Reports/Report/rptMTDADBReport.rdl";
                                rptReportViewer.LocalReport.DisplayName = "MTD ADB";
                                rptReportViewer.LocalReport.DataSources.Add(new ReportDataSource("CMMC", this.GetEnrollment(CreatedBy, dtStartDate, dtEndDate)));
                                rptReportViewer.LocalReport.Refresh();
                            }
                            else if (strReportName.Equals("ADBPerformanceReport"))
                            {
                                DateTime? dtStartDate = Request.QueryString["StartDate"].ToString().ToDateTimeParse();
                                DateTime? dtEndDate = Request.QueryString["EndDate"].ToString().ToDateTimeParse();
                                string CreatedBy = Request.QueryString["CreatedBy"] != null ? Request.QueryString["CreatedBy"] : "";
                                rptReportViewer.LocalReport.ReportPath = "Reports/Report/rptADBPerformanceReport.rdl";
                                rptReportViewer.LocalReport.DisplayName = "ADB Performance";
                                rptReportViewer.LocalReport.DataSources.Add(new ReportDataSource("CMMC", this.GetEnrollment(CreatedBy, dtStartDate, dtEndDate)));
                                rptReportViewer.LocalReport.Refresh();
                            }
                            else if (strReportName.Equals("MonthlyADBPenFee"))
                            {
                                DateTime? dtStartDate = Request.QueryString["StartDate"].ToString().ToDateTimeParse();
                                DateTime? dtEndDate = Request.QueryString["EndDate"].ToString().ToDateTimeParse();
                                string CreatedBy = Request.QueryString["CreatedBy"] != null ? Request.QueryString["CreatedBy"] : "";
                                rptReportViewer.LocalReport.ReportPath = "Reports/Report/rptMonthlyADBPenFee.rdl";
                                rptReportViewer.LocalReport.DisplayName = "Monthly ADB Penalty Fee";
                                rptReportViewer.LocalReport.DataSources.Add(new ReportDataSource("CMMC", this.GetEnrollment(CreatedBy, dtStartDate, dtEndDate)));
                                rptReportViewer.LocalReport.Refresh();
                            }
                            else if (strReportName.Equals("ForeignCIFNumber"))
                            {
                                DateTime? dtStartDate = Request.QueryString["StartDate"].ToString().ToDateTimeParse();
                                DateTime? dtEndDate = Request.QueryString["EndDate"].ToString().ToDateTimeParse();
                                string CreatedBy = Request.QueryString["CreatedBy"] != null ? Request.QueryString["CreatedBy"] : "";
                                rptReportViewer.LocalReport.ReportPath = "Reports/Report/rptForeignCIFNumber.rdl";
                                rptReportViewer.LocalReport.DisplayName = "Foreign CIF Number";
                                rptReportViewer.LocalReport.DataSources.Add(new ReportDataSource("CMMC", this.GetEnrollment(CreatedBy, dtStartDate, dtEndDate)));
                                rptReportViewer.LocalReport.Refresh();
                            }
                            else if (strReportName.Equals("OprhanAccounts"))
                            {
                                DateTime? dtStartDate = Request.QueryString["StartDate"].ToString().ToDateTimeParse();
                                DateTime? dtEndDate = Request.QueryString["EndDate"].ToString().ToDateTimeParse();
                                string CreatedBy = Request.QueryString["CreatedBy"] != null ? Request.QueryString["CreatedBy"] : "";
                                rptReportViewer.LocalReport.ReportPath = "Reports/Report/rptOprhanAccounts.rdl";
                                rptReportViewer.LocalReport.DisplayName = "Orphan Accounts Exception";
                                rptReportViewer.LocalReport.DataSources.Add(new ReportDataSource("CMMC", this.GetEnrollment(CreatedBy, dtStartDate, dtEndDate)));
                                rptReportViewer.LocalReport.Refresh();
                            }
                            else if (strReportName.Equals("AlienSubAccounts"))
                            {
                                DateTime? dtStartDate = Request.QueryString["StartDate"].ToString().ToDateTimeParse();
                                DateTime? dtEndDate = Request.QueryString["EndDate"].ToString().ToDateTimeParse();
                                string CreatedBy = Request.QueryString["CreatedBy"] != null ? Request.QueryString["CreatedBy"] : "";
                                rptReportViewer.LocalReport.ReportPath = "Reports/Report/rptAlienSubAccounts.rdl";
                                rptReportViewer.LocalReport.DisplayName = "Alien Sub Accounts Exception";
                                rptReportViewer.LocalReport.DataSources.Add(new ReportDataSource("CMMC", this.GetEnrollment(CreatedBy, dtStartDate, dtEndDate)));
                                rptReportViewer.LocalReport.Refresh();
                            }
                            else if (strReportName.Equals("CMSAccounts"))
                            {
                                DateTime? dtStartDate = Request.QueryString["StartDate"].ToString().ToDateTimeParse();
                                DateTime? dtEndDate = Request.QueryString["EndDate"].ToString().ToDateTimeParse();
                                string CreatedBy = Request.QueryString["CreatedBy"] != null ? Request.QueryString["CreatedBy"] : "";
                                rptReportViewer.LocalReport.ReportPath = "Reports/Report/rptCMSAccounts.rdl";
                                rptReportViewer.LocalReport.DisplayName = "CMS Accounts";
                                rptReportViewer.LocalReport.DataSources.Add(new ReportDataSource("CMMC", this.GetEnrollment(CreatedBy, dtStartDate, dtEndDate)));
                                rptReportViewer.LocalReport.Refresh();
                            }
                            else if (strReportName.Equals("SubAccountsCount"))
                            {
                                DateTime? dtStartDate = Request.QueryString["StartDate"].ToString().ToDateTimeParse();
                                DateTime? dtEndDate = Request.QueryString["EndDate"].ToString().ToDateTimeParse();
                                string CreatedBy = Request.QueryString["CreatedBy"] != null ? Request.QueryString["CreatedBy"] : "";
                                rptReportViewer.LocalReport.ReportPath = "Reports/Report/rptSubAccountsCount.rdl";
                                rptReportViewer.LocalReport.DisplayName = "Sub Accounts Count";
                                rptReportViewer.LocalReport.DataSources.Add(new ReportDataSource("CMMC", this.GetEnrollment(CreatedBy, dtStartDate, dtEndDate)));
                                rptReportViewer.LocalReport.Refresh();
                            }
                            else if (strReportName.Equals("CustomerProfile"))
                            {
                                DateTime? dtStartDate = Request.QueryString["StartDate"].ToString().ToDateTimeParse();
                                DateTime? dtEndDate = Request.QueryString["EndDate"].ToString().ToDateTimeParse();
                                string CreatedBy = Request.QueryString["CreatedBy"] != null ? Request.QueryString["CreatedBy"] : "";
                                rptReportViewer.LocalReport.ReportPath = "Reports/Report/rptCustomerProfile.rdl";
                                rptReportViewer.LocalReport.DisplayName = "Customer Profile";
                                rptReportViewer.LocalReport.DataSources.Add(new ReportDataSource("CMMC", this.GetEnrollment(CreatedBy, dtStartDate, dtEndDate)));
                                rptReportViewer.LocalReport.Refresh();
                            }
                            else if (strReportName.Equals("CMSDeals"))
                            {
                                DateTime? dtStartDate = Request.QueryString["StartDate"].ToString().ToDateTimeParse();
                                DateTime? dtEndDate = Request.QueryString["EndDate"].ToString().ToDateTimeParse();
                                string CreatedBy = Request.QueryString["CreatedBy"] != null ? Request.QueryString["CreatedBy"] : "";
                                rptReportViewer.LocalReport.ReportPath = "Reports/Report/rptCMSDeals.rdl";
                                rptReportViewer.LocalReport.DisplayName = "CMS Deals";
                                rptReportViewer.LocalReport.DataSources.Add(new ReportDataSource("CMMC", this.GetEnrollment(CreatedBy, dtStartDate, dtEndDate)));
                                rptReportViewer.LocalReport.Refresh();
                            }
                            else if (strReportName.Equals("ClosureCharges"))
                            {
                                DateTime? dtStartDate = Request.QueryString["StartDate"].ToString().ToDateTimeParse();
                                DateTime? dtEndDate = Request.QueryString["EndDate"].ToString().ToDateTimeParse();
                                string CreatedBy = Request.QueryString["CreatedBy"] != null ? Request.QueryString["CreatedBy"] : "";
                                rptReportViewer.LocalReport.ReportPath = "Reports/Report/rptClosureCharges.rdl";
                                rptReportViewer.LocalReport.DisplayName = "Closure/Accumulated Charges";
                                rptReportViewer.LocalReport.DataSources.Add(new ReportDataSource("CMMC", this.GetEnrollment(CreatedBy, dtStartDate, dtEndDate)));
                                rptReportViewer.LocalReport.Refresh();
                            }
                            else if (strReportName.Equals("TotalADBRequirement"))
                            {
                                DateTime? dtStartDate = Request.QueryString["StartDate"].ToString().ToDateTimeParse();
                                DateTime? dtEndDate = Request.QueryString["EndDate"].ToString().ToDateTimeParse();
                                string CreatedBy = Request.QueryString["CreatedBy"] != null ? Request.QueryString["CreatedBy"] : "";
                                rptReportViewer.LocalReport.ReportPath = "Reports/Report/rptTotalADBRequirement.rdl";
                                rptReportViewer.LocalReport.DisplayName = "Total ADB Requirement";
                                rptReportViewer.LocalReport.DataSources.Add(new ReportDataSource("CMMC", this.GetEnrollment(CreatedBy, dtStartDate, dtEndDate)));
                                rptReportViewer.LocalReport.Refresh();
                            }
                            else if (strReportName.Equals("UncollectedCMSADBPenFee"))
                            {
                                DateTime? dtStartDate = Request.QueryString["StartDate"].ToString().ToDateTimeParse();
                                DateTime? dtEndDate = Request.QueryString["EndDate"].ToString().ToDateTimeParse();
                                string CreatedBy = Request.QueryString["CreatedBy"] != null ? Request.QueryString["CreatedBy"] : "";
                                rptReportViewer.LocalReport.ReportPath = "Reports/Report/rptUncollectedCMSADBPenFee.rdl";
                                rptReportViewer.LocalReport.DisplayName = "Uncollected CMS ADB Penalty Fee";
                                rptReportViewer.LocalReport.DataSources.Add(new ReportDataSource("CMMC", this.GetEnrollment(CreatedBy, dtStartDate, dtEndDate)));
                                rptReportViewer.LocalReport.Refresh();
                            }
                            else if (strReportName.Equals("InsufficientFundCMSADBPenFee"))
                            {
                                DateTime? dtStartDate = Request.QueryString["StartDate"].ToString().ToDateTimeParse();
                                DateTime? dtEndDate = Request.QueryString["EndDate"].ToString().ToDateTimeParse();
                                string CreatedBy = Request.QueryString["CreatedBy"] != null ? Request.QueryString["CreatedBy"] : "";
                                rptReportViewer.LocalReport.ReportPath = "Reports/Report/rptInsufficientFundCMSADBPenFee.rdl";
                                rptReportViewer.LocalReport.DisplayName = "Insufficient Fund CMS ADB Penalty Fee";
                                rptReportViewer.LocalReport.DataSources.Add(new ReportDataSource("CMMC", this.GetEnrollment(CreatedBy, dtStartDate, dtEndDate)));
                                rptReportViewer.LocalReport.Refresh();
                            }
                            else if (strReportName.Equals("NoLinkedAccounts"))
                            {
                                DateTime? dtStartDate = Request.QueryString["StartDate"].ToString().ToDateTimeParse();
                                DateTime? dtEndDate = Request.QueryString["EndDate"].ToString().ToDateTimeParse();
                                string CreatedBy = Request.QueryString["CreatedBy"] != null ? Request.QueryString["CreatedBy"] : "";
                                rptReportViewer.LocalReport.ReportPath = "Reports/Report/rptNoLinkedAccounts.rdl";
                                rptReportViewer.LocalReport.DisplayName = "No Linked Accounts";
                                rptReportViewer.LocalReport.DataSources.Add(new ReportDataSource("CMMC", this.GetEnrollment(CreatedBy, dtStartDate, dtEndDate)));
                                rptReportViewer.LocalReport.Refresh();
                            }
                            else if (strReportName.Equals("ClientAccountsType"))
                            {
                                DateTime? dtStartDate = Request.QueryString["StartDate"].ToString().ToDateTimeParse();
                                DateTime? dtEndDate = Request.QueryString["EndDate"].ToString().ToDateTimeParse();
                                string CreatedBy = Request.QueryString["CreatedBy"] != null ? Request.QueryString["CreatedBy"] : "";
                                rptReportViewer.LocalReport.ReportPath = "Reports/Report/rptClientAccountsType.rdl";
                                rptReportViewer.LocalReport.DisplayName = "Client-Accounts Type";
                                rptReportViewer.LocalReport.DataSources.Add(new ReportDataSource("CMMC", this.GetEnrollment(CreatedBy, dtStartDate, dtEndDate)));
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
