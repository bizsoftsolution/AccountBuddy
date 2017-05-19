﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountBuddy.Common;

namespace AccountBuddy.BLL
{
    public class Payment : INotifyPropertyChanged
    {
        #region Fields
        private long _Id;
        private string _EntryNo;
        private DateTime? _PaymentDate;
        private int? _LedgerId;
        private string _PaymentMode;
        private decimal? _Amount;
        private string _RefNo;
        private string _Status;
        private decimal? _ExtraCharge;
        private string _ChequeNo;
        private DateTime? _ChequeDate;
        private DateTime? _ClearDate;
        private string _Particulars;
        private string _PayTo;
        private string _VoucherNo;
        private string _LedgerName;

        private Ledger _PLedger;

        private PaymentDetail _PDetail;
        private ObservableCollection<PaymentDetail> _PDetails;

        private string _SearchText;

        private string _PayMode;
        private static List<string> _PayModeList;

        private bool _IsShowChequeDetail;
        private bool _IsShowOnlineDetail;
        private bool _IsShowTTDetail;

        #endregion

        #region  Property

        public static List<string> PayModeList
        {
            get
            {
                if (_PayModeList == null)
                {
                    _PayModeList = new List<string>();
                    _PayModeList.Add("Cash");
                    _PayModeList.Add("Cheque");
                    _PayModeList.Add("Online");
                    _PayModeList.Add("TT");
                }
                return _PayModeList;
            }
            set
            {
                if (_PayModeList != value)
                {
                    _PayModeList = value;
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
        public string EntryNo
        {
            get
            {
                return _EntryNo;
            }
            set
            {
                if (_EntryNo != value)
                {
                    _EntryNo = value;
                    NotifyPropertyChanged(nameof(EntryNo));
                }
            }
        }
        public DateTime? PaymentDate
        {
            get
            {
                return _PaymentDate;
            }
            set
            {
                if (_PaymentDate != value)
                {
                    _PaymentDate = value;
                    NotifyPropertyChanged(nameof(PaymentDate));
                }
            }
        }
        public int? LedgerId
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
        public string PaymentMode
        {
            get
            {
                return _PaymentMode;
            }
            set
            {
                if (_PaymentMode != value)
                {
                    _PaymentMode = value;
                    NotifyPropertyChanged(nameof(PaymentMode));
                }
            }
        }
        public decimal? Amount
        {
            get
            {
                return _Amount;
            }
            set
            {
                if (_Amount != value)
                {
                    _Amount = value;
                    NotifyPropertyChanged(nameof(Amount));
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
        public Nullable<decimal> ExtraCharge
        {
            get
            {
                return _ExtraCharge;
            }
            set
            {
                if (_ExtraCharge != value)
                {
                    _ExtraCharge = value;
                    NotifyPropertyChanged(nameof(ExtraCharge));
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
        public Nullable<System.DateTime> ClearDate
        {
            get
            {
                return _ClearDate;
            }
            set
            {
                if (_ClearDate != value)
                {
                    _ClearDate = value;
                    NotifyPropertyChanged(nameof(ClearDate));
                }
            }
        }
        public string Particulars
        {
            get
            {
                return _Particulars;
            }
            set
            {
                if (_Particulars != value)
                {
                    _Particulars = value;
                    NotifyPropertyChanged(nameof(Particulars));
                }
            }
        }
        public string PayTo
        {
            get
            {
                return _PayTo;
            }
            set
            {
                if (_PayTo != value)
                {
                    _PayTo = value;
                    NotifyPropertyChanged(nameof(PayTo));
                }
            }
        }
        public string VoucherNo
        {
            get
            {
                return _VoucherNo;
            }
            set
            {
                if (_VoucherNo != value)
                {
                    _VoucherNo = value;
                    NotifyPropertyChanged(nameof(VoucherNo));
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

        public string PayMode
        {
            get
            {
                return _PayMode;
            }
            set
            {
                if (_PayMode != value)
                {
                    _PayMode = value;
                    IsShowChequeDetail = value == "Cheque";
                    IsShowOnlineDetail = value == "Online";
                    IsShowTTDetail = value == "TT";
                    NotifyPropertyChanged(nameof(PayMode));
                }
            }
        }

        public PaymentDetail PDetail
        {
            get
            {
                if (_PDetail == null) _PDetail = new PaymentDetail();
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

        public ObservableCollection<PaymentDetail> PDetails
        {
            get
            {
                if (_PDetails == null) _PDetails = new ObservableCollection<PaymentDetail>();
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
        public bool IsShowOnlineDetail
        {
            get
            {
                return _IsShowOnlineDetail;
            }
            set
            {
                if (_IsShowOnlineDetail != value)
                {
                    _IsShowOnlineDetail = value;
                    NotifyPropertyChanged(nameof(IsShowOnlineDetail));
                }
            }
        }
        public bool IsShowTTDetail
        {
            get
            {
                return _IsShowTTDetail;
            }
            set
            {
                if (_IsShowTTDetail != value)
                {
                    _IsShowTTDetail = value;
                    NotifyPropertyChanged(nameof(IsShowTTDetail));
                }
            }
        }
       

        #endregion

        public Ledger PLedger
        {
            get
            {
                if (_PLedger == null) _PLedger = new Ledger();
                return _PLedger;
            }
            set
            {
                if (_PLedger != value)
                {
                    _PLedger = value;
                    NotifyPropertyChanged(nameof(Ledger));
                }
            }
        }


        #region List
        public static ObservableCollection<Ledger> LedgerList
        {
            get
            {
                return Ledger.toList;
            }
        }
        #endregion

        #region Master
        public bool Save()
        {
            try
            {
                return ABClientHub.FMCGHub.Invoke<bool>("Payment_Save", this).Result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void Clear()
        {
            new Payment().toCopy<Payment>(this);
            this.PDetail= new PaymentDetail();
            this.PDetails = new ObservableCollection<PaymentDetail>();

            PaymentDate = DateTime.Now;

            NotifyAllPropertyChanged();
        }

        public bool Find()
        {
            try
            {
                Payment po = ABClientHub.FMCGHub.Invoke<Payment>("Payment_Find", SearchText).Result;
                if (po.Id == 0) return false;
                po.toCopy<Payment>(this);
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
                return ABClientHub.FMCGHub.Invoke<bool>("Payment_Delete", this.Id).Result;
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

            PaymentDetail pod = PDetails.Where(x => x.LedgerId == PDetail.LedgerId).FirstOrDefault();

            if (pod == null)
            {
                pod = new PaymentDetail();
                PDetails.Add(pod);
            }
            else
            {
                PDetail.Amount = pod.Amount;
            }
            PDetail.toCopy<PaymentDetail>(pod);
            ClearDetail();
           

        }

        public void ClearDetail()
        {
            PaymentDetail pod = new PaymentDetail();
            pod.toCopy<PaymentDetail>(PDetail);
        }

        public void DeleteDetail(string PName)
        {
            PaymentDetail pod = PDetails.Where(x => x.LedgerName == PName).FirstOrDefault();

            if (pod != null)
            {
                PDetails.Remove(pod);
                //ItemAmount = PDetails.Sum(x => x.Amount);
            }
        }
        #endregion


    
        public bool FindRefNo()
        {
            var rv = false;
            try
            {
                rv = ABClientHub.FMCGHub.Invoke<bool>("Find_PORef", EntryNo, this).Result;
            }
            catch (Exception ex)
            {
                rv = true;
            }
            return rv;
        }


        #region Property  Changed Event

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }


        private void NotifyAllPropertyChanged()
        {
            foreach (var p in this.GetType().GetProperties()) NotifyPropertyChanged(p.Name);
        }

        #endregion
    }
}
