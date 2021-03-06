﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountBuddy.Common;
using System.Collections.ObjectModel;

namespace AccountBuddy.BLL
{
    public class CompanyDetail : INotifyPropertyChanged
    {
        #region Field

        private static ObservableCollection<CompanyDetail> _toList;

        private static UserTypeDetail _UserPermission;
        private bool _IsReadOnly;
        private bool _IsEnabled;

        private int _LoginAccYear;
        private static ObservableCollection<string> _AcYearList;
        public List<BLL.Validation> lstValidation = new List<BLL.Validation>();
        private int _id;
        private string _CompanyName;
        private string _addressLine1;
        private string _addressLine2;
        private string _CityName;
        private string _postalCode;
        private string _telephoneNo;
        private string _mobileNo;
        private string _eMailId;
        private string _gstNo;
        private byte[] _logo;
        private bool _IsActive;
        private string _UserId;
        private string _Password;

        #endregion

        #region Property
        public static UserTypeDetail UserPermission
        {
            get
            {
                if (_UserPermission == null)
                {
                    _UserPermission = UserAccount.User.UserType==null?new UserTypeDetail() : UserAccount.User.UserType.UserTypeDetails.Where(x => x.UserTypeFormDetail.FormName == AppLib.Forms.frmCompanySetting.ToString()).FirstOrDefault();
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

        public static ObservableCollection<CompanyDetail> toList
        {
            get
            {
                if (_toList == null)
                {
                    var l1 = ABClientHub.FMCGHub.Invoke<List<CompanyDetail>>("CompanyDetail_List").Result;
                    _toList = new ObservableCollection<CompanyDetail>(l1);
                }

                return _toList;
            }
        }

        public static ObservableCollection<string> AcYearList
        {
            get
            {
                if (_AcYearList == null)
                {
                    var l1 = ABClientHub.FMCGHub.Invoke<List<string>>("CompanyDetail_AcYearList").Result;
                    _AcYearList = new ObservableCollection<string>(l1);
                }

                return _AcYearList;
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
                    IsEnabled = !value;
                    NotifyPropertyChanged(nameof(IsReadOnly));
                }
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

        public int LoginAccYear
        {
            get
            {
                return _LoginAccYear;
            }

            set
            {
                if (_LoginAccYear != value)
                {
                    _LoginAccYear = value;
                    NotifyPropertyChanged(nameof(LoginAccYear));
                }
            }
        }
        public string CompanyName
        {
            get
            {
                return _CompanyName;
            }

            set
            {
                if (_CompanyName != value)
                {
                    _CompanyName = value;
                    NotifyPropertyChanged(nameof(CompanyName));
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

        public string UserId
        {
            get
            {
                return _UserId;
            }

            set
            {
                if (_UserId != value)
                {
                    _UserId = value;
                    NotifyPropertyChanged(nameof(UserId));
                }
            }
        }

        public string Password
        {
            get
            {
                return _Password;
            }

            set
            {
                if (_Password != value)
                {
                    _Password = value;
                    NotifyPropertyChanged(nameof(Password));
                }
            }
        }

        public string AddressLine1
        {
            get
            {
                return _addressLine1;
            }

            set
            {
                if (_addressLine1 != value)
                {
                    _addressLine1 = value;
                    NotifyPropertyChanged(nameof(AddressLine1));
                }
            }
        }
        public string AddressLine2
        {
            get
            {
                return _addressLine2;
            }

            set
            {
                if (_addressLine2 != value)
                {
                    _addressLine2 = value;
                    NotifyPropertyChanged(nameof(AddressLine2));
                }
            }
        }
        
        public string PostalCode
        {
            get
            {
                return _postalCode;
            }

            set
            {
                if (_postalCode != value)
                {
                    _postalCode = value;
                    NotifyPropertyChanged(nameof(PostalCode));
                }
            }
        }
        public string TelephoneNo
        {
            get
            {
                return _telephoneNo;
            }

            set
            {
                if (_telephoneNo != value)
                {
                    _telephoneNo = value;
                    NotifyPropertyChanged(nameof(TelephoneNo));
                }
            }
        }
        public string MobileNo
        {
            get
            {
                return _mobileNo;
            }

            set
            {
                if (_mobileNo != value)
                {
                    _mobileNo = value;
                    NotifyPropertyChanged(nameof(MobileNo));
                }
            }
        }
        public string EMailId
        {
            get
            {
                return _eMailId;
            }

            set
            {
                if (_eMailId != value)
                {
                    _eMailId = value;
                    NotifyPropertyChanged(nameof(EMailId));
                }
            }
        }
        public string GSTNo
        {
            get
            {
                return _gstNo;
            }

            set
            {
                if (_gstNo != value)
                {
                    _gstNo = value;
                    NotifyPropertyChanged(nameof(GSTNo));
                }
            }
        }
        public byte[] Logo
        {
            get
            {
                return _logo;
            }

            set
            {
                if (_logo != value)
                {
                    _logo = value;
                    NotifyPropertyChanged(nameof(Logo));
                }
            }
        }


        public string CityName
        {
            get
            {
                return _CityName;
            }
            set
            {
                if (_CityName != value)
                {
                    _CityName = value;
                    NotifyPropertyChanged(nameof(CityName));
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
                CompanyDetail d = toList.Where(x => x.Id == Id).FirstOrDefault();
                int i = 0;
                if (d == null)
                {
                    d = new CompanyDetail();
                    toList.Add(d);
                }

                this.toCopy<CompanyDetail>(d);
                if (isServerCall == false)
                {
                    i = ABClientHub.FMCGHub.Invoke<int>("CompanyDetail_Save", this).Result;
                    d.Id = i;
                }

                return i != 0;
            }
            catch (Exception ex)
            {
                lstValidation.Add(new Validation() { Name = string.Empty, Message = ex.Message });
                return false;

            }

        }

        public void Clear()
        {
           new CompanyDetail().toCopy<CompanyDetail>(this);
           IsReadOnly = !UserPermission.AllowInsert;
           
           NotifyAllPropertyChanged();
        }

        public bool Find(int pk)
        {
            var d = toList.Where(x => x.Id == pk).FirstOrDefault();
            if (d != null)
            {
                d.toCopy<CompanyDetail>(this);
                IsReadOnly = !UserPermission.AllowUpdate;
                return true;
            }

            return false;
        }

        public bool isValid()
        {
            bool RValue = true;
            lstValidation.Clear();

            
            if (string.IsNullOrWhiteSpace(CompanyName))
            {
                lstValidation.Add(new Validation() { Name = nameof(CompanyName), Message = string.Format(Message.BLL.Required_Data, nameof(CompanyName)) });
                RValue = false;
            }
            else if (toList.Where(x => x.CompanyName.ToLower() == CompanyName.ToLower() && x.Id != Id).Count() > 0)
            {
                lstValidation.Add(new Validation() { Name = nameof(CompanyName), Message = string.Format(Message.BLL.Existing_Data, CompanyName) });
                RValue = false;
            }
            if (Id == 0)
            {
                if (string.IsNullOrWhiteSpace(UserId))
                {
                    lstValidation.Add(new Validation() { Name = nameof(UserId), Message = string.Format(Message.BLL.Required_Data, nameof(UserId)) });
                    RValue = false;
                }

                if (string.IsNullOrWhiteSpace(Password))
                {
                    lstValidation.Add(new Validation() { Name = nameof(Password), Message = string.Format(Message.BLL.Required_Data, nameof(Password)) });
                    RValue = false;
                }

            }

            return RValue;

        }

        public bool Delete(bool isServerCall = false)
        {
            var d = toList.Where(x => x.Id == Id).FirstOrDefault();
            if (d != null)
            {
                toList.Remove(d);
                if (isServerCall == false) ABClientHub.FMCGHub.Invoke<int>("CompanyDetail_Delete", this.Id);
                return true;
            }

            return false;
        }

        #endregion
    }
}
