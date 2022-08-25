using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using CTBC;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Runtime.Serialization;

namespace CMMC.Models
{
    public class AccountInformation : IDisposable
    {
        public void Dispose() { GC.SuppressFinalize(this); }

        public string PendingRequest { get; set; }

        public struct Details
        {
            public int ID { get; set; }
            public int CMSCode { get; set; }
            public string AccountNumber { get; set; }
            public string AccountName { get; set; }
            public string BranchCode { get; set; }
            public string BranchName { get; set; }
            public DateTime DateEnrolled { get; set; }
            public string InvestmentDesc { get; set; }
            public string InvestmentCode { get; set; }
            public DateTime EffectivityDate { get; set; }
            public string Tag { get; set; }
            public string Status { get; set; }
        }

        public Details Fill(int pCMSCode, string sAcctNo)
        {
            Details details = new Details();

            try
            {
                using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
                {
                    using (SqlCommand cmd = cn.CreateCommand())
                    {
                        if (pCMSCode == 0)
                        {
                            cmd.CommandText = "SELECT * FROM AccountInformations WHERE CMSCode = @pCMSCode";
                            cmd.Parameters.Add(new SqlParameter("@pCMSCode", pCMSCode));
                        }
                        else
                        {
                            cmd.CommandText = "SELECT * FROM AccountInformations WHERE AccountNo = @sAcctNo";
                            cmd.Parameters.Add(new SqlParameter("@sAcctNo", sAcctNo));
                        }

                        cn.Open();
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                details.AccountName = dr["AccountName"].ToString();
                                details.AccountNumber = dr["AccountNumber"].ToString();
                                details.BranchCode = dr["BranchCode"].ToString();
                                details.CMSCode = dr["CMSCode"].ToString().ToInt();
                                details.DateEnrolled = dr["DateEnrolled"].ToString().ToDateTime();
                                details.EffectivityDate = dr["EffectivityDate"].ToString().ToDateTime();
                                details.ID = dr["ID"].ToString().ToInt();
                                details.InvestmentDesc = dr["InvestmentDesc"].ToString();
                                details.InvestmentCode = dr["InvestmentCode"].ToString();
                                details.Tag = dr["Tag"].ToString();
                                details.Status = dr["Status"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CTBC.Logs.Write("Fill", ex.Message, "Account Information");
            }

            return details;
        }

        public int Insert(Details pDetails)
        {
            int intReturn = 0;
            string strSQL = "";
            strSQL += "INSERT INTO AccountInformations ";
            strSQL += "VALUES ";
            strSQL += "(";
            strSQL += "@pCMSCode ";
            strSQL += ",@pAccountNo ";
            strSQL += ",@pAccountName ";
            strSQL += ",@pBranchCode ";
            strSQL += ",@pDateEnrolled ";
            strSQL += ",@pInvestmentDesc ";
            strSQL += ",@pInvestmentCode ";
            strSQL += ",@pEffectivityDate ";
            strSQL += ",@pTagging ";
            strSQL += ",@pStatus ";
            strSQL += ") ";

            try
            {
                using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
                {
                    using (SqlCommand cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = strSQL;
                        cmd.Parameters.Add(new SqlParameter("@pCMSCode", pDetails.CMSCode));
                        cmd.Parameters.Add(new SqlParameter("@pAccountNo", pDetails.AccountNumber));
                        cmd.Parameters.Add(new SqlParameter("@pAccountName", pDetails.AccountName));
                        cmd.Parameters.Add(new SqlParameter("@pBranchCode", pDetails.BranchCode));
                        cmd.Parameters.Add(new SqlParameter("@pDateEnrolled", pDetails.DateEnrolled));
                        cmd.Parameters.Add(new SqlParameter("@pInvestmentDesc", pDetails.InvestmentDesc));
                        cmd.Parameters.Add(new SqlParameter("@pInvestmentCode", pDetails.InvestmentCode));
                        cmd.Parameters.Add(new SqlParameter("@pEffectivityDate", pDetails.EffectivityDate));
                        cmd.Parameters.Add(new SqlParameter("@pTagging", pDetails.Tag));
                        cmd.Parameters.Add(new SqlParameter("@pStatus", pDetails.Status));
                        cn.Open();
                        intReturn = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                CTBC.Logs.Write("Insert", ex.Message, "Account Information");
            }
            return intReturn;
        }

        public int Update(Details pDetails)
        {
            int intReturn = 0;
            string strSQL = "";
            strSQL += "UPDATE AccountInformations ";
            strSQL += "SET ";
            strSQL += "AccountName = @pAccountName ";
            strSQL += ",BranchCode = @pBranchCode ";
            strSQL += ",DateEnrolled = @pDateEnrolled ";
            strSQL += ",InvestmentCode = @pInvestmentCode ";
            strSQL += ",InvestmentDesc = @pInvestmentDesc ";
            strSQL += ",EffectivityDate = @pEffectivityDate ";
            strSQL += ",Tagging = @pTagging ";
            strSQL += ",Status = @pStatus ";
            strSQL += "WHERE ";
            strSQL += "CMSCode = @pCMSCode ";
            strSQL += "AND AccountNo = @pAccountNo ";

            using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
            {
                using (SqlCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = strSQL;
                    cmd.Parameters.Add(new SqlParameter("@pCMSCode", pDetails.CMSCode));
                    cmd.Parameters.Add(new SqlParameter("@pAccountNo", pDetails.AccountNumber));
                    cmd.Parameters.Add(new SqlParameter("@pAccountName", pDetails.AccountName));
                    cmd.Parameters.Add(new SqlParameter("@pBranchCode", pDetails.BranchCode));
                    cmd.Parameters.Add(new SqlParameter("@pDateEnrolled", pDetails.DateEnrolled));
                    cmd.Parameters.Add(new SqlParameter("@pInvestmentDesc", pDetails.InvestmentDesc));
                    cmd.Parameters.Add(new SqlParameter("@pInvestmentCode", pDetails.InvestmentCode));
                    cmd.Parameters.Add(new SqlParameter("@pEffectivityDate", pDetails.EffectivityDate));
                    cmd.Parameters.Add(new SqlParameter("@pTagging", pDetails.Tag));
                    cmd.Parameters.Add(new SqlParameter("@pStatus", pDetails.Status));
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
            strSQL += "DELETE FROM AccountInformations ";
            strSQL += "WHERE ";
            strSQL += "CMSCode = @pCMSCode ";
            strSQL += "AND ID = @pID ";
            strSQL += "AND AccountNo = @pAccountNo";

            using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
            {
                using (SqlCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = strSQL;
                    cmd.Parameters.Add(new SqlParameter("@pCMSCode", pDetails.CMSCode));
                    cmd.Parameters.Add(new SqlParameter("@pID", pDetails.ID));
                    cmd.Parameters.Add(new SqlParameter("@pAccountNo", pDetails.AccountNumber));
                    cn.Open();
                    intReturn = cmd.ExecuteNonQuery();
                }
            }
            return intReturn;
        }

        public int CancelandRemove(int pCMSCode)
        {
            int intReturn = 0;
            string strSQL = "";
            strSQL += "DELETE FROM AccountInformations ";
            strSQL += "WHERE ";
            strSQL += "CMSCode = @pCMSCode ";

            using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
            {
                using (SqlCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = strSQL;
                    cmd.Parameters.Add(new SqlParameter("@pCMSCode", pCMSCode));
                    cn.Open();
                    intReturn = cmd.ExecuteNonQuery();
                }
            }
            return intReturn;
        }

        public List<Details> GetList(int pCMSCode = 0)
        {
            List<Details> list = new List<Details>();
            string strSQL = string.Empty;
            strSQL = "SELECT RIGHT('000000000000' + CONVERT(VARCHAR(12),AI.AccountNo),12) AS AccountNo ";
            strSQL += " , AI.AccountName ";
            strSQL += " , BR.BranchName ";
            strSQL += " , AI.DateEnrolled ";
            strSQL += " , AI.InvestmentDesc ";
            strSQL += " , ST.VAL ";
            strSQL += " FROM AccountInformations AI ";
            strSQL += " INNER JOIN Branches BR ON (AI.BranchCode = BR.BranchCode) ";
            strSQL += " INNER JOIN dbo.CMMC_Lookup ST ON ST.type = 1 AND (AI.Status = ST.id) ";
            strSQL += (pCMSCode > 0 ? " WHERE AI.CMSCode = @pCMSCode " : "");
            strSQL += " ORDER	BY AI.AccountNo ";



            using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
            {
                using (SqlCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = strSQL;
                    if (pCMSCode > 0)
                    {
                        cmd.Parameters.Add(new SqlParameter("@pCMSCode", pCMSCode));
                    }
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new Details()
                            {
                                AccountName = dr["AccountName"].ToString()
                             ,
                                AccountNumber = dr["AccountNo"].ToString()
                             ,
                                BranchName = dr["BranchName"].ToString()
                             ,
                                DateEnrolled = dr["DateEnrolled"].ToString().ToDateTime()
                             ,
                                InvestmentDesc = dr["InvestmentDesc"].ToString()
                             ,
                                Status = dr["VAL"].ToString()
                            });
                        }
                    }
                }
            }
            return list;
        }

        public List<Details> GetNumber(Details pDetails)
        {
            List<Details> list = new List<Details>();
            try
            {
                using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
                {
                    using (SqlCommand cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT AccountNo FROM AccountInformations WHERE CMSCode = @pCMSCode";
                        cmd.Parameters.Add(new SqlParameter("@pCMSCode", pDetails.CMSCode));
                        cn.Open();
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                list.Add(new Details()
                                {
                                    AccountNumber = dr["AccountNo"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CTBC.Logs.Write("Get Number", ex.Message, "Account Information");
            }
            return list;
        }

        public List<Details> LoadAccountNumber()
        {
            List<Details> list = new List<Details>();
            try
            {
                using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
                {
                    using (SqlCommand cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT AccountNo, AccountName FROM AccountInformations";
                        cn.Open();
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                list.Add(new Details()
                                {
                                    AccountNumber = dr["AccountNo"].ToString(),
                                    AccountName = dr["AccountName"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CTBC.Logs.Write("Load Account Number", ex.Message, "Account Information");
            }
            return list;
        }

        public List<Details> getInvestmentType()
        {
            List<Details> list = new List<Details>();
            string strSQL = "";
            strSQL += "SELECT ";
            strSQL += "it_InvstTypeCode, it_InvstTypeDesc ";
            strSQL += "FROM InvestmentType ";
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
                            list.Add(new Details
                            {
                                InvestmentCode = dr["it_InvstTypeCode"].ToString(),
                                InvestmentDesc = dr["it_InvstTypeDesc"].ToString()
                            });
                        }
                    }
                }
            }
            return list;
        }

        public AccountInformation GetPendingRequest()
        {
            AccountInformation total = new AccountInformation();
            using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
            {
                using (SqlCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "SELECT COUNT(*) as total FROM AccountInformations WHERE Status = 1";
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

    }
}
