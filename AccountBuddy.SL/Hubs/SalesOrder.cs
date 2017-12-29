using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AccountBuddy.Common;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        #region Sales Order     
        public string SalesOrder_NewRefNo()
        {
            return SalesOrder_NewRefNoByCompanyId(Caller.CompanyId);
        }
        public string SalesOrder_NewRefNoByCompanyId(int CompanyId)
        {
            DateTime dt = DateTime.Now;
            string Prefix = string.Format("{0}{1:yy}{2:X}", BLL.FormPrefix.SalesOrder, dt, dt.Month);
            long No = 0;

            var d = DB.SalesOrders.Where(x => x.Ledger.AccountGroup.CompanyId == CompanyId && x.RefNo.StartsWith(Prefix))
                                     .OrderByDescending(x => x.RefNo)
                                     .FirstOrDefault();

            if (d != null) No = Convert.ToInt64(d.RefNo.Substring(Prefix.Length), 16);

            return string.Format("{0}{1:X5}", Prefix, No + 1);
        }
        public bool SalesOrder_Save(BLL.SalesOrder SO)
        {
            try
            {

                DAL.SalesOrder d = DB.SalesOrders.Where(x => x.Id == SO.Id).FirstOrDefault();

                if (d == null)
                {

                    d = new DAL.SalesOrder();
                    DB.SalesOrders.Add(d);

                    SO.ToMap(d);

                    foreach (var b_pod in SO.SODetails)
                    {
                        DAL.SalesOrderDetail d_pod = new DAL.SalesOrderDetail();
                        b_pod.ToMap(d_pod);
                        d.SalesOrderDetails.Add(d_pod);
                    }
                    DB.SaveChanges();
                    SO.Id = d.Id;
                    LogDetailStore(SO, LogDetailType.INSERT);
                }
                else
                {
                    //foreach (var d_SOd in d.SalesOrderDetails.ToList())
                    //{
                    //    BLL.SalesOrderDetail b_SOd = SO.SODetails.Where(x => x.Id == d_SOd.Id).FirstOrDefault();
                    //    if (b_SOd == null) d.SalesOrderDetails.Remove(d_SOd);
                    //}
                    decimal rd = SO.SODetails.Select(X => X.SOId).FirstOrDefault().Value;
                    DB.SalesOrderDetails.RemoveRange(d.SalesOrderDetails.Where(x => x.SOId == rd).ToList());

                    SO.ToMap(d);
                    foreach (var b_SOd in SO.SODetails)
                    {
                        //  DAL.SalesOrderDetail d_SOd = d.SalesOrderDetails.Where(x => x.Id == b_SOd.Id).FirstOrDefault();
                        //  if (d_SOd == null)
                        // {
                        DAL.SalesOrderDetail d_SOd = new DAL.SalesOrderDetail();
                        d.SalesOrderDetails.Add(d_SOd);
                        //  }
                        b_SOd.ToMap(d_SOd);
                    }
                    LogDetailStore(SO, LogDetailType.UPDATE);

                }
                if (OtherClientsOnGroup.Count > 0) Clients.Clients(OtherClientsOnGroup).SalesOrder_RefNoRefresh(SalesOrder_NewRefNo());
                if (OtherClientsOnGroup.Count > 0) Clients.Clients(OtherClientsOnGroup).SalesOrder_SOPendingSave(SO);
                PurchaseOrder_SaveBySalesOrder(d);
                return true;

            }

            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return false;
        }

        public bool SalesOrder_MakeSales(BLL.SalesOrder SO)
        {
            try
            {
                BLL.Sale S = new BLL.Sale();

                S.SalesDate = SO.SODate.Value;
                S.RefNo = Sales_NewRefNo();
                S.LedgerId = SO.LedgerId.Value;
                S.TransactionType = "Cash";
                S.TransactionTypeId = 1;
                S.ItemAmount = SO.ItemAmount.Value;
                S.DiscountAmount = SO.DiscountAmount.Value;
                S.GSTAmount = SO.GSTAmount.Value;
                S.ExtraAmount = SO.ExtraAmount.Value;
                S.TotalAmount = SO.TotalAmount.Value;
                S.Narration = SO.Narration;


                foreach (var SOd in SO.SODetails)
                {
                    BLL.SalesDetail PD = new BLL.SalesDetail()
                    {
                        SODId = SOd.Id,
                        ProductId = SOd.ProductId.Value,
                        UOMId = SOd.UOMId.Value,
                        UOMName = SOd.UOMName,
                        Quantity = SOd.Quantity.Value,
                        UnitPrice = SOd.UnitPrice.Value,
                        DiscountAmount = SOd.DiscountAmount,
                        GSTAmount = SOd.GSTAmount.Value,
                        ProductName = SOd.ProductName,
                        Amount = SOd.Amount.Value
                    };


                    S.SDetails.Add(PD);
                }
                return Sales_Save(S);
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return true;
        }




        public BLL.SalesOrder SalesOrder_Find(string SearchText)
        {
            BLL.SalesOrder SO = new BLL.SalesOrder();
            try
            {

                DAL.SalesOrder d = DB.SalesOrders.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.RefNo == SearchText).FirstOrDefault();
                DB.Entry(d).Reload();
                if (d != null)
                {

                    d.ToMap(SO);
                    SO.LedgerName = (d.Ledger ?? DB.Ledgers.Find(d.LedgerId) ?? new DAL.Ledger()).LedgerName;
                    int i = 0;

                    foreach (var d_pod in d.SalesOrderDetails)
                    {
                        BLL.SalesOrderDetail b_pod = new BLL.SalesOrderDetail();
                        d_pod.ToMap(b_pod);
                        b_pod.SNo = ++i;
                        SO.SODetails.Add(b_pod);
                        b_pod.ProductName = (d_pod.Product ?? DB.Products.Find(d_pod.ProductId) ?? new DAL.Product()).ProductName;
                        b_pod.UOMName = (d_pod.UOM ?? DB.UOMs.Find(d_pod.UOMId) ?? new DAL.UOM()).Symbol;
                        SO.Status = d.SalesOrderDetails.FirstOrDefault().SalesDetails.Count() > 0 ? "Sold" : "Pending";
                    }

                }
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return SO;
        }

        public bool SalesOrder_Delete(long pk)
        {
            try
            {
                DAL.SalesOrder d = DB.SalesOrders.Where(x => x.Id == pk).FirstOrDefault();

                if (d != null)
                {
                    var s = SalesOrder_DALtoBLL(d);
                    DB.SalesOrderDetails.RemoveRange(d.SalesOrderDetails);
                    DB.SalesOrders.Remove(d);
                    DB.SaveChanges();

                    LogDetailStore(s, LogDetailType.DELETE);
                    PurchaseOrder_DeleteBySalesOrder(d);
                    if (OtherClientsOnGroup.Count > 0) Clients.Clients(OtherClientsOnGroup).SalesOrder_RefNoRefresh(SalesOrder_NewRefNo());
                }
                return true;
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return false;
        }

        public List<BLL.SalesOrder> SalesOrder_SOPendingList()
        {
            return DB.SalesOrders.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId)
                                     .ToList()
                                     .Select(x => SalesOrder_DALtoBLL(x))
                                     .ToList();
        }
        public BLL.SalesOrder SalesOrder_DALtoBLL(DAL.SalesOrder d)
        {
            BLL.SalesOrder SO = d.ToMap(new BLL.SalesOrder());
            foreach (var d_SOd in d.SalesOrderDetails)
            {
                SO.SODetails.Add(d_SOd.ToMap(new BLL.SalesOrderDetail()));
            }
            SO.Status = d.SalesOrderDetails.FirstOrDefault().SalesDetails.Count() > 0 ? "Sold" : "Pending";
            return SO;
        }
        public bool Find_SORef(string RefNo, BLL.SalesOrder PO)
        {
            DAL.SalesOrder d1 = DB.SalesOrders.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.RefNo == RefNo & x.Id != PO.Id).FirstOrDefault();
            DAL.SalesOrder d2 = null;


            var LName = DB.Ledgers.Where(x => x.Id == PO.LedgerId).FirstOrDefault().LedgerName;

            if (LName.StartsWith("CM-"))
            {
                var LNameTo = LedgerNameByCompanyId(Caller.CompanyId);
                var LId = LedgerIdByCompany(LNameTo, Caller.UnderCompanyId.Value);

                d2 = DB.SalesOrders.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.UnderCompanyId && x.RefNo == RefNo).FirstOrDefault();
                if (d2 != null)
                {
                    if (d2.LedgerId == LId)
                    {
                        d2 = null;
                    }
                }
            }



            if (d1 == null || d2 == null)
            {
                return false;
            }
            else
            {
                return true;
            }

        }
        #endregion
        #region PurchaseOrder
        void SalesOrder_SaveByPurchaseOrder(DAL.PurchaseOrder P)
        {
            try
            {
                string RefCode = string.Format("{0}{1}", BLL.FormPrefix.PurchaseOrder, P.Id);
                if (P.Ledger == null)
                {
                    P.Ledger = DB.Ledgers.Where(x => x.Id == P.LedgerId).FirstOrDefault();
                }
                DAL.SalesOrder s = DB.SalesOrders.Where(x => x.RefCode == RefCode).FirstOrDefault();
                if (P.Ledger.LedgerName.StartsWith("CM-") || P.Ledger.LedgerName.StartsWith("WH-") || P.Ledger.LedgerName.StartsWith("DL-"))
                {
                    var LName = LedgerNameByCompanyId(Caller.CompanyId);
                    var CId = CompanyIdByLedgerName(P.Ledger.LedgerName);
                    var LId = LedgerIdByCompany(LName, CId);

                    if (LId != 0)
                    {
                        if (s == null)
                        {
                            s = new DAL.SalesOrder();
                            s.RefNo = SalesOrder_NewRefNoByCompanyId(CId);
                            s.RefCode = RefCode;
                            DB.SalesOrders.Add(s);
                        }
                        else
                        {
                            DB.SalesOrderDetails.RemoveRange(s.SalesOrderDetails);
                        }

                        s.SODate = P.PODate;
                        s.DiscountAmount = P.DiscountAmount;
                        s.ExtraAmount = P.Extras;
                        s.GSTAmount = P.GSTAmount;
                        s.ItemAmount = P.ItemAmount;
                        s.TotalAmount = P.TotalAmount;
                        s.LedgerId = LId;
                        foreach (var b_pod in P.PurchaseOrderDetails)
                        {
                            DAL.SalesOrderDetail d_pod = new DAL.SalesOrderDetail();
                            b_pod.ToMap(d_pod);
                            s.SalesOrderDetails.Add(d_pod);
                        }
                        DB.SaveChanges();

                    }
                }
            }
            catch(Exception ex) { Common.AppLib.WriteLog(ex); }
            
        }
        public bool SalesOrder_DeleteByPurchaseOrder(DAL.PurchaseOrder P)
        {
            try
            {
                string RefCode = string.Format("{0}{1}", BLL.FormPrefix.PurchaseOrder, P.Id);
                DAL.SalesOrder d = DB.SalesOrders.Where(x => x.RefCode == RefCode).FirstOrDefault();
                if (d != null)
                {
                    SalesOrder_Delete(d.Id);
                }
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return false;
        }
        #endregion


        public List<BLL.SalesOrder> SalesOrder_List(int? LedgerId, DateTime dtFrom, DateTime dtTo, string BillNo, decimal amtFrom, decimal amtTo)
        {
           List<BLL.SalesOrder> lstSalesOrder = new List<BLL.SalesOrder>();
            
            BLL.SalesOrder rp = new BLL.SalesOrder();
            try
            {
                foreach (var l in DB.SalesOrders.
                      Where(x => x.SODate >= dtFrom && x.SODate <= dtTo
                      && (x.LedgerId == LedgerId || LedgerId == null)
                      && (BillNo == "" || x.RefNo == BillNo)
                      && (x.TotalAmount >= amtFrom && x.TotalAmount <= amtTo) &&
                      x.Ledger.AccountGroup.CompanyId == Caller.CompanyId).ToList())
                {
                    rp = new BLL.SalesOrder();
                    rp.TotalAmount = l.TotalAmount;
                    rp.SODate = l.SODate;
                    rp.RefNo = l.RefNo;
                    rp.Id = l.Id;
                    rp.LedgerId = l.LedgerId;
                    rp.LedgerName = string.Format("{0}-{1}", l.Ledger.AccountGroup.GroupCode, l.Ledger.LedgerName);
                    rp.RefCode = l.RefCode;
                    rp.RefNo = l.RefNo;
                    rp.Status = l.SalesOrderDetails.FirstOrDefault().SalesDetails.Count() > 0 ? "Sold" : "Pending";
                    lstSalesOrder.Add(rp);
                    lstSalesOrder = lstSalesOrder.OrderBy(x => x.SODate).ToList();
                }

            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return lstSalesOrder;
        }


    }
}