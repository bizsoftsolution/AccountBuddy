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

            if (d != null) No = Convert.ToInt64(d.RefNo.Substring(Prefix.Length), 16);

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

                    P.ToMap( d);

                    foreach (var b_pod in P.SRDetails)
                    {
                        DAL.SalesReturnDetail d_pod = new DAL.SalesReturnDetail();
                        b_pod.ToMap( d_pod);
                        d.SalesReturnDetails.Add(d_pod);
                    }
                    DB.SaveChanges();

                    P.Id = d.Id;
                    LogDetailStore(P, LogDetailType.INSERT);
                }
                else
                {
                    //    foreach (var d_SRd in d.SalesReturnDetails.ToList())
                    //    {
                    //        BLL.SalesReturnDetail b_SRd = P.SRDetails.Where(x => x.Id == d_SRd.Id).FirstOrDefault();
                    //        if (b_SRd == null) d.SalesReturnDetails.Remove(d_SRd);
                    //    }

                    decimal rd = P.SRDetails.Select(X => X.SRId).FirstOrDefault();
                    DB.SalesReturnDetails.RemoveRange(d.SalesReturnDetails.Where(x => x.SRId == rd).ToList());


                    P.ToMap(d);
                    foreach (var b_SRd in P.SRDetails)
                    {
                        //DAL.SalesReturnDetail d_SRd = d.SalesReturnDetails.Where(x => x.Id == b_SRd.Id).FirstOrDefault();
                        // if (d_SRd == null)
                        // {
                        DAL.SalesReturnDetail d_SRd = new DAL.SalesReturnDetail();
                        d.SalesReturnDetails.Add(d_SRd);
                        //}
                        b_SRd.ToMap(d_SRd);
                    }
                    LogDetailStore(P, LogDetailType.UPDATE);
                }
                if (OtherClientsOnGroup.Count > 0) Clients.Clients(OtherClientsOnGroup).SalesReturn_RefNoRefresh(SalesReturn_NewRefNo());
                var s = P.TaxDetails.Where(x => x.TaxAmount > 0).ToList();
                Journal_SaveBySalesReturn(d,s);
                PurchaseReturn_SaveBySalesReturn(d,s);
                return true;
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
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

                    d.ToMap( P);
                    P.LedgerName = (d.Ledger ?? DB.Ledgers.Find(d.LedgerId) ?? new DAL.Ledger()).LedgerName;
                    P.TransactionType = (d.TransactionType ?? DB.TransactionTypes.Find(d.TransactionTypeId) ?? new DAL.TransactionType()).Type;
                    int i = 0;
                    foreach (var d_pod in d.SalesReturnDetails)
                    {
                        BLL.SalesReturnDetail b_pod = new BLL.SalesReturnDetail();
                        d_pod.ToMap( b_pod);
                        P.SRDetails.Add(b_pod);
                        b_pod.SNo = ++i;
                        b_pod.ProductName = (d_pod.Product ?? DB.Products.Find(d_pod.ProductId) ?? new DAL.Product()).ProductName;
                        b_pod.UOMName = (d_pod.UOM ?? DB.UOMs.Find(d_pod.UOMId) ?? new DAL.UOM()).Symbol;
                    }
                    DAL.Journal j = DB.Journals.Where(x => x.EntryNo == SearchText && x.JournalDetails.FirstOrDefault().Ledger.AccountGroup.CompanyId == Caller.CompanyId).FirstOrDefault();
                    foreach (var t in j.JournalDetails.Where(x => x.Ledger.AccountGroup.GroupName == BLL.DataKeyValue.DutiesTaxes_Key).ToList())
                    {
                        P.TaxDetails.Add(new BLL.TaxMaster()
                        {
                            Id = TaxIdByCompany_LedgerId(Caller.CompanyId, t.LedgerId),
                            Status = true,
                            Ledger = LedgerDAL_BLL(t.Ledger),
                            TaxPercentage = TaxPercentByCompany_LedgerId(Caller.CompanyId, t.LedgerId),
                            TaxAmount = t.CrAmt,
                            TaxName = string.Format("{0}({1})", t.Ledger.LedgerName, TaxPercentByCompany_LedgerId(Caller.CompanyId, t.LedgerId).ToString()),
                            LedgerId = t.LedgerId
                        });

                    }
                    var tl = DB.TaxMasters.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId).ToList();
                    var t2 = tl.Where(p => !P.TaxDetails.Any(p2 => p2.Ledger.Id == p.Ledger.Id)).ToList();

                    foreach (var t1 in t2)
                    {
                        P.TaxDetails.Add(new BLL.TaxMaster()
                        {
                            Id = TaxIdByCompany_LedgerId(Caller.CompanyId, t1.LedgerId),
                            LedgerId = t1.LedgerId,
                            Status = false,
                            Ledger = LedgerDAL_BLL(t1.Ledger),
                            TaxPercentage = t1.TaxPercentage,
                            TaxAmount = 0,
                            TaxName = string.Format("{0}({1})", t1.Ledger.LedgerName, t1.TaxPercentage.ToString()),

                        });
                    }

                }
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
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
                    PurchaseReturn_DeleteBySalesReturn(d);
                    if (OtherClientsOnGroup.Count > 0) Clients.Clients(OtherClientsOnGroup).SalesReturn_RefNoRefresh(SalesReturn_NewRefNo());
                }
                return true;
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return false;
        }

        public BLL.SalesReturn SalesReturn_DALtoBLL(DAL.SalesReturn d)
        {
            BLL.SalesReturn SR =d.ToMap(new BLL.SalesReturn());
            foreach (var d_SRd in d.SalesReturnDetails)
            {
                SR.SRDetails.Add(d_SRd.ToMap(new BLL.SalesReturnDetail()));
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
        void SaleReturn_SaveByPurchaseReturn(DAL.PurchaseReturn PR, List<BLL.TaxMaster> TaxDetail)
        {
            try
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
                            b_pod.ToMap(d_pod);
                            s.SalesReturnDetails.Add(d_pod);
                        }
                        DB.SaveChanges();
                        Journal_SaveBySalesReturn(s, TaxDetail);
                    }
                }
            }

            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex);
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
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
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

                    d.ToMap(P);
                    P.LedgerName = (d.Ledger ?? DB.Ledgers.Find(d.LedgerId) ?? new DAL.Ledger()).LedgerName;
                    P.TransactionType = (d.TransactionType ?? DB.TransactionTypes.Find(d.TransactionTypeId) ?? new DAL.TransactionType()).Type;
                    foreach (var d_pod in d.SalesReturnDetails)
                    {
                        BLL.SalesReturnDetail b_pod = new BLL.SalesReturnDetail();
                        d_pod.ToMap(b_pod);
                        P.SRDetails.Add(b_pod);
                        b_pod.ProductName = (d_pod.Product ?? DB.Products.Find(d_pod.ProductId) ?? new DAL.Product()).ProductName;
                        b_pod.UOMName = (d_pod.UOM ?? DB.UOMs.Find(d_pod.UOMId) ?? new DAL.UOM()).Symbol;
                    }

                }
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return P;
        }


        public List<BLL.SalesReturn> SalesReturn_List(int? LedgerId, int? TType, DateTime dtFrom, DateTime dtTo, string BillNo, decimal amtFrom, decimal amtTo)
        {
            List<BLL.SalesReturn> lstSalesReturn = new List<BLL.SalesReturn>();

            BLL.SalesReturn rp = new BLL.SalesReturn();
            try
            {
                foreach (var l in DB.SalesReturns.
                      Where(x => x.SRDate >= dtFrom && x.SRDate <= dtTo
                      && (x.LedgerId == LedgerId || LedgerId == null)
                      && (TType == null || x.TransactionTypeId == TType)
                      && (BillNo == "" || x.RefNo == BillNo)
                      && (x.TotalAmount >= amtFrom && x.TotalAmount <= amtTo) &&
                      x.Ledger.AccountGroup.CompanyId == Caller.CompanyId).ToList())
                {
                    rp = new BLL.SalesReturn();
                    rp.TotalAmount = l.TotalAmount;
                    rp.ChequeDate = l.ChequeDate;
                    rp.ChequeNo = l.ChequeNo;
                    rp.SRDate = l.SRDate;
                    rp.RefNo = l.RefNo;

                    rp.Id = l.Id;
                    rp.LedgerId = l.LedgerId;
                    rp.LedgerName = string.Format("{0}-{1}", l.Ledger.AccountGroup.GroupCode, l.Ledger.LedgerName);

                    rp.TransactionType = l.TransactionType.Type;
                    rp.RefCode = l.RefCode;
                    rp.RefNo = l.RefNo;

                    lstSalesReturn.Add(rp);
                    lstSalesReturn = lstSalesReturn.OrderBy(x => x.SRDate).ToList();
                }

            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return lstSalesReturn;
        }

        #endregion


    }
}