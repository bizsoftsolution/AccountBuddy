using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBuddy.BLL
{
    public class UserTypeDetail : INotifyPropertyChanged
    {
        #region Field
        private static ObservableCollection<UserTypeDetail> _toList;

        private int _id;
        private int _userTypeId;
        private int _userTypeFormDetailId;
        private bool _isViewForm;
        private bool _allowInsert;
        private bool _allowUpdate;
        private bool _allowDelete;
        private string _FormName;
        private string _FormType;
        private bool _IsNotReport;
        private string _UserTypeName;
        #endregion

        #region Property

        public static ObservableCollection<UserTypeDetail> ToList
        {
            get
            {
                if (_toList == null)
                {
                    _toList = new ObservableCollection<UserTypeDetail>( ABClientHub.FMCGHub.Invoke<List<UserTypeDetail>>("UserTypeDetail_List").Result);
                }
                return _toList;
            }
            set
            {
                _toList = value;
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
                    IsNotReport = value != "Report";
                    NotifyPropertyChanged(nameof(FormType));
                }
            }
        }

        public bool IsNotReport
        {
            get
            {
                return _IsNotReport;
            }
            set
            {
                if (_IsNotReport != value)
                {
                    _IsNotReport = value;
                    NotifyPropertyChanged(nameof(IsNotReport));
                }
            }
        }

        public string UserTypeName
        {
            get
            {

                return _UserTypeName;
            }
            set
            {
                if (_UserTypeName != value)
                {
                    _UserTypeName = value;
                    NotifyPropertyChanged(nameof(UserTypeName));
                }
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
        public int UserTypeId
        {
            get
            {
                return _userTypeId;
            }
            set
            {
                if (_userTypeId != value)
                {
                    _userTypeId = value;
                    NotifyPropertyChanged(nameof(UserTypeId));
                }
            }
        }
        public int UserTypeFormDetailId
        {
            get
            {
                return _userTypeFormDetailId;
            }
            set
            {
                if (_userTypeFormDetailId != value)
                {
                    _userTypeFormDetailId = value;
                    NotifyPropertyChanged(nameof(UserTypeFormDetailId));
                }
            }
        }
        public bool IsViewForm
        {
            get
            {
                return _isViewForm;
            }
            set
            {
                if (_isViewForm != value)
                {
                    _isViewForm = value;
                    NotifyPropertyChanged(nameof(IsViewForm));
                }
            }
        }
        public bool AllowInsert
        {
            get
            {
                return _allowInsert;
            }
            set
            {
                if (_allowInsert != value)
                {
                    _allowInsert = value;
                    NotifyPropertyChanged(nameof(AllowInsert));
                }
            }
        }
        public bool AllowUpdate
        {
            get
            {
                return _allowUpdate;
            }
            set
            {
                if (_allowUpdate != value)
                {
                    _allowUpdate = value;
                    NotifyPropertyChanged(nameof(AllowUpdate));
                }
            }
        }
        public bool AllowDelete
        {
            get
            {
                return _allowDelete;
            }
            set
            {
                if (_allowDelete != value)
                {
                    _allowDelete = value;
                    NotifyPropertyChanged(nameof(AllowDelete));
                }
            }
        }

        #endregion

        #region Property Notify Changed

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }


        #endregion

    }
}
