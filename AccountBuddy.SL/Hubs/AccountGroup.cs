using AccountBuddy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        #region Account Group

        #region list
        public static List<BLL.AccountGroup> _accountGroupList;
        public static List<BLL.AccountGroup> accountGroupList
        {
            get
            {
                if (_accountGroupList == null)
                {
                    _accountGroupList = new List<BLL.AccountGroup>();
                    foreach (var d1 in DB.AccountGroups.Where(x=> x.GroupName!="Primary").
                        Select(x => new BLL.AccountGroup()
                        {
                            Id = x.Id,
                            GroupCode = x.GroupCode,
                            GroupName = x.GroupName,
                            UnderGroupId = x.UnderGroupId == null ? 0 : (int)x.UnderGroupId,
                            underGroupName = x.AccountGroup2.GroupName
                        }).
                        OrderBy(x => x.GroupCode).ToList())
                    {
                        BLL.AccountGroup d2 = new BLL.AccountGroup();
                        d1.toCopy<BLL.AccountGroup>(d2);
                        _accountGroupList.Add(d2);
                    }

                }
                return _accountGroupList;
            }
            set
            {
                _accountGroupList = value;
            }

        }
        #endregion

        public List<BLL.AccountGroup> accountGroup_List()
        {
            return accountGroupList;
        }

        public int AccountGroup_Save(BLL.AccountGroup agp)
        {
            try
            {

                BLL.AccountGroup b = accountGroupList.Where(x => x.Id == agp.Id).FirstOrDefault();
                DAL.AccountGroup d = DB.AccountGroups.Where(x => x.Id == agp.Id).FirstOrDefault();

                if (d == null)
                {

                    b = new BLL.AccountGroup();
                    accountGroupList.Add(b);

                    d = new DAL.AccountGroup();
                    DB.AccountGroups.Add(d);

                    agp.toCopy<DAL.AccountGroup>(d);
                    DB.SaveChanges();
                    d.toCopy<BLL.AccountGroup>(b);

                    DB.SaveChanges();
                    agp.Id = d.Id;
                    LogDetailStore(agp, LogDetailType.INSERT);
                }
                else
                {
                    agp.toCopy<BLL.AccountGroup>(b);
                    agp.toCopy<DAL.AccountGroup>(d);
                    DB.SaveChanges();
                    LogDetailStore(agp, LogDetailType.UPDATE);
                }

                Clients.Clients(OtherLoginClientsOnGroup).AccountGroup_Save(agp);

                return agp.Id;
            }
            catch (Exception ex) { }
            return 0;
        }

        public void AccountGroup_Delete(int pk)
        {
            try
            {
                BLL.AccountGroup b = accountGroupList.Where(x => x.Id == pk).FirstOrDefault();
                if (b != null)
                {
                    var d = DB.AccountGroups.Where(x => x.Id == pk).FirstOrDefault();

                    DB.AccountGroups.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(b, LogDetailType.DELETE);
                    accountGroupList.Remove(b);
                }

                Clients.Clients(OtherLoginClientsOnGroup).SAccountGroup_Delete(pk);
                Clients.All.delete(pk);
            }
            catch (Exception ex) { }
        }


        #endregion
    }
}
