using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBuddy.BLL
{
    public class PaymentDetail : INotifyPropertyChanged
    {
        #region Fields
        private long _Id;
        private long _PaymentId;
        private int _LedgerId;
        private decimal _Amount;
        private string _Particular;
        private ObservableCollection<Payment_Tax_Detail> _PaymentTaxDetails;

        private string _LedgerName;
        private int _SNo;
        private int _GSTStatusId;
        private ObservableCollection<TaxMaster> _TaxDetails;
        private decimal _GSTAmount;
        private int _GSTDRefNo;
        private int _RefLedgerId;
        private bool _IncludingGST;
        private bool _AllowEdit;

        public ObservableCollection<Payment_Tax_Detail> PaymentTaxDetails
        {
            get
            {
                if (_PaymentTaxDetails == null) _PaymentTaxDetails = new ObservableCollection<Payment_Tax_Detail>();
                return _PaymentTaxDetails;
            }
            set
            {
                if (_PaymentTaxDetails != value)
                {
                    _PaymentTaxDetails = value;
                    NotifyPropertyChanged(nameof(PaymentTaxDetails));
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

        #endregion

        #region Property
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
        public long PaymentId
        {
            get
            {
                return _PaymentId;
            }
            set
            {
                if (_PaymentId != value)
                {
                    _PaymentId = value;
                    NotifyPropertyChanged(nameof(PaymentId));
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
        public string Particular
        {
            get
            {
                return _Particular;
            }
            set
            {
                if (_Particular != value)
                {
                    _Particular = value;
                    NotifyPropertyChanged(nameof(Particular));
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
                    //SetGST();
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
        public int GSTDRefNo
        {
            get
            {
                return _GSTDRefNo;
            }
            set
            {
                if (_GSTDRefNo != value)
                {
                    _GSTDRefNo = value;
                    NotifyPropertyChanged(nameof(GSTDRefNo));
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

        public void GSTCalculation(PaymentDetail pod, List<TaxMaster> TDetail)
        {
            foreach (var t in TDetail)
            {
                PaymentTaxDetails.Add(new Payment_Tax_Detail
                {
                    Ledger = t.Ledger,
                    PD_ID = pod.Id,
                    TaxAmount = (t.TaxPercentage / 100) * pod.Amount,
                    TaxId = t.Ledger.Id,
                    TaxPercentage = t.TaxPercentage
                });
            }


        }
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
    }
}
