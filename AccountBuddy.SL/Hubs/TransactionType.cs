using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AccountBuddy.Common;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {

        #region CreditLimit

        #region list
        public  List<BLL.TransactionType> _transactionType_List;
        public  List<BLL.TransactionType> transactionType_List
        {
            get
            {
                if (_transactionType_List == null)
                {
                    _transactionType_List = new List<BLL.TransactionType>();
                    foreach (var d1 in DB.TransactionTypes.ToList())
                    {
                        BLL.TransactionType d2 = new BLL.TransactionType();
                        d1.toCopy<BLL.TransactionType>(d2);
                        _transactionType_List.Add(d2);
                    }

                }
                return _transactionType_List;
            }
            set
            {
                _transactionType_List = value;
            }

        }
        #endregion

        public List<BLL.TransactionType> TransactionType_List()
        {
            return transactionType_List;
        }



        #endregion
    }
}