using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AccountBuddy.Common;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        #region Purchase Order       
        public string PurchaseOrder_NewRefNo()
        {
            return PurchaseOrder_NewRefNoByCompanyId(Caller.CompanyId);
        }
        public string PurchaseOrder_NewRefNoByCompanyId(int CompanyId)
        {
            DateTime dt = DateTime.Now;
            string Prefix = string.Format("{0}{1:yy}{2:X}", BLL.FormPrefix.PurchaseOrder, dt, dt.Month);
            long No = 0;

            var d = DB.PurchaseOrders.Where(x => x.Ledger.AccountGroup.CompanyId == CompanyId && x.RefNo.StartsWith(Prefix))
                                     .OrderByDescending(x => x.RefNo)
                                     .FirstOrDefault();

            if (d != null) No = Convert.ToInt64(d.RefNo.Substring(Prefix.Length), 16);

            return string.Format("{0}{1:X5}", Prefix, No + 1);
        }
        public string PurchaseOrder_NewRefNo(DateTime dt)
        {
            //  string Prefix = string.Format("{0}{1:yy}{2:X}", BLL.FormPrefix.Payment, dt, dt.Month);
            string Prefix = string.Format("{0}{1:yy}{2:X}", BLL.FormPrefix.PurchaseOrder, dt, dt.Month);
            long No = 0;

            var d1 = DB.PurchaseOrders.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.RefNo.StartsWith(Prefix) && x.PODate.Month == dt.Month).Select(x => x.RefNo).ToList();
            if (d1.Count() > 0)
            {
                No = d1.Select(x => Convert.ToInt64(x.Substring(Prefix.Length), 16)).Max();
            }

            return string.Format("{0}{1:x5}", Prefix, No + 1);
        }

        public bool PurchaseOrder_Save(BLL.PurchaseOrder PO)
        {
            try
            {

                DAL.PurchaseOrder d = DB.PurchaseOrders.Where(x => x.Id == PO.Id).FirstOrDefault();

                if (d == null)
                {

                    d = new DAL.PurchaseOrder();
                    DB.PurchaseOrders.Add(d);

                    PO.ToMap(d);

                    foreach (var b_pod in PO.PODetails)
                    {
                        DAL.PurchaseOrderDetail d_pod = new DAL.PurchaseOrderDetail();
                        b_pod.ToMap(d_pod);
                        d.PurchaseOrderDetails.Add(d_pod);
                    }
                    foreach (var td in PO.TaxDetails.Where(x=>x.TaxAmount>0))
                    {
                        DAL.Purchase_Order_TaxDetail sd = new DAL.Purchase_Order_TaxDetail();
                        sd.TaxAmount = td.TaxAmount;
                        sd.TaxId = td.Id;
                        sd.TaxPercentage = td.TaxPercentage;
                        d.Purchase_Order_TaxDetail.Add(sd);
                    }
                    DB.SaveChanges();
                    PO.Id = d.Id;
                    LogDetailStore(PO, LogDetailType.INSERT);
                }
                else
                {
                    decimal rd = PO.PODetails.Select(X => X.POId).FirstOrDefault();
                    DB.PurchaseOrderDetails.RemoveRange(d.PurchaseOrderDetails.Where(x => x.POId == rd).ToList());

                     DB.Purchase_Order_TaxDetail.RemoveRange(d.Purchase_Order_TaxDetail.Where(x => x.PO_ID == d.Id).ToList());


                    PO.ToMap(d);
                    foreach (var b_pod in PO.PODetails)
                    {
                        DAL.PurchaseOrderDetail d_pod = new DAL.PurchaseOrderDetail();
                        b_pod.ToMap(d_pod);

                        d.PurchaseOrderDetails.Add(d_pod);
                    }
                    foreach (var td in PO.TaxDetails.Where(x => x.TaxAmount > 0))
                    {
                        DAL.Purchase_Order_TaxDetail sd = new DAL.Purchase_Order_TaxDetail();
                        sd.TaxAmount = td.TaxAmount;
                        sd.TaxId = td.Id;
                        sd.TaxPercentage = td.TaxPercentage;
                        d.Purchase_Order_TaxDetail.Add(sd);
                    }
                    DB.SaveChanges();
                    LogDetailStore(PO, LogDetailType.UPDATE);
                }

                if (OtherClientsOnGroup.Count > 0) Clients.Clients(OtherClientsOnGroup).PurchaseOrder_RefNoRefresh(PurchaseOrder_NewRefNo());
                if (OtherClientsOnGroup.Count > 0) Clients.Clients(OtherClientsOnGroup).PurchaseOrder_Save(PO);
                SalesOrder_SaveByPurchaseOrder(d);
                return true;
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return false;
        }

        public bool PurchaseOrder_MakePurchase(BLL.PurchaseOrder PO)
        {
            try
            {
                BLL.Purchase P = new BLL.Purchase();

                P.PurchaseDate = PO.PODate.Value;
                P.RefNo = Purchase_NewRefNo();
                P.LedgerId = PO.LedgerId;
                P.TransactionType = "Cash";
                P.TransactionTypeId = 1;
                P.ItemAmount = PO.ItemAmount.Value;
                P.DiscountAmount = PO.DiscountAmount.Value;
                P.GSTAmount = PO.GSTAmount.Value;
                P.ExtraAmount = PO.Extras.Value;
                P.TotalAmount = PO.TotalAmount.Value;
                P.Narration = PO.Narration;
                P.TaxDetails = PO.TaxDetails;

                foreach (var pod in PO.PODetails)
                {
                    BLL.PurchaseDetail PD = new BLL.PurchaseDetail()
                    {
                        PODId = pod.Id,
                        ProductId = pod.ProductId,
                        UOMId = pod.UOMId,
                        UOMName = pod.UOMName,
                        Quantity = pod.Quantity,
                        UnitPrice = pod.UnitPrice,
                        DiscountAmount = pod.DiscountAmount,
                        GSTAmount = pod.GSTAmount,
                        ProductName = pod.ProductName,
                        Amount = pod.Amount
                    };


                    P.PDetails.Add(PD);
                }
                
                return Purchase_Save(P);
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return true;
        }

        #region PurchaseOrder

        void PurchaseOrder_SaveBySalesOrder(DAL.SalesOrder S)
        {
            try
            {
                string RefCode = string.Format("{0}{1}", BLL.FormPrefix.SalesOrder, S.Id);

                DAL.PurchaseOrder p = DB.PurchaseOrders.Where(x => x.RefCode == RefCode).FirstOrDefault();
                if (S.Ledger == null)
                {
                    S.Ledger = DB.Ledgers.Where(x => x.Id == S.LedgerId).FirstOrDefault();
                }
                if (S.Ledger.LedgerName.StartsWith("CM-") || S.Ledger.LedgerName.StartsWith("WH-") || S.Ledger.LedgerName.StartsWith("DL-"))
                {
                    var LName = LedgerNameByCompanyId(Caller.CompanyId);
                    var CId = CompanyIdByLedgerName(S.Ledger.LedgerName);
                    var LId = LedgerIdByCompany(LName, CId);

                    if (LId != 0)
                    {
                        if (p == null)
                        {
                            p = new DAL.PurchaseOrder();
                            p.RefNo = PurchaseOrder_NewRefNoByCompanyId(CId);
                            p.RefCode = RefCode;
                            DB.PurchaseOrders.Add(p);
                        }
                        else
                        {
                            DB.Purchase_Order_TaxDetail.RemoveRange(p.Purchase_Order_TaxDetail);

                            DB.PurchaseOrderDetails.RemoveRange(p.PurchaseOrderDetails);
                        }

                        p.PODate = S.SODate;
                        p.DiscountAmount = S.DiscountAmount;
                        p.Extras = S.ExtraAmount;
                        p.GSTAmount = S.GSTAmount;
                        p.ItemAmount = S.ItemAmount;
                        p.TotalAmount = S.TotalAmount;
                        p.LedgerId = LId;
                        foreach (var b_pod in S.SalesOrderDetails)
                        {
                            DAL.PurchaseOrderDetail d_pod = new DAL.PurchaseOrderDetail();
                            b_pod.ToMap(d_pod);
                            p.PurchaseOrderDetails.Add(d_pod);
                        }
                        DB.SaveChanges();

                    }
                }

            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }

        }
        public bool PurchaseOrder_DeleteBySalesOrder(DAL.SalesOrder s)
        {
            try
            {
                string RefCode = string.Format("{0}{1}", BLL.FormPrefix.SalesOrder, s.Id);
                DAL.PurchaseOrder d = DB.PurchaseOrders.Where(x => x.RefCode == RefCode).FirstOrDefault();
                if (d != null)
                {
                    PurchaseOrder_Delete(d.Id);
                }


            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return false;
        }

        #endregion

        public BLL.PurchaseOrder PurchaseOrder_Find(string SearchText)
        {
            BLL.PurchaseOrder PO = new BLL.PurchaseOrder();
            try
            {

                DAL.PurchaseOrder d = DB.PurchaseOrders.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.RefNo == SearchText).FirstOrDefault();
                DB.Entry(d).Reload();
                if (d != null)
                {

                    d.ToMap(PO);
                    PO.LedgerName = (d.Ledger ?? DB.Ledgers.Find(d.LedgerId) ?? new DAL.Ledger()).LedgerName;
                    int i = 0;
                    foreach (var d_pod in DB.PurchaseOrderDetails.Where(x => x.POId == d.Id).ToList())
                    {
                        BLL.PurchaseOrderDetail b_pod = new BLL.PurchaseOrderDetail();
                        d_pod.ToMap(b_pod);
                        b_pod.SNo = ++i;
                        PO.PODetails.Add(b_pod);

                        b_pod.ProductName = (d_pod.Product ?? DB.Products.Find(d_pod.ProductId) ?? new DAL.Product()).ProductName;
                        b_pod.UOMName = (d_pod.UOM ?? DB.UOMs.Find(d_pod.UOMId) ?? new DAL.UOM()).Symbol;
                        PO.Status = d.PurchaseOrderDetails.FirstOrDefault().PurchaseDetails.Count() > 0 ? "Purchased" : "Pending";

                    }
                    foreach (var t in DB.Purchase_Order_TaxDetail.Where(x => x.PO_ID == d.Id).ToList())
                    {
                        BLL.TaxMaster tm = new BLL.TaxMaster();
                        tm.Id = t.TaxId.Value;
                        tm.LedgerId = LedgerIdByCompany_TaxId(Caller.CompanyId, t.TaxId.Value);
                        tm.Status = true;
                        tm.TaxAmount = t.TaxAmount.Value;
                        tm.TaxName = string.Format("{0}({1})", t.TaxMaster.Ledger.LedgerName, t.TaxMaster.TaxPercentage);
                        tm.TaxPercentage = t.TaxMaster.TaxPercentage;
                  
                        PO.TaxDetails.Add(tm);
                    }
                    var tl = DB.TaxMasters.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId).ToList();
                    var t2 = tl.Where(p => !PO.TaxDetails.Any(p2 => p2.Id== p.Id)).ToList();

                    foreach (var t1 in t2)
                    {
                        PO.TaxDetails.Add(new BLL.TaxMaster()
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
            return PO;
        }

        public bool PurchaseOrder_Delete(long pk)
        {
            try
            {
                DAL.PurchaseOrder d = DB.PurchaseOrders.Where(x => x.Id == pk).FirstOrDefault();

                if (d != null)
                {
                    var P = PurchaseOrder_DALtoBLL(d);
                    DB.Purchase_Order_TaxDetail.RemoveRange(DB.Purchase_Order_TaxDetail.Where(x => x.PO_ID == d.Id).ToList());
                    DB.PurchaseOrderDetails.RemoveRange(d.PurchaseOrderDetails);
                    DB.PurchaseOrders.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(P, LogDetailType.DELETE);
                    SalesOrder_DeleteByPurchaseOrder(d);
                    if (OtherClientsOnGroup.Count > 0) Clients.Clients(OtherClientsOnGroup).PurchaseOrder_RefNoRefresh(PurchaseOrder_NewRefNo());
                }

                return true;
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return false;
        }

        public List<BLL.PurchaseOrder> PurchaseOrder_POPendingList()
        {
            return DB.PurchaseOrders.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId)
                                    .ToList()
                                    .Select(x => PurchaseOrder_DALtoBLL(x))
                                    .ToList();
        }

        public BLL.PurchaseOrder PurchaseOrder_DALtoBLL(DAL.PurchaseOrder d)
        {
            BLL.PurchaseOrder PO = AppLib.ToMap(d, new BLL.PurchaseOrder());
            try
            {
                foreach (var d_pod in d.PurchaseOrderDetails)
                {
                    PO.PODetails.Add(AppLib.ToMap(d_pod, new BLL.PurchaseOrderDetail()));
                }
                PO.Status = d.PurchaseOrderDetails.FirstOrDefault().PurchaseDetails.Count() > 0 ? "Purchased" : "Pending";
                return PO;
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex);
                return PO;
            }
        }
        public bool Find_PORef(string RefNo, BLL.PurchaseOrder PO)

        {
            DAL.PurchaseOrder d = DB.PurchaseOrders.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.RefNo == RefNo & x.Id != PO.Id).FirstOrDefault();
            if (d == null)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        public List<BLL.PurchaseOrder> PurchaseOrder_List(int? LedgerId, DateTime dtFrom, DateTime dtTo, string BillNo, decimal amtFrom, decimal amtTo)
        {
            List<BLL.PurchaseOrder> lstPurchaseOrder = new List<BLL.PurchaseOrder>();

            BLL.PurchaseOrder rp = new BLL.PurchaseOrder();
            try
            {
                foreach (var l in DB.PurchaseOrders.
                      Where(x => x.PODate >= dtFrom && x.PODate <= dtTo
                      && (x.LedgerId == LedgerId || LedgerId == null)

                      && (BillNo == "" || x.RefNo == BillNo)
                      && (x.TotalAmount >= amtFrom && x.TotalAmount <= amtTo) &&
                      x.Ledger.AccountGroup.CompanyId == Caller.CompanyId).ToList())
                {
                    rp = new BLL.PurchaseOrder();
                    rp.TotalAmount = l.TotalAmount;

                    rp.RefNo = l.RefNo;

                    rp.Id = l.Id;
                    rp.LedgerId = l.LedgerId;
                    rp.LedgerName = string.Format("{0}-{1}", l.Ledger.AccountGroup.GroupCode, l.Ledger.LedgerName);
                    rp.PODate = l.PODate;
                    rp.RefCode = l.RefCode;
                    rp.RefNo = l.RefNo;
                    rp.Status = l.PurchaseOrderDetails.FirstOrDefault().PurchaseDetails.Count() > 0 ? "Purchased" : "Pending";
                    lstPurchaseOrder.Add(rp);
                    lstPurchaseOrder = lstPurchaseOrder.OrderBy(x => x.PODate).ToList();
                }

            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return lstPurchaseOrder;
        }





        #endregion
    }
}