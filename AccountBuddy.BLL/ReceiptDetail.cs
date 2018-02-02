using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBuddy.BLL
{
    public class ReceiptDetail : INotifyPropertyChanged
    {
        #region Fields
        private long _Id;
        private long _ReceipttId;
        private int _LedgerId;
        private decimal _Amount;
        private string _Particulars;

        private string _LedgerName;
        private int _SNo;
        private int _GSTStatusId;
        private ObservableCollection<TaxMaster> _TaxDetails;
        private decimal _GSTAmount;
        private int _RefLedgerId;
        private bool _IncludingGST;
        private bool _AllowEdit;


        #endregion

        #region Property

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


        public int SNo
        {
            get
            {
                return _SNo;
            }
            set
            {
                if (_SNo != value)
                {
                    _SNo = value;
                    NotifyPropertyChanged(nameof(SNo));
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
        public long ReceiptId
        {
            get
            {
                return _ReceipttId;
            }
            set
            {
                if (_ReceipttId != value)
                {
                    _ReceipttId = value;
                    NotifyPropertyChanged(nameof(ReceiptId));
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
        public decimal Amount
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
                    SetGST();

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
        public int GSTStatusId
        {
            get
            {
                return _GSTStatusId;
            }
            set
            {
                if (_GSTStatusId != value)
                {
                    _GSTStatusId = value;
                    NotifyPropertyChanged(nameof(GSTStatusId));
                }
            }
        }
        public bool IncludingGST
        {
            get
            {
                return _IncludingGST;
            }
            set
            {
                if (_IncludingGST != value)
                {
                    _IncludingGST = value;
                    NotifyPropertyChanged(nameof(IncludingGST));
                    SetGST();
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
        public int RefLedgerId
        {
            get
            {
                return _RefLedgerId;
            }
            set
            {
                if (_RefLedgerId != value)
                {
                    _RefLedgerId = value;
                    NotifyPropertyChanged(nameof(RefLedgerId));
                }
            }
        }
        public bool AllowEdit
        {
            get
            {
                return _AllowEdit;
            }
            set
            {
                if (_AllowEdit != value)
                {
                    _AllowEdit = value;
                    NotifyPropertyChanged(nameof(AllowEdit));
                }
            }
        }
        #endregion
        public void SetGST()
        {
            if (IncludingGST)
            {
                GSTAmount = TaxMaster.SetRPGST(TaxDetails, Amount);
            }
            else
            {
                GSTAmount = 0;

            }
        }
        //public void GSTCalculation(ReceiptDetail rod, List<TaxMaster> TDetail)
        //{
        //    foreach (var t in TDetail)
        //    {
        //        Receipt_Tax_Detail.Add(new Receipt_Tax_Detail
        //        {
        //            Ledger = t.Ledger,
        //            RD_ID = rod.Id,
        //            TaxAmount = (t.TaxPercentage / 100) * pod.Amount,
        //            TaxId = t.Ledger.Id,
        //            TaxPercentage = t.TaxPercentage
        //        });
        //    }


        //}
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
