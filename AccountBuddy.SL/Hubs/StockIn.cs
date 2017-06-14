using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AccountBuddy.Common;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        #region StockIn      

        public bool StockIn_Save(BLL.StockIn P)
        {
            try
            {

                DAL.StockIn d = DB.StockIns.Where(x => x.Id == P.Id).FirstOrDefault();

                if (d == null)
                {

                    d = new DAL.StockIn();
                    DB.StockIns.Add(d);


                    P.toCopy<DAL.StockIn>(d);

                    foreach (var b_pod in P.STInDetails)
                    {
                        DAL.StockInDetail d_pod = new DAL.StockInDetail();
                        b_pod.toCopy<DAL.StockInDetail>(d_pod);
                        d.StockInDetails.Add(d_pod);
                    }
                    DB.SaveChanges();
                    P.Id = d.Id;

                    LogDetailStore(P, LogDetailType.INSERT);
                }
                else
                {

                    foreach (var d_Pd in d.StockInDetails)
                    {
                        BLL.StockInDetail b_Pd = P.STInDetails.Where(x => x.Id == d_Pd.Id).FirstOrDefault();
                        if (b_Pd == null) d.StockInDetails.Remove(d_Pd);
                    }

                    P.toCopy<DAL.StockIn>(d);
                    foreach (var b_Pd in P.STInDetails)
                    {
                        DAL.StockInDetail d_Pd = d.StockInDetails.Where(x => x.Id == b_Pd.Id).FirstOrDefault();
                        if (d_Pd == null)
                        {
                            d_Pd = new DAL.StockInDetail();
                            d.StockInDetails.Add(d_Pd);
                        }
                        b_Pd.toCopy<DAL.StockInDetail>(d_Pd);
                    }
                    DB.SaveChanges();
                    LogDetailStore(P, LogDetailType.UPDATE);
                }
                Journal_SaveByStockIn(P);
                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public bool StockIn_SaveBySales(BLL.Sale S)
        {
            try
            {
                var LName = DB.Ledgers.Where(x => x.Id == S.LedgerId).FirstOrDefault().LedgerName;

                if (LName.StartsWith("CM-") || LName.StartsWith("WH-"))
                {

                    DAL.StockIn d = DB.StockIns.Where(x => x.RefNo == S.RefNo && x.Ledger.AccountGroup.CompanyId == Caller.UnderCompanyId).FirstOrDefault();
                    if (d != null)
                    {
                        DB.StockInDetails.RemoveRange(d.StockInDetails);
                        DB.StockIns.Remove(d);
                        DB.SaveChanges();
                    }


                    d = new DAL.StockIn();

                    d.Date = S.SalesDate;

                    DB.StockIns.Add(d);
                    var LNameTo = LedgerNameByCompanyId(Caller.CompanyId);
                    S.LedgerId = LedgerIdByCompany(LNameTo, Caller.UnderCompanyId);

                    S.toCopy<DAL.StockIn>(d);


                    foreach (var b_SOd in S.SDetails)
                    {
                        DAL.StockInDetail d_SOd = new DAL.StockInDetail();
                        b_SOd.toCopy<DAL.StockInDetail>(d_SOd);
                        d.StockInDetails.Add(d_SOd);
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

        public bool StockIn_DeleteBySales(BLL.Sale s)
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




        public BLL.StockIn StockIn_Find(string SearchText)
        {
            BLL.StockIn P = new BLL.StockIn();
            try
            {

                DAL.StockIn d = DB.StockIns.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.RefNo == SearchText).FirstOrDefault();
                DB.Entry(d).Reload();
                if (d != null)
                {

                    d.toCopy<BLL.StockIn>(P);
                    P.LedgerName = (d.Ledger ?? DB.Ledgers.Find(d.LedgerId) ?? new DAL.Ledger()).LedgerName;

                    foreach (var d_pod in d.StockInDetails)
                    {
                        BLL.StockInDetail b_pod = new BLL.StockInDetail();
                        d_pod.toCopy<BLL.StockInDetail>(b_pod);
                        P.STInDetails.Add(b_pod);
                        b_pod.ProductName = (d_pod.Product ?? DB.Products.Find(d_pod.ProductId) ?? new DAL.Product()).ProductName;
                        b_pod.UOMName = (d_pod.UOM ?? DB.UOMs.Find(d_pod.UOMId) ?? new DAL.UOM()).Symbol;
                    }

                }
            }
            catch (Exception ex) { }
            return P;
        }

        public bool StockIn_Delete(long pk)
        {
            try
            {
                DAL.StockIn d = DB.StockIns.Where(x => x.Id == pk).FirstOrDefault();

                if (d != null)
                {
                    var P = StockIn_DALtoBLL(d);
                    DB.StockInDetails.RemoveRange(d.StockInDetails);
                    DB.StockIns.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(P, LogDetailType.DELETE);
                    // Journal_DeleteByStockIn(P);
                    return true;
                }

            }
            catch (Exception ex) { }
            return false;
        }
        public BLL.StockIn StockIn_DALtoBLL(DAL.StockIn d)
        {
            BLL.StockIn P = d.toCopy<BLL.StockIn>(new BLL.StockIn());
            foreach (var d_Pd in d.StockInDetails)
            {
                P.STInDetails.Add(d_Pd.toCopy<BLL.StockInDetail>(new BLL.StockInDetail()));
            }
            return P;
        }
        public bool Find_STInRef(string RefNo, BLL.StockIn PO)

        {
            DAL.StockIn d = DB.StockIns.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.RefNo == RefNo & x.Id != PO.Id).FirstOrDefault();
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