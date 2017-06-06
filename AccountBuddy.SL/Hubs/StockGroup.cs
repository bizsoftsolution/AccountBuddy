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
        private BLL.StockGroup StockGroup_DALtoBLL(DAL.StockGroup StockGroupFrom)
        {
            BLL.StockGroup StockGroupTo = StockGroupFrom.toCopy<BLL.StockGroup>(new BLL.StockGroup());

            StockGroupTo.AccountGroup = AccountGroupDAL_BLL(StockGroupFrom.AccountGroup);


            return StockGroupTo;
        }

        public List<BLL.StockGroup> StockGroup_List()
        {
            return DB.StockGroups.Where(x => x.AccountGroup.CompanyDetail.Id == Caller.CompanyId).ToList()
                             .Select(x => StockGroup_DALtoBLL(x)).ToList();
        }

        public int StockGroup_Save(BLL.StockGroup cus)
        {
            try
            {
                DAL.StockGroup d = DB.StockGroups.Where(x => x.Id == cus.Id).FirstOrDefault();
                if (d == null)
                {

                    d = new DAL.StockGroup();
                    d.AccountGroupId = AccountGroup_Save(cus.AccountGroup);
                    if (d.AccountGroupId != 0)
                    {
                        DB.StockGroups.Add(d);
                        DB.SaveChanges();
                        cus.Id = d.Id;
                        LogDetailStore(cus, LogDetailType.INSERT);
                    }
                }
                else
                {
                    cus.toCopy<DAL.StockGroup>(d);
                    AccountGroup_Save(cus.AccountGroup);
                    DB.SaveChanges();
                    LogDetailStore(cus, LogDetailType.UPDATE);
                }

                Clients.Clients(OtherLoginClientsOnGroup).StockGroup_Save(cus);

                return d.Id;
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
                if (d != null && AccountGroup_CanDelete(d.AccountGroup))
                {
                    DB.StockGroups.Remove(d);
                    AccountGroup_Delete(d.AccountGroupId);
                    DB.SaveChanges();
                    LogDetailStore(StockGroup_DALtoBLL(d), LogDetailType.DELETE);
                }

                Clients.Clients(OtherLoginClientsOnGroup).StockGroup_Delete(pk);
                Clients.All.delete(pk);

                rv = true;

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