using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBuddy.BLL
{
    public class Purchase : INotifyPropertyChanged
    {
        #region Fields
        private int _Id;
        private DateTime _PurchaseDate;
        private string _RefNo;
        private string _InvoiceNo;
        private int _SupplierId;
        private int _TransactionTypeId;
        private decimal _ItemAmount;
        private decimal _DiscountAmount;
        private decimal _GSTAmount;
        private decimal _ExtraAmount;
        private decimal _TotalAmount;
        private string _Narration;

        private Supplier _Supplier;
        private TransactionType _TransactionType;

        #endregion

        #region Property

        public int Id
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

        public DateTime PurchaseDate
        {
            get
            {
                return _PurchaseDate;
            }
            set
            {
                if (_PurchaseDate != value)
                {
                    _PurchaseDate = value;
                    NotifyPropertyChanged(nameof(PurchaseDate));
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

        public string InvoiceNo
        {
            get
            {
                return _InvoiceNo;
            }
            set
            {
                if (_InvoiceNo != value)
                {
                    _InvoiceNo = value;
                    NotifyPropertyChanged(nameof(InvoiceNo));
                }
            }
        }
        public int SupplierId
        {
            get
            {
                return _SupplierId;
            }
            set
            {
                if (_SupplierId != value)
                {
                    _SupplierId = value;
                    NotifyPropertyChanged(nameof(SupplierId));
                }
            }
        }

        public int TransactionTypeId
        {
            get
            {
                return _TransactionTypeId;
            }
            set
            {
                if (_TransactionTypeId != value)
                {
                    _TransactionTypeId = value;
                    NotifyPropertyChanged(nameof(_TransactionTypeId));
                }
            }

        }

        public decimal ItemAmount
        {
            get
            {
                return _ItemAmount;
            }
            set
            {
                if (_ItemAmount != value)
                {
                    _ItemAmount = value;
                    NotifyPropertyChanged(nameof(ItemAmount));
                }
            }
        }

        public decimal DiscountAmount
        {
            get
            {
                return _DiscountAmount;
            }
            set
            {
                if (_DiscountAmount != value)
                {
                    _DiscountAmount = value;
                    NotifyPropertyChanged(nameof(DiscountAmount));
                }
            }
        }

        public decimal GSTAmount
        {
            get
            {
                return _GSTAmount;
            }
            set
            {
                if (_GSTAmount != value)
                {
                    _GSTAmount = value;
                    NotifyPropertyChanged(nameof(GSTAmount));
                }
            }
        }

        public decimal ExtraAmount
        {
            get
            {
                return _ExtraAmount;
            }
            set
            {
                if (_ExtraAmount != value)
                {
                    _ExtraAmount = value;
                    NotifyPropertyChanged(nameof(ExtraAmount));
                }
            }
        }

        public decimal TotalAmount
        {
            get
            {
                return _TotalAmount;
            }
            set
            {
                if(_TotalAmount!=value)
                {
                    _TotalAmount = value;
                    NotifyPropertyChanged(nameof(TotalAmount));

                }
            }
        }

        public string Narration
        {
            get
            {
                return _Narration;
            }
            set
            {
                if(_Narration!=value)
                {
                    _Narration = value;
                    NotifyPropertyChanged(nameof(_Narration));
                }
            }
        }

        public Supplier Supplier
        {
            get
            {
                return _Supplier;
            }
            set
            {
                if(_Supplier!=value)
                {
                    _Supplier = value;
                    NotifyPropertyChanged(nameof(Supplier));
                }
            }
        }

        public TransactionType TransactionType
        {
            get
            {
                return _TransactionType;
            }
            set
            {
                if(_TransactionType!=value)
                {
                    _TransactionType = value;
                    NotifyPropertyChanged(nameof(TransactionType));
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
