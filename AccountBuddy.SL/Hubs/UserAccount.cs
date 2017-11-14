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
            var rv = new BLL.UserAccount();
            try
            {
                DAL.UserAccount ua = DB.UserAccounts
                                  .Where(x => x.UserType.CompanyDetail.CompanyName == CompanyName
                                    && x.LoginId == LoginId
                                    && x.Password == Password && x.UserType.CompanyDetail.IsActive != false)
                                    .FirstOrDefault();
                if (ua != null)
                {
                    Groups.Add(Context.ConnectionId, ua.UserType.CompanyId.ToString());
                    Caller.CompanyId = ua.UserType.CompanyId;
                    Caller.UnderCompanyId = ua.UserType.CompanyDetail.UnderCompanyId;
                    Caller.CompanyType = ua.UserType.CompanyDetail.CompanyType;
                    Caller.UserId = ua.Id;
                    Caller.AccYear = AccYear;
                    Caller.HierarchicalOrderNo = ua.UserType.HierarchicalOrderNo;
                    var stf = ua.Staffs.FirstOrDefault();
                    if (stf != null)
                    {
                        Caller.StaffId = stf.Id;
                    }
                    rv = UserAccountDAL_BLL(ua);
                    int yy = DateTime.Now.Month < 4 ? DateTime.Now.Year - 1 : DateTime.Now.Year;
                    if (AccYear.Length > 4) int.TryParse(AccYear.Substring(0, 4), out yy);
                    rv.UserType.Company.LoginAccYear = yy;
                    return rv;

                }
                else
                {
                    return rv;
                }

            }
            catch (Exception ex)
            {

                WriteErrorLog("Login", "UserAccount_Login", rv.Id, Caller.CompanyId, ex.Message);
                return rv;
            }


        }

        public List<BLL.UserAccount> UserAccount_List()
        {
            var l1 = DB.UserAccounts.Where(x => x.UserType.CompanyDetail.Id == Caller.CompanyId || x.UserType.CompanyDetail.UnderCompanyId == Caller.CompanyId).ToList()
                                  .Select(x => UserAccountDAL_BLL(x)).ToList();

            return l1;
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
                    var ld = DB.LogDetails.Where(x => x.LogMaster.EntityTypeId == pk);
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

        public Boolean Admin_Authentication(string CompanyName, String LoginId, String Password)
        {
          
            try
            {
                DAL.UserAccount ua = DB.UserAccounts
                                  .Where(x => x.UserType.CompanyDetail.CompanyName == CompanyName
                                    && x.LoginId == LoginId
                                    && x.Password == Password 
                                    && x.UserType.TypeOfUser ==BLL.DataKeyValue.Administrator_Key
                                    && x.UserType.CompanyDetail.IsActive != false)
                                    .FirstOrDefault();
                if (ua != null)
                {
                   
                    return true;

                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {

                WriteErrorLog("Login", "UserAccount_Login", 0, Caller.CompanyId, ex.Message);
                return true;
            }


        }


        #endregion
    }
}
