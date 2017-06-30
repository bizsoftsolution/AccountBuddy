﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AccountBuddy.Common;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {

        #region Sales
        public string Sales_NewRefNo()
        {
            return Sales_NewRefNoByCompanyId(Caller.CompanyId);
        }
        public string Sales_NewRefNoByCompanyId(int CompanyId)
        {
            DateTime dt = DateTime.Now;
            string Prefix = string.Format("{0}{1:yy}{2:X}", BLL.FormPrefix.Sales, dt, dt.Month);
            long No = 0;

            var d = DB.Sales.Where(x => x.Ledger.AccountGroup.CompanyId == CompanyId && x.RefNo.StartsWith(Prefix))
                                     .OrderByDescending(x => x.RefNo)
                                     .FirstOrDefault();

            if (d != null) No = Convert.ToInt64(d.RefNo.Substring(Prefix.Length ), 16);

            return string.Format("{0}{1:X5}", Prefix, No + 1);
        }
        public bool Sales_Save(BLL.Sale P)
        {
            try
            {
                
                DAL.Sale d = DB.Sales.Where(x => x.Id == P.Id).FirstOrDefault();

                if (d == null)
                {

                    d = new DAL.Sale();
                    DB.Sales.Add(d);

                    P.toCopy<DAL.Sale>(d);

                    foreach (var b_pod in P.SDetails)
                    {
                        DAL.SalesDetail d_pod = new DAL.SalesDetail();
                        b_pod.toCopy<DAL.SalesDetail>(d_pod);
                       d.SalesDetails.Add(d_pod);
                    }
                    DB.SaveChanges();

                    P.Id = d.Id;
                    LogDetailStore(P, LogDetailType.INSERT);
                }
                else
                {
                    foreach (var d_Sd in d.SalesDetails)
                    {
                        BLL.SalesDetail b_Sd = P.SDetails.Where(x => x.Id == d_Sd.Id).FirstOrDefault();
                        if (b_Sd == null) d.SalesDetails.Remove(d_Sd);
                    }

                    P.toCopy<DAL.Sale>(d);
                    foreach (var b_Sd in P.SDetails)
                    {
                        DAL.SalesDetail d_Sd = d.SalesDetails.Where(x => x.Id == b_Sd.Id).FirstOrDefault();
                        if (d_Sd == null)
                        {
                            d_Sd = new DAL.SalesDetail();
                            d.SalesDetails.Add(d_Sd);
                        }
                        b_Sd.toCopy<DAL.SalesDetail>(d_Sd);
                    }
                    LogDetailStore(P, LogDetailType.UPDATE);
                   
                }
                Clients.Clients(OtherLoginClientsOnGroup).Sales_RefNoRefresh(Sales_NewRefNo());
                Journal_SaveBySales(d);
                Purchase_SaveBySales(d);
         return true;
            }
            catch (Exception ex) { }
            return false;
        }

        #region Purchase
        void Sales_SaveByPurchase(DAL.Purchase P)
        {
            string RefCode = string.Format("{0}{1}", BLL.FormPrefix.Purchase, P.Id);

            DAL.Sale s = DB.Sales.Where(x => x.RefCode == RefCode).FirstOrDefault();
            if (P.Ledger.LedgerName.StartsWith("CM-") || P.Ledger.LedgerName.StartsWith("WH-") || P.Ledger.LedgerName.StartsWith("DL-"))
            {
                var LName = LedgerNameByCompanyId(Caller.CompanyId);
                var CId = CompanyIdByLedgerName(P.Ledger.LedgerName);
                var LId = LedgerIdByCompany(LName, CId);

                if (LId != 0)
                {
                    if (s == null)
                    {
                        s = new DAL.Sale();
                        s.RefNo = Sales_NewRefNoByCompanyId(CId);
                        s.RefCode = RefCode;
                        DB.Sales.Add(s);
                    }
                    else
                    {
                        DB.SalesDetails.RemoveRange(s.SalesDetails);
                    }

                    s.SalesDate = P.PurchaseDate;
                  s.DiscountAmount = P.DiscountAmount;
                    s.ExtraAmount = P.ExtraAmount;
                    s.GSTAmount = P.GSTAmount;
                    s.ItemAmount = P.ItemAmount;
                    s.TotalAmount = P.TotalAmount;
                    s.LedgerId = LId;
                    s.TransactionTypeId = P.TransactionTypeId;
                    foreach (var b_pod in P.PurchaseDetails)
                    {
                        DAL.SalesDetail d_pod = new DAL.SalesDetail();
                        b_pod.toCopy<DAL.SalesDetail>(d_pod);
                        s.SalesDetails.Add(d_pod);
                    }
                    DB.SaveChanges();
                    Journal_SaveBySales(s);
                }
            }
        }
        public bool Sales_DeleteByPurchase(DAL.Purchase P)
        {
            try
            {
                string RefCode = string.Format("{0}{1}", BLL.FormPrefix.Purchase, P.Id);
                DAL.Sale d = DB.Sales.Where(x => x.RefCode == RefCode).FirstOrDefault();
                if (d != null)
                {
                    Sales_Delete(d.Id);
                }
            }
            catch (Exception ex) { }
            return false;
        }
        #endregion


        public BLL.Sale Sales_Find(string SearchText)
        {
            BLL.Sale P = new BLL.Sale();
            try
            {

                DAL.Sale d = DB.Sales.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.RefNo == SearchText).FirstOrDefault();
                DB.Entry(d).Reload();
                if (d != null)
                {

                    d.toCopy<BLL.Sale>(P);
                    P.LedgerName = (d.Ledger ?? DB.Ledgers.Find(d.LedgerId) ?? new DAL.Ledger()).LedgerName;
                    P.TransactionType = (d.TransactionType ?? DB.TransactionTypes.Find(d.TransactionTypeId) ?? new DAL.TransactionType()).Type;
                    foreach (var d_pod in d.SalesDetails)
                    {
                        BLL.SalesDetail b_pod = new BLL.SalesDetail();
                        d_pod.toCopy<BLL.SalesDetail>(b_pod);
                        P.SDetails.Add(b_pod);
                        b_pod.ProductName = (d_pod.Product ?? DB.Products.Find(d_pod.ProductId) ?? new DAL.Product()).ProductName;
                        b_pod.UOMName = (d_pod.UOM ?? DB.UOMs.Find(d_pod.UOMId) ?? new DAL.UOM()).Symbol;
                    }

                }
            }
            catch (Exception ex) { }
            return P;
        }
        public BLL.Sale Sales_FindById(int Id)
        {
            BLL.Sale P = new BLL.Sale();
            try
            {

                DAL.Sale d = DB.Sales.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.Id == Id).FirstOrDefault();
                DB.Entry(d).Reload();
                if (d != null)
                {

                    d.toCopy<BLL.Sale>(P);
                    P.LedgerName = (d.Ledger ?? DB.Ledgers.Find(d.LedgerId) ?? new DAL.Ledger()).LedgerName;
                    P.TransactionType = (d.TransactionType ?? DB.TransactionTypes.Find(d.TransactionTypeId) ?? new DAL.TransactionType()).Type;
                    foreach (var d_pod in d.SalesDetails)
                    {
                        BLL.SalesDetail b_pod = new BLL.SalesDetail();
                        d_pod.toCopy<BLL.SalesDetail>(b_pod);
                        P.SDetails.Add(b_pod);
                        b_pod.ProductName = (d_pod.Product ?? DB.Products.Find(d_pod.ProductId) ?? new DAL.Product()).ProductName;
                        b_pod.UOMName = (d_pod.UOM ?? DB.UOMs.Find(d_pod.UOMId) ?? new DAL.UOM()).Symbol;
                    }

                }
            }
            catch (Exception ex) { }
            return P;
        }
        public bool Sales_Delete(long pk)
        {
            try
            {
                DAL.Sale d = DB.Sales.Where(x => x.Id == pk).FirstOrDefault();

                if (d != null)
                {
                    var P = Sales_DALtoBLL(d);
                    DB.SalesDetails.RemoveRange(d.SalesDetails);
                    DB.Sales.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(P, LogDetailType.DELETE);
                    Purchase_DeleteBySales(d);
                    Journal_DeleteBySales(P);
                }
                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public BLL.Sale Sales_DALtoBLL(DAL.Sale d)
        {
            BLL.Sale S = d.toCopy<BLL.Sale>(new BLL.Sale());
            foreach (var d_Sd in d.SalesDetails)
            {
                S.SDetails.Add(d_Sd.toCopy<BLL.SalesDetail>(new BLL.SalesDetail()));
            }
            return S;
        }
        public bool Find_SRef(string RefNo, BLL.Sale PO)
        {
            try
            {
                Common.AppLib.WriteLog(string.Format("Find_SRef: RefNo={0}, SaleId = {1}", RefNo, PO.Id));
                DAL.Sale d = DB.Sales.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.RefNo == RefNo & x.Id != PO.Id).FirstOrDefault();

                if (d == null)
                {
                    Common.AppLib.WriteLog(string.Format("Find_SRef is return false"));
                    return false;
                }                
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog("Error on Find_SRef");
            }
            return true;
        }

        #endregion
    }
}