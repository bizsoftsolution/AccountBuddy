using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AccountBuddy.Common;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        #region StockOut      

        public bool StockOut_Save(BLL.StockOut P)
        {
            try
            {

                DAL.StockOut d = DB.StockOuts.Where(x => x.Id == P.Id).FirstOrDefault();

                if (d == null)
                {

                    d = new DAL.StockOut();
                    DB.StockOuts.Add(d);


                    P.toCopy<DAL.StockOut>(d);

                    foreach (var b_pod in P.STOutDetails)
                    {
                        DAL.StockOutDetail d_pod = new DAL.StockOutDetail();
                        b_pod.toCopy<DAL.StockOutDetail>(d_pod);
                        d.StockOutDetails.Add(d_pod);
                    }
                    DB.SaveChanges();
                    P.Id = d.Id;

                    LogDetailStore(P, LogDetailType.INSERT);
                }
                else
                {

                    foreach (var d_Pd in d.StockOutDetails)
                    {
                        BLL.StockOutDetail b_Pd = P.STOutDetails.Where(x => x.Id == d_Pd.Id).FirstOrDefault();
                        if (b_Pd == null) d.StockOutDetails.Remove(d_Pd);
                    }

                    P.toCopy<DAL.StockOut>(d);
                    foreach (var b_Pd in P.STOutDetails)
                    {
                        DAL.StockOutDetail d_Pd = d.StockOutDetails.Where(x => x.Id == b_Pd.Id).FirstOrDefault();
                        if (d_Pd == null)
                        {
                            d_Pd = new DAL.StockOutDetail();
                            d.StockOutDetails.Add(d_Pd);
                        }
                        b_Pd.toCopy<DAL.StockOutDetail>(d_Pd);
                    }
                    DB.SaveChanges();
                    LogDetailStore(P, LogDetailType.UPDATE);
                }
                Journal_SaveByStockOut(P);
                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public bool StockOut_SaveBySales(BLL.Sale S)
        {
            try
            {
                var LName = DB.Ledgers.Where(x => x.Id == S.LedgerId).FirstOrDefault().LedgerName;

                if (LName.StartsWith("CM-") || LName.StartsWith("WH-"))
                {

                    DAL.StockOut d = DB.StockOuts.Where(x => x.RefNo == S.RefNo && x.Ledger.AccountGroup.CompanyId == Caller.UnderCompanyId).FirstOrDefault();
                    if (d != null)
                    {
                        DB.StockOutDetails.RemoveRange(d.StockOutDetails);
                        DB.StockOuts.Remove(d);
                        DB.SaveChanges();
                    }


                    d = new DAL.StockOut();

                    d.Date = S.SalesDate;

                    DB.StockOuts.Add(d);
                    var LNameTo = LedgerNameByCompanyId(Caller.CompanyId);
                    S.LedgerId = LedgerIdByCompany(LNameTo, Caller.UnderCompanyId);

                    S.toCopy<DAL.StockOut>(d);


                    foreach (var b_SOd in S.SDetails)
                    {
                        DAL.StockOutDetail d_SOd = new DAL.StockOutDetail();
                        b_SOd.toCopy<DAL.StockOutDetail>(d_SOd);
                        d.StockOutDetails.Add(d_SOd);
                    }
                    DB.SaveChanges();
                    S.Id = d.Id;
                    LogDetailStore(S, LogDetailType.INSERT);

                    return true;
                }


            }
            catch (Exception ex) { }
            return false;
        }

        public bool StockOut_DeleteBySales(BLL.Sale s)
        {
            try
            {
                var LName = DB.Ledgers.Where(x => x.Id == s.LedgerId).FirstOrDefault().LedgerName;

                if (LName.StartsWith("CM-") || LName.StartsWith("WH-"))
                {

                    DAL.Sale d = DB.Sales.Where(x => x.RefNo == s.RefNo && x.Ledger.AccountGroup.CompanyId == Caller.UnderCompanyId).FirstOrDefault();

                    if (d != null)
                    {
                        DB.SalesDetails.RemoveRange(d.SalesDetails);
                        DB.Sales.Remove(d);
                        DB.SaveChanges();
                    }

                    return true;
                }


            }
            catch (Exception ex) { }
            return false;
        }




        public BLL.StockOut StockOut_Find(string SearchText)
        {
            BLL.StockOut P = new BLL.StockOut();
            try
            {

                DAL.StockOut d = DB.StockOuts.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.RefNo == SearchText).FirstOrDefault();
                DB.Entry(d).Reload();
                if (d != null)
                {

                    d.toCopy<BLL.StockOut>(P);
                    P.LedgerName = (d.Ledger ?? DB.Ledgers.Find(d.LedgerId) ?? new DAL.Ledger()).LedgerName;

                    foreach (var d_pod in d.StockOutDetails)
                    {
                        BLL.StockOutDetail b_pod = new BLL.StockOutDetail();
                        d_pod.toCopy<BLL.StockOutDetail>(b_pod);
                        P.STOutDetails.Add(b_pod);
                        b_pod.ProductName = (d_pod.Product ?? DB.Products.Find(d_pod.ProductId) ?? new DAL.Product()).ProductName;
                        b_pod.UOMName = (d_pod.UOM ?? DB.UOMs.Find(d_pod.UOMId) ?? new DAL.UOM()).Symbol;
                    }

                }
            }
            catch (Exception ex) { }
            return P;
        }

        public bool StockOut_Delete(long pk)
        {
            try
            {
                DAL.StockOut d = DB.StockOuts.Where(x => x.Id == pk).FirstOrDefault();

                if (d != null)
                {
                    var P = StockOut_DALtoBLL(d);
                    DB.StockOutDetails.RemoveRange(d.StockOutDetails);
                    DB.StockOuts.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(P, LogDetailType.DELETE);
                    // Journal_DeleteByStockOut(P);
                    return true;
                }

            }
            catch (Exception ex) { }
            return false;
        }
        public BLL.StockOut StockOut_DALtoBLL(DAL.StockOut d)
        {
            BLL.StockOut P = d.toCopy<BLL.StockOut>(new BLL.StockOut());
            foreach (var d_Pd in d.StockOutDetails)
            {
                P.STOutDetails.Add(d_Pd.toCopy<BLL.StockOutDetail>(new BLL.StockOutDetail()));
            }
            return P;
        }
        public bool Find_STOutRef(string RefNo, BLL.StockOut PO)

        {
            DAL.StockOut d = DB.StockOuts.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.RefNo == RefNo & x.Id != PO.Id).FirstOrDefault();
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