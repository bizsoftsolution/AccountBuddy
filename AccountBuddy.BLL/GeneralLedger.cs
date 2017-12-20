using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountBuddy.Common;

namespace AccountBuddy.BLL
{
    public class GeneralLedger : INotifyPropertyChanged
    {
        #region Fields
        private long _EId;
        private string _EType;
        private DateTime? _EDate;
        private String _EntryNo;
        private string _RefNo;
        private Ledger _Ledger;
        private decimal _CrAmt;
        private decimal _DrAmt;
        private decimal _BalAmt;
        private string _AccountName;
        private string _Particular;
        private string _RefEntryNo;
        private string _RefCode;
        private static UserTypeDetail _UserPermission;
        private string _TType;
        #endregion

        #region Property
        public static UserTypeDetail UserPermission
        {
            get
            {
                if (_UserPermission == null)
                {
                    _UserPermission = UserAccount.User.UserType == null ? new UserTypeDetail() : UserAccount.User.UserType.UserTypeDetails.Where(x => x.UserTypeFormDetail.FormName == Forms.frmGeneralLedger.ToString()).FirstOrDefault();
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

        public long EId
        {
            get
            {
                return _EId;
            }
            set
            {
                if (_EId != value)
                {
                    _EId = value;
                    NotifyPropertyChanged(nameof(EId));
                }
            }
        }
        public string EType
        {
            get
            {
                return _EType;
            }
            set
            {
                if (_EType != value)
                {
                    _EType = value;
                    NotifyPropertyChanged(nameof(EType));
                }
            }
        }
        public string TType
        {
            get
            {
                return _TType;
            }
            set
            {
                if (_TType != value)
                {
                    _TType = value;
                    NotifyPropertyChanged(nameof(TType));
                }
            }
        }
        public DateTime? EDate
        {
            get
            {
                return _EDate;
            }
            set
            {
                if (_EDate != value)
                {
                    _EDate = value;
                    NotifyPropertyChanged(nameof(EDate));
                }
            }
        }
        public string RefNo
        {
            get
            {
                return _RefNo;
            }
            set
            {
                if (_RefNo != value)
                {
                    _RefNo = value;
                    NotifyPropertyChanged(nameof(RefNo));
                }
            }
        }
        public string RefCode
        {
            get
            {
                return _RefCode;
            }
            set
            {
                if (_RefCode != value)
                {
                    _RefCode = value;
                    NotifyPropertyChanged(nameof(RefCode));
                }
            }
        }
        public string RefEntryNo
        {
            get
            {
                return _RefEntryNo;
            }
            set
            {
                if (_RefEntryNo != value)
                {
                    _RefEntryNo = value;
                    NotifyPropertyChanged(nameof(RefEntryNo));
                }
            }
        }
        public string EntryNo
        {
            get
            {
                return _EntryNo;
            }
            set
            {
                if (_EntryNo != value)
                {
                    _EntryNo = value;
                    NotifyPropertyChanged(nameof(EntryNo));
                }
            }
        }
        public Ledger Ledger
        {
            get
            {
                return _Ledger;
            }
            set
            {
                if (_Ledger != value)
                {
                    _Ledger = value;
                    NotifyPropertyChanged(nameof(Ledger));
                }
            }
        }
        public decimal CrAmt
        {
            get
            {
                return _CrAmt;
            }
            set
            {
                if (_CrAmt != value)
                {
                    _CrAmt = value;
                    NotifyPropertyChanged(nameof(CrAmt));
                }
            }
        }
        public decimal DrAmt
        {
            get
            {
                return _DrAmt;
            }
            set
            {
                if (_DrAmt != value)
                {
                    _DrAmt = value;
                    NotifyPropertyChanged(nameof(DrAmt));
                }
            }
        }
        public decimal BalAmt
        {
            get
            {
                return _BalAmt;
            }
            set
            {
                if (_BalAmt != value)
                {
                    _BalAmt = value;
                    NotifyPropertyChanged(nameof(BalAmt));
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

        public string Particular
        {
            get
            {
                return _Particular;
            }
            set
            {
                if (_Particular != value)
                {
                    _Particular = value;
                    NotifyPropertyChanged(nameof(Particular));
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

        public static List<GeneralLedger> ToList(int LedgerId, DateTime dtFrom, DateTime dtTo)
        {
            List<GeneralLedger> l = new List<GeneralLedger>();
            try
            {
                l = FMCGHubClient.HubCaller.Invoke<List<GeneralLedger>>("GeneralLedger_List", LedgerId, dtFrom, dtTo).Result;
                return l;

            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex);
                return l;
            }
        }

        #endregion
    }
}
