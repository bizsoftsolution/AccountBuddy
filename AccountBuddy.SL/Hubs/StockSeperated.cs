using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AccountBuddy.Common;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        #region Stock Seperated 
        public string StockSeperated_NewRefNo()
        {
            return StockSeperated_NewRefNoByCompanyId(Caller.CompanyId);
        }
        public string StockSeperated_NewRefNoByCompanyId(int CompanyId)
        {
            DateTime dt = DateTime.Now;
            string Prefix = string.Format("{0}{1:yy}{2:X}", BLL.FormPrefix.StockSeparated, dt, dt.Month);
            long No = 0;

            var d = DB.StockSeparateds.Where(x => x.Staff.Ledger.AccountGroup.CompanyId == CompanyId && x.RefNo.StartsWith(Prefix))
                                     .OrderByDescending(x => x.RefNo)
                                     .FirstOrDefault();

            if (d != null) No = Convert.ToInt64(d.RefNo.Substring(Prefix.Length), 16);

            return string.Format("{0}{1:X5}", Prefix, No + 1);
        }
        public bool StockSeperated_Save(BLL.StockSeperated SO)
        {
            try
            {

                DAL.StockSeparated d = DB.StockSeparateds.Where(x => x.Id == SO.Id).FirstOrDefault();

                if (d == null)
                {

                    d = new DAL.StockSeparated();
                    DB.StockSeparateds.Add(d);

                    SO.ToMap(d);

                    foreach (var b_pod in SO.SSDetails)
                    {
                        DAL.StockSeperatedDetail d_pod = new DAL.StockSeperatedDetail();
                        b_pod.ToMap(d_pod);
                        d.StockSeperatedDetails.Add(d_pod);
                    }
                    DB.SaveChanges();
                    SO.Id = d.Id;
                    LogDetailStore(SO, LogDetailType.INSERT);
                }
                else
                {
                    //foreach (var d_SOd in d.StockSeperatedDetails)
                    //{
                    //    BLL.StockSeperatedDetail b_SOd = SO.SSDetails.Where(x => x.Id == d_SOd.Id).FirstOrDefault();
                    //    if (b_SOd == null) d.StockSeperatedDetails.Remove(d_SOd);
                    //}

                    decimal rd = SO.SSDetails.Select(X => X.SSId).FirstOrDefault().Value;
                    DB.StockSeperatedDetails.RemoveRange(d.StockSeperatedDetails.Where(x => x.SSId == rd).ToList());


                    SO.ToMap(d);
                    foreach (var b_SOd in SO.SSDetails)
                    {
                        //DAL.StockSeperatedDetail d_SOd = d.StockSeperatedDetails.Where(x => x.Id == b_SOd.Id).FirstOrDefault();
                        //if (d_SOd == null)
                        //{
                        DAL.StockSeperatedDetail d_SOd = new DAL.StockSeperatedDetail();
                        d.StockSeperatedDetails.Add(d_SOd);
                        //}
                        b_SOd.ToMap(d_SOd);
                    }
                    LogDetailStore(SO, LogDetailType.UPDATE);

                }
                if (OtherClientsOnGroup.Count > 0) Clients.Clients(OtherClientsOnGroup).StockSeperated_RefNoRefresh(StockSeperated_NewRefNo());
                Journal_SaveByStockSeparated(d);
                return true;

            }

            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return false;
        }

        public BLL.StockSeperated StockSeperated_Find(string SearchText)
        {
            BLL.StockSeperated SO = new BLL.StockSeperated();
            try
            {

                DAL.StockSeparated d = DB.StockSeparateds.Where(x => x.Staff.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.RefNo == SearchText).FirstOrDefault();
                DB.Entry(d).Reload();
                if (d != null)
                {

                    d.ToMap(SO);
                    SO.StaffName = (d.Staff ?? DB.Staffs.Find(d.StaffId) ?? new DAL.Staff()).Ledger.LedgerName;
                    int i = 0;
                    foreach (var d_pod in d.StockSeperatedDetails)
                    {
                        BLL.StockSeperatedDetail b_pod = new BLL.StockSeperatedDetail();
                        d_pod.ToMap(b_pod);
                        SO.SSDetails.Add(b_pod);
                        b_pod.SNo = ++i;
                        b_pod.ProductName = (d_pod.Product ?? DB.Products.Find(d_pod.ProductId) ?? new DAL.Product()).ProductName;
                        b_pod.UOMName = (d_pod.UOM ?? DB.UOMs.Find(d_pod.UOMId) ?? new DAL.UOM()).Symbol;
                        //  SO.Status = d.StockSeperatedDetails.FirstOrDefault()..Count() > 0 ? "Sold" : "Pending";
                    }

                }
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return SO;
        }

        public bool StockSeperated_Delete(long pk)
        {
            try
            {
                DAL.StockSeparated d = DB.StockSeparateds.Where(x => x.Id == pk).FirstOrDefault();

                if (d != null)
                {
                    var s = StockSeperated_DALtoBLL(d);
                    DB.StockSeperatedDetails.RemoveRange(d.StockSeperatedDetails);
                    DB.StockSeparateds.Remove(d);
                    DB.SaveChanges();

                    LogDetailStore(s, LogDetailType.DELETE);
                    Journal_DeleteByStockSeparated(s);
                }
                return true;
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return false;
        }

        public List<BLL.StockSeperated> StockSeperated_JRPendingList()
        {
            return DB.StockSeparateds.Where(x => x.Staff.Ledger.AccountGroup.CompanyId == Caller.CompanyId)
                                     .ToList()
                                     .Select(x => StockSeperated_DALtoBLL(x))
                                     .ToList();
        }
        public BLL.StockSeperated StockSeperated_DALtoBLL(DAL.StockSeparated d)
        {
            BLL.StockSeperated SO = d.ToMap(new BLL.StockSeperated());
            foreach (var d_SOd in d.StockSeperatedDetails)
            {
                SO.SSDetails.Add(d_SOd.ToMap(new BLL.StockSeperatedDetail()));
            }
            //SO.Status = d.StockSeperatedDetails.FirstOrDefault().SalesDetails.Count() > 0 ? "Sold" : "Pending";
            return SO;
        }
        public bool Find_SSRef(string RefNo, BLL.StockSeperated JO)
        {
            DAL.StockSeparated d1 = DB.StockSeparateds.Where(x => x.Staff.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.RefNo == RefNo & x.Id != JO.Id).FirstOrDefault();

            if (d1 == null)
            {
                return false;
            }
            else
            {
                return true;
            }

        }
        public BLL.StockSeperated StockSeperated_FindById(int ID)
        {
            BLL.StockSeperated P = new BLL.StockSeperated();
            try
            {

                DAL.StockSeparated d = DB.StockSeparateds.Where(x => x.Staff.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.Id == ID).FirstOrDefault();
                DB.Entry(d).Reload();
                if (d != null)
                {

                    d.ToMap(P);
                    P.StaffName = (d.Staff ?? DB.Staffs.Find(d.StaffId) ?? new DAL.Staff()).Ledger.LedgerName;
                    foreach (var d_pod in d.StockSeperatedDetails)
                    {
                        BLL.StockSeperatedDetail b_pod = new BLL.StockSeperatedDetail();
                        d_pod.ToMap(b_pod);
                        P.SSDetails.Add(b_pod);
                        b_pod.ProductName = (d_pod.Product ?? DB.Products.Find(d_pod.ProductId) ?? new DAL.Product()).ProductName;
                        b_pod.UOMName = (d_pod.UOM ?? DB.UOMs.Find(d_pod.UOMId) ?? new DAL.UOM()).Symbol;
                    }

                }
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return P;
        }

        public List<BLL.StockSeperated> StockSeperated_List(int? LedgerId,  DateTime dtFrom, DateTime dtTo, string BillNo, decimal amtFrom, decimal amtTo)
        {
            List<BLL.StockSeperated> lstStockSeperated = new List<BLL.StockSeperated>();
            
            BLL.StockSeperated rp = new BLL.StockSeperated();
            try
            {
                foreach (var l in DB.StockSeparateds.
                      Where(x => x.Date >= dtFrom && x.Date <= dtTo
                      && (x.StaffId == LedgerId || LedgerId == null)
                      && (BillNo == "" || x.RefNo == BillNo)
                      && (x.TotalAmount >= amtFrom && x.TotalAmount <= amtTo) &&
                      x.Staff.Ledger.AccountGroup.CompanyId == Caller.CompanyId).ToList())
                {
                    rp = new BLL.StockSeperated();
                    rp.ItemAmount = l.ItemAmount;

                    rp.RefNo = l.RefNo;

                    rp.Id = l.Id;
                    rp.StaffId = l.StaffId;
                    rp.StaffName = string.Format("{0}-{1}", l.Staff.Ledger.AccountGroup.GroupCode, l.Staff.Ledger.LedgerName);
                    rp.RefCode = l.RefCode;
                    rp.RefNo = l.RefNo;
                    rp.Date = l.Date;
                    lstStockSeperated.Add(rp);
                    lstStockSeperated = lstStockSeperated.OrderBy(x => x.Date).ToList();
                }

            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return lstStockSeperated;
        }


        #endregion
    }

}