using AccountBuddy.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBuddy.BLL
{
    public class PurchaseRequestReport : INotifyPropertyChanged
    {

        #region Fields
        private long _PurchaseRequestId;
        private string _PurchaseRequestRefNo;
        private long _PurchaseRequestStatusDetailsId;
        private string _RequestBy;
        private string _RequestTo;
        private DateTime? _RequestAt;
        private DateTime? _ResponseAt;
        private string _SupplierName;
        private string _Particulars;
        private decimal _Amount;
        private string _Department;
        private string _Status;
        private string _Remarks;
        private bool _IsNew;
        private bool _IsHold;
        private bool _IsReject;
        private bool _IsApproval;
        private bool _IsRequestTo;
        private bool _IsAdmin;

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
        public long PurchaseRequestId
        {
            get
            {
                return _PurchaseRequestId;
            }
            set
            {
                if (_PurchaseRequestId != value)
                {
                    _PurchaseRequestId = value;
                    NotifyPropertyChanged(nameof(PurchaseRequestId));
                }
            }
        }
        public long PurchaseRequestStatusDetailsId
        {
            get
            {
                return _PurchaseRequestStatusDetailsId;
            }
            set
            {
                if (_PurchaseRequestStatusDetailsId != value)
                {
                    _PurchaseRequestStatusDetailsId = value;
                    NotifyPropertyChanged(nameof(PurchaseRequestStatusDetailsId));
                }
            }
        }
        public string PurchaseRequestRefNo
        {
            get
            {
                return _PurchaseRequestRefNo;
            }
            set
            {
                if (_PurchaseRequestRefNo != value)
                {
                    _PurchaseRequestRefNo = value;
                    NotifyPropertyChanged(nameof(PurchaseRequestRefNo));

                }
            }
        }
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
        public DateTime? ResponseAt
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
        public string SupplierName
        {
            get
            {
                return _SupplierName;
            }
            set
            {
                if (_SupplierName != value)
                {
                    _SupplierName = value;
                    NotifyPropertyChanged(nameof(SupplierName));

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

        public bool IsNew
        {
            get
            {
                return _IsNew;
            }
            set
            {
                if (_IsNew != value)
                {
                    _IsNew = value;
                    NotifyPropertyChanged(nameof(IsNew));

                }
            }
        }

        public bool IsHold
        {
            get
            {
                return _IsHold;
            }
            set
            {
                if (_IsHold != value)
                {
                    _IsHold = value;
                    NotifyPropertyChanged(nameof(IsHold));

                }
            }
        }

        public bool IsReject
        {
            get
            {
                return _IsReject;
            }
            set
            {
                if (_IsReject != value)
                {
                    _IsReject = value;
                    NotifyPropertyChanged(nameof(IsReject));

                }
            }
        }

        public bool IsApproval
        {
            get
            {
                return _IsApproval;
            }
            set
            {
                if (_IsApproval != value)
                {
                    _IsApproval = value;
                    NotifyPropertyChanged(nameof(IsApproval));

                }
            }
        }

        public bool IsRequestTo
        {
            get
            {
                return _IsRequestTo;
            }
            set
            {
                if (_IsRequestTo != value)
                {
                    _IsRequestTo = value;
                    NotifyPropertyChanged(nameof(IsRequestTo));

                }
            }
        }

        public bool IsAdmin
        {
            get
            {
                return _IsAdmin;
            }
            set
            {
                if (_IsAdmin != value)
                {
                    _IsAdmin = value;
                    NotifyPropertyChanged(nameof(IsAdmin));

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
