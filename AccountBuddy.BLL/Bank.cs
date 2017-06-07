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

                Bank d = toList.Where(x => x.Id == Id).FirstOrDefault();

                if (d == null)
                {
                    d = new Bank();
                    toList.Add(d);
                }

                Ledger.AccountGroupId = BLL.DataKeyValue.SundryDebtors;
                this.toCopy<Bank>(d);
                if (isServerCall == false)
                {
                    var i = FMCGHubClient.FMCGHub.Invoke<int>("Bank_Save", this).Result;
                    d.Id = i;
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;

            }

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
            if (d != null)
            {

                if (isServerCall == false)
                {
                    rv = FMCGHubClient.FMCGHub.Invoke<bool>("Bank_Delete", this.Id).Result;
                    if (rv == true) toList.Remove(d);

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
