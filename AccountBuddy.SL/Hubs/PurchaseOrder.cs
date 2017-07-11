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
            string Prefix = string.Format("{0}{1:yy}{2:X}",BLL.FormPrefix.PurchaseOrder,dt,dt.Month);
            long No = 0;

            var d = DB.PurchaseOrders.Where(x => x.Ledger.AccountGroup.CompanyId == CompanyId && x.RefNo.StartsWith(Prefix))
                                     .OrderByDescending(x=> x.RefNo)
                                     .FirstOrDefault();

            if (d != null)  No = Convert.ToInt64(d.RefNo.Substring(Prefix.Length), 16);
            
            return string.Format("{0}{1:X5}", Prefix, No + 1);
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

                Clients.Clients(OtherLoginClientsOnGroup).PurchaseOrder_RefNoRefresh(PurchaseOrder_NewRefNo());

                SalesOrder_SaveByPurchaseOrder(d);
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
                P.RefNo =Purchase_NewRefNo();
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

        #region SalesOrder

        void PurchaseOrder_SaveBySalesOrder(DAL.SalesOrder S)
        {
            string RefCode = string.Format("{0}{1}", BLL.FormPrefix.SalesOrder, S.Id);

            DAL.PurchaseOrder p = DB.PurchaseOrders.Where(x => x.RefCode == RefCode).FirstOrDefault();

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
                        b_pod.toCopy<DAL.PurchaseOrderDetail>(d_pod);
                        p.PurchaseOrderDetails.Add(d_pod);
                    }
                    DB.SaveChanges();
                  
                }
            }


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
            catch (Exception ex) { }
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

                    d.toCopy<BLL.PurchaseOrder>(PO);
                    PO.LedgerName = (d.Ledger?? DB.Ledgers.Find(d.LedgerId) ?? new DAL.Ledger()).LedgerName;
                    foreach (var d_pod in d.PurchaseOrderDetails)
                    {
                        BLL.PurchaseOrderDetail b_pod = new BLL.PurchaseOrderDetail();
                        d_pod.toCopy<BLL.PurchaseOrderDetail>(b_pod);
                        PO.PODetails.Add(b_pod);
                        b_pod.ProductName = (d_pod.Product ?? DB.Products.Find(d_pod.ProductId) ?? new DAL.Product()).ProductName;
                        b_pod.UOMName = (d_pod.UOM ?? DB.UOMs.Find(d_pod.UOMId) ?? new DAL.UOM()).Symbol;
                        PO.Status = d.PurchaseOrderDetails.FirstOrDefault().PurchaseDetails.Count() > 0 ? "Purchased" : "Pending";

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
                    SalesOrder_DeleteByPurchaseOrder(d);
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