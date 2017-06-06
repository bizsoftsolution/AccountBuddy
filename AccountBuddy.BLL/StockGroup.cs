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

        private int _id;
        private int _AccountGroupId;
        private AccountGroup _AccountGroup;
       
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
                    _UserPermission = UserAccount.User.UserType == null ? new UserTypeDetail() : UserAccount.User.UserType.UserTypeDetails.Where(x => x.UserTypeFormDetail.FormName == AppLib.Forms.frmStockGroup.ToString()).FirstOrDefault();
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
                        _toList = new ObservableCollection<StockGroup>(l1.OrderBy(x => x.AccountGroup.GroupCode));
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
        public int AccountGroupId
        {
            get
            {
                return _AccountGroupId;
            }
            set
            {
                if (_AccountGroupId != value)
                {
                    _AccountGroupId = value;
                    NotifyPropertyChanged(nameof(AccountGroupId));
                }
            }
        }

        public AccountGroup AccountGroup
        {
            get
            {
                if (_AccountGroup == null) _AccountGroup = new BLL.AccountGroup();
                return _AccountGroup;
            }
            set
            {
                if(_AccountGroup!=value)
                {
                    _AccountGroup = value;
                    NotifyPropertyChanged(nameof(AccountGroup));
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
                    d = new StockGroup();
                    toList.Add(d);
                }

                this.toCopy<StockGroup>(d);
                if (isServerCall == false)
                {
                    var i = FMCGHubClient.FMCGHub.Invoke<int>("StockGroup_Save", this).Result;
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

            //if (toList.Where(x => x.StockGroupName.ToLower() == StockGroupName.ToLower() && x.Id != Id).Count() > 0)
            //{
            //    RValue = false;
            //}
            return RValue;

        }

        public static void Init()
        {
            _toList = null;
        }

       
        #endregion
    }
}
