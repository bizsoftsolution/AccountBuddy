using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBuddy.BLL
{
   public class Purchase_Order_TaxDetail : INotifyPropertyChanged
    {
        #region Fields
        private long _Id;
        private long _PO_ID;
        private int _TaxId;
        private decimal _TaxAmount;
        private decimal _TaxPercentage;
        private string _TaxName;
        private TaxMaster _TaxMaster;
        private Ledger _Ledger;
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
        public long PO_ID
        {
            get
            {
                return _PO_ID;
            }
            set
            {
                if (_PO_ID != value)
                {
                    _PO_ID = value;
                    NotifyPropertyChanged(nameof(PO_ID));
                }
            }
        }
        public int TaxId
        {
            get
            {
                return _TaxId;
            }
            set
            {
                if (_TaxId != value)
                {
                    _TaxId = value;
                    NotifyPropertyChanged(nameof(TaxId));
                }
            }
        }
        public decimal TaxAmount
        {
            get
            {
                return _TaxAmount;
            }
            set
            {
                if (_TaxAmount != value)
                {
                    _TaxAmount = value;
                    NotifyPropertyChanged(nameof(TaxAmount));
                }
            }
        }
        public decimal TaxPercentage
        {
            get
            {
                return _TaxPercentage;
            }
            set
            {
                if (_TaxPercentage != value)
                {
                    _TaxPercentage = value;
                    NotifyPropertyChanged(nameof(TaxPercentage));
                }
            }
        }
        public string TaxName
        {
            get
            {
                return _TaxName;
            }
            set
            {
                if (_TaxName != value)
                {
                    _TaxName = value;
                    NotifyPropertyChanged(nameof(TaxName));
                }
            }
        }
        public TaxMaster TaxMaster
        {
            get
            {
                return _TaxMaster;
            }
            set
            {
                if (_TaxMaster != value)
                {
                    _TaxMaster = value;
                    NotifyPropertyChanged(nameof(TaxMaster));
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
