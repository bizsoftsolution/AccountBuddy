using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBuddy.BLL
{
    public class TaxType : INotifyPropertyChanged
    {
        #region Fields
        private int _Id;
        private string _Type;
        private static List<BLL.TaxType> _tolist;

        #endregion

        #region Property
        public static List<BLL.TaxType> toList
        {
            get
            {
                if (_tolist == null)
                {
                    _tolist = new List<TaxType>();
                    _tolist = FMCGHubClient.HubCaller.Invoke<List<BLL.TaxType>>("TaxType_List").Result;
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
                return _Id;
            }
            set
            {
                if (_Id != value)
                {
                    _Id = value;
                    NotifypropertyChanged(nameof(Id));

                }
            }
        }

        public string Type
        {
            get
            {
                return _Type;
            }
            set
            {
                if (_Type != value)
                {
                    _Type = value;
                    NotifypropertyChanged(nameof(Type));
                }
            }
        }
        #endregion

        #region PropertyChanged Event
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifypropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }

        private void NotifyAllPropertyChanged()
        {
            foreach (var p in this.GetType().GetProperties()) NotifypropertyChanged(nameof(p.Name));
        }
        #endregion
    }
}
