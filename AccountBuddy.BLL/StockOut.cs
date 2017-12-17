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
   public class StockOut : INotifyPropertyChanged
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

        private StockOutDetail _STOutDetail;
        private ObservableCollection<StockOutDetail> _STOutDetails;
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
                    _UserPermission = UserAccount.User.UserType == null ? new UserTypeDetail() : UserAccount.User.UserType.UserTypeDetails.Where(x => x.UserTypeFormDetail.FormName == Forms.frmStockOut.ToString()).FirstOrDefault();
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

        public StockOutDetail STOutDetail
        {
            get
            {
                if (_STOutDetail == null) _STOutDetail = new StockOutDetail();
                return _STOutDetail;
            }
            set
            {
                if (_STOutDetail != value)
                {
                    _STOutDetail = value;
                    NotifyPropertyChanged(nameof(_STOutDetail));
                }
            }
        }
        public long PaymentLedgerId { get; set; }

        public ObservableCollection<StockOutDetail> STOutDetails
        {
            get
            {
                if (_STOutDetails == null) _STOutDetails = new ObservableCollection<StockOutDetail>();
                return _STOutDetails;
            }
            set
            {
                if (_STOutDetails != value)
                {
                    _STOutDetails = value;
                    NotifyPropertyChanged(nameof(STOutDetails));
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
                return FMCGHubClient.FMCGHub.Invoke<bool>("StockOut_Save", this).Result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void Clear()
        {
            new StockOut().toCopy<StockOut>(this);
            this.STOutDetail = new StockOutDetail();
            this.STOutDetails = new ObservableCollection<StockOutDetail>();

            Date = DateTime.Now;
            RefNo = FMCGHubClient.FMCGHub.Invoke<string>("StockOut_NewRefNo").Result;
            NotifyAllPropertyChanged();
        }

        public bool Find()
        {
            try
            {
                StockOut po = FMCGHubClient.FMCGHub.Invoke<StockOut>("StockOut_Find", SearchText).Result;
                if (po.Id == 0) return false;
                po.toCopy<StockOut>(this);
                this.STOutDetails = po.STOutDetails;
                NotifyAllPropertyChanged();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool FindById(int id)
        {
            try
            {
                StockOut po = FMCGHubClient.FMCGHub.Invoke<StockOut>("StockOut_FindById", id).Result;
                if (po.Id == 0) return false;
                po.toCopy<StockOut>(this);
                this.STOutDetails = po.STOutDetails;
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
                return FMCGHubClient.FMCGHub.Invoke<bool>("StockOut_Delete", this.Id).Result;
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
            if (STOutDetail.ProductId != 0)
            {
                StockOutDetail pod = STOutDetails.Where(x => x.SNo == STOutDetail.SNo).FirstOrDefault();

                if (pod == null)
                {
                    pod = new StockOutDetail();
                    STOutDetails.Add(pod);
                }
                else
                {
                    STOutDetail.Quantity += pod.Quantity;
                }
                STOutDetail.toCopy<StockOutDetail>(pod);
                ClearDetail();
                ItemAmount = STOutDetails.Sum(x => x.Amount);
               
            }

        }

        public void ClearDetail()
        {
            StockOutDetail pod = new StockOutDetail();
            pod.SNo = STOutDetails.Count == 0 ? 1 : STOutDetails.Max(x => x.SNo) + 1;
            pod.toCopy<StockOutDetail>(STOutDetail);
        }

        public void DeleteDetail(int SNo)
        {
            StockOutDetail pod = STOutDetails.Where(x => x.SNo == SNo).FirstOrDefault();

            if (pod != null)
            {
                STOutDetails.Remove(pod);
                ItemAmount = STOutDetails.Sum(x => x.Amount);
                ClearDetail();
            }
        }

        #endregion

        public static List<StockOut> ToList(int? LedgerId, DateTime dtFrom, DateTime dtTo, string BillNo, decimal amtFrom, decimal amtTo)
        {
            List<StockOut> rv = new List<StockOut>();
            try
            {
                rv = FMCGHubClient.FMCGHub.Invoke<List<StockOut>>("StockOut_List", LedgerId,  dtFrom, dtTo, BillNo, amtFrom, amtTo).Result;
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(string.Format("StockOut List= {0}-{1}", ex.Message, ex.InnerException));
            }
            return rv;

        }



        public bool FindRefNo()
        {
            var rv = false;
            try
            {
                rv = FMCGHubClient.FMCGHub.Invoke<bool>("Find_STOutRef", RefNo, this).Result;
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
