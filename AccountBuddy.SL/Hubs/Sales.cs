using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AccountBuddy.Common;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {

        #region Sales
     
        public bool Sales_Save(BLL.Sale P)
        {
            try
            {
                
                DAL.Sale d = DB.Sales.Where(x => x.Id == P.Id).FirstOrDefault();

                if (d == null)
                {

                    d = new DAL.Sale();
                    DB.Sales.Add(d);

                    P.toCopy<DAL.Sale>(d);

                    foreach (var b_pod in P.SDetails)
                    {
                        DAL.SalesDetail d_pod = new DAL.SalesDetail();
                        b_pod.toCopy<DAL.SalesDetail>(d_pod);
                        d.SalesDetails.Add(d_pod);
                    }
                    DB.SaveChanges();

                    P.Id = d.Id;
                    LogDetailStore(P, LogDetailType.INSERT);
                }
                else
                {
                    foreach (var d_Sd in d.SalesDetails)
                    {
                        BLL.SalesDetail b_Sd = P.SDetails.Where(x => x.Id == d_Sd.Id).FirstOrDefault();
                        if (b_Sd == null) d.SalesDetails.Remove(d_Sd);
                    }

                    P.toCopy<DAL.Sale>(d);
                    foreach (var b_Sd in P.SDetails)
                    {
                        DAL.SalesDetail d_Sd = d.SalesDetails.Where(x => x.Id == b_Sd.Id).FirstOrDefault();
                        if (d_Sd == null)
                        {
                            d_Sd = new DAL.SalesDetail();
                            d.SalesDetails.Add(d_Sd);
                        }
                        b_Sd.toCopy<DAL.SalesDetail>(d_Sd);
                    }
                    LogDetailStore(P, LogDetailType.UPDATE);
                   
                }
                Journal_SaveBySales(P);
                Purchase_SaveBySales(P);
                return true;
            }
            catch (Exception ex) { }
            return false;
        }

       void Sales_SaveByPurchase(BLL.Purchase P)
        {
            var refNo = string.Format("SAL-{0}", P.Id);

            DAL.Sale s = DB.Sales.Where(x => x.RefNo == refNo).FirstOrDefault();
            if (s != null)
            {
                DB.SalesDetails.RemoveRange(s.SalesDetails);
                DB.Sales.Remove(s);
                DB.SaveChanges();
            }
            var pd = P.PDetails.FirstOrDefault();
            var ld = DB.Ledgers.Where(x => x.Id == P.LedgerId).FirstOrDefault();

            if (ld.LedgerName.StartsWith("CM-") || ld.LedgerName.StartsWith("WH-") || ld.LedgerName.StartsWith("DL-"))
            {
                var LName = LedgerNameByCompanyId(Caller.CompanyId);

                var CId = CompanyIdByLedgerName(ld.LedgerName);

                s = new DAL.Sale();
                s.RefNo = refNo;
                s.SalesDate  =P.PurchaseDate;
                s.DiscountAmount = P.DiscountAmount;
                s.ExtraAmount = P.ExtraAmount;
                s.GSTAmount = P.GSTAmount;
                s.ItemAmount = P.ItemAmount;
                s.TotalAmount = P.TotalAmount;
                s.LedgerId = LedgerIdByCompany(LName, CId);
                s.TransactionTypeId = P.TransactionTypeId;
                if (CId != 0)
                {
                    foreach (var b_pod in P.PDetails)
                    {
                        DAL.SalesDetail d_pod = new DAL.SalesDetail();
                        b_pod.toCopy<DAL.SalesDetail>(d_pod);
                        s.SalesDetails.Add(d_pod);
                    }
                    DB.Sales.Add(s);
                    DB.SaveChanges();
                    var Sal = Sales_DALtoBLL(s);
                    Sal.TransactionType=P.TransactionType;
                    Journal_SaveBySales(Sal);
                }
            }
        }
        public bool Sales_DeleteByPurchase(BLL.Purchase P)
        {
            try
            {
                var LName = DB.Ledgers.Where(x => x.Id == P.LedgerId).FirstOrDefault().LedgerName;

                if (LName.StartsWith("CM-") || LName.StartsWith("WH-"))
                {

                    DAL.Sale d = DB.Sales.Where(x => x.RefNo == P.RefNo && x.Ledger.AccountGroup.CompanyId == Caller.UnderCompanyId).FirstOrDefault();

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



        public BLL.Sale Sales_Find(string SearchText)
        {
            BLL.Sale P = new BLL.Sale();
            try
            {

                DAL.Sale d = DB.Sales.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.RefNo == SearchText).FirstOrDefault();
                DB.Entry(d).Reload();
                if (d != null)
                {

                    d.toCopy<BLL.Sale>(P);
                    P.LedgerName = (d.Ledger ?? DB.Ledgers.Find(d.LedgerId) ?? new DAL.Ledger()).LedgerName;
                    P.TransactionType = (d.TransactionType ?? DB.TransactionTypes.Find(d.TransactionTypeId) ?? new DAL.TransactionType()).Type;
                    foreach (var d_pod in d.SalesDetails)
                    {
                        BLL.SalesDetail b_pod = new BLL.SalesDetail();
                        d_pod.toCopy<BLL.SalesDetail>(b_pod);
                        P.SDetails.Add(b_pod);
                        b_pod.ProductName = (d_pod.Product ?? DB.Products.Find(d_pod.ProductId) ?? new DAL.Product()).ProductName;
                        b_pod.UOMName = (d_pod.UOM ?? DB.UOMs.Find(d_pod.UOMId) ?? new DAL.UOM()).Symbol;
                    }

                }
            }
            catch (Exception ex) { }
            return P;
        }

        public bool Sales_Delete(long pk)
        {
            try
            {
                DAL.Sale d = DB.Sales.Where(x => x.Id == pk).FirstOrDefault();

                if (d != null)
                {
                    var P = Sales_DALtoBLL(d);
                    DB.SalesDetails.RemoveRange(d.SalesDetails);
                    DB.Sales.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(P, LogDetailType.DELETE);
                    Journal_DeleteBySales(P);
                }
                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public BLL.Sale Sales_DALtoBLL(DAL.Sale d)
        {
            BLL.Sale S = d.toCopy<BLL.Sale>(new BLL.Sale());
            foreach (var d_Sd in d.SalesDetails)
            {
                S.SDetails.Add(d_Sd.toCopy<BLL.SalesDetail>(new BLL.SalesDetail()));
            }
            return S;
        }
        public bool Find_SRef(string RefNo, BLL.Sale PO)

        {
            DAL.Sale d = DB.Sales.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.RefNo == RefNo & x.Id != PO.Id).FirstOrDefault();
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