using AccountBuddy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {

        #region TaxType

        #region list
        public List<BLL.TaxType> _TaxType_List;
        public List<BLL.TaxType> taxType_List
        {
            get
            {
                if (_TaxType_List == null)
                {
                    _TaxType_List = new List<BLL.TaxType>();
                    foreach (var d1 in DB.TaxTypes.ToList())
                    {
                        BLL.TaxType d2 = new BLL.TaxType();
                        d1.ToMap<BLL.TaxType>(d2);
                        _TaxType_List.Add(d2);
                    }

                }
                return _TaxType_List;
            }
            set
            {
                _TaxType_List = value;
            }

        }
        #endregion

        public List<BLL.TaxType> TaxType_List()
        {
            return taxType_List;
        }



        #endregion
    }
}