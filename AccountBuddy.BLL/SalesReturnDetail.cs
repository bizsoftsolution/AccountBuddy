using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace AccountBuddy.BLL
{
    public class SalesReturnDetail : INotifyPropertyChanged
    {
        #region Field
        private long _Id;
        private long _SRId;
        private long? _SDId;
        private int _ProductId;
        private int _UOMId;
        private double _Quantity;
        private decimal _UnitPrice;
        private decimal _DiscountAmount;
        private decimal _GSTAmount;
        private decimal _Amount;

        private string _ItemCode;
        private string _ProductName;
        private string _UOMName;
        private object _Particulars;
        private bool _IsResale;
        private Product _Product;
        private int _SNo;
        #endregion

        #region Property
        public Product Product
        {
            get
            {
                return _Product;
            }
            set
            {
                if (_Product != value)
                {
                    _Product = value;
                    SetProduct();

                    NotifyPropertyChanged(nameof(Product));
                }
            }
        }
        public int SNo
        {
            get
            {
                return _SNo;
            }
            set
            {
                if (_SNo != value)
                {
                    _SNo = value;
                    NotifyPropertyChanged(nameof(SNo));
                }
            }
        }
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
        public long SRId
        {
            get
            {
                return _SRId;
            }
            set
            {
                if (_SRId != value)
                {
                    _SRId = value;
                    NotifyPropertyChanged(nameof(SRId));
                }
            }
        }
        public long? SDId
        {
            get
            {
                return _SDId;
            }
            set
            {
                if (_SDId != value)
                {
                    _SDId = value;
                    NotifyPropertyChanged(nameof(SDId));
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
        public double Quantity
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
                    Amount = Convert.ToDecimal(_Quantity) * _UnitPrice - DiscountAmount;
                  
                    NotifyPropertyChanged(nameof(Quantity));
                }
            }
        }

        private void SetDiscount()
        {
            var p = Product ?? new Product();
            DiscountAmount = p.DiscountAmount * (decimal)Quantity;
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
                    Amount = Convert.ToDecimal(_Quantity) * _UnitPrice - DiscountAmount;
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
                    Amount = Convert.ToDecimal(_Quantity) * _UnitPrice - DiscountAmount;
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
        public string ItemCode
        {
            get
            {
                return _ItemCode;
            }
            set
            {
                if (_ItemCode != value)
                {
                    _ItemCode = value;
                    if (value != null) SetProductbyItemCode();

                    NotifyPropertyChanged(nameof(ItemCode));
                }
            }
        }

        private void SetProductbyItemCode()
        {
            var p = Product ?? new Product();
            ItemCode = p.ItemCode;
            ProductId = p.Id;
            UOMId = p.UOMId;
            ProductName = p.ProductName;
            UnitPrice = p.SellingRate;
            Quantity = p.Id != 0 ? 1 : 0;

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
        public string UOMName
        {
            get
            {
                return _UOMName;
            }
            set
            {
                if (_UOMName != value)
                {
                    _UOMName = value;
                    NotifyPropertyChanged(nameof(UOMName));
                }
            }
        }

        public object Particulars
        {
            get
            {
                return _Particulars;
            }
            set
            {
                if (_Particulars != value)
                {
                    _Particulars = value;
                    NotifyPropertyChanged(nameof(Particulars));
                }
            }
        }
        public bool IsResale
        {
            get
            {
                return _IsResale;
            }
            set
            {
                if (_IsResale != value)
                {
                    _IsResale = value;
                    NotifyPropertyChanged(nameof(IsResale));
                }
            }
        }
        #endregion

        #region Property Changed
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String ProperName)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(ProperName));
        }
        private void NotifyAllPropertyChanged()
        {
            foreach (var p in this.GetType().GetProperties()) NotifyPropertyChanged(p.Name);
        }

        #endregion

        #region Methods
        private void SetProduct()
        {
            var p = Product ?? new Product();
            ProductId = p.Id;
            UOMId = p.UOMId;
            ProductName = p.ProductName;
            UnitPrice = p.SellingRate;
            Quantity = p.Id != 0 ? 1 : 0;
           // DiscountAmount = p.DiscountAmount;
        }
        #endregion
    }
}
