﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AccountBuddy.Common;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        #region Purchase

        #region list
        public static List<BLL.Purchase> _PurPendingList;
        public static List<BLL.Purchase> PurPendingList
        {
            get
            {
                if (_PurPendingList == null)
                {
                    _PurPendingList = new List<BLL.Purchase>();
                    foreach (var d1 in DB.Purchases.Where(x => x.TransactionType.Type == "Credit").OrderBy(x => x.RefNo).ToList())
                    {
                        BLL.Purchase d2 = new BLL.Purchase();
                        d1.toCopy<BLL.Purchase>(d2);
                        _PurPendingList.Add(d2);
                    }

                }
                return _PurPendingList;
            }
            set
            {
                _PurPendingList = value;
            }
        }
        #endregion

        public bool Purchase_Save(BLL.Purchase P)
        {
            try
            {
                P.CompanyId = Caller.CompanyId;

                DAL.Purchase d = DB.Purchases.Where(x => x.Id == P.Id).FirstOrDefault();

                if (d == null)
                {

                    d = new DAL.Purchase();
                    DB.Purchases.Add(d);

                    P.toCopy<DAL.Purchase>(d);

                    foreach (var b_pod in P.PDetails)
                    {
                        DAL.PurchaseDetail d_pod = new DAL.PurchaseDetail();
                        b_pod.toCopy<DAL.PurchaseDetail>(d_pod);
                        d.PurchaseDetails.Add(d_pod);
                    }
                    DB.SaveChanges();

                    LogDetailStore(P, LogDetailType.INSERT);
                }
                else
                {
                    P.toCopy<DAL.Purchase>(d);
                    foreach (var b_pod in P.PDetails)
                    {
                        DAL.PurchaseDetail d_pod = new DAL.PurchaseDetail();
                        b_pod.toCopy<DAL.PurchaseDetail>(d_pod);
                        d.PurchaseDetails.Add(d_pod);
                    }
                    DB.SaveChanges();
                    LogDetailStore(P, LogDetailType.UPDATE);
                }

                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public BLL.Purchase Purchase_Find(string SearchText)
        {
            BLL.Purchase P = new BLL.Purchase();
            try
            {

                DAL.Purchase d = DB.Purchases.Where(x => x.RefNo == SearchText).FirstOrDefault();
                DB.Entry(d).Reload();
                if (d != null)
                {

                    d.toCopy<BLL.Purchase>(P);
                    P.LedgerName = (d.Ledger ?? DB.Ledgers.Find(d.LedgerId) ?? new DAL.Ledger()).LedgerName;
                    P.TransactionType = (d.TransactionType ?? DB.TransactionTypes.Find(d.TransactionTypeId) ?? new DAL.TransactionType()).Type;
                    foreach (var d_pod in d.PurchaseDetails)
                    {
                        BLL.PurchaseDetail b_pod = new BLL.PurchaseDetail();
                        d_pod.toCopy<BLL.PurchaseDetail>(b_pod);
                        P.PDetails.Add(b_pod);
                        b_pod.ProductName = (d_pod.Product ?? DB.Products.Find(d_pod.ProductId) ?? new DAL.Product()).ProductName;
                        b_pod.UOMName = (d_pod.UOM ?? DB.UOMs.Find(d_pod.UOMId) ?? new DAL.UOM()).Symbol;
                    }

                }
            }
            catch (Exception ex) { }
            return P;
        }

        public bool Purchase_Delete(long pk)
        {
            try
            {
                DAL.Purchase d = DB.Purchases.Where(x => x.Id == pk).FirstOrDefault();

                if (d != null)
                {
                    BLL.Purchase P = new BLL.Purchase();
                    d.toCopy<BLL.Purchase>(P);
                    P.LedgerName = d.Ledger.LedgerName;
                    P.TransactionType = d.TransactionType.Type;
                    foreach (var d_pod in d.PurchaseDetails)
                    {
                        BLL.PurchaseDetail b_pod = new BLL.PurchaseDetail();
                        d_pod.toCopy<BLL.PurchaseDetail>(b_pod);
                        P.PDetails.Add(b_pod);

                    }
                    DB.PurchaseDetails.RemoveRange(d.PurchaseDetails);
                    DB.Purchases.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(P, LogDetailType.DELETE);
                }
                return true;
            }
            catch (Exception ex) { }
            return false;
        }
        public List<BLL.Purchase> Purchase_PurPendingList()
        {
            return PurPendingList.Where(x => x.CompanyId == Caller.CompanyId).ToList();
        }

        public bool Find_PRef(string RefNo, BLL.Purchase PO)

        {
            DAL.Purchase d = DB.Purchases.Where(x => x.RefNo == RefNo & x.Id != PO.Id).FirstOrDefault();
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