using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AccountBuddy.Common;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {

        #region Sales
     
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

                    LogDetailStore(P, LogDetailType.INSERT);
                }
                else
                {
                    P.toCopy<DAL.Sale>(d);
                    foreach (var b_pod in P.SDetails)
                    {
                        DAL.SalesDetail d_pod = new DAL.SalesDetail();
                        b_pod.toCopy<DAL.SalesDetail>(d_pod);
                        d.SalesDetails.Add(d_pod);
                    }
                    DB.SaveChanges();
                    LogDetailStore(P, LogDetailType.UPDATE);
                }

                return true;
            }
            catch (Exception ex) { }
            return false;
        }

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

        public bool Sales_Delete(long pk)
        {
            try
            {
                DAL.Sale d = DB.Sales.Where(x => x.Id == pk).FirstOrDefault();

                if (d != null)
                {
                    BLL.Sale P = new BLL.Sale();
                    d.toCopy<BLL.Sale>(P);
                    P.LedgerName = d.Ledger.LedgerName;
                    P.TransactionType = d.TransactionType.Type;
                    foreach (var d_pod in d.SalesDetails)
                    {
                        BLL.SalesDetail b_pod = new BLL.SalesDetail();
                        d_pod.toCopy<BLL.SalesDetail>(b_pod);
                        P.SDetails.Add(b_pod);

                    }
                    DB.SalesDetails.RemoveRange(d.SalesDetails);
                    DB.Sales.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(P, LogDetailType.DELETE);
                }
                return true;
            }
            catch (Exception ex) { }
            return false;
        }
        

        public bool Find_SRef(string RefNo, BLL.Sale PO)

        {
            DAL.Sale d = DB.Sales.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.RefNo == RefNo & x.Id != PO.Id).FirstOrDefault();
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