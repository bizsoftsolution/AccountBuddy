using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using AccountBuddy.Common;

namespace AccountBuddy.BLL
{
    public class PurchaseReturn:INotifyPropertyChanged
    {
        #region Field

        private long _Id;
        private DateTime _PRDate;
        private string _RefNo;        
        private int _LedgerId;
        private int _TransactionTypeId;
        private decimal _ItemAmount;
        private decimal _DiscountAmount;
        private decimal _GSTAmount;
        private decimal _ExtraAmount;
        private decimal _TotalAmount;
        private string _Narration;

        private decimal? _PaidAmount;
        private decimal? _PayAmount;
        private string _LedgerName;
        private string _TransactionType;
        private string _AmountInwords;

        private string _SearchText;

        private PurchaseReturnDetail _PRDetail;
        private ObservableCollection<PurchaseReturnDetail> _PRDetails;
        private string _RefCode;
        private decimal _TotalGST;
        private decimal _IGSTAmount;
        private decimal _SGSTAmount;
        private decimal _CGSTAmount;

        #endregion

        #region Property
        public long ReceiptLedgerId { get; set; }
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

        public DateTime PRDate
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
        public int TransactionTypeId
        {
            get
            {
                return _TransactionTypeId;
            }
            set
            {
                if (_TransactionTypeId != value)
                {
                    _TransactionTypeId = value;
                    NotifyPropertyChanged(nameof(TransactionTypeId));
                }
            }
        }
        public decimal ItemAmount
        {
            get
            {
                return _ItemAmount;
            }
            set
            {
                if (_ItemAmount != value)
                {
                    _ItemAmount = value;
                    NotifyPropertyChanged(nameof(ItemAmount));
                    if (value != 0) SetAmount();
                }
            }
        }
        public decimal DiscountAmount
        {
            get
            {
                return _DiscountAmount;
            }
            set
            {
                if (_DiscountAmount != value)
                {
                    _DiscountAmount = value;
                    NotifyPropertyChanged(nameof(DiscountAmount));
                    if (value != 0) SetAmount();
                }
            }
        }
        public decimal CGSTAmount
        {
            get
            {
                return _CGSTAmount;
            }
            set
            {
                if (_CGSTAmount != value)
                {
                    _CGSTAmount = value;
                    NotifyPropertyChanged(nameof(CGSTAmount));
                    if (value != 0) SetAmount();
                }
            }
        }
        public decimal SGSTAmount
        {
            get
            {
                return _SGSTAmount;
            }
            set
            {
                if (_SGSTAmount != value)
                {
                    _SGSTAmount = value;
                    NotifyPropertyChanged(nameof(SGSTAmount));
                    if (value != 0) SetAmount();
                }
            }
        }
        public decimal IGSTAmount
        {
            get
            {
                return _IGSTAmount;
            }
            set
            {
                if (_IGSTAmount != value)
                {
                    _IGSTAmount = value;
                    NotifyPropertyChanged(nameof(IGSTAmount));
                    if (value != 0) SetAmount();
                }
            }
        }
        public decimal TotalGST
        {
            get
            {
                return _TotalGST;
            }
            set
            {
                if (_TotalGST != value)
                {
                    _TotalGST = value;
                    NotifyPropertyChanged(nameof(TotalGST));
                    if (value != 0) SetAmount();
                }
            }
        }
        public decimal GSTAmount
        {
            get
            {
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
        public decimal ExtraAmount
        {
            get
            {
                return _ExtraAmount;
            }
            set
            {
                if (_ExtraAmount != value)
                {
                    _ExtraAmount = value;
                    NotifyPropertyChanged(nameof(ExtraAmount));
                    if (value != 0) SetAmount();
                }
            }
        }
        public decimal TotalAmount
        {
            get
            {
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

        public decimal? PaidAmount
        {
            get
            {
                if (_PaidAmount == null) _PaidAmount = 0;
                return _PaidAmount;
            }
            set
            {
                if (_PaidAmount != value)
                {
                    _PaidAmount = value;
                    NotifyPropertyChanged(nameof(PaidAmount));
                    AmountInwords = value.ToCurrencyInWords();
                }
            }
        }
        public decimal? BalanceAmount
        {
            get
            {
                if (_TotalAmount == 0) return null;
                if (_PaidAmount == null) return _TotalAmount;
                return _TotalAmount - _PaidAmount.Value;
            }
        }
        public decimal? PayAmount
        {
            get
            {
                if (_PayAmount == null) _PayAmount = 0;
                return _PayAmount;
            }
            set
            {
                if (_PayAmount != value)
                {
                    _PayAmount = value;
                    NotifyPropertyChanged(nameof(PayAmount));
                    AmountInwords = value.ToCurrencyInWords();
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
        public string TransactionType
        {
            get
            {
                return _TransactionType;
            }
            set
            {
                if (_TransactionType != value)
                {
                    _TransactionType = value;
                    NotifyPropertyChanged(nameof(TransactionType));
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

        public PurchaseReturnDetail PRDetail
        {
            get
            {
                if (_PRDetail == null) _PRDetail = new PurchaseReturnDetail();
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

        public ObservableCollection<PurchaseReturnDetail> PRDetails
        {
            get
            {
                if (_PRDetails == null) _PRDetails = new ObservableCollection<PurchaseReturnDetail>();
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
                return FMCGHubClient.FMCGHub.Invoke<bool>("PurchaseReturn_Save", this).Result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void Clear()
        {
            new PurchaseReturn().toCopy<PurchaseReturn>(this);
            this.PRDetail = new PurchaseReturnDetail();
            this.PRDetails = new ObservableCollection<PurchaseReturnDetail>();

            PRDate = DateTime.Now;
            RefNo = FMCGHubClient.FMCGHub.Invoke<string>("PurchaseReturn_NewRefNo").Result;
            NotifyAllPropertyChanged();
        }

        public bool Find()
        {
            try
            {
                PurchaseReturn po = FMCGHubClient.FMCGHub.Invoke<PurchaseReturn>("PurchaseReturn_Find", RefNo).Result;
                if (po.Id == 0) return false;
                po.toCopy<PurchaseReturn>(this);
                this.PRDetails = po.PRDetails;
                NotifyAllPropertyChanged();
                return true;
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
                PurchaseReturn po = FMCGHubClient.FMCGHub.Invoke<PurchaseReturn>("PurchaseReturn_FindById", Id).Result;
                if (po.Id == 0) return false;
                po.toCopy<PurchaseReturn>(this);
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
                return FMCGHubClient.FMCGHub.Invoke<bool>("PurchaseReturn_Delete", this.Id).Result;
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
            if (PRDetail.ProductId != 0)
            {
                PurchaseReturnDetail pod = PRDetails.Where(x => x.ProductId == PRDetail.ProductId).FirstOrDefault();

                if (pod == null)
                {
                    pod = new PurchaseReturnDetail();
                    PRDetails.Add(pod);
                }
                else
                {
                    PRDetail.Quantity += pod.Quantity;
                }
                PRDetail.toCopy<PurchaseReturnDetail>(pod);
                ClearDetail();
                ItemAmount = PRDetails.Sum(x => x.Amount);
            }

        }

        public void ClearDetail()
        {
            PurchaseReturnDetail pod = new PurchaseReturnDetail();
            pod.toCopy<PurchaseReturnDetail>(PRDetail);
        }

        public void DeleteDetail(string PName)
        {
            PurchaseReturnDetail pod = PRDetails.Where(x => x.ProductName == PName).FirstOrDefault();

            if (pod != null)
            {
                PRDetails.Remove(pod);
                ItemAmount = PRDetails.Sum(x => x.Amount);
            }
        }

        #endregion

        public void SetAmount()
        {
            CGSTAmount = Common.AppLib.CGSTPer / 100;
            SGSTAmount = Common.AppLib.SGSTPer / 100;
            IGSTAmount = Common.AppLib.IGSTPer / 100;
            TotalGST = (CGSTAmount + SGSTAmount + IGSTAmount);
            GSTAmount = (ItemAmount - DiscountAmount) * (TotalGST);
            TotalAmount = ItemAmount - DiscountAmount + GSTAmount + ExtraAmount;
        }

        public bool FindRefNo()
        {
            var rv = false;
            try
            {
                rv = FMCGHubClient.FMCGHub.Invoke<bool>("Find_PRRef", RefNo, this).Result;
            }
            catch (Exception ex)
            {
                rv = true;
            }
            return rv;
        }
        public static List<BLL.PurchaseReturn> tolist(int? SID, DateTime dtFrom, DateTime dtTo, string InvoiceNo)
        {
            return FMCGHubClient.FMCGHub.Invoke<List<BLL.PurchaseReturn>>("PurchaseReturn_List", SID, dtFrom, dtTo, InvoiceNo).Result;
        }
        #endregion
    }
}
