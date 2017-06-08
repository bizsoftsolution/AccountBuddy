﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AccountBuddy.Common;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        #region Account Group
        BLL.UOM UOM_DALtoBLL(DAL.UOM d)
        {
            BLL.UOM b = d.toCopy<BLL.UOM>(new BLL.UOM());
            b.Company = d.CompanyDetail.toCopy<BLL.CompanyDetail>(new BLL.CompanyDetail());
             return b;
        }
        public List<BLL.UOM> UOM_List()
        {
            return DB.UOMs.Where(x => x.CompanyId == Caller.CompanyId ).ToList()
                               .Select(x => UOM_DALtoBLL(x)).ToList();
        }

        public int UOM_Save(BLL.UOM agp)
        {
            try
            {
                agp.CompanyId = Caller.CompanyId;
                DAL.UOM d = DB.UOMs.Where(x => x.Id == agp.Id).FirstOrDefault();

                if (d == null)
                {
                    d = new DAL.UOM();
                    DB.UOMs.Add(d);

                    agp.toCopy<DAL.UOM>(d);
                    DB.SaveChanges();

                    agp.Id = d.Id;
                    LogDetailStore(agp, LogDetailType.INSERT);
                }
                else
                {
                    agp.toCopy<DAL.UOM>(d);
                    DB.SaveChanges();
                    LogDetailStore(agp, LogDetailType.UPDATE);
                }

                Clients.Clients(OtherLoginClientsOnGroup).UOM_Save(agp);

                return agp.Id;
            }
            catch (Exception ex) { }
            return 0;
        }

        public bool UOM_Delete(int pk)
        {
            var rv = false;
            try
            {
                var d = DB.UOMs.Where(x => x.Id == pk).FirstOrDefault();
                if (d.Products != null)
                {
                    if (d != null)
                    {
                        DB.UOMs.Remove(d);
                        DB.SaveChanges();
                        LogDetailStore(d.toCopy<BLL.UOM>(new BLL.UOM()), LogDetailType.DELETE);
                    }

                    Clients.Clients(OtherLoginClientsOnGroup).SUOM_Delete(pk);
                    Clients.All.delete(pk);
                    rv = true;
                }
                else
                {
                    rv = false;
                }

            }
            catch (Exception ex)
            {
                rv = false;
            }
            return rv;
        }


        #endregion
    }
}