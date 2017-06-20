﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBuddy.BLL
{
    public class StockOutDetail : INotifyPropertyChanged
    {
        #region Field
        private long _Id;
        private long _StockOutId;
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

        public long StockOutId
        {
            get
            {
                return _StockOutId;
            }
            set
            {
                if (_StockOutId != value)
                {
                    _StockOutId = value;
                    NotifyPropertyChanged(nameof(StockOutId));
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
                    if (value != 0) SetProduct(new Product(_ProductId));
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
                    if (_ProductId != 0) SetDiscount(new Product(_ProductId));
                    Amount = Convert.ToDecimal(_Quantity) * _UnitPrice - DiscountAmount;
                    NotifyPropertyChanged(nameof(UnitPrice));
                }
            }
        }

        private void SetDiscount(Product p)
        {
            DiscountAmount = p.DiscountAmount * (decimal)Quantity;
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
                    if (value != null) SetProductbyItemCode(new Product(_ItemCode.ToLower()));

                    NotifyPropertyChanged(nameof(ItemCode));
                }
            }
        }

        private void SetProductbyItemCode(Product p)
        {
            UOMId = p.UOMId;
            ProductName = p.ProductName;
            UnitPrice = p.SellingRate;
            Quantity = p.Id != 0 ? 1 : 0;
            DiscountAmount = p.DiscountAmount;

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
        private void SetProduct(Product p)
        {
            UOMId = p.UOMId;
            ProductName = p.ProductName;
            UnitPrice = p.SellingRate;
            Quantity = p.Id != 0 ? 1 : 0;
            DiscountAmount = p.DiscountAmount;
        }
        #endregion
    }
}
