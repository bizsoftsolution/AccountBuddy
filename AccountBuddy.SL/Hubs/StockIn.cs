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
        private string StockIn_NewRefNoByCompanyId(int CompanyId)
        {
            DateTime dt = DateTime.Now;
            string Prefix = string.Format("{0}{1:yy}{2:X}", BLL.FormPrefix.StockIn, dt, dt.Month);
            long No = 0;

            var d = Caller.DB.StockIns.Where(x => x.Ledger.AccountGroup.CompanyId == CompanyId && x.RefNo.StartsWith(Prefix))
                                     .OrderByDescending(x => x.RefNo)
                                     .FirstOrDefault();

            if (d != null) No = Convert.ToInt64(d.RefNo.Substring(Prefix.Length), 16);

            return string.Format("{0}{1:X5}", Prefix, No + 1);
        }
        public string StockIn_NewRefNo()
        {
            return StockIn_NewRefNoByCompanyId(Caller.CompanyId);
        }
        public bool StockIn_Save(BLL.StockIn P)
        {
            try
            {

                DAL.StockIn d = Caller.DB.StockIns.Where(x => x.Id == P.Id).FirstOrDefault();

                if (d == null)
                {

                    d = new DAL.StockIn();
                    Caller.DB.StockIns.Add(d);


                    P.toCopy<DAL.StockIn>(d);

                    foreach (var b_pod in P.STInDetails)
                    {
                        DAL.StockInDetail d_pod = new DAL.StockInDetail();
                        b_pod.toCopy<DAL.StockInDetail>(d_pod);
                        d.StockInDetails.Add(d_pod);
                    }
                    Caller.DB.SaveChanges();
                    P.Id = d.Id;

                    LogDetailStore(P, LogDetailType.INSERT);
                }
                else
                {


                    decimal pd = P.STInDetails.Select(X => X.StockInId).FirstOrDefault();
                    Caller.DB.StockInDetails.RemoveRange(d.StockInDetails.Where(x => x.StockInId == pd).ToList());

                    P.toCopy<DAL.StockIn>(d);
                    foreach (var b_Pd in P.STInDetails)
                    {
                        //DAL.StockInDetail d_Pd = d.StockInDetails.Where(x => x.Id == b_Pd.Id).FirstOrDefault();
                       // if (d_Pd == null)
                        //{
                            DAL.StockInDetail d_Pd = new DAL.StockInDetail();
                            d.StockInDetails.Add(d_Pd);
                       // }
                        b_Pd.toCopy<DAL.StockInDetail>(d_Pd);
                    }
                    Caller.DB.SaveChanges();
                    LogDetailStore(P, LogDetailType.UPDATE);
                }
                Clients.Clients(OtherLoginClients).StockIn_RefNoRefresh(StockIn_NewRefNo());
                Journal_SaveByStockIn(d);
                StockOut_SaveByStockIn(d);
                return true;
            }
            catch (Exception ex) { }
            return false;
        }
        #region stock In
        void StockIn_SaveByStockOut(DAL.StockOut S)
        {
            string RefCode = string.Format("{0}{1}", BLL.FormPrefix.StockOut, S.Id);

            DAL.StockIn p = Caller.DB.StockIns.Where(x => x.RefCode == RefCode).FirstOrDefault();

            if (S.Ledger.LedgerName.StartsWith("CM-") || S.Ledger.LedgerName.StartsWith("WH-") || S.Ledger.LedgerName.StartsWith("DL-"))
            {
                var LName = LedgerNameByCompanyId(Caller.CompanyId);
                var CId = CompanyIdByLedgerName(S.Ledger.LedgerName);
                var LId = LedgerIdByCompany(LName, CId);

                if (LId != 0)
                {
                    if (p == null)
                    {
                        p = new DAL.StockIn();
                        p.RefNo = StockIn_NewRefNoByCompanyId(CId);
                        p.RefCode = RefCode;
                        Caller.DB.StockIns.Add(p);
                    }
                    else
                    {
                        Caller.DB.StockInDetails.RemoveRange(p.StockInDetails);
                    }

                    p.Date = S.Date;

                    p.ItemAmount = S.ItemAmount;

                    p.LedgerId = LId;
                    p.Type = "Inwards";
                    foreach (var b_pod in S.StockOutDetails)
                    {
                        DAL.StockInDetail d_pod = new DAL.StockInDetail();
                        b_pod.toCopy<DAL.StockInDetail>(d_pod);
                        p.StockInDetails.Add(d_pod);
                    }
                    Caller.DB.SaveChanges();
                    Journal_SaveByStockIn(p);
                }
            }


        }

        public bool StockIn_DeleteByStockOut(DAL.StockOut s)

        {
            try
            {
                string RefCode = string.Format("{0}{1}", BLL.FormPrefix.StockOut, s.Id);
                DAL.StockOut d = Caller.DB.StockOuts.Where(x => x.RefCode == RefCode).FirstOrDefault();
                if (d != null)
                {
                    StockOut_Delete(d.Id);
                }


            }
            catch (Exception ex) { }
            return false;
        }

        #endregion
        public BLL.StockIn StockIn_Find(string SearchText)
        {
            BLL.StockIn P = new BLL.StockIn();
            try
            {

                DAL.StockIn d = Caller.DB.StockIns.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.RefNo == SearchText).FirstOrDefault();
                Caller.DB.Entry(d).Reload();
                if (d != null)
                {

                    d.toCopy<BLL.StockIn>(P);
                    P.LedgerName = (d.Ledger ?? Caller.DB.Ledgers.Find(d.LedgerId) ?? new DAL.Ledger()).LedgerName;

                    foreach (var d_pod in d.StockInDetails)
                    {
                        BLL.StockInDetail b_pod = new BLL.StockInDetail();
                        d_pod.toCopy<BLL.StockInDetail>(b_pod);
                        P.STInDetails.Add(b_pod);
                        b_pod.ProductName = (d_pod.Product ?? Caller.DB.Products.Find(d_pod.ProductId) ?? new DAL.Product()).ProductName;
                        b_pod.UOMName = (d_pod.UOM ?? Caller.DB.UOMs.Find(d_pod.UOMId) ?? new DAL.UOM()).Symbol;
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

                DAL.StockIn d = Caller.DB.StockIns.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.Id == Id).FirstOrDefault();
                Caller.DB.Entry(d).Reload();
                if (d != null)
                {

                    d.toCopy<BLL.StockIn>(P);
                    P.LedgerName = (d.Ledger ?? Caller.DB.Ledgers.Find(d.LedgerId) ?? new DAL.Ledger()).LedgerName;

                    foreach (var d_pod in d.StockInDetails)
                    {
                        BLL.StockInDetail b_pod = new BLL.StockInDetail();
                        d_pod.toCopy<BLL.StockInDetail>(b_pod);
                        P.STInDetails.Add(b_pod);
                        b_pod.ProductName = (d_pod.Product ?? Caller.DB.Products.Find(d_pod.ProductId) ?? new DAL.Product()).ProductName;
                        b_pod.UOMName = (d_pod.UOM ?? Caller.DB.UOMs.Find(d_pod.UOMId) ?? new DAL.UOM()).Symbol;
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
                DAL.StockIn d = Caller.DB.StockIns.Where(x => x.Id == pk).FirstOrDefault();

                if (d != null)
                {
                    var P = StockIn_DALtoBLL(d);
                    Caller.DB.StockInDetails.RemoveRange(d.StockInDetails);
                    Caller.DB.StockIns.Remove(d);
                    Caller.DB.SaveChanges();
                    LogDetailStore(P, LogDetailType.DELETE);
                    Journal_DeleteByStockIn(P);
                    StockOut_DeleteByStockIn(d);
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
            DAL.StockIn d = Caller.DB.StockIns.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.RefNo == RefNo & x.Id != PO.Id).FirstOrDefault();
            if (d == null)
            {
                return false;
            }
            else
            {
                return true;
            }

        }
        public List<BLL.StockIn> StockIn_List(int? LedgerId, DateTime dtFrom, DateTime dtTo, string BillNo, decimal amtFrom, decimal amtTo)
        {
            List<BLL.StockIn> lstStockIn = new List<BLL.StockIn>();
            Caller.DB = new DAL.DBFMCGEntities();
            BLL.StockIn rp = new BLL.StockIn();
            try
            {
                foreach (var l in Caller.DB.StockIns.
                      Where(x => x.Date >= dtFrom && x.Date <= dtTo
                      && (x.LedgerId == LedgerId || LedgerId == null)
                      && (BillNo == "" || x.RefNo == BillNo)
                      && (x.ItemAmount >= amtFrom && x.ItemAmount <= amtTo) &&
                      x.Ledger.AccountGroup.CompanyId == Caller.CompanyId).ToList())
                {
                    rp = new BLL.StockIn();
                    rp.ItemAmount = l.ItemAmount;
                    rp.Date = l.Date;
                    rp.RefNo = l.RefNo;

                    rp.Id = l.Id;
                    rp.LedgerId = l.LedgerId;
                    rp.LedgerName = string.Format("{0}-{1}", l.Ledger.AccountGroup.GroupCode, l.Ledger.LedgerName);

                    rp.RefCode = l.RefCode;
                    rp.RefNo = l.RefNo;

                    lstStockIn.Add(rp);
                    lstStockIn = lstStockIn.OrderBy(x => x.Date).ToList();
                }

            }
            catch (Exception ex) { }
            return lstStockIn;
        }


        #endregion
    }
}