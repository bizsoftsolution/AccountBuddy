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
    public class JobOrderIssue : INotifyPropertyChanged
    {
        #region Field
        private static ObservableCollection<JobOrderIssue> _JOPendingList;

        private long _Id;
        private DateTime? _JODate;
        private string _RefNo;
        private string _JONo;
        private int? _JobWorkerId;
        private decimal? _ItemAmount;
        private decimal? _DiscountAmount;
        private decimal? _GSTAmount;
        private decimal? _Extras;
        private decimal? _TotalAmount;
        private string _Narration;
        private int? _CompanyId;

        private string _JobWorkerName;
        private string _AmountInwords;

        private string _SearchText;

        private JobOrderIssueDetail _JODetail;
        private ObservableCollection<JobOrderIssueDetail> _JODetails;
        private string _Status;
        private string _RefCode;
        private static UserTypeDetail _UserPermission;
        private string _lblDiscount;
        private string _lblExtra;

        #endregion

        #region Property
        public static UserTypeDetail UserPermission
        {
            get
            {
                if (_UserPermission == null)
                {
                    _UserPermission = UserAccount.User.UserType == null ? new UserTypeDetail() : UserAccount.User.UserType.UserTypeDetails.Where(x => x.UserTypeFormDetail.FormName == Forms.frmJobOrderIssue.ToString()).FirstOrDefault();
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

        public static ObservableCollection<JobOrderIssue> JOPendingList
        {
            get
            {
                if (_JOPendingList == null)
                {
                    try
                    {
                        _JOPendingList = new ObservableCollection<JobOrderIssue>();
                        var l1 = FMCGHubClient.HubCaller.Invoke<List<JobOrderIssue>>("JobOrderIssue_JOPendingList").Result;
                        _JOPendingList = new ObservableCollection<JobOrderIssue>(l1);
                    }
                    catch(Exception ex)
                    {
                        Common.AppLib.WriteLog(string.Format("JOPendingList_{0}_{1}", ex.Message, ex.InnerException));
                    }
                }
                return _JOPendingList;
            }
            set
            {
                _JOPendingList = value;
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

        public DateTime? JODate
        {
            get
            {
                return _JODate;
            }
            set
            {
                if (_JODate != value)
                {
                    _JODate = value;
                    NotifyPropertyChanged(nameof(JODate));
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
        public string JONo
        {
            get
            {
                return _JONo;
            }
            set
            {
                if (_JONo != value)
                {
                    _JONo = value;
                    NotifyPropertyChanged(nameof(JONo));
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
                     SetAmount();
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
                     SetAmount();
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
        public decimal? Extras
        {
            get
            {
                if (_Extras == null) _Extras = 0;
                return _Extras;
            }
            set
            {
                if (_Extras != value)
                {
                    _Extras = value;
                    NotifyPropertyChanged(nameof(Extras));
                     SetAmount();
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

        public JobOrderIssueDetail JODetail
        {
            get
            {
                if (_JODetail == null) _JODetail = new JobOrderIssueDetail();
                return _JODetail;
            }
            set
            {
                if (_JODetail != value)
                {
                    _JODetail = value;
                    NotifyPropertyChanged(nameof(_JODetail));
                }
            }
        }

        public ObservableCollection<JobOrderIssueDetail> JODetails
        {
            get
            {
                if (_JODetails == null) _JODetails = new ObservableCollection<JobOrderIssueDetail>();
                return _JODetails;
            }
            set
            {
                if (_JODetails != value)
                {
                    _JODetails = value;
                    NotifyPropertyChanged(nameof(_JODetails));
                }
            }
        }
        public string lblDiscount
        {
            get
            {
                return _lblDiscount;
            }
            set
            {
                if (_lblDiscount != value)
                {
                    _lblDiscount = value;
                    NotifyPropertyChanged(nameof(lblDiscount));

                }
            }
        }

        public string lblExtra
        {
            get
            {
                return _lblExtra;
            }
            set
            {
                if (_lblExtra != value)
                {
                    _lblExtra = value;
                    NotifyPropertyChanged(nameof(lblExtra));

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

        public bool MakeReceived()
        {
             
            try
            {
                return FMCGHubClient.HubCaller.Invoke<bool>("JobOrderIssue_MakeReceieved", this).Result;
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex);
                return false;
            }
        
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
                return FMCGHubClient.HubCaller.Invoke<bool>("JobOrderIssue_Save", this).Result;
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex);
                return false;
            }
        }

        public void Clear()
        {try
            {
                new JobOrderIssue().ToMap(this);
                _JODetail = new JobOrderIssueDetail();
                _JODetails = new ObservableCollection<JobOrderIssueDetail>();

                JODate = DateTime.Now;
                RefNo = FMCGHubClient.HubCaller.Invoke<string>("JobOrderIssue_NewRefNo").Result;
                NotifyAllPropertyChanged();

            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex);
            }
        }

        public bool Find()
        {
            try
            {
                JobOrderIssue po = FMCGHubClient.HubCaller.Invoke<JobOrderIssue>("JobOrderIssue_Find", RefNo).Result;
                if (po.Id == 0) return false;
                po.ToMap(this);
                this.JODetails = po.JODetails;
                NotifyAllPropertyChanged();
                return true;
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex);
                return false;
            }
        }

        public bool Delete()
        {
            try
            {
                return FMCGHubClient.HubCaller.Invoke<bool>("JobOrderIssue_Delete", this.Id).Result;
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex);
                return false;
            }
        }

        public bool FindById(int Id)
        {
            try
            {
                JobOrderIssue po = FMCGHubClient.HubCaller.Invoke<JobOrderIssue>("JobOrderIssue_FindById", Id).Result;
                if (po.Id == 0) return false;
                po.ToMap(this);
                this.JODetails = po.JODetails;
                NotifyAllPropertyChanged();
                return true;
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex);
                return false;
            }
        }
        #endregion

        #region Detail

        public void SaveDetail()
        {
            if (JODetail.ProductId != 0)
            {
                JobOrderIssueDetail pod = JODetails.Where(x => x.SNo == JODetail.SNo).FirstOrDefault();

                if (pod == null)
                {
                    pod = new JobOrderIssueDetail();
                    JODetails.Add(pod);
                }
                JODetail.ToMap(pod);
                ClearDetail();
                ItemAmount = JODetails.Sum(x => x.Amount);

            }

        }

        public void ClearDetail()
        {
            JobOrderIssueDetail pod = new JobOrderIssueDetail();
            pod.SNo = JODetails.Count == 0 ? 1 : JODetails.Max(x => x.SNo) + 1;
            pod.ToMap(JODetail);
        }

        public void DeleteDetail(int SNo)
        {
            JobOrderIssueDetail pod = JODetails.Where(x => x.SNo == SNo).FirstOrDefault();

            if (pod != null)
            {
                JODetails.Remove(pod);
                ItemAmount = JODetails.Sum(x => x.Amount);
                ClearDetail();
            }
        }
        #endregion

        private void SetAmount()
        {
            GSTAmount = (ItemAmount  - DiscountAmount ) * Common.AppLib.GSTPer;
            TotalAmount = (ItemAmount -DiscountAmount ) + GSTAmount + (Extras ?? 0);
            setLabel();

        }
        public void setLabel()
        {
            lblDiscount = string.Format("{0}({1})", "Discount Amount", AppLib.CurrencyPositiveSymbolPrefix);
            lblExtra = string.Format("{0}({1})", "Extra Amount", AppLib.CurrencyPositiveSymbolPrefix);

        }

        public bool FindRefNo()
        {
            var rv = false;
            try
            {
                rv = FMCGHubClient.HubCaller.Invoke<bool>("Find_JOIssueRef", RefNo, this).Result;

            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex);
                rv = true;
            }
            return rv;
        }

        public static List<JobOrderIssue> ToList(int? LedgerId,DateTime dtFrom, DateTime dtTo, string BillNo, decimal amtFrom, decimal amtTo)
        {
            List<JobOrderIssue> rv = new List<JobOrderIssue>();
            try
            {
                rv = FMCGHubClient.HubCaller.Invoke<List<JobOrderIssue>>("JobOrderIssue_List", LedgerId, dtFrom, dtTo, BillNo, amtFrom, amtTo).Result;
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(string.Format("JobOrderIssue List= {0}-{1}", ex.Message, ex.InnerException));
            }
            return rv;

        }


        #endregion
    }
}
