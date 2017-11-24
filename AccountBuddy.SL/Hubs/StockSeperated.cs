using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AccountBuddy.Common;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        #region Job OrderIssue     
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

                    SO.toCopy<DAL.StockSeparated>(d);

                    foreach (var b_pod in SO.SSDetails)
                    {
                        DAL.StockSeperatedDetail d_pod = new DAL.StockSeperatedDetail();
                        b_pod.toCopy<DAL.StockSeperatedDetail>(d_pod);
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


                    SO.toCopy<DAL.StockSeparated>(d);
                    foreach (var b_SOd in SO.SSDetails)
                    {
                        //DAL.StockSeperatedDetail d_SOd = d.StockSeperatedDetails.Where(x => x.Id == b_SOd.Id).FirstOrDefault();
                        //if (d_SOd == null)
                        //{
                        DAL.StockSeperatedDetail d_SOd = new DAL.StockSeperatedDetail();
                        d.StockSeperatedDetails.Add(d_SOd);
                        //}
                        b_SOd.toCopy<DAL.StockSeperatedDetail>(d_SOd);
                    }
                    LogDetailStore(SO, LogDetailType.UPDATE);

                }
                Clients.Clients(OtherLoginClientsOnGroup).StockSeperated_RefNoRefresh(StockSeperated_NewRefNo());
                Journal_SaveByStockSeparated(d);
                return true;

            }

            catch (Exception ex) { }
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

                    d.toCopy<BLL.StockSeperated>(SO);
                    SO.StaffName = (d.Staff ?? DB.Staffs.Find(d.StaffId) ?? new DAL.Staff()).Ledger.LedgerName;
                    foreach (var d_pod in d.StockSeperatedDetails)
                    {
                        BLL.StockSeperatedDetail b_pod = new BLL.StockSeperatedDetail();
                        d_pod.toCopy<BLL.StockSeperatedDetail>(b_pod);
                        SO.SSDetails.Add(b_pod);
                        b_pod.ProductName = (d_pod.Product ?? DB.Products.Find(d_pod.ProductId) ?? new DAL.Product()).ProductName;
                        b_pod.UOMName = (d_pod.UOM ?? DB.UOMs.Find(d_pod.UOMId) ?? new DAL.UOM()).Symbol;
                        //  SO.Status = d.StockSeperatedDetails.FirstOrDefault()..Count() > 0 ? "Sold" : "Pending";
                    }

                }
            }
            catch (Exception ex) { }
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
            catch (Exception ex) { }
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
            BLL.StockSeperated SO = d.toCopy<BLL.StockSeperated>(new BLL.StockSeperated());
            foreach (var d_SOd in d.StockSeperatedDetails)
            {
                SO.SSDetails.Add(d_SOd.toCopy<BLL.StockSeperatedDetail>(new BLL.StockSeperatedDetail()));
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

                    d.toCopy<BLL.StockSeperated>(P);
                    P.StaffName = (d.Staff ?? DB.Staffs.Find(d.StaffId) ?? new DAL.Staff()).Ledger.LedgerName;
                    foreach (var d_pod in d.StockSeperatedDetails)
                    {
                        BLL.StockSeperatedDetail b_pod = new BLL.StockSeperatedDetail();
                        d_pod.toCopy<BLL.StockSeperatedDetail>(b_pod);
                        P.SSDetails.Add(b_pod);
                        b_pod.ProductName = (d_pod.Product ?? DB.Products.Find(d_pod.ProductId) ?? new DAL.Product()).ProductName;
                        b_pod.UOMName = (d_pod.UOM ?? DB.UOMs.Find(d_pod.UOMId) ?? new DAL.UOM()).Symbol;
                    }

                }
            }
            catch (Exception ex) { }
            return P;
        }

        #endregion
    }

}