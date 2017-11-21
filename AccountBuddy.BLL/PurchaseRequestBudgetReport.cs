using AccountBuddy.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBuddy.BLL
{
    public class PurchaseRequestBudgetReport : INotifyPropertyChanged
    {

        #region Fields

        private string _Department;
        private decimal _BudgetAmount;
        private decimal _ApprovedAmount;
        private decimal _BalanceAmount;
        private decimal _RequestAmount;
        private decimal _RemainingAmount;

        private static UserTypeDetail _UserPermission;
        #endregion

        #region Property

        public static UserTypeDetail UserPermission
        {
            get
            {
                if (_UserPermission == null)
                {
                    _UserPermission = UserAccount.User.UserType == null ? new UserTypeDetail() : UserAccount.User.UserType.UserTypeDetails.Where(x => x.UserTypeFormDetail.FormName == Forms.frmPurchaseRequestReport).FirstOrDefault();
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
        public decimal BudgetAmount
        {
            get
            {
                return _BudgetAmount;
            }
            set
            {
                if (_BudgetAmount != value)
                {
                    _BudgetAmount = value;
                    NotifyPropertyChanged(nameof(BudgetAmount));

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
        public decimal BalanceAmount
        {
            get
            {
                return _BalanceAmount;
            }
            set
            {
                if (_BalanceAmount != value)
                {
                    _BalanceAmount = value;
                    NotifyPropertyChanged(nameof(BalanceAmount));

                }
            }
        }
        public decimal RequestAmount
        {
            get
            {
                return _RequestAmount;
            }
            set
            {
                if (_RequestAmount != value)
                {
                    _RequestAmount = value;
                    NotifyPropertyChanged(nameof(RequestAmount));

                }
            }
        }
        public decimal RemainingAmount
        {
            get
            {
                return _RemainingAmount;
            }
            set
            {
                if (_RemainingAmount != value)
                {
                    _RemainingAmount = value;
                    NotifyPropertyChanged(nameof(RemainingAmount));

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

        public static List<PurchaseRequestReport> ToList(DateTime dtFrom, DateTime dtTo)
        {
            return FMCGHubClient.FMCGHub.Invoke<List<PurchaseRequestReport>>("PurchaseRequestReport_List", dtFrom, dtTo).Result;
        }

        #endregion

    }
}
