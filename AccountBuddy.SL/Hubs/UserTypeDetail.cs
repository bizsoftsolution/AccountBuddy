using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        #region UserTypeDetail
        public static List<BLL.UserTypeDetail> _listUserTypeDetail;
        public static List<BLL.UserTypeDetail> ListUserTypeDetail
        {
            get
            {
                if (_listUserTypeDetail == null)
                {
                    _listUserTypeDetail = DB.UserTypeDetails.Select(x => new BLL.UserTypeDetail()
                    {
                        Id = x.Id,
                        UserTypeId = x.UserTypeId,
                        UserTypeFormDetailId = x.UserTypeFormDetailId,
                        IsViewForm = x.IsViewForm,
                        AllowDelete = x.AllowDelete,
                        AllowInsert = x.AllowInsert,
                        AllowUpdate = x.AllowUpdate,
                        FormName = x.UserTypeFormDetail.FormName,
                        UserTypeName = x.UserType.TypeOfUser

                    }
                    ).ToList();
                }
                return _listUserTypeDetail;
            }
            set
            {
                _listUserTypeDetail = value;
            }
        }

        public List<BLL.UserTypeDetail> UserTypeDetail_List()
        {
            return ListUserTypeDetail;
        }
        #endregion
    }
}
