using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBuddy.BLL
{
    public class DataKeyValue : INotifyPropertyChanged
    {


        #region fields
        private static List<DataKeyValue> _toList;
        private int _Id;
        private string _DataKey;
        private int _DataValue;
        private int _CompanyId;
        #endregion

        #region Property

        #region Static Property
        #region AccountsGroup
        public static int Assets
        {
            get
            {
                return toList.Where(x => x.DataKey == nameof(Assets)).FirstOrDefault().DataValue;
            }
        }
        public static int Liabilities
        {
            get
            {
                return toList.Where(x => x.DataKey == nameof(Liabilities)).FirstOrDefault().DataValue;
            }
        }
        public static int Income
        {
            get
            {
                return toList.Where(x => x.DataKey == nameof(Income)).FirstOrDefault().DataValue;
            }
        }
        public static int Expenses
        {
            get
            {
                return toList.Where(x => x.DataKey == nameof(Expenses)).FirstOrDefault().DataValue;
            }
        }
        public static int SundryCreditors
        {
            get
            {
                return toList.Where(x => x.DataKey == nameof(SundryCreditors)).FirstOrDefault().DataValue;
            }
        }
        public static int SundryDebtors
        {
            get
            {
                return toList.Where(x => x.DataKey == nameof(SundryDebtors)).FirstOrDefault().DataValue;
            }
        }

        #endregion

        public static List<DataKeyValue> toList
        {
            get
            {
                if (_toList == null) _toList = FMCGHubClient.FMCGHub.Invoke<List<DataKeyValue>>("DataKeyValue_List").Result;
                return _toList;
            }
            set
            {
                _toList = value;
            }
        }
        #endregion

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

        public string DataKey
        {
            get
            {
                return _DataKey;
            }
            set
            {
                if (_DataKey != value)
                {
                    _DataKey = value;
                    NotifyPropertyChanged(nameof(DataKey));
                }
            }
        }

        public int DataValue
        {
            get
            {
                return _DataValue;
            }
            set
            {
                if (_DataValue != value)
                {
                    _DataValue = value;
                    NotifyPropertyChanged(nameof(DataValue));
                }

            }
        }

        public int CompanyId
        {
            get
            {
                return _CompanyId;
            }
            set
            {
                if (_CompanyId != value)
                {
                    _CompanyId = value;
                    NotifyPropertyChanged(nameof(CompanyId));
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
        public static void Init()
        {
            _toList = null;
        }
    }
}
