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
    public class Bank : INotifyPropertyChanged
    {
        #region fields
        private int _Id;
        private string _AccountNo;
        private string _AccountName;
        private Ledger _Ledger;
        private int _LedgerId;
        private static ObservableCollection<Bank> _toList;
        #endregion

        #region Property
        public int Id
        {
            get
            {
                return _Id;
            }
            set
            {
                if(_Id!=value)
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

        public string AccountName
        {
            get
            {
                return _AccountName;
            }
            set
            {
                if(_AccountName!=value)
                {
                    _AccountName = value;
                    NotifyPropertyChanged(nameof(AccountName));
                }
            }
        }

        public string AccountNo
        {
            get
            {
                return _AccountNo;
            }
            set
            {
                if(_AccountNo!=value)
                {
                    _AccountNo = value;
                    NotifyPropertyChanged(nameof(AccountNo));
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
                if(_Ledger!=value)
                {
                    _Ledger = value;
                    NotifyPropertyChanged(nameof(Ledger));
                }
            }
        }
        public static ObservableCollection<Bank> toList
        {
            get
            {
                if (_toList == null) _toList = new ObservableCollection<Bank>(FMCGHubClient.FMCGHub.Invoke<List<Bank>>("Bank_List").Result);
                return _toList;
            }
            set
            {
                _toList = value;
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
                    var d = FMCGHubClient.FMCGHub.Invoke<Bank>("Bank_Save", this).Result;
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
                            d.toCopy<Bank>(d1);
                            d.Ledger.toCopy<Ledger>(l1);
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
                        d1 = new Bank();
                        toList.Add(d1);
                        l1 = new Ledger();
                        Ledger.toList.Add(l1);
                    }
                    this.toCopy<Bank>(d1);
                    this.Ledger.toCopy<Ledger>(l1);
                }
            }
            catch (Exception ex) { }
            return false;

        }

        public void Clear()
        {
            new Bank().toCopy<Bank>(this);
            this.Ledger.Clear();
            NotifyAllPropertyChanged();
        }

        public bool Find(int pk)
        {
            var d = toList.Where(x => x.Id == pk).FirstOrDefault();
            if (d != null)
            {
                d.toCopy<Bank>(this);
               // IsReadOnly = !UserPermission.AllowUpdate;

                return true;
            }

            return false;
        }

        public bool Delete(bool isServerCall = false)
        {
            var rv = false;
            var d = toList.Where(x => x.Id == Id).FirstOrDefault();
            var b = FMCGHubClient.FMCGHub.Invoke<bool>("Ledger_CanDeleteById", this.LedgerId).Result;
            if (d != null && b == true)
            {

                if (isServerCall == false)
                {
                    rv = FMCGHubClient.FMCGHub.Invoke<bool>("Bank_Delete", this.Id).Result;
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

        public bool isValid()
        {
            bool RValue = true;
            if (!Ledger.isValid())
            {
                RValue = false;
            }
            return RValue;

        }

        public static void Init()
        {
            _toList = null;
        }

       


        #endregion
    }
}
