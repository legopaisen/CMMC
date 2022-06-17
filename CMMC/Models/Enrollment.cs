using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CTBC;

namespace CMMC.Models
{
 public class Enrollment: IDisposable
 {
  public void Dispose() { GC.SuppressFinalize(this); }

  public struct Pending
  {
   public string PendingCMSCode { get; set; }
   public string PendingAccounts { get; set; }
   public string PendingServices { get; set; }
   public string PendingRelatedAccounts { get; set; }
  }

  public struct RequestListDetails
  {
   public int RequestListCode { get; set; }	
   public int RequestCode { get; set; }	
   public string Module { get; set; }	
   public string Action { get; set; }
   public string NewValues { get; set; }
   public string OldValues { get; set; }
   public string WhereValues { get; set; }
   public string AffectedTable { get; set; }
   public string Remarks { get; set; }
   public string Status { get; set; }
   public string ApprovedBy { get; set; }	
   public DateTime ApprovedOn { get; set; }
   public RequestsDetails RequestsDetails { get; set; }
   public string ConcernedCMSCode { get; set; } 
  }

  public struct RequestsDetails
  {
   public int RequestCode { get; set; }
   public string CreatedBy { get; set; }	
   public DateTime CreatedOn { get; set; }	
   public string ModifiedBy { get; set; }	
   public DateTime ModifiedOn { get; set; }	
   public string IPAddress { get; set; }
   public string AssignedApprover { get; set; }
  }

  public struct CmsCodeDetails
  {
  // public int CMSCode { get; set; }
  // public string Description { get; set; }
  // public int BranchCode { get; set; }
  // public string BranchName { get; set; }
  // public string Status { get; set; }
  // public bool IsActive { get; set; }
  // public string CreatedBy { get; set; }
  // public DateTime? CreatedOn { get; set; }
  // public string ModifiedBy { get; set; }
  // public DateTime ModifiedOn { get; set; }
  // public string Tagging { get; set; }
   public Models.CMSCode.Details GeneralDetails { get; set;  }
   public List<Models.ServiceOptions.AvailedDetails> AvailedDetailsList { get; set; }   
   public List<Models.AccountInformation.Details> AccountInformationList { get; set; }
   public List<Models.RelatedAccounts.Details> RelatedAccountList { get; set; }
   public List<Models.ChildAccounts.Details> ChildAccountList { get; set; }
  }

  public CmsCodeDetails GetEnrollmentDetails(int pCMSCode)
  {
   CmsCodeDetails details = new CmsCodeDetails();
   //CMSCode.Details cms = new CMSCode().Fill(pCMSCode);
   //details.CMSCode = cms.CMSCode;
   //details.BranchCode = cms.BranchCode;
   //details.Description = cms.Description;
   //details.IsActive = cms.IsActive;
   //details.ModifiedBy = cms.ModifiedBy; 
   //details.ModifiedOn = cms.ModifiedOn;
   //details.CreatedBy = cms.CreatedBy;
   //if (cms.CreatedOn.ToString() == "") { details.CreatedOn = null; } 
   //else { details.CreatedOn = Convert.ToDateTime(cms.CreatedOn); };
   //details.Status = cms.Status;
   //details.Tagging = cms.Tagging;
   details.GeneralDetails = new Models.CMSCode().Fill(pCMSCode);
   details.AccountInformationList = new Models.AccountInformation().GetList(pCMSCode);
   details.AvailedDetailsList = new Models.ServiceOptions().GetEverything(pCMSCode);
   //details.AvailedDetailsList = new Models.ServiceOptions().GetRelationshipManager(pCMSCode);
   //details.PenaltyCharges = new Models.PenaltyCharges().Fill(pCMSCode);
   details.RelatedAccountList = new Models.RelatedAccounts().GetList(pCMSCode);
   details.ChildAccountList = new Models.ChildAccounts().GetList(pCMSCode);
   return details;
  }

