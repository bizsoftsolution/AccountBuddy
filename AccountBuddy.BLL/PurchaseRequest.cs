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
    public class PurchaseRequest : INotifyPropertyChanged
    {
        #region Field
        private static ObservableCollection<PurchaseRequest> _PRPendingList;

        private long _Id;
        private DateTime? _PRDate;
        private string _RefNo;
        private int _LedgerId;
        private decimal? _ItemAmount;
        private decimal? _DiscountAmount;
        private decimal? _GSTAmount;
        private decimal? _Extras;
        private decimal? _TotalAmount;
        private string _Narration;
        private string _Status;
        private int _RequestTo;

        private string _LedgerName;
        private string _AmountInwords;

        private string _SearchText;

        private PurchaseRequestDetail _PRDetail;
        private ObservableCollection<PurchaseRequestDetail> _PRDetails;
        private string _RefCode;
        private static UserTypeDetail _UserPermission;

        #endregion

        #region Property
        public static UserTypeDetail UserPermission
        {
            get
            {
                if (_UserPermission == null)
                {
                    _UserPermission = UserAccount.User.UserType == null ? new UserTypeDetail() : UserAccount.User.UserType.UserTypeDetails.Where(x => x.UserTypeFormDetail.FormName == Forms.frmPurchaseRequest.ToString()).FirstOrDefault();
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

        public static ObservableCollection<PurchaseRequest> PRPendingList
        {
            get
            {
                if (_PRPendingList == null)
                {
                    _PRPendingList = new ObservableCollection<PurchaseRequest>();
                    //var l1 = FMCGHubClient.FMCGHub.Invoke<List<PurchaseRequest>>("PurchaseRequest_PRPendingList").Result;
                    //_PRPendingList = new ObservableCollection<PurchaseRequest>(l1);
                }
                return _PRPendingList;
            }
            set
            {
                _PRPendingList = value;
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

        public DateTime? PRDate
        {
            get
            {
                return _PRDate;
            }
            set
            {
                if (_PRDate != value)
                {
                    _PRDate = value;
                    NotifyPropertyChanged(nameof(PRDate));
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

        public int RequestTo
        {
            get
            {
                return _RequestTo;
            }
            set
            {
                if (_RequestTo != value)
                {
                    _RequestTo = value;
                    NotifyPropertyChanged(nameof(RequestTo));
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

        public PurchaseRequestDetail PRDetail
        {
            get
            {
                if (_PRDetail == null) _PRDetail = new PurchaseRequestDetail();
                return _PRDetail;
            }
            set
            {
                if (_PRDetail != value)
                {
                    _PRDetail = value;
                    NotifyPropertyChanged(nameof(PRDetail));
                }
            }
        }

        public ObservableCollection<PurchaseRequestDetail> PRDetails
        {
            get
            {
                if (_PRDetails == null) _PRDetails = new ObservableCollection<PurchaseRequestDetail>();
                return _PRDetails;
            }
            set
            {
                if (_PRDetails != value)
                {
                    _PRDetails = value;
                    NotifyPropertyChanged(nameof(PRDetails));
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
                return FMCGHubClient.HubCaller.Invoke<bool>("PurchaseRequest_Save", this).Result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        
        public void Clear()
        {
            new PurchaseRequest().toCopy<PurchaseRequest>(this);
            _PRDetail = new PurchaseRequestDetail();
            _PRDetails = new ObservableCollection<PurchaseRequestDetail>();

            PRDate = DateTime.Now;
            RefNo = FMCGHubClient.HubCaller.Invoke<string>("PurchaseRequest_NewRefNo").Result;
            NotifyAllPropertyChanged();
        }

        private void MaxRef()
        {
            RefNo = FMCGHubClient.HubCaller.Invoke<string>("PurchaseOrder_MaxRef").Result;
        }

        public bool Find()
        {
            try
            {
                PurchaseRequest po = FMCGHubClient.HubCaller.Invoke<PurchaseRequest>("PurchaseRequest_Find", SearchText).Result;
                if (po.Id == 0) return false;
                po.toCopy<PurchaseRequest>(this);
                this.PRDetails = po.PRDetails;
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
                return FMCGHubClient.HubCaller.Invoke<bool>("PurchaseRequest_Delete", this.Id).Result;
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

            PurchaseRequestDetail pod = PRDetails.Where(x => x.ProductId == PRDetail.ProductId).FirstOrDefault();

            if (pod == null)
            {
                pod = new PurchaseRequestDetail();
                PRDetails.Add(pod);
            }
            else
            {
                PRDetail.Quantity += pod.Quantity;
            }
            PRDetail.toCopy<PurchaseRequestDetail>(pod);
            ClearDetail();
            ItemAmount = PRDetails.Sum(x => x.Amount);


        }

        public void ClearDetail()
        {
            PurchaseRequestDetail pod = new PurchaseRequestDetail();
            pod.toCopy<PurchaseRequestDetail>(PRDetail);
        }

        public void DeleteDetail(string PName)
        {
            PurchaseRequestDetail pod = PRDetails.Where(x => x.ProductName == PName).FirstOrDefault();

            if (pod != null)
            {
                PRDetails.Remove(pod);
                ItemAmount = PRDetails.Sum(x => x.Amount);
            }
        }
        #endregion
        public static bool Reject(long PurchaseRequestStatusDetailId)
        {
            return FMCGHubClient.HubCaller.Invoke<bool>("PurchaseRequest_Reject", PurchaseRequestStatusDetailId).Result;
        }

        public static bool Approval(long PurchaseRequestStatusDetailId)
        {
            return FMCGHubClient.HubCaller.Invoke<bool>("PurchaseRequest_Approval", PurchaseRequestStatusDetailId).Result;
        }

        private void SetAmount()
        {

            GSTAmount = ((ItemAmount ?? 0) - (DiscountAmount ?? 0)) * Common.AppLib.GSTPer;
            TotalAmount = (ItemAmount ?? 0) - (DiscountAmount ?? 0) + GSTAmount + (Extras ?? 0);
        }

        public bool FindRefNo()
        {
            var rv = false;
            try
            {
                rv = FMCGHubClient.HubCaller.Invoke<bool>("Find_PRQRef", RefNo, this).Result;
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
