using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AccountBuddy.Common;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        #region Stock In Process     
        public string StockInProcess_NewRefNo()
        {
            return StockInProcess_NewRefNoByCompanyId(Caller.CompanyId);
        }
        public string StockInProcess_NewRefNoByCompanyId(int CompanyId)
        {
            DateTime dt = DateTime.Now;
            string Prefix = string.Format("{0}{1:yy}{2:X}", BLL.FormPrefix.StockInProcess, dt, dt.Month);
            long No = 0;

            var d = DB.StockInProcesses.Where(x => x.Staff.Ledger.AccountGroup.CompanyId == CompanyId && x.RefNo.StartsWith(Prefix))
                                     .OrderByDescending(x => x.RefNo)
                                     .FirstOrDefault();

            if (d != null) No = Convert.ToInt64(d.RefNo.Substring(Prefix.Length), 16);

            return string.Format("{0}{1:X5}", Prefix, No + 1);
        }
        public bool StockInProcess_Save(BLL.StockInProcess SO)
        {
            try
            {

                DAL.StockInProcess d = DB.StockInProcesses.Where(x => x.Id == SO.Id).FirstOrDefault();

                if (d == null)
                {

                    d = new DAL.StockInProcess();
                    DB.StockInProcesses.Add(d);

                    SO.toCopy<DAL.StockInProcess>(d);

                    foreach (var b_pod in SO.STPDetails)
                    {
                        DAL.StockInProcessDetail d_pod = new DAL.StockInProcessDetail();
                        b_pod.toCopy<DAL.StockInProcessDetail>(d_pod);
                        d.StockInProcessDetails.Add(d_pod);
                    }
                    DB.SaveChanges();
                    SO.Id = d.Id;
                    LogDetailStore(SO, LogDetailType.INSERT);
                }
                else
                {
                    //foreach (var d_SOd in d.StockInProcessDetails)
                    //{
                    //    BLL.StockInProcessDetail b_SOd = SO.STPDetails.Where(x => x.Id == d_SOd.Id).FirstOrDefault();
                    //    if (b_SOd == null) d.StockInProcessDetails.Remove(d_SOd);
                    //}
                    decimal rd = SO.STPDetails.Select(X => X.SPId).FirstOrDefault().Value;
                    DB.StockInProcessDetails.RemoveRange(d.StockInProcessDetails.Where(x => x.SPId == rd).ToList());

                    SO.toCopy<DAL.StockInProcess>(d);
                    foreach (var b_SOd in SO.STPDetails)
                    {
                        //DAL.StockInProcessDetail d_SOd = d.StockInProcessDetails.Where(x => x.Id == b_SOd.Id).FirstOrDefault();
                        //if (d_SOd == null)
                        //{
                        DAL.StockInProcessDetail d_SOd = new DAL.StockInProcessDetail();
                        d.StockInProcessDetails.Add(d_SOd);
                        // }
                        b_SOd.toCopy<DAL.StockInProcessDetail>(d_SOd);
                    }
                    DB.SaveChanges();
                    LogDetailStore(SO, LogDetailType.UPDATE);

                }
                Clients.Clients(OtherLoginClients).StockInProcess_RefNoRefresh(StockInProcess_NewRefNo());
                Journal_SaveByStockInProcess(d);
                return true;

            }

            catch (Exception ex) { }
            return false;
        }

        public BLL.StockInProcess StockInProcess_Find(string SearchText)
        {
            BLL.StockInProcess SO = new BLL.StockInProcess();
            try
            {

                DAL.StockInProcess d = DB.StockInProcesses.Where(x => x.Staff.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.RefNo == SearchText).FirstOrDefault();
                DB.Entry(d).Reload();
                if (d != null)
                {

                    d.toCopy<BLL.StockInProcess>(SO);
                    SO.StaffName = (d.Staff ?? DB.Staffs.Find(d.StaffId) ?? new DAL.Staff()).Ledger.LedgerName;
                    int i = 0;

                    foreach (var d_pod in d.StockInProcessDetails)
                    {
                        BLL.StockInProcessDetail b_pod = new BLL.StockInProcessDetail();
                        d_pod.toCopy<BLL.StockInProcessDetail>(b_pod);
                        SO.STPDetails.Add(b_pod);
                        b_pod.SNo = i++;
                        b_pod.ProductName = (d_pod.Product ?? DB.Products.Find(d_pod.ProductId) ?? new DAL.Product()).ProductName;
                        b_pod.UOMName = (d_pod.UOM ?? DB.UOMs.Find(d_pod.UOMId) ?? new DAL.UOM()).Symbol;
                        //SO.Status = d.StockInProcessDetails.FirstOrDefault().JobOrderReceivedDetails.Count() > 0 ? "Received" : "Pending";
                    }

                }
            }
            catch (Exception ex) { }
            return SO;
        }

        public bool StockInProcess_Delete(long pk)
        {
            try
            {
                DAL.StockInProcess d = DB.StockInProcesses.Where(x => x.Id == pk).FirstOrDefault();

                if (d != null)
                {
                    var s = StockInProcess_DALtoBLL(d);
                    DB.StockInProcessDetails.RemoveRange(d.StockInProcessDetails);
                    DB.StockInProcesses.Remove(d);
                    DB.SaveChanges();

                    LogDetailStore(s, LogDetailType.DELETE);
                    Journal_DeleteByStockInProcess(s);
                    Clients.Clients(OtherLoginClients).StockInProcess_RefNoRefresh(StockInProcess_NewRefNo());

                }
                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public List<BLL.StockInProcess> StockInProcess_JOPendingList()
        {
            return DB.StockInProcesses.Where(x => x.Staff.Ledger.AccountGroup.CompanyId == Caller.CompanyId)
                                     .ToList()
                                     .Select(x => StockInProcess_DALtoBLL(x))
                                     .ToList();
        }
        public BLL.StockInProcess StockInProcess_DALtoBLL(DAL.StockInProcess d)
        {
            BLL.StockInProcess SO = d.toCopy<BLL.StockInProcess>(new BLL.StockInProcess());
            foreach (var d_SOd in d.StockInProcessDetails)
            {
                SO.STPDetails.Add(d_SOd.toCopy<BLL.StockInProcessDetail>(new BLL.StockInProcessDetail()));
            }
            //SO.Status = d.StockInProcessDetails.FirstOrDefault().SalesDetails.Count() > 0 ? "Sold" : "Pending";
            return SO;
        }
        public bool Find_STPRef(string RefNo, BLL.StockInProcess JO)
        {
            DAL.StockInProcess d1 = DB.StockInProcesses.Where(x => x.Staff.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.RefNo == RefNo && x.Id != JO.Id).FirstOrDefault();

            if (d1 == null)
            {
                return false;
            }
            else
            {
                return true;
            }

        }
        public BLL.StockInProcess StockInProcess_FindById(int ID)
        {
            BLL.StockInProcess P = new BLL.StockInProcess();
            try
            {

                DAL.StockInProcess d = DB.StockInProcesses.Where(x => x.Staff.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.Id == ID).FirstOrDefault();
                DB.Entry(d).Reload();
                if (d != null)
                {

                    d.toCopy<BLL.StockInProcess>(P);
                    P.StaffName = (d.Staff ?? DB.Staffs.Find(d.StaffId) ?? new DAL.Staff()).Ledger.LedgerName;
                    foreach (var d_pod in d.StockInProcessDetails)
                    {
                        BLL.StockInProcessDetail b_pod = new BLL.StockInProcessDetail();
                        d_pod.toCopy<BLL.StockInProcessDetail>(b_pod);
                        P.STPDetails.Add(b_pod);
                        b_pod.ProductName = (d_pod.Product ?? DB.Products.Find(d_pod.ProductId) ?? new DAL.Product()).ProductName;
                        b_pod.UOMName = (d_pod.UOM ?? DB.UOMs.Find(d_pod.UOMId) ?? new DAL.UOM()).Symbol;
                    }

                }
            }
            catch (Exception ex) { }
            return P;
        }
        public List<BLL.StockInProcess> StockInProcess_List(int? LedgerId, DateTime dtFrom, DateTime dtTo, string BillNo, decimal amtFrom, decimal amtTo)
        {
            List<BLL.StockInProcess> lstStockIn = new List<BLL.StockInProcess>();
            
            BLL.StockInProcess rp = new BLL.StockInProcess();
            try
            {
                foreach (var l in DB.StockInProcesses.
                      Where(x => x.SPDate >= dtFrom && x.SPDate <= dtTo
                      && (x.StaffId == LedgerId || LedgerId == null)
                      && (BillNo == "" || x.RefNo == BillNo)
                      && (x.ItemAmount >= amtFrom && x.ItemAmount <= amtTo) &&
                      x.Staff.Ledger.AccountGroup.CompanyId == Caller.CompanyId).ToList())
                {
                    rp = new BLL.StockInProcess();
                    rp.ItemAmount = l.ItemAmount;
                    rp.SPDate = l.SPDate;
                    rp.RefNo = l.RefNo;

                    rp.Id = l.Id;
                    rp.StaffId = l.StaffId;
                    rp.StaffName = string.Format("{0}-{1}", l.Staff.Ledger.AccountGroup.GroupCode, l.Staff.Ledger.LedgerName);

                    rp.RefCode = l.RefCode;
                    rp.RefNo = l.RefNo;

                    lstStockIn.Add(rp);
                    lstStockIn = lstStockIn.OrderBy(x => x.SPDate).ToList();
                }

            }
            catch (Exception ex) { }
            return lstStockIn;
        }

    }

    #endregion
}