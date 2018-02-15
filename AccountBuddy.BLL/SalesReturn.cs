﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using AccountBuddy.Common;

namespace AccountBuddy.BLL
{
    public class SalesReturn : INotifyPropertyChanged
    {

        #region Field
        
        private long _Id;
        private DateTime _SRDate;
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

        private bool _IsGST = true;
        private string _SearchText;

        private SalesReturnDetail _SRDetail;
        private ObservableCollection<SalesReturnDetail> _SRDetails;
        private ObservableCollection<TaxMaster> _TaxDetails;
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
                    _UserPermission = UserAccount.User.UserType == null ? new UserTypeDetail() : UserAccount.User.UserType.UserTypeDetails.Where(x => x.UserTypeFormDetail.FormName == Forms.frmSalesReturn.ToString()).FirstOrDefault();
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
        public DateTime SRDate
        {
            get
            {
                return _SRDate;
            }
            set
            {
                if (_SRDate != value)
                {
                    _SRDate = value;
                    NotifyPropertyChanged(nameof(SRDate));
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

        public SalesReturnDetail SRDetail
        {
            get
            {
                if (_SRDetail == null) _SRDetail = new SalesReturnDetail();
                return _SRDetail;
            }
            set
            {
                if (_SRDetail != value)
                {
                    _SRDetail = value;
                    NotifyPropertyChanged(nameof(SRDetail));
                }
            }
        }

        public ObservableCollection<SalesReturnDetail> SRDetails
        {
            get
            {
                if (_SRDetails == null) _SRDetails = new ObservableCollection<SalesReturnDetail>();
                return _SRDetails;
            }
            set
            {
                if (_SRDetails != value)
                {
                    _SRDetails = value;
                    NotifyPropertyChanged(nameof(SRDetails));
                }
            }
        }
        public ObservableCollection<TaxMaster> TaxDetails
        {
            get
            {
                if (_TaxDetails == null) _TaxDetails = new ObservableCollection<TaxMaster>();
                return _TaxDetails;
            }
            set
            {
                if (_TaxDetails != value)
                {
                    _TaxDetails = value;
                    NotifyPropertyChanged(nameof(TaxDetails));
                }
            }
        }

        public long PaymentLedgerId { get; set; }
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
                return FMCGHubClient.HubCaller.Invoke<bool>("SalesReturn_Save", this).Result;
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex); return false;
            }
        }

        public void Clear()
        {
            new SalesReturn().ToMap(this);
            this.SRDetail = new SalesReturnDetail();
            this.SRDetails = new ObservableCollection<SalesReturnDetail>();
            this.TaxDetails = new ObservableCollection<TaxMaster>();
            var l1 = BLL.TaxMaster.toList;
            foreach (var t in l1)
            {
                TaxDetails.Add(new TaxMaster()
                {
                    Id = t.Id,
                    Status = t.Status,
                    Ledger = t.Ledger,
                    TaxPercentage = t.TaxPercentage,
                    TaxName = string.Format("{0}({1})", t.Ledger.LedgerName, t.TaxPercentage.ToString()),
                    LedgerId = t.Ledger.Id,
                    TaxAmount = 0
                });
            }

            SRDate = DateTime.Now;
            RefNo = FMCGHubClient.HubCaller.Invoke<string>("SalesReturn_NewRefNo").Result;
            NotifyAllPropertyChanged();
        }

        public bool Find()
        {
            try
            {
                SalesReturn po = FMCGHubClient.HubCaller.Invoke<SalesReturn>("SalesReturn_Find", RefNo).Result;
                if (po.Id == 0) return false;
                po.ToMap(this);
                this.SRDetails = po.SRDetails;
                this.TaxDetails = po.TaxDetails;
                SetGST();
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
                SalesReturn po = FMCGHubClient.HubCaller.Invoke<SalesReturn>("SalesReturn_FindById", Id).Result;
                if (po.Id == 0) return false;
                po.ToMap(this);
                this.SRDetails = po.SRDetails;
                this.TaxDetails = po.TaxDetails;
                SetGST();
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
                return FMCGHubClient.HubCaller.Invoke<bool>("SalesReturn_Delete", this.Id).Result;
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
            if (SRDetail.ProductId != 0)
            {
                SalesReturnDetail pod = SRDetails.Where(x => x.SNo == SRDetail.SNo).FirstOrDefault();

                if (pod == null)
                {
                    pod = new SalesReturnDetail();
                    SRDetails.Add(pod);
                }
                SRDetail.ToMap(pod);
                ClearDetail();
                ItemAmount = SRDetails.Sum(x => x.Amount);
                SetAmount();
                if (_IsGST == true) SetGST();
            }

        }

        public void ClearDetail()
        {
            SalesReturnDetail pod = new SalesReturnDetail();
            pod.SNo = SRDetails.Count == 0 ? 1 : SRDetails.Max(x => x.SNo) + 1;
            pod.ToMap(SRDetail);
        }

        public void DeleteDetail(int SNo)
        {
            SalesReturnDetail pod = SRDetails.Where(x => x.SNo == SNo).FirstOrDefault();

            if (pod != null)
            {
                SRDetails.Remove(pod);
                ItemAmount = SRDetails.Sum(x => x.Amount);
                SetAmount(); if (_IsGST == true) SetGST();
                ClearDetail();
            }
        }

        #endregion

        private void SetAmount()
        {
            TotalAmount = ItemAmount  - DiscountAmount + GSTAmount + ExtraAmount ;
            setLabel();
                }
        public void SetGST()
        {
            GSTAmount = TaxMaster.SetGST(TaxDetails, ItemAmount-DiscountAmount);
            SetAmount();
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
                rv = FMCGHubClient.HubCaller.Invoke<bool>("Find_SRRef", RefNo, this).Result;
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex); rv = true;
            }
            return rv;
        }

        public static List<SalesReturn> ToList(int? LedgerId, int? TType, DateTime dtFrom, DateTime dtTo, string BillNo, decimal amtFrom, decimal amtTo)
        {
            List<SalesReturn> rv = new List<SalesReturn>();
            try
            {
                rv = FMCGHubClient.HubCaller.Invoke<List<SalesReturn>>("SalesReturn_List", LedgerId, TType, dtFrom, dtTo, BillNo, amtFrom, amtTo).Result;
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(string.Format("SalesReturn List= {0}-{1}", ex.Message, ex.InnerException));
            }
            return rv;

        }

        public void setEntryNo()
        {
            RefNo = FMCGHubClient.HubCaller.Invoke<string>("SalesReturn_NewRefNo", SRDate).Result;
        }

        #endregion
    }
}
