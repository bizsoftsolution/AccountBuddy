using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AccountBuddy.Common;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        #region Job Order Received   
        public string JobOrderReceived_NewRefNo()
        {
            return JobOrderReceived_NewRefNoByCompanyId(Caller.CompanyId);
        }
        public string JobOrderReceived_NewRefNoByCompanyId(int CompanyId)
        {
            DateTime dt = DateTime.Now;
            string NewRefNo = "";
            string Prefix = string.Format("{0}{1:yy}{2:X}", BLL.FormPrefix.JobOrderReceived, dt, dt.Month);
            long No = 0;
            try
            {
                var d = Caller.DB.JobOrderReceiveds.Where(x => x.JobWorker.Ledger.AccountGroup.CompanyId == CompanyId && x.RefNo.StartsWith(Prefix))
                                                       .OrderByDescending(x => x.RefNo)
                                                       .FirstOrDefault();
                if (d != null) No = Convert.ToInt64(d.RefNo.Substring(Prefix.Length), 16);
                NewRefNo = string.Format("{0}{1:X5}", Prefix, No + 1);

            }
            catch (Exception ex)
            {
                WriteLog("Job Order Received New RefNo", BLL.UserAccount.User.Id, CompanyId, ex.ToString());
            }

            return NewRefNo;
        }
        public bool JobOrderReceived_Save(BLL.JobOrderReceived SO)
        {
            try
            {

                DAL.JobOrderReceived d = Caller.DB.JobOrderReceiveds.Where(x => x.Id == SO.Id).FirstOrDefault();

                if (d == null)
                {

                    d = new DAL.JobOrderReceived();
                    Caller.DB.JobOrderReceiveds.Add(d);

                    SO.toCopy<DAL.JobOrderReceived>(d);

                    foreach (var b_pod in SO.JRDetails)
                    {
                        DAL.JobOrderReceivedDetail d_pod = new DAL.JobOrderReceivedDetail();
                        b_pod.toCopy<DAL.JobOrderReceivedDetail>(d_pod);
                        d.JobOrderReceivedDetails.Add(d_pod);
                    }
                    Caller.DB.SaveChanges();
                    SO.Id = d.Id;
                    LogDetailStore(SO, LogDetailType.INSERT);
                }
                else
                {
                    //foreach (var d_SOd in d.JobOrderReceivedDetails)
                    //{
                    //    BLL.JobOrderReceivedDetail b_SOd = SO.JRDetails.Where(x => x.Id == d_SOd.Id).FirstOrDefault();
                    //    if (b_SOd == null) d.JobOrderReceivedDetails.Remove(d_SOd);
                    //}

                    decimal rd = SO.JRDetails.Select(X => X.JRId).FirstOrDefault().Value;
                    Caller.DB.JobOrderReceivedDetails.RemoveRange(d.JobOrderReceivedDetails.Where(x => x.JRId == rd).ToList());

                    SO.toCopy<DAL.JobOrderReceived>(d);
                    foreach (var b_SOd in SO.JRDetails)
                    {
                        //DAL.JobOrderReceivedDetail d_SOd = d.JobOrderReceivedDetails.Where(x => x.Id == b_SOd.Id).FirstOrDefault();
                        //if (d_SOd == null)
                        //{
                        DAL.JobOrderReceivedDetail d_SOd = new DAL.JobOrderReceivedDetail();
                        d.JobOrderReceivedDetails.Add(d_SOd);
                        //}
                        b_SOd.toCopy<DAL.JobOrderReceivedDetail>(d_SOd);
                    }
                    LogDetailStore(SO, LogDetailType.UPDATE);

                }
                Clients.Clients(OtherLoginClients).JobOrderReceived_RefNoRefresh(JobOrderReceived_NewRefNo());
                Journal_SaveByJobOrderReceived(d);
                return true;

            }

            catch (Exception ex) { }
            return false;
        }

        public BLL.JobOrderReceived JobOrderReceived_Find(string SearchText)
        {
            BLL.JobOrderReceived SO = new BLL.JobOrderReceived();
            try
            {

                DAL.JobOrderReceived d = Caller.DB.JobOrderReceiveds.Where(x => x.JobWorker.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.RefNo == SearchText).FirstOrDefault();
                Caller.DB.Entry(d).Reload();
                if (d != null)
                {

                    d.toCopy<BLL.JobOrderReceived>(SO);
                    SO.JobWorkerName = (d.JobWorker ?? Caller.DB.JobWorkers.Find(d.JobWorkerId) ?? new DAL.JobWorker()).Ledger.LedgerName;
                    foreach (var d_pod in d.JobOrderReceivedDetails)
                    {
                        BLL.JobOrderReceivedDetail b_pod = new BLL.JobOrderReceivedDetail();
                        d_pod.toCopy<BLL.JobOrderReceivedDetail>(b_pod);
                        SO.JRDetails.Add(b_pod);
                        b_pod.ProductName = (d_pod.Product ?? Caller.DB.Products.Find(d_pod.ProductId) ?? new DAL.Product()).ProductName;
                        b_pod.UOMName = (d_pod.UOM ?? Caller.DB.UOMs.Find(d_pod.UOMId) ?? new DAL.UOM()).Symbol;
                        //  SO.Status = d.JobOrderReceivedDetails.FirstOrDefault()..Count() > 0 ? "Sold" : "Pending";
                    }

                }
            }
            catch (Exception ex) { }
            return SO;
        }

        public bool JobOrderReceived_Delete(long pk)
        {
            try
            {
                DAL.JobOrderReceived d = Caller.DB.JobOrderReceiveds.Where(x => x.Id == pk).FirstOrDefault();

                if (d != null)
                {
                    var s = JobOrderReceived_DALtoBLL(d);
                    Caller.DB.JobOrderReceivedDetails.RemoveRange(d.JobOrderReceivedDetails);
                    Caller.DB.JobOrderReceiveds.Remove(d);
                    Caller.DB.SaveChanges();

                    LogDetailStore(s, LogDetailType.DELETE);
                    Journal_DeleteByJobOrderReceived(s);
                }
                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public List<BLL.JobOrderReceived> JobOrderReceived_JRPendingList()
        {
            return Caller.DB.JobOrderReceiveds.Where(x => x.JobWorker.Ledger.AccountGroup.CompanyId == Caller.CompanyId)
                                     .ToList()
                                     .Select(x => JobOrderReceived_DALtoBLL(x))
                                     .ToList();
        }
        public BLL.JobOrderReceived JobOrderReceived_DALtoBLL(DAL.JobOrderReceived d)
        {
            BLL.JobOrderReceived SO = d.toCopy<BLL.JobOrderReceived>(new BLL.JobOrderReceived());
            foreach (var d_SOd in d.JobOrderReceivedDetails)
            {
                SO.JRDetails.Add(d_SOd.toCopy<BLL.JobOrderReceivedDetail>(new BLL.JobOrderReceivedDetail()));
            }
            //SO.Status = d.JobOrderReceivedDetails.FirstOrDefault().SalesDetails.Count() > 0 ? "Sold" : "Pending";
            return SO;
        }
        public bool Find_JobReceiveRef(string RefNo, BLL.JobOrderReceived JO)
        {
            DAL.JobOrderReceived d1 = Caller.DB.JobOrderReceiveds.Where(x => x.JobWorker.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.RefNo == RefNo & x.Id != JO.Id).FirstOrDefault();

            if (d1 == null)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        public BLL.JobOrderReceived JobOrderReceived_FindById(int ID)
        {
            BLL.JobOrderReceived P = new BLL.JobOrderReceived();
            try
            {

                DAL.JobOrderReceived d = Caller.DB.JobOrderReceiveds.Where(x => x.JobWorker.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.Id == ID).FirstOrDefault();
                Caller.DB.Entry(d).Reload();
                if (d != null)
                {

                    d.toCopy<BLL.JobOrderReceived>(P);
                    P.JobWorkerName = (d.JobWorker ?? Caller.DB.JobWorkers.Find(d.JobWorkerId) ?? new DAL.JobWorker()).Ledger.LedgerName;
                    foreach (var d_pod in d.JobOrderReceivedDetails)
                    {
                        BLL.JobOrderReceivedDetail b_pod = new BLL.JobOrderReceivedDetail();
                        d_pod.toCopy<BLL.JobOrderReceivedDetail>(b_pod);
                        P.JRDetails.Add(b_pod);
                        b_pod.ProductName = (d_pod.Product ?? Caller.DB.Products.Find(d_pod.ProductId) ?? new DAL.Product()).ProductName;
                        b_pod.UOMName = (d_pod.UOM ?? Caller.DB.UOMs.Find(d_pod.UOMId) ?? new DAL.UOM()).Symbol;
                    }

                }
            }
            catch (Exception ex) { }
            return P;
        }


        public List<BLL.JobOrderReceived> JobOrderReceived_List(int? LedgerId,  DateTime dtFrom, DateTime dtTo, string BillNo, decimal amtFrom, decimal amtTo)
        {
            List<BLL.JobOrderReceived> lstJobOrderReceived = new List<BLL.JobOrderReceived>();
            Caller.DB = new DAL.DBFMCGEntities();
            BLL.JobOrderReceived rp = new BLL.JobOrderReceived();
            try
            {
                foreach (var l in Caller.DB.JobOrderReceiveds.
                      Where(x => x.JRDate >= dtFrom && x.JRDate <= dtTo
                      && (x.JobWorkerId == LedgerId || LedgerId == null)
                      && (BillNo == "" || x.RefNo == BillNo)
                      && (x.TotalAmount >= amtFrom && x.TotalAmount <= amtTo) &&
                      x.JobWorker.Ledger.AccountGroup.CompanyId == Caller.CompanyId).ToList())
                {
                    rp = new BLL.JobOrderReceived();
                    rp.TotalAmount = l.TotalAmount;
                    rp.JRDate = l.JRDate;
                    rp.RefNo = l.RefNo;

                    rp.Id = l.Id;
                    rp.JobWorkerId = l.JobWorkerId;
                    rp.JobWorkerName = string.Format("{0}-{1}", l.JobWorker.Ledger.AccountGroup.GroupCode, l.JobWorker.Ledger.LedgerName);

                    rp.RefCode = l.RefCode;
                    rp.RefNo = l.RefNo;

                    lstJobOrderReceived.Add(rp);
                    lstJobOrderReceived = lstJobOrderReceived.OrderBy(x => x.JRDate).ToList();
                }

            }
            catch (Exception ex) { }
            return lstJobOrderReceived;
        }

        #endregion
    }
}