  public CmsCodeDetails SaveUpdate(CmsCodeDetails details, Models.Enrollment.RequestListDetails pRequestListDetails)
  {
   int intRequestCode = 0;
   //CMSCode.Details cms = new CMSCode.Details();
   //cms.BranchCode = details.BranchCode;
   //cms.CMSCode = details.CMSCode;
   //cms.Description = details.Description;
   //cms.IsActive = details.IsActive;
   //cms.ModifiedBy = details.ModifiedBy;
   //cms.ModifiedOn = details.ModifiedOn;
   //cms.Status = details.Status;
   //cms.Tagging = details.Tagging;
   //cms = details.CMSCode;
   new CMSCode().Update(details.GeneralDetails);
   if (details.GeneralDetails.CMSCode > 0)
   {
    //Penalty Details Changes
    //new Models.PenaltyCharges().Update(details.PenaltyCharges);

    //Service Option Changes
    //List<Models.ServiceOptions.AvailedDetails> list = new Models.ServiceOptions().GetEverything(details.CMSCodeDetails.CMSCode);
    //if (details.AvailedDetailsList != null)
    //{
    // var tobeDeleted = (from n in list
    //                    where !(from m in details.AvailedDetailsList select m.ServiceID).Contains(n.ServiceID)
    //                    select n).ToList();
    // var tobeInserted = (from n in details.AvailedDetailsList
    //                     where !(from m in list select m.ServiceID).Contains(n.ServiceID)
    //                     select n).ToList();
    // var forUpdate = (from n in details.AvailedDetailsList
    //                  where (from m in list select m.ServiceID).Contains(n.ServiceID)
    //                  select n).ToList();
    // //update    
    // if (forUpdate.Count != 0)  {
    //  foreach (var item in forUpdate)   {
    //   new Models.ServiceOptions().Update(item);
    //  }  
    // }

    // //delete
    // if (tobeDeleted.Count != 0)  {
    //  foreach (var item in tobeDeleted)  {
    //   new Models.ServiceOptions().Delete(item.ServiceOptionID, item.ServiceID);
    //  }
    // }

    // //Insert
    // if (tobeInserted.Count != 0)  {
    //  foreach (var item in tobeInserted)   {
    //   new Models.ServiceOptions().Insert(item);
    //  }
    // }  
    //}
    //else {
    // foreach(var item in list){
    //  new Models.ServiceOptions().Delete(item.ServiceOptionID, item.ServiceID);
    // }
    //}
    
    //Related Account Changes
    List<CMMC.Models.RelatedAccounts.Details> rdetails = new CMMC.Models.RelatedAccounts().GetList(details.GeneralDetails.CMSCode);
    try
    {
     if (details.RelatedAccountList != null)
     {
      if (rdetails.Count != 0)
      {
       var forRUpdate = (from n in details.RelatedAccountList
                         where (from m in rdetails select m.AccountID).Contains(n.AccountID)
                         select n).ToList();
       var forRInsert = (from n in details.RelatedAccountList
                         where !(from m in rdetails select m.AccountID).Contains(n.AccountID)
                         select n).ToList();
       var forRDelete = (from n in rdetails
                         where !(from m in details.RelatedAccountList select m.AccountID).Contains(n.AccountID)
                         select n).ToList();

       //forupdate
       if (forRUpdate.Count != 0)
       {
        foreach (var item in forRUpdate)
        {
         new Models.RelatedAccounts().Update(item);
        }
       }

       //forInsert
       if (forRInsert.Count != 0)
       {
        foreach (var item in forRInsert)
        {
         new Models.RelatedAccounts().Insert(item);
        }
       }

       //forDelete
       if (forRDelete.Count != 0)
       {
        foreach (var item in forRDelete)
        {
         new Models.RelatedAccounts().Delete(item.LinkedCMSCode, item.AccountID);
        }
       }
      }
      else {
       foreach(var item in details.RelatedAccountList){
        new Models.RelatedAccounts().Insert(item);
       }
      }
     }
     else
     {
      foreach(var item in rdetails)
      {
       new Models.RelatedAccounts().Delete(item.LinkedCMSCode, item.AccountID);
      }
     }
    }
   catch(Exception e){
    CTBC.Logs.Write("Save Update Related Account Changes", e.Message, "Enrollment");
   }

    //AccountInfo Changes
    List<CMMC.Models.AccountInformation.Details> aidetails = new CMMC.Models.AccountInformation().GetList(details.GeneralDetails.CMSCode);
    try
    {    
     if (details.AccountInformationList != null)
     {
      if (aidetails.Count != 0)
      {
       var forAUpdate = (from n in details.AccountInformationList
                         where (from m in aidetails select m.AccountNumber).Contains(n.AccountNumber)
                         select n).ToList();
       var forAInsert = (from n in details.AccountInformationList
                         where !(from m in aidetails select m.AccountNumber).Contains(n.AccountNumber)
                         select n).ToList();
       var forADelete = (from n in aidetails
                         where !(from m in details.AccountInformationList select m.AccountNumber).Contains(n.AccountNumber)
                         select n).ToList();

       //forupdate
       if (forAUpdate.Count != 0)
       {
        foreach (var item in forAUpdate)
        {
         new Models.AccountInformation().Update(item);
        }
       }
        
       //forInsert
       if (forAInsert.Count != 0)
       {
        foreach (var item in forAInsert)
        {
         new Models.AccountInformation().Insert(item);
        }
       }

       //forDelete
       if (forADelete.Count != 0)
       {
        foreach (var item in forADelete)
        {
         new Models.AccountInformation().Delete(item);
        }
       }
      }
      else
      {
       foreach (var item in details.AccountInformationList)
       {
        new Models.AccountInformation().Insert(item);
       }
      }
     }
     else
     {
      foreach (var item in aidetails)
      {
       new Models.AccountInformation().Delete(item);
      }
     }
    }
    catch (Exception ex)
    {
     CTBC.Logs.Write("Save Update Account Info Changes", ex.Message, "SYS_ACCESS_MODS");
    }
   }
   return details;
  }

