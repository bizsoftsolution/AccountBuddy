using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AccountBuddy.Common;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        #region Stock Group
        BLL.StockGroup StockGroup_DALtoBLL(DAL.StockGroup d)
        {
            BLL.StockGroup b = d.toCopy<BLL.StockGroup>(new BLL.StockGroup());
             b.AccountGroup =  AccountGroupDAL_BLL(d.AccountGroup);
            return b;
        }
        public List<BLL.StockGroup> StockGroup_List()
        {
            return DB.StockGroups.Where(x => x.AccountGroup.CompanyId == Caller.CompanyId).ToList()
            .Select(x => StockGroup_DALtoBLL(x)).ToList();
        }

        public int StockGroup_Save(BLL.StockGroup agp)
        {
            try
            {
               agp.AccountGroup.CompanyId = Caller.CompanyId;
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
                var d = DB.AccountGroups.Where(x => x.Id == pk).FirstOrDefault();
                //var sd = DB.StockGroups.Where(x => x.AccountGroupId == pk).FirstOrDefault();
                //if (d.Products != null)
                //{
                //    if (d != null)
                //    {
                //        DB.StockGroups.Remove(sd);
                //        DB.SaveChanges();
                //        DB.AccountGroups.Remove(d);
                //        DB.SaveChanges();
                //        LogDetailStore(d.toCopy<BLL.StockGroup>(new BLL.StockGroup()), LogDetailType.DELETE);
                //    }

                //    Clients.Clients(OtherLoginClientsOnGroup).SStockGroup_Delete(pk);
                //    Clients.All.delete(pk);
                //    rv = true;
                //}
                //else
                //{
                //    rv = false;
                //}

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