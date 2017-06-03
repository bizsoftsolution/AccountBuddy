using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBuddy.BLL
{
    public class PurchaseReturnDetail : INotifyPropertyChanged
    {
        #region Fields
        private int _Id;
        private int _PRId;
        private int _PDId;
        private int _ProductId;
        private int _UOMId;
        private decimal _Quantity;
        private decimal _UnitPrice;
        private decimal _DiscountAmount;
        private decimal _GSTAmount;
        private decimal _Amount;
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
        public int PRId
        {
            get
            {
                return _PRId;
            }
            set
            {
                if (_PRId != value)
                {
                    _PRId = value;
                    NotifyPropertyChanged(nameof(PRId));
                }
            }
        }

        public int PDId
        {
            get
            {
                return _PDId;
            }
            set
            {
                if (_PDId != value)
                {
                    _PDId = value;
                    NotifyPropertyChanged(nameof(PDId));
                }
            }
        }

        public int ProductId
        {
            get
            {
                return _ProductId;
            }
            set
            {
                if (_ProductId != value)
                {

                    _ProductId = value;
                    NotifyPropertyChanged(nameof(ProductId));
                }
            }
        }

        public int UOMId
        {
            get
            {
                return _UOMId;
            }
            set
            {
                if (_UOMId != value)
                {
                    _UOMId = value;
                    NotifyPropertyChanged(nameof(UOMId));
                }
            }
        }

        public decimal Quantity
        {
            get
            {
                return _Quantity;
            }
            set
            {
                if (_Quantity != value)
                {
                    _Quantity = value;
                    NotifyPropertyChanged(nameof(Quantity));
                }
            }
        }

        public decimal UnitPrice
        {
            get
            {
                return _UnitPrice;
            }
            set
            {
                if (_UnitPrice != value)
                {
                    _UnitPrice = value;
                    NotifyPropertyChanged(nameof(UnitPrice));
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
