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

        private string _SearchText;

        private SalesDetail _SDetail;
        private ObservableCollection<SalesDetail> _SDetails;

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
                return FMCGHubClient.FMCGHub.Invoke<bool>("Sales_Save", this).Result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void Clear()
        {
            new Sale().toCopy<Sale>(this);
            _SDetail = new SalesDetail();
           _SDetails = new ObservableCollection<SalesDetail>();

            SalesDate = DateTime.Now;

            NotifyAllPropertyChanged();
        }

        public bool Find()
        {
            try
            {
                Sale S = FMCGHubClient.FMCGHub.Invoke<Sale>("Sales_Find", SearchText).Result;
                if (S.Id == 0) return false;
                S.toCopy<Sale>(this);
                this.SDetails = S.SDetails;
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
                return FMCGHubClient.FMCGHub.Invoke<bool>("Sales_Delete", this.Id).Result;
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
            if (SDetail.ProductId != 0)
            {
                SalesDetail sod = SDetails.Where(x => x.ProductId == SDetail.ProductId).FirstOrDefault();

                if (sod == null)
                {
                    sod = new SalesDetail();
                    SDetails.Add(sod);
                }
                else
                {
                    SDetail.Quantity += sod.Quantity;
                }
                SDetail.toCopy<SalesDetail>(sod);
                ClearDetail();
                ItemAmount = SDetails.Sum(x => x.Amount);
            }

        }

        public void ClearDetail()
        {
            SalesDetail sod = new SalesDetail();
            sod.toCopy<SalesDetail>(SDetail);
        }

        public void DeleteDetail(string PName)
        {
            SalesDetail sod = SDetails.Where(x => x.ProductName == PName).FirstOrDefault();

            if (sod != null)
            {
                SDetails.Remove(sod);
                ItemAmount = SDetails.Sum(x => x.Amount);
            }
        }

        #endregion

        private void SetAmount()
        {
            GSTAmount = (ItemAmount - DiscountAmount ) * Common.AppLib.GSTPer;
            TotalAmount = ItemAmount - DiscountAmount + GSTAmount + ExtraAmount;
        }
        public bool FindRefNo()
        {
            var rv = false;
            try
            {
                rv = FMCGHubClient.FMCGHub.Invoke<bool>("Find_SRef", RefNo, this).Result;
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
