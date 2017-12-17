using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountBuddy.Common;

namespace AccountBuddy.BLL
{
    public class TrialBalance:INotifyPropertyChanged
    {
        
        #region Fields

        private Ledger _Ledger;
        private decimal _CrAmt;
        private decimal _DrAmt;
        private decimal _CrAmtOP;
        private decimal _DrAmtOP;

        private string _AccountName;
        private static UserTypeDetail _UserPermission;
        #endregion

        #region Property
        public static UserTypeDetail UserPermission
        {
            get
            {
                if (_UserPermission == null)
                {
                    _UserPermission = UserAccount.User.UserType == null ? new UserTypeDetail() : UserAccount.User.UserType.UserTypeDetails.Where(x => x.UserTypeFormDetail.FormName == Forms.frmTrialBalance.ToString()).FirstOrDefault();
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
        public Ledger Ledger
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
                    NotifyPropertyChanged(nameof(Ledger));
                }
            }
        }
        public decimal CrAmt
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
        public decimal DrAmt
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

        public decimal CrAmtOP
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
        public decimal DrAmtOP
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

        public string AccountName
        {
            get
            {
                return _AccountName;
            }
            set
            {
                if (_AccountName != value)
                {
                    _AccountName = value;
                    NotifyPropertyChanged(nameof(AccountName));
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

        public static List<TrialBalance> ToList(DateTime dtFrom, DateTime dtTo)
        {
            return FMCGHubClient.FMCGHub.Invoke<List<TrialBalance>>("TrialBalance_List",dtFrom,dtTo).Result;        
        }
        public static decimal GetLedgerBalance(int LedgerId, DateTime dt)
        {
            return FMCGHubClient.FMCGHub.Invoke<decimal>("TrialBalance_GetLedgerBalance", LedgerId, dt).Result;
        }
        #endregion

    }
}
