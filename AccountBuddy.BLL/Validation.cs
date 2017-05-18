using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBuddy.BLL
{
    public class Validation : INotifyPropertyChanged
    {
        #region Fields
        private string _name;
        private string _message;
        #endregion

        #region Properties
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    NotifyPropertyChanged(nameof(Name));
                }
            }
        }

        public string Message
        {
            get
            {
                return _message;
            }

            set
            {
                if (_message != value)
                {
                    _message = value;
                    NotifyPropertyChanged(nameof(Message));
                }
            }
        }
        #endregion

        #region Methods and Events
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }

        #endregion
    }
}
