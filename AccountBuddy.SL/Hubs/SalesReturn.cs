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

                    P.Id = d.Id;
                    LogDetailStore(P, LogDetailType.INSERT);
                }
                else
                {
                    foreach (var d_SRd in d.SalesReturnDetails)
                    {
                        BLL.SalesReturnDetail b_SRd = P.SRDetails.Where(x => x.Id == d_SRd.Id).FirstOrDefault();
                        if (b_SRd == null) d.SalesReturnDetails.Remove(d_SRd);
                    }

                    P.toCopy<DAL.SalesReturn>(d);
                    foreach (var b_SRd in P.SRDetails)
                    {
                        DAL.SalesReturnDetail d_SRd = d.SalesReturnDetails.Where(x => x.Id == b_SRd.Id).FirstOrDefault();
                        if (d_SRd == null)
                        {
                            d_SRd = new DAL.SalesReturnDetail();
                            d.SalesReturnDetails.Add(d_SRd);
                        }
                        b_SRd.toCopy<DAL.SalesReturnDetail>(d_SRd);
                    }
                    LogDetailStore(P, LogDetailType.UPDATE);
                }
                Journal_SaveBySalesReturn(P);
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

                DAL.SalesReturn d = DB.SalesReturns.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.RefNo == SearchText).FirstOrDefault();
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
                    DB.SalesReturnDetails.RemoveRange(d.SalesReturnDetails);
                    DB.SalesReturns.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(SalesReturn_DALtoBLL(d), LogDetailType.DELETE);
                }
                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public BLL.SalesReturn SalesReturn_DALtoBLL(DAL.SalesReturn d)
        {
            BLL.SalesReturn SR = d.toCopy<BLL.SalesReturn>(new BLL.SalesReturn());
            foreach (var d_SRd in d.SalesReturnDetails)
            {
                SR.SRDetails.Add(d_SRd.toCopy<BLL.SalesReturnDetail>(new BLL.SalesReturnDetail()));
            }
            return SR;
        }
        public bool Find_SRRef(string RefNo, BLL.SalesReturn PO)

        {
            DAL.SalesReturn d = DB.SalesReturns.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.RefNo == RefNo & x.Id != PO.Id).FirstOrDefault();
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