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
        #region UserTypeFormDetail
        #region list
        public static List<BLL.UserTypeFormDetail> _UserTypeFormDetailList;

        public static List<BLL.UserTypeFormDetail> UserTypeFormDetailList
        {
            get
            {
                if (_UserTypeFormDetailList == null)
                {
                    _UserTypeFormDetailList = new List<BLL.UserTypeFormDetail>();
                    foreach (var d1 in DB.UserTypeFormDetails.Where(x=> x.IsActive==true).OrderBy(x => x.OrderNo).ToList())
                    {
                        BLL.UserTypeFormDetail d2 = new BLL.UserTypeFormDetail();
                        d1.toCopy<BLL.UserTypeFormDetail>(d2);
                        _UserTypeFormDetailList.Add(d2);
                    }

                }
                return _UserTypeFormDetailList;
            }
            set
            {
                _UserTypeFormDetailList = value;
            }

        }

        #endregion

        public List<BLL.UserTypeFormDetail> UserTypeFormDetail_List()
        {
            return UserTypeFormDetailList;
        }

        #endregion
    }
}
