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
        private static ObservableCollection<string> _AcYearList;

        private int _id;
        private string _CompanyName;
        private string _addressLine1;
        private string _addressLine2;
        private string _postalCode;
        private string _telephoneNo;
        private string _mobileNo;
        private string _eMailId;
        private string _gstNo;
        private byte[] _logo;

        private string _CityName;
        private int _UnderCompanyId;
        private string _CompanyType;
        #endregion

        #region Property

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

        public int UnderCompanyId
        {
            get
            {
                return _UnderCompanyId;
            }
            set
            {
                if (_UnderCompanyId != value)
                {
                    _UnderCompanyId = value;
                    NotifyPropertyChanged(nameof(UnderCompanyId));
                }
            }
        }

        public string CompanyType
        {
            get
            {
                return _CompanyType;
            }
            set
            {
                if (_CompanyType != value)
                {
                    _CompanyType = value;
                    NotifyPropertyChanged(nameof(CompanyType));
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
                return false;

            }

        }

        public void Clear()
        {
            new CompanyDetail().toCopy<CompanyDetail>(this);
            NotifyAllPropertyChanged();
        }

        public bool Find(int pk)
        {
            var d = toList.Where(x => x.Id == pk).FirstOrDefault();
            if (d != null)
            {
                d.toCopy<CompanyDetail>(this);
                return true;
            }

            return false;
        }

        

        #endregion
    }
}
