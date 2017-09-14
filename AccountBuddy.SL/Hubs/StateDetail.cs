using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AccountBuddy.Common;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {

        #region StateDetail
        BLL.StateDetail StateDetail_DALtoBLL(DAL.StateDetail d)
        {
            BLL.StateDetail b = d.toCopy<BLL.StateDetail>(new BLL.StateDetail());
            return b;
        }
        #region list
        public static List<BLL.StateDetail> _StateDetail_List;
        public static List<BLL.StateDetail> stateDetail_List
        {
            get
            {
                if (_StateDetail_List == null)
                {
                    _StateDetail_List = new List<BLL.StateDetail>();
                    foreach (var d1 in DB.StateDetails.ToList())
                    {
                        BLL.StateDetail d2 = new BLL.StateDetail();
                        d1.toCopy<BLL.StateDetail>(d2);
                        _StateDetail_List.Add(d2);
                    }

                }
                return _StateDetail_List;
            }
            set
            {
                _StateDetail_List = value;
            }

        }
        #endregion

        public List<BLL.StateDetail> StateDetail_List()
        {
            return stateDetail_List;
        }



        #endregion
    }
}