using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AccountBuddy.Common;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        #region Purchase Request       
        public string PurchaseRequest_NewRefNo()
        {
            return PurchaseRequest_NewRefNoByCompanyId(Caller.CompanyId);
        }
        public string PurchaseRequest_NewRefNoByCompanyId(int CompanyId)
        {
            DateTime dt = DateTime.Now;
            string Prefix = string.Format("{0}{1:yy}{2:X}", BLL.FormPrefix.PurchaseRequest, dt, dt.Month);
            long No = 0;

            var d = DB.PurchaseRequests.Where(x => x.Ledger.AccountGroup.CompanyId == CompanyId && x.RefNo.StartsWith(Prefix))
                                     .OrderByDescending(x => x.RefNo)
                                     .FirstOrDefault();

            if (d != null) No = Convert.ToInt64(d.RefNo.Substring(Prefix.Length), 16);

            return string.Format("{0}{1:X5}", Prefix, No + 1);
        }

        public bool PurchaseRequest_Save(BLL.PurchaseRequest PR)
        {
            try
            {

                DAL.PurchaseRequest d = DB.PurchaseRequests.Where(x => x.Id == PR.Id).FirstOrDefault();

                if (d == null)
                {

                    d = new DAL.PurchaseRequest();
                    DB.PurchaseRequests.Add(d);

                    PR.toCopy<DAL.PurchaseRequest>(d);

                    foreach (var b_prd in PR.PRDetails)
                    {
                        DAL.PurchaseRequestDetail d_prd = new DAL.PurchaseRequestDetail();
                        b_prd.toCopy<DAL.PurchaseRequestDetail>(d_prd);
                        d.PurchaseRequestDetails.Add(d_prd);
                    }
                    d.PurchaseRequestStatusDetails.Add(new DAL.PurchaseRequestStatusDetail() {
                        RequestBy=Caller.StaffId,
                        RequestAt = DateTime.Now,
                        RequestTo = PR.RequestTo                        
                    });
                    DB.SaveChanges();
                    PR.Id = d.Id;
                    LogDetailStore(PR, LogDetailType.INSERT);
                }
                else
                {

                    foreach (var d_prd in d.PurchaseRequestDetails)
                    {
                        BLL.PurchaseRequestDetail b_prd = PR.PRDetails.Where(x => x.Id == d_prd.Id).FirstOrDefault();
                        if (b_prd == null) d.PurchaseRequestDetails.Remove(d_prd);
                    }

                    PR.toCopy<DAL.PurchaseRequest>(d);
                    foreach (var b_prd in PR.PRDetails)
                    {
                        DAL.PurchaseRequestDetail d_prd = d.PurchaseRequestDetails.Where(x => x.Id == b_prd.Id).FirstOrDefault();
                        if (d_prd == null)
                        {
                            d_prd = new DAL.PurchaseRequestDetail();
                            d.PurchaseRequestDetails.Add(d_prd);
                        }
                        b_prd.toCopy<DAL.PurchaseRequestDetail>(d_prd);
                    }
                    DB.SaveChanges();
                    LogDetailStore(PR, LogDetailType.UPDATE);
                }

                Clients.Clients(OtherLoginClientsOnGroup).PurchaseRequest_RefNoRefresh(PurchaseRequest_NewRefNo());

             //   SalesOrder_SaveByPurchaseRequest(d);
                return true;
            }
            catch (Exception ex) { }
            return false;
        }

       
        #region SalesOrder

        void PurchaseRequest_SaveBySalesOrder(DAL.SalesOrder S)
        {
            string RefCode = string.Format("{0}{1}", BLL.FormPrefix.SalesOrder, S.Id);

            DAL.PurchaseRequest p = DB.PurchaseRequests.Where(x => x.RefCode == RefCode).FirstOrDefault();

            if (S.Ledger.LedgerName.StartsWith("CM-") || S.Ledger.LedgerName.StartsWith("WH-") || S.Ledger.LedgerName.StartsWith("DL-"))
            {
                var LName = LedgerNameByCompanyId(Caller.CompanyId);
                var CId = CompanyIdByLedgerName(S.Ledger.LedgerName);
                var LId = LedgerIdByCompany(LName, CId);

                if (LId != 0)
                {
                    if (p == null)
                    {
                        p = new DAL.PurchaseRequest();
                        p.RefNo = PurchaseRequest_NewRefNoByCompanyId(CId);
                        p.RefCode = RefCode;
                        DB.PurchaseRequests.Add(p);
                    }
                    else
                    {
                        DB.PurchaseRequestDetails.RemoveRange(p.PurchaseRequestDetails);
                    }

                    p.PRDate = S.SODate;
                    p.DiscountAmount = S.DiscountAmount;
                    p.Extras = S.ExtraAmount;
                    p.GSTAmount = S.GSTAmount;
                    p.ItemAmount = S.ItemAmount;
                    p.TotalAmount = S.TotalAmount;
                    p.LedgerId = LId;
                    foreach (var b_prd in S.SalesOrderDetails)
                    {
                        DAL.PurchaseRequestDetail d_prd = new DAL.PurchaseRequestDetail();
                        b_prd.toCopy<DAL.PurchaseRequestDetail>(d_prd);
                        p.PurchaseRequestDetails.Add(d_prd);
                    }
                    DB.SaveChanges();

                }
            }


        }
        public bool PurchaseRequest_DeleteBySalesOrder(DAL.SalesOrder s)
        {
            try
            {
                string RefCode = string.Format("{0}{1}", BLL.FormPrefix.SalesOrder, s.Id);
                DAL.PurchaseRequest d = DB.PurchaseRequests.Where(x => x.RefCode == RefCode).FirstOrDefault();
                if (d != null)
                {
                    PurchaseRequest_Delete(d.Id);
                }


            }
            catch (Exception ex) { }
            return false;
        }

        #endregion

        public BLL.PurchaseRequest PurchaseRequest_Find(string SearchText)
        {
            BLL.PurchaseRequest PR = new BLL.PurchaseRequest();
            try
            {

                DAL.PurchaseRequest d = DB.PurchaseRequests.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.RefNo == SearchText).FirstOrDefault();
                DB.Entry(d).Reload();
                if (d != null)
                {

                    d.toCopy<BLL.PurchaseRequest>(PR);
                    PR.LedgerName = (d.Ledger ?? DB.Ledgers.Find(d.LedgerId) ?? new DAL.Ledger()).LedgerName;
                    foreach (var d_prd in d.PurchaseRequestDetails)
                    {
                        BLL.PurchaseRequestDetail b_prd = new BLL.PurchaseRequestDetail();
                        d_prd.toCopy<BLL.PurchaseRequestDetail>(b_prd);
                        PR.PRDetails.Add(b_prd);
                        b_prd.ProductName = (d_prd.Product ?? DB.Products.Find(d_prd.ProductId) ?? new DAL.Product()).ProductName;
                        b_prd.UOMName = (d_prd.UOM ?? DB.UOMs.Find(d_prd.UOMId) ?? new DAL.UOM()).Symbol;
                    }

                }
            }
            catch (Exception ex) { }
            return PR;
        }

        public bool PurchaseRequest_Delete(long pk)
        {
            try
            {
                DAL.PurchaseRequest d = DB.PurchaseRequests.Where(x => x.Id == pk).FirstOrDefault();

                if (d != null)
                {
                    var P = PurchaseRequest_DALtoBLL(d);
                    DB.PurchaseRequestDetails.RemoveRange(d.PurchaseRequestDetails);
                    DB.PurchaseRequests.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(P, LogDetailType.DELETE);
                  //  SalesOrder_DeleteByPurchaseRequest(d);
                }

                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public List<BLL.PurchaseRequest> PurchaseRequest_PRPendingList()
        {
            return DB.PurchaseRequests.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId)
                                    .ToList()
                                    .Select(x => PurchaseRequest_DALtoBLL(x))
                                    .ToList();
        }

        public BLL.PurchaseRequest PurchaseRequest_DALtoBLL(DAL.PurchaseRequest d)
        {
            BLL.PurchaseRequest PR = d.toCopy<BLL.PurchaseRequest>(new BLL.PurchaseRequest());
            foreach (var d_prd in d.PurchaseRequestDetails)
            {
                PR.PRDetails.Add(d_prd.toCopy<BLL.PurchaseRequestDetail>(new BLL.PurchaseRequestDetail()));
            }
            return PR;
        }
        public bool Find_PRQRef(string RefNo, BLL.PurchaseRequest PR)
        {
            DAL.PurchaseRequest d = DB.PurchaseRequests.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.RefNo == RefNo & x.Id != PR.Id).FirstOrDefault();
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