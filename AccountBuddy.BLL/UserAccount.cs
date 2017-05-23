using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountBuddy.Common;
using System.Collections.ObjectModel;

namespace AccountBuddy.BLL
{
    public class UserAccount : INotifyPropertyChanged
    {
        #region Field

        public static UserAccount User = new UserAccount();
        public static CompanyDetail Company = new CompanyDetail();
        public static UserType Type = new UserType();
        public static ObservableCollection<UserTypeDetail> TypeDetails = new ObservableCollection<UserTypeDetail>();

        private static ObservableCollection<UserAccount> _toList;
        public List<BLL.Validation> lstValidation = new List<BLL.Validation>();

        private int _id;
        private int _companyId;
        private int _userTypeId;
        private string _userName;
        private string _loginId;
        private string _password;
        private string _UserTypeName;
        #endregion

        #region Property

        public static ObservableCollection<UserAccount> toList
        {
            get
            {
                if (_toList == null)
                {
                    var l1 = ABClientHub.FMCGHub.Invoke<List<UserAccount>>("UserAccount_List").Result;
                    _toList = new ObservableCollection<UserAccount>(l1);
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
        public int CompanyId
        {
            get
            {
                return _companyId;
            }
            set
            {
                if (_companyId != value)
                {
                    _companyId = value;
                    NotifyPropertyChanged(nameof(CompanyId));
                }
            }
        }
        public int UserTypeId
        {
            get
            {
                return _userTypeId;
            }
            set
            {
                if (_userTypeId != value)
                {
                    _userTypeId = value;
                    NotifyPropertyChanged(nameof(UserTypeId));
                }
            }
        }
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                if (_userName != value)
                {
                    _userName = value;
                    NotifyPropertyChanged(nameof(UserName));
                }
            }
        }
        public string LoginId
        {
            get
            {
                return _loginId;
            }
            set
            {
                if (_loginId != value)
                {
                    _loginId = value;
                    NotifyPropertyChanged(nameof(LoginId));
                }
            }
        }
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                if (_password != value)
                {
                    _password = value;
                    NotifyPropertyChanged(nameof(Password));
                }
            }
        }

