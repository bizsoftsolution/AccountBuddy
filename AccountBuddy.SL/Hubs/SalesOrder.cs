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

        #region list
        public static List<BLL.SalesOrder> _SOPendingList;
        public static List<BLL.SalesOrder> SOPendingList
        {
            get
            {
                if (_SOPendingList == null)
                {
                    _SOPendingList = new List<BLL.SalesOrder>();
                    foreach (var d1 in DB.SalesOrders.OrderBy(x => x.RefNo).ToList())
                    {
                        BLL.SalesOrder d2 = new BLL.SalesOrder();
                        d1.toCopy<BLL.SalesOrder>(d2);
                        _SOPendingList.Add(d2);
                    }

                }
                return _SOPendingList;
            }
            set
            {
                _SOPendingList = value;
            }
        }
        #endregion

        public bool SalesOrder_Save(BLL.SalesOrder SO)
        {
            try
            {
                SO.CompanyId = Caller.CompanyId;

                DAL.SalesOrder d = DB.SalesOrders.Where(x => x.Id == SO.Id).FirstOrDefault();

                if (d == null)
                {

                    d = new DAL.SalesOrder();
                    DB.SalesOrders.Add(d);

                    SO.toCopy<DAL.SalesOrder>(d);

                    foreach (var b_pod in SO.SODetails)
                    {
                        DAL.SalesOrderDetail d_pod = new DAL.SalesOrderDetail();
                        b_pod.toCopy<DAL.SalesOrderDetail>(d_pod);
                        d.SalesOrderDetails.Add(d_pod);
                    }
                    DB.SaveChanges();

                    LogDetailStore(SO, LogDetailType.INSERT);
                }
                else
                {
                    SO.toCopy<DAL.SalesOrder>(d);
                    foreach (var b_pod in SO.SODetails)
                    {
                        DAL.SalesOrderDetail d_pod = new DAL.SalesOrderDetail();
                        b_pod.toCopy<DAL.SalesOrderDetail>(d_pod);
                        d.SalesOrderDetails.Add(d_pod);
                    }
                    DB.SaveChanges();
                    LogDetailStore(SO, LogDetailType.UPDATE);
                }
                BLL.SalesOrder B_SO = SOPendingList.Where(x => x.Id == SO.Id).FirstOrDefault();

                if (B_SO == null)
                {
                    B_SO = new BLL.SalesOrder();
                    SOPendingList.Add(B_SO);
                }

                SO.toCopy<BLL.SalesOrder>(B_SO);
                Clients.Clients(OtherLoginClientsOnGroup).SalesOrder_SOPendingSave(B_SO);

                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public BLL.SalesOrder SalesOrder_Find(string SearchText)
        {
            BLL.SalesOrder SO = new BLL.SalesOrder();
            try
            {

                DAL.SalesOrder d = DB.SalesOrders.Where(x => x.RefNo == SearchText).FirstOrDefault();
                DB.Entry(d).Reload();
                if (d != null)
                {

                    d.toCopy<BLL.SalesOrder>(SO);
                    SO.LedgerName = (d.Ledger ?? DB.Ledgers.Find(d.LedgerId) ?? new DAL.Ledger()).LedgerName;
                    SO.TransactionType = (d.TransactionType ?? DB.TransactionTypes.Find(d.TransactionTypeId) ?? new DAL.TransactionType()).Type;
                    foreach (var d_pod in d.SalesOrderDetails)
                    {
                        BLL.SalesOrderDetail b_pod = new BLL.SalesOrderDetail();
                        d_pod.toCopy<BLL.SalesOrderDetail>(b_pod);
                        SO.SODetails.Add(b_pod);
                        b_pod.ProductName = (d_pod.Product ?? DB.Products.Find(d_pod.ProductId) ?? new DAL.Product()).ProductName;
                        b_pod.UOMName = (d_pod.UOM ?? DB.UOMs.Find(d_pod.UOMId) ?? new DAL.UOM()).Symbol;
                    }

                }
            }
            catch (Exception ex) { }
            return SO;
        }

        public bool SalesOrder_Delete(long pk)
        {
            try
            {
                DAL.SalesOrder d = DB.SalesOrders.Where(x => x.Id == pk).FirstOrDefault();

                if (d != null)
                {
                    BLL.SalesOrder SO = new BLL.SalesOrder();
                    d.toCopy<BLL.SalesOrder>(SO);
                    SO.LedgerName = d.Ledger.LedgerName;
                    SO.TransactionType = d.TransactionType.Type;
                    foreach (var d_pod in d.SalesOrderDetails)
                    {
                        BLL.SalesOrderDetail b_pod = new BLL.SalesOrderDetail();
                        d_pod.toCopy<BLL.SalesOrderDetail>(b_pod);
                        SO.SODetails.Add(b_pod);

                    }
                    DB.SalesOrderDetails.RemoveRange(d.SalesOrderDetails);
                    DB.SalesOrders.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(SO, LogDetailType.DELETE);

                    BLL.SalesOrder B_SO = SOPendingList.Where(x => x.Id == SO.Id).FirstOrDefault();
                    if (B_SO != null)
                    {
                        Clients.Clients(OtherLoginClientsOnGroup).SalesOrder_SOPendingDelete(B_SO.Id);
                        SOPendingList.Remove(B_SO);
                    }
                }
                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public List<BLL.SalesOrder> SalesOrder_SOPendingList()
        {
            return SOPendingList.Where(x => x.CompanyId == Caller.CompanyId).ToList();
        }

        public bool Find_SORef(string RefNo, BLL.SalesOrder PO)

        {
            DAL.SalesOrder d = DB.SalesOrders.Where(x => x.RefNo == RefNo & x.Id != PO.Id).FirstOrDefault();
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