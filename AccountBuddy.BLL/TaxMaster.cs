using AccountBuddy.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBuddy.BLL
{
   public class TaxMaster : INotifyPropertyChanged
    {
        #region Fields
        private static ObservableCollection<TaxMaster> _toList;

        private int _Id;
        private int _LedgerId;
        private decimal _TaxPercentage;
        private decimal _TaxAmount;
        private Ledger _Ledger;

        private static UserTypeDetail _UserPermission;
        private bool _IsReadOnly;
        private bool _IsEnabled;
        private bool _Status;
        private string _TaxName;
        #endregion

        #region Property

        public static UserTypeDetail UserPermission
        {
            get
            {
                if (_UserPermission == null)
                {
                    _UserPermission = UserAccount.User.UserType == null ? new UserTypeDetail() : UserAccount.User.UserType.UserTypeDetails.Where(x => x.UserTypeFormDetail.FormName == Forms.frmTaxMaster.ToString()).FirstOrDefault();
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

        public static ObservableCollection<TaxMaster> toList
        {
            get
            {
                try
                {
                    if (_toList == null)
                    {
                        _toList = new ObservableCollection<TaxMaster>();
                        var l1 = FMCGHubClient.HubCaller.Invoke<List<TaxMaster>>("TaxMaster_List").Result;
                        _toList = new ObservableCollection<TaxMaster>(l1.OrderBy(x => x.Ledger.LedgerName));
                    }
                }
                catch (Exception ex)
                {
                    Common.AppLib.WriteLog(ex);
                }

                return _toList;
            }
            set
            {
                _toList = value;
            }
        }

        public int Id
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
        public decimal TaxPercentage
        {
            get
            {
                return _TaxPercentage;
            }
            set
            {
                if (_TaxPercentage != value)
                {
                    _TaxPercentage = value;
                    NotifyPropertyChanged(nameof(TaxPercentage));
                }
            }
        }

        public decimal TaxAmount
        {
            get
            {
                return _TaxAmount;
            }
            set
            {
                if (_TaxAmount != value)
                {
                    _TaxAmount = value;
                    NotifyPropertyChanged(nameof(TaxAmount));
                }
            }
        }
        public bool Status
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
        public Ledger Ledger
        {
            get
            {
                if (_Ledger == null) _Ledger = new Ledger();
                return _Ledger;
            }

            set
            {
                if (_Ledger != value)
                {
                    _Ledger = value;
                    NotifyPropertyChanged(nameof(Ledger));
                    setTaxName();
                }
            }
        }

        private void setTaxName()
        {
            TaxName = string.Format("{0}({1})", Ledger.LedgerName, TaxPercentage);
        }

        public bool IsReadOnly
        {
            get
            {
                return _IsReadOnly;
            }

            set
            {
                if (_IsReadOnly != value)
                {
                    _IsReadOnly = value;
                    NotifyPropertyChanged(nameof(IsReadOnly));
                }
                IsEnabled = !value;
            }
        }

        public bool IsEnabled
        {
            get
            {
                return _IsEnabled;
            }

            set
            {
                if (_IsEnabled != value)
                {
                    _IsEnabled = value;
                    NotifyPropertyChanged(nameof(IsEnabled));
                }
            }
        }
        public string TaxName
        {
            get
            {
                return _TaxName;
            }

            set
            {
                if (_TaxName != value)
                {
                    _TaxName = value;
                    NotifyPropertyChanged(nameof(TaxName));
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

        #region Methods

        public bool Save(bool isServerCall = false)
        {
            if (!isValid()) return false;
            try
            {
                if (isServerCall == false)
                {
                    var d = FMCGHubClient.HubCaller.Invoke<TaxMaster>("TaxMaster_Save", this).Result;
                    if (d.Id != 0)
                    {
                        if (Id == 0)
                        {
                            toList.Add(d);
                            Ledger.toList.Add(d.Ledger);
                        }
                        else
                        {
                            var d1 = toList.Where(x => x.Id == d.Id).FirstOrDefault();
                            var l1 = Ledger.toList.Where(x => x.Id == d.LedgerId).FirstOrDefault();
                            d.ToMap(d1);
                            d.Ledger.ToMap(l1);
                        }
                        return true;
                    }
                }
                else
                {
                    var d1 = toList.Where(x => x.Id == Id).FirstOrDefault();
                    var l1 = Ledger.toList.Where(x => x.Id == LedgerId).FirstOrDefault();
                    if (d1 == null)
                    {
                        d1 = new TaxMaster();
                        toList.Add(d1);
                        l1 = new Ledger();
                        Ledger.toList.Add(l1);
                    }
                    this.ToMap(d1);
                    this.Ledger.ToMap(l1);
                }
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return false;

        }

        public bool isValid()
        {
            bool RValue = true;
            if (!Ledger.isValid())
            {
                RValue = false;
            }
            return RValue;

        }

        public void Clear()
        {
            try
            {
                new TaxMaster().ToMap(this);
                IsReadOnly = !UserPermission.AllowInsert;
                Ledger.Clear();
                Ledger.AccountGroupId = BLL.DataKeyValue.DutiesTaxes_Value;
                NotifyAllPropertyChanged();
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex);
            }
        }

        public bool Find(int pk)
        {
            var d = toList.Where(x => x.Id == pk).FirstOrDefault();
            if (d != null)
            {
                d.ToMap(this);
                IsReadOnly = !UserPermission.AllowUpdate;
                return true;
            }

            return false;
        }

        public bool Delete(bool isServerCall = false)
        {
            var rv = false;
            var d = toList.Where(x => x.Id == Id).FirstOrDefault();
            var b = FMCGHubClient.HubCaller.Invoke<bool>("Ledger_CanDeleteById", this.LedgerId).Result;
            if (d != null && b == true)
            {

                if (isServerCall == false)
                {
                    rv = FMCGHubClient.HubCaller.Invoke<bool>("TaxMaster_Delete", this.Id).Result;
                    if (rv == true)
                    {
                        toList.Remove(d);
                        var l1 = Ledger.toList.Where(x => x.Id == d.LedgerId).FirstOrDefault();
                        Ledger.toList.Remove(l1);
                    }

                }
                else
                {
                    toList.Remove(d);
                    var l1 = Ledger.toList.Where(x => x.Id == d.LedgerId).FirstOrDefault();
                    Ledger.toList.Remove(l1);
                }
                return rv;
            }

            return rv;
        }

    
        public static void Init()
        {
            _toList = null;
        }


        public static decimal SetGST(ObservableCollection<TaxMaster> TDetails, decimal IAmount, decimal DAmount)
        {            
            foreach (var t in TDetails)
            {
                
                t.TaxAmount = t.Status == false ? 0 : GetGST(IAmount, DAmount, t.TaxPercentage);
            }

            return TDetails.Sum(x => x.TaxAmount);

        }
        public static decimal GetGST(decimal ItemAmount, decimal DiscountAmount, decimal TaxPercentage)
        {
            return (ItemAmount - DiscountAmount) * TaxPercentage / 100;

        }
        public static decimal SetRPGST(ObservableCollection<TaxMaster> TDetails, decimal Amount)
        {
            foreach (var t in TDetails)
            {

                t.TaxAmount = t.Status == false ? 0 : GetRPGST(Amount, t.TaxPercentage);
            }

            return TDetails.Sum(x => x.TaxAmount);

        }
        public static decimal GetRPGST(decimal Amount,  decimal TaxPercentage)
        {
            return (Amount) * TaxPercentage / 100;

        }

        #endregion
    }
}
