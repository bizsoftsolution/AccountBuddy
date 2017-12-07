using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBuddy.BLL
{
    public class LedgerList : INotifyPropertyChanged
    {


        private string _AccountName;
        private static ObservableCollection<LedgerList> _toList;
        private int _Id;
        private AccountGroup _AccountGroup;
        private Ledger _Ledger;

        public string AccountName
        {
            get
            {
                return _AccountName;
            }

            set
            {
                if (_AccountName != value)
                {
                    _AccountName = value;
                    NotifyPropertyChanged(nameof(AccountName));
                }
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
        public AccountGroup AccountGroup
        {
            get
            {
                return _AccountGroup;
            }

            set
            {
                if (_AccountGroup != value)
                {
                    _AccountGroup = value;
                    NotifyPropertyChanged(nameof(AccountGroup));
                }
            }
        }
        public Ledger Ledger
        {
            get
            {
                return _Ledger;
            }

            set
            {
                if (_Ledger != value)
                {
                    _Ledger = value;
                    NotifyPropertyChanged(nameof(Ledger));
                }
            }
        }

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

        public static ObservableCollection<LedgerList> toList
        {
            get
            {
                if (_toList == null) _toList = new ObservableCollection<LedgerList>(FMCGHubClient.FMCGHub.Invoke<List<LedgerList>>("LedgerList").Result);
                return _toList;
            }
            set
            {
                _toList = value;
            }
        }

        #endregion
    }
}
