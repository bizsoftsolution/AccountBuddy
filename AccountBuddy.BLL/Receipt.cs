using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBuddy.BLL
{
    public class Receipt : INotifyPropertyChanged
    {
        #region Fields
        private long _Id;
        private string _EntryNo;
        private DateTime _ReceiptDate;
        private int _LedgerId;
        private string _ReceiptMode;
        private decimal _Amount;
        private string _RefNo;
        private string _Status;
        private decimal? _ExtraCharge;
        private string _ChequeNo;
        private DateTime? _ChequeDate;
        private DateTime? _ClearDate;
        private string _Particulars;
        private string _PayTo;
        private string _VoucherNo;
        private string _LedgerName;
        private string _AmountInwords;


        private Ledger _RLedger;

        private ReceiptDetail _RDetail;


        private string _SearchText;

        private string _ReceivingMode;

        private bool _IsShowChequeDetail;
        private bool _IsShowOnlineDetail;
        private bool _IsShowTTDetail;

        private ObservableCollection<ReceiptDetail> _RDetails;
        private static List<string> _ReceiptModeList;
        private static List<string> _StatusList;

        #endregion

        #region  Property

        public static List<string> ReceiptModeList
        {
            get
            {
                if (_ReceiptModeList == null)
                {
                    _ReceiptModeList = new List<string>();
                    _ReceiptModeList.Add("Cash");
                    _ReceiptModeList.Add("Cheque");
                    _ReceiptModeList.Add("Online");
                    _ReceiptModeList.Add("TT");
                }
                return _ReceiptModeList;
            }
            set
            {
                if (_ReceiptModeList != value)
                {
                    _ReceiptModeList = value;
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
                    _StatusList.Add("Proccess");
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
        public DateTime ReceiptDate
        {
            get
            {
                return _ReceiptDate;
            }
            set
            {
                if (_ReceiptDate != value)
                {
                    _ReceiptDate = value;
                    NotifyPropertyChanged(nameof(ReceiptDate));
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
        public string ReceiptMode
        {
            get
            {
                return _ReceiptMode;
            }
            set
            {
                if (_ReceiptMode != value)
                {
                    _ReceiptMode = value;
                    NotifyPropertyChanged(nameof(ReceiptMode));
                }
            }
        }
        public decimal Amount
        {
            get
            {
                return _Amount;
            }
            set
            {
                if (_Amount != value)
                {
                    _Amount = value;
                    NotifyPropertyChanged(nameof(Amount));
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
        public string Particulars
        {
            get
            {
                return _Particulars;
            }
            set
            {
                if (_Particulars != value)
                {
                    _Particulars = value;
                    NotifyPropertyChanged(nameof(Particulars));
                }
            }
        }
        public string PayTo
        {
            get
            {
                return _PayTo;
            }
            set
            {
                if (_PayTo != value)
                {
                    _PayTo = value;
                    NotifyPropertyChanged(nameof(PayTo));
                }
            }
        }
        public string VoucherNo
        {
            get
            {
                return _VoucherNo;
            }
            set
            {
                if (_VoucherNo != value)
                {
                    _VoucherNo = value;
                    NotifyPropertyChanged(nameof(VoucherNo));
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

        public string ReceivingMode
        {
            get
            {
                return _ReceivingMode;
            }
            set
            {
                if (_ReceivingMode != value)
                {
                    _ReceivingMode = value;
                    IsShowChequeDetail = value == "Cheque";
                    IsShowOnlineDetail = value == "Online";
                    IsShowTTDetail = value == "TT";
                    NotifyPropertyChanged(nameof(ReceivingMode));
                }
            }
        }

        public ReceiptDetail RDetail
        {
            get
            {
                if (_RDetail == null)
                {
                    _RDetail = new ReceiptDetail();
                }
                return _RDetail;
            }
            set
            {
                if (_RDetail != value)
                {
                    _RDetail = value;
                    NotifyPropertyChanged(nameof(_RDetail));
                }
            }
        }

        public ObservableCollection<ReceiptDetail> RDetails
        {
            get
            {
                if (_RDetails == null) _RDetails = new ObservableCollection<ReceiptDetail>();
                return _RDetails;
            }
            set
            {
                if (_RDetails != value)
                {
                    _RDetails = value;
                    NotifyPropertyChanged(nameof(_RDetails));
                }
            }
        }

        public string SearchText
        {
            get
            {
                return _SearchText;
            }
            set
            {
                if (_SearchText != value)
                {
                    _SearchText = value;
                    NotifyPropertyChanged(nameof(SearchText));
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

        public string AmountInwords
        {
            get
            {
                if (_AmountInwords == null) _AmountInwords = "";
                return _AmountInwords;
            }
            set
            {
                if (_AmountInwords != value)
                {
                    _AmountInwords = value;
                    NotifyPropertyChanged(nameof(AmountInwords));
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
