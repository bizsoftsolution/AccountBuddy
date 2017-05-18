using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBuddy.Common
{
    public class NavMenuItem : INotifyPropertyChanged
    {
        private string _name;
        private object _content;
        private string _formname;

        public string MenuName
        {
            get { return _name; }
            set
            {
                this.MutateVerbose(ref _name, value, RaisePropertyChanged());
            }
        }

        public string FormName
        {
            get { return _formname; }
            set
            {
                this.MutateVerbose(ref _formname, value, RaisePropertyChanged());
            }
        }

        public object Content
        {
            get { return _content; }
            set
            {
                this.MutateVerbose(ref _content, value, RaisePropertyChanged());
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private Action<PropertyChangedEventArgs> RaisePropertyChanged()
        {
            return args => PropertyChanged?.Invoke(this, args);
        }
    }
}
