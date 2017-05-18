﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountBuddy.Common;
using System.Collections.ObjectModel;

namespace AccountBuddy.BLL
{
    public class UserType : INotifyPropertyChanged
    {
        #region Field

        private static ObservableCollection<UserType> _toList;
        public List<BLL.Validation> lstValidation = new List<BLL.Validation>();
        
        private int _id;
        private string _typeOfUser;
        private string _description;

        private ObservableCollection<UserTypeDetail> _UserTypeDetails;
        
        
        #endregion

        #region Property

        public static ObservableCollection<UserType> toList
        {
            get
            {
                if (_toList == null)
                {
                    _toList =new ObservableCollection<UserType>( ABClientHub.FMCGHub.Invoke<List<UserType>>("UserType_List").Result);
                }

                return _toList;
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
        public string TypeOfUser
        {
            get
            {
                return _typeOfUser;
            }
            set
            {
                if (_typeOfUser != value)
                {
                    _typeOfUser = value;
                    NotifyPropertyChanged(nameof(TypeOfUser));
                }
            }
        }
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    NotifyPropertyChanged(nameof(Description));
                }
            }
        }

        public ObservableCollection<UserTypeDetail> UserTypeDetails
        {
            get
            {
                if (_UserTypeDetails == null) _UserTypeDetails = new ObservableCollection<UserTypeDetail>();
                return _UserTypeDetails;
            }
            set
            {
                if (_UserTypeDetails != value)
                {
                    _UserTypeDetails = value;
                    NotifyPropertyChanged(nameof(UserTypeDetails));
                }
            }
        }

        #endregion

        #region Property Notify Changed

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

        public bool Save(bool isServerCall = false)
        {
            if (!isValid()) return false;
            try
            {

                UserType d = toList.Where(x => x.Id == Id).FirstOrDefault();

                if (d == null)
                {
                    d = new UserType();
                    toList.Add(d);
                }

                this.toCopy<UserType>(d);
                if (isServerCall == false)
                {
                    var i = ABClientHub.FMCGHub.Invoke<int>("userType_Save", this).Result;
                    d.Id = i;
                }

                return true;
            }
            catch (Exception ex)
            {
                lstValidation.Add(new Validation() { Name = string.Empty, Message = ex.Message });
                return false;
            }

        }

        public void Clear()
        {
            new UserType().toCopy<UserType>(this);
            this.UserTypeDetails = new ObservableCollection<UserTypeDetail>();
            foreach(var d in UserTypeFormDetail.toList)
            {
                UserTypeDetail utd = new UserTypeDetail();
                UserTypeDetails.Add(utd);
                utd.UserTypeFormDetailId = d.Id;
                utd.FormName = d.FormName;
                utd.FormType = d.FormType;             
            }
            NotifyAllPropertyChanged();
        }

        public bool Find(int pk)
        {
            var d = toList.Where(x => x.Id == pk).FirstOrDefault();
            if (d != null)
            {
                d.toCopy<UserType>(this);
                return true;
            }

            return false;
        }

        public bool Delete(bool isServerCall = false)
        {
            var d = toList.Where(x => x.Id == Id).FirstOrDefault();
            if (d != null)
            {
                toList.Remove(d);
                if (isServerCall == false) ABClientHub.FMCGHub.Invoke<int>("userType_Delete", this.Id);
                return true;
            }

            return false;
        }
        public bool isValid()
        {
            bool RValue = true;
            lstValidation.Clear();

            if (string.IsNullOrWhiteSpace(TypeOfUser))
            {
                lstValidation.Add(new Validation() { Name = nameof(TypeOfUser), Message = string.Format(Message.BLL.Required_Data, nameof(TypeOfUser)) });
                RValue = false;
            }
            else if (toList.Where(x => x.TypeOfUser.ToLower() == TypeOfUser.ToLower() && x.Id != Id).Count() > 0)
            {
                lstValidation.Add(new Validation() { Name = nameof(TypeOfUser), Message = string.Format(Message.BLL.Existing_Data, TypeOfUser) });
                RValue = false;
            }
            
            return RValue;

        }

        #endregion
    }
}
