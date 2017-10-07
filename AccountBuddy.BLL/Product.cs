using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountBuddy.Common;

namespace AccountBuddy.BLL
{
    public class Product : INotifyPropertyChanged
    {
        #region Constructor

        public Product()
        {

        }

        public Product(int ProductId)
        {
            Product p = toList.Where(x => x.Id == ProductId).FirstOrDefault();
            if (p == null) p = new Product();
            p.toCopy<Product>(this);
        }

        public Product(string ProductCode)
        {
            Product p = toList.Where(x => x.ItemCode.ToLower() == ProductCode.ToLower()).FirstOrDefault();
            if (p == null) p = new Product();
            p.toCopy<Product>(this);
        }
        public double? POQty
        {
            get
            {
                return _POQty;
            }
            set
            {
                if (_POQty != value)
                {
                    _POQty = value;
                    NotifyPropertyChanged(nameof(POQty));
                }

            }
        }
        public double? SOQty
        {
            get
            {
                return _SOQty;
            }
            set
            {
                if (_SOQty != value)
                {
                    _SOQty = value;
                    NotifyPropertyChanged(nameof(SOQty));
                }

            }
        }

        public double? PQty
        {
            get
            {
                return _PQty;
            }
            set
            {
                if (_PQty != value)
                {
                    _PQty = value;
                    NotifyPropertyChanged(nameof(PQty));
                    NotifyPropertyChanged(nameof(AvailableStock));
                }

            }
        }
        public double? SRQty
        {
            get
            {
                return _SRQty;
            }
            set
            {
                if (_SRQty != value)
                {
                    _SRQty = value;
                    NotifyPropertyChanged(nameof(SRQty));
                    NotifyPropertyChanged(nameof(AvailableStock));
                }

            }
        }

        public double? PRQty
        {
            get
            {
                return _PRQty;
            }
            set
            {
                if (_PRQty != value)
                {
                    _PRQty = value;
                    NotifyPropertyChanged(nameof(PRQty));
                    NotifyPropertyChanged(nameof(AvailableStock));
                }

            }
        }
        public double? SQty
        {
            get
            {
                return _SQty;
            }
            set
            {
                if (_SQty != value)
                {
                    _SQty = value;
                    NotifyPropertyChanged(nameof(SQty));
                    NotifyPropertyChanged(nameof(AvailableStock));
                }

            }
        }

        public double AvailableStock
        {
            get
            {
                return (OpeningStock + (PQty ?? 0) + (SRQty ?? 0) + (SInQty ?? 0) + (JRQty ?? 0) + (SSQty ?? 0)) - ((SQty ?? 0) + (PRQty ?? 0) + (SOutQty ?? 0) + (JOQty ?? 0) + (SPQty ?? 0));
            }
        }
        public double StockLeftForSales
        {
            get
            {
                return (OpeningStock + (PQty ?? 0) + (SRQtyForSales ?? 0) + (SInQty ?? 0) + (JRQty ?? 0) + (SSQty ?? 0)) - ((SQty ?? 0) + (PRQty ?? 0) + (SOutQty ?? 0) + (JOQty ?? 0) + (SPQty ?? 0));
            }
        }
        public double StockLeftNotForSales
        {
            get
            {
                return (double)(SRQtyNotForSales ?? 0);
            }
        }
        public bool IsReOrderLevel
        {
            get
            {
                return ReOrderLevel > AvailableStock;
            }
        }



        public double? SInQty
        {
            get
            {
                return _SInQty;
            }
            set
            {
                if (_SInQty != value)
                {
                    _SInQty = value;
                    NotifyPropertyChanged(nameof(SInQty));
                    NotifyPropertyChanged(nameof(AvailableStock));
                }

            }
        }
        public double? SOutQty
        {
            get
            {
                return _SOutQty;
            }
            set
            {
                if (_SOutQty != value)
                {
                    _SOutQty = value;
                    NotifyPropertyChanged(nameof(SOutQty));
                    NotifyPropertyChanged(nameof(AvailableStock));
                }

            }
        }

        public double? JOQty
        {
            get
            {
                return _JOQty;
            }
            set
            {
                if (_JOQty != value)
                {
                    _JOQty = value;
                    NotifyPropertyChanged(nameof(JOQty));
                    NotifyPropertyChanged(nameof(AvailableStock));
                }

            }
        }
        public double? JRQty
        {
            get
            {
                return _JRQty;
            }
            set
            {
                if (_JRQty != value)
                {
                    _JRQty = value;
                    NotifyPropertyChanged(nameof(JRQty));
                    NotifyPropertyChanged(nameof(AvailableStock));
                }

            }
        }

        public double? SPQty
        {
            get
            {
                return _SPQty;
            }
            set
            {
                if (_SPQty != value)
                {
                    _SPQty = value;
                    NotifyPropertyChanged(nameof(SPQty));
                    NotifyPropertyChanged(nameof(AvailableStock));
                }

            }
        }
        public double? SSQty
        {
            get
            {
                return _SSQty;
            }
            set
            {
                if (_SSQty != value)
                {
                    _SSQty = value;
                    NotifyPropertyChanged(nameof(SSQty));
                    NotifyPropertyChanged(nameof(AvailableStock));
                }

            }
        }
        public double? SRQtyForSales
        {
            get
            {
                return _SRQtyForSales;
            }
            set
            {
                if (_SRQtyForSales != value)
                {
                    _SRQtyForSales = value;
                    NotifyPropertyChanged(nameof(SRQtyForSales));
                    NotifyPropertyChanged(nameof(AvailableStock));
                }

            }
        }
        public double? SRQtyNotForSales
        {
            get
            {
                return _SRQtyNotForSales;
            }
            set
            {
                if (_SRQtyNotForSales != value)
                {
                    _SRQtyNotForSales = value;
                    NotifyPropertyChanged(nameof(SRQtyNotForSales));
                    NotifyPropertyChanged(nameof(AvailableStock));
                }

            }
        }
        #endregion


        #region Fileds

        private static ObservableCollection<Product> _toList;

        private int _Id;
        private string _ProductName;
        private StockGroup _StockGroup;
        private int _StockGroupId;
        private string _ItemCode;
        private int _UOMId;
        private UOM _UOM;
        private decimal _PurchaseRate;
        private decimal _SellingRate;
        private decimal _MaxSellingRate;
        private decimal _MinSellingRate;
        private decimal _MRP;
        private decimal _Discount;
        private double _OpeningStock;
        private double _ReOrderLevel;

        private byte[] _ProductImage;

        private string _UOMName;
        private string _AccountGroupName;

        private static UserTypeDetail _UserPermission;
        private bool _IsReadOnly;
        private bool _IsEnabled;
        private double? _SRQty;
        private double? _SQty;
        private double? _PRQty;
        private double? _SOQty;
        private double? _PQty;
        private double? _POQty;
        private double? _SInQty;
        private double? _SOutQty;
        private double? _JOQty;
        private double? _JRQty;
        private double? _SPQty;
        private double? _SSQty;


        private decimal _DiscountAmount;
        private double? _SRQtySales;
        private double? _SRQtyForSales;
        private double? _SRQtyNotForSales;

        #endregion

        #region Property
        public static UserTypeDetail UserPermission
        {
            get
            {
                if (_UserPermission == null)
                {
                    _UserPermission = UserAccount.User.UserType == null ? new UserTypeDetail() : UserAccount.User.UserType.UserTypeDetails.Where(x => x.UserTypeFormDetail.FormName == Forms.frmProducts.ToString()).FirstOrDefault();
                }
                return _UserPermission;
            }

            set
            {
                if (_UserPermission != value)
                {
                    _UserPermission = value;
                }
            }
        }


        public bool IsReadOnly
        {
            get
            {
                return _IsReadOnly;
            }

            set
            {
                if (_IsReadOnly != value)
                {
                    _IsReadOnly = value;
                    NotifyPropertyChanged(nameof(IsReadOnly));
                }
                IsEnabled = !value;
            }
        }

        public bool IsEnabled
        {
            get
            {
                return _IsEnabled;
            }

            set
            {
                if (_IsEnabled != value)
                {
                    _IsEnabled = value;
                    NotifyPropertyChanged(nameof(IsEnabled));
                }
            }
        }

        public static ObservableCollection<Product> toList
        {
            get
            {
                if (_toList == null) _toList = new ObservableCollection<Product>(FMCGHubClient.FMCGHub.Invoke<List<Product>>("Product_List").Result);
                return _toList;
            }
            set
            {
                _toList = value;
            }
        }

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
        public int StockGroupId
        {
            get
            {
                return _StockGroupId;
            }

            set
            {
                if (_StockGroupId != value)
                {
                    _StockGroupId = value;
                    NotifyPropertyChanged(nameof(StockGroupId));
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
                    NotifyPropertyChanged(nameof(ItemCode));
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
        public decimal PurchaseRate
        {
            get
            {
                return _PurchaseRate;
            }

            set
            {
                if (_PurchaseRate != value)
                {
                    _PurchaseRate = value;
                    NotifyPropertyChanged(nameof(PurchaseRate));
                }
            }
        }
        public decimal SellingRate
        {
            get
            {
                return _SellingRate;
            }
            set
            {
                if (_SellingRate != value)
                {
                    _SellingRate = value;
                    NotifyPropertyChanged(nameof(SellingRate));
                }
            }
        }
        public decimal MaxSellingRate
        {
            get
            {
                return _MaxSellingRate;
            }
            set
            {
                if (_MaxSellingRate != value)
                {
                    _MaxSellingRate = value;
                    NotifyPropertyChanged(nameof(MaxSellingRate));
                }
            }
        }
        public decimal MinSellingRate
        {
            get
            {
                return _MinSellingRate;
            }
            set
            {
                if (_MinSellingRate != value)
                {
                    _MinSellingRate = value;
                    NotifyPropertyChanged(nameof(MinSellingRate));
                }
            }
        }
        public decimal MRP
        {
            get
            {
                return _MRP;
            }

            set
            {
                if (_MRP != value)
                {
                    _MRP = value;
                    NotifyPropertyChanged(nameof(MRP));
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
        public double OpeningStock
        {
            get
            {
                return _OpeningStock;
            }

            set
            {
                if (_OpeningStock != value)
                {
                    _OpeningStock = value;
                    NotifyPropertyChanged(nameof(OpeningStock));
                    NotifyPropertyChanged(nameof(AvailableStock));
                }
            }
        }
        public double ReOrderLevel
        {
            get
            {
                return _ReOrderLevel;
            }

            set
            {
                if (_ReOrderLevel != value)
                {
                    _ReOrderLevel = value;
                    NotifyPropertyChanged(nameof(ReOrderLevel));
                }
            }
        }
        public byte[] ProductImage
        {
            get
            {
                return _ProductImage;
            }

            set
            {
                if (_ProductImage != value)
                {
                    _ProductImage = value;
                    NotifyPropertyChanged(nameof(ProductImage));
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
        public string StockGroupName
        {
            get
            {
                return _AccountGroupName;
            }

            set
            {
                if (_AccountGroupName != value)
                {
                    _AccountGroupName = value;
                    NotifyPropertyChanged(nameof(StockGroupName));
                }
            }
        }
        public UOM UOM
        {
            get
            {
                return _UOM;
            }
            set
            {
                if (_UOM != value)
                {
                    _UOM = value;
                    NotifyPropertyChanged(nameof(UOM));
                }
            }
        }
        public StockGroup StockGroup
        {
            get
            {
                if (_StockGroup == null) _StockGroup = new StockGroup();
                return _StockGroup;
            }
            set
            {
                if (_StockGroup != value)
                {
                    _StockGroup = value;
                    NotifyPropertyChanged(nameof(StockGroup));
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

        public bool Save(bool isServerCall = false)
        {
            if (!isValid()) return false;
            try
            {
                if (isServerCall == false)
                {
                    var d = FMCGHubClient.FMCGHub.Invoke<Product>("Product_Save", this).Result;
                    if (d.Id != 0)
                    {
                        if (Id == 0)
                        {
                            toList.Add(d);
                        }
                        else
                        {
                            var d1 = toList.Where(x => x.Id == d.Id).FirstOrDefault();
                            d.toCopy<Product>(d1);
                        }
                        return true;
                    }
                }
                else
                {
                    var d1 = toList.Where(x => x.Id == Id).FirstOrDefault();
                    if (d1 == null)
                    {
                        d1 = new Product();
                        toList.Add(d1);
                    }
                    this.toCopy<Product>(d1);
                }
            }
            catch (Exception ex) { }
            return false;
        }

        public void Clear()
        {
            new Product().toCopy<Product>(this);
            NotifyAllPropertyChanged();
        }

        public bool Find(int pk)
        {
            var d = toList.Where(x => x.Id == pk).FirstOrDefault();
            if (d != null)
            {
                d.toCopy<Product>(this);
                IsReadOnly = !UserPermission.AllowUpdate;

                return true;
            }

            return false;
        }

        public bool Delete(bool isServerCall = false)
        {
            var rv = false;
            var d = toList.Where(x => x.Id == Id).FirstOrDefault();
            var b = FMCGHubClient.FMCGHub.Invoke<bool>("Product_CanDeleteById", Id).Result;
            if (d != null && b == true)
            {

                if (isServerCall == false)
                {
                    rv = FMCGHubClient.FMCGHub.Invoke<bool>("Product_Delete", this.Id).Result;
                    if (rv == true)
                    {
                        toList.Remove(d);
                    }

                }
                else
                {
                    toList.Remove(d);
                }
                return rv;
            }

            return rv;
        }

        public bool isValid()
        {
            bool RValue = true;
            if (toList.Where(x => x.ProductName.ToLower() == ProductName.ToLower() && x.Id != Id).Count() > 0)
            {
                RValue = false;
            }
            else if (toList.Where(x => x.ItemCode.ToLower() == ItemCode.ToLower() && x.Id != Id).Count() > 0)
            {
                RValue = false;
            }
            return RValue;

        }

        public static void Init()
        {
            _toList = null;
        }

        #endregion


    }
}