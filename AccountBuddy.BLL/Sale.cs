﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountBuddy.Common;
using System.Collections.ObjectModel;

namespace AccountBuddy.BLL
{
    public class Sale : INotifyPropertyChanged
    {
        #region Field

        private long _Id;
        private DateTime _SalesDate;
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

        private bool _IsGST=true;
        private string _SearchText;

        private SalesDetail _SDetail;
        private ObservableCollection<SalesDetail> _SDetails;
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
                    _UserPermission = UserAccount.User.UserType == null ? new UserTypeDetail() : UserAccount.User.UserType.UserTypeDetails.Where(x => x.UserTypeFormDetail.FormName == Forms.frmSales.ToString()).FirstOrDefault();
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

        public DateTime SalesDate
        {
            get
            {
                return _SalesDate;
            }
            set
            {
                if (_SalesDate != value)
                {
                    _SalesDate = value;
                    NotifyPropertyChanged(nameof(SalesDate));
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
                    if (_IsGST == true) SetGST();
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
                    if (_IsGST == true) SetGST();
                }
            }
        }
        public decimal GSTAmount
        {
            get
            {
                if (_GSTAmount == 0) _GSTAmount = 0;
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
                    if (_IsGST == true) SetGST();
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

        public SalesDetail SDetail
        {
            get
            {
                if (_SDetail == null) _SDetail = new SalesDetail();
                return _SDetail;
            }
            set
            {
                if (_SDetail != value)
                {
                    _SDetail = value;
                    NotifyPropertyChanged(nameof(SDetail));
                }
            }
        }

        public ObservableCollection<SalesDetail> SDetails
        {
            get
            {
                if (_SDetails == null) _SDetails = new ObservableCollection<SalesDetail>();
                return _SDetails;
            }
            set
            {
                if (_SDetails != value)
                {
                    _SDetails = value;
                    NotifyPropertyChanged(nameof(SDetails));
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
        public bool IsGST
        {
            get
            {
                return _IsGST;
            }
            set
            {
                if (_IsGST != value)
                {
                    _IsGST = value;
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
                return FMCGHubClient.HubCaller.Invoke<bool>("Sales_Save", this).Result;
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
                new Sale().ToMap<Sale>(this);
                _SDetail = new SalesDetail();
                _SDetails = new ObservableCollection<SalesDetail>();

                SalesDate = DateTime.Now;
                RefNo = FMCGHubClient.HubCaller.Invoke<string>("Sales_NewRefNo").Result;
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
                Sale S = FMCGHubClient.HubCaller.Invoke<Sale>("Sales_Find", SearchText).Result;
                if (S.Id == 0) return false;
                S.ToMap<Sale>(this);
                this.SDetails = S.SDetails;
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
                Sale S = FMCGHubClient.HubCaller.Invoke<Sale>("Sales_FindById", Id).Result;
                if (S.Id == 0) return false;
                S.ToMap<Sale>(this);
                this.SDetails = S.SDetails;
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
                return FMCGHubClient.HubCaller.Invoke<bool>("Sales_Delete", this.Id).Result;
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
            if (SDetail.ProductId != 0)
            {
                SalesDetail sod = SDetails.Where(x => x.SNo == SDetail.SNo).FirstOrDefault();

                if (sod == null)
                {
                    sod = new SalesDetail();
                    SDetails.Add(sod);
                }
                else
                {
                    SDetail.Quantity += sod.Quantity;
                }
                SDetail.ToMap<SalesDetail>(sod);
                ClearDetail();
                ItemAmount = SDetails.Sum(x => x.Amount);
                SetAmount();
                if (_IsGST == true) SetGST();
            }

        }

        public void ClearDetail()
        {
            SalesDetail sod = new SalesDetail();
            sod.SNo = SDetails.Count == 0 ? 1 : SDetails.Max(x => x.SNo) + 1;
            sod.ToMap<SalesDetail>(SDetail);
        }

        public void DeleteDetail(int SNo)
        {
            SalesDetail sod = SDetails.Where(x => x.SNo == SNo).FirstOrDefault();

            if (sod != null)
            {
                SDetails.Remove(sod);
                ClearDetail();
                ItemAmount = SDetails.Sum(x => x.Amount);
                SetAmount();
                if (_IsGST == true) SetGST();
            }
        }

        #endregion

        private void SetAmount()
        {

            TotalAmount = ItemAmount - DiscountAmount + GSTAmount + ExtraAmount;
            setLabel();
        }
       private void SetGST()
        {
            GSTAmount = (ItemAmount - DiscountAmount) * Common.AppLib.GSTPer;
        }
        public bool FindRefNo()
        {
            var rv = false;
            try
            {


                Common.AppLib.WriteLog("FindRefNo_Start");
                rv = FMCGHubClient.HubCaller.Invoke<bool>("Find_SRef", RefNo, this).Result;
                Common.AppLib.WriteLog("FindRefNo_End");
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(string.Format("FindRefNo_Error {0}-{1}", ex.Message, ex.InnerException));
                rv = true;
            }
            return rv;
        }
        public static List<Sale> ToList(int? LedgerId, int? TType, DateTime dtFrom, DateTime dtTo, string BillNo, decimal amtFrom, decimal amtTo)
        {
            List<Sale> rv = new List<Sale>();
            try
            {
                rv = FMCGHubClient.HubCaller.Invoke<List<Sale>>("Sale_List", LedgerId, TType, dtFrom, dtTo, BillNo, amtFrom, amtTo).Result;
            }
            catch (Exception ex)
            {
               Common.AppLib.WriteLog(string.Format("Sales List= {0}-{1}", ex.Message, ex.InnerException));               
            }
            return rv;

        }

        

        #endregion
    }
}
