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

        #endregion

        #region Property

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
                StockOutDetail pod = STOutDetails.Where(x => x.ProductId == STOutDetail.ProductId).FirstOrDefault();

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
            pod.toCopy<StockOutDetail>(STOutDetail);
        }

        public void DeleteDetail(string PName)
        {
            StockOutDetail pod = STOutDetails.Where(x => x.ProductName == PName).FirstOrDefault();

            if (pod != null)
            {
                STOutDetails.Remove(pod);
                ItemAmount = STOutDetails.Sum(x => x.Amount);
            }
        }

        #endregion



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
