﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBuddy.BLL
{
    public class SOPending : INotifyPropertyChanged
    {

        #region Fields

        private Ledger _Ledger;
        private DateTime? _SODate;
        private string _EntryNo;
        private decimal _Amount;
        private string _Status;


        private string _AccountName;
        #endregion

        #region Property
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

        public DateTime? SODate
        {
            get
            {
                return _SODate;
            }
            set
            {
                if (_SODate != value)
                {
                    _SODate = value;
                    NotifyPropertyChanged(nameof(SODate));

                }
            }
        }
        public string EntryNo
        {
            get
            {
                return _EntryNo;
            }
            set
            {
                if (_EntryNo != value)
                {
                    _EntryNo = value;
                    NotifyPropertyChanged(nameof(EntryNo));

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

                }
            }
        }

        public string Status
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

        public static List<SOPending> ToList(DateTime dtFrom, DateTime dtTo)
        {
            return FMCGHubClient.FMCGHub.Invoke<List<SOPending>>("SOPending_List", dtFrom, dtTo).Result;
        }

        #endregion
    }
}