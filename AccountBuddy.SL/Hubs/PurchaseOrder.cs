﻿using System;
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
                    DB.PurchaseOrderDetails.RemoveRange(d.PurchaseOrderDetails);
                    DB.PurchaseOrders.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(PurchaseOrder_DALtoBLL(d), LogDetailType.DELETE);
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