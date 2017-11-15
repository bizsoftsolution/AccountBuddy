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
   public class StockGroup : INotifyPropertyChanged
    {
        #region Fields
        private static ObservableCollection<StockGroup> _toList;
        private static ObservableCollection<StockGroup> _StocktoList;

        private int _id;
        private string _StockGroupName;
        private string _StockGroupNameWithCode;
        private string _groupCode;
        private int? _underGroupId;
        private int _companyId;
        private StockGroup _UnderStockGroup;
        private CompanyDetail _Company;
        private string _underStockGroupName;
        private List<StockGroup> _SubStockGroup;
        private bool _isPurchase;
        private bool _isSale;

        private static UserTypeDetail _UserPermission;
        private bool _IsReadOnly;
        private bool _IsEnabled;
        #endregion

        #region Property

        public string AccountPath
        {
            get
            {
                return UnderStockGroup == null ? "" : UnderStockGroup.AccountPath + "/" + _StockGroupName;
            }
        }
        public static UserTypeDetail UserPermission
        {
            get
            {
                if (_UserPermission == null)
                {
                    _UserPermission = UserAccount.User.UserType == null ? new UserTypeDetail() : UserAccount.User.UserType.UserTypeDetails.Where(x => x.UserTypeFormDetail.FormName == Forms.frmStockGroup.ToString()).FirstOrDefault();
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

        public static ObservableCollection<StockGroup> toList
        {
            get
            {
                try
                {
                    if (_toList == null)
                    {
                        _toList = new ObservableCollection<StockGroup>();
                        var l1 = FMCGHubClient.FMCGHub.Invoke<List<StockGroup>>("StockGroup_List").Result;
                        _toList = new ObservableCollection<StockGroup>(l1.OrderBy(x => x.StockGroupNameWithCode));
                    }
                }
                catch (Exception ex)
                {

                }

                return _toList;
            }
            set
            {
                _toList = value;
            }
        }
        public static List<StockGroup> toGroup(int? UGId)
        {

            List<StockGroup> RV = new List<StockGroup>();

            foreach (var sg in toList.Where(x => x.UnderGroupId == UGId).OrderBy(x => x.GroupCode).ThenBy(x => x.StockGroupName).ToList())
            {
                sg.SubStockGroup = toGroup(sg.Id);
                RV.Add(sg);
            }

            return RV;

        }
        public static ObservableCollection<StockGroup> StocktoList
        {
            get
            {
                try
                {
                    if (_StocktoList == null)
                    {
                        _StocktoList = new ObservableCollection<StockGroup>();
                        var l1 = FMCGHubClient.FMCGHub.Invoke<List<StockGroup>>("StockGroup_PrimaryList").Result;
                        _StocktoList = new ObservableCollection<StockGroup>(l1.OrderBy(x => x.StockGroupNameWithCode));
                    }
                }
                catch (Exception ex)
                {

                }

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
                return _id;
            }
            set
            {
                if (_id != value)
                {
                    _id = value;
                    NotifyPropertyChanged(nameof(Id));
                }
            }
        }
        public string StockGroupName
        {
            get
            {
                return _StockGroupName;
            }
            set
            {
                if (_StockGroupName != value)
                {
                    _StockGroupName = value;
                    NotifyPropertyChanged(nameof(StockGroupName));
                    SetStockGroupNameWithCode();
                }
            }
        }
        public string GroupCode
        {
            get
            {
                return _groupCode;
            }
            set
            {
                if (_groupCode != value)
                {
                    _groupCode = value;
                    NotifyPropertyChanged(nameof(GroupCode));
                    SetStockGroupNameWithCode();
                }
            }
        }
        public string StockGroupNameWithCode
        {
            get
            {
                return _StockGroupNameWithCode;
            }
            set
            {
                if (_StockGroupNameWithCode != value)
                {
                    _StockGroupNameWithCode = value;
                    NotifyPropertyChanged(nameof(StockGroupNameWithCode));
                }
            }
        }
        public int CompanyId
        {
            get
            {
                return _companyId;
            }
            set
            {
                if (_companyId != value)
                {
                    _companyId = value;
                    NotifyPropertyChanged(nameof(CompanyId));
                }
            }
        }
        public int? UnderGroupId
        {
            get
            {
                return _underGroupId;
            }
            set
            {
                if (_underGroupId != value)
                {
                    _underGroupId = value;
                    NotifyPropertyChanged(nameof(UnderGroupId));
                }
            }
        }
        public StockGroup UnderStockGroup
        {
            get
            {
                return _UnderStockGroup;
            }
            set
            {
                if (_UnderStockGroup != value)
                {
                    _UnderStockGroup = value;
                    NotifyPropertyChanged(nameof(UnderStockGroup));
                    NotifyPropertyChanged(nameof(AccountPath));
                }
            }
        }

        public List<StockGroup> SubStockGroup
        {
            get
            {
                return _SubStockGroup;
            }
            set
            {
                if (_SubStockGroup!= value)
                {
                    _SubStockGroup = value;
                    NotifyPropertyChanged(nameof(SubStockGroup));
                }
            }
        }
        public CompanyDetail Company
        {
            get
            {
                return _Company;
            }
            set
            {
                if (_Company != value)
                {
                    _Company = value;
                    NotifyPropertyChanged(nameof(Company));
                }
            }
        }

        public string underStockGroupName
        {
            get
            {
                return _underStockGroupName;
            }
            set
            {
                if (_underStockGroupName != value)
                {
                    _underStockGroupName = value;
                    NotifyPropertyChanged(nameof(underStockGroupName));
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

        public bool IsPurchase
        {
            get
            {
                return _isPurchase;
            }
            set
            {
                if(_isPurchase!=value)
                {
                    _isPurchase=value;
                    NotifyPropertyChanged(nameof(IsPurchase));
                }
            }
        }
        public bool IsSale
        {
            get
            {
                return _isSale;
            }
            set
            {
                if (_isSale != value)
                {
                   _isSale=value;
                    NotifyPropertyChanged(nameof(IsSale));
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
                StockGroup d = toList.Where(x => x.Id == Id).FirstOrDefault();

                if (d == null)
                {
                    d = new StockGroup() { StockGroupName = this.StockGroupName, UnderGroupId = this.UnderGroupId, GroupCode = this.GroupCode };
                    toList.Add(d);
                }

                else
                {
                    d.StockGroupName = this.StockGroupName;
                    d.UnderGroupId = this.UnderGroupId;
                    d.GroupCode = this.GroupCode;
                }
                this.toCopy<StockGroup>(d);
                if (isServerCall == false)
                {
                  //  StockGroup sg = new StockGroup() { StockGroupName = this.StockGroupName, UnderGroupId = this.UnderGroupId, GroupCode = this.GroupCode, IsPurchase=this.IsPurchase, IsSale=this.IsSale };
                    var i = FMCGHubClient.FMCGHub.Invoke<int>("StockGroup_Save", d).Result;
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
            try
            {
                new StockGroup().toCopy<StockGroup>(this);
                IsReadOnly = !UserPermission.AllowInsert;
                IsPurchase = true;
                IsSale = true;
                NotifyAllPropertyChanged();
            }
            catch (Exception ex)
            {

            }
        }

        public bool Find(int pk)
        {
            var d = toList.Where(x => x.Id == pk).FirstOrDefault();
            if (d != null)
            {
                d.toCopy<StockGroup>(this);
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
                    rv = FMCGHubClient.FMCGHub.Invoke<bool>("StockGroup_Delete", this.Id).Result;
                    if (rv == true)
                    {
                        toList.Remove(d);
                    }
                }
                return rv;
            }

            return false;
        }

        public bool isValid()
        {
            bool RValue = true;

            if (toList.Where(x => x.StockGroupName.ToLower() == StockGroupName.ToLower() && x.Id != Id).Count() > 0)
            {
                RValue = false;
            }
            return RValue;

        }

        public static void Init()
        {
            _toList = null;
        }

        void SetStockGroupNameWithCode()
        {
            StockGroupNameWithCode = StockGroupName;// string.Format("{0}{1}{2}", GroupCode, string.IsNullOrWhiteSpace(GroupCode) ? "" : "-", StockGroupName);
        }
        #endregion
    }
}
