﻿using AccountBuddy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
      

        #region Account Group
        BLL.AccountGroup AccountGroupDAL_BLL(DAL.AccountGroup d)
        {
            BLL.AccountGroup b = d.toCopy<BLL.AccountGroup>(new BLL.AccountGroup());
            b.Company = d.CompanyDetail == null ? new BLL.CompanyDetail(): d.CompanyDetail.toCopy<BLL.CompanyDetail>(new BLL.CompanyDetail());
            b.UnderAccountGroup = d.AccountGroup2 == null ? new BLL.AccountGroup() : AccountGroupDAL_BLL(d.AccountGroup2);
            return b;
        }
        public List<BLL.AccountGroup> accountGroup_List()
        {
            var l= DB.AccountGroups.Where(x => x.CompanyId == Caller.CompanyId && x.GroupName != "Primary").ToList()
                              .Select(x => AccountGroupDAL_BLL(x)).ToList();
            //var l = DB.AccountGroups.Where(x => x.CompanyId == Caller.CompanyId).ToList()
            //                   .Select(x => AccountGroupDAL_BLL(x)).ToList();
            return l;
        }
        protected DbContext DbContext { get; set; }
     
        public int AccountGroup_Save(BLL.AccountGroup agp)
        {
            try
            {
            
        agp.CompanyId = Caller.CompanyId;
                DAL.AccountGroup d = DB.AccountGroups.Where(x => x.Id == agp.Id).FirstOrDefault();

                if (d == null)
                {
                    d = new DAL.AccountGroup();
                    DB.AccountGroups.Add(d);

                    agp.toCopy<DAL.AccountGroup>(d);
                    DB.SaveChanges();

                    agp.Id = d.Id;
                    LogDetailStore(agp, LogDetailType.INSERT);
                }
                else
                {
                    agp.toCopy<DAL.AccountGroup>(d);
                    DB.SaveChanges();
                    LogDetailStore(agp, LogDetailType.UPDATE);
                }

                //Clients.Clients(OtherLoginClientsOnGroup).AccountGroup_Save(agp);
                Clients.Clients(OtherLoginClients).AccountGroup_Save(agp);
                return agp.Id;
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return 0;
        }

        public bool AccountGroup_Delete(int pk)
        {
            var rv = false;
            try
            {
                var d = DB.AccountGroups.Where(x => x.Id == pk).FirstOrDefault();
                if (d.Ledgers != null && AccountGroup_CanDelete(d))
                {
                    if (d != null)
                    {
                        DB.AccountGroups.Remove(d);
                        DB.SaveChanges();
                        LogDetailStore(d.toCopy<BLL.AccountGroup>(new BLL.AccountGroup()), LogDetailType.DELETE);
                    }

                    // Clients.Clients(OtherLoginClientsOnGroup).AccountGroup_Delete(pk);
                    // Clients.All.delete(pk);
                    Clients.Clients(OtherLoginClients).AccountGroup_Save(d);

                    rv = true;
                }
                else
                {
                    rv = false;
                }

            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex); rv = false;
            }
            return rv;
        }

        public bool AccountGroup_CanDelete(DAL.AccountGroup l)
        {
            return l.Ledgers.Count() == 0;
                 
        }
        #endregion
    }
}
