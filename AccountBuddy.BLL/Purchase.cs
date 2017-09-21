using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountBuddy.Common;
using System.Collections.ObjectModel;

namespace AccountBuddy.BLL
{
    public class Purchase : INotifyPropertyChanged
    {

        #region Field

        private long _Id;
        private DateTime _PurchaseDate;
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

        private PurchaseDetail _PDetail;
        private ObservableCollection<PurchaseDetail> _PDetails;
        private string _RefCode;
        private decimal _CGSTAmount;
        private decimal _SGSTAmount;
        private decimal _IGSTAmount;
        private decimal _TotalGST;
        private decimal _IGSTPer;
        private decimal _SGSTPer;
        private decimal _CGSTPer;

        #endregion

        #region Property

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

        public DateTime PurchaseDate
        {
            get
            {
                return _PurchaseDate;
            }
            set
            {
                if (_PurchaseDate != value)
                {
                    _PurchaseDate = value;
                    NotifyPropertyChanged(nameof(PurchaseDate));
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
        public decimal CGSTPer
        {
            get
            {
                return _CGSTPer;
            }
            set
            {
                if (_CGSTPer != value)
                {
                    _CGSTPer = value;
                    NotifyPropertyChanged(nameof(CGSTPer));
                    SetAmount();
                }
            }
        }
        public decimal SGSTPer
        {
            get
            {
                return _SGSTPer;
            }
            set
            {
                if (_SGSTPer != value)
                {
                    _SGSTPer = value;
                    NotifyPropertyChanged(nameof(SGSTPer));
                    SetAmount();
                }
            }
        }
        public decimal IGSTPer
        {
            get
            {
                return _IGSTPer;
            }
            set
            {
                if (_IGSTPer != value)
                {
                    _IGSTPer = value;
                    NotifyPropertyChanged(nameof(IGSTPer));
                   SetAmount();
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
                return _TotalAmount - _PaidAmount;
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

        public PurchaseDetail PDetail
        {
            get
            {
                if (_PDetail == null) _PDetail = new PurchaseDetail();
                return _PDetail;
            }
            set
            {
                if (_PDetail != value)
                {
                    _PDetail = value;
                    NotifyPropertyChanged(nameof(PDetail));
                }
            }
        }
        public long PaymentLedgerId{get;set;}
        public ObservableCollection<PurchaseDetail> PDetails
        {
            get
            {
                if (_PDetails == null) _PDetails = new ObservableCollection<PurchaseDetail>();
                return _PDetails;
            }
            set
            {
                if (_PDetails != value)
                {
                    _PDetails = value;
                    NotifyPropertyChanged(nameof(PDetails));
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
                return FMCGHubClient.FMCGHub.Invoke<bool>("Purchase_Save", this).Result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool Save_To_Purchase()
        {
            try
            {
                return FMCGHubClient.FMCGHub.Invoke<bool>("PurchaseOrder_Save", this).Result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public void Clear()
        {
            new Purchase().toCopy<Purchase>(this);
            this.PDetail = new PurchaseDetail();
            this.PDetails = new ObservableCollection<PurchaseDetail>();

            PurchaseDate = DateTime.Now;
            RefNo = FMCGHubClient.FMCGHub.Invoke<string>("Purchase_NewRefNo").Result;
            NotifyAllPropertyChanged();
        }

        public bool Find()
        {
            try
            {
                Purchase po = FMCGHubClient.FMCGHub.Invoke<Purchase>("Purchase_Find", RefNo).Result;
                if (po.Id == 0) return false;
                po.toCopy<Purchase>(this);
                this.PDetails = po.PDetails;
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
                Purchase po = FMCGHubClient.FMCGHub.Invoke<Purchase>("Purchase_FindById", Id).Result;
                if (po.Id == 0) return false;
                po.toCopy<Purchase>(this);
                this.PDetails = po.PDetails;
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
                return FMCGHubClient.FMCGHub.Invoke<bool>("Purchase_Delete", this.Id).Result;
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
            if (PDetail.ProductId != 0)
            {
                PurchaseDetail pod = PDetails.Where(x => x.ProductId == PDetail.ProductId).FirstOrDefault();

                if (pod == null)
                {
                    pod = new PurchaseDetail();
                    PDetails.Add(pod);
                }
                else
                {
                    PDetail.Quantity += pod.Quantity;
                }
                PDetail.toCopy<PurchaseDetail>(pod);
                ClearDetail();
                ItemAmount = PDetails.Sum(x => x.Amount);
            }

        }

        public void ClearDetail()
        {
            PurchaseDetail pod = new PurchaseDetail();
            pod.toCopy<PurchaseDetail>(PDetail);
        }

        public void DeleteDetail(string PName)
        {
            PurchaseDetail pod = PDetails.Where(x => x.ProductName == PName).FirstOrDefault();

            if (pod != null)
            {
                PDetails.Remove(pod);
                ItemAmount = PDetails.Sum(x => x.Amount);
                ClearDetail();
            }
        }

        #endregion

        public void SetAmount()
        {
            CGSTAmount = (CGSTPer / 100) * (ItemAmount - DiscountAmount);
            SGSTAmount = (SGSTPer / 100) * (ItemAmount - DiscountAmount);
            IGSTAmount = (IGSTPer / 100) * (ItemAmount - DiscountAmount);
            TotalGST = (CGSTAmount + SGSTAmount + IGSTAmount);
            GSTAmount = TotalGST;
            // GSTAmount = (ItemAmount - DiscountAmount ) + TotalGST;
            TotalAmount = ItemAmount - DiscountAmount + GSTAmount + ExtraAmount;
        }
        
        public bool FindRefNo()
        {
            var rv = false;
            try
            {
                rv = FMCGHubClient.FMCGHub.Invoke<bool>("Find_PRef", RefNo, this).Result;
            }
            catch (Exception ex)
            {
                rv = true;
            }
            return rv;
        }
        public static List<BLL.Purchase> tolist(int? SID, DateTime dtFrom, DateTime dtTo, string InvoiceNo)
        {
            return FMCGHubClient.FMCGHub.Invoke<List<BLL.Purchase>>("Purchase_List", SID, dtFrom, dtTo, InvoiceNo).Result;
        }
        #endregion

    }
}
