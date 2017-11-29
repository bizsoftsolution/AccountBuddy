using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AccountBuddy.Common;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {

        private BLL.JobWorker JobWorker_DALtoBLL(DAL.JobWorker JobWorkerFrom)
        {
            BLL.JobWorker JobWorkerTo = JobWorkerFrom.toCopy<BLL.JobWorker>(new BLL.JobWorker());

            JobWorkerTo.Ledger = LedgerDAL_BLL(JobWorkerFrom.Ledger);

            return JobWorkerTo;
        }

        public List<BLL.JobWorker> JobWorker_List()
        {
            return Caller.DB.JobWorkers.Where(x => x.Ledger.AccountGroup.CompanyDetail.Id == Caller.CompanyId).ToList()
                             .Select(x => JobWorker_DALtoBLL(x)).ToList();
        }

        public BLL.JobWorker JobWorker_Save(BLL.JobWorker cus)
        {
            try
            {
                DAL.JobWorker d = Caller.DB.JobWorkers.Where(x => x.Id == cus.Id).FirstOrDefault();
                if (d == null)
                {

                    d = new DAL.JobWorker();
                    cus.toCopy<DAL.JobWorker>(d);
                    d.LedgerId = Ledger_Save(cus.Ledger);
                    if (d.LedgerId != 0)
                    {

                        Caller.DB.JobWorkers.Add(d);
                        Caller.DB.SaveChanges();
                        cus.Id = d.Id;
                        LogDetailStore(cus, LogDetailType.INSERT);
                    }
                }
                else
                {
                    cus.toCopy<DAL.JobWorker>(d);
                    Ledger_Save(cus.Ledger);
                    Caller.DB.SaveChanges();
                    LogDetailStore(cus, LogDetailType.UPDATE);
                }
                var b = JobWorker_DALtoBLL(d);

                Clients.Clients(OtherLoginClientsOnGroup).JobWorker_Save(b);
                //Clients.All.JobWorker_Save(b);
                // WriteLog("JobWorker_Save", BLL.UserAccount.User.Id,BLL.UserAccount.User.UserType.CompanyId , "Connection Timedout");
                return b;
            }
            catch (Exception ex)
            {
                WriteErrorLog("JobWorker", "JobWorker_Save", BLL.UserAccount.User.Id, Caller.CompanyId, ex.Message);
                return new BLL.JobWorker();
            }

        }

        public bool JobWorker_Delete(int pk)
        {
            var rv = false;
            try
            {
                var d = Caller.DB.JobWorkers.Where(x => x.Id == pk).FirstOrDefault();
                int LID =(int) d.LedgerId;
                if (d != null && Ledger_CanDelete(d.Ledger))
                {
                    var b = JobWorker_DALtoBLL(d);
                    Caller.DB.JobWorkers.Remove(d);
                    Caller.DB.SaveChanges();
                    Ledger_Delete(LID);
                    LogDetailStore(b, LogDetailType.DELETE);
                }

                Clients.Clients(OtherLoginClientsOnGroup).JobWorker_Delete(pk);
                Clients.All.delete(pk);

                rv = true;

            }
            catch (Exception ex)
            {
                rv = false;
            }
            return rv;
        }
   
    }
}