  public int SaveUpdateRequest(CmsCodeDetails pdetails, Enrollment.RequestListDetails pRequestListDetails)
  {
   int intReturn = 0;
   bool ForInsert = false;

   bool AServices = false;

   CMSCode.Details details = new CMSCode.Details();
   details.BranchCode = pdetails.GeneralDetails.BranchCode;
   details.CMSCode = pdetails.GeneralDetails.CMSCode;
   details.Description = pdetails.GeneralDetails.Description;
   details.IsActive = pdetails.GeneralDetails.IsActive;
   details.CreatedOn = "null".ToString().ToDateTime();
   details.ModifiedBy = pdetails.GeneralDetails.ModifiedBy;
   details.ModifiedOn = pdetails.GeneralDetails.ModifiedOn;   
   details.Status = pdetails.GeneralDetails.Status;
   details.Tagging = pdetails.GeneralDetails.Tagging;
   details.BasePenalty = pdetails.GeneralDetails.BasePenalty;
   details.PenaltyFee = pdetails.GeneralDetails.PenaltyFee;
   details.IsAutoDebit = pdetails.GeneralDetails.IsAutoDebit;
   details.MaxFreeTransaction = pdetails.GeneralDetails.MaxFreeTransaction;
   details.MaxWithdrawalPaidByEmployer = pdetails.GeneralDetails.MaxWithdrawalPaidByEmployer;
   details.WithdrawalFeePerTransaction = pdetails.GeneralDetails.WithdrawalFeePerTransaction;
   pRequestListDetails.AffectedTable = "CMSCodes";
   pRequestListDetails.Remarks = "Request to update CMSCode";
   ForInsert = new Models.RequestList().IsForInsert(details, pRequestListDetails, pdetails.GeneralDetails.CMSCode);
      
   //Service Option Changes
   List<Models.ServiceOptions.AvailedDetails> list = new Models.ServiceOptions().GetEverything(details.CMSCode);
   if (pdetails.AvailedDetailsList != null)
    {
     var tobeDeleted = (from n in list
                        where !(from m in pdetails.AvailedDetailsList select m.ServiceID).Contains(n.ServiceID)
                        select n).ToList();
     var tobeInserted = (from n in pdetails.AvailedDetailsList
                         where !(from m in list select m.ServiceID).Contains(n.ServiceID)
                         select n).ToList();
     var forUpdate = (from n in pdetails.AvailedDetailsList
                      where (from m in list select m.ServiceID).Contains(n.ServiceID)
                      select n).ToList();
     //update    
     if (forUpdate.Count != 0)  {
      foreach (var item in forUpdate)   {
       AServices = AServices == true? true: new Models.RequestList().IsServiceForinsert(item, pRequestListDetails, false, pdetails.GeneralDetails.CMSCode);
       ForInsert = ForInsert ? true : AServices;
      }
     }

     //delete
     if (tobeDeleted.Count != 0)  {
      foreach (var item in tobeDeleted){
       pRequestListDetails.Action = "0";
       AServices = AServices == true ? true : new Models.RequestList().IsServiceForinsert(item, pRequestListDetails, false, pdetails.GeneralDetails.CMSCode);
      ForInsert = ForInsert ? true : AServices;
      }
     }

     //Insert
     if (tobeInserted.Count != 0) {
      foreach (var item in tobeInserted){
       AServices = AServices == true ? true : new Models.RequestList().IsServiceForinsert(item, pRequestListDetails, false, pdetails.GeneralDetails.CMSCode);
       ForInsert = ForInsert ? true : AServices;
      }
     }
    }
    else {
     foreach(var item in list){
      pRequestListDetails.Action = "0";
      AServices = AServices == true ? true : new Models.RequestList().IsServiceForinsert(item, pRequestListDetails, false, pdetails.GeneralDetails.CMSCode);
      ForInsert = ForInsert ? true : AServices;
     }    
    AServices = AServices? true : false;
    ForInsert = ForInsert ? true : AServices;
    }

   if(ForInsert){
    pRequestListDetails.Action = "1";
    intReturn = new Models.RequestList().InsertRequest(pRequestListDetails.RequestsDetails);
    pRequestListDetails.RequestCode = intReturn;
    pRequestListDetails.AffectedTable = "CMSCodes";
    pRequestListDetails.Remarks = "Request to update CMSCode";
    new Models.RequestList().InsertRequestList(details, pRequestListDetails, false, pdetails.GeneralDetails.CMSCode);
    if(AServices){
     if (pdetails.AvailedDetailsList != null)
    {
     var tobeDeleted = (from n in list
                        where !(from m in pdetails.AvailedDetailsList select m.ServiceID).Contains(n.ServiceID)
                        select n).ToList();
     var tobeInserted = (from n in pdetails.AvailedDetailsList
                         where !(from m in list select m.ServiceID).Contains(n.ServiceID)
                         select n).ToList();
     var forUpdate = (from n in pdetails.AvailedDetailsList
                      where (from m in list select m.ServiceID).Contains(n.ServiceID)
                      select n).ToList();
     //update    
     if (forUpdate.Count != 0)
     {
      foreach (var item in forUpdate)   {
      pRequestListDetails.Action = "1";
      new Models.RequestList().InsertRequestServiceandDetails(item, pRequestListDetails, false, pdetails.GeneralDetails.CMSCode);
      }
     }

     //delete
     if (tobeDeleted.Count != 0)
     {
      foreach (var item in tobeDeleted){
      pRequestListDetails.Action = "0";
      new Models.RequestList().InsertRequestServiceandDetails(item, pRequestListDetails, false, pdetails.GeneralDetails.CMSCode);
      }
     }

     //Insert
     if (tobeInserted.Count != 0)
     {
      foreach (var item in tobeInserted){
      pRequestListDetails.Action = "2";
      new Models.RequestList().InsertRequestServiceandDetails(item, pRequestListDetails, true, pdetails.GeneralDetails.CMSCode);
      }
     }
    }
    else
    {
     if (list.Count != 0)
     {
      foreach (var item in list)
      {
       pRequestListDetails.Action = "0";
       new Models.RequestList().InsertRequestServiceandDetails(item, pRequestListDetails, false, pdetails.GeneralDetails.CMSCode);
      }
     }
    }
   }
   }
   
    //Related Account Changes
    List<CMMC.Models.RelatedAccounts.Details> rdetails = new CMMC.Models.RelatedAccounts().GetList(details.CMSCode);
    try
    {
     if (pdetails.RelatedAccountList != null)
     {
      if (rdetails.Count != 0)
      {
       var forRUpdate = (from n in pdetails.RelatedAccountList
                         where (from m in rdetails select m.AccountID).Contains(n.AccountID)
                         select n).ToList();
       var forRInsert = (from n in pdetails.RelatedAccountList
                         where !(from m in rdetails select m.AccountID).Contains(n.AccountID)
                         select n).ToList();
       var forRDelete = (from n in rdetails
                         where !(from m in pdetails.RelatedAccountList select m.AccountID).Contains(n.AccountID)
                         select n).ToList();

       //forupdate
       if (forRUpdate.Count != 0)
       {
        foreach (var item in forRUpdate)
        {
         new Models.RelatedAccounts().Update(item);
        }
       }

       //forInsert
       if (forRInsert.Count != 0)
       {
        foreach (var item in forRInsert)
        {
         new Models.RelatedAccounts().Insert(item);
        }
       }

       //forDelete
       if (forRDelete.Count != 0)
       {
        foreach (var item in forRDelete)
        {
         new Models.RelatedAccounts().Delete(item.LinkedCMSCode, item.AccountID);
        }
       }
      }
      else
      {
       foreach (var item in pdetails.RelatedAccountList)
       {
        new Models.RelatedAccounts().Insert(item);
       }
      }
     }
     else
     {
      foreach (var item in rdetails)
      {
       new Models.RelatedAccounts().Delete(item.LinkedCMSCode, item.AccountID);
      }
     }
    }
    catch (Exception e)
    {
     CTBC.Logs.Write("Save Update Request Related Account Changes", e.Message, "SYS_ACCESS_MODS");
    }
   return intReturn;
  } 


