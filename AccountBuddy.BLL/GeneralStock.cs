using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountBuddy.Common;

namespace AccountBuddy.BLL
{
   public class GeneralStock : INotifyPropertyChanged
    {
        #region Fields
        private long _EId;
        private string _EType;
        private DateTime? _EDate;
        private String _EntryNo;
        private string _TType;
        private Product _Product;
        private decimal _Inwards;
        private decimal _Outwards;
        private decimal _BalStock;
        private string _LedgerName;
        private string _Particular;
        private Ledger _Ledger;
        private static UserTypeDetail _UserPermission;

        #endregion

        #region Property
        public static UserTypeDetail UserPermission
        {
            get
            {
                if (_UserPermission == null)
                {
                    _UserPermission = UserAccount.User.UserType == null ? new UserTypeDetail() : UserAccount.User.UserType.UserTypeDetails.Where(x => x.UserTypeFormDetail.FormName == Forms.frmGeneralStock.ToString()).FirstOrDefault();
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

        public long EId
        {
            get
            {
                return _EId;
            }
            set
            {
                if (_EId != value)
                {
                    _EId = value;
                    NotifyPropertyChanged(nameof(EId));
                }
            }
        }
        public string EType
        {
            get
            {
                return _EType;
            }
            set
            {
                if (_EType != value)
                {
                    _EType = value;
                    NotifyPropertyChanged(nameof(EType));
                }
            }
        }
        public DateTime? EDate
        {
            get
            {
                return _EDate;
            }
            set
            {
                if (_EDate != value)
                {
                    _EDate = value;
                    NotifyPropertyChanged(nameof(EDate));
                }
            }
        }
        public string TType
        {
            get
            {
                return _TType;
            }
            set
            {
                if (_TType != value)
                {
                    _TType = value;
                    NotifyPropertyChanged(nameof(TType));
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
        public Product Product
        {
            get
            {
                return _Product;
            }
            set
            {
                if (_Product != value)
                {
                    _Product = value;
                    NotifyPropertyChanged(nameof(Product));
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
        public decimal Inwards
        {
            get
            {
                return _Inwards;
            }
            set
            {
                if (_Inwards != value)
                {
                    _Inwards = value;
                    NotifyPropertyChanged(nameof(Inwards));
                }
            }
        }
        public decimal Outwards
        {
            get
            {
                return _Outwards;
            }
            set
            {
                if (_Outwards != value)
                {
                    _Outwards = value;
                    NotifyPropertyChanged(nameof(Outwards));
                }
            }
        }
        public decimal BalStock
        {
            get
            {
                return _BalStock;
            }
            set
            {
                if (_BalStock != value)
                {
                    _BalStock = value;
                    NotifyPropertyChanged(nameof(BalStock));
                }
            }
        }

        public string LedgerName
        {
            get
            {
                return _LedgerName;
            }
            set
            {
                if (_LedgerName != value)
                {
                    _LedgerName = value;
                    NotifyPropertyChanged(nameof(LedgerName));
                }
            }
        }

        public string Particular
        {
            get
            {
                return _Particular;
            }
            set
            {
                if (_Particular != value)
                {
                    _Particular = value;
                    NotifyPropertyChanged(nameof(Particular));
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

        public static List<GeneralStock> ToList(int? CompanyId, int ProductId, DateTime dtFrom, DateTime dtTo)
        {
            return FMCGHubClient.HubCaller.Invoke<List<GeneralStock>>("GeneralStock_List", CompanyId, ProductId, dtFrom, dtTo).Result;
        }

        #endregion
    }
}
