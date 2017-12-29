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
                    _UserPermission = UserAccount.User.UserType == null ? new UserTypeDetail() : UserAccount.User.UserType.UserTypeDetails.Where(x => x.UserTypeFormDetail.FormName == Forms.frmPurchase.ToString()).FirstOrDefault();
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
                     SetAmount();
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
                    IsShowChequeDetail = value == "Cheque";
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
                if(_ChequeNo!=value)
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
                return FMCGHubClient.HubCaller.Invoke<bool>("Purchase_Save", this).Result;
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex);
                return false;
            }
        }
        public bool Save_To_Purchase()
        {
            try
            {
                return FMCGHubClient.HubCaller.Invoke<bool>("PurchaseOrder_Save", this).Result;
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex); return false;
            }
        }
        public void Clear()
        {
            try
            {
                new Purchase().ToMap(this);
                this.PDetail = new PurchaseDetail();
                this.PDetails = new ObservableCollection<PurchaseDetail>();

                PurchaseDate = DateTime.Now;
                RefNo = FMCGHubClient.HubCaller.Invoke<string>("Purchase_NewRefNo").Result;
                NotifyAllPropertyChanged();
            }
            catch(Exception ex)
            {
                Common.AppLib.WriteLog(ex);
            }
                }
        public void setLabel()
        {
            lblDiscount = string.Format("{0}({1})", "Discount Amount", AppLib.CurrencyPositiveSymbolPrefix);
            lblExtra = string.Format("{0}({1})", "Extra Amount", AppLib.CurrencyPositiveSymbolPrefix);

        }
        public bool Find()
        {
            try
            {
                Purchase po = FMCGHubClient.HubCaller.Invoke<Purchase>("Purchase_Find", SearchText).Result;
                if (po.Id == 0) return false;
                po.ToMap(this);
                this.PDetails = po.PDetails;
                NotifyAllPropertyChanged();
                return true;
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
                Purchase po = FMCGHubClient.HubCaller.Invoke<Purchase>("Purchase_FindById", Id).Result;
                if (po.Id == 0) return false;
                po.ToMap(this);
                this.PDetails = po.PDetails;
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
                return FMCGHubClient.HubCaller.Invoke<bool>("Purchase_Delete", this.Id).Result;
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
            if (PDetail.ProductId != 0)
            {
                PurchaseDetail pod = PDetails.Where(x => x.SNo == PDetail.SNo).FirstOrDefault();

                if (pod == null)
                {
                    pod = new PurchaseDetail();
                    PDetails.Add(pod);
                }
                else
                {
                    PDetail.Quantity += pod.Quantity;
                }
                PDetail.ToMap(pod);
                ClearDetail();
                ItemAmount = PDetails.Sum(x => x.Amount);
                SetAmount();
            }

        }

        public void ClearDetail()
        {
            PurchaseDetail pod = new PurchaseDetail();
            pod.SNo = PDetails.Count == 0 ? 1 : PDetails.Max(x => x.SNo) + 1;
            pod.ToMap(PDetail);
        }

        public void DeleteDetail(int SNo)
        {
            PurchaseDetail pod = PDetails.Where(x => x.SNo == SNo).FirstOrDefault();

            if (pod != null)
            {
                PDetails.Remove(pod);
                ClearDetail();
                ItemAmount = PDetails.Sum(x => x.Amount);
                SetAmount();
            }
        }

        #endregion

        private void SetAmount()
        {
            GSTAmount = (ItemAmount - DiscountAmount) * Common.AppLib.GSTPer;
            TotalAmount = ItemAmount - DiscountAmount  + GSTAmount + ExtraAmount;
            setLabel();
        }

        public bool FindRefNo()
        {
            var rv = false;
            try
            {
                rv = FMCGHubClient.HubCaller.Invoke<bool>("Find_PRef", RefNo, this).Result;
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex); rv = true;
            }
            return rv;
        }


        public static List<Purchase> ToList(int? LedgerId, int? TType, DateTime dtFrom, DateTime dtTo, string BillNo, decimal amtFrom, decimal amtTo)
        {
            List<Purchase> rv = new List<Purchase>();
            try
            {
                rv = FMCGHubClient.HubCaller.Invoke<List<Purchase>>("Purchase_List", LedgerId, TType, dtFrom, dtTo, BillNo, amtFrom, amtTo).Result;
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(string.Format("Purchase List= {0}-{1}", ex.Message, ex.InnerException));
            }
            return rv;

        }



        #endregion

    }
}
