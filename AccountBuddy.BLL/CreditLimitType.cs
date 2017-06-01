using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBuddy.BLL
{
    public class CreditLimitType : INotifyPropertyChanged
    {
        private static List<BLL.CreditLimitType> _tolist;


        private int _id;
        private string _limitType;


        #region Property

        public static List<BLL.CreditLimitType> toList
        {
            get
            {
                if (_tolist == null)
                {
                    _tolist = new List<CreditLimitType>();
                    _tolist = FMCGHubClient.FMCGHub.Invoke<List<BLL.CreditLimitType>>("creditLimitType_List").Result;
                }
                return _tolist;
            }
            set
            {
                _tolist = value;
            }
        }

        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                if (_id != value)
                {
                    _id = value;
                    NotifyPropertyChanged(nameof(Id));
                }
            }
        }

        public string LimitType
        {
            get
            {
                return _limitType;
            }
            set
            {
                if (_limitType != value)
                {
                    _limitType = value;
                    NotifyPropertyChanged(nameof(LimitType));
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
