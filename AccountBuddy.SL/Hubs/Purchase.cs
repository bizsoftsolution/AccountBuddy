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

            var d = DB.Purchases.Where(x => x.Ledger.AccountGroup.CompanyId == CompanyId && x.RefNo.StartsWith(Prefix))
                                     .OrderByDescending(x => x.RefNo)
                                     .FirstOrDefault();

            if (d != null) No = Convert.ToInt64(d.RefNo.Substring(Prefix.Length), 16);

            return string.Format("{0}{1:X5}", Prefix, No + 1);
        }
        public bool Purchase_Save(BLL.Purchase P)
        {
            try
            {

                DAL.Purchase d = DB.Purchases.Where(x => x.Id == P.Id).FirstOrDefault();

                if (d == null)
                {
                    d = new DAL.Purchase();
                    DB.Purchases.Add(d);
                    P.ToMap<DAL.Purchase>(d);
                    foreach (var b_pod in P.PDetails)
                    {
                        DAL.PurchaseDetail d_pod = new DAL.PurchaseDetail();
                        b_pod.ToMap<DAL.PurchaseDetail>(d_pod);
                        d.PurchaseDetails.Add(d_pod);
                    }
                    DB.SaveChanges();
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
                    DB.PurchaseDetails.RemoveRange(d.PurchaseDetails.Where(x => x.PurchaseId == rd).ToList());

                    P.ToMap<DAL.Purchase>(d);
                    foreach (var b_Pd in P.PDetails)
                    {
                        // DAL.PurchaseDetail d_Pd = d.PurchaseDetails.Where(x => x.Id == b_Pd.Id).FirstOrDefault();
                        // if (d_Pd == null)
                        // {
                        DAL.PurchaseDetail d_Pd = new DAL.PurchaseDetail();
                            d.PurchaseDetails.Add(d_Pd);
                      //  }
                        b_Pd.ToMap<DAL.PurchaseDetail>(d_Pd);
                    }
                    DB.SaveChanges();
                    LogDetailStore(P, LogDetailType.UPDATE);
                }
                if (OtherClientsOnGroup.Count > 0) Clients.Clients(OtherClientsOnGroup).Purchase_RefNoRefresh(Purchase_NewRefNo());
                Journal_SaveByPurchase(d);
                Sales_SaveByPurchase(d);
                return true;
            }

            catch (Exception ex) { Common.AppLib.WriteLog(ex); return false; }
            
        }

        #region Sales
        void Purchase_SaveBySales(DAL.Sale S)
        {
            try
            {
                string RefCode = string.Format("{0}{1}", BLL.FormPrefix.Sales, S.Id);

                DAL.Purchase p = DB.Purchases.Where(x => x.RefCode == RefCode).FirstOrDefault();

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
                            DB.Purchases.Add(p);
                        }
                        else
                        {
                            DB.PurchaseDetails.RemoveRange(p.PurchaseDetails);
                        }

                        p.PurchaseDate = S.SalesDate;
                        p.DiscountAmount = S.DiscountAmount;
                        p.ExtraAmount = S.ExtraAmount;
                        p.GSTAmount = S.GSTAmount;
                        p.ItemAmount = S.ItemAmount;
                        p.TotalAmount = S.TotalAmount;
                        p.LedgerId = LId;
                        p.TransactionTypeId = S.TransactionTypeId;
                        p.Narration = S.Narration;
                        foreach (var b_pod in S.SalesDetails)
                        {
                            DAL.PurchaseDetail d_pod = new DAL.PurchaseDetail();
                            b_pod.ToMap<DAL.PurchaseDetail>(d_pod);
                            p.PurchaseDetails.Add(d_pod);
                        }
                        DB.SaveChanges();
                        Journal_SaveByPurchase(p);
                    }
                }
            }
            catch(Exception ex)
            {
                Common.AppLib.WriteLog(ex);
            }

        }
        public bool Purchase_DeleteBySales(DAL.Sale s)
        {
            try
            {
                string RefCode = string.Format("{0}{1}", BLL.FormPrefix.Sales, s.Id);
                DAL.Purchase d = DB.Purchases.Where(x => x.RefCode == RefCode).FirstOrDefault();
                if (d != null)
                {
                    Purchase_Delete(d.Id);
                }


            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return false;
        }

        #endregion


        public BLL.Purchase Purchase_Find(string SearchText)
        {
            BLL.Purchase P = new BLL.Purchase();
            try
            {

                DAL.Purchase d = DB.Purchases.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.RefNo == SearchText).FirstOrDefault();
                DB.Entry(d).Reload();
                if (d != null)
                {

                    d.ToMap<BLL.Purchase>(P);
                    P.LedgerName = (d.Ledger ?? DB.Ledgers.Find(d.LedgerId) ?? new DAL.Ledger()).LedgerName;
                    P.TransactionType = (d.TransactionType ?? DB.TransactionTypes.Find(d.TransactionTypeId) ?? new DAL.TransactionType()).Type;

                    int i = 0;
                    foreach (var d_pod in d.PurchaseDetails)
                    {
                        BLL.PurchaseDetail b_pod = new BLL.PurchaseDetail();
                        d_pod.ToMap<BLL.PurchaseDetail>(b_pod);
                        P.PDetails.Add(b_pod);
                        b_pod.SNo = ++i;
                        b_pod.ProductName = (d_pod.Product ?? DB.Products.Find(d_pod.ProductId) ?? new DAL.Product()).ProductName;
                        b_pod.UOMName = (d_pod.UOM ?? DB.UOMs.Find(d_pod.UOMId) ?? new DAL.UOM()).Symbol;
                    }

                }
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
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
                    Sales_DeleteByPurchase(d);
                    return true;
                }
                if (OtherClientsOnGroup.Count > 0) Clients.Clients(OtherClientsOnGroup).Purchase_RefNoRefresh(Purchase_NewRefNo());

            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return false;
        }
        public BLL.Purchase Purchase_DALtoBLL(DAL.Purchase d)
        {
            BLL.Purchase P = d.ToMap<BLL.Purchase>(new BLL.Purchase());
            foreach (var d_Pd in d.PurchaseDetails)
            {
                P.PDetails.Add(d_Pd.ToMap<BLL.PurchaseDetail>(new BLL.PurchaseDetail()));
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
        public BLL.Purchase Purchase_FindById(int ID)
        {
            BLL.Purchase P = new BLL.Purchase();
            try
            {

                DAL.Purchase d = DB.Purchases.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.Id == ID).FirstOrDefault();
                DB.Entry(d).Reload();
                if (d != null)
                {

                    d.ToMap<BLL.Purchase>(P);
                    P.LedgerName = (d.Ledger ?? DB.Ledgers.Find(d.LedgerId) ?? new DAL.Ledger()).LedgerName;
                    P.TransactionType = (d.TransactionType ?? DB.TransactionTypes.Find(d.TransactionTypeId) ?? new DAL.TransactionType()).Type;
                    foreach (var d_pod in d.PurchaseDetails)
                    {
                        BLL.PurchaseDetail b_pod = new BLL.PurchaseDetail();
                        d_pod.ToMap<BLL.PurchaseDetail>(b_pod);
                        P.PDetails.Add(b_pod);
                        b_pod.ProductName = (d_pod.Product ?? DB.Products.Find(d_pod.ProductId) ?? new DAL.Product()).ProductName;
                        b_pod.UOMName = (d_pod.UOM ?? DB.UOMs.Find(d_pod.UOMId) ?? new DAL.UOM()).Symbol;
                    }

                }
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return P;
        }


        public List<BLL.Purchase> Purchase_List(int? LedgerId, int? TType, DateTime dtFrom, DateTime dtTo, string BillNo, decimal amtFrom, decimal amtTo)
        {
            List<BLL.Purchase> lstPurchase = new List<BLL.Purchase>();
            
            BLL.Purchase rp = new BLL.Purchase();
            try
            {
                foreach (var l in DB.Purchases.
                      Where(x => x.PurchaseDate >= dtFrom && x.PurchaseDate <= dtTo
                      && (x.LedgerId == LedgerId || LedgerId == null)
                      && (TType == null || x.TransactionTypeId == TType)
                      && (BillNo == "" || x.RefNo == BillNo)
                      && (x.TotalAmount >= amtFrom && x.TotalAmount <= amtTo) &&
                      x.Ledger.AccountGroup.CompanyId == Caller.CompanyId).ToList())
                {
                    rp = new BLL.Purchase();
                    rp.TotalAmount = l.TotalAmount;
                    rp.ChequeDate = l.ChequeDate;
                    rp.ChequeNo = l.ChequeNo;
                    rp.PurchaseDate = l.PurchaseDate;
                    rp.RefNo = l.RefNo;

                    rp.Id = l.Id;
                    rp.LedgerId = l.LedgerId;
                    rp.LedgerName = string.Format("{0}-{1}", l.Ledger.AccountGroup.GroupCode, l.Ledger.LedgerName);

                    rp.TransactionType = l.TransactionType.Type;
                    rp.RefCode = l.RefCode;
                    rp.RefNo = l.RefNo;

                    lstPurchase.Add(rp);
                    lstPurchase = lstPurchase.OrderBy(x => x.PurchaseDate).ToList();
                }

            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return lstPurchase;
        }


        #endregion
    }
}