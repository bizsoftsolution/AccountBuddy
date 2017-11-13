using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBuddy.BLL
{
    class PurchaseRequestStatusDetail : INotifyPropertyChanged
    {
        #region Field
        private long _Id;
        private long _PRId;
        private Staff _RequestBy;
        private DateTime _RequestAt;
        private Staff _RequestTo;
        private DateTime _ResponseAt;
        private string _Remarks;
        private string _Status;

        #endregion

        #region Property

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

        public long PRId
        {
            get
            {
                return _PRId;
            }
            set
            {
                if (_PRId != value)
                {
                    _PRId = value;
                    NotifyPropertyChanged(nameof(PRId));
                }
            }
        }

        public Staff RequestBy
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

        public DateTime RequestAt
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

        public Staff RequestTo
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

        public DateTime ResponseAt
        {
            get
            {
                return _ResponseAt;
            }
            set
            {
                if (_ResponseAt != value)
                {
                    _ResponseAt = value;
                    NotifyPropertyChanged(nameof(ResponseAt));
                }
            }
        }

        public string Remarks
        {
            get
            {
                return _Remarks;
            }
            set
            {
                if (_Remarks != value)
                {
                    _Remarks = value;
                    NotifyPropertyChanged(nameof(Remarks));
                }
            }
        }

        public String Status
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

        #region Property Changed
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String ProperName)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(ProperName));
        }
        private void NotifyAllPropertyChanged()
        {
            foreach (var p in this.GetType().GetProperties()) NotifyPropertyChanged(p.Name);
        }

        #endregion

        #region Methods

        #endregion

    }
}
