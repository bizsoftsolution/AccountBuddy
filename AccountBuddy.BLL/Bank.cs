using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBuddy.BLL
{
    public class Bank : INotifyPropertyChanged
    {
        #region fields
        private int _Id;
        private string _AccountNo;
        private string _AccountName;
        private Ledger _Ledger;
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

    }
}
