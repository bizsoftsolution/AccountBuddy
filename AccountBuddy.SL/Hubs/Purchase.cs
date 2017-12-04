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
        public string Purchase_NewRefNo()
        {
            return Purchase_NewRefNoByCompanyId(Caller.CompanyId);
        }
        public string Purchase_NewRefNoByCompanyId(int CompanyId)
        {
            DateTime dt = DateTime.Now;
            string Prefix = string.Format("{0}{1:yy}{2:X}", BLL.FormPrefix.Purchase, dt, dt.Month);
            long No = 0;

            var d = Caller.DB.Purchases.Where(x => x.Ledger.AccountGroup.CompanyId == CompanyId && x.RefNo.StartsWith(Prefix))
                                     .OrderByDescending(x => x.RefNo)
                                     .FirstOrDefault();

            if (d != null) No = Convert.ToInt64(d.RefNo.Substring(Prefix.Length), 16);

            return string.Format("{0}{1:X5}", Prefix, No + 1);
        }
        public bool Purchase_Save(BLL.Purchase P)
        {
            try
            {

                DAL.Purchase d = Caller.DB.Purchases.Where(x => x.Id == P.Id).FirstOrDefault();

                if (d == null)
                {

                    d = new DAL.Purchase();
                    Caller.DB.Purchases.Add(d);


                    P.toCopy<DAL.Purchase>(d);

                    foreach (var b_pod in P.PDetails)
                    {
                        DAL.PurchaseDetail d_pod = new DAL.PurchaseDetail();
                        b_pod.toCopy<DAL.PurchaseDetail>(d_pod);
                        d.PurchaseDetails.Add(d_pod);
                    }
                    Caller.DB.SaveChanges();
                    P.Id = d.Id;

                    LogDetailStore(P, LogDetailType.INSERT);
                }
                else
                {

                    //foreach (var d_Pd in d.PurchaseDetails.ToList())
                    //{
                    //    BLL.PurchaseDetail b_Pd = P.PDetails.Where(x => x.Id == d_Pd.Id).FirstOrDefault();
                    //    if (b_Pd == null) d.PurchaseDetails.Remove(d_Pd);
                    //}

                    decimal rd = P.PDetails.Select(X => X.PurchaseId).FirstOrDefault();
                    Caller.DB.PurchaseDetails.RemoveRange(d.PurchaseDetails.Where(x => x.PurchaseId == rd).ToList());

                    P.toCopy<DAL.Purchase>(d);
                    foreach (var b_Pd in P.PDetails)
                    {
                        // DAL.PurchaseDetail d_Pd = d.PurchaseDetails.Where(x => x.Id == b_Pd.Id).FirstOrDefault();
                        // if (d_Pd == null)
                        // {
                        DAL.PurchaseDetail d_Pd = new DAL.PurchaseDetail();
                            d.PurchaseDetails.Add(d_Pd);
                      //  }
                        b_Pd.toCopy<DAL.PurchaseDetail>(d_Pd);
                    }
                    Caller.DB.SaveChanges();
                    LogDetailStore(P, LogDetailType.UPDATE);
                }
                Clients.Clients(OtherLoginClients).Purchase_RefNoRefresh(Purchase_NewRefNo());
                Journal_SaveByPurchase(d);
                Sales_SaveByPurchase(d);
                return true;
            }

            catch (Exception ex) { return false; }
            
        }

        #region Sales
        void Purchase_SaveBySales(DAL.Sale S)
        {
            string RefCode = string.Format("{0}{1}", BLL.FormPrefix.Sales, S.Id);

            DAL.Purchase p = Caller.DB.Purchases.Where(x => x.RefCode == RefCode).FirstOrDefault();

            if (S.Ledger.LedgerName.StartsWith("CM-") || S.Ledger.LedgerName.StartsWith("WH-") || S.Ledger.LedgerName.StartsWith("DL-"))
            {
                var LName = LedgerNameByCompanyId(Caller.CompanyId);
                var CId = CompanyIdByLedgerName(S.Ledger.LedgerName);
                var LId = LedgerIdByCompany(LName, CId);

                if (LId != 0)
                {
                    if (p == null)
                    {
                        p = new DAL.Purchase();
                        p.RefNo = Purchase_NewRefNoByCompanyId(CId);
                        p.RefCode = RefCode;
                        Caller.DB.Purchases.Add(p);
                    }
                    else
                    {
                        Caller.DB.PurchaseDetails.RemoveRange(p.PurchaseDetails);
                    }

                    p.PurchaseDate = S.SalesDate;
                    p.DiscountAmount = S.DiscountAmount;
                    p.ExtraAmount = S.ExtraAmount;
                    p.GSTAmount = S.GSTAmount;
                    p.ItemAmount = S.ItemAmount;
                    p.TotalAmount = S.TotalAmount;
                    p.LedgerId = LId;
                    p.TransactionTypeId = S.TransactionTypeId;
                    foreach (var b_pod in S.SalesDetails)
                    {
                        DAL.PurchaseDetail d_pod = new DAL.PurchaseDetail();
                        b_pod.toCopy<DAL.PurchaseDetail>(d_pod);
                        p.PurchaseDetails.Add(d_pod);
                    }
                    Caller.DB.SaveChanges();
                    Journal_SaveByPurchase(p);
                }
            }


        }
        public bool Purchase_DeleteBySales(DAL.Sale s)
        {
            try
            {
                string RefCode = string.Format("{0}{1}", BLL.FormPrefix.Sales, s.Id);
                DAL.Purchase d = Caller.DB.Purchases.Where(x => x.RefCode == RefCode).FirstOrDefault();
                if (d != null)
                {
                    Purchase_Delete(d.Id);
                }


            }
            catch (Exception ex) { }
            return false;
        }

        #endregion


        public BLL.Purchase Purchase_Find(string SearchText)
        {
            BLL.Purchase P = new BLL.Purchase();
            try
            {

                DAL.Purchase d = Caller.DB.Purchases.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.RefNo == SearchText).FirstOrDefault();
                Caller.DB.Entry(d).Reload();
                if (d != null)
                {

                    d.toCopy<BLL.Purchase>(P);
                    P.LedgerName = (d.Ledger ?? Caller.DB.Ledgers.Find(d.LedgerId) ?? new DAL.Ledger()).LedgerName;
                    P.TransactionType = (d.TransactionType ?? Caller.DB.TransactionTypes.Find(d.TransactionTypeId) ?? new DAL.TransactionType()).Type;
                    foreach (var d_pod in d.PurchaseDetails)
                    {
                        BLL.PurchaseDetail b_pod = new BLL.PurchaseDetail();
                        d_pod.toCopy<BLL.PurchaseDetail>(b_pod);
                        P.PDetails.Add(b_pod);
                        b_pod.ProductName = (d_pod.Product ?? Caller.DB.Products.Find(d_pod.ProductId) ?? new DAL.Product()).ProductName;
                        b_pod.UOMName = (d_pod.UOM ?? Caller.DB.UOMs.Find(d_pod.UOMId) ?? new DAL.UOM()).Symbol;
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
                DAL.Purchase d = Caller.DB.Purchases.Where(x => x.Id == pk).FirstOrDefault();

                if (d != null)
                {
                    var P = Purchase_DALtoBLL(d);
                    Caller.DB.PurchaseDetails.RemoveRange(d.PurchaseDetails);
                    Caller.DB.Purchases.Remove(d);
                    Caller.DB.SaveChanges();
                    LogDetailStore(P, LogDetailType.DELETE);
                    Journal_DeleteByPurchase(P);
                    Sales_DeleteByPurchase(d);
                    return true;
                }
                Clients.Clients(OtherLoginClients).Purchase_RefNoRefresh(Purchase_NewRefNo());

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
            DAL.Purchase d = Caller.DB.Purchases.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.RefNo == RefNo & x.Id != PO.Id).FirstOrDefault();
            if (d == null)
            {
                return false;
            }
            else
            {
                return true;
            }

        }
        public BLL.Purchase Purchase_FindById(int ID)
        {
            BLL.Purchase P = new BLL.Purchase();
            try
            {

                DAL.Purchase d = Caller.DB.Purchases.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.Id == ID).FirstOrDefault();
                Caller.DB.Entry(d).Reload();
                if (d != null)
                {

                    d.toCopy<BLL.Purchase>(P);
                    P.LedgerName = (d.Ledger ?? Caller.DB.Ledgers.Find(d.LedgerId) ?? new DAL.Ledger()).LedgerName;
                    P.TransactionType = (d.TransactionType ?? Caller.DB.TransactionTypes.Find(d.TransactionTypeId) ?? new DAL.TransactionType()).Type;
                    foreach (var d_pod in d.PurchaseDetails)
                    {
                        BLL.PurchaseDetail b_pod = new BLL.PurchaseDetail();
                        d_pod.toCopy<BLL.PurchaseDetail>(b_pod);
                        P.PDetails.Add(b_pod);
                        b_pod.ProductName = (d_pod.Product ?? Caller.DB.Products.Find(d_pod.ProductId) ?? new DAL.Product()).ProductName;
                        b_pod.UOMName = (d_pod.UOM ?? Caller.DB.UOMs.Find(d_pod.UOMId) ?? new DAL.UOM()).Symbol;
                    }

                }
            }
            catch (Exception ex) { }
            return P;
        }

        #endregion
    }
}