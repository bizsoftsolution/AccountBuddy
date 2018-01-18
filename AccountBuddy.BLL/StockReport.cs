using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBuddy.BLL
{
    public class StockReport : INotifyPropertyChanged
    {

        private string _ProductName;
        private string _TransactionType;
        private double _Qty;
     
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

        public string TransactionType
        {
            get
            {

                return _TransactionType;
            }
            set
            {
                if(_TransactionType!= value)
                {
                    _TransactionType = value;
                    NotifyPropertyChanged(nameof(TransactionType));
                }
            }
        }

        public double Qty
        {
            get
            {

                return _Qty;
            }
            set
            {
                if (_Qty != value)
                {
                    _Qty = value;
                    NotifyPropertyChanged(nameof (Qty));
                }
            }
        }
      

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

        public static List<StockReport> List(int? PID, DateTime dateFrom, DateTime dtTo)
        {
            return FMCGHubClient.HubCaller.Invoke<List<StockReport>>("StockReport_List", PID, dateFrom, dtTo).Result;
        }
      
        #endregion

    }
}
