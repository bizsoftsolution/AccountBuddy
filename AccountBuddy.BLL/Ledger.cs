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
    public class Ledger : INotifyPropertyChanged
    {
        #region Fileds
        private static ObservableCollection<Ledger> _toList;
        private static List<string> _LedgerList;


        private int _Id;
        private string _LedgerName;
        private string _PersonIncharge;
        private string _AddressLine1;
        private string _AddressLine2;
        private int? _CityId;
        private string _TelephoneNo;
        private string _MobileNo;
        private string _EMailId;
        private string _GSTNo;
        private short _CreditLimit;
        private int? _CreditLimitTypeId;
        private double _CreditAmount;
        private int? _AccountGroupId;
        private decimal? _OPDr;
        private decimal? _OPCr;
        private string _LedgerCode;
        private string _GroupCode;
        private string _AccountCode;
        private string _GroupName;
        private int? _CompanyId;
        private string _cityName;
        private string _creditLimitType;
        private string _AccountName;

        #endregion

        #region Property
        public static ObservableCollection<Ledger> toList
        {
            get
            {
                try
                {
                    if (_toList == null)
                    {
                        _toList = new ObservableCollection<Ledger>();
                        var l1 = ABClientHub.FMCGHub.Invoke<List<Ledger>>("Ledger_List").Result;
                        _toList = new ObservableCollection<Ledger>(l1);
                    }
                    _toList = new ObservableCollection<Ledger>(_toList.OrderBy(x => x.AccountName));
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


        public static List<string> LedgerList
        {
            get
            {
                if (_LedgerList == null)
                {
                    _LedgerList = new List<string>();
                    _LedgerList.Add("Supplier");
                    _LedgerList.Add("Ledger");
                    _LedgerList.Add("Customer");
                    _LedgerList.Add("Bank");
                }
                return _LedgerList;
            }
            set
            {
                if (_LedgerList != value)
                {
                    _LedgerList = value;
                }
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
        public string LedgerName
        {
            get
            {
                return _LedgerName;
            }

            set
            {
                if (_LedgerName != value)
                {
                    _LedgerName = value;
                    NotifyPropertyChanged(nameof(LedgerName));
                    AccountName = string.Format("{0}{1}{2}", AccountCode, string.IsNullOrWhiteSpace(AccountCode)?"":"-", LedgerName);
                }
            }
        }

        public string AccountName
        {
            get
            {
                return _AccountName;
            }

            set
            {
                if (_AccountName != value)
                {
                    _AccountName = value;
                    NotifyPropertyChanged(nameof(AccountName));
                }
            }
        }

        public string PersonIncharge
        {
            get
            {
                return
                    _PersonIncharge;
            }

            set
            {
                if (_PersonIncharge != value)
                {
                    _PersonIncharge = value;
                    NotifyPropertyChanged(nameof(PersonIncharge));
                }
            }
        }

        public string AddressLine1
        {
            get
            {
                return
                    _AddressLine1;
            }

            set
            {
                if (_AddressLine1 != value)
                {
                    _AddressLine1 = value;
                    NotifyPropertyChanged(nameof(AddressLine1));
                }
            }
        }

        public string AddressLine2
        {
            get
            {
                return _AddressLine2;
            }

            set
            {
                if (_AddressLine2 != value)
                {
                    _AddressLine2 = value;
                    NotifyPropertyChanged(nameof(AddressLine2));
                }
            }
        }
        public int? CityId
        {
            get
            {
                return _CityId;
            }
            set
            {
                if (_CityId != value)
                {
                    _CityId = value;
                    NotifyPropertyChanged(nameof(CityId));
                }

            }
        }

        public string TelephoneNo
        {
            get
            {
                return _TelephoneNo;
            }

            set
            {
                if (_TelephoneNo != value)
                {
                    _TelephoneNo = value;
                    NotifyPropertyChanged(nameof(TelephoneNo));
                }
            }
        }

        public string MobileNo
        {
            get
            {
                return _MobileNo;
            }

            set
            {
                if (_MobileNo != value)
                {
                    _MobileNo = value;
                    NotifyPropertyChanged(nameof(MobileNo));
                }
            }
        }

        public string GSTNo
        {
            get
            {
                return _GSTNo;
            }

            set
            {
                if (_GSTNo != value)
                {
                    _GSTNo = value;
                    NotifyPropertyChanged(nameof(GSTNo));
                }
            }
        }

        public string EMailId
        {
            get
            {
                return _EMailId;
            }

            set
            {
                if (_EMailId != value)
                {
                    _EMailId = value;
                    NotifyPropertyChanged(nameof(EMailId));
                }
            }
        }

        public short CreditLimit
        {
            get
            {
                return _CreditLimit;
            }

            set
            {
                if (_CreditLimit != value)
                {
                    _CreditLimit = value;
                    NotifyPropertyChanged(nameof(CreditLimit));
                }
            }
        }

        public int? CreditLimitTypeId
        {
            get
            {
                return _CreditLimitTypeId;
            }
            set
            {
                if (_CreditLimitTypeId != value)
                {
                    _CreditLimitTypeId = value;
                    NotifyPropertyChanged(nameof(CreditLimitTypeId));
                }

            }
        }

        public double CreditAmount
        {
            get
            {
                return _CreditAmount;
            }

            set
            {
                if (_CreditAmount != value)
                {
                    _CreditAmount = value;
                    NotifyPropertyChanged(nameof(CreditAmount));
                }
            }
        }

        public int? AccountGroupId
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

        public decimal? OPDr
        {
            get
            {
                return _OPDr;
            }
            set
            {
                if (_OPDr != value)
                {
                    _OPDr = value;
                    NotifyPropertyChanged(nameof(OPDr));
                }

            }
        }

        public decimal? OPCr
        {
            get
            {
                return _OPCr;
            }
            set
            {
                if (_OPCr != value)
                {
                    _OPCr = value;
                    NotifyPropertyChanged(nameof(OPCr));
                }

            }
        }

        public string LedgerCode
        {
            get
            {
                return _LedgerCode;
            }
            set
            {
                if (_LedgerCode != value)
                {
                    _LedgerCode = value;
                    AccountCode = string.Format("{0}{1}{2}", GroupCode, LedgerCode == "" ? "" : "-", LedgerCode);
                    NotifyPropertyChanged(nameof(LedgerCode));
                }
            }
        }

        public string GroupCode
        {
            get
            {
                return _GroupCode;
            }
            set
            {
                if (_GroupCode != value)
                {
                    _GroupCode = value;
                    AccountCode = string.Format("{0}{1}{2}",GroupCode,LedgerCode==""?"":"-",LedgerCode);
                    NotifyPropertyChanged(nameof(GroupCode));
                }
            }
        }

        public string GroupName
        {
            get
            {
                return _GroupName;
            }
            set
            {
                if (_GroupName != value)
                {
                    _GroupName = value;                    
                    NotifyPropertyChanged(nameof(GroupName));
                }
            }
        }

        public string AccountCode
        {
            get
            {
                return _AccountCode;
            }
            set
            {
                if (_AccountCode != value)
                {
                    _AccountCode = value;
                    NotifyPropertyChanged(nameof(AccountCode));
                    AccountName = string.Format("{0}{1}{2}", AccountCode, string.IsNullOrWhiteSpace(AccountCode) ? "" : "-", LedgerName);
                }
            }
        }

        public int? CompanyId
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

        public string CityName
        {
            get
            {
                return _cityName;
            }
            set
            {
                if (_cityName != value)
                {
                    _cityName = value;
                    NotifyPropertyChanged(nameof(CityName));
                }
            }
        }
        public string CreditLimitType
        {
            get
            {
                return _creditLimitType;
            }
            set
            {
                if (_creditLimitType != value)
                {
                    _creditLimitType = value;
                    NotifyPropertyChanged(nameof(CreditLimitType));
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

                Ledger d = toList.Where(x => x.Id == Id).FirstOrDefault();

                if (d == null)
                {
                    d = new Ledger();
                    toList.Add(d);
                }

                this.toCopy<Ledger>(d);
                if (isServerCall == false)
                {
                    var i = ABClientHub.FMCGHub.Invoke<int>("Ledger_Save", this).Result;
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
            new Ledger().toCopy<Ledger>(this);
            NotifyAllPropertyChanged();
        }

        public bool Find(int pk)
        {
            var d = toList.Where(x => x.Id == pk).FirstOrDefault();
            if (d != null)
            {
                d.toCopy<Ledger>(this);
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
                if (isServerCall == false) ABClientHub.FMCGHub.Invoke<int>("Ledger_Delete", this.Id);
                return true;
            }

            return false;
        }

        public bool isValid()
        {
            bool RValue = true;
            if (toList.Where(x => x.LedgerName.ToLower() == LedgerName.ToLower() && x.Id != Id).Count() > 0)
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
