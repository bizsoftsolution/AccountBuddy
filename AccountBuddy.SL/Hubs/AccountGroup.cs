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
            BLL.AccountGroup b = d.ToMap(new BLL.AccountGroup());
            b.Company = d.CompanyDetail == null ? new BLL.CompanyDetail(): d.CompanyDetail.ToMap(new BLL.CompanyDetail());
            b.UnderAccountGroup = d.AccountGroup2 == null ? new BLL.AccountGroup() : AccountGroupDAL_BLL(d.AccountGroup2);
            return b;
        }
        public List<BLL.AccountGroup> accountGroup_List()
        {
            var l= DB.AccountGroups.Where(x => x.CompanyId == Caller.CompanyId && x.GroupName != "Primary").ToList()
                              .Select(x => AccountGroupDAL_BLL(x)).ToList();
            return l;
        }
     
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

                    agp.ToMap(d);
                    DB.SaveChanges();

                    agp.Id = d.Id;
                    LogDetailStore(agp, LogDetailType.INSERT);
                }
                else
                {
                    agp.ToMap(d);
                    DB.SaveChanges();
                    LogDetailStore(agp, LogDetailType.UPDATE);
                }


                if (OtherClientsOnGroup.Count > 0) Clients.Clients(OtherClientsOnGroup).AccountGroup_Save(agp);
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
                        LogDetailStore(d.ToMap(new BLL.AccountGroup()), LogDetailType.DELETE);
                    }
                    
                    if(OtherClientsOnGroup.Count>0)Clients.Clients(OtherClientsOnGroup).AccountGroup_Save(d);

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
