using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using AccountBuddy.Common;
using System.Collections.ObjectModel;

namespace AccountBuddy.BLL
{
    public class SalesOrder : INotifyPropertyChanged
    {
        #region Field
        private static ObservableCollection<SalesOrder> _SOPendingList;

        private long _Id;
        private DateTime? _SODate;
        private string _RefNo;
        private string _SONo;
        private int? _LedgerId;
        private decimal? _ItemAmount;
        private decimal? _DiscountAmount;
        private decimal? _GSTAmount;
        private decimal? _ExtraAmount;
        private decimal? _TotalAmount;
        private string _Narration;
        private int? _CompanyId;

        private string _LedgerName;
        private string _AmountInwords;

        private string _SearchText;

        private SalesOrderDetail _SODetail;
        private ObservableCollection<SalesOrderDetail> _SODetails;
        private string _Status;
        private string _RefCode;
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
                    _UserPermission = UserAccount.User.UserType == null ? new UserTypeDetail() : UserAccount.User.UserType.UserTypeDetails.Where(x => x.UserTypeFormDetail.FormName == Forms.frmSalesOrder.ToString()).FirstOrDefault();
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

        public static ObservableCollection<SalesOrder> SOPendingList
        {
            get
            {
                if (_SOPendingList == null)
                {try
                    {
                        _SOPendingList = new ObservableCollection<SalesOrder>();
                        var l1 = FMCGHubClient.HubCaller.Invoke<List<SalesOrder>>("SalesOrder_SOPendingList").Result;
                        _SOPendingList = new ObservableCollection<SalesOrder>(l1);
                    }
                    catch(Exception ex)
                    {
                        Common.AppLib.WriteLog(string.Format("SOPending List_{0}_{1}", ex.Message, ex.InnerException));
                    }
                }
                return _SOPendingList;
            }
            set
            {
                _SOPendingList = value;
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

        public DateTime? SODate
        {
            get
            {
                return _SODate;
            }
            set
            {
                if (_SODate != value)
                {
                    _SODate = value;
                    NotifyPropertyChanged(nameof(SODate));
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
        public string SONo
        {
            get
            {
                return _SONo;
            }
            set
            {
                if (_SONo != value)
                {
                    _SONo = value;
                    NotifyPropertyChanged(nameof(SONo));
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

        public SalesOrderDetail SODetail
        {
            get
            {
                if (_SODetail == null) _SODetail = new SalesOrderDetail();
                return _SODetail;
            }
            set
            {
                if (_SODetail != value)
                {
                    _SODetail = value;
                    NotifyPropertyChanged(nameof(SODetail));
                }
            }
        }

        public ObservableCollection<SalesOrderDetail> SODetails
        {
            get
            {
                if (_SODetails == null) _SODetails = new ObservableCollection<SalesOrderDetail>();
                return _SODetails;
            }
            set
            {
                if (_SODetails != value)
                {
                    _SODetails = value;
                    NotifyPropertyChanged(nameof(SODetails));
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
                return FMCGHubClient.HubCaller.Invoke<bool>("SalesOrder_Save", this).Result;
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex); return false;
            }
        }

        public bool MakeSales()
        {
            try
            {
                return FMCGHubClient.HubCaller.Invoke<bool>("SalesOrder_MakeSales", this).Result;
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
                new SalesOrder().ToMap(this);
                _SODetail = new SalesOrderDetail();
                _SODetails = new ObservableCollection<SalesOrderDetail>();

                SODate = DateTime.Now;
                RefNo = FMCGHubClient.HubCaller.Invoke<string>("SalesOrder_NewRefNo").Result;
                NotifyAllPropertyChanged();
            }
            catch(Exception ex)
            {
                Common.AppLib.WriteLog(ex);
            }
        }

        public bool Find()
        {
            try
            {
                SalesOrder po = FMCGHubClient.HubCaller.Invoke<SalesOrder>("SalesOrder_Find", SearchText).Result;
                if (po.Id == 0) return false;
                po.ToMap(this);
                this.SODetails = po.SODetails;
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
                return FMCGHubClient.HubCaller.Invoke<bool>("SalesOrder_Delete", this.Id).Result;
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex);
                return false;
            }
        }
        #endregion

        #region Detail

        public void SaveDetail()
        {
            if (SODetail.ProductId != 0)
            {
                SalesOrderDetail pod = SODetails.Where(x => x.SNo == SODetail.SNo).FirstOrDefault();

                if (pod == null)
                {
                    pod = new SalesOrderDetail();
                    SODetails.Add(pod);
                }
                SODetail.ToMap(pod);               
                ItemAmount = SODetails.Sum(x => x.Amount);
                SetAmount();
                ClearDetail();
            }

        }

        public void ClearDetail()
        {
            SalesOrderDetail pod = new SalesOrderDetail();
            pod.SNo = SODetails.Count == 0 ? 1 : SODetails.Max(x => x.SNo) + 1;
            pod.ToMap(SODetail);
        }

        public void DeleteDetail(int sno)
        {
            SalesOrderDetail pod = SODetails.Where(x => x.SNo == sno).FirstOrDefault();

            if (pod != null)
            {
                SODetails.Remove(pod);
                ItemAmount = SODetails.Sum(x => x.Amount);
                SetAmount();
                ClearDetail();
            }
        }
        #endregion


        private void SetAmount()
        {
            GSTAmount = ((ItemAmount ?? 0) - (DiscountAmount ?? 0)) * Common.AppLib.GSTPer;
            TotalAmount = (ItemAmount ?? 0) - (DiscountAmount ?? 0) + GSTAmount + (ExtraAmount ?? 0);
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
                rv = FMCGHubClient.HubCaller.Invoke<bool>("Find_SORef", RefNo, this).Result;
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex); rv = true;
            }
            return rv;
        }

        public static List<SalesOrder> ToList(int? LedgerId,DateTime dtFrom, DateTime dtTo, string BillNo, decimal amtFrom, decimal amtTo)
        {
            List<SalesOrder> rv = new List<SalesOrder>();
            try
            {
                rv = FMCGHubClient.HubCaller.Invoke<List<SalesOrder>>("SalesOrder_List", LedgerId, dtFrom, dtTo, BillNo, amtFrom, amtTo).Result;
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(string.Format("Sales Order List= {0}-{1}", ex.Message, ex.InnerException));
            }
            return rv;

        }


        #endregion

    }
}
