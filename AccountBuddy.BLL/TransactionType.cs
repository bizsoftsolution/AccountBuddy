using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBuddy.BLL
{
    public class TransactionType : INotifyPropertyChanged
    {
        #region Fields
        private int _Id;
        private string _Type;

        #endregion

        #region Property

        public int Id
        {
            get
            {
                return _Id;
            }
            set
            {
                if(_Id!=value)
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
                if(_Type!=value)
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
