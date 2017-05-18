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

        #region CreditLimit

        #region list
        public static List<BLL.CreditLimitType> _creditLimitList;
        public static List<BLL.CreditLimitType> creditLimitList
        {
            get
            {
                if (_creditLimitList == null)
                {
                    _creditLimitList = new List<BLL.CreditLimitType>();
                    foreach (var d1 in DB.CreditLimitTypes.ToList())
                    {
                        BLL.CreditLimitType d2 = new BLL.CreditLimitType();
                        d1.toCopy<BLL.CreditLimitType>(d2);
                        _creditLimitList.Add(d2);
                    }

                }
                return _creditLimitList;
            }
            set
            {
                _creditLimitList = value;
            }

        }
        #endregion

        public List<BLL.CreditLimitType> creditLimitType_List()
        {
            return creditLimitList;
        }



        #endregion
    }
}
