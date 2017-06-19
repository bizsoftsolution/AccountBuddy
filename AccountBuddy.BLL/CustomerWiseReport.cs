using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBuddy.BLL
{
    public class CustomerWiseReport : INotifyPropertyChanged
    {
        #region Fields
        private string _ProductName;
        private string _CustomerName;
        private Ledger _Ledger;
        private decimal _Month1;
        private decimal _Month2;
        private decimal _Month3;
        private decimal _Month4;
        private decimal _Month5;
        private decimal _Month6;

        #endregion

        #region Property

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
        public decimal Month1
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
        public decimal Month2
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
        public decimal Month3
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

        public decimal Month4
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

        public decimal Month5
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
        public decimal Month6
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

        //public static List<Customer> ToList(int LedgerId, DateTime dtFrom, DateTime dtTo)
        //{
        //    return FMCGHubClient.FMCGHub.Invoke<List<GeneralLedger>>("GeneralLedger_List", LedgerId, dtFrom, dtTo).Result;
        //}

        #endregion
    }
}
