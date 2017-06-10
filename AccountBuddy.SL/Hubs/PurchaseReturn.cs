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

                    P.Id = d.Id;
                    LogDetailStore(P, LogDetailType.INSERT);
                }
                else
                {
                    foreach (var d_Pd in d.PurchaseReturnDetails)
                    {
                        BLL.PurchaseReturnDetail b_Pd = P.PRDetails.Where(x => x.Id == d_Pd.Id).FirstOrDefault();
                        if (b_Pd == null) d.PurchaseReturnDetails.Remove(d_Pd);
                    }

                    P.toCopy<DAL.PurchaseReturn>(d);
                    foreach (var b_Pd in P.PRDetails)
                    {
                        DAL.PurchaseReturnDetail d_Pd = d.PurchaseReturnDetails.Where(x => x.Id == b_Pd.Id).FirstOrDefault();
                        if (d_Pd == null)
                        {
                            d_Pd = new DAL.PurchaseReturnDetail();
                            d.PurchaseReturnDetails.Add(d_Pd);
                        }
                        b_Pd.toCopy<DAL.PurchaseReturnDetail>(d_Pd);
                    }
                    DB.SaveChanges();
                    LogDetailStore(P, LogDetailType.UPDATE);
                }
                Journal_SaveByPurchaseReturn(P);
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

                DAL.PurchaseReturn d = DB.PurchaseReturns.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.RefNo == SearchText).FirstOrDefault();
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
                    var P = PurchaseReturn_DALtoBLL(d);
                    DB.PurchaseReturnDetails.RemoveRange(d.PurchaseReturnDetails);
                    DB.PurchaseReturns.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(P, LogDetailType.DELETE);
                    Journal_DeleteByPurchaseReturn(P);
                }
                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public BLL.PurchaseReturn PurchaseReturn_DALtoBLL(DAL.PurchaseReturn d)
        {
            BLL.PurchaseReturn PR = d.toCopy<BLL.PurchaseReturn>(new BLL.PurchaseReturn());
            foreach (var d_PRd in d.PurchaseReturnDetails)
            {
                PR.PRDetails.Add(d_PRd.toCopy<BLL.PurchaseReturnDetail>(new BLL.PurchaseReturnDetail()));
            }
            return PR;
        }
        public bool Find_PRRef(string RefNo, BLL.PurchaseReturn PO)

        {
            DAL.PurchaseReturn d = DB.PurchaseReturns.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.RefNo == RefNo & x.Id != PO.Id).FirstOrDefault();
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