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

        #region list
        public static List<BLL.PurchaseOrder> _POPendingList;
        public static List<BLL.PurchaseOrder> POPendingList
        {
            get
            {
                if (_POPendingList == null)
                {
                    _POPendingList = new List<BLL.PurchaseOrder>();
                    foreach (var d1 in DB.PurchaseOrders.OrderBy(x => x.RefNo).ToList())
                    {
                        BLL.PurchaseOrder d2 = new BLL.PurchaseOrder();
                        d1.toCopy<BLL.PurchaseOrder>(d2);
                        _POPendingList.Add(d2);
                    }

                }
                return _POPendingList;
            }
            set
            {
                _POPendingList = value;
            }
        }
        #endregion


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
                    PO.toCopy<DAL.PurchaseOrder>(d);
                    foreach (var b_pod in PO.PODetails)
                    {
                        DAL.PurchaseOrderDetail d_pod = new DAL.PurchaseOrderDetail();
                        b_pod.toCopy<DAL.PurchaseOrderDetail>(d_pod);
                        d.PurchaseOrderDetails.Add(d_pod);
                    }
                    DB.SaveChanges();
                    LogDetailStore(PO, LogDetailType.UPDATE);
                }

                BLL.PurchaseOrder B_PO = POPendingList.Where(x => x.Id == PO.Id).FirstOrDefault();

                if (B_PO == null)
                {
                    B_PO = new BLL.PurchaseOrder();
                    POPendingList.Add(B_PO);
                }

                PO.toCopy<BLL.PurchaseOrder>(B_PO);
                Clients.Clients(OtherLoginClientsOnGroup).PurchaseOrder_POPendingSave(B_PO);

                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public BLL.PurchaseOrder PurchaseOrder_Find(string SearchText)
        {
            BLL.PurchaseOrder PO = new BLL.PurchaseOrder();
            try
            {

                DAL.PurchaseOrder d = DB.PurchaseOrders.Where(x => x.RefNo == SearchText).FirstOrDefault();
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
                    BLL.PurchaseOrder PO = new BLL.PurchaseOrder();
                    d.toCopy<BLL.PurchaseOrder>(PO);
                    PO.LedgerName = d.Ledger.LedgerName;
                    foreach (var d_pod in d.PurchaseOrderDetails)
                    {
                        BLL.PurchaseOrderDetail b_pod = new BLL.PurchaseOrderDetail();
                        d_pod.toCopy<BLL.PurchaseOrderDetail>(b_pod);
                        PO.PODetails.Add(b_pod);

                    }
                    DB.PurchaseOrderDetails.RemoveRange(d.PurchaseOrderDetails);
                    DB.PurchaseOrders.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(PO, LogDetailType.DELETE);

                    BLL.PurchaseOrder B_PO = POPendingList.Where(x => x.Id == PO.Id).FirstOrDefault();
                    if (B_PO != null)
                    {
                        Clients.Clients(OtherLoginClientsOnGroup).PurchaseOrder_POPendingDelete(B_PO.Id);
                        POPendingList.Remove(B_PO);
                    }
                }
                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public List<BLL.PurchaseOrder> PurchaseOrder_POPendingList()
        {
            return POPendingList;
        }

        public bool Find_PORef(string RefNo, BLL.PurchaseOrder PO)

        {
            DAL.PurchaseOrder d = DB.PurchaseOrders.Where(x => x.RefNo == RefNo & x.Id != PO.Id).FirstOrDefault();
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