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

        private BLL.UserAccount UserAccountDAL_BLL(DAL.UserAccount d)
        {
            BLL.UserAccount b = d.toCopy<BLL.UserAccount>(new BLL.UserAccount());
            b.UserType = UserTypeDAL_BLL(d.UserType);
            return b;
        }
        
        public BLL.UserAccount UserAccount_Login(string AccYear, String CompanyName, String LoginId, String Password)
        {

            DAL.UserAccount ua = DB.UserAccounts
                                   .Where(x => x.UserType.CompanyDetail.CompanyName == CompanyName

                                                && x.LoginId == LoginId
                                                
                                                && x.Password == Password && x.CompanyDetail.IsActive!=false)
                                   .FirstOrDefault();
            if (ua != null)
            {
                Groups.Add(Context.ConnectionId, ua.UserType.CompanyId.ToString());
                Caller.CompanyId = ua.UserType.CompanyId;
                Caller.UnderCompanyId = ua.UserType.CompanyDetail.UnderCompanyId.Value;
                Caller.CompanyType= ua.UserType.CompanyDetail.CompanyType;
                Caller.UserId = ua.Id;
                Caller.AccYear = AccYear;
                BLL.UserAccount u = UserAccountDAL_BLL(ua);
                int yy = DateTime.Now.Month < 4 ? DateTime.Now.Year - 1 : DateTime.Now.Year;
                if(AccYear.Length>4) int.TryParse(AccYear.Substring(0, 4), out yy);
                u.UserType.Company.LoginAccYear = yy;
                return u;
            }
            else
            {
                return new BLL.UserAccount();
            }

            
        }

        public List<BLL.UserAccount> UserAccount_List()
        {
            return DB.UserAccounts.Where(x => x.UserType.CompanyDetail.Id == Caller.CompanyId).ToList()
                                  .Select(x=> UserAccountDAL_BLL(x)).ToList();
        }

        public int UserAccount_Save(BLL.UserAccount ua)
        {
            try
            {
                
                DAL.UserAccount d = DB.UserAccounts.Where(x => x.Id == ua.Id).FirstOrDefault();

                if (d == null)
                {
                    d = new DAL.UserAccount();
                    DB.UserAccounts.Add(d);

                    ua.toCopy<DAL.UserAccount>(d);
                    DB.SaveChanges();

                    ua.Id = d.Id;
                    LogDetailStore(ua, LogDetailType.INSERT);
                }
                else
                {
                    ua.toCopy<DAL.UserAccount>(d);
                    DB.SaveChanges();
                    LogDetailStore(ua, LogDetailType.UPDATE);
                }

                Clients.Clients(OtherLoginClientsOnGroup).UserAccount_Save(ua);

                return ua.Id;
            }
            catch (Exception ex) { }
            return 0;
        }

        public void UserAccount_Delete(int pk)
        {
            try
            {
                var d = DB.UserAccounts.Where(x => x.Id == pk).FirstOrDefault();
                if (d != null)
                {
                    var ld=DB.LogDetails.Where(x => x.LogMaster.EntityTypeId == pk);
                    DB.LogDetails.RemoveRange(ld);
                    DB.SaveChanges();

                    DB.LogMasters.RemoveRange(DB.LogMasters.Where(x => x.EntityTypeId == pk));
                    DB.SaveChanges();

                    DB.UserAccounts.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(d.toCopy<BLL.UserAccount>(new BLL.UserAccount()), LogDetailType.DELETE);                    
                }
                Clients.Clients(OtherLoginClientsOnGroup).UserAccount_Delete(pk);
                Clients.All.delete(pk);
            }
            catch (Exception ex) { }
        }
        
        #endregion
    }
}
