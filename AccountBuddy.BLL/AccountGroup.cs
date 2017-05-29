
using AccountBuddy.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBuddy.BLL
{
    public class AccountGroup : INotifyPropertyChanged
    {
        #region Fields
        private static ObservableCollection<AccountGroup> _toList;

        private int _id;
        private string _groupName;
        private string _groupCode;
        private AccountGroup _UnderAccountGroup;
        private CompanyDetail _Company;
        
        #endregion

        #region Property
        public static ObservableCollection<AccountGroup> toList
        {
            get
            {
                try
                {
                    if (_toList == null)
                    {
                        _toList = new ObservableCollection<AccountGroup>();
                        var l1 = ABClientHub.FMCGHub.Invoke<List<AccountGroup>>("accountGroup_List").Result;
                        _toList = new ObservableCollection<AccountGroup>(l1);
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
        public string GroupName
        {
            get
            {
                return _groupName;
            }
            set
            {
                if (_groupName != value)
                {
                    _groupName = value;
                    NotifyPropertyChanged(nameof(GroupName));
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
                }
            }
        }

        public AccountGroup UnderAccountGroup
        {
            get
            {
                return _UnderAccountGroup;
            }
            set
            {
                if (_UnderAccountGroup != value)
                {
                    _UnderAccountGroup = value;
                    NotifyPropertyChanged(nameof(UnderAccountGroup));
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
                AccountGroup d = toList.Where(x => x.Id == Id).FirstOrDefault();

                if (d == null)
                {
                    d = new AccountGroup();
                    toList.Add(d);
                }

                this.toCopy<AccountGroup>(d);
                if (isServerCall == false)
                {
                    var i = ABClientHub.FMCGHub.Invoke<int>("AccountGroup_Save", this).Result;
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
            new AccountGroup().toCopy<AccountGroup>(this);
            NotifyAllPropertyChanged();
        }

        public bool Find(int pk)
        {
            var d = toList.Where(x => x.Id == pk).FirstOrDefault();
            if (d != null)
            {
                d.toCopy<AccountGroup>(this);
                return true;
            }

            return false;
        }

        public bool Delete(bool isServerCall = false)
        {
            var d = toList.Where(x => x.Id == Id).FirstOrDefault();
            if (d != null)
            {
                toList.Remove(d);
                if (isServerCall == false) ABClientHub.FMCGHub.Invoke<int>("AccountGroup_Delete", this.Id);
                return true;
            }

            return false;
        }

        public bool isValid()
        {
            bool RValue = true;
            
            if (toList.Where(x => x.GroupName.ToLower() == GroupName.ToLower() && x.Id != Id).Count() > 0)
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