        public string UserTypeName
        {
            get
            {
                return _UserTypeName;
            }
            set
            {
                if (_UserTypeName != value)
                {
                    _UserTypeName = value;
                    NotifyPropertyChanged(nameof(UserTypeName));
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

        #region Method
        public static string Login(string AccYear, String CompanyName, String LId, String Pwd)
        {
            var ua = ABClientHub.FMCGHub.Invoke<UserAccount>("UserAccount_Login", AccYear, CompanyName, LId, Pwd).Result;
            
            if ( isValidLogin(ua,AccYear,CompanyName, LId,Pwd))
            {
                try
                {
                    DateTime d = DateTime.Now;
                    int yy = d.Month < 4 ? d.Year - 1 : d.Year;
                    int.TryParse(AccYear.Substring(0, 4), out yy);

                    User = ua;
                    Company = CompanyDetail.toList.Where(x => x.Id == ua.CompanyId).FirstOrDefault();
                    Company.LoginAccYear = yy;
                    Type = UserType.toList.Where(x => x.Id == ua.UserTypeId).FirstOrDefault();
                    TypeDetails = new ObservableCollection<UserTypeDetail>(UserTypeDetail.ToList.Where(x => x.UserTypeId == ua.UserTypeId).ToList());
                    Data_Init();
                    return "";
                }
                catch(Exception ex)
                { }
              
               
            }
            return string.Join("\n", ua.lstValidation.Select(x => x.Message));
        }

        static void Data_Init()
        {
            BLL.UserAccount.Init();
            BLL.AccountGroup.Init();
            BLL.Ledger.Init();
        }

        public static bool AllowFormShow(string FormName)
        {
            bool rv = true;
            var t = TypeDetails.Where(x => x.FormName == FormName).FirstOrDefault();
            if (t != null) rv = t.IsViewForm;
            return rv;
        }

        public static bool AllowInsert(string FormName)
        {
            bool rv = true;
            var t = TypeDetails.Where(x => x.FormName == FormName).FirstOrDefault();
            if (t != null) rv = t.AllowInsert;
            return rv;
        }

        public static bool AllowUpdate(string FormName)
        {
            bool rv = true;
            var t = TypeDetails.Where(x => x.FormName == FormName).FirstOrDefault();
            if (t != null) rv = t.AllowUpdate;
            return rv;
        }

        public static bool AllowDelete(string FormName)
        {
            bool rv = true;
            var t = TypeDetails.Where(x => x.FormName == FormName).FirstOrDefault();
            if (t != null) rv = t.AllowDelete;
            return rv;
        }
        
        public bool Save(bool isServerCall = false)
        {
            if (!isValid()) return false;
            try
            {

                UserAccount d = toList.Where(x => x.Id == Id).FirstOrDefault();

                if (d == null)
                {
                    d = new UserAccount();
                    toList.Add(d);
                }

                this.toCopy<UserAccount>(d);
                if (isServerCall == false)
                {

                    var i = ABClientHub.FMCGHub.Invoke<int>("UserAccount_Save", this).Result;
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
            new UserAccount().toCopy<UserAccount>(this);
            NotifyAllPropertyChanged();
        }

        public bool Find(int pk)
        {
            var d = toList.Where(x => x.Id == pk).FirstOrDefault();
            if (d != null)
            {
                d.toCopy<UserAccount>(this);
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
                if (isServerCall == false) ABClientHub.FMCGHub.Invoke<int>("UserAccount_Delete", this.Id);
                return true;
            }

            return false;
        }

        public bool isValid()
        {
            bool RValue = true;
            lstValidation.Clear();

            if (string.IsNullOrWhiteSpace(UserName))
            {
                lstValidation.Add(new Validation() { Name = nameof(UserName), Message = string.Format(Message.BLL.Required_Data, nameof(UserName)) });
                RValue = false;
            }

            if (string.IsNullOrWhiteSpace(LoginId))
            {
                lstValidation.Add(new Validation() { Name = nameof(LoginId), Message = string.Format(Message.BLL.Required_Data, nameof(LoginId)) });
                RValue = false;
            }
            else if (toList.Where(x => x.LoginId.ToLower() == LoginId.ToLower() && x.Id != Id).Count() > 0)
            {
                lstValidation.Add(new Validation() { Name = nameof(LoginId), Message = string.Format(Message.BLL.Existing_Data, LoginId) });
                RValue = false;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                lstValidation.Add(new Validation() { Name = nameof(Password), Message = string.Format(Message.BLL.Required_Data, nameof(Password)) });
                RValue = false;
            }
            if (string.IsNullOrWhiteSpace(UserTypeName))
            {
                lstValidation.Add(new Validation() { Name = nameof(UserTypeName), Message = string.Format(Message.BLL.Required_Data, nameof(UserTypeName)) });
                RValue = false;
            }


            return RValue;

        }

        public static bool isValidLogin(UserAccount ua,string AccYear,string CompanyName,string LId, string pwd)
        {
            bool RValue = true;
            ua.lstValidation.Clear();

            if (string.IsNullOrWhiteSpace(AccYear))
            {
                ua.lstValidation.Add(new Validation() { Name = nameof(AccYear), Message = "Please Select the Account Year" });
                RValue = false;
            }

            if (string.IsNullOrWhiteSpace(CompanyName))
            {
                ua.lstValidation.Add(new Validation() { Name = nameof(CompanyName), Message = "Please Select the Company" });
                RValue = false;
            }

            if (string.IsNullOrWhiteSpace(LId))
            {
                ua.lstValidation.Add(new Validation() { Name = nameof(LoginId), Message = "Please Enter the Login Id" });
                RValue = false;
            }

            if (string.IsNullOrWhiteSpace(pwd))
            {
                ua.lstValidation.Add(new Validation() { Name = nameof(Password), Message = "Please Enter the Password" });
                RValue = false;
            }
            if (RValue == true)
            {
                if (ua.LoginId != LId || ua.Password!=pwd)
                {
                    ua.lstValidation.Add(new Validation() { Name = nameof(LoginId), Message = "Please Enter the Valid User Id or Password" });
                    RValue = false;
                }
            }

            return RValue;

        }


        public static void Init()
        {
            _toList = null;
        }
        #endregion
    }
}
