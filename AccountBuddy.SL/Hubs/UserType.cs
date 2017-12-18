using AccountBuddy.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        #region UserType

        private BLL.UserType UserTypeDAL_BLL(DAL.UserType d)
        {
            BLL.UserType b = d.toCopy<BLL.UserType>(new BLL.UserType());

            if (d!=null)
            {
                b.UserTypeDetails = new ObservableCollection<BLL.UserTypeDetail>(d.UserTypeDetails.Select(x => UserTypeDetailDAL_BLL(x)).ToList());
                b.Company = d.CompanyDetail.toCopy<BLL.CompanyDetail>(new BLL.CompanyDetail());
               
            }
            return b;
        }
     
        public List<BLL.UserType> UserType_List()
        {
            return DB.UserTypes.Where(x => x.CompanyId == Caller.CompanyId || x.CompanyDetail.UnderCompanyId==Caller.CompanyId).ToList()
                               .Select(x=> UserTypeDAL_BLL(x)).ToList();
        }

        public int userType_Save(BLL.UserType ut)
        {
            try
            {
                DAL.UserType d = DB.UserTypes.Where(x => x.Id == ut.Id).FirstOrDefault();
                
                if (d == null)
                {
                    var c = DB.CompanyDetails.Where(x => x.Id == ut.CompanyId).FirstOrDefault();

                    d = new DAL.UserType();
                    c.UserTypes.Add(d);
                    ut.toCopy<DAL.UserType>(d);

                    foreach(var utd in ut.UserTypeDetails)
                    {                        
                        d.UserTypeDetails.Add(utd.toCopy<DAL.UserTypeDetail>(new DAL.UserTypeDetail()));
                    }
                    DB.SaveChanges();
                    ut.Id = d.Id;
                    ut.Company = c.toCopy<BLL.CompanyDetail>(new BLL.CompanyDetail());
                    LogDetailStore(ut, LogDetailType.INSERT);
                }
                else
                {
                    ut.toCopy<DAL.UserType>(d);

                    foreach (var utd in ut.UserTypeDetails)
                    {
                        DAL.UserTypeDetail dd = d.UserTypeDetails.Where(x => x.Id == utd.Id).FirstOrDefault();
                        if (dd == null)
                        {
                            dd = new DAL.UserTypeDetail();
                            d.UserTypeDetails.Add(dd);
                        }
                        utd.toCopy<DAL.UserTypeDetail>(dd);                        
                    }

                    DB.SaveChanges();
                    LogDetailStore(ut, LogDetailType.UPDATE);
                }

                Clients.Clients(OtherLoginClients).userType_Save(ut);

                return ut.Id;
            }
            catch (Exception ex) { }
            return 0;
        }

        public void userType_Delete(int pk)
        {
            try
            {
                var d = DB.UserTypes.Where(x => x.Id == pk).FirstOrDefault();
                if (d != null)
                {
                    DB.UserTypeDetails.RemoveRange(d.UserTypeDetails);
                    DB.UserTypes.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(UserTypeDAL_BLL(d), LogDetailType.DELETE);                 
                }

                Clients.Clients(OtherLoginClients).userType_Delete(pk);
                Clients.All.delete(pk);
            }
            catch (Exception ex) { }
        }

        #endregion

    }
}
