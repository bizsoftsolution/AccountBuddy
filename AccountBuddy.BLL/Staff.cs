﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountBuddy.Common;

namespace AccountBuddy.BLL
{
   public class Staff : INotifyPropertyChanged
    {
        #region Fileds

        private static ObservableCollection<Staff> _toList;

        private int _Id;
        private int _LedgerId;
        private Ledger _Ledger;
        private decimal _Salary;
        private string _Designation;
        private DateTime _DOB;
        private DateTime _DOJ;


        private static UserTypeDetail _UserPermission;
        private bool _IsReadOnly;
        private bool _IsEnabled;

        #endregion

        #region Property
        public static UserTypeDetail UserPermission
        {
            get
            {
                if (_UserPermission == null)
                {
                    _UserPermission = UserAccount.User.UserType == null ? new UserTypeDetail() : UserAccount.User.UserType.UserTypeDetails.Where(x => x.UserTypeFormDetail.FormName == AppLib.Forms.frmStaff.ToString()).FirstOrDefault();
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
        public bool IsReadOnly
        {
            get
            {
                return _IsReadOnly;
            }

            set
            {
                if (_IsReadOnly != value)
                {
                    _IsReadOnly = value;
                    NotifyPropertyChanged(nameof(IsReadOnly));
                }
                IsEnabled = !value;
            }
        }

        public bool IsEnabled
        {
            get
            {
                return _IsEnabled;
            }

            set
            {
                if (_IsEnabled != value)
                {
                    _IsEnabled = value;
                    NotifyPropertyChanged(nameof(IsEnabled));
                }
            }
        }

        public static ObservableCollection<Staff> toList
        {
            get
            {
                if (_toList == null) _toList = new ObservableCollection<Staff>(FMCGHubClient.FMCGHub.Invoke<List<Staff>>("Staff_List").Result);
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
        public int LedgerId
        {
            get
            {
                return _LedgerId;
            }

            set
            {
                if (_LedgerId != value)
                {
                    _LedgerId = value;
                    NotifyPropertyChanged(nameof(LedgerId));
                    SetAccountName();
                }
            }
        }
        public Ledger Ledger
        {
            get
            {
                if (_Ledger == null) _Ledger = new Ledger();
                return _Ledger;
            }

            set
            {
                if (_Ledger != value)
                {
                    _Ledger = value;
                    NotifyPropertyChanged(nameof(Ledger));
                    //SetAccountName();
                }
            }
        }

        public decimal Salary
        {
            get
            {
                  return _Salary;
            }

            set
            {
                if (_Salary != value)
                {
                    _Salary = value;
                    NotifyPropertyChanged(nameof(Salary));
                    
                }
            }
        }
        public string Designation
        {
            get
            {
                return _Designation;
            }

            set
            {
                if (_Designation != value)
                {
                    _Designation = value;
                    NotifyPropertyChanged(nameof(Designation));

                }
            }
        }
        public DateTime DOB
        {
            get
            {
                return _DOB;
            }

            set
            {
                if (_DOB != value)
                {
                    _DOB = value;
                    NotifyPropertyChanged(nameof(DOB));

                }
            }
        }
        public DateTime DOJ
        {
            get
            {
                return _DOJ;
            }

            set
            {
                if (_DOJ != value)
                {
                    _DOJ = value;
                    NotifyPropertyChanged(nameof(DOJ));

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

        public bool Save(bool isServerCall = false)
        {
            if (!isValid()) return false;
            try
            {
                if (isServerCall == false)
                {
                    var d = FMCGHubClient.FMCGHub.Invoke<Staff>("Staff_Save", this).Result;
                    if (d.Id != 0)
                    {
                        if (Id == 0)
                        {
                            toList.Add(d);
                            Ledger.toList.Add(d.Ledger);
                        }
                        else
                        {
                            var d1 = toList.Where(x => x.Id == d.Id).FirstOrDefault();
                            var l1 = Ledger.toList.Where(x => x.Id == d.LedgerId).FirstOrDefault();
                            d.toCopy<Staff>(d1);
                            d.Ledger.toCopy<Ledger>(l1);
                        }
                        return true;
                    }
                }
                else
                {
                    var d1 = toList.Where(x => x.Id == Id).FirstOrDefault();
                    var l1 = Ledger.toList.Where(x => x.Id == LedgerId).FirstOrDefault();
                    if (d1 == null)
                    {
                        d1 = new Staff();
                        toList.Add(d1);
                        l1 = new Ledger();
                        Ledger.toList.Add(l1);
                    }
                    this.toCopy<Staff>(d1);
                    this.Ledger.toCopy<Ledger>(l1);
                }
            }
            catch (Exception ex) { }
            return false;
        }

        public void Clear()
        {
            new Staff().toCopy<Staff>(this);
            this.Ledger.Clear();
            this.Ledger.AccountGroupId = BLL.DataKeyValue.SundryCreditors;

            DOB = DateTime.Now;
            DOJ = DateTime.Now;
            NotifyAllPropertyChanged();
        }

        public bool Find(int pk)
        {
            var d = toList.Where(x => x.Id == pk).FirstOrDefault();
            if (d != null)
            {
                d.toCopy<Staff>(this);
                IsReadOnly = !UserPermission.AllowUpdate;

                return true;
            }

            return false;
        }

        public bool Delete(bool isServerCall = false)
        {
            var rv = false;
            var d = toList.Where(x => x.Id == Id).FirstOrDefault();
            var b = FMCGHubClient.FMCGHub.Invoke<bool>("Ledger_CanDeleteById", this.LedgerId).Result;
            if (d != null && b == true)
            {

                if (isServerCall == false)
                {
                    rv = FMCGHubClient.FMCGHub.Invoke<bool>("Staff_Delete", this.Id).Result;
                    if (rv == true)
                    {
                        toList.Remove(d);
                        var l1 = Ledger.toList.Where(x => x.Id == d.LedgerId).FirstOrDefault();
                        Ledger.toList.Remove(l1);
                    }

                }
                else
                {
                    toList.Remove(d);
                    var l1 = Ledger.toList.Where(x => x.Id == d.LedgerId).FirstOrDefault();
                    Ledger.toList.Remove(l1);
                }
                return rv;
            }

            return rv;
        }

        public bool isValid()
        {
            bool RValue = true;
            if (!Ledger.isValid())
            {
                RValue = false;
            }
            return RValue;

        }

        public static void Init()
        {
            _toList = null;
        }

        private void SetAccountName()
        {
            try
            {
                Ledger.AccountName = string.Format("{0}{1}{2}{3}{4}", Ledger.AccountGroup.GroupCode, string.IsNullOrWhiteSpace(Ledger.AccountGroup.GroupCode) ? "" : "-", Ledger.LedgerCode, string.IsNullOrWhiteSpace(Ledger.LedgerCode) ? "" : "-", Ledger.LedgerName);
            }
            catch (Exception ex)
            {

            }
        }


        #endregion
    }
}