  public CmsCodeDetails CancelandRemove(CmsCodeDetails details)
  {
   CMSCode.Details cDetails = new CMSCode.Details();
   cDetails.BranchCode = details.GeneralDetails.BranchCode;
   cDetails.CMSCode = details.GeneralDetails.CMSCode;
   cDetails.CreatedBy = details.GeneralDetails.CreatedBy;
   cDetails.CreatedOn = details.GeneralDetails.CreatedOn;
   cDetails.Description = details.GeneralDetails.Description;
   cDetails.IsActive = details.GeneralDetails.IsActive;
   cDetails.ModifiedBy = details.GeneralDetails.ModifiedBy;
   cDetails.ModifiedOn = details.GeneralDetails.ModifiedOn;
   cDetails.Status = details.GeneralDetails.Status;
   cDetails.Tagging = details.GeneralDetails.Tagging;
   cDetails.BasePenalty = details.GeneralDetails.BasePenalty;
   cDetails.PenaltyFee = details.GeneralDetails.PenaltyFee;
   cDetails.IsAutoDebit = details.GeneralDetails.IsAutoDebit;
   cDetails.MaxFreeTransaction = details.GeneralDetails.MaxFreeTransaction;
   cDetails.MaxWithdrawalPaidByEmployer = details.GeneralDetails.MaxWithdrawalPaidByEmployer;
   cDetails.WithdrawalFeePerTransaction = details.GeneralDetails.WithdrawalFeePerTransaction;
   
   if (details.GeneralDetails.Status == "4")
   {
    new Models.CMSCode().CancelCMSCode(cDetails);
   }
   else if (details.GeneralDetails.Status == "5")
   {
    new Models.CMSCode().RemoveCMSCode(cDetails);
   }  

   new Models.AccountInformation().CancelandRemove(details.GeneralDetails.CMSCode);
   new Models.PenaltyCharges().CancelandRemove(details.GeneralDetails.CMSCode);
   new Models.ServiceOptions().CancelandRemove(details.GeneralDetails.CMSCode);
   new Models.RelatedAccounts().CancelandRemove(details.GeneralDetails.CMSCode);

   return details;
  }

