using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBuddy.BLL
{
    public class SalesReport : INotifyPropertyChanged
    {
        #region Fields
        private string _Description;
        private decimal? _M1;
        private decimal? _M2;
        private decimal? _M3;
        private decimal? _M4;
        private decimal? _M5;
        private decimal? _M6;
        private decimal? _Amount;
        

        #endregion

        #region Property

        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                if (_Description != value)
                {
                    _Description = value;
                    NotifyPropertyChanged(nameof(Description));
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
     
        public decimal? M1
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
        public decimal? M2
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
        public decimal? M3
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
        public decimal? M4
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
        public decimal? M5
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
        public decimal? M6
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

        public static List<SalesReport> ToListCustomerWise(DateTime dtFrom)
        {
            return FMCGHubClient.FMCGHub.Invoke<List<SalesReport>>("SalesReport_ListCustomerWise", dtFrom).Result;
        }
        public static List<SalesReport> ToListProductWise(DateTime dtFrom)
        {
            return FMCGHubClient.FMCGHub.Invoke<List<SalesReport>>("SalesReport_ListProductWise", dtFrom).Result;
        }

        #endregion
    }
}
