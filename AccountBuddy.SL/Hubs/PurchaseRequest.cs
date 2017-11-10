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

        public bool PurchaseRequest_Save(BLL.PurchaseRequest PO)
        {
            try
            {

                DAL.PurchaseRequest d = DB.PurchaseRequests.Where(x => x.Id == PO.Id).FirstOrDefault();

                if (d == null)
                {

                    d = new DAL.PurchaseRequest();
                    DB.PurchaseRequests.Add(d);

                    PO.toCopy<DAL.PurchaseRequest>(d);

                    foreach (var b_pod in PO.PODetails)
                    {
                        DAL.PurchaseRequestDetail d_pod = new DAL.PurchaseRequestDetail();
                        b_pod.toCopy<DAL.PurchaseRequestDetail>(d_pod);
                        d.PurchaseRequestDetails.Add(d_pod);
                    }
                    DB.SaveChanges();
                    PO.Id = d.Id;
                    LogDetailStore(PO, LogDetailType.INSERT);
                }
                else
                {

                    foreach (var d_pod in d.PurchaseRequestDetails)
                    {
                        BLL.PurchaseRequestDetail b_pod = PO.PODetails.Where(x => x.Id == d_pod.Id).FirstOrDefault();
                        if (b_pod == null) d.PurchaseRequestDetails.Remove(d_pod);
                    }

                    PO.toCopy<DAL.PurchaseRequest>(d);
                    foreach (var b_pod in PO.PODetails)
                    {
                        DAL.PurchaseRequestDetail d_pod = d.PurchaseRequestDetails.Where(x => x.Id == b_pod.Id).FirstOrDefault();
                        if (d_pod == null)
                        {
                            d_pod = new DAL.PurchaseRequestDetail();
                            d.PurchaseRequestDetails.Add(d_pod);
                        }
                        b_pod.toCopy<DAL.PurchaseRequestDetail>(d_pod);
                    }
                    DB.SaveChanges();
                    LogDetailStore(PO, LogDetailType.UPDATE);
                }

                Clients.Clients(OtherLoginClientsOnGroup).PurchaseRequest_RefNoRefresh(PurchaseRequest_NewRefNo());

             //   SalesOrder_SaveByPurchaseRequest(d);
                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public bool PurchaseRequest_MakePurchase(BLL.PurchaseRequest PO)
        {
            try
            {
                BLL.Purchase P = new BLL.Purchase();

                P.PurchaseDate = PO.PRDate.Value;
                P.RefNo = Purchase_NewRefNo();
                P.LedgerId = PO.LedgerId;
                P.TransactionType = "Cash";
                P.TransactionTypeId = 1;
                P.ItemAmount = PO.ItemAmount.Value;
                P.DiscountAmount = PO.DiscountAmount.Value;
                P.GSTAmount = PO.GSTAmount.Value;
                P.ExtraAmount = PO.Extras.Value;
                P.TotalAmount = PO.TotalAmount.Value;
                P.Narration = PO.Narration;


                foreach (var pod in PO.PODetails)
                {
                    BLL.PurchaseDetail PD = new BLL.PurchaseDetail()
                    {
                        PODId = pod.Id,
                        ProductId = pod.ProductId,
                        UOMId = pod.UOMId,
                        UOMName = pod.UOMName,
                        Quantity = pod.Quantity,
                        UnitPrice = pod.UnitPrice,
                        DiscountAmount = pod.DiscountAmount,
                        GSTAmount = pod.GSTAmount,
                        ProductName = pod.ProductName,
                        Amount = pod.Amount
                    };


                    P.PDetails.Add(PD);
                }
                return Purchase_Save(P);
            }
            catch (Exception ex) { }
            return true;
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
                    foreach (var b_pod in S.SalesOrderDetails)
                    {
                        DAL.PurchaseRequestDetail d_pod = new DAL.PurchaseRequestDetail();
                        b_pod.toCopy<DAL.PurchaseRequestDetail>(d_pod);
                        p.PurchaseRequestDetails.Add(d_pod);
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
            BLL.PurchaseRequest PO = new BLL.PurchaseRequest();
            try
            {

                DAL.PurchaseRequest d = DB.PurchaseRequests.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.RefNo == SearchText).FirstOrDefault();
                DB.Entry(d).Reload();
                if (d != null)
                {

                    d.toCopy<BLL.PurchaseRequest>(PO);
                    PO.LedgerName = (d.Ledger ?? DB.Ledgers.Find(d.LedgerId) ?? new DAL.Ledger()).LedgerName;
                    foreach (var d_pod in d.PurchaseRequestDetails)
                    {
                        BLL.PurchaseRequestDetail b_pod = new BLL.PurchaseRequestDetail();
                        d_pod.toCopy<BLL.PurchaseRequestDetail>(b_pod);
                        PO.PODetails.Add(b_pod);
                        b_pod.ProductName = (d_pod.Product ?? DB.Products.Find(d_pod.ProductId) ?? new DAL.Product()).ProductName;
                        b_pod.UOMName = (d_pod.UOM ?? DB.UOMs.Find(d_pod.UOMId) ?? new DAL.UOM()).Symbol;
                    //   PO.Status = d.PurchaseRequestDetails.FirstOrDefault().PurchaseDetails.Count() > 0 ? "Purchased" : "Pending";

                    }

                }
            }
            catch (Exception ex) { }
            return PO;
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

        public List<BLL.PurchaseRequest> PurchaseRequest_POPendingList()
        {
            return DB.PurchaseRequests.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId)
                                    .ToList()
                                    .Select(x => PurchaseRequest_DALtoBLL(x))
                                    .ToList();
        }

        public BLL.PurchaseRequest PurchaseRequest_DALtoBLL(DAL.PurchaseRequest d)
        {
            BLL.PurchaseRequest PO = d.toCopy<BLL.PurchaseRequest>(new BLL.PurchaseRequest());
            foreach (var d_pod in d.PurchaseRequestDetails)
            {
                PO.PODetails.Add(d_pod.toCopy<BLL.PurchaseRequestDetail>(new BLL.PurchaseRequestDetail()));
            }
            //PO.Status = d.PurchaseRequestDetails.FirstOrDefault().PurchaseDetails.Count() > 0 ? "Purchased" : "Pending";
            return PO;
        }
        public bool Find_PRQRef(string RefNo, BLL.PurchaseRequest PO)

        {
            DAL.PurchaseRequest d = DB.PurchaseRequests.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.RefNo == RefNo & x.Id != PO.Id).FirstOrDefault();
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