using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountBuddy.Common;

namespace AccountBuddy.BLL
{
    public class BalanceSheet : INotifyPropertyChanged
    {
        #region Fields

        private Ledger _LedgerList;
        private decimal? _CrAmt;
        private decimal? _DrAmt;
        private decimal? _CrAmtOP;
        private decimal? _DrAmtOP;
        private string _Ledger;
        private static UserTypeDetail _UserPermission;

        #endregion

        #region Property
        public static UserTypeDetail UserPermission
        {
            get
            {
                if (_UserPermission == null)
                {
                    _UserPermission = UserAccount.User.UserType == null ? new UserTypeDetail() : UserAccount.User.UserType.UserTypeDetails.Where(x => x.UserTypeFormDetail.FormName == Forms.frmBalanceSheet.ToString()).FirstOrDefault();
                }
                return _UserPermission;
            }

            set
            {
                if (_UserPermission != value)
                {
                    _UserPermission = value;
                }
            }
        }
        public Ledger LedgerList
        {
            get
            {
                return _LedgerList;
            }
            set
            {
                if (_LedgerList != value)
                {
                    _LedgerList = value;
                    NotifyPropertyChanged(nameof(LedgerList));
                }
            }
        }
        public decimal? CrAmt
        {
            get
            {
                return _CrAmt;
            }
            set
            {
                if (_CrAmt != value)
                {
                    _CrAmt = value;
                    NotifyPropertyChanged(nameof(CrAmt));
                }
            }
        }
        public decimal? DrAmt
        {
            get
            {
                return _DrAmt;
            }
            set
            {
                if (_DrAmt != value)
                {
                    _DrAmt = value;
                    NotifyPropertyChanged(nameof(DrAmt));
                }
            }
        }

        public decimal? CrAmtOP
        {
            get
            {
                return _CrAmtOP;
            }
            set
            {
                if (_CrAmtOP != value)
                {
                    _CrAmtOP = value;
                    NotifyPropertyChanged(nameof(CrAmtOP));
                }
            }
        }
        public decimal? DrAmtOP
        {
            get
            {
                return _DrAmtOP;
            }
            set
            {
                if (_DrAmtOP != value)
                {
                    _DrAmtOP = value;
                    NotifyPropertyChanged(nameof(DrAmtOP));
                }
            }
        }
        public string  Ledger
        {
            get
            {
                return _Ledger;
            }
            set
            {
                if (_Ledger != value)
                {
                    _Ledger = value;
                    NotifyPropertyChanged(nameof(_Ledger));
                }
            }
        }

        #endregion        

        #region Property  Changed Event

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }

        private void NotifyAllPropertyChanged()
        {
            foreach (var p in this.GetType().GetProperties()) NotifyPropertyChanged(p.Name);
        }

        #endregion

        #region Methods

        public static List<BalanceSheet> ToList(DateTime dtFrom, DateTime dtTo)
        {
            return FMCGHubClient.FMCGHub.Invoke<List<BalanceSheet>>("Balancesheet_List", dtFrom, dtTo).Result;
        }

        #endregion
    }
}
