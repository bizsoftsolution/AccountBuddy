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
   public class StockIn : INotifyPropertyChanged
    {
        #region Field

        private long _Id;
        private DateTime _Date;
        private string _RefNo;
        private int _LedgerId;
        private string _Type;
        private decimal _ItemAmount;
        private string _Narration;




        private string _LedgerName;

        private string _AmountInwords;

        private string _SearchText;

        private StockInDetail _STInDetail;
        private ObservableCollection<StockInDetail> _STInDetails;
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
                    _UserPermission = UserAccount.User.UserType == null ? new UserTypeDetail() : UserAccount.User.UserType.UserTypeDetails.Where(x => x.UserTypeFormDetail.FormName == Forms.frmStockInOut.ToString()).FirstOrDefault();
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

        public DateTime Date
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
        public string Type
        {
            get
            {
                return _Type;
            }
            set
            {
                if (_Type != value)
                {
                    _Type = value;
                    NotifyPropertyChanged(nameof(Type));
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

        public StockInDetail STInDetail
        {
            get
            {
                if (_STInDetail == null) _STInDetail = new StockInDetail();
                return _STInDetail;
            }
            set
            {
                if (_STInDetail != value)
                {
                    _STInDetail = value;
                    NotifyPropertyChanged(nameof(_STInDetail));
                }
            }
        }
        public long PaymentLedgerId { get; set; }

        public ObservableCollection<StockInDetail> STInDetails
        {
            get
            {
                if (_STInDetails == null) _STInDetails = new ObservableCollection<StockInDetail>();
                return _STInDetails;
            }
            set
            {
                if (_STInDetails != value)
                {
                    _STInDetails = value;
                    NotifyPropertyChanged(nameof(STInDetails));
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
                return FMCGHubClient.HubCaller.Invoke<bool>("StockIn_Save", this).Result;
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex); return false;
            }
        }

        public void Clear()
        {
            new StockIn().toCopy<StockIn>(this);
            this.STInDetail = new StockInDetail();
            this.STInDetails = new ObservableCollection<StockInDetail>();

            Date = DateTime.Now;
            RefNo = FMCGHubClient.HubCaller.Invoke<string>("StockIn_NewRefNo").Result;
            NotifyAllPropertyChanged();
        }

        public bool Find()
        {
            try
            {
                StockIn po = FMCGHubClient.HubCaller.Invoke<StockIn>("StockIn_Find", SearchText).Result;
                if (po.Id == 0) return false;
                po.toCopy<StockIn>(this);
                this.STInDetails = po.STInDetails;
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
                StockIn po = FMCGHubClient.HubCaller.Invoke<StockIn>("StockIn_FindById", Id).Result;
                if (po.Id == 0) return false;
                po.toCopy<StockIn>(this);
                this.STInDetails = po.STInDetails;
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
                return FMCGHubClient.HubCaller.Invoke<bool>("StockIn_Delete", this.Id).Result;
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
            if (STInDetail.ProductId != 0)
            {
                StockInDetail pod = STInDetails.Where(x => x.SNo == STInDetail.SNo).FirstOrDefault();

                if (pod == null)
                {
                    pod = new StockInDetail();
                    STInDetails.Add(pod);
                }
                else
                {
                    STInDetail.Quantity += pod.Quantity;
                }
                STInDetail.toCopy<StockInDetail>(pod);
                ClearDetail();
                ItemAmount = STInDetails.Sum(x => x.Amount);
            }

        }

        public void ClearDetail()
        {
            StockInDetail pod = new StockInDetail();
            pod.SNo = STInDetails.Count == 0 ? 1 : STInDetails.Max(x => x.SNo) + 1;
            pod.toCopy<StockInDetail>(STInDetail);
        }

        public void DeleteDetail(int SNo)
        {
            StockInDetail pod = STInDetails.Where(x => x.SNo == SNo).FirstOrDefault();

            if (pod != null)
            {
                STInDetails.Remove(pod);
                ItemAmount = STInDetails.Sum(x => x.Amount);
                ClearDetail();
            }
        }

        #endregion



        public bool FindRefNo()
        {
            var rv = false;
            try
            {
                rv = FMCGHubClient.HubCaller.Invoke<bool>("Find_STInRef", RefNo, this).Result;
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex);
                rv = true;
            }
            return rv;
        }

        public static List<StockIn> ToList(int? LedgerId,  DateTime dtFrom, DateTime dtTo, string BillNo, decimal amtFrom, decimal amtTo)
        {
            List<StockIn> rv = new List<StockIn>();
            try
            {
                rv = FMCGHubClient.HubCaller.Invoke<List<StockIn>>("StockIn_List", LedgerId, dtFrom, dtTo, BillNo, amtFrom, amtTo).Result;
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(string.Format("StockIn List= {0}-{1}", ex.Message, ex.InnerException));
            }
            return rv;

        }


        #endregion
    }
}
