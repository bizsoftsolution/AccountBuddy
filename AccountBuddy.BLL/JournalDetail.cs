using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AccountBuddy.BLL
{
    public class JournalDetail : INotifyPropertyChanged
    {

        #region Fields
        private long _Id;
        private long _JournalId;
        private int _LedgerId;
        private decimal _DrAmt;
        private decimal _CrAmt;
        private string _Particular;
        private Ledger _JLedger;
        private string _LedgerName;
        private DateTime? _ClearDate;
        private DateTime? _ChequeDate;
        private string _ChequeNo;
        private decimal? _ExtraCharge;
        private bool _IsShowTTDetail;
        private bool _IsShowOnlineDetail;
        private bool _IsShowChequeDetail;
        private string _Status;
        private bool _IsShowComplete;
        private bool _IsShowReturn;
        private string _RefNo;
        private string _TransactionMode;
        private static List<string> _TransactionModeList;
        private static List<string> _StatusList;

        #endregion

        #region Property
        public static List<string> TransactionModeList
        {
            get
            {
                if (_TransactionModeList == null)
                {
                    _TransactionModeList = new List<string>();
                    _TransactionModeList.Add("Cash");
                    _TransactionModeList.Add("Cheque");
                    _TransactionModeList.Add("Online");
                    _TransactionModeList.Add("TT");
                }
                return _TransactionModeList;
            }
            set
            {
                if (_TransactionModeList != value)
                {
                    _TransactionModeList = value;
                }
            }
        }
        public static List<string> StatusList
        {
            get
            {
                if (_StatusList == null)
                {
                    _StatusList = new List<string>();
                    _StatusList.Add("Process");
                    _StatusList.Add("Completed");
                    _StatusList.Add("Returned");
                }
                return _StatusList;
            }
            set
            {
                if (_StatusList != value)
                {
                    _StatusList = value;
                }
            }
        }

        public long Id
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

        public long JournalId
        {
            get
            {
                return _JournalId;
            }
            set
            {
                if (_JournalId != value)
                {
                    _JournalId = value;
                    NotifyPropertyChanged(nameof(JournalId));
                }
            }
        }
        public int LedgerId
        {
            get
            {
                return _LedgerId;
            }
            set
            {
                if (_LedgerId != value)
                {
                    _LedgerId = value;
                    NotifyPropertyChanged(nameof(LedgerId));
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
                    if(value!=0)
                    {
                        CrAmt = 0;

                    }
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
                    if(value!=0)
                    {
                        DrAmt = 0;
                    }
                }
            }
        }

        public string Particulars
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
                    NotifyPropertyChanged(nameof(Particulars));
                }
            }
        }
        public string TransactionMode
        {
            get
            {
                return _TransactionMode;
            }
            set
            {
                if (_TransactionMode != value)
                {
                    _TransactionMode = value;

                    IsShowChequeDetail = value == "Cheque";
                    IsShowOnlineDetail = value == "Online";
                    IsShowTTDetail = value == "TT";
                    NotifyPropertyChanged(nameof(TransactionMode));
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
        public string Status
        {
            get
            {
                return _Status;
            }
            set
            {
                if (_Status != value)
                {
                    _Status = value;
                    IsShowComplete = value == "Completed";
                    IsShowReturn = value == "Returned";
                    NotifyPropertyChanged(nameof(Status));
                }
            }
        }
        public Nullable<decimal> ExtraCharge
        {
            get
            {
                return _ExtraCharge;
            }
            set
            {
                if (_ExtraCharge != value)
                {
                    _ExtraCharge = value;
                    NotifyPropertyChanged(nameof(ExtraCharge));
                }
            }
        }
        public string ChequeNo
        {
            get
            {
                return _ChequeNo;
            }
            set
            {
                if (_ChequeNo != value)
                {
                    _ChequeNo = value;
                    NotifyPropertyChanged(nameof(ChequeNo));
                }
            }
        }
        public Nullable<System.DateTime> ChequeDate
        {
            get
            {
                return _ChequeDate;
            }
            set
            {
                if (_ChequeDate != value)
                {
                    _ChequeDate = value;
                    NotifyPropertyChanged(nameof(ChequeDate));
                }
            }
        }
        public Nullable<System.DateTime> ClearDate
        {
            get
            {
                return _ClearDate;
            }
            set
            {
                if (_ClearDate != value)
                {
                    _ClearDate = value;
                    NotifyPropertyChanged(nameof(ClearDate));
                }
            }
        }
        public bool IsShowChequeDetail
        {
            get
            {
                return _IsShowChequeDetail;
            }
            set
            {
                if (_IsShowChequeDetail != value)
                {
                    _IsShowChequeDetail = value;
                    NotifyPropertyChanged(nameof(IsShowChequeDetail));
                }
            }
        }
        public bool IsShowOnlineDetail
        {
            get
            {
                return _IsShowOnlineDetail;
            }
            set
            {
                if (_IsShowOnlineDetail != value)
                {
                    _IsShowOnlineDetail = value;
                    NotifyPropertyChanged(nameof(IsShowOnlineDetail));
                }
            }
        }
        public bool IsShowTTDetail
        {
            get
            {
                return _IsShowTTDetail;
            }
            set
            {
                if (_IsShowTTDetail != value)
                {
                    _IsShowTTDetail = value;
                    NotifyPropertyChanged(nameof(IsShowTTDetail));
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
                }
            }
        }

        public Ledger JLedger
        {
            get
            {
                if (_JLedger == null) _JLedger = new Ledger();
                return _JLedger;
            }
            set
            {
                if (_JLedger != value)
                {
                    _JLedger = value;
                    NotifyPropertyChanged(nameof(JLedger));
                }
            }
        }

    

        public bool IsShowComplete
        {
            get
            {
                return _IsShowComplete;
            }
            set
            {
                if (_IsShowComplete != value)
                {
                    _IsShowComplete = value;
                    NotifyPropertyChanged(nameof(IsShowComplete));
                }
            }
        }

        public bool IsShowReturn
        {
            get
            {
                return _IsShowReturn;
            }
            set
            {
                if (_IsShowReturn != value)
                {
                    _IsShowReturn = value;
                    NotifyPropertyChanged(nameof(IsShowReturn));
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
    }
}
