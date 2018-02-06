using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBuddy.BLL
{
    public class SalesReportNew : INotifyPropertyChanged
    {
        #region Fields
        private string _CustomerName;
        private string _ProductName;
        private decimal _Amount;
        private string _Month;
        private string _Description;

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
        public decimal Amount
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
        public string Month
        {
            get
            {
                return _Month;
            }
            set
            {
                if (_Month != value)
                {
                    _Month = value;
                    NotifyPropertyChanged(nameof(Month));
                }
            }
        }
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

        public static List<SalesReportNew> ToList(DateTime dtFrom, DateTime dtTo, bool isMonthly, string ReportType)
        {
            return FMCGHubClient.HubCaller.Invoke<List<SalesReportNew>>("SalesReportNew_ToList", dtFrom, dtTo, isMonthly, ReportType).Result;
        }
        public static List<SalesReport> ToListCustomerWise(DateTime dtFrom, DateTime dtTo, bool isMonthly, string ReportType)
        {
            return FMCGHubClient.HubCaller.Invoke<List<SalesReport>>("SalesReport_ListCustomerWise", dtFrom, dtTo, isMonthly, ReportType).Result;
        }
        //public static List<SalesReport> ToListCustomerWise(DateTime dtFrom)
        //{
        //    return FMCGHubClient.FMCGHub.Invoke<List<SalesReport>>("SalesReport_ListCustomerWise", dtFrom).Result;
        //}
        public static List<SalesReport> ToListProductWise(DateTime dtFrom)
        {
            return FMCGHubClient.HubCaller.Invoke<List<SalesReport>>("SalesReport_ListProductWise", dtFrom).Result;
        }

        #endregion


    }
}
