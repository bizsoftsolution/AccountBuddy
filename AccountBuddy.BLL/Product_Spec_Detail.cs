using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBuddy.BLL
{
    public class Product_Spec_Detail : INotifyPropertyChanged
    {

        #region Field
        private long _Id;
        private int _ProductId;
        private int _Product_Spec_Id;
        private int _Qty;
        private int _SNo;
        private string _ProductName;
        private Product _Product;
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
        public int Product_Spec_Id
        {
            get
            {
                return _Product_Spec_Id;
            }
            set
            {
                if (_Product_Spec_Id != value)
                {
                    _Product_Spec_Id = value;
                    NotifyPropertyChanged(nameof(Product_Spec_Id));
                }
            }
        }
        public int Qty
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
                    NotifyPropertyChanged(nameof(Qty));
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
        private void SetProduct()
        {
            var p = Product ?? new Product();
            ProductId = p.Id;

            ProductName = p.ProductName;
            Qty = p.Id != 0 ? 1 : 0;
            //  DiscountAmount = p.DiscountAmount;
        }
    }

}
