using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AccountBuddy.Common;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        #region Purchase      

        public bool Purchase_Save(BLL.Purchase P)
        {
            try
            {

                DAL.Purchase d = DB.Purchases.Where(x => x.Id == P.Id).FirstOrDefault();

                if (d == null)
                {

                    d = new DAL.Purchase();
                    DB.Purchases.Add(d);


                    P.toCopy<DAL.Purchase>(d);

                    foreach (var b_pod in P.PDetails)
                    {
                        DAL.PurchaseDetail d_pod = new DAL.PurchaseDetail();
                        b_pod.toCopy<DAL.PurchaseDetail>(d_pod);
                        d.PurchaseDetails.Add(d_pod);
                    }
                    DB.SaveChanges();

                    DAL.Journal j = new DAL.Journal();
                    j.EntryNo = string.Format("PUR-{0}", P.RefNo);
                    j.JournalDate = P.PurchaseDate;                    
                    DB.Journals.Add(j);

                    DAL.JournalDetail jD = new DAL.JournalDetail();                   
                    if(P.TransactionType=="Cash")
                    {                        
                        jD.LedgerId = DB.DataKeyValues.Where(x => x.CompanyId == Caller.CompanyId && x.DataKey == BLL.DataKeyValue.CashLedger_Key).FirstOrDefault().DataValue;
                    }
                    else
                    {
                        jD.LedgerId = P.LedgerId;
                    }
                    jD.Particulars = "Purchase";
                    jD.CrAmt = P.TotalAmount;
                    DB.JournalDetails.Add(jD);
                    DB.SaveChanges();

                    jD.JournalId = j.Id;
                    jD.LedgerId = DB.Ledgers.Where(x => x.LedgerName == BLL.DataKeyValue.PurchaseAccount_Key).Select(x => x.Id).FirstOrDefault();
                    jD.Particulars = "Purchase Account";
                    jD.DrAmt = P.GSTAmount;
                    DB.JournalDetails.Add(jD);
                    DB.SaveChanges();

                    jD.JournalId = j.Id;
                    jD.LedgerId = DB.Ledgers.Where(x => x.LedgerName == BLL.DataKeyValue.Output_Tax_Ledger_Key).Select(x => x.Id).FirstOrDefault();
                    jD.Particulars = "Purchase Tax";
                    jD.DrAmt = P.GSTAmount;
                    DB.JournalDetails.Add(jD);
                    DB.SaveChanges();

                    jD.JournalId = j.Id;
                    jD.LedgerId = DB.Ledgers.Where(x => x.LedgerName == BLL.DataKeyValue.Profit_Loss_Ledger_Key).Select(x => x.Id).FirstOrDefault();
                    jD.Particulars = "Extras";
                    jD.DrAmt = P.ExtraAmount;
                    DB.JournalDetails.Add(jD);
                    DB.SaveChanges();

                    j.JournalDetails.Add(jD);

                    LogDetailStore(P, LogDetailType.INSERT);
                }
                else
                {
                    P.toCopy<DAL.Purchase>(d);
                    foreach (var b_pod in P.PDetails)
                    {
                        DAL.PurchaseDetail d_pod = new DAL.PurchaseDetail();
                        b_pod.toCopy<DAL.PurchaseDetail>(d_pod);
                        d.PurchaseDetails.Add(d_pod);
                    }
                    DB.SaveChanges();
                    LogDetailStore(P, LogDetailType.UPDATE);
                }

                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public BLL.Purchase Purchase_Find(string SearchText)
        {
            BLL.Purchase P = new BLL.Purchase();
            try
            {

                DAL.Purchase d = DB.Purchases.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.RefNo == SearchText).FirstOrDefault();
                DB.Entry(d).Reload();
                if (d != null)
                {

                    d.toCopy<BLL.Purchase>(P);
                    P.LedgerName = (d.Ledger ?? DB.Ledgers.Find(d.LedgerId) ?? new DAL.Ledger()).LedgerName;
                    P.TransactionType = (d.TransactionType ?? DB.TransactionTypes.Find(d.TransactionTypeId) ?? new DAL.TransactionType()).Type;
                    foreach (var d_pod in d.PurchaseDetails)
                    {
                        BLL.PurchaseDetail b_pod = new BLL.PurchaseDetail();
                        d_pod.toCopy<BLL.PurchaseDetail>(b_pod);
                        P.PDetails.Add(b_pod);
                        b_pod.ProductName = (d_pod.Product ?? DB.Products.Find(d_pod.ProductId) ?? new DAL.Product()).ProductName;
                        b_pod.UOMName = (d_pod.UOM ?? DB.UOMs.Find(d_pod.UOMId) ?? new DAL.UOM()).Symbol;
                    }

                }
            }
            catch (Exception ex) { }
            return P;
        }

        public bool Purchase_Delete(long pk)
        {
            try
            {
                DAL.Purchase d = DB.Purchases.Where(x => x.Id == pk).FirstOrDefault();

                if (d != null)
                {
                    BLL.Purchase P = new BLL.Purchase();
                    d.toCopy<BLL.Purchase>(P);
                    P.LedgerName = d.Ledger.LedgerName;
                    P.TransactionType = d.TransactionType.Type;
                    foreach (var d_pod in d.PurchaseDetails)
                    {
                        BLL.PurchaseDetail b_pod = new BLL.PurchaseDetail();
                        d_pod.toCopy<BLL.PurchaseDetail>(b_pod);
                        P.PDetails.Add(b_pod);

                    }
                    DB.PurchaseDetails.RemoveRange(d.PurchaseDetails);
                    DB.Purchases.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(P, LogDetailType.DELETE);
                }
                return true;
            }
            catch (Exception ex) { }
            return false;
        }
        
        public bool Find_PRef(string RefNo, BLL.Purchase PO)

        {
            DAL.Purchase d = DB.Purchases.Where(x => x.Ledger.AccountGroup.CompanyId==Caller.CompanyId && x.RefNo == RefNo & x.Id != PO.Id).FirstOrDefault();
            if (d == null)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        #endregion
    }
}