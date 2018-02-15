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
    public class StockInProcess : INotifyPropertyChanged
    {
        #region Field
  
        private long _Id;
        private DateTime? _SPDate;
        private string _RefNo;
        private string _JONo;
        private int? _StaffId;
        private decimal? _ItemAmount;
        private decimal? _DiscountAmount;
        private decimal? _GSTAmount;
        private decimal? _Extras;
        private decimal? _TotalAmount;
        private string _Narration;
        private int? _CompanyId;

        private string _StaffName;
        private string _AmountInwords;

        private string _SearchText;

        private StockInProcessDetail _STPDetail;

        private string _Status;
        private string _RefCode;
        private ObservableCollection<StockInProcessDetail> _STPDetails;
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
                    _UserPermission = UserAccount.User.UserType == null ? new UserTypeDetail() : UserAccount.User.UserType.UserTypeDetails.Where(x => x.UserTypeFormDetail.FormName == Forms.frmStockInProcess.ToString()).FirstOrDefault();
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

        public DateTime? SPDate
        {
            get
            {
                return _SPDate;
            }
            set
            {
                if (_SPDate != value)
                {
                    _SPDate = value;
                    NotifyPropertyChanged(nameof(SPDate));
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
        public int? StaffId
        {
            get
            {
                return _StaffId;
            }
            set
            {
                if (_StaffId != value)
                {
                    _StaffId = value;
                    NotifyPropertyChanged(nameof(StaffId));
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

        public string StaffName
        {
            get
            {
                return _StaffName;
            }
            set
            {
                if (_StaffName != value)
                {
                    _StaffName = value;
                    NotifyPropertyChanged(nameof(StaffName));
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

        public StockInProcessDetail STPDetail
        {
            get
            {
                if (_STPDetail == null) _STPDetail = new StockInProcessDetail();
                return _STPDetail;
            }
            set
            {
                if (_STPDetail != value)
                {
                    _STPDetail = value;
                    NotifyPropertyChanged(nameof(_STPDetail));
                }
            }
        }

        public ObservableCollection<StockInProcessDetail> STPDetails
        {
            get
            {
                if (_STPDetails == null) _STPDetails = new ObservableCollection<StockInProcessDetail>();
                return _STPDetails;
            }
            set
            {
                if (_STPDetails != value)
                {
                    _STPDetails = value;
                    NotifyPropertyChanged(nameof(_STPDetails));
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
                return FMCGHubClient.HubCaller.Invoke<bool>("StockInProcess_MakeReceieved", this).Result;
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex); return false;
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
                return FMCGHubClient.HubCaller.Invoke<bool>("StockInProcess_Save", this).Result;
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex); return false;
            }
        }

        public void Clear()
        {
            new StockInProcess().ToMap(this);
            STPDetail = new StockInProcessDetail();
            STPDetails = new ObservableCollection<StockInProcessDetail>();

            SPDate = DateTime.Now;
            RefNo = FMCGHubClient.HubCaller.Invoke<string>("StockInProcess_NewRefNo").Result;
            NotifyAllPropertyChanged();
        }

        public bool Find()
        {
            try
            {
                StockInProcess po = FMCGHubClient.HubCaller.Invoke<StockInProcess>("StockInProcess_Find", SearchText).Result;
                if (po.Id == 0) return false;
                po.ToMap(this);
                this.STPDetails = po.STPDetails;
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
                return FMCGHubClient.HubCaller.Invoke<bool>("StockInProcess_Delete", this.Id).Result;
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
                StockInProcess po = FMCGHubClient.HubCaller.Invoke<StockInProcess>("StockInProcess_FindById", Id).Result;
                if (po.Id == 0) return false;
                po.ToMap(this);
                this.STPDetails = po.STPDetails;
                NotifyAllPropertyChanged();
                return true;
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
            if (STPDetail.ProductId != 0)
            {
                StockInProcessDetail pod = STPDetails.Where(x => x.SNo == STPDetail.SNo).FirstOrDefault();

                if (pod == null)
                {
                    pod = new StockInProcessDetail();
                    STPDetails.Add(pod);
                }
                STPDetail.ToMap(pod);
                ClearDetail();
                ItemAmount = STPDetails.Sum(x => x.Amount);
                SetAmount();
            }

        }

        public void ClearDetail()
        {
            StockInProcessDetail pod = new StockInProcessDetail();
            pod.SNo = STPDetails.Count == 0 ? 1 : STPDetails.Max(x => x.SNo) + 1;
            pod.ToMap(STPDetail);
        }

        public void DeleteDetail(int SNo)
        {
            StockInProcessDetail pod = STPDetails.Where(x => x.SNo == SNo).FirstOrDefault();

            if (pod != null)
            {
                STPDetails.Remove(pod);
                ItemAmount = STPDetails.Sum(x => x.Amount);
                SetAmount();
                ClearDetail();
            }
        }
        #endregion


        private void SetAmount()
        {
            GSTAmount = ((ItemAmount ?? 0) - (DiscountAmount ?? 0)) * Common.AppLib.GSTPer;
            TotalAmount = (ItemAmount ?? 0) - (DiscountAmount ?? 0) + GSTAmount + (Extras ?? 0);
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
                rv = FMCGHubClient.HubCaller.Invoke<bool>("Find_STPRef", RefNo, this).Result;

            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex); rv = true;
            }
            return rv;
        }
        public static List<StockInProcess> ToList(int? LedgerId,DateTime dtFrom, DateTime dtTo, string BillNo, decimal amtFrom, decimal amtTo)
        {
            List<StockInProcess> rv = new List<StockInProcess>();
            try
            {
                rv = FMCGHubClient.HubCaller.Invoke<List<StockInProcess>>("StockInProcess_List", LedgerId, dtFrom, dtTo, BillNo, amtFrom, amtTo).Result;
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(string.Format("StockInProcess List= {0}-{1}", ex.Message, ex.InnerException));
            }
            return rv;

        }

        public void setEntryNo()
        {
            RefNo = FMCGHubClient.HubCaller.Invoke<string>("StockInProcess_NewRefNo", SPDate).Result;
        }

        #endregion
    }
}
