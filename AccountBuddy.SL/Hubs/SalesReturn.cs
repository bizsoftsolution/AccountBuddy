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
            return SalesReturn_NewRefNoByCompanyId(Caller.CompanyId);
        }
        public string SalesReturn_NewRefNoByCompanyId(int CompanyId)
        {
            DateTime dt = DateTime.Now;
            string Prefix = string.Format("{0}{1:yy}{2:X}", BLL.FormPrefix.SalesReturn, dt, dt.Month);
            long No = 0;

            var d = DB.SalesReturns.Where(x => x.Ledger.AccountGroup.CompanyId == CompanyId && x.RefNo.StartsWith(Prefix))
                                     .OrderByDescending(x => x.RefNo)
                                     .FirstOrDefault();

            if (d != null) No = Convert.ToInt64(d.RefNo.Substring(Prefix.Length ), 16);

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
                      }
                Clients.Clients(OtherLoginClientsOnGroup).SalesReturn_RefNoRefresh(SalesReturn_NewRefNo());
                //Journal_SaveBySalesReturn(d);
                //PurchaseReturn_SaveBySalesReturn(d);
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
                   // Journal_DeleteBySalesReturn(P);
//PurchaseReturn_DeleteBySalesReturn(d);
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
      
        #region Purchase Return 
        void SaleReturn_SaveByPurchaseReturn(DAL.PurchaseReturn PR)
        {
             string RefCode = string.Format("{0}{1}", BLL.FormPrefix.PurchaseReturn, PR.Id);

            DAL.SalesReturn s = DB.SalesReturns.Where(x => x.RefCode == RefCode).FirstOrDefault();
            if (PR.Ledger.LedgerName.StartsWith("CM-") || PR.Ledger.LedgerName.StartsWith("WH-") || PR.Ledger.LedgerName.StartsWith("DL-"))
            {
                var LName = LedgerNameByCompanyId(Caller.CompanyId);
                var CId = CompanyIdByLedgerName(PR.Ledger.LedgerName);
                var LId = LedgerIdByCompany(LName, CId);

                if (LId != 0)
                {
                    if (s == null)
                    {
                        s = new DAL.SalesReturn();
                        s.RefNo = SalesReturn_NewRefNoByCompanyId(CId);
                        s.RefCode = RefCode;
                        DB.SalesReturns.Add(s);
                    }
                    else
                    {
                       DB.SalesReturnDetails.RemoveRange(s.SalesReturnDetails);
                    }

                    s.SRDate = PR.PRDate;
                    s.DiscountAmount = PR.DiscountAmount;
                    s.ExtraAmount = PR.ExtraAmount;
                    s.GSTAmount = PR.GSTAmount;
                    s.ItemAmount = PR.ItemAmount;
                    s.TotalAmount = PR.TotalAmount;
                    s.LedgerId = LId;
                    s.TransactionTypeId = PR.TransactionTypeId;
                    foreach (var b_pod in PR.PurchaseReturnDetails)
                    {
                        DAL.SalesReturnDetail d_pod = new DAL.SalesReturnDetail();
                        b_pod.toCopy<DAL.SalesReturnDetail>(d_pod);
                        s.SalesReturnDetails.Add(d_pod);
                    }
                    DB.SaveChanges();
                    //Journal_SaveBySalesReturn(s);
                }
            }
        }
        public bool SalesReturn_DeleteByPurchaseReturn(DAL.PurchaseReturn P)
        {
            try
            {
                string RefCode = string.Format("{0}{1}", BLL.FormPrefix.PurchaseReturn, P.Id);
                DAL.SalesReturn d = DB.SalesReturns.Where(x => x.RefCode == RefCode).FirstOrDefault();
                if (d != null)
                {
                    SalesReturn_Delete(d.Id);
                }
            }
            catch (Exception ex) { }
            return false;
        }
        #endregion

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
        public List<BLL.SalesReturn> SalesReturn_List(int? SID, DateTime dtFrom, DateTime dtTo, String InvoiceNo)
        {
            BLL.SalesReturn P = new BLL.SalesReturn();
            List<BLL.SalesReturn> lstSalesReturn = new List<BLL.SalesReturn>();
            try
            {
                var d = DB.SalesReturns.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId &&
                (SID == null || x.LedgerId == SID) && x.SRDate >= dtFrom &&
                x.SRDate <= dtTo &&
                (InvoiceNo == "" || x.RefNo == InvoiceNo)).ToList();
                foreach (var l in d)
                {
                    P = new BLL.SalesReturn();
                    P.Id = l.Id;
                    P.LedgerName = l.Ledger.LedgerName;
                    P.SRDate = l.SRDate;
                    P.TotalAmount = l.TotalAmount;
                    P.RefNo = l.RefNo;
                    lstSalesReturn.Add(P);
                }
            }
            catch (Exception ex) { }
            return lstSalesReturn;
        }
        #endregion
    }
}