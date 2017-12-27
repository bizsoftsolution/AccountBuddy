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
    public class Department : INotifyPropertyChanged
    {
        #region Fields
        private static ObservableCollection<Department> _toList;

        private int _Id;
        private string _DepartmentName;
        private string _Description;
        private int _CompanyId;
        private CompanyDetail _Company;
        private double? _Budget;

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
                    _UserPermission = UserAccount.User.UserType == null ? new UserTypeDetail() : UserAccount.User.UserType.UserTypeDetails.Where(x => x.UserTypeFormDetail.FormName == Forms.frmDepartment).FirstOrDefault();
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

        public static ObservableCollection<Department> toList
        {
            get
            {
                try
                {
                    if (_toList == null)
                    {
                        _toList = new ObservableCollection<Department>();
                        var l1 = FMCGHubClient.HubCaller.Invoke<List<Department>>("Department_List").Result;
                        _toList = new ObservableCollection<Department>(l1.OrderBy(x => x.DepartmentName));
                    }
                }
                catch (Exception ex)
                {
                    Common.AppLib.WriteLog(ex);
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

        public string DepartmentName
        {
            get
            {
                return _DepartmentName;
            }
            set
            {
                if (_DepartmentName != value)
                {
                    _DepartmentName = value;
                    NotifyPropertyChanged(nameof(DepartmentName));
                }
            }
        }
        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                if (_Description != value)
                {
                    _Description = value;
                    NotifyPropertyChanged(nameof(Description));
                }
            }
        }

        public int CompanyId
        {
            get
            {
                return _CompanyId;
            }
            set
            {
                if (_CompanyId != value)
                {
                    _CompanyId = value;
                    NotifyPropertyChanged(nameof(_CompanyId));
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

        public double? Budget
        {
            get
            {
                return _Budget;
            }
            set
            {
                if (_Budget != value)
                {
                    _Budget = value;
                    NotifyPropertyChanged(nameof(Budget));
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
                Department d = toList.Where(x => x.Id == Id).FirstOrDefault();

                if (d == null)
                {
                    d = new Department();
                    toList.Add(d);
                }

                this.ToMap<Department>(d);
                if (isServerCall == false)
                {
                    var i = FMCGHubClient.HubCaller.Invoke<int>("Department_Save", this).Result;
                    d.Id = i;
                }

                return true;
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex);
                return false;

            }

        }

        public void Clear()
        {
            try
            {
                new Department().ToMap<Department>(this);
                IsReadOnly = !UserPermission.AllowInsert;

                NotifyAllPropertyChanged();
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex);
            }
        }

        public bool Find(int pk)
        {
            var d = toList.Where(x => x.Id == pk).FirstOrDefault();
            if (d != null)
            {
                d.ToMap<Department>(this);
               // IsReadOnly = !UserPermission.AllowUpdate;
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
                    rv = FMCGHubClient.HubCaller.Invoke<bool>("Department_Delete", this.Id).Result;
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

            return false;
        }

        public bool isValid()
        {
            bool RValue = true;

            if (toList.Where(x => x.Description.ToLower() == Description.ToLower() && x.Id != Id).Count() > 0)
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
