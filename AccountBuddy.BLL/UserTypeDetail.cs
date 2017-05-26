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

        private int _id;
        private int _userTypeId;        
        private bool _isViewForm;
        private bool _allowInsert;
        private bool _allowUpdate;
        private bool _allowDelete;
        private int _UserTypeFormDetailId;
        private BLL.UserTypeFormDetail _UserTypeFormDetail;
        
        #endregion

        #region Property
        
        public BLL.UserTypeFormDetail UserTypeFormDetail
        {
            get
            {
                return _UserTypeFormDetail;
            }
            set
            {
                if (_UserTypeFormDetail != value)
                {
                    _UserTypeFormDetail = value;
                    NotifyPropertyChanged(nameof(UserTypeFormDetail));                    
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
        public int UserTypeFormDetailId
        {
            get
            {
                return _UserTypeFormDetailId;
            }
            set
            {
                if (_UserTypeFormDetailId != value)
                {
                    _UserTypeFormDetailId = value;
                    NotifyPropertyChanged(nameof(UserTypeFormDetailId));
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
                    if (value == false)
                    {
                        AllowInsert = false;
                        AllowUpdate = false;
                        AllowDelete = false;
                    }
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
