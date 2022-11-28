using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using CTBC;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Runtime.Serialization;
using System.Data.OracleClient;

namespace CMMC.Models
{
    public class CMSCode : IDisposable
    {
        public void Dispose() { GC.SuppressFinalize(this); }

        public string Tagging { get; set; }


        public string PendingRequest { get; set; }

        public struct Details
        {
            public int CMSCode { get; set; }
            public string Description { get; set; }
            public int BranchCode { get; set; }
            public string BranchName { get; set; }
            public string Status { get; set; }
            public bool IsActive { get; set; }
            public string CreatedBy { get; set; }
            public DateTime? CreatedOn { get; set; }
            public string ModifiedBy { get; set; }
            public DateTime? ModifiedOn { get; set; }
            public string Tagging { get; set; }
            public decimal BasePenalty { get; set; }
            public decimal PenaltyFee { get; set; }
            public bool IsAutoDebit { get; set; }
            public int MaxFreeTransaction { get; set; } // int
            public string MaxWithdrawalPaidByEmployer { get; set; }
            public decimal WithdrawalFeePerTransaction { get; set; }
        }

        public List<CMSCode> GetTagging()
        {
            var tag = new List<CMSCode>
   {
    new CMSCode{Tagging = "RBG"},
    new CMSCode{Tagging = "IBG"},
   };
            return tag;
        }

