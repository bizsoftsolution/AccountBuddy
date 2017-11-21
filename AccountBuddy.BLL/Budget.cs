using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBuddy.BLL
{
    public class Budget : INotifyPropertyChanged
    {

        #region Fields


        private string _Department;
        private decimal _Budgets;
        private decimal _ApprovedAmount;
        private decimal _Balance;
        private decimal _Request;
        private decimal _Remaining;


        private static UserTypeDetail _UserPermission;
        #endregion

        #region Property


        public string Department
        {
            get
            {
                return _Department;
            }
            set
            {
                if (_Department != value)
                {
                    _Department = value;
                    NotifyPropertyChanged(nameof(Department));

                }
            }
        }

        public decimal Budgets
        {
            get
            {
                return _Budgets;
            }
            set
            {
                if (_Budgets != value)
                {
                    _Budgets = value;
                    NotifyPropertyChanged(nameof(Budgets));

                }
            }
        }

        public decimal ApprovedAmount
        {
            get
            {
                return _ApprovedAmount;
            }
            set
            {
                if (_ApprovedAmount != value)
                {
                    _ApprovedAmount = value;
                    NotifyPropertyChanged(nameof(ApprovedAmount));

                }
            }
        }

        public decimal Balance
        {
            get
            {
                return _Balance;
            }
            set
            {
                if (_Balance != value)
                {
                    _Balance = value;
                    NotifyPropertyChanged(nameof(Balance));

                }
            }
        }


        public decimal Request
        {
            get
            {
                return _Request;
            }
            set
            {
                if (_Request != value)
                {
                    _Request = value;
                    NotifyPropertyChanged(nameof(Request));

                }
            }
        }

        public decimal Remaining
        {
            get
            {
                return _Remaining;
            }
            set
            {
                if (_Remaining != value)
                {
                    _Remaining = value;
                    NotifyPropertyChanged(nameof(Remaining));

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
