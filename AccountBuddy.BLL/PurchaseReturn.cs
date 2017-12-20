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
        private string _ChequeNo;
        private DateTime? _ChequeDate;
        private string _BankName;
        private bool _IsShowChequeDetail;
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
                    _UserPermission = UserAccount.User.UserType == null ? new UserTypeDetail() : UserAccount.User.UserType.UserTypeDetails.Where(x => x.UserTypeFormDetail.FormName == Forms.frmPurchaseReturn.ToString()).FirstOrDefault();
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
                   SetAmount();
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
        public DateTime? ChequeDate
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
        public string BankName
        {
            get
            {
                return _BankName;
            }
            set
            {
                if (_BankName != value)
                {
                    _BankName = value;
                    NotifyPropertyChanged(nameof(BankName));
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
                     SetAmount();
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
                    SetAmount();
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
                    IsShowChequeDetail = value == "Cheque";
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
                return FMCGHubClient.HubCaller.Invoke<bool>("PurchaseReturn_Save", this).Result;
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex); return false;
            }
        }
        public void setLabel()
        {
            lblDiscount = string.Format("{0}({1})", "Discount Amount", AppLib.CurrencyPositiveSymbolPrefix);
            lblExtra = string.Format("{0}({1})", "Extra Amount", AppLib.CurrencyPositiveSymbolPrefix);

        }
        public void Clear()
        {
            new PurchaseReturn().toCopy<PurchaseReturn>(this);
            this.PRDetail = new PurchaseReturnDetail();
            this.PRDetails = new ObservableCollection<PurchaseReturnDetail>();

            PRDate = DateTime.Now;
            RefNo = FMCGHubClient.HubCaller.Invoke<string>("PurchaseReturn_NewRefNo").Result;
            NotifyAllPropertyChanged();
        }

        public bool Find()
        {
            try
            {
                PurchaseReturn po = FMCGHubClient.HubCaller.Invoke<PurchaseReturn>("PurchaseReturn_Find", SearchText).Result;
                if (po.Id == 0) return false;
                po.toCopy<PurchaseReturn>(this);
                this.PRDetails = po.PRDetails;
                NotifyAllPropertyChanged();
                return true;
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex); return false;
            }
        }
        public bool FindById(int Id)
        {
            try
            {
                PurchaseReturn po = FMCGHubClient.HubCaller.Invoke<PurchaseReturn>("PurchaseReturn_FindById", Id).Result;
                if (po.Id == 0) return false;
                po.toCopy<PurchaseReturn>(this);
                this.PRDetails = po.PRDetails;
                NotifyAllPropertyChanged();
                return true;
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex); return false;
            }
        }
        public bool Delete()
        {
            try
            {
                return FMCGHubClient.HubCaller.Invoke<bool>("PurchaseReturn_Delete", this.Id).Result;
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex); return false;
            }
        }
        #endregion

        #region Detail

        public void SaveDetail()
        {
            if (PRDetail.ProductId != 0)
            {
                PurchaseReturnDetail pod = PRDetails.Where(x => x.SNo == PRDetail.SNo).FirstOrDefault();

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
                SetAmount();
            }

        }

        public void ClearDetail()
        {
            PurchaseReturnDetail pod = new PurchaseReturnDetail();
            pod.SNo = PRDetails.Count == 0 ? 1 : PRDetails.Max(x => x.SNo) + 1;
            pod.toCopy<PurchaseReturnDetail>(PRDetail);
        }

        public void DeleteDetail(int SNo)
        {
            PurchaseReturnDetail pod = PRDetails.Where(x => x.SNo == SNo).FirstOrDefault();

            if (pod != null)
            {
                PRDetails.Remove(pod);
                ItemAmount = PRDetails.Sum(x => x.Amount);
                SetAmount();
                ClearDetail();
            }
        }

        #endregion

        private void SetAmount()
        {
            GSTAmount = (ItemAmount - DiscountAmount) * Common.AppLib.GSTPer;
            TotalAmount = ItemAmount - DiscountAmount + GSTAmount + ExtraAmount;
            setLabel();
        }
       
        public bool FindRefNo()
        {
            var rv = false;
            try
            {
                rv = FMCGHubClient.HubCaller.Invoke<bool>("Find_PRRef", RefNo, this).Result;
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex); rv = true;
            }
            return rv;
        }

        public static List<PurchaseReturn> ToList(int? LedgerId, int? TType, DateTime dtFrom, DateTime dtTo, string BillNo, decimal amtFrom, decimal amtTo)
        {
            List<PurchaseReturn> rv = new List<PurchaseReturn>();
            try
            {
                rv = FMCGHubClient.HubCaller.Invoke<List<PurchaseReturn>>("PurchaseReturn_List", LedgerId, TType, dtFrom, dtTo, BillNo, amtFrom, amtTo).Result;
            }
            catch (Exception ex)
            {
               Common.AppLib.WriteLog(ex);  Common.AppLib.WriteLog(string.Format("PurchaseReturn List= {0}-{1}", ex.Message, ex.InnerException));
            }
            return rv;

        }


        #endregion
    }
}
