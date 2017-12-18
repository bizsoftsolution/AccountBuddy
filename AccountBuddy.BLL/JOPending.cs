using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountBuddy.Common;

namespace AccountBuddy.BLL
{
    public class JOPending : INotifyPropertyChanged
    {

        #region Fields

        private JobWorker _JobWorker;
        private DateTime? _JODate;
        private string _EntryNo;
        private decimal _Amount;
        private string _Status;


        private string _AccountName;
        private static UserTypeDetail _UserPermission;
        #endregion

        #region Property
        public static UserTypeDetail UserPermission
        {
            get
            {
                if (_UserPermission == null)
                {
                    _UserPermission = UserAccount.User.UserType == null ? new UserTypeDetail() : UserAccount.User.UserType.UserTypeDetails.Where(x => x.UserTypeFormDetail.FormName == Forms.frmJobOrderPendingReport.ToString()).FirstOrDefault();
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
        public JobWorker JobWorker
        {
            get
            {
                return _JobWorker;
            }
            set
            {
                if (_JobWorker != value)
                {
                    _JobWorker = value;
                    NotifyPropertyChanged(nameof(JobWorker));
                }
            }
        }

        public DateTime? JODate
        {
            get
            {
                return _JODate;
            }
            set
            {
                if (_JODate != value)
                {
                    _JODate = value;
                    NotifyPropertyChanged(nameof(JODate));

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

        public static List<JOPending> ToList(DateTime dtFrom, DateTime dtTo)
        {
            return FMCGHubClient.HubCaller.Invoke<List<JOPending>>("JOPending_List", dtFrom, dtTo).Result;
        }

        #endregion

    }
}
