﻿using System;
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
            BLL.JobWorker JobWorkerTo = JobWorkerFrom.ToMap<BLL.JobWorker>(new BLL.JobWorker());

            JobWorkerTo.Ledger = LedgerDAL_BLL(JobWorkerFrom.Ledger);

            return JobWorkerTo;
        }

        public List<BLL.JobWorker> JobWorker_List()
        {
            return DB.JobWorkers.Where(x => x.Ledger.AccountGroup.CompanyDetail.Id == Caller.CompanyId).ToList()
                             .Select(x => JobWorker_DALtoBLL(x)).ToList();
        }

        public BLL.JobWorker JobWorker_Save(BLL.JobWorker cus)
        {
            try
            {
                DAL.JobWorker d = DB.JobWorkers.Where(x => x.Id == cus.Id).FirstOrDefault();
                if (d == null)
                {

                    d = new DAL.JobWorker();
                    cus.ToMap<DAL.JobWorker>(d);
                    d.LedgerId = Ledger_Save(cus.Ledger);
                    if (d.LedgerId != 0)
                    {

                        DB.JobWorkers.Add(d);
                        DB.SaveChanges();
                        cus.Id = d.Id;
                        LogDetailStore(cus, LogDetailType.INSERT);
                    }
                }
                else
                {
                    cus.ToMap<DAL.JobWorker>(d);
                    Ledger_Save(cus.Ledger);
                    DB.SaveChanges();
                    LogDetailStore(cus, LogDetailType.UPDATE);
                }
                var b = JobWorker_DALtoBLL(d);

                if (OtherClientsOnGroup.Count > 0) Clients.Clients(OtherClientsOnGroup).JobWorker_Save(b);
                
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
                var d = DB.JobWorkers.Where(x => x.Id == pk).FirstOrDefault();
                int LID =(int) d.LedgerId;
                if (d != null && Ledger_CanDelete(d.Ledger))
                {
                    var b = JobWorker_DALtoBLL(d);
                    DB.JobWorkers.Remove(d);
                    DB.SaveChanges();
                    Ledger_Delete(LID);
                    LogDetailStore(b, LogDetailType.DELETE);
                }

                if (OtherClientsOnGroup.Count > 0) Clients.Clients(OtherClientsOnGroup).JobWorker_Delete(pk);
                Clients.All.delete(pk);

                rv = true;

            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex);
                rv = false;
            }
            return rv;
        }
   
    }
}