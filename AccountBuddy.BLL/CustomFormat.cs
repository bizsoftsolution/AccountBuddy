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
    public class CustomFormat : INotifyPropertyChanged
    {
        #region Fileds

        private static ObservableCollection<CustomFormat> _toList;

        private int _Id;
        private string _CurrencySymbol;
        private string _CurrencyName1;
        private string _CurrencyName2;
        private string _DateFormat;
        private string _NumberFormat;
        private int _CompanyId;

        private CompanyDetail _Company;

        private static UserTypeDetail _UserPermission;
        private bool _IsReadOnly;
        private bool _IsPrefix;
        private CustomFormat d;
        private bool _IsSuffix;


        #endregion

        #region Property
        public static UserTypeDetail UserPermission
        {
            get
            {
                if (_UserPermission == null)
                {
                    _UserPermission = UserAccount.User.UserType == null ? new UserTypeDetail() : UserAccount.User.UserType.UserTypeDetails.Where(x => x.UserTypeFormDetail.FormName == AppLib.Forms.frmCustomFormat.ToString()).FirstOrDefault();
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
                return _IsPrefix;
            }

            set
            {
                if (_IsPrefix != value)
                {
                    _IsPrefix = value;
                    NotifyPropertyChanged(nameof(IsEnabled));
                }
            }
        }
        public static ObservableCollection<CustomFormat> toList
        {
            get
            {
                if (_toList == null) _toList = new ObservableCollection<CustomFormat>(FMCGHubClient.FMCGHub.Invoke<List<CustomFormat>>("CustomFormat_List").Result);
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
        public string CurrencySymbol
        {
            get
            {
                return _CurrencySymbol;
            }

            set
            {
                if (_CurrencySymbol != value)
                {
                    _CurrencySymbol = value;
                    NotifyPropertyChanged(nameof(CurrencySymbol));
                }
            }
        }
        public string CurrencyName1
        {
            get
            {
                return _CurrencyName1;
            }

            set
            {
                if (_CurrencyName1 != value)
                {
                    _CurrencyName1 = value;
                    NotifyPropertyChanged(nameof(CurrencyName1));
                }
            }
        }
        public string CurrencyName2
        {
            get
            {
                return _CurrencyName2;
            }

            set
            {
                if (_CurrencyName2 != value)
                {
                    _CurrencyName2 = value;
                    NotifyPropertyChanged(nameof(CurrencyName2));
                }
            }
        }
        public string DateFormat
        {
            get
            {
                return _DateFormat;
            }

            set
            {
                if (_DateFormat != value)
                {
                    _DateFormat = value;
                    NotifyPropertyChanged(nameof(DateFormat));
                }
            }
        }
        public string NumberFormat
        {
            get
            {
                return _NumberFormat;
            }

            set
            {
                if (_NumberFormat != value)
                {
                    _NumberFormat = value;
                    NotifyPropertyChanged(nameof(NumberFormat));
                }
            }
        }
        public bool IsPrefix
        {
            get
            {
                return _IsPrefix;
            }

            set
            {
                if (_IsPrefix != value)
                {
                    _IsPrefix = value;
                    NotifyPropertyChanged(nameof(IsPrefix));
                }
            }
        }
        public bool IsSuffix
        {
            get
            {
                return _IsSuffix;
            }

            set
            {
                if (_IsSuffix != value)
                {
                    _IsSuffix = value;
                    NotifyPropertyChanged(nameof(IsSuffix));
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
                    NotifyPropertyChanged(nameof(CompanyId));
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
           
            try
            {
                CustomFormat d = toList.Where(x => x.Id == Id).FirstOrDefault();

                if (d == null)
                {
                    d = new CustomFormat();
                    toList.Add(d);
                }

                this.toCopy<CustomFormat>(d);
                if (isServerCall == false)
                {
                    var i = FMCGHubClient.FMCGHub.Invoke<int>("CustomFormat_Save", this).Result;
                    d.Id = i;
                }
                var V = BLL.CustomFormat.toList;
                Common.AppLib.CurrencyName1 = V.FirstOrDefault().CurrencyName1;
                Common.AppLib.CurrencyName2 = V.FirstOrDefault().CurrencyName2;
                Common.AppLib.IsPrefix = V.FirstOrDefault().IsPrefix;
                Common.AppLib.IsSuffix = V.FirstOrDefault().IsSuffix;
                return true;
              
            }
            catch (Exception ex) { }
            return false;
        }

        public void Clear()
        {
            new CustomFormat().toCopy<CustomFormat>(this);
           
            NotifyAllPropertyChanged();
        }

        public bool Find(int CompanyId)
        {
            int CId;
           
            if (BLL.UserAccount.User.UserType.Company.CompanyType == "Warehouse" || BLL.UserAccount.User.UserType.Company.CompanyType == "Dealer")
            {
                CId =(int) BLL.UserAccount.User.UserType.Company.UnderCompanyId;
                d = toList.Where(x => x.CompanyId == CId).FirstOrDefault();

            }
            else
            {
                d = toList.Where(x => x.CompanyId == CompanyId).FirstOrDefault();

            }
            if (d != null)
            {
                d.toCopy<CustomFormat>(this);
                //IsReadOnly = !UserPermission.AllowUpdate;

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
                    rv = FMCGHubClient.FMCGHub.Invoke<bool>("CustomFormat_Delete", this.Id).Result;
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

            return rv;
        }

       

        public static void Init()
        {
            _toList = null;
        }

      


        #endregion

    }
}
