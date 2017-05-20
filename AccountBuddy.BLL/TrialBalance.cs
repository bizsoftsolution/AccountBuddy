using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBuddy.BLL
{
    public class TrialBalance:INotifyPropertyChanged
    {
        #region Fields

        private string _LedgerName;
        private string _GroupName;
        private decimal? _CrAmt;
        private decimal? _DrAmt;

        private DateTime _VoucherPayDate;
        private DateTime _VoucherRecDate;
        #endregion

        #region Property
        public string LedgerName
        {
            get
            {
                return _LedgerName;
            }
            set
            {
                if (_LedgerName != value)
                {
                    _LedgerName = value;
                    NotifyPropertyChanged(nameof(LedgerName));
                }
            }
        }
        public string GroupName
        {
            get
            {
                return _GroupName;
            }
            set
            {
                if (_GroupName != value)
                {
                    _GroupName = value;
                    NotifyPropertyChanged(nameof(GroupName));
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
        public DateTime VoucherPayDate
        {
           get
            {
                return _VoucherPayDate;
            }
            set
            {
                if(_VoucherPayDate != value)
                {
                    _VoucherPayDate = value;
                    NotifyPropertyChanged(nameof(VoucherPayDate));
                }
            }
        }
        public DateTime VoucherRecDate
        {
            get
            {
                return _VoucherRecDate;
            }
            set
            {
                if (_VoucherRecDate != value)
                {
                    _VoucherRecDate = value;
                    NotifyPropertyChanged(nameof(VoucherRecDate));
                }
            }
        }
        private static List<TrialBalance> _toList;

        #endregion

        public static List<TrialBalance> toList
        {
            get
            {
                if (_toList == null)
                {
                    _toList = ABClientHub.FMCGHub.Invoke<List<TrialBalance>>("TrialBalance_List").Result;
                }

                return _toList;
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

        #endregion

    }
}
