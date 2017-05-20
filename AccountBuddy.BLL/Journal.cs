using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBuddy.BLL
{
    public class Journal : INotifyPropertyChanged
    {
        #region Fields
        private long _Id;
        private string _EntryNo;
        private DateTime _JournalDate;
        private decimal _Amount;
        private string _Particular;
        private string _HQNo;
        private string _VoucherNo;
        private string _Status;

        private string _AmountInwords;

        private Ledger _JLedger;

        private JournalDetail _JDetail;

        private string _SearchText;

        private string _PayMode;

        private bool _IsShowChequeDetail;
        private bool _IsShowOnlineDetail;
        private bool _IsShowTTDetail;

        private ObservableCollection<JournalDetail> _JDetails;
        private static List<string> _PayModeList;
        private static List<string> _StatusList;
       
        #endregion

        #region Property

        public static List<string> PayModeList
        {
            get
            {
                if (_PayModeList == null)
                {
                    _PayModeList = new List<string>();
                    _PayModeList.Add("Cash");
                    _PayModeList.Add("Cheque");
                    _PayModeList.Add("Online");
                    _PayModeList.Add("TT");
                }
                return _PayModeList;
            }
            set
            {
                if (_PayModeList != value)
                {
                    _PayModeList = value;
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
        public DateTime JournalDate
        {
            get
            {
                return _JournalDate;
            }
            set
            {
                if (_JournalDate != value)
                {
                    _JournalDate = value;
                    NotifyPropertyChanged(nameof(JournalDate));
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

        public string HQNo
        {
            get
            {
                return _HQNo;
            }
            set
            {
                if (_HQNo != value)
                {
                    _HQNo = value;
                    NotifyPropertyChanged(nameof(HQNo));
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

        public JournalDetail JDetail
        {
            get
            {
                if (_JDetail == null)
                {
                    _JDetail = new JournalDetail();
                }
                return _JDetail;
            }
            set
            {
                if (_JDetail != value)
                {
                    _JDetail = value;
                    NotifyPropertyChanged(nameof(JDetail));
                }
            }
        }

        public ObservableCollection<JournalDetail> JDetails
        {
            get
            {
                if (_JDetails == null) _JDetails = new ObservableCollection<JournalDetail>();
                return _JDetails;
            }
            set
            {
                if (_JDetails != value)
                {
                    _JDetails = value;
                    NotifyPropertyChanged(nameof(JDetails));
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
