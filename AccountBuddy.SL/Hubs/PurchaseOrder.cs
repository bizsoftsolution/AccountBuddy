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

        public bool PurchaseOrder_Save(BLL.PurchaseOrder PO)
        {
            try
            {
             
                DAL.PurchaseOrder d = DB.PurchaseOrders.Where(x => x.Id == PO.Id).FirstOrDefault();

                if (d == null)
                {

                    d = new DAL.PurchaseOrder();
                    DB.PurchaseOrders.Add(d);

                    PO.toCopy<DAL.PurchaseOrder>(d);

                    foreach (var b_pod in PO.PODetails)
                    {
                        DAL.PurchaseOrderDetail d_pod = new DAL.PurchaseOrderDetail();
                        b_pod.toCopy<DAL.PurchaseOrderDetail>(d_pod);
                        d.PurchaseOrderDetails.Add(d_pod);
                    }
                    DB.SaveChanges();
                    PO.Id = d.Id;
                    LogDetailStore(PO, LogDetailType.INSERT);
                }
                else
                {
                    
                    foreach(var d_pod in d.PurchaseOrderDetails)
                    {
                        BLL.PurchaseOrderDetail b_pod = PO.PODetails.Where(x => x.Id == d_pod.Id).FirstOrDefault();
                        if (b_pod == null)d.PurchaseOrderDetails.Remove(d_pod);                        
                    }

                    PO.toCopy<DAL.PurchaseOrder>(d);
                    foreach (var b_pod in PO.PODetails)
                    {
                        DAL.PurchaseOrderDetail d_pod = d.PurchaseOrderDetails.Where(x => x.Id == b_pod.Id).FirstOrDefault();
                        if (d_pod == null)
                        {
                            d_pod = new DAL.PurchaseOrderDetail();
                            d.PurchaseOrderDetails.Add(d_pod);
                        }
                        b_pod.toCopy<DAL.PurchaseOrderDetail>(d_pod);                        
                    }
                    DB.SaveChanges();
                    LogDetailStore(PO, LogDetailType.UPDATE);
                }

                SalesOrder_SaveByPurchaseOrder(PO);
                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public bool PurchaseOrder_MakePurchase(BLL.PurchaseOrder PO)
        {
            try
            {
                BLL.Purchase P = new BLL.Purchase();

                P.PurchaseDate = PO.PODate.Value;
                P.RefNo = PO.RefNo;
                P.LedgerId = PO.LedgerId;
                P.TransactionType = "Cash";
                P.TransactionTypeId = 1;
                P.ItemAmount = PO.ItemAmount.Value;
                P.DiscountAmount = PO.DiscountAmount.Value;
                P.GSTAmount = PO.GSTAmount.Value;
                P.ExtraAmount = PO.Extras.Value;
                P.TotalAmount = PO.TotalAmount.Value;
                P.Narration = PO.Narration;               


                foreach (var pod in PO.PODetails)
                {
                    BLL.PurchaseDetail PD = new BLL.PurchaseDetail() {
                        PODId = pod.Id,
                        ProductId =pod.ProductId,
                        UOMId = pod.UOMId,
                        UOMName = pod.UOMName,
                        Quantity = pod.Quantity,
                        UnitPrice =pod.UnitPrice,
                        DiscountAmount = pod.DiscountAmount,
                        GSTAmount = pod.GSTAmount,
                        ProductName = pod.ProductName,
                        Amount = pod.Amount
                    };


                    P.PDetails.Add(PD);
                }
                return Purchase_Save(P);
            }
            catch (Exception ex) { }
            return true;
        }

        void PurchaseOrder_SaveBySalesOrder(BLL.SalesOrder SO)
        {
            var refNo = string.Format("PO-{0}", SO.Id);

            DAL.PurchaseOrder p = DB.PurchaseOrders.Where(x => x.RefNo == refNo).FirstOrDefault();
            if (p != null)
            {
                DB.PurchaseOrderDetails.RemoveRange(p.PurchaseOrderDetails);
                DB.PurchaseOrders.Remove(p);
                DB.SaveChanges();
            }
            var pd = SO.SODetails.FirstOrDefault();
            var ld = DB.Ledgers.Where(x => x.Id == SO.LedgerId).FirstOrDefault();

            if (ld.LedgerName.StartsWith("CM-") || ld.LedgerName.StartsWith("WH-") || ld.LedgerName.StartsWith("DL-"))
            {
                var LName = LedgerNameByCompanyId(Caller.CompanyId);

                var CId = CompanyIdByLedgerName(ld.LedgerName);

                p = new DAL.PurchaseOrder();
                p.RefNo = refNo;
                p.PODate = SO.SODate.Value;
                p.DiscountAmount = SO.DiscountAmount.Value;
                p.Extras = SO.ExtraAmount.Value;
                p.GSTAmount = SO.GSTAmount.Value;
                p.ItemAmount = SO.ItemAmount.Value;
                p.TotalAmount = SO.TotalAmount.Value;
                p.LedgerId = LedgerIdByCompany(LName, CId);
               
                if (CId != 0)
                {
                    foreach (var b_pod in SO.SODetails)
                    {
                        DAL.PurchaseOrderDetail d_pod = new DAL.PurchaseOrderDetail();
                        b_pod.toCopy<DAL.PurchaseOrderDetail>(d_pod);
                        p.PurchaseOrderDetails.Add(d_pod);
                    }
                    DB.PurchaseOrders.Add(p);
                    DB.SaveChanges();
                   
                 
                }
            }

        }

        public bool PurchaseOrder_DeleteBySalesOrder(BLL.SalesOrder PO)
        {
            try
            {
                var LName = DB.Ledgers.Where(x => x.Id == PO.LedgerId).FirstOrDefault().LedgerName;

                if (LName.StartsWith("CM-") || LName.StartsWith("WH-"))
                {

                    DAL.PurchaseOrder d = DB.PurchaseOrders.Where(x => x.RefNo == PO.RefNo && x.Ledger.AccountGroup.CompanyId == Caller.UnderCompanyId).FirstOrDefault();

                    if (d != null)
                    {
                        DB.PurchaseOrderDetails.RemoveRange(d.PurchaseOrderDetails);
                        DB.PurchaseOrders.Remove(d);
                        DB.SaveChanges();
                    }

                    return true;
                }


            }
            catch (Exception ex) { }
            return false;
        }


        public BLL.PurchaseOrder PurchaseOrder_Find(string SearchText)
        {
            BLL.PurchaseOrder PO = new BLL.PurchaseOrder();
            try
            {

                DAL.PurchaseOrder d = DB.PurchaseOrders.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.RefNo == SearchText).FirstOrDefault();
                DB.Entry(d).Reload();
                if (d != null)
                {

                    d.toCopy<BLL.PurchaseOrder>(PO);
                    PO.LedgerName = (d.Ledger?? DB.Ledgers.Find(d.LedgerId) ?? new DAL.Ledger()).LedgerName;
                    foreach (var d_pod in d.PurchaseOrderDetails)
                    {
                        BLL.PurchaseOrderDetail b_pod = new BLL.PurchaseOrderDetail();
                        d_pod.toCopy<BLL.PurchaseOrderDetail>(b_pod);
                        PO.PODetails.Add(b_pod);
                        b_pod.ProductName = (d_pod.Product ?? DB.Products.Find(d_pod.ProductId) ?? new DAL.Product()).ProductName;
                        b_pod.UOMName = (d_pod.UOM ?? DB.UOMs.Find(d_pod.UOMId) ?? new DAL.UOM()).Symbol;
                    }

                }
            }
            catch (Exception ex) { }
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
                    DB.PurchaseOrderDetails.RemoveRange(d.PurchaseOrderDetails);
                    DB.PurchaseOrders.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(P, LogDetailType.DELETE);
                    SalesOrder_DeleteByPurchaseOrder(P);
                }

                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public List<BLL.PurchaseOrder> PurchaseOrder_POPendingList()
        {
            return DB.PurchaseOrders.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId)
                                    .ToList()
                                    .Select(x=> PurchaseOrder_DALtoBLL(x) )
                                    .ToList() ;
        }

        public BLL.PurchaseOrder PurchaseOrder_DALtoBLL(DAL.PurchaseOrder d)
        {
            BLL.PurchaseOrder PO = d.toCopy<BLL.PurchaseOrder>(new BLL.PurchaseOrder());
            foreach(var d_pod in d.PurchaseOrderDetails)
            {
                PO.PODetails.Add(d_pod.toCopy<BLL.PurchaseOrderDetail>(new BLL.PurchaseOrderDetail()));
            }
            PO.Status = d.PurchaseOrderDetails.FirstOrDefault().PurchaseDetails.Count() > 0 ? "Purchased" : "Pending";
            return PO;
        }
        public bool Find_PORef(string RefNo, BLL.PurchaseOrder PO)

        {
            DAL.PurchaseOrder d = DB.PurchaseOrders.Where(x => x.Ledger.AccountGroup.CompanyId==Caller.CompanyId && x.RefNo == RefNo & x.Id != PO.Id).FirstOrDefault();
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