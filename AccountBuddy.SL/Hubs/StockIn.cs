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
                StockOut_SaveByStockIn(P);
                return true;
            }
            catch (Exception ex) { }
            return false;
        }
        void StockIn_SaveByStockOut(BLL.StockOut SOut)
        {
            var refNo = string.Format("SIN-{0}", SOut.Id);

            DAL.StockIn p = DB.StockIns.Where(x => x.RefNo == refNo).FirstOrDefault();
            if (p != null)
            {
                DB.StockInDetails.RemoveRange(p.StockInDetails);
                DB.StockIns.Remove(p);
                DB.SaveChanges();
            }
            var pd = SOut.STOutDetails.FirstOrDefault();
            var ld = DB.Ledgers.Where(x => x.Id == SOut.LedgerId).FirstOrDefault();

            if (ld.LedgerName.StartsWith("CM-") || ld.LedgerName.StartsWith("WH-") || ld.LedgerName.StartsWith("DL-"))
            {
                var LName = LedgerNameByCompanyId(Caller.CompanyId);

                var CId = CompanyIdByLedgerName(ld.LedgerName);

                p = new DAL.StockIn();
                p.RefNo = refNo;
                p.Date = SOut.Date;
              
                p.ItemAmount = SOut.ItemAmount;
             
                p.LedgerId = LedgerIdByCompany(LName, CId);
                p.Type = "Inward";
                if (CId != 0)
                {
                    foreach (var b_pod in SOut.STOutDetails)
                    {
                        DAL.StockInDetail d_pod = new DAL.StockInDetail();
                        b_pod.toCopy<DAL.StockInDetail>(d_pod);
                        p.StockInDetails.Add(d_pod);
                    }
                    DB.StockIns.Add(p);
                    DB.SaveChanges();
                    var sin = StockIn_DALtoBLL(p);
                    Journal_SaveByStockIn(sin);
                  
                }
            }
           

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
        public BLL.StockIn StockIn_FindById(int Id)
        {
            BLL.StockIn P = new BLL.StockIn();
            try
            {

                DAL.StockIn d = DB.StockIns.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.Id == Id).FirstOrDefault();
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