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

        public bool SalesOrder_Save(BLL.SalesOrder SO)
        {
            try
            {
                
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
                    SO.Id = d.Id;
                    LogDetailStore(SO, LogDetailType.INSERT);
                }
                else
                {
                    foreach (var d_SOd in d.SalesOrderDetails)
                    {
                        BLL.SalesOrderDetail b_SOd = SO.SODetails.Where(x => x.Id == d_SOd.Id).FirstOrDefault();
                        if (b_SOd == null) d.SalesOrderDetails.Remove(d_SOd);
                    }

                    SO.toCopy<DAL.SalesOrder>(d);
                    foreach (var b_SOd in SO.SODetails)
                    {
                        DAL.SalesOrderDetail d_SOd = d.SalesOrderDetails.Where(x => x.Id == b_SOd.Id).FirstOrDefault();
                        if (d_SOd == null)
                        {
                            d_SOd = new DAL.SalesOrderDetail();
                            d.SalesOrderDetails.Add(d_SOd);
                        }
                        b_SOd.toCopy<DAL.SalesOrderDetail>(d_SOd);
                    }
                    LogDetailStore(SO, LogDetailType.UPDATE);
                    PurchaseOrder_SaveBySalesOrder(SO);
                }
                
                Clients.Clients(OtherLoginClientsOnGroup).SalesOrder_SOPendingSave(SO);

                return true;
                
            }

           catch (Exception ex) { }
            return false;
        }


        public bool SalesOrder_SaveByPurchaseOrder(BLL.PurchaseOrder PO)
        {
            try
            {
                var LName = DB.Ledgers.Where(x => x.Id == PO.LedgerId).FirstOrDefault().LedgerName;

                if(LName.StartsWith("CM-") || LName.StartsWith("WH-"))
                {
                   
                    DAL.SalesOrder d = DB.SalesOrders.Where(x => x.RefNo == PO.RefNo && x.Ledger.AccountGroup.CompanyId==Caller.UnderCompanyId).FirstOrDefault();
                    d.ExtraAmount = PO.Extras.Value;
                    d.SODate = PO.PODate.Value;
                    if (d != null)
                    {
                        DB.SalesOrderDetails.RemoveRange(d.SalesOrderDetails);
                        DB.SalesOrders.Remove(d);
                        DB.SaveChanges();
                    }


                    d = new DAL.SalesOrder();
                    DB.SalesOrders.Add(d);
                    var LNameTo = LedgerNameByCompanyId(Caller.CompanyId);
                    PO.LedgerId = LedgerIdByCompany(LNameTo, Caller.UnderCompanyId);

                    PO.toCopy<DAL.SalesOrder>(d);


                    foreach (var b_pod in PO.PODetails)
                    {
                        DAL.SalesOrderDetail d_pod = new DAL.SalesOrderDetail();
                        b_pod.toCopy<DAL.SalesOrderDetail>(d_pod);
                        d.SalesOrderDetails.Add(d_pod);
                    }
                    DB.SaveChanges();
                    PO.Id = d.Id;
                    LogDetailStore(PO, LogDetailType.INSERT);

                    return true;
                }

                
            }
            catch (Exception ex) { }
            return false;
        }
        public bool SalesOrder_DeleteByPurchaseOrder(BLL.PurchaseOrder PO)
        {
            try
            {
                var LName = DB.Ledgers.Where(x => x.Id == PO.LedgerId).FirstOrDefault().LedgerName;

                if (LName.StartsWith("CM-") || LName.StartsWith("WH-"))
                {

                    DAL.SalesOrder d = DB.SalesOrders.Where(x => x.RefNo == PO.RefNo && x.Ledger.AccountGroup.CompanyId == Caller.UnderCompanyId).FirstOrDefault();

                    if (d != null)
                    {
                        DB.SalesOrderDetails.RemoveRange(d.SalesOrderDetails);
                        DB.SalesOrders.Remove(d);
                        DB.SaveChanges();
                    }

                    return true;
                }


            }
            catch (Exception ex) { }
            return false;
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

                    d.toCopy<BLL.SalesOrder>(SO);
                    SO.LedgerName = (d.Ledger ?? DB.Ledgers.Find(d.LedgerId) ?? new DAL.Ledger()).LedgerName;
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
                    var s = SalesOrder_DALtoBLL(d);
                    DB.SalesOrderDetails.RemoveRange(d.SalesOrderDetails);
                    DB.SalesOrders.Remove(d);
                    DB.SaveChanges();

                    LogDetailStore(s, LogDetailType.DELETE);
                    PurchaseOrder_DeleteBySalesOrder(s);
                }
                return true;
            }
            catch (Exception ex) { }
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
            BLL.SalesOrder SO = d.toCopy<BLL.SalesOrder>(new BLL.SalesOrder());
            foreach (var d_SOd in d.SalesOrderDetails)
            {
                SO.SODetails.Add(d_SOd.toCopy<BLL.SalesOrderDetail>(new BLL.SalesOrderDetail()));
            }
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
                var LId=  LedgerIdByCompany(LNameTo, Caller.UnderCompanyId);

                d2 = DB.SalesOrders.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.UnderCompanyId && x.RefNo == RefNo ).FirstOrDefault();
                if (d2 != null)
                {
                    if(d2.LedgerId == LId)
                    {
                        d2 = null;
                    }
                }
            }
           


            if (d1 == null || d2 ==null)
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