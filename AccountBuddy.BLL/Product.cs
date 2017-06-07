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
    public class Products : INotifyPropertyChanged
    {
        #region Fileds

        private static ObservableCollection<Products> _toList;
       
        private int _Id;
        private string _ProductName;
        private Ledger _Ledger;
        private int? _LedgerId;
        private StockGroup _StockGroup;
        private int? _StockGroupId;
        private string _ItemCode;
        private int? _UOMId;
        private UOM _UOM;
        private decimal? _PurchaseRate;
        private decimal? _SellingRate;
        private decimal? _MRP;
        private double? _GST;
        private double? _OpeningStock;
        private double? _ReOrderLevel;

        private byte[] _ProductImage;

        private string _UOMName;
        private string _AccountGroupName;

        private static UserTypeDetail _UserPermission;
        private bool _IsReadOnly;
        private bool _IsEnabled;

        #endregion

        #region Property
        public static UserTypeDetail UserPermission
        {
            get
            {
                if (_UserPermission == null)
                {
                    _UserPermission = UserAccount.User.UserType == null ? new UserTypeDetail() : UserAccount.User.UserType.UserTypeDetails.Where(x => x.UserTypeFormDetail.FormName == AppLib.Forms.frmProducts.ToString()).FirstOrDefault();
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

        public static ObservableCollection<Products> toList
        {
            get
            {
                if (_toList == null) _toList = new ObservableCollection<Products>(FMCGHubClient.FMCGHub.Invoke<List<Products>>("Products_List").Result);
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
        public int? LedgerId
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
        public int? StockGroupId
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
        public int? UOMId
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
        public decimal? PurchaseRate
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
        public decimal? SellingRate
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
        public decimal? MRP
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
        public double? GST
        {
            get
            {
                return _GST;
            }

            set
            {
                if (_GST != value)
                {
                    _GST = value;
                    NotifyPropertyChanged(nameof(GST));
                }
            }
        }
        public double? OpeningStock
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
                }
            }
        }
        public double? ReOrderLevel
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
        public Ledger Ledger
        {
            get
            {
                if (_Ledger == null) _Ledger = new Ledger();
                return _Ledger;
            }
            set
            {
                if(_Ledger!=value)
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

        #region Methods

        public bool Save(bool isServerCall = false)
        {
            if (!isValid()) return false;
            try
            {

                Products d = toList.Where(x => x.Id == Id).FirstOrDefault();

                if (d == null)
                {
                    d = new Products();
                    toList.Add(d);
                }

                this.toCopy<Products>(d);
                if (isServerCall == false)
                {
                    var i = FMCGHubClient.FMCGHub.Invoke<int>("Products_Save", this).Result;
                    d.Id = i;
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;

            }

        }

        public void Clear()
        {
            new Products().toCopy<Products>(this);
            NotifyAllPropertyChanged();
        }

        public bool Find(int pk)
        {
            var d = toList.Where(x => x.Id == pk).FirstOrDefault();
            if (d != null)
            {
                d.toCopy<Products>(this);
                IsReadOnly = !UserPermission.AllowUpdate;

                return true;
            }

            return false;
        }

        public bool Delete(bool isServerCall = false)
        {
            var rv = false;
            var d = toList.Where(x => x.Id == Id).FirstOrDefault();
            if (d != null)
            {

                if (isServerCall == false)
                {
                    rv = FMCGHubClient.FMCGHub.Invoke<bool>("Products_Delete", this.Id).Result;
                    if (rv == true) toList.Remove(d);

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
