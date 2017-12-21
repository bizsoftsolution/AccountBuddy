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
        private string StockOut_NewRefNoByCompanyId(int CompanyId)
        {
            DateTime dt = DateTime.Now;
            string Prefix = string.Format("{0}{1:yy}{2:X}", BLL.FormPrefix.StockOut, dt, dt.Month);
            long No = 0;

            var d = DB.StockOuts.Where(x => x.Ledger.AccountGroup.CompanyId == CompanyId && x.RefNo.StartsWith(Prefix))
                                     .OrderByDescending(x => x.RefNo)
                                     .FirstOrDefault();

            if (d != null) No = Convert.ToInt64(d.RefNo.Substring(Prefix.Length), 16);

            return string.Format("{0}{1:X5}", Prefix, No + 1);
        }
        public string StockOut_NewRefNo()
        {
            return StockOut_NewRefNoByCompanyId(Caller.CompanyId);
        }
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

                    //foreach (var d_Pd in d.StockOutDetails)
                    //{
                    //    BLL.StockOutDetail b_Pd = P.STOutDetails.Where(x => x.Id == d_Pd.Id).FirstOrDefault();
                    //    if (b_Pd == null) d.StockOutDetails.Remove(d_Pd);
                    //}

                    decimal rd = P.STOutDetails.Select(X => X.StockOutId).FirstOrDefault();
                    DB.StockOutDetails.RemoveRange(d.StockOutDetails.Where(x => x.StockOutId == rd).ToList());


                    P.toCopy<DAL.StockOut>(d);
                    foreach (var b_Pd in P.STOutDetails)
                    {
                        //DAL.StockOutDetail d_Pd = d.StockOutDetails.Where(x => x.Id == b_Pd.Id).FirstOrDefault();
                        // if (d_Pd == null)
                        // {
                        DAL.StockOutDetail d_Pd = new DAL.StockOutDetail();
                            d.StockOutDetails.Add(d_Pd);
                      //  }
                        b_Pd.toCopy<DAL.StockOutDetail>(d_Pd);
                    }
                    DB.SaveChanges();
                    LogDetailStore(P, LogDetailType.UPDATE);

                }
                if (OtherClientsOnGroup.Count > 0) Clients.Clients(OtherClientsOnGroup).StockOut_RefNoRefresh(StockOut_NewRefNo());
                Journal_SaveByStockOut(d);
                StockIn_SaveByStockOut(d);
                return true;
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return false;
        }

        #region Stock In
        void StockOut_SaveByStockIn(DAL.StockIn P)
        {
            try
            {
                string RefCode = string.Format("{0}{1}", BLL.FormPrefix.StockIn, P.Id);
                if (P.Ledger == null)
                {
                    P.Ledger = DB.Ledgers.Where(x => x.Id == P.LedgerId).FirstOrDefault();
                }
                DAL.StockOut s = DB.StockOuts.Where(x => x.RefCode == RefCode).FirstOrDefault();
                if (P.Ledger.LedgerName.StartsWith("CM-") || P.Ledger.LedgerName.StartsWith("WH-") || P.Ledger.LedgerName.StartsWith("DL-"))
                {
                    var LName = LedgerNameByCompanyId(Caller.CompanyId);
                    var CId = CompanyIdByLedgerName(P.Ledger.LedgerName);
                    var LId = LedgerIdByCompany(LName, CId);

                    if (LId != 0)
                    {
                        if (s == null)
                        {
                            s = new DAL.StockOut();
                            s.RefNo = StockOut_NewRefNoByCompanyId(CId);
                            s.RefCode = RefCode;
                            DB.StockOuts.Add(s);
                        }
                        else
                        {
                            DB.StockOutDetails.RemoveRange(s.StockOutDetails);
                        }

                        s.Date = P.Date;
                        s.ItemAmount = P.ItemAmount;
                        s.LedgerId = LId;
                        s.Type = "Outwards";
                        foreach (var b_pod in P.StockInDetails)
                        {
                            DAL.StockOutDetail d_pod = new DAL.StockOutDetail();
                            b_pod.toCopy<DAL.StockOutDetail>(d_pod);
                            s.StockOutDetails.Add(d_pod);
                        }
                        DB.SaveChanges();
                        Journal_SaveByStockOut(s);
                    }
                }
            }
            catch(Exception ex)
            {
                Common.AppLib.WriteLog(ex);
            }
}
        public bool StockOut_DeleteByStockIn(DAL.StockIn s)
        {
            try
            {
                string RefCode = string.Format("{0}{1}", BLL.FormPrefix.StockIn, s.Id);
                DAL.StockOut d = DB.StockOuts.Where(x => x.RefCode == RefCode).FirstOrDefault();
                if (d != null)
                {
                    StockOut_Delete(d.Id);
                }
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return false;
        }
        #endregion


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
                    int i = 0;
                    foreach (var d_pod in d.StockOutDetails)
                    {
                        BLL.StockOutDetail b_pod = new BLL.StockOutDetail();
                        d_pod.toCopy<BLL.StockOutDetail>(b_pod);
                        P.STOutDetails.Add(b_pod);
                        b_pod.SNo = ++i;
                        b_pod.ProductName = (d_pod.Product ?? DB.Products.Find(d_pod.ProductId) ?? new DAL.Product()).ProductName;
                        b_pod.UOMName = (d_pod.UOM ?? DB.UOMs.Find(d_pod.UOMId) ?? new DAL.UOM()).Symbol;
                    }

                }
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return P;
        }
        public BLL.StockOut StockOut_FindById(int ID)
        {
            BLL.StockOut P = new BLL.StockOut();
            try
            {

                DAL.StockOut d = DB.StockOuts.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.Id == ID).FirstOrDefault();
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
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
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
                    Journal_DeleteByStockOut(P);
                    StockIn_DeleteByStockOut(d);
                    return true;
                }

            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
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


        public List<BLL.StockOut> StockOut_List(int? LedgerId, DateTime dtFrom, DateTime dtTo, string BillNo, decimal amtFrom, decimal amtTo)
        {
            List<BLL.StockOut> lstStockOut = new List<BLL.StockOut>();
            
            BLL.StockOut rp = new BLL.StockOut();
            try
            {
                foreach (var l in DB.StockOuts.
                      Where(x => x.Date >= dtFrom && x.Date <= dtTo
                      && (x.LedgerId == LedgerId || LedgerId == null)
                      && (BillNo == "" || x.RefNo == BillNo)
                      && (x.ItemAmount >= amtFrom && x.ItemAmount <= amtTo) &&
                      x.Ledger.AccountGroup.CompanyId == Caller.CompanyId).ToList())
                {
                    rp = new BLL.StockOut();
                    rp.ItemAmount = l.ItemAmount;
                  
                    rp.RefNo = l.RefNo;

                    rp.Id = l.Id;
                    rp.LedgerId = l.LedgerId;
                    rp.LedgerName = string.Format("{0}-{1}", l.Ledger.AccountGroup.GroupCode, l.Ledger.LedgerName);

                    rp.RefCode = l.RefCode;
                    rp.RefNo = l.RefNo;

                    lstStockOut.Add(rp);
                    lstStockOut = lstStockOut.OrderBy(x => x.Date).ToList();
                }

            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return lstStockOut;
        }

        #endregion
    }
}