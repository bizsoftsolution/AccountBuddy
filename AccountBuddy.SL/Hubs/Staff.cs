﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AccountBuddy.Common;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {

        private BLL.Staff Staff_DALtoBLL(DAL.Staff StaffFrom)
        {
            BLL.Staff StaffTo = StaffFrom.toCopy<BLL.Staff>(new BLL.Staff());

            StaffTo.Ledger = LedgerDAL_BLL(StaffFrom.Ledger);
            
            return StaffTo;
        }

        public List<BLL.Staff> Staff_List()
        {
            return DB.Staffs.Where(x => x.Ledger.AccountGroup.CompanyDetail.Id == Caller.CompanyId).ToList()
                             .Select(x => Staff_DALtoBLL(x)).ToList();
        }

        public BLL.Staff Staff_Save(BLL.Staff cus)
        {
            try
            {
                DAL.Staff d = DB.Staffs.Where(x => x.Id == cus.Id).FirstOrDefault();
                if (d == null)
                {

                    d = new DAL.Staff();
                    cus.toCopy<DAL.Staff>(d);
                    d.LedgerId = Ledger_Save(cus.Ledger);
                    if (d.LedgerId != 0)
                    {
                       
                        DB.Staffs.Add(d);
                        DB.SaveChanges();
                        cus.Id = d.Id;
                        LogDetailStore(cus, LogDetailType.INSERT);
                    }
                }
                else
                {
                    cus.toCopy<DAL.Staff>(d);
                    Ledger_Save(cus.Ledger);
                    DB.SaveChanges();
                    LogDetailStore(cus, LogDetailType.UPDATE);
                }
                var b = Staff_DALtoBLL(d);

                Clients.Clients(OtherLoginClientsOnGroup).Staff_Save(b);
                //Clients.All.Staff_Save(b);
                // WriteLog("Staff_Save", BLL.UserAccount.User.Id,BLL.UserAccount.User.UserType.CompanyId , "Connection Timedout");
                return b;
            }
            catch (Exception ex)
            {
                WriteErrorLog("Staff", "Staff_Save", BLL.UserAccount.User.Id, Caller.CompanyId, ex.Message);
                return new BLL.Staff();
            }
           
        }

        public bool Staff_Delete(int pk)
        {
            var rv = false;
            try
            {
                var d = DB.Staffs.Where(x => x.Id == pk).FirstOrDefault();
                if (d != null && Ledger_CanDelete(d.Ledger))
                {
                    var b = Staff_DALtoBLL(d);
                    DB.Staffs.Remove(d);
                    DB.SaveChanges();
                    Ledger_Delete(d.LedgerId.Value);
                    LogDetailStore(b, LogDetailType.DELETE);
                }

                Clients.Clients(OtherLoginClientsOnGroup).Staff_Delete(pk);
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