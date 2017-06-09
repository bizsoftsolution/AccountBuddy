using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AccountBuddy.Common;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        #region SalesReturn
        
        public bool SalesReturn_Save(BLL.SalesReturn P)
        {
            try
            {
                P.CompanyId = Caller.CompanyId;

                DAL.SalesReturn d = DB.SalesReturns.Where(x => x.Id == P.Id).FirstOrDefault();

                if (d == null)
                {

                    d = new DAL.SalesReturn();
                    DB.SalesReturns.Add(d);

                    P.toCopy<DAL.SalesReturn>(d);

                    foreach (var b_pod in P.SRDetails)
                    {
                        DAL.SalesReturnDetail d_pod = new DAL.SalesReturnDetail();
                        b_pod.toCopy<DAL.SalesReturnDetail>(d_pod);
                        d.SalesReturnDetails.Add(d_pod);
                    }
                    DB.SaveChanges();

                    LogDetailStore(P, LogDetailType.INSERT);
                }
                else
                {
                    P.toCopy<DAL.SalesReturn>(d);
                    foreach (var b_pod in P.SRDetails)
                    {
                        DAL.SalesReturnDetail d_pod = new DAL.SalesReturnDetail();
                        b_pod.toCopy<DAL.SalesReturnDetail>(d_pod);
                        d.SalesReturnDetails.Add(d_pod);
                    }
                    DB.SaveChanges();
                    LogDetailStore(P, LogDetailType.UPDATE);
                }

                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public BLL.SalesReturn SalesReturn_Find(string SearchText)
        {
            BLL.SalesReturn P = new BLL.SalesReturn();
            try
            {

                DAL.SalesReturn d = DB.SalesReturns.Where(x => x.RefNo == SearchText).FirstOrDefault();
                DB.Entry(d).Reload();
                if (d != null)
                {

                    d.toCopy<BLL.SalesReturn>(P);
                    P.LedgerName = (d.Ledger ?? DB.Ledgers.Find(d.LedgerId) ?? new DAL.Ledger()).LedgerName;
                    P.TransactionType = (d.TransactionType ?? DB.TransactionTypes.Find(d.TransactionTypeId) ?? new DAL.TransactionType()).Type;
                    foreach (var d_pod in d.SalesReturnDetails)
                    {
                        BLL.SalesReturnDetail b_pod = new BLL.SalesReturnDetail();
                        d_pod.toCopy<BLL.SalesReturnDetail>(b_pod);
                        P.SRDetails.Add(b_pod);
                        b_pod.ProductName = (d_pod.Product ?? DB.Products.Find(d_pod.ProductId) ?? new DAL.Product()).ProductName;
                        b_pod.UOMName = (d_pod.UOM ?? DB.UOMs.Find(d_pod.UOMId) ?? new DAL.UOM()).Symbol;
                    }

                }
            }
            catch (Exception ex) { }
            return P;
        }

        public bool SalesReturn_Delete(long pk)
        {
            try
            {
                DAL.SalesReturn d = DB.SalesReturns.Where(x => x.Id == pk).FirstOrDefault();

                if (d != null)
                {
                    BLL.SalesReturn P = new BLL.SalesReturn();
                    d.toCopy<BLL.SalesReturn>(P);
                  //  P.LedgerName = d.Ledger.LedgerName;
                  //  P.TransactionType = d.TransactionType.Type;
                    foreach (var d_pod in d.SalesReturnDetails)
                    {
                        BLL.SalesReturnDetail b_pod = new BLL.SalesReturnDetail();
                        d_pod.toCopy<BLL.SalesReturnDetail>(b_pod);
                        P.SRDetails.Add(b_pod);

                    }
                    DB.SalesReturnDetails.RemoveRange(d.SalesReturnDetails);
                    DB.SalesReturns.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(P, LogDetailType.DELETE);
                }
                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        
        public bool Find_SRRef(string RefNo, BLL.SalesReturn PO)

        {
            DAL.SalesReturn d = DB.SalesReturns.Where(x => x.RefNo == RefNo & x.Id != PO.Id).FirstOrDefault();
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