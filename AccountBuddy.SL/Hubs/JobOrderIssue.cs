using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AccountBuddy.Common;
using AccountBuddy.DAL;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        #region Job OrderIssue     
        public string JobOrderIssue_NewRefNo()
        {
            return JobOrderIssue_NewRefNoByCompanyId(Caller.CompanyId);
        }
        public string JobOrderIssue_NewRefNoByCompanyId(int CompanyId)
        {
            DateTime dt = DateTime.Now;
            string Prefix = string.Format("{0}{1:yy}{2:X}", BLL.FormPrefix.JobOrderIssue, dt, dt.Month);
            long No = 0;

            var d = DB.JobOrderIssues.Where(x => x.JobWorker.Ledger.AccountGroup.CompanyId == CompanyId && x.RefNo.StartsWith(Prefix))
                                     .OrderByDescending(x => x.RefNo)
                                     .FirstOrDefault();

            if (d != null) No = Convert.ToInt64(d.RefNo.Substring(Prefix.Length), 16);

            return string.Format("{0}{1:X5}", Prefix, No + 1);
        }
        public bool JobOrderIssue_Save(BLL.JobOrderIssue SO)
        {
            try
            {

                DAL.JobOrderIssue d = DB.JobOrderIssues.Where(x => x.Id == SO.Id).FirstOrDefault();

                if (d == null)
                {

                    d = new DAL.JobOrderIssue();
                    DB.JobOrderIssues.Add(d);

                    SO.ToMap(d);

                    foreach (var b_pod in SO.JODetails)
                    {
                        DAL.JobOrderIssueDetail d_pod = new DAL.JobOrderIssueDetail();
                        b_pod.ToMap(d_pod);
                        d.JobOrderIssueDetails.Add(d_pod);
                    }
                    DB.SaveChanges();
                    SO.Id = d.Id;
                    LogDetailStore(SO, LogDetailType.INSERT);
                }
                else
                {
                    //    foreach (var d_SOd in d.JobOrderIssueDetails)
                    //    {
                    //        BLL.JobOrderIssueDetail b_SOd = SO.JODetails.Where(x => x.Id == d_SOd.Id).FirstOrDefault();
                    //        if (b_SOd == null) d.JobOrderIssueDetails.Remove(d_SOd);
                    //    }
                    decimal rd = SO.JODetails.Select(X => X.JOId).FirstOrDefault().Value;
                    DB.JobOrderIssueDetails.RemoveRange(d.JobOrderIssueDetails.Where(x => x.JOId == rd).ToList());

                    SO.ToMap(d);
                    foreach (var b_SOd in SO.JODetails)
                    {
                        //DAL.JobOrderIssueDetail d_SOd = d.JobOrderIssueDetails.Where(x => x.Id == b_SOd.Id).FirstOrDefault();
                        //if (d_SOd == null)
                        //{
                        DAL.JobOrderIssueDetail d_SOd = new DAL.JobOrderIssueDetail();
                        d.JobOrderIssueDetails.Add(d_SOd);
                        //}
                        b_SOd.ToMap(d_SOd);
                    }
                    DB.SaveChanges();
                    LogDetailStore(SO, LogDetailType.UPDATE);

                }
                if (OtherClientsOnGroup.Count > 0) Clients.Clients(OtherClientsOnGroup).JobOrderIssue_RefNoRefresh(JobOrderIssue_NewRefNo());
                Journal_SaveByJobOrderIssue(d);
                return true;

            }

            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return false;
        }
        public BLL.JobOrderIssue JobOrderIssue_Find(string SearchText)
        {
            BLL.JobOrderIssue SO = new BLL.JobOrderIssue();
            try
            {

                DAL.JobOrderIssue d = DB.JobOrderIssues.Where(x => x.JobWorker.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.RefNo == SearchText).FirstOrDefault();
                DB.Entry(d).Reload();
                if (d != null)
                {

                    d.ToMap(SO);
                    SO.JobWorkerName = (d.JobWorker ?? DB.JobWorkers.Find(d.JobWorkerId) ?? new DAL.JobWorker()).Ledger.LedgerName;

                    int i = 0;
                    foreach (var d_pod in d.JobOrderIssueDetails)
                    {
                        BLL.JobOrderIssueDetail b_pod = new BLL.JobOrderIssueDetail();
                        d_pod.ToMap(b_pod);
                        SO.JODetails.Add(b_pod);
                        b_pod.SNo = ++i;
                        b_pod.ProductName = (d_pod.Product ?? DB.Products.Find(d_pod.ProductId) ?? new DAL.Product()).ProductName;
                        b_pod.UOMName = (d_pod.UOM ?? DB.UOMs.Find(d_pod.UOMId) ?? new DAL.UOM()).Symbol;
                        SO.Status = d.JobOrderIssueDetails.FirstOrDefault().JobOrderReceivedDetails.Count() > 0 ? "Received" : "Pending";
                    }

                }
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return SO;
        }

        public bool JobOrderIssue_Delete(long pk)
        {
            try
            {
                DAL.JobOrderIssue d = DB.JobOrderIssues.Where(x => x.Id == pk).FirstOrDefault();

                if (d != null)
                {
                    var s = JobOrderIssue_DALtoBLL(d);
                    DB.JobOrderIssueDetails.RemoveRange(d.JobOrderIssueDetails);
                    DB.JobOrderIssues.Remove(d);
                    DB.SaveChanges();

                    LogDetailStore(s, LogDetailType.DELETE);
                    Journal_DeleteByJobOrderIssue(s);

                }
                return true;
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return false;
        }



        public List<BLL.JobOrderIssue> JobOrderIssue_JOPendingList()
        {
            return DB.JobOrderIssues.Where(x => x.JobWorker.Ledger.AccountGroup.CompanyId == Caller.CompanyId)
                                     .ToList()
                                     .Select(x => JobOrderIssue_DALtoBLL(x))
                                     .ToList();
        }
        public BLL.JobOrderIssue JobOrderIssue_DALtoBLL(DAL.JobOrderIssue d)
        {
            BLL.JobOrderIssue SO = d.ToMap(new BLL.JobOrderIssue());
            foreach (var d_SOd in d.JobOrderIssueDetails)
            {
                SO.JODetails.Add(d_SOd.ToMap(new BLL.JobOrderIssueDetail()));
            }
            //SO.Status = d.JobOrderIssueDetails.FirstOrDefault().SalesDetails.Count() > 0 ? "Sold" : "Pending";
            return SO;
        }
        public bool Find_JOIssueRef(string RefNo, BLL.JobOrderIssue JO)
        {
            DAL.JobOrderIssue d1 = DB.JobOrderIssues.Where(x => x.JobWorker.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.RefNo == RefNo && x.Id != JO.Id).FirstOrDefault();

            if (d1 == null)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        public bool JobOrderIssue_MakeReceieved(BLL.JobOrderIssue JO)
        {
            try
            {
                BLL.JobOrderReceived P = new BLL.JobOrderReceived();

                P.JRDate = JO.JODate.Value;
                P.RefNo = JobOrderReceived_NewRefNo();
                P.JobWorkerId = JO.JobWorkerId;

                P.ItemAmount = JO.ItemAmount.Value;
                P.DiscountAmount = JO.DiscountAmount;
                P.GSTAmount = JO.GSTAmount.Value;
                P.ExtraAmount = JO.Extras.Value;
                P.TotalAmount = JO.TotalAmount.Value;
                P.Narration = JO.Narration;


                foreach (var pod in JO.JODetails)
                {
                    BLL.JobOrderReceivedDetail PD = new BLL.JobOrderReceivedDetail()
                    {
                        JODId = pod.Id,
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


                    P.JRDetails.Add(PD);
                }
                return JobOrderReceived_Save(P);
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return true;
        }

        public BLL.JobOrderIssue JobOrderIssue_FindById(int ID)
        {
            BLL.JobOrderIssue P = new BLL.JobOrderIssue();
            try
            {

                DAL.JobOrderIssue d = DB.JobOrderIssues.Where(x => x.JobWorker.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.Id == ID).FirstOrDefault();
                DB.Entry(d).Reload();
                if (d != null)
                {

                    d.ToMap(P);
                    P.JobWorkerName = (d.JobWorker ?? DB.JobWorkers.Find(d.JobWorkerId) ?? new DAL.JobWorker()).Ledger.LedgerName;
                    foreach (var d_pod in d.JobOrderIssueDetails)
                    {
                        BLL.JobOrderIssueDetail b_pod = new BLL.JobOrderIssueDetail();
                        d_pod.ToMap(b_pod);
                        P.JODetails.Add(b_pod);
                        b_pod.ProductName = (d_pod.Product ?? DB.Products.Find(d_pod.ProductId) ?? new DAL.Product()).ProductName;
                        b_pod.UOMName = (d_pod.UOM ?? DB.UOMs.Find(d_pod.UOMId) ?? new DAL.UOM()).Symbol;
                    }

                }
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return P;
        }


        public List<BLL.JobOrderIssue> JobOrderIssue_List(int? LedgerId,  DateTime dtFrom, DateTime dtTo, string BillNo, decimal amtFrom, decimal amtTo)
        {
            List<BLL.JobOrderIssue> lstJobOrderIssue = new List<BLL.JobOrderIssue>();
            
            BLL.JobOrderIssue rp = new BLL.JobOrderIssue();
            try
            {
                foreach (var l in DB.JobOrderIssues.
                      Where(x => x.JODate >= dtFrom && x.JODate <= dtTo
                      && (x.JobWorkerId == LedgerId || LedgerId == null)
                      && (BillNo == "" || x.RefNo == BillNo)
                      && (x.TotalAmount >= amtFrom && x.TotalAmount <= amtTo) &&
                      x.JobWorker.Ledger.AccountGroup.CompanyId == Caller.CompanyId).ToList())
                {
                    rp = new BLL.JobOrderIssue();
                    rp.ItemAmount = l.ItemAmount;
                    rp.TotalAmount = l.TotalAmount;                
                    rp.RefNo = l.RefNo;
                    rp.Id = l.Id;
                    rp.JobWorkerId = l.JobWorkerId;
                    rp.JobWorkerName = string.Format("{0}-{1}", l.JobWorker.Ledger.AccountGroup.GroupCode, l.JobWorker.Ledger.LedgerName);
                    rp.JODate = l.JODate;
                  
                    rp.RefCode = l.RefCode;
                    rp.RefNo = l.RefNo;

                    lstJobOrderIssue.Add(rp);
                    lstJobOrderIssue = lstJobOrderIssue.OrderBy(x => x.JODate).ToList();
                }

            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return lstJobOrderIssue;
        }

        #endregion
    }
}