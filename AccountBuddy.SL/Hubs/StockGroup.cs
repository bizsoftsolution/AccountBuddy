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
        BLL.StockGroup StockGroupDAL_BLL(DAL.StockGroup d)
        {
            BLL.StockGroup b = d.toCopy<BLL.StockGroup>(new BLL.StockGroup());
            b.Company = d.CompanyDetail.toCopy<BLL.CompanyDetail>(new BLL.CompanyDetail());
            b.UnderStockGroup = d.StockGroup2 == null ? new BLL.StockGroup() : StockGroupDAL_BLL(d.StockGroup2);
            return b;
        }
        public List<BLL.StockGroup> StockGroup_List()
        {
            return DB.StockGroups.Where(x => x.CompanyId == Caller.CompanyId).ToList()
                               .Select(x => StockGroupDAL_BLL(x)).ToList();
        }

        public int StockGroup_Save(BLL.StockGroup agp)
        {
            try
            {
                agp.CompanyId = Caller.CompanyId;
                DAL.StockGroup d = DB.StockGroups.Where(x => x.Id == agp.Id).FirstOrDefault();

                if (d == null)
                {
                    d = new DAL.StockGroup();
                    DB.StockGroups.Add(d);

                    agp.toCopy<DAL.StockGroup>(d);
                    DB.SaveChanges();

                    agp.Id = d.Id;
                    LogDetailStore(agp, LogDetailType.INSERT);
                }
                else
                {
                    agp.toCopy<DAL.StockGroup>(d);
                    DB.SaveChanges();
                    LogDetailStore(agp, LogDetailType.UPDATE);
                }

                Clients.Clients(OtherLoginClientsOnGroup).StockGroup_Save(agp);

                return agp.Id;
            }
            catch (Exception ex) { }
            return 0;
        }

        public bool StockGroup_Delete(int pk)
        {
            var rv = false;
            try
            {
                var d = DB.StockGroups.Where(x => x.Id == pk).FirstOrDefault();
                if (d.Ledgers != null)
                {
                    if (d != null)
                    {
                        DB.StockGroups.Remove(d);
                        DB.SaveChanges();
                        LogDetailStore(d.toCopy<BLL.StockGroup>(new BLL.StockGroup()), LogDetailType.DELETE);
                    }

                    Clients.Clients(OtherLoginClientsOnGroup).SStockGroup_Delete(pk);
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