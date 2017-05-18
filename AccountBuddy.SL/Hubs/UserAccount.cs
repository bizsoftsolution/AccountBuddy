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
        #region UserAccount
        public static List<BLL.UserAccount> _UserAccountList;
        public static List<BLL.UserAccount> UserAccountList
        {
            get
            {
                if (_UserAccountList == null)
                {
                    _UserAccountList = DB.UserAccounts.Select(x => new BLL.UserAccount()
                    {
                        Id = x.Id,
                        UserName = x.UserName,
                        LoginId = x.LoginId,
                        Password = x.Password,
                        UserTypeId = x.UserTypeId,
                        UserTypeName = x.UserType.TypeOfUser,
                        CompanyId = x.CompanyId
                    }
                    ).ToList();
                }
                return _UserAccountList;
            }
        }
        
        public BLL.UserAccount UserAccount_Login(string AccYear, String CompanyName, String LoginId, String Password)
        {
            BLL.UserAccount u = new BLL.UserAccount();

            DAL.UserAccount ua = DB.UserAccounts
                                   .Where(x => x.CompanyDetail.CompanyName == CompanyName

                                                && x.LoginId == LoginId
                                                && x.Password == Password)
                                   .FirstOrDefault();
            if (ua != null)
            {
                Groups.Add(Context.ConnectionId, ua.CompanyId.ToString());
                Caller.CompanyId = ua.CompanyId;
                Caller.UserId = ua.Id;
                Caller.AccYear = AccYear;
                ua.toCopy<BLL.UserAccount>(u);
            }

            return u;
        }

        public List<BLL.UserAccount> UserAccount_List()
        {
            return UserAccountList.Where(x => x.CompanyId == Caller.CompanyId).ToList();
        }

        public int UserAccount_Save(BLL.UserAccount UserAccount)
        {
            try
            {
                UserAccount.CompanyId = Caller.CompanyId;

                BLL.UserAccount b = UserAccountList.Where(x => x.Id == UserAccount.Id).FirstOrDefault();
                DAL.UserAccount d = DB.UserAccounts.Where(x => x.Id == UserAccount.Id).FirstOrDefault();

                if (d == null)
                {

                    b = new BLL.UserAccount();
                    UserAccountList.Add(b);

                    d = new DAL.UserAccount();
                    DB.UserAccounts.Add(d);

                    UserAccount.toCopy<DAL.UserAccount>(d);
                    DB.SaveChanges();
                    d.toCopy<BLL.UserAccount>(b);

                    DB.SaveChanges();
                    UserAccount.Id = d.Id;
                    LogDetailStore(UserAccount, LogDetailType.INSERT);
                }
                else
                {
                    UserAccount.toCopy<BLL.UserAccount>(b);
                    UserAccount.toCopy<DAL.UserAccount>(d);
                    DB.SaveChanges();
                    LogDetailStore(UserAccount, LogDetailType.UPDATE);
                }

                Clients.Clients(OtherLoginClientsOnGroup).UserAccount_Save(UserAccount);

                return UserAccount.Id;
            }
            catch (Exception ex) { }
            return 0;
        }

        public void UserAccount_Delete(int pk)
        {
            try
            {
                BLL.UserAccount b = UserAccountList.Where(x => x.Id == pk).FirstOrDefault();
                if (b != null)
                {
                    var d = DB.UserAccounts.Where(x => x.Id == pk).FirstOrDefault();

                    DB.UserAccounts.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(b, LogDetailType.DELETE);
                    UserAccountList.Remove(b);
                }

                Clients.Clients(OtherLoginClientsOnGroup).UserAccount_Delete(pk);
                Clients.All.delete(pk);
            }
            catch (Exception ex) { }
        }

        
        #endregion
    }
}
