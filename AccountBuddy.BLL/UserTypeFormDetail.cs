using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBuddy.BLL
{
    public class UserTypeFormDetail : INotifyPropertyChanged
    {

        #region Field

        private int _Id;
        private string _FormName;
        private string _FormType;
        private string _OrderNo;
        private bool _IsActive;
        private bool _IsInsert;
        private bool _IsUpdate;
        private bool _IsDelete;
        private bool _IsMenu;
        private string _Description;

        private static ObservableCollection<UserTypeFormDetail> _toList;

        #endregion

        #region Property

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
        public string FormName
        {
            get
            {
                return _FormName;
            }
            set
            {
                if (_FormName != value)
                {
                    _FormName = value;
                    NotifyPropertyChanged(nameof(FormName));
                }

            }
        }
        public string FormType
        {
            get
            {
                return _FormType;
            }
            set
            {
                if (_FormType != value)
                {
                    _FormType = value;
                    NotifyPropertyChanged(nameof(FormType));
                }

            }
        }
        public string OrderNo
        {
            get
            {
                return _OrderNo;
            }
            set
            {
                if (_OrderNo != value)
                {
                    _OrderNo = value;
                    NotifyPropertyChanged(nameof(OrderNo));
                }

            }
        }

        public bool IsActive
        {
            get
            {
                return _IsActive;
            }
            set
            {
                if (_IsActive != value)
                {
                    _IsActive = value;
                    NotifyPropertyChanged(nameof(IsActive));
                }
            }
        }

        public bool IsInsert
        {
            get
            {
                return _IsInsert;
            }
            set
            {
                if (_IsInsert != value)
                {
                    _IsInsert = value;
                    NotifyPropertyChanged(nameof(IsInsert));
                }
            }
        }

        public bool IsUpdate
        {
            get
            {
                return _IsUpdate;
            }
            set
            {
                if (_IsUpdate != value)
                {
                    _IsUpdate = value;
                    NotifyPropertyChanged(nameof(IsUpdate));
                }
            }
        }

        public bool IsDelete
        {
            get
            {
                return _IsDelete;
            }
            set
            {
                if (_IsDelete != value)
                {
                    _IsDelete = value;
                    NotifyPropertyChanged(nameof(IsDelete));
                }
            }
        }

        public bool IsMenu
        {
            get
            {
                return _IsMenu;
            }
            set
            {
                if (_IsMenu != value)
                {
                    _IsMenu = value;
                    NotifyPropertyChanged(nameof(IsMenu));
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

        public static ObservableCollection<UserTypeFormDetail> toList
        {
            get
            {
                if (_toList == null)
                {
                    _toList = new ObservableCollection<UserTypeFormDetail>(FMCGHubClient.FMCGHub.Invoke<List<UserTypeFormDetail>>("UserTypeFormDetail_List").Result);
                }

                return _toList;
            }
        }


        #endregion

        #region Property Notify Changed

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
        
    }
}