        public Details Fill(int pCMSCode)
        {
            Details details = new Details();
            using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
            {
                using (SqlCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM CMSCodes WHERE CMSCode = @pCMSCode";
                    cmd.Parameters.Add(new SqlParameter("@pCMSCode", pCMSCode));
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            details.CMSCode = dr["CMSCode"].ToString().ToInt();
                            details.Description = dr["Description"].ToString();
                            details.BranchCode = dr["BranchCode"].ToString().ToInt();
                            details.Status = dr["Status"].ToString();
                            details.IsActive = dr["IsActive"].ToString().ToBoolean();
                            details.CreatedBy = dr["CreatedBy"].ToString();
                            details.CreatedOn = dr["CreatedOn"].ToString().ToDateTime();
                            details.ModifiedBy = dr["ModifiedBy"].ToString();
                            details.ModifiedOn = dr["ModifiedOn"].ToString().ToDateTime();
                            details.Tagging = dr["Tagging"].ToString();
                            details.BasePenalty = dr["BasePenalty"].ToString().ToDecimal();
                            details.PenaltyFee = dr["PenaltyFee"].ToString().ToDecimal();
                            details.IsAutoDebit = dr["IsAutoDebit"].ToString().ToBoolean();
                            details.MaxFreeTransaction = dr["MaxFreeTransaction"].ToString().ToInt(); //.toint()
                            details.MaxWithdrawalPaidByEmployer = dr["MaxWithdrawalPaidByEmployer"].ToString();
                            details.WithdrawalFeePerTransaction = dr["WithdrawalFeePerTransaction"].ToString().ToDecimal();
                        }
                    }
                }
            }
            return details;
        }

        public int Insert(Details pDetails)
        {
            int intReturn = 0;
            string strSQL = "";
            strSQL += "BEGIN TRAN ";
            strSQL += "if (SELECT COUNT(*) FROM CMSCodes where Status IN (4,5)) > 0 ";
            strSQL += "begin ";
            strSQL += "declare @tablenew table (CMSCode int ";
            strSQL += ",Description varchar(50) ";
            strSQL += ",BranchCode int ";
            strSQL += ",Status char(1) ";
            strSQL += ",IsActive bit ";
            strSQL += ",CreatedBy varchar(10) ";
            strSQL += ",CreatedOn datetime ";
            //strSQL += ",ModifiedBy varchar(10) ";
            //strSQL += ",ModifiedOn datetime ";
            strSQL += ",Tagging varchar(10) ";
            strSQL += ",BasePenalty decimal(18,2) ";
            strSQL += ",PenaltyFee decimal(18,2) ";
            strSQL += ",IsAutoDebit bit ";
            strSQL += ",MaxFreeTransaction char(1) ";
            strSQL += ",MaxWithdrawalPaidByEmployer char(3) ";
            strSQL += ",WithdrawalFeePerTransaction decimal(18,2) ";
            strSQL += ") ";
            strSQL += "UPDATE CMSCodes SET ";
            strSQL += "Description = @pDescription ";
            strSQL += ",BranchCode = @pBranchCode ";
            strSQL += ",IsActive = @pIsActive ";
            strSQL += ",CreatedBy = @pCreatedBy ";
            strSQL += ",CreatedOn = @pCreatedOn ";
            strSQL += ",ModifiedBy = null ";
            strSQL += ",ModifiedOn = null ";
            strSQL += ",Tagging = @pTagging ";
            strSQL += ",Status = @pStatus ";
            strSQL += ",BasePenalty = @pBasePenalty ";
            strSQL += ",PenaltyFee = @pPenaltyFee ";
            strSQL += ",IsAutoDebit = @pIsAutoDebit ";
            strSQL += ",MaxFreeTransaction = @pMaxFreeTransaction ";
            strSQL += ",MaxWithdrawalPaidByEmployer = @pMaxWithdrawalPaidByEmployer ";
            strSQL += ",WithdrawalFeePerTransaction = @pWithdrawalFeePerTransaction ";
            strSQL += "OUTPUT ";
            strSQL += "CAST(inserted.CMSCode as int) ";
            strSQL += ",CAST(inserted.Description as varchar(50)) ";
            strSQL += ",CAST(inserted.BranchCode as int) ";
            strSQL += ",CAST(inserted.Status as char(1)) ";
            strSQL += ",CAST(inserted.IsActive as bit) ";
            strSQL += ",CAST(inserted.CreatedBy as varchar(10)) ";
            strSQL += ",CAST(inserted.CreatedOn as datetime) ";
            //strSQL += ",CAST(inserted.ModifiedBy as varchar(10)) ";
            //strSQL += ",CAST(inserted.ModifiedOn as datetime ) ";
            strSQL += ",CAST(inserted.Tagging as varchar(10)) ";
            strSQL += ",CAST(inserted.BasePenalty as decimal(18,2)) ";
            strSQL += ",CAST(inserted.PenaltyFee as decimal(18,2)) ";
            strSQL += ",CAST(inserted.IsAutoDebit as bit) ";
            strSQL += ",CAST(inserted.MaxFreeTransaction as char(1)) ";
            strSQL += ",CAST(inserted.MaxWithdrawalPaidByEmployer as char(3)) ";
            strSQL += ",CAST(inserted.WithdrawalFeePerTransaction as decimal(18,2)) ";
            strSQL += "INTO @tablenew ";
            strSQL += "WHERE CMSCode = (SELECT MIN(CMSCode) as CMSCode FROM CMSCodes where Status IN (4,5)) ";
            strSQL += "SELECT CMSCode From @tablenew ";
            strSQL += "end ";
            strSQL += "ELSE ";
            strSQL += "begin ";
            strSQL += "INSERT INTO CMSCodes ";
            strSQL += "VALUES ";
            strSQL += "( ";
            strSQL += "@pDescription ";
            strSQL += ",@pBranchCode ";
            strSQL += ",@pStatus ";
            strSQL += ",@pIsActive ";
            strSQL += ",@pCreatedBy ";
            strSQL += ",@pCreatedOn ";
            strSQL += ",null ";
            strSQL += ",null ";
            strSQL += ",@pTagging ";
            strSQL += ",@pBasePenalty ";
            strSQL += ",@pPenaltyFee ";
            strSQL += ",@pIsAutoDebit ";
            strSQL += ",@pMaxFreeTransaction ";
            strSQL += ",@pMaxWithdrawalPaidByEmployer ";
            strSQL += ",@pWithdrawalFeePerTransaction ";
            strSQL += ") ";
            strSQL += "SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY] ";
            strSQL += "end ";
            strSQL += "COMMIT TRAN; ";

            using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
            {
                using (SqlCommand cmd = cn.CreateCommand())
                {
                    try
                    {
                        cmd.CommandText = strSQL;
                        cmd.Parameters.Add(new SqlParameter("@pDescription", pDetails.Description));
                        cmd.Parameters.Add(new SqlParameter("@pBranchCode", pDetails.BranchCode));
                        cmd.Parameters.Add(new SqlParameter("@pStatus", pDetails.Status));
                        cmd.Parameters.Add(new SqlParameter("@pIsActive", "1"));
                        cmd.Parameters.Add(new SqlParameter("@pCreatedBy", pDetails.CreatedBy));
                        cmd.Parameters.Add(new SqlParameter("@pCreatedOn", pDetails.CreatedOn));
                        cmd.Parameters.Add(new SqlParameter("@pTagging", pDetails.Tagging));
                        cmd.Parameters.Add(new SqlParameter("@pBasePenalty", pDetails.BasePenalty));
                        cmd.Parameters.Add(new SqlParameter("@pPenaltyFee", pDetails.PenaltyFee));
                        cmd.Parameters.Add(new SqlParameter("@pIsAutoDebit", pDetails.IsAutoDebit));
                        cmd.Parameters.Add(new SqlParameter("@pMaxWithdrawalPaidByEmployer", pDetails.MaxWithdrawalPaidByEmployer));
                        cmd.Parameters.Add(new SqlParameter("@pMaxFreeTransaction", pDetails.MaxFreeTransaction));
                        cmd.Parameters.Add(new SqlParameter("@pWithdrawalFeePerTransaction", pDetails.WithdrawalFeePerTransaction));
                        cn.Open();
                        intReturn = cmd.ExecuteScalar().ToString().ToInt();
                    }
                    catch (Exception e)
                    {
                        CTBC.Logs.Write("Insert", e.Message, "Pay Code");
                    }
                }
            }
            return intReturn;
        }

        public int Update(Details pDetails)
        {
            int intReturn = 0;
            string strSQL = "";
            strSQL += "UPDATE CMSCodes ";
            strSQL += "SET ";
            strSQL += "Description = @pDescription ";
            strSQL += ",BranchCode = @pBranchCode ";
            strSQL += ",IsActive = @pIsActive ";
            strSQL += ",ModifiedBy = @pModifiedBy ";
            strSQL += ",ModifiedOn = GETDATE() ";
            strSQL += ",Tagging = @pTagging ";
            strSQL += ",BasePenalty = @pBasePenalty ";
            strSQL += ",PenaltyFee = @pPenaltyFee ";
            strSQL += ",IsAutoDebit = @pIsAutoDebit ";
            strSQL += ",MaxFreeTransaction = @pMaxFreeTransaction ";
            strSQL += ",MaxWithdrawalPaidByEmployer = @pMaxWithdrawalPaidByEmployer ";
            strSQL += ",WithdrawalFeePerTransaction = @pWithdrawalFeePerTransaction ";
            strSQL += ",Remarks = @pRemarks ";
            strSQL += "WHERE ";
            strSQL += "CMSCode = @pCMSCode ";

            using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
            {
                using (SqlCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = strSQL;
                    cmd.Parameters.Add(new SqlParameter("@pDescription ", pDetails.Description));
                    cmd.Parameters.Add(new SqlParameter("@pBranchCode ", pDetails.BranchCode));
                    cmd.Parameters.Add(new SqlParameter("@pIsActive ", pDetails.IsActive));
                    cmd.Parameters.Add(new SqlParameter("@pModifiedBy ", pDetails.ModifiedBy));
                    cmd.Parameters.Add(new SqlParameter("@pCMSCode ", pDetails.CMSCode));
                    cmd.Parameters.Add(new SqlParameter("@pTagging", pDetails.Tagging ?? DBNull.Value.ToString()));
                    cmd.Parameters.Add(new SqlParameter("@pBasePenalty", pDetails.BasePenalty));
                    cmd.Parameters.Add(new SqlParameter("@pPenaltyFee", pDetails.PenaltyFee));
                    cmd.Parameters.Add(new SqlParameter("@pIsAutoDebit", pDetails.IsAutoDebit));
                    cmd.Parameters.Add(new SqlParameter("@pMaxWithdrawalPaidByEmployer", pDetails.MaxWithdrawalPaidByEmployer));
                    cmd.Parameters.Add(new SqlParameter("@pMaxFreeTransaction", pDetails.MaxFreeTransaction));
                    cmd.Parameters.Add(new SqlParameter("@pWithdrawalFeePerTransaction", pDetails.WithdrawalFeePerTransaction));
                    cn.Open();
                    intReturn = cmd.ExecuteNonQuery();
                }
            }
            return intReturn;
        }

        public int Delete(Details pDetails)
        {
            int intReturn = 0;
            string strSQL = "";
            strSQL += "DELETE FROM CMSCodes ";
            strSQL += "WHERE ";
            strSQL += "CMSCode = @pCMSCode ";
            using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
            {
                using (SqlCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = strSQL;
                    cmd.Parameters.Add(new SqlParameter("@pCMSCode", pDetails.CMSCode));
                    cn.Open();
                    intReturn = cmd.ExecuteNonQuery();
                }
            }
            return intReturn;
        }

        public List<Details> GetUnmanagedAccounts() //AFM 202205
        {
            List<Details> list = new List<Details>();
            //string strSQL = "SELECT * ";
            //strSQL += "FROM ";
            //strSQL += "CMSCodes C ";
            //strSQL += "left join branches B ON B.BranchCode = C.BranchCode ";
            //strSQL += "where not EXISTS (select * from AccountInformations A where A.cmscode = C.cmscode) ";
            //strSQL += "and C.Status IN (2) AND C.IsActive IN (1) ";
            //strSQL += "and Convert(date, C.CreatedOn) < Convert(date, getdate()) ";
            //strSQL += "order by C.CMSCode, C.Description ";
            string strSQL = "SELECT * ";
            strSQL += "FROM ";
            strSQL += "CMSCodes C ";
            strSQL += "left join branches B ON B.BranchCode = C.BranchCode ";
            strSQL += "where C.Status IN (2) AND C.IsActive IN (1) ";
            strSQL += "and Convert(date, C.CreatedOn) < Convert(date, getdate()) ";
            strSQL += "order by C.CMSCode, C.Description ";

            List<Models.BANCS_ACCOUNT.Details> paycodeList = new Models.BANCS_ACCOUNT().GetPayrollCodeList();

            using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
            {
                using (SqlCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = strSQL;
                    cmd.CommandTimeout = 0;
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            string CMSCode = dr["CMSCode"].ToString();
                            if (!paycodeList.Any(x => x.PayrollCode == CMSCode))
                            {
                                list.Add(new Details()
                                {
                                    CMSCode = dr["CMSCode"].ToString().ToInt(),
                                    Description = dr["Description"].ToString(),
                                    BranchCode = dr["BranchCode"].ToString().ToInt(),
                                    BranchName = dr["BranchName"].ToString(),
                                    Status = dr["Status"].ToString(),
                                    IsActive = dr["IsActive"].ToString().ToBoolean(),
                                    CreatedBy = dr["CreatedBy"].ToString(),
                                    CreatedOn = dr["CreatedOn"].ToString().ToDateTime(),
                                    ModifiedBy = dr["ModifiedBy"].ToString(),
                                    ModifiedOn = dr["ModifiedOn"].ToString().ToDateTime(),
                                    Tagging = dr["Tagging"].ToString(),
                                    BasePenalty = dr["BasePenalty"].ToString().ToDecimal(),
                                    PenaltyFee = dr["PenaltyFee"].ToString().ToDecimal(),
                                    IsAutoDebit = dr["IsAutoDebit"].ToString().ToBoolean(),
                                    MaxFreeTransaction = dr["MaxFreeTransaction"].ToString().ToInt(), //toint()
                                    MaxWithdrawalPaidByEmployer = dr["MaxWithdrawalPaidByEmployer"].ToString(),
                                    WithdrawalFeePerTransaction = dr["WithdrawalFeePerTransaction"].ToString().ToDecimal()
                                });
                            }
                        }
                    }
                }
            }
            return list;
        }

        public List<Details> GetListPending()
        {
            List<Details> list = new List<Details>();
            string strSQL = "SELECT * ";
            strSQL += "FROM ";
            strSQL += "CMSCodes C ";
            strSQL += "WHERE Status IN (1) and IsActive IN(1) ";
            strSQL += "ORDER BY CMSCode, Description ";

            using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
            {
                using (SqlCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = strSQL;
                    cmd.CommandTimeout = 0;
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new Details()
                            {
                                CMSCode = dr["CMSCode"].ToString().ToInt(),
                                Description = dr["Description"].ToString(),
                                BranchCode = dr["BranchCode"].ToString().ToInt(),
                                Status = dr["Status"].ToString(),
                                IsActive = dr["IsActive"].ToString().ToBoolean(),
                                CreatedBy = dr["CreatedBy"].ToString(),
                                CreatedOn = dr["CreatedOn"].ToString().ToDateTime(),
                                ModifiedBy = dr["ModifiedBy"].ToString(),
                                ModifiedOn = dr["ModifiedOn"].ToString().ToDateTime(),
                                Tagging = dr["Tagging"].ToString(),
                                BasePenalty = dr["BasePenalty"].ToString().ToDecimal(),
                                PenaltyFee = dr["PenaltyFee"].ToString().ToDecimal(),
                                IsAutoDebit = dr["IsAutoDebit"].ToString().ToBoolean(),
                                MaxFreeTransaction = dr["MaxFreeTransaction"].ToString().ToInt(),//toint()
                                MaxWithdrawalPaidByEmployer = dr["MaxWithdrawalPaidByEmployer"].ToString(),
                                WithdrawalFeePerTransaction = dr["WithdrawalFeePerTransaction"].ToString().ToDecimal()
                            });
                        }
                    }
                }
            }
            return list;
        }

        public List<Details> GetListApproved()
        {
            List<Details> list = new List<Details>();
            string strSQL = "SELECT * ";
            strSQL += "FROM ";
            strSQL += "CMSCodes C ";
            strSQL += "left join branches b ON b.BranchCode = c.BranchCode ";
            strSQL += "WHERE C.Status IN (2) AND C.IsActive IN (1) ";
            strSQL += "ORDER BY CMSCode, Description ";

            using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
            {
                using (SqlCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = strSQL;
                    cmd.CommandTimeout = 0;
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new Details()
                            {
                                CMSCode = dr["CMSCode"].ToString().ToInt(),
                                Description = dr["Description"].ToString(),
                                BranchCode = dr["BranchCode"].ToString().ToInt(),
                                BranchName = dr["BranchName"].ToString(),
                                Status = dr["Status"].ToString(),
                                IsActive = dr["IsActive"].ToString().ToBoolean(),
                                CreatedBy = dr["CreatedBy"].ToString(),
                                CreatedOn = dr["CreatedOn"].ToString().ToDateTime(),
                                ModifiedBy = dr["ModifiedBy"].ToString(),
                                ModifiedOn = dr["ModifiedOn"].ToString().ToDateTime(),
                                Tagging = dr["Tagging"].ToString(),
                                BasePenalty = dr["BasePenalty"].ToString().ToDecimal(),
                                PenaltyFee = dr["PenaltyFee"].ToString().ToDecimal(),
                                IsAutoDebit = dr["IsAutoDebit"].ToString().ToBoolean(),
                                MaxFreeTransaction = dr["MaxFreeTransaction"].ToString().ToInt(), //toint()
                                MaxWithdrawalPaidByEmployer = dr["MaxWithdrawalPaidByEmployer"].ToString(),
                                WithdrawalFeePerTransaction = dr["WithdrawalFeePerTransaction"].ToString().ToDecimal()
                            });
                        }
                    }
                }
            }
            return list;
        }

        public int ApproveCMSCode(Details details)
        {
            int intReturn = 0;
            string strSQL = "UPDATE ";
            strSQL += "CMSCodes SET ";
            strSQL += "Status = @pStatus ";
            strSQL += ", IsActive = @pIsActive ";
            strSQL += "WHERE CMSCode = @pCMSCode ";
            using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
            {
                using (SqlCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = strSQL;
                    cmd.Parameters.Add(new SqlParameter("@pStatus", details.Status));
                    cmd.Parameters.Add(new SqlParameter("@pCMSCode", details.CMSCode));
                    cmd.Parameters.Add(new SqlParameter("@pIsActive", details.IsActive));
                    cn.Open();
                    intReturn = cmd.ExecuteNonQuery();
                }
            }
            return intReturn;
        }

        public int CancelCMSCode(Details details)
        {
            int intReturn = 0;
            string strSQL = "UPDATE ";
            strSQL += "CMSCodes SET ";
            strSQL += "Status = @pStatus ";
            strSQL += ",IsActive = @pIsActive ";
            strSQL += "WHERE CMSCode = @pCMSCode ";
            using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
            {
                using (SqlCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = strSQL;
                    cmd.Parameters.Add(new SqlParameter("@pStatus", details.Status));
                    cmd.Parameters.Add(new SqlParameter("@pCMSCode", details.CMSCode));
                    cmd.Parameters.Add(new SqlParameter("@pIsActive", details.IsActive));
                    cn.Open();
                    intReturn = cmd.ExecuteNonQuery();
                }
            }
            return intReturn;
        }

        public int RemoveCMSCode(Details details)
        {
            int intReturn = 0;
            string strSQL = "UPDATE ";
            strSQL += "CMSCodes SET ";
            strSQL += "Status = @pStatus ";
            strSQL += ", IsActive = @pIsActive ";
            strSQL += "WHERE CMSCode = @pCMSCode ";
            using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
            {
                using (SqlCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = strSQL;
                    cmd.Parameters.Add(new SqlParameter("@pStatus", details.Status));
                    cmd.Parameters.Add(new SqlParameter("@pCMSCode", details.CMSCode));
                    cmd.Parameters.Add(new SqlParameter("@pIsActive", details.IsActive));
                    cn.Open();
                    intReturn = cmd.ExecuteNonQuery();
                }
            }
            return intReturn;
        }

        public CMSCode GetPendingRequest()
        {
            CMSCode total = new CMSCode();
            using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
            {
                using (SqlCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "SELECT COUNT(*) as total FROM CMSCodes WHERE Status = 1";
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            total.PendingRequest = dr["total"].ToString();
                        }
                    }
                }
            }
            return total;
        }

        public Details GetCMSCodeDetails(int pCMSCode, string sCompany) //AFM 20220502 added company parameter
        {
            Details cms = new Details();
            string strSQL = "SELECT * ";
            strSQL += "FROM ";
            strSQL += "CMSCodes ";
            strSQL += "WHERE ";
            if (pCMSCode != 0)
                strSQL += "CMSCode = @pCMSCode ";
            else
                strSQL += "Description = @sCompany ";

            using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
            {
                using (SqlCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = strSQL;
                    if (pCMSCode != 0)
                        cmd.Parameters.Add(new SqlParameter("@pCMSCode", pCMSCode));
                    else
                        cmd.Parameters.Add(new SqlParameter("@sCompany", sCompany));
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            cms.BranchCode = dr["BranchCode"].ToString().ToInt();
                            cms.CMSCode = dr["CMSCode"].ToString().ToInt();
                            cms.CreatedBy = dr["CreatedBy"].ToString();
                            cms.CreatedOn = dr["CreatedOn"].ToString().ToDateTime();
                            cms.Description = dr["Description"].ToString();
                            cms.IsActive = dr["IsActive"].ToString().ToBoolean();
                            cms.ModifiedBy = dr["ModifiedBy"].ToString();
                            cms.ModifiedOn = dr["ModifiedOn"].ToString().ToDateTime();
                            cms.Status = dr["Status"].ToString();
                            cms.Tagging = dr["Tagging"].ToString();
                            cms.BasePenalty = dr["BasePenalty"].ToString().ToDecimal();
                            cms.PenaltyFee = dr["PenaltyFee"].ToString().ToDecimal();
                            cms.IsAutoDebit = dr["IsAutoDebit"].ToString().ToBoolean();
                            cms.MaxFreeTransaction = dr["MaxFreeTransaction"].ToString().ToInt(); //toint()
                            cms.MaxWithdrawalPaidByEmployer = dr["MaxWithdrawalPaidByEmployer"].ToString();
                            cms.WithdrawalFeePerTransaction = dr["WithdrawalFeePerTransaction"].ToString().ToDecimal();
                        }
                    }
                }
            }
            return cms;
        }

        public List<RequestList> ListForTermination()
        {
            List<RequestList> list = new List<RequestList>();
            string strSQL = "SELECT * FROM RequestList where Status = 1 AND Module = 'Home'";
            using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
            {
                using (SqlCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = strSQL;
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new RequestList
                            {
                                Module = dr["Module"].ToString(),
                                WhereValues = new Models.RequestList().ToListValues(dr["WhereValues"].ToString()),
                                Remarks = dr["Remarks"].ToString(),
                                Request = new Requests
                                {
                                    RequestCode = dr["RequestCode"].ToString().ToInt()
                                }
                            });
                        }
                    }
                }
            }
            return list;
        }
    }
}

