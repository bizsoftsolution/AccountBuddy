
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
        private string _groupNameWithCode;
        private string _groupCode;
        private int? _underGroupId;
        private int _companyId;
        private AccountGroup _UnderAccountGroup;
        private List<AccountGroup> _SubAccountGroup;
        private CompanyDetail _Company;
        private string _underGroupName;

        private static UserTypeDetail _UserPermission;
        private bool _IsReadOnly;
        private bool _IsEnabled;
        #endregion

        #region Property

        public string AccountPath
        {
            get
            {
                return UnderAccountGroup == null ? "" : UnderAccountGroup.AccountPath + "/" + _groupName;
            }
        }
        public static UserTypeDetail UserPermission
        {
            get
            {
                if (_UserPermission == null)
                {
                    _UserPermission = UserAccount.User.UserType == null ? new UserTypeDetail() : UserAccount.User.UserType.UserTypeDetails.Where(x => x.UserTypeFormDetail.FormName == Forms.frmAccountGroup.ToString()).FirstOrDefault();
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

        public static ObservableCollection<AccountGroup> toList
        {
            get
            {
                try
                {
                    if (_toList == null||_toList.Count==0)
                    {
                        try
                        {
                            _toList = new ObservableCollection<AccountGroup>();
                            var l1 = FMCGHubClient.FMCGHub.Invoke<List<AccountGroup>>("accountGroup_List").Result;
                            _toList = new ObservableCollection<AccountGroup>(l1.OrderBy(x => x.GroupNameWithCode));
                        }
                        catch(Exception ex)
                        {
                            Common.AppLib.WriteLog(string.Format("{Account Group List={0}", ex.Message));
                        }
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
        public static List<AccountGroup> toGroup(int? UGId)
        {
            List<AccountGroup> RV = new List<AccountGroup>();
            try
            {             
                foreach (var ag in toList.Where(x => x.UnderGroupId == UGId ).OrderBy(x => x.GroupCode).ThenBy(x => x.GroupName).ToList())
                {
                    ag.SubAccountGroup = toGroup(ag.Id);
                    RV.Add(ag);
                }
                return RV;
            }
            catch(Exception ex) { }
            return RV;
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
                    SetGroupNameWithCode();
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
                    SetGroupNameWithCode();
                }
            }
        }
        public string GroupNameWithCode
        {
            get
            {
                return _groupNameWithCode;
            }
            set
            {
                if (_groupNameWithCode != value)
                {
                    _groupNameWithCode = value;
                    NotifyPropertyChanged(nameof(GroupNameWithCode));
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
                    NotifyPropertyChanged(nameof(AccountPath));
                }
            }
        }
        public List<AccountGroup> SubAccountGroup

        {
            get
            {
                return _SubAccountGroup;
            }
            set
            {
                if (_SubAccountGroup != value)
                {
                    _SubAccountGroup = value;
                    NotifyPropertyChanged(nameof(SubAccountGroup));                    
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

        public string underGroupName
        {
            get
            {
                return _underGroupName;
            }
            set
            {
                if (_underGroupName != value)
                {
                    _underGroupName = value;
                    NotifyPropertyChanged(nameof(underGroupName));
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
                AccountGroup d = toList.Where(x => x.Id == Id).Select(x => new BLL.AccountGroup() { GroupCode= x.GroupCode, GroupName=x.GroupName, UnderGroupId= x.UnderGroupId }).FirstOrDefault();
                if (d == null)
                {
                    d = new AccountGroup() { GroupName = this.GroupName, UnderGroupId = this.UnderGroupId, GroupCode = this.GroupCode };
                   
                }
              
                else
                {
                    d.GroupName = this.GroupName;
                    d.UnderGroupId = this.UnderGroupId;
                    d.GroupCode = this.GroupCode;
                }



                this.toCopy<AccountGroup>(d);
                if (isServerCall == false)
                {
                  // AccountGroup ag = new AccountGroup() {GroupName=this.GroupName,UnderGroupId=this.UnderGroupId, GroupCode = this.GroupCode };
                    var i = FMCGHubClient.FMCGHub.Invoke<int>("AccountGroup_Save",d).Result;
                    d.Id = i;
                    toList.Add(d);
                   
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
                new AccountGroup().toCopy<AccountGroup>(this);
                IsReadOnly = !UserPermission.AllowInsert;

                NotifyAllPropertyChanged();
            }
           catch(Exception ex)
            {

            }
        }

        public bool Find(int pk)
        {
            var d = toList.Where(x => x.Id == pk).FirstOrDefault();
            if (d != null)
            {
                d.toCopy<AccountGroup>(this);
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
                    rv = FMCGHubClient.FMCGHub.Invoke<bool>("AccountGroup_Delete", this.Id).Result;
                    if(rv==true)
                    {
                        toList.Remove(d);
                        toList = null;
                    }
                }
                else
                {
                    toList.Remove(d);
                }
                return rv;
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
            _UserPermission = null;
        }

        void SetGroupNameWithCode()
        {
            GroupNameWithCode = GroupName;// string.Format("{0}{1}{2}", GroupCode, string.IsNullOrWhiteSpace(GroupCode) ? "" : "-", GroupName);
        }
        #endregion
    }
}
