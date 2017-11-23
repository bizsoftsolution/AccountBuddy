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
    public class JobOrderReceived : INotifyPropertyChanged
    {
        #region Field
        private static ObservableCollection<JobOrderReceived> _JRPendingList;

        private long _Id;
        private DateTime? _JRDate;
        private string _RefNo;
        private string _JRNo;
        private int? _JobWorkerId;
        private decimal? _ItemAmount;
        private decimal? _DiscountAmount;
        private decimal? _GSTAmount;
        private decimal? _ExtraAmount;
        private decimal? _TotalAmount;
        private string _Narration;
        private int? _CompanyId;

        private string _JobWorkerName;
        private string _AmountInwords;

        private string _SearchText;

        private JobOrderReceivedDetail _JRDetail;
        private ObservableCollection<JobOrderReceivedDetail> _JRDetails;
        private string _Status;
        private string _RefCode;
        private static UserTypeDetail _UserPermission;

        #endregion

        #region Property

        public static ObservableCollection<JobOrderReceived> JRPendingList
        {
            get
            {
                if (_JRPendingList== null)
                {
                    _JRPendingList = new ObservableCollection<JobOrderReceived>();
                    var l1 = FMCGHubClient.FMCGHub.Invoke<List<JobOrderReceived>>("JobOrderReceived_JRPendingList").Result;
                    _JRPendingList = new ObservableCollection<JobOrderReceived>(l1);
                }
                return _JRPendingList;
            }
            set
            {
                _JRPendingList = value;
            }
        }
        public static UserTypeDetail UserPermission
        {
            get
            {
                if (_UserPermission == null)
                {
                    _UserPermission = UserAccount.User.UserType == null ? new UserTypeDetail() : UserAccount.User.UserType.UserTypeDetails.Where(x => x.UserTypeFormDetail.FormName == Forms.frmJobOrderReceived.ToString()).FirstOrDefault();
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

        public DateTime? JRDate
        {
            get
            {
                return _JRDate;
            }
            set
            {
                if (_JRDate != value)
                {
                    _JRDate = value;
                    NotifyPropertyChanged(nameof(JRDate));
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
        public string JRNo
        {
            get
            {
                return _JRNo;
            }
            set
            {
                if (_JRNo != value)
                {
                    _JRNo = value;
                    NotifyPropertyChanged(nameof(JRNo));
                }
            }
        }
        public int? JobWorkerId
        {
            get
            {
                return _JobWorkerId;
            }
            set
            {
                if (_JobWorkerId != value)
                {
                    _JobWorkerId = value;
                    NotifyPropertyChanged(nameof(JobWorkerId));
                }
            }
        }
        public decimal? ItemAmount
        {
            get
            {
                if (_ItemAmount == null) _ItemAmount = 0;
                return _ItemAmount;
            }
            set
            {
                if (_ItemAmount != value)
                {
                    _ItemAmount = value;
                    NotifyPropertyChanged(nameof(ItemAmount));
                    if (value != null) SetAmount();
                }
            }
        }
        public decimal? DiscountAmount
        {
            get
            {
                if (_DiscountAmount == null) _DiscountAmount = 0;
                return _DiscountAmount;
            }
            set
            {
                if (_DiscountAmount != value)
                {
                    _DiscountAmount = value;
                    NotifyPropertyChanged(nameof(DiscountAmount));
                    if (value != null) SetAmount();
                }
            }
        }
        public decimal? GSTAmount
        {
            get
            {
                if (_GSTAmount == null) _GSTAmount = 0;
                return _GSTAmount;
            }
            set
            {
                if (_GSTAmount != value)
                {
                    _GSTAmount = value;
                    NotifyPropertyChanged(nameof(GSTAmount));
                }
            }
        }
        public decimal? ExtraAmount
        {
            get
            {
                if (_ExtraAmount == null) _ExtraAmount = 0;
                return _ExtraAmount;
            }
            set
            {
                if (_ExtraAmount != value)
                {
                    _ExtraAmount = value;
                    NotifyPropertyChanged(nameof(ExtraAmount));
                    if (value != null) SetAmount();
                }
            }
        }
        public decimal? TotalAmount
        {
            get
            {
                if (_TotalAmount == null) _TotalAmount = 0;
                return _TotalAmount;
            }
            set
            {
                if (_TotalAmount != value)
                {
                    _TotalAmount = value;
                    NotifyPropertyChanged(nameof(TotalAmount));
                    AmountInwords = value.ToCurrencyInWords();
                }
            }
        }
        public string Narration
        {
            get
            {
                return _Narration;
            }
            set
            {
                if (_Narration != value)
                {
                    _Narration = value;
                    NotifyPropertyChanged(nameof(Narration));
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

        public string JobWorkerName
        {
            get
            {
                return _JobWorkerName;
            }
            set
            {
                if (_JobWorkerName != value)
                {
                    _JobWorkerName = value;
                    NotifyPropertyChanged(nameof(JobWorkerName));
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

        public JobOrderReceivedDetail JRDetail
        {
            get
            {
                if (_JRDetail == null) _JRDetail = new JobOrderReceivedDetail();
                return _JRDetail;
            }
            set
            {
                if (_JRDetail != value)
                {
                    _JRDetail = value;
                    NotifyPropertyChanged(nameof(JRDetail));
                }
            }
        }

        public ObservableCollection<JobOrderReceivedDetail> JRDetails
        {
            get
            {
                if (_JRDetails == null) _JRDetails = new ObservableCollection<JobOrderReceivedDetail>();
                return _JRDetails;
            }
            set
            {
                if (_JRDetails != value)
                {
                    _JRDetails = value;
                    NotifyPropertyChanged(nameof(JRDetails));
                }
            }
        }

        #endregion

        #region Property Changed
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String ProperName)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(ProperName));
        }
        private void NotifyAllPropertyChanged()
        {
            foreach (var p in this.GetType().GetProperties()) NotifyPropertyChanged(p.Name);
        }

        #endregion

        #region Methods

        #region Master
        public bool Save()
        {
            try
            {
                return FMCGHubClient.FMCGHub.Invoke<bool>("JobOrderReceived_Save", this).Result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        

        public void Clear()
        {
            new JobOrderReceived().toCopy<JobOrderReceived>(this);
            _JRDetail = new JobOrderReceivedDetail();
            _JRDetails = new ObservableCollection<JobOrderReceivedDetail>();

            JRDate = DateTime.Now;
            RefNo = FMCGHubClient.FMCGHub.Invoke<string>("JobOrderReceived_NewRefNo").Result;
            NotifyAllPropertyChanged();
        }

        public bool Find()
        {
            try
            {
                JobOrderReceived po = FMCGHubClient.FMCGHub.Invoke<JobOrderReceived>("JobOrderReceived_Find", SearchText).Result;
                if (po.Id == 0) return false;
                po.toCopy<JobOrderReceived>(this);
                this.JRDetails = po.JRDetails;
                NotifyAllPropertyChanged();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete()
        {
            try
            {
                return FMCGHubClient.FMCGHub.Invoke<bool>("JobOrderReceived_Delete", this.Id).Result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool FindById(int Id)
        {
            try
            {
                JobOrderReceived po = FMCGHubClient.FMCGHub.Invoke<JobOrderReceived>("JobOrderReceived_FindById", Id).Result;
                if (po.Id == 0) return false;
                po.toCopy<JobOrderReceived>(this);
                this.JRDetails = po.JRDetails;
                NotifyAllPropertyChanged();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region Detail

        public void SaveDetail()
        {
            if (JRDetail.ProductId != 0)
            {
                JobOrderReceivedDetail pod = JRDetails.Where(x => x.ProductId == JRDetail.ProductId).FirstOrDefault();

                if (pod == null)
                {
                    pod = new JobOrderReceivedDetail();
                    JRDetails.Add(pod);
                }
                else
                {
                    JRDetail.Quantity += pod.Quantity;
                }
                JRDetail.toCopy<JobOrderReceivedDetail>(pod);
                ClearDetail();
                ItemAmount = JRDetails.Sum(x => x.Amount);

            }

        }

        public void ClearDetail()
        {
            JobOrderReceivedDetail pod = new JobOrderReceivedDetail();
            pod.toCopy<JobOrderReceivedDetail>(JRDetail);
        }

        public void DeleteDetail(string PName)
        {
            JobOrderReceivedDetail pod = JRDetails.Where(x => x.ProductName == PName).FirstOrDefault();

            if (pod != null)
            {
                JRDetails.Remove(pod);
                ItemAmount = JRDetails.Sum(x => x.Amount);
            }
        }
        #endregion


        private void SetAmount()
        {
            GSTAmount = ((ItemAmount ?? 0) - (DiscountAmount ?? 0)) * Common.AppLib.GSTPer;
            TotalAmount = (ItemAmount ?? 0) - (DiscountAmount ?? 0) + GSTAmount + (ExtraAmount ?? 0);
        }

        public bool FindRefNo()
        {
            var rv = false;
            try
            {
                rv = FMCGHubClient.FMCGHub.Invoke<bool>("Find_JobReceiveRef", RefNo, this).Result;
            }
            catch (Exception ex)
            {
                rv = true;
            }
            return rv;
        }
        #endregion

    }
}
