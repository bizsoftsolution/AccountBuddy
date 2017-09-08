using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBuddy.BLL
{
    public class StateDetail : INotifyPropertyChanged
    {
        #region Fields
        private static List<StateDetail> _toList;

        private int _Id;
        private string _StateName;
        private string _TINNo;
        private string _StateCode;
      
        #endregion

        #region Property


        public static List<StateDetail> toList
        {
            get
            {
                try
                {
                    if (_toList == null) _toList = new List<StateDetail>(FMCGHubClient.FMCGHub.Invoke<List<StateDetail>>("StateDetail_List").Result);
                    return _toList;
                }
                catch (Exception ex)
                {

                }

                return _toList;
            }
            set
            {
                _toList = value;
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
                    NotifyPropertyChanged(nameof(Id));
                }
            }
        }

        public string StateName
        {
            get
            {
                return _StateName;
            }
            set
            {
                if (_StateName != value)
                {
                    _StateName = value;
                    NotifyPropertyChanged(nameof(StateName));
                }
            }
        }
        public string TINNo
        {
            get
            {
                return _TINNo;
            }
            set
            {
                if (_TINNo != value)
                {
                    _TINNo = value;
                    NotifyPropertyChanged(nameof(TINNo));
                }
            }
        }

        public string StateCode
        {
            get
            {
                return _StateCode;
            }
            set
            {
                if (_StateCode != value)
                {
                    _StateCode = value;
                    NotifyPropertyChanged(nameof(_StateCode));
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

        public static void Init()
        {
            _toList = null;
        }


        #endregion
    }
}