  public Pending GetPendingRequest()
  {
   Pending pending = new Pending();
   pending.PendingCMSCode = new Models.CMSCode().GetPendingRequest().PendingRequest;
   pending.PendingAccounts = new Models.AccountInformation().GetPendingRequest().PendingRequest;
   pending.PendingServices = new Models.ServiceOptions().GetPendingRequest().PendingRequest;
   pending.PendingRelatedAccounts = new Models.RelatedAccounts().GetPendingRequest().PendingRequest;
   return pending;
  }

  

  //PENALTY CHARGES
  public struct PenaltyCharges
  {
   public int CMSCode { get; set; }
   public string DebitAccountNo { get; set; }
   public decimal BasePenalty { get; set; }
   public decimal PenaltyFee { get; set; }
   public bool IsAutoDebit { get; set; }
   public string Status { get; set; }
  }
  
     //Service Details
  public struct ServiceOptionDetails
  {
   public int ServiceOptionID { get; set; }
   public int CMSCode { get; set; }
   public string AccountNoServiceType { get; set; }
   public decimal MotherRequiredADB { get; set; }
   public decimal SubRequiredADB { get; set; }
   public string MinNumberEmployee { get; set; }
   public int MaxFreeTransaction { get; set; }
   public string MaxWithdrawalPaidByEmployer { get; set; }
   public decimal WithdrawalFeePerTransaction { get; set; }
   public string WithdrawalFeeAccountNo { get; set; }
   public string PayrollFrequency { get; set; }
   public string Remarks { get; set; }
   public string Status { get; set; }
  }

  //ACCOUNT INFORMATION
  public struct AccountInformation
  {
   public int ID { get; set; }
   public int CMSCode { get; set; }
   public string AccountNumber { get; set; }
   public string AccountName { get; set; }
   public string BranchCode { get; set; }
   public DateTime DateEnrolled { get; set; }
   public string InvestmentType { get; set; }
   public DateTime EffectivityDate { get; set; }
   public string Tag { get; set; }
  }

  //RELATED ACCOUNTS
  public struct RelatedAccounts
  {
   public List<string> RelatedAccountsName { get; set; }
   public string AccountName { get; set; }
   public DateTime DateAdded { get; set; }
   public string Status { get; set; }
  }


 }
}