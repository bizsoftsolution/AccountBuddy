using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AccountBuddy.Common;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        #region PurchaseReturn
        public string PurchaseReturn_NewRefNo()
        {
            return PurchaseReturn_NewRefNoByCompanyId(Caller.CompanyId);
        }
        public  string PurchaseReturn_NewRefNoByCompanyId(int CompanyId)
        {
            DateTime dt = DateTime.Now;
            string Prefix = string.Format("{0}{1:yy}{2:X}", BLL.FormPrefix.PurchaseReturn, dt, dt.Month);
            long No = 0;

            var d = DB.PurchaseReturns.Where(x => x.Ledger.AccountGroup.CompanyId == CompanyId && x.RefNo.StartsWith(Prefix))
                                     .OrderByDescending(x => x.RefNo)
                                     .FirstOrDefault();

            if (d != null) No = Convert.ToInt64(d.RefNo.Substring(Prefix.Length), 16);

            return string.Format("{0}{1:X5}", Prefix, No + 1);
        }
        public bool PurchaseReturn_Save(BLL.PurchaseReturn P)
        {
            try
            {

                DAL.PurchaseReturn d = DB.PurchaseReturns.Where(x => x.Id == P.Id).FirstOrDefault();

                if (d == null)
                {

                    d = new DAL.PurchaseReturn();
                    DB.PurchaseReturns.Add(d);

                    P.toCopy<DAL.PurchaseReturn>(d);

                    foreach (var b_pod in P.PRDetails)
                    {
                        DAL.PurchaseReturnDetail d_pod = new DAL.PurchaseReturnDetail();
                        b_pod.toCopy<DAL.PurchaseReturnDetail>(d_pod);
                        d.PurchaseReturnDetails.Add(d_pod);
                    }
                    DB.SaveChanges();

                    P.Id = d.Id;
                    LogDetailStore(P, LogDetailType.INSERT);
                }
                else
                {
                    //foreach (var d_Pd in d.PurchaseReturnDetails)
                    //{
                    //    BLL.PurchaseReturnDetail b_Pd = P.PRDetails.Where(x => x.Id == d_Pd.Id).FirstOrDefault();
                    //    if (b_Pd == null) d.PurchaseReturnDetails.Remove(d_Pd);
                    //}
                    decimal rd = P.PRDetails.Select(X => X.PRId).FirstOrDefault();
                    DB.PurchaseReturnDetails.RemoveRange(d.PurchaseReturnDetails.Where(x => x.PRId == rd).ToList());

                    P.toCopy<DAL.PurchaseReturn>(d);
                    foreach (var b_Pd in P.PRDetails)
                    {
                        // DAL.PurchaseReturnDetail d_Pd = d.PurchaseReturnDetails.Where(x => x.Id == b_Pd.Id).FirstOrDefault();
                        //if (d_Pd == null)
                        //{
                        DAL.PurchaseReturnDetail d_Pd = new DAL.PurchaseReturnDetail();
                            d.PurchaseReturnDetails.Add(d_Pd);
                       // }
                        b_Pd.toCopy<DAL.PurchaseReturnDetail>(d_Pd);
                    }
                    DB.SaveChanges();
                    LogDetailStore(P, LogDetailType.UPDATE);
                }
                if (OtherClientsOnGroup.Count > 0) Clients.Clients(OtherClientsOnGroup).PurchaseReturn_RefNoRefresh(PurchaseReturn_NewRefNo());
                Journal_SaveByPurchaseReturn(d);
                SaleReturn_SaveByPurchaseReturn(d);
                return true;
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return false;
        }

        #region Salesreturn 
        void PurchaseReturn_SaveBySalesReturn(DAL.SalesReturn SR)
        {
            string RefCode = string.Format("{0}{1}", BLL.FormPrefix.SalesReturn, SR.Id);

            DAL.PurchaseReturn p = DB.PurchaseReturns.Where(x => x.RefCode == RefCode).FirstOrDefault();
            if (SR.Ledger == null)
            {
                SR.Ledger = DB.Ledgers.Where(x => x.Id == SR.LedgerId).FirstOrDefault();
            }
            if (SR.Ledger.LedgerName.StartsWith("CM-") || SR.Ledger.LedgerName.StartsWith("WH-") || SR.Ledger.LedgerName.StartsWith("DL-"))
            {
                var LName = LedgerNameByCompanyId(Caller.CompanyId);
                var CId = CompanyIdByLedgerName(SR.Ledger.LedgerName);
                var LId = LedgerIdByCompany(LName, CId);

                if (LId != 0)
                {
                    if (p == null)
                    {
                        p = new DAL.PurchaseReturn();
                        p.RefNo = PurchaseReturn_NewRefNoByCompanyId(CId);
                        p.RefCode = RefCode;
                        DB.PurchaseReturns.Add(p);
                    }
                    else
                    {
                        DB.PurchaseReturnDetails.RemoveRange(p.PurchaseReturnDetails);
                    }

                    p.PRDate = SR.SRDate;
                    p.DiscountAmount = SR.DiscountAmount;
                    p.ExtraAmount = SR.ExtraAmount;
                    p.GSTAmount = SR.GSTAmount;
                    p.ItemAmount = SR.ItemAmount;
                    p.TotalAmount = SR.TotalAmount;
                    p.LedgerId = LId;
                    p.TransactionTypeId = SR.TransactionTypeId;
                    foreach (var b_pod in SR.SalesReturnDetails)
                    {
                        DAL.PurchaseReturnDetail d_pod = new DAL.PurchaseReturnDetail();
                        b_pod.toCopy<DAL.PurchaseReturnDetail>(d_pod);
                        p.PurchaseReturnDetails.Add(d_pod);
                    }
                    DB.SaveChanges();
                    Journal_SaveByPurchaseReturn(p);
                }
            }
        }
        public bool PurchaseReturn_DeleteBySalesReturn(DAL.SalesReturn sr)
        {
            try
            {
                string RefCode = string.Format("{0}{1}", BLL.FormPrefix.SalesReturn, sr.Id);
                DAL.SalesReturn d = DB.SalesReturns.Where(x => x.RefCode == RefCode).FirstOrDefault();
                if (d != null)
                {
                    SalesReturn_Delete(d.Id);
                }


            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return false;

        }
        #endregion

        public BLL.PurchaseReturn PurchaseReturn_Find(string SearchText)
        {
            BLL.PurchaseReturn P = new BLL.PurchaseReturn();
            try
            {

                DAL.PurchaseReturn d = DB.PurchaseReturns.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.RefNo == SearchText).FirstOrDefault();
                DB.Entry(d).Reload();
                if (d != null)
                {

                    d.toCopy<BLL.PurchaseReturn>(P);
                    P.LedgerName = (d.Ledger ?? DB.Ledgers.Find(d.LedgerId) ?? new DAL.Ledger()).LedgerName;
                    P.TransactionType = (d.TransactionType ?? DB.TransactionTypes.Find(d.TransactionTypeId) ?? new DAL.TransactionType()).Type;
                    int i = 0;
                        foreach (var d_pod in d.PurchaseReturnDetails)
                    {
                        BLL.PurchaseReturnDetail b_pod = new BLL.PurchaseReturnDetail();
                        d_pod.toCopy<BLL.PurchaseReturnDetail>(b_pod);
                        P.PRDetails.Add(b_pod);
                        b_pod.SNo = ++i; 
                        b_pod.ProductName = (d_pod.Product ?? DB.Products.Find(d_pod.ProductId) ?? new DAL.Product()).ProductName;
                        b_pod.UOMName = (d_pod.UOM ?? DB.UOMs.Find(d_pod.UOMId) ?? new DAL.UOM()).Symbol;
                    }

                }
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return P;
        }
        public bool PurchaseReturn_Delete(long pk)
        {
            try
            {
                DAL.PurchaseReturn d = DB.PurchaseReturns.Where(x => x.Id == pk).FirstOrDefault();

                if (d != null)
                {
                    var P = PurchaseReturn_DALtoBLL(d);
                    DB.PurchaseReturnDetails.RemoveRange(d.PurchaseReturnDetails);
                    DB.PurchaseReturns.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(P, LogDetailType.DELETE);
                    Journal_DeleteByPurchaseReturn(P);
                    SalesReturn_DeleteByPurchaseReturn(d);
                    if (OtherClientsOnGroup.Count > 0) Clients.Clients(OtherClientsOnGroup).PurchaseReturn_RefNoRefresh(PurchaseReturn_NewRefNo());
                }
                return true;
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return false;
        }
        public BLL.PurchaseReturn PurchaseReturn_DALtoBLL(DAL.PurchaseReturn d)
        {
            BLL.PurchaseReturn PR = d.toCopy<BLL.PurchaseReturn>(new BLL.PurchaseReturn());
            foreach (var d_PRd in d.PurchaseReturnDetails)
            {
                PR.PRDetails.Add(d_PRd.toCopy<BLL.PurchaseReturnDetail>(new BLL.PurchaseReturnDetail()));
            }
            return PR;
        }
        public bool Find_PRRef(string RefNo, BLL.PurchaseReturn PO)

        {
            DAL.PurchaseReturn d = DB.PurchaseReturns.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.RefNo == RefNo & x.Id != PO.Id).FirstOrDefault();
            if (d == null)
            {
                return false;
            }
            else
            {
                return true;
            }

        }
        public BLL.PurchaseReturn PurchaseReturn_FindById(int ID)
        {
            BLL.PurchaseReturn P = new BLL.PurchaseReturn();
            try
            {

                DAL.PurchaseReturn d = DB.PurchaseReturns.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.Id == ID).FirstOrDefault();
                DB.Entry(d).Reload();
                if (d != null)
                {

                    d.toCopy<BLL.PurchaseReturn>(P);
                    P.LedgerName = (d.Ledger ?? DB.Ledgers.Find(d.LedgerId) ?? new DAL.Ledger()).LedgerName;
                    P.TransactionType = (d.TransactionType ?? DB.TransactionTypes.Find(d.TransactionTypeId) ?? new DAL.TransactionType()).Type;
                    foreach (var d_pod in d.PurchaseReturnDetails)
                    {
                        BLL.PurchaseReturnDetail b_pod = new BLL.PurchaseReturnDetail();
                        d_pod.toCopy<BLL.PurchaseReturnDetail>(b_pod);
                        P.PRDetails.Add(b_pod);
                        b_pod.ProductName = (d_pod.Product ?? DB.Products.Find(d_pod.ProductId) ?? new DAL.Product()).ProductName;
                        b_pod.UOMName = (d_pod.UOM ?? DB.UOMs.Find(d_pod.UOMId) ?? new DAL.UOM()).Symbol;
                    }

                }
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return P;
        }


        public List<BLL.PurchaseReturn> PurchaseReturn_List(int? LedgerId, int? TType, DateTime dtFrom, DateTime dtTo, string BillNo, decimal amtFrom, decimal amtTo)
        {
            List<BLL.PurchaseReturn> lstPurchaseReturn = new List<BLL.PurchaseReturn>();
            
            BLL.PurchaseReturn rp = new BLL.PurchaseReturn();
            try
            {
                foreach (var l in DB.PurchaseReturns.
                      Where(x => x.PRDate >= dtFrom && x.PRDate <= dtTo
                      && (x.LedgerId == LedgerId || LedgerId == null)
                      && (TType ==null|| x.TransactionTypeId == TType)
                      && (BillNo == "" || x.RefNo == BillNo)
                      && (x.TotalAmount >= amtFrom && x.TotalAmount <= amtTo) &&
                      x.Ledger.AccountGroup.CompanyId == Caller.CompanyId).ToList())
                {
                    rp = new BLL.PurchaseReturn();
                    rp.TotalAmount = l.TotalAmount;
                    rp.ChequeDate = l.ChequeDate;
                    rp.ChequeNo = l.ChequeNo;
                    rp.PRDate = l.PRDate;
                    rp.RefNo = l.RefNo;

                    rp.Id = l.Id;
                    rp.LedgerId = l.LedgerId;
                    rp.LedgerName = string.Format("{0}-{1}", l.Ledger.AccountGroup.GroupCode, l.Ledger.LedgerName);

                    rp.TransactionType = l.TransactionType.Type;
                    rp.RefCode = l.RefCode;
                    rp.RefNo = l.RefNo;

                    lstPurchaseReturn.Add(rp);
                    lstPurchaseReturn = lstPurchaseReturn.OrderBy(x => x.PRDate).ToList();
                }

            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return lstPurchaseReturn;
        }

        #endregion
    }
}