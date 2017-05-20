using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBuddy.BLL
{
    public class ReceiptDetail : INotifyPropertyChanged
    {
        #region Fields
        private long _Id;
        private long _ReceipttId;
        private int _LedgerId;
        private decimal _Amount;
        private string _Particular;

        private string _LedgerName;

        #endregion

        #region Property
        public long Id
        {
            get
            {
                return _Id;
            }
            set
            {
                if (_Id != value)
                {
                    _Id = value;
                    NotifyPropertyChanged(nameof(Id));
                }
            }
        }
        public long ReceiptId
        {
            get
            {
                return _ReceipttId;
            }
            set
            {
                if (_ReceipttId != value)
                {
                    _ReceipttId = value;
                    NotifyPropertyChanged(nameof(ReceiptId));
                }
            }
        }
        public int LedgerId
        {
            get
            {
                return _LedgerId;
            }
            set
            {
                if (_LedgerId != value)
                {
                    _LedgerId = value;
                    NotifyPropertyChanged(nameof(LedgerId));
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
        public string Particular
        {
            get
            {
                return _Particular;
            }
            set
            {
                if (_Particular != value)
                {
                    _Particular = value;
                    NotifyPropertyChanged(nameof(Particular));
                }
            }
        }
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
    }
}
