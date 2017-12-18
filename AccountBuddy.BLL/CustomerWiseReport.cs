using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountBuddy.Common;

namespace AccountBuddy.BLL
{
    public class CustomerWiseReport : INotifyPropertyChanged
    {
        #region Fields
        private string _ProductName;
        private string _CustomerName;
        private Ledger _Ledger;
        private string _Month1;
        private string _Month2;
        private string _Month3;
        private string _Month4;
        private string _Month5;
        private string _Month6;
        private decimal _M1;
        private decimal _M2;
        private decimal _M3;
        private decimal _M4;
        private decimal _M5;
        private decimal _M6;
        private decimal? _Amount;
        private static UserTypeDetail _UserPermission;

        #endregion

        #region Property
        public static UserTypeDetail UserPermission
        {
            get
            {
                if (_UserPermission == null)
                {
                    _UserPermission = UserAccount.User.UserType == null ? new UserTypeDetail() : UserAccount.User.UserType.UserTypeDetails.Where(x => x.UserTypeFormDetail.FormName == Forms.frmCustomerSalesReport.ToString()).FirstOrDefault();
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

        public string ProductName
        {
            get
            {
                return _ProductName;
            }
            set
            {
                if (_ProductName != value)
                {
                    _ProductName = value;
                    NotifyPropertyChanged(nameof(ProductName));
                }
            }
        }

        public string CustomerName
        {
            get
            {
                return _CustomerName;
            }
            set
            {
                if (_CustomerName != value)
                {
                    _CustomerName = value;
                    NotifyPropertyChanged(nameof(CustomerName));
                }
            }
        }
        public decimal? Amount
        {
            get
            {
                return _Amount;
            }
            set
            {
                if (_Amount != value)
                {
                    _Amount = value;
                    NotifyPropertyChanged(nameof(Amount));
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
        public string Month1
        {
            get
            {
                return _Month1;
            }
            set
            {
                if (_Month1 != value)
                {
                    _Month1 = value;
                    NotifyPropertyChanged(nameof(Month1));
                }
            }
        }
        public string Month2
        {
            get
            {
                return _Month2;
            }
            set
            {
                if (_Month2 != value)
                {
                    _Month2 = value;
                    NotifyPropertyChanged(nameof(Month2));
                }
            }
        }
        public string Month3
        {
            get
            {
                return _Month3;
            }
            set
            {
                if (_Month3 != value)
                {
                    _Month3 = value;
                    NotifyPropertyChanged(nameof(Month3));
                }
            }
        }
        public string Month4
        {
            get
            {
                return _Month4;
            }
            set
            {
                if (_Month4 != value)
                {
                    _Month4 = value;
                    NotifyPropertyChanged(nameof(Month4));
                }
            }
        }
        public string Month5
        {
            get
            {
                return _Month5;
            }
            set
            {
                if (_Month5 != value)
                {
                    _Month5 = value;
                    NotifyPropertyChanged(nameof(Month5));
                }
            }
        }
        public string Month6
        {
            get
            {
                return _Month6;
            }
            set
            {
                if (_Month6 != value)
                {
                    _Month6 = value;
                    NotifyPropertyChanged(nameof(Month6));
                }
            }
        }
        public decimal M1
        {
            get
            {
                return _M1;
            }
            set
            {
                if (_M1 != value)
                {
                    _M1 = value;
                    NotifyPropertyChanged(nameof(M1));
                }
            }
        }
        public decimal M2
        {
            get
            {
                return _M2;
            }
            set
            {
                if (_M2 != value)
                {
                    _M2 = value;
                    NotifyPropertyChanged(nameof(M2));
                }
            }
        }
        public decimal M3
        {
            get
            {
                return _M3;
            }
            set
            {
                if (_M3 != value)
                {
                    _M3 = value;
                    NotifyPropertyChanged(nameof(M3));
                }
            }
        }
        public decimal M4
        {
            get
            {
                return _M4;
            }
            set
            {
                if (_M4 != value)
                {
                    _M4 = value;
                    NotifyPropertyChanged(nameof(M4));
                }
            }
        }
        public decimal M5
        {
            get
            {
                return _M5;
            }
            set
            {
                if (_M5 != value)
                {
                    _M5 = value;
                    NotifyPropertyChanged(nameof(M5));
                }
            }
        }
        public decimal M6
        {
            get
            {
                return _M6;
            }
            set
            {
                if (_M6 != value)
                {
                    _M6 = value;
                    NotifyPropertyChanged(nameof(M6));
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

        public static List<CustomerWiseReport> ToList(int LedgerId, DateTime dtFrom)
        {
            return FMCGHubClient.HubCaller.Invoke<List<CustomerWiseReport>>("CustomerWiseReport_List", LedgerId, dtFrom).Result;
        }

        #endregion
    }
}
