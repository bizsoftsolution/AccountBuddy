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
   public class StockSeperated : INotifyPropertyChanged
    {
        #region Field
        private static ObservableCollection<StockSeperated> _JRPendingList;

        private long _Id;
        private DateTime? _Date;
        private string _RefNo;
        private string _SSNo;
        private int? _StaffId;
        private decimal? _ItemAmount;
        private decimal? _DiscountAmount;
        private decimal? _GSTAmount;
        private decimal? _ExtraAmount;
        private decimal? _TotalAmount;
        private string _Narration;
        private int? _CompanyId;

        private string _StaffName;
        private string _AmountInwords;

        private string _SearchText;

        private StockSeperatedDetail _SSDetail;
        private ObservableCollection<StockSeperatedDetail> _SSDetails;
        private string _Status;
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
                    _UserPermission = UserAccount.User.UserType == null ? new UserTypeDetail() : UserAccount.User.UserType.UserTypeDetails.Where(x => x.UserTypeFormDetail.FormName == Forms.frmStockSeparated.ToString()).FirstOrDefault();
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

        public DateTime? Date
        {
            get
            {
                return _Date;
            }
            set
            {
                if (_Date != value)
                {
                    _Date = value;
                    NotifyPropertyChanged(nameof(Date));
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
        public string SSNo
        {
            get
            {
                return _SSNo;
            }
            set
            {
                if (_SSNo != value)
                {
                    _SSNo = value;
                    NotifyPropertyChanged(nameof(SSNo));
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
        public decimal? ExtraAmount
        {
            get
            {
                if (_ExtraAmount == null) _ExtraAmount = 0;
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

        public StockSeperatedDetail SSDetail
        {
            get
            {
                if (_SSDetail == null) _SSDetail = new StockSeperatedDetail();
                return _SSDetail;
            }
            set
            {
                if (_SSDetail != value)
                {
                    _SSDetail = value;
                    NotifyPropertyChanged(nameof(SSDetail));
                }
            }
        }

        public ObservableCollection<StockSeperatedDetail> SSDetails
        {
            get
            {
                try
                {
                    if (_SSDetails == null) _SSDetails = new ObservableCollection<StockSeperatedDetail>();
                  
                }
                catch(Exception ex)
                {
                    Common.AppLib.WriteLog(string.Format("Stock Seperated _{0}_{1}", ex.InnerException, ex.Message));
                }
                return _SSDetails;
            }
            set
            {
                if (_SSDetails != value)
                {
                    _SSDetails = value;
                    NotifyPropertyChanged(nameof(SSDetails));
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
                return FMCGHubClient.FMCGHub.Invoke<bool>("StockSeperated_Save", this).Result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void Clear()
        {
            new StockSeperated().toCopy<StockSeperated>(this);
            _SSDetail = new StockSeperatedDetail();
            _SSDetails = new ObservableCollection<StockSeperatedDetail>();

            Date = DateTime.Now;
            RefNo = FMCGHubClient.FMCGHub.Invoke<string>("StockSeperated_NewRefNo").Result;
            NotifyAllPropertyChanged();
        }

        public bool Find()
        {
            try
            {
                StockSeperated po = FMCGHubClient.FMCGHub.Invoke<StockSeperated>("StockSeperated_Find", SearchText).Result;
                if (po.Id == 0) return false;
                po.toCopy<StockSeperated>(this);
                this.SSDetails = po.SSDetails;
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
                return FMCGHubClient.FMCGHub.Invoke<bool>("StockSeperated_Delete", this.Id).Result;
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
                StockSeperated po = FMCGHubClient.FMCGHub.Invoke<StockSeperated>("StockSeperated_FindById", Id).Result;
                if (po.Id == 0) return false;
                po.toCopy<StockSeperated>(this);
                this.SSDetails = po.SSDetails;
                NotifyAllPropertyChanged();
                return true;
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
            if (SSDetail.ProductId != 0)
            {
                StockSeperatedDetail pod = SSDetails.Where(x => x.ProductId == SSDetail.ProductId).FirstOrDefault();

                if (pod == null)
                {
                    pod = new StockSeperatedDetail();
                    SSDetails.Add(pod);
                }
                else
                {
                    SSDetail.Quantity += pod.Quantity;
                }
                SSDetail.toCopy<StockSeperatedDetail>(pod);
                ClearDetail();
                ItemAmount = SSDetails.Sum(x => x.Amount);
                SetAmount();
            }

        }

        public void ClearDetail()
        {
            StockSeperatedDetail pod = new StockSeperatedDetail();
            pod.toCopy<StockSeperatedDetail>(SSDetail);
        }

        public void DeleteDetail(string PName)
        {
            StockSeperatedDetail pod = SSDetails.Where(x => x.ProductName == PName).FirstOrDefault();

            if (pod != null)
            {
                SSDetails.Remove(pod);
                ItemAmount = SSDetails.Sum(x => x.Amount);
                SetAmount();
                ClearDetail();
            }
        }
        #endregion


        private void SetAmount()
        {
            GSTAmount = ((ItemAmount ?? 0) - (DiscountAmount ?? 0)) * Common.AppLib.GSTPer;
            TotalAmount = (ItemAmount ?? 0) - (DiscountAmount ?? 0) + GSTAmount + (ExtraAmount ?? 0);
        }

        public bool FindRefNo()
        {
            var rv = false;
            try
            {
                rv = FMCGHubClient.FMCGHub.Invoke<bool>("Find_SSRef", RefNo, this).Result;
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
