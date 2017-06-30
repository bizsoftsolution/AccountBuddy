using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AccountBuddy.Common;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        #region SalesReturn
        public string SalesReturn_NewRefNo()
        {
            DateTime dt = DateTime.Now;
            string Prefix = string.Format("{0}{1:yy}{2:X}", BLL.FormPrefix.SalesReturn, dt, dt.Month);
            long No = 0;

            var d = DB.SalesReturns.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.RefNo.StartsWith(Prefix))
                                     .OrderByDescending(x => x.RefNo)
                                     .FirstOrDefault();

            if (d != null) No = Convert.ToInt64(d.RefNo.Substring(Prefix.Length - 1), 16);

            return string.Format("{0}{1:X5}", Prefix, No + 1);
        }
        public bool SalesReturn_Save(BLL.SalesReturn P)
        {
            try
            {
               
                DAL.SalesReturn d = DB.SalesReturns.Where(x => x.Id == P.Id).FirstOrDefault();

                if (d == null)
                {

                    d = new DAL.SalesReturn();
                    DB.SalesReturns.Add(d);

                    P.toCopy<DAL.SalesReturn>(d);

                    foreach (var b_pod in P.SRDetails)
                    {
                        DAL.SalesReturnDetail d_pod = new DAL.SalesReturnDetail();
                        b_pod.toCopy<DAL.SalesReturnDetail>(d_pod);
                        d.SalesReturnDetails.Add(d_pod);
                    }
                    DB.SaveChanges();

                    P.Id = d.Id;
                    LogDetailStore(P, LogDetailType.INSERT);
                }
                else
                {
                    foreach (var d_SRd in d.SalesReturnDetails)
                    {
                        BLL.SalesReturnDetail b_SRd = P.SRDetails.Where(x => x.Id == d_SRd.Id).FirstOrDefault();
                        if (b_SRd == null) d.SalesReturnDetails.Remove(d_SRd);
                    }

                    P.toCopy<DAL.SalesReturn>(d);
                    foreach (var b_SRd in P.SRDetails)
                    {
                        DAL.SalesReturnDetail d_SRd = d.SalesReturnDetails.Where(x => x.Id == b_SRd.Id).FirstOrDefault();
                        if (d_SRd == null)
                        {
                            d_SRd = new DAL.SalesReturnDetail();
                            d.SalesReturnDetails.Add(d_SRd);
                        }
                        b_SRd.toCopy<DAL.SalesReturnDetail>(d_SRd);
                    }
                    LogDetailStore(P, LogDetailType.UPDATE);
                    PurchaseReturn_SaveBySalesReturn(P);
                }
                Clients.Clients(OtherLoginClientsOnGroup).SalesReturn_RefNoRefresh(SalesReturn_NewRefNo());
                Journal_SaveBySalesReturn(P);
                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public BLL.SalesReturn SalesReturn_Find(string SearchText)
        {
            BLL.SalesReturn P = new BLL.SalesReturn();
            try
            {

                DAL.SalesReturn d = DB.SalesReturns.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.RefNo == SearchText).FirstOrDefault();
                DB.Entry(d).Reload();
                if (d != null)
                {

                    d.toCopy<BLL.SalesReturn>(P);
                    P.LedgerName = (d.Ledger ?? DB.Ledgers.Find(d.LedgerId) ?? new DAL.Ledger()).LedgerName;
                    P.TransactionType = (d.TransactionType ?? DB.TransactionTypes.Find(d.TransactionTypeId) ?? new DAL.TransactionType()).Type;
                    foreach (var d_pod in d.SalesReturnDetails)
                    {
                        BLL.SalesReturnDetail b_pod = new BLL.SalesReturnDetail();
                        d_pod.toCopy<BLL.SalesReturnDetail>(b_pod);
                        P.SRDetails.Add(b_pod);
                        b_pod.ProductName = (d_pod.Product ?? DB.Products.Find(d_pod.ProductId) ?? new DAL.Product()).ProductName;
                        b_pod.UOMName = (d_pod.UOM ?? DB.UOMs.Find(d_pod.UOMId) ?? new DAL.UOM()).Symbol;
                    }

                }
            }
            catch (Exception ex) { }
            return P;
        }

        public bool SalesReturn_Delete(long pk)
        {
            try
            {
                DAL.SalesReturn d = DB.SalesReturns.Where(x => x.Id == pk).FirstOrDefault();

                if (d != null)
                {
                    var P = SalesReturn_DALtoBLL(d);
                    DB.SalesReturnDetails.RemoveRange(d.SalesReturnDetails);
                    DB.SalesReturns.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(P, LogDetailType.DELETE);
                    Journal_DeleteBySalesReturn(P);
                }
                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public BLL.SalesReturn SalesReturn_DALtoBLL(DAL.SalesReturn d)
        {
            BLL.SalesReturn SR = d.toCopy<BLL.SalesReturn>(new BLL.SalesReturn());
            foreach (var d_SRd in d.SalesReturnDetails)
            {
                SR.SRDetails.Add(d_SRd.toCopy<BLL.SalesReturnDetail>(new BLL.SalesReturnDetail()));
            }
            return SR;
        }
        public bool Find_SRRef(string RefNo, BLL.SalesReturn PO)

        {
            DAL.SalesReturn d = DB.SalesReturns.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.RefNo == RefNo & x.Id != PO.Id).FirstOrDefault();
            if (d == null)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        void SaleReturn_SaveByPurchaseReturn(BLL.PurchaseReturn PR)
        {
            var refNo = string.Format("SAL-R-{0}", PR.Id);

            DAL.SalesReturn sr = DB.SalesReturns.Where(x => x.RefNo == refNo).FirstOrDefault();
            if (sr != null)
            {
                DB.SalesReturnDetails.RemoveRange(sr.SalesReturnDetails);
                DB.SalesReturns.Remove(sr);
                DB.SaveChanges();
            }
            var pd = PR.PRDetails.FirstOrDefault();
            var ld = DB.Ledgers.Where(x => x.Id == PR.LedgerId).FirstOrDefault();

            if (ld.LedgerName.StartsWith("CM-") || ld.LedgerName.StartsWith("WH-") || ld.LedgerName.StartsWith("DL-"))
            {
                var LName = LedgerNameByCompanyId(Caller.CompanyId);

                var CId = CompanyIdByLedgerName(ld.LedgerName);

                sr = new DAL.SalesReturn();
                sr.RefNo = refNo;
                sr.SRDate = PR.PRDate;
                sr.DiscountAmount = PR.DiscountAmount;
                sr.ExtraAmount = PR.ExtraAmount;
                sr.GSTAmount = PR.GSTAmount;
                sr.ItemAmount = PR.ItemAmount;
                sr.TotalAmount = PR.TotalAmount;
                sr.LedgerId = LedgerIdByCompany(LName, CId);
                sr.TransactionTypeId = PR.TransactionTypeId;
                if (CId != 0)
                {
                    foreach (var b_pod in PR.PRDetails)
                    {
                        DAL.SalesReturnDetail d_pod = new DAL.SalesReturnDetail();
                        b_pod.toCopy<DAL.SalesReturnDetail>(d_pod);
                        sr.SalesReturnDetails.Add(d_pod);
                    }
                    DB.SalesReturns.Add(sr);
                    DB.SaveChanges();
                    var SalR = SalesReturn_DALtoBLL(sr);
                    SalR.TransactionType = PR.TransactionType;
                    Journal_SaveBySalesReturn(SalR);
                }
            }
        }

        public BLL.SalesReturn SalesReturn_FindById(int ID)
        {
            BLL.SalesReturn P = new BLL.SalesReturn();
            try
            {

                DAL.SalesReturn d = DB.SalesReturns.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.Id == ID).FirstOrDefault();
                DB.Entry(d).Reload();
                if (d != null)
                {

                    d.toCopy<BLL.SalesReturn>(P);
                    P.LedgerName = (d.Ledger ?? DB.Ledgers.Find(d.LedgerId) ?? new DAL.Ledger()).LedgerName;
                    P.TransactionType = (d.TransactionType ?? DB.TransactionTypes.Find(d.TransactionTypeId) ?? new DAL.TransactionType()).Type;
                    foreach (var d_pod in d.SalesReturnDetails)
                    {
                        BLL.SalesReturnDetail b_pod = new BLL.SalesReturnDetail();
                        d_pod.toCopy<BLL.SalesReturnDetail>(b_pod);
                        P.SRDetails.Add(b_pod);
                        b_pod.ProductName = (d_pod.Product ?? DB.Products.Find(d_pod.ProductId) ?? new DAL.Product()).ProductName;
                        b_pod.UOMName = (d_pod.UOM ?? DB.UOMs.Find(d_pod.UOMId) ?? new DAL.UOM()).Symbol;
                    }

                }
            }
            catch (Exception ex) { }
            return P;
        }

        #endregion


    }
}