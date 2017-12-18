using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        public List<BLL.JOPending> JOPending_List(DateTime dtFrom, DateTime dtTo)
        {
            List<BLL.JOPending> lstJOPending = new List<BLL.JOPending>();
            BLL.JOPending tb = new BLL.JOPending();

            var lstLedger = DB.JobWorkers.Where(x=>x.Ledger.AccountGroup.CompanyId==Caller.CompanyId).ToList();

            foreach (var l in lstLedger)
            {
                foreach (var pd in l.JobOrderIssues.Where(x => x.JODate >= dtFrom && x.JODate <= dtTo && x.JobWorker.Ledger.AccountGroup.CompanyId == Caller.CompanyId).ToList())
                {
                    var po = l.JobOrderIssues.FirstOrDefault();
                    tb = new BLL.JOPending();
                    tb.JobWorker = JobWorker_DALtoBLL(l);
                    
                    tb.EntryNo = pd.RefNo;
                    tb.Amount = pd.TotalAmount;
                    tb.JODate = pd.JODate;
                    tb.Status = pd.JobOrderIssueDetails.FirstOrDefault().JobOrderReceivedDetails.Count() > 0 ? "Received" : "Pending";
                    lstJOPending.Add(tb);
                }

            }

            return lstJOPending;
        }

    }
}