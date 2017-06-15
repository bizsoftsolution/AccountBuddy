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
                    P.Id = d.Id;

                    LogDetailStore(P, LogDetailType.INSERT);
                }
                else
                {

                    foreach (var d_Pd in d.PurchaseDetails)
                    {
                        BLL.PurchaseDetail b_Pd = P.PDetails.Where(x => x.Id == d_Pd.Id).FirstOrDefault();
                        if (b_Pd == null) d.PurchaseDetails.Remove(d_Pd);
                    }

                    P.toCopy<DAL.Purchase>(d);
                    foreach (var b_Pd in P.PDetails)
                    {
                        DAL.PurchaseDetail d_Pd = d.PurchaseDetails.Where(x => x.Id == b_Pd.Id).FirstOrDefault();
                        if (d_Pd == null)
                        {
                            d_Pd = new DAL.PurchaseDetail();
                            d.PurchaseDetails.Add(d_Pd);
                        }
                        b_Pd.toCopy<DAL.PurchaseDetail>(d_Pd);
                    }
                    DB.SaveChanges();
                    LogDetailStore(P, LogDetailType.UPDATE);
                }
                Journal_SaveByPurchase(P);
                Sales_SaveByPurchase(P);
                return true;
            }
            catch (Exception ex) { }
            return false;
        }

       void Purchase_SaveBySales(BLL.Sale S)
        {
            var refNo = string.Format("PUR-{0}", S.Id);

            DAL.Purchase p = DB.Purchases.Where(x => x.RefNo == refNo).FirstOrDefault();
            if (p != null)
            {
                DB.PurchaseDetails.RemoveRange(p.PurchaseDetails);
                DB.Purchases.Remove(p);
                DB.SaveChanges();
            }
            var pd = S.SDetails.FirstOrDefault();
            var ld = DB.Ledgers.Where(x => x.Id == S.LedgerId).FirstOrDefault();

            if (ld.LedgerName.StartsWith("CM-") || ld.LedgerName.StartsWith("WH-") || ld.LedgerName.StartsWith("DL-"))
            {
                var LName = LedgerNameByCompanyId(Caller.CompanyId);

                var CId = CompanyIdByLedgerName(ld.LedgerName);

                p = new DAL.Purchase();                
                p.RefNo = refNo;
                p.PurchaseDate = S.SalesDate;
                p.DiscountAmount = S.DiscountAmount;
                p.ExtraAmount = S.ExtraAmount;
                p.GSTAmount = S.GSTAmount;
                p.ItemAmount = S.ItemAmount;
                p.TotalAmount = S.TotalAmount;
                p.LedgerId = LedgerIdByCompany(LName, CId);
                p.TransactionTypeId = S.TransactionTypeId;
                if (CId != 0)
                {
                    foreach (var b_pod in S.SDetails)
                    {
                        DAL.PurchaseDetail d_pod = new DAL.PurchaseDetail();
                        b_pod.toCopy<DAL.PurchaseDetail>(d_pod);
                        p.PurchaseDetails.Add(d_pod);
                    }
                    DB.Purchases.Add(p);
                    DB.SaveChanges();
                    var Pur = Purchase_DALtoBLL(p);
                    Pur.TransactionType = S.TransactionType;
                    Journal_SaveByPurchase(Pur);
                }                
            }
            
        }
    

    public bool Purchase_DeleteBySales(BLL.Sale s)
    {
        try
        {
            var LName = DB.Ledgers.Where(x => x.Id == s.LedgerId).FirstOrDefault().LedgerName;

            if (LName.StartsWith("CM-") || LName.StartsWith("WH-"))
            {

                DAL.Sale d = DB.Sales.Where(x => x.RefNo == s.RefNo && x.Ledger.AccountGroup.CompanyId == Caller.UnderCompanyId).FirstOrDefault();

                if (d != null)
                {
                    DB.SalesDetails.RemoveRange(d.SalesDetails);
                    DB.Sales.Remove(d);
                    DB.SaveChanges();
                }

                return true;
            }


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
                var P = Purchase_DALtoBLL(d);
                DB.PurchaseDetails.RemoveRange(d.PurchaseDetails);
                DB.Purchases.Remove(d);
                DB.SaveChanges();
                LogDetailStore(P, LogDetailType.DELETE);
                Journal_DeleteByPurchase(P);
                return true;
            }

        }
        catch (Exception ex) { }
        return false;
    }
    public BLL.Purchase Purchase_DALtoBLL(DAL.Purchase d)
    {
        BLL.Purchase P = d.toCopy<BLL.Purchase>(new BLL.Purchase());
        foreach (var d_Pd in d.PurchaseDetails)
        {
            P.PDetails.Add(d_Pd.toCopy<BLL.PurchaseDetail>(new BLL.PurchaseDetail()));
        }
        return P;
    }
    public bool Find_PRef(string RefNo, BLL.Purchase PO)

    {
        DAL.Purchase d = DB.Purchases.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.RefNo == RefNo & x.Id != PO.Id).FirstOrDefault();
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