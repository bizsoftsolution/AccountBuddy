using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBuddy.BLL
{
    public class ReceiptAndPayment : INotifyPropertyChanged
    {
        #region Fields
        private long _EId;
        private Char _EType;
        private DateTime? _EDate;
        private String _EntryNo;
        private string _RefNo;
        private Ledger _Ledger;
        private decimal _Amount;
        private string _AccountName;

        #endregion

        #region Property

        public long EId
        {
            get
            {
                return _EId;
            }
            set
            {
                if (_EId != value)
                {
                    _EId = value;
                    NotifyPropertyChanged(nameof(EId));
                }
            }
        }
        public char EType
        {
            get
            {
                return _EType;
            }
            set
            {
                if (_EType != value)
                {
                    _EType = value;
                    NotifyPropertyChanged(nameof(EType));
                }
            }
        }
        public DateTime? EDate
        {
            get
            {
                return _EDate;
            }
            set
            {
                if (_EDate != value)
                {
                    _EDate = value;
                    NotifyPropertyChanged(nameof(EDate));
                }
            }
        }
        public string RefNo
        {
            get
            {
                return _RefNo;
            }
            set
            {
                if (_RefNo != value)
                {
                    _RefNo = value;
                    NotifyPropertyChanged(nameof(RefNo));
                }
            }
        }
        public string EntryNo
        {
            get
            {
                return _EntryNo;
            }
            set
            {
                if (_EntryNo != value)
                {
                    _EntryNo = value;
                    NotifyPropertyChanged(nameof(EntryNo));
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

        public static List<ReceiptAndPayment> ToList(int? LedgerId, DateTime dtFrom, DateTime dtTo)
        {
            return ABClientHub.FMCGHub.Invoke<List<ReceiptAndPayment>>("ReceiptAndPayment_List", LedgerId, dtFrom, dtTo).Result;
        }

        #endregion
    }
}
