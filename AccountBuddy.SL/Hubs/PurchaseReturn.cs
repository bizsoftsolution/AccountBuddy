using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AccountBuddy.Common;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        #region PurchaseReturn

     

        public bool PurchaseReturn_Save(BLL.PurchaseReturn P)
        {
            try
            {
                P.CompanyId = Caller.CompanyId;

                DAL.PurchaseReturn d = DB.PurchaseReturns.Where(x => x.Id == P.Id).FirstOrDefault();

                if (d == null)
                {

                    d = new DAL.PurchaseReturn();
                    DB.PurchaseReturns.Add(d);

                    P.toCopy<DAL.PurchaseReturn>(d);

                    foreach (var b_pod in P.PRDetails)
                    {
                        DAL.PurchaseReturnDetail d_pod = new DAL.PurchaseReturnDetail();
                        b_pod.toCopy<DAL.PurchaseReturnDetail>(d_pod);
                        d.PurchaseReturnDetails.Add(d_pod);
                    }
                    DB.SaveChanges();

                    LogDetailStore(P, LogDetailType.INSERT);
                }
                else
                {
                    P.toCopy<DAL.PurchaseReturn>(d);
                    foreach (var b_pod in P.PRDetails)
                    {
                        DAL.PurchaseReturnDetail d_pod = new DAL.PurchaseReturnDetail();
                        b_pod.toCopy<DAL.PurchaseReturnDetail>(d_pod);
                        d.PurchaseReturnDetails.Add(d_pod);
                    }
                    DB.SaveChanges();
                    LogDetailStore(P, LogDetailType.UPDATE);
                }

                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public BLL.PurchaseReturn PurchaseReturn_Find(string SearchText)
        {
            BLL.PurchaseReturn P = new BLL.PurchaseReturn();
            try
            {

                DAL.PurchaseReturn d = DB.PurchaseReturns.Where(x => x.RefNo == SearchText).FirstOrDefault();
                DB.Entry(d).Reload();
                if (d != null)
                {

                    d.toCopy<BLL.PurchaseReturn>(P);
                    P.LedgerName = (d.Ledger ?? DB.Ledgers.Find(d.LedgerId) ?? new DAL.Ledger()).LedgerName;
                    P.TransactionType = (d.TransactionType ?? DB.TransactionTypes.Find(d.TransactionTypeId) ?? new DAL.TransactionType()).Type;
                    foreach (var d_pod in d.PurchaseReturnDetails)
                    {
                        BLL.PurchaseReturnDetail b_pod = new BLL.PurchaseReturnDetail();
                        d_pod.toCopy<BLL.PurchaseReturnDetail>(b_pod);
                        P.PRDetails.Add(b_pod);
                        b_pod.ProductName = (d_pod.Product ?? DB.Products.Find(d_pod.ProductId) ?? new DAL.Product()).ProductName;
                        b_pod.UOMName = (d_pod.UOM ?? DB.UOMs.Find(d_pod.UOMId) ?? new DAL.UOM()).Symbol;
                    }

                }
            }
            catch (Exception ex) { }
            return P;
        }

        public bool PurchaseReturn_Delete(long pk)
        {
            try
            {
                DAL.PurchaseReturn d = DB.PurchaseReturns.Where(x => x.Id == pk).FirstOrDefault();

                if (d != null)
                {
                    BLL.PurchaseReturn P = new BLL.PurchaseReturn();
                    d.toCopy<BLL.PurchaseReturn>(P);
                    P.LedgerName = d.Ledger.LedgerName;
                    P.TransactionType = d.TransactionType.Type;
                    foreach (var d_pod in d.PurchaseReturnDetails)
                    {
                        BLL.PurchaseReturnDetail b_pod = new BLL.PurchaseReturnDetail();
                        d_pod.toCopy<BLL.PurchaseReturnDetail>(b_pod);
                        P.PRDetails.Add(b_pod);

                    }
                    DB.PurchaseReturnDetails.RemoveRange(d.PurchaseReturnDetails);
                    DB.PurchaseReturns.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(P, LogDetailType.DELETE);
                }
                return true;
            }
            catch (Exception ex) { }
            return false;
        }
       

        public bool Find_PRRef(string RefNo, BLL.PurchaseReturn PO)

        {
            DAL.PurchaseReturn d = DB.PurchaseReturns.Where(x => x.RefNo == RefNo & x.Id != PO.Id).FirstOrDefault();
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