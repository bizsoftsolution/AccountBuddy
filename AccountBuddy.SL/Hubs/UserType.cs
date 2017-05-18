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

        public static List<BLL.UserType> _listUserType;
        public static List<BLL.UserType> ListUserType
        {
            get
            {
                if (_listUserType == null)
                {
                    
                    _listUserType = new List<BLL.UserType>();

                    foreach (var ut in DB.UserTypes)
                    {
                        BLL.UserType b = new BLL.UserType();
                        ut.toCopy<BLL.UserType>(b);
                        b.UserTypeDetails = new ObservableCollection<BLL.UserTypeDetail>();

                        foreach (var utf in UserTypeFormDetailList)
                        {
                            BLL.UserTypeDetail utd = new BLL.UserTypeDetail();

                            var d = ut.UserTypeDetails.Where(x => x.UserTypeFormDetailId == utf.Id).FirstOrDefault();
                            
                            if (d != null)
                            {
                                d.toCopy<BLL.UserTypeDetail>(utd);                               
                            }
                            utd.UserTypeFormDetailId = utf.Id;
                            utd.FormName = utf.FormName;
                            utd.FormType = utf.FormType;
                            b.UserTypeDetails.Add(utd);
                        }
                        _listUserType.Add(b);
                    }
                }
                return _listUserType;
            }

            set
            {
                _listUserType = value;
            }
        }

        public List<BLL.UserType> UserType_List()
        {
            return ListUserType;

        }

        public int userType_Save(BLL.UserType ut)
        {
            try
            {

                BLL.UserType b = ListUserType.Where(x => x.Id == ut.Id).FirstOrDefault();
                DAL.UserType d = DB.UserTypes.Where(x => x.Id == ut.Id).FirstOrDefault();

                if (d == null)
                {

                    b = new BLL.UserType();
                    ListUserType.Add(b);

                    d = new DAL.UserType();
                    DB.UserTypes.Add(d);

                    ut.toCopy<DAL.UserType>(d);

                    foreach(var utd in ut.UserTypeDetails)
                    {
                        DAL.UserTypeDetail dd = new DAL.UserTypeDetail();
                        utd.toCopy<DAL.UserTypeDetail>(dd);
                        d.UserTypeDetails.Add(dd);
                    }


                    DB.SaveChanges();
                    d.toCopy<BLL.UserType>(b);
                    b.UserTypeDetails = ut.UserTypeDetails;
                    
                    ut.Id = d.Id;
                    LogDetailStore(ut, LogDetailType.INSERT);
                }
                else
                {
                    ut.toCopy<BLL.UserType>(b);
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

                    b.UserTypeDetails = ut.UserTypeDetails;

                    DB.SaveChanges();
                    LogDetailStore(ut, LogDetailType.UPDATE);
                }

                Clients.Clients(OtherLoginClientsOnGroup).userType_Save(ut);

                return ut.Id;
            }
            catch (Exception ex) { }
            return 0;
        }

        public void userType_Delete(int pk)
        {
            try
            {
                BLL.UserType b = ListUserType.Where(x => x.Id == pk).FirstOrDefault();
                if (b != null)
                {
                    var d = DB.UserTypes.Where(x => x.Id == pk).FirstOrDefault();
                    DB.UserTypeDetails.RemoveRange(d.UserTypeDetails);
                    DB.UserTypes.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(b, LogDetailType.DELETE);
                    ListUserType.Remove(b);
                }

                Clients.Clients(OtherLoginClientsOnGroup).userType_Delete(pk);
                Clients.All.delete(pk);
            }
            catch (Exception ex) { }
        }

        #endregion

    }
}
