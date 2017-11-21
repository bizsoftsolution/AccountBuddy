using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBuddy.BLL
{
  public  class RequestReport : INotifyPropertyChanged
    {

        #region Fields
      
        private DateTime? _RequestAt;
        private string _RequestBy;
        private string _RequestTo;
        private decimal _Amount;
        private string _Particulars;
        private string _Status;
    

        private static UserTypeDetail _UserPermission;
        #endregion

        #region Property

      
        public DateTime? RequestAt
        {
            get
            {
                return _RequestAt;
            }
            set
            {
                if (_RequestAt != value)
                {
                    _RequestAt = value;
                    NotifyPropertyChanged(nameof(RequestAt));

                }
            }
        }
        public string RequestBy
        {
            get
            {
                return _RequestBy;
            }
            set
            {
                if (_RequestBy != value)
                {
                    _RequestBy = value;
                    NotifyPropertyChanged(nameof(RequestBy));

                }
            }
        }
        public string RequestTo
        {
            get
            {
                return _RequestTo;
            }
            set
            {
                if (_RequestTo != value)
                {
                    _RequestTo = value;
                    NotifyPropertyChanged(nameof(RequestTo));

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

        public string Department
        {
            get
            {
                return Particulars;
            }
            set
            {
                if (Particulars != value)
                {
                    Particulars = value;
                    NotifyPropertyChanged(nameof(Department));

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

