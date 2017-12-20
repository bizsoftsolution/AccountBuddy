using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountBuddy.SL.Hubs
{
    public static class DataMapper
    {
        public static DAL.CompanyDetail ToMap(this BLL.CompanyDetail S, DAL.CompanyDetail D)
        {
            D.Id = S.Id;
            D.AddressLine1 = S.AddressLine1;
            D.AddressLine2 = S.AddressLine2;
            D.CityName = S.CityName;
            D.CompanyName = S.CompanyName;
            D.CompanyType = S.CompanyType;
            D.EMailId = S.EMailId;
            D.GSTNo = S.GSTNo;
            D.IsActive = S.IsActive;
            D.Logo = S.Logo;
            D.MobileNo = S.MobileNo;            
            D.PostalCode = S.PostalCode;
            D.TelephoneNo = S.TelephoneNo;
            D.UnderCompanyId = S.UnderCompanyId;

            return D;
        }

        public static BLL.CompanyDetail ToMap(this DAL.CompanyDetail S, BLL.CompanyDetail D)
        {
            D.Id = S.Id;
            D.AddressLine1 = S.AddressLine1;
            D.AddressLine2 = S.AddressLine2;
            D.CityName = S.CityName;
            D.CompanyName = S.CompanyName;
            D.CompanyType = S.CompanyType;
            D.EMailId = S.EMailId;
            D.GSTNo = S.GSTNo;
            D.IsActive = S.IsActive;
            D.Logo = S.Logo;
            D.MobileNo = S.MobileNo;
            D.PostalCode = S.PostalCode;
            D.TelephoneNo = S.TelephoneNo;
            D.UnderCompanyId = S.UnderCompanyId;

            return D;
        }

        public static BLL.UserType ToMap(this DAL.UserType S, BLL.UserType D)
        {
            D.CompanyId = S.CompanyId;
            D.Description = S.Description;
            D.Id = S.Id;
            D.TypeOfUser = S.TypeOfUser;
            return D;
        }

        public static DAL.UserType ToMap(this BLL.UserType S, DAL.UserType D)
        {
            D.CompanyId = S.CompanyId;
            D.Description = S.Description;
            D.Id = S.Id;
            D.TypeOfUser = S.TypeOfUser;
            return D;
        }

        public static BLL.UserTypeDetail ToMap(this DAL.UserTypeDetail S, BLL.UserTypeDetail D)
        {
            D.AllowDelete = S.AllowDelete;
            D.AllowInsert = S.AllowInsert;
            D.AllowUpdate = S.AllowUpdate;
            D.Id = S.Id;
            D.IsViewForm = S.IsViewForm;
            D.UserTypeFormDetailId = S.UserTypeFormDetailId;
            D.UserTypeId = S.UserTypeId;

            return D;
        }

        public static DAL.UserTypeDetail ToMap(this BLL.UserTypeDetail S, DAL.UserTypeDetail D)
        {
            D.AllowDelete = S.AllowDelete;
            D.AllowInsert = S.AllowInsert;
            D.AllowUpdate = S.AllowUpdate;
            D.Id = S.Id;
            D.IsViewForm = S.IsViewForm;
            D.UserTypeFormDetailId = S.UserTypeFormDetailId;
            D.UserTypeId = S.UserTypeId;

            return D;
        }

        public static BLL.UserTypeFormDetail ToMap(this DAL.UserTypeFormDetail S, BLL.UserTypeFormDetail D)
        {
            D.Description = S.Description;
            D.FormName = S.FormName;
            D.FormType = S.FormType;
            D.Id = S.Id;
            D.IsActive = S.IsActive;
            D.IsDelete = S.IsDelete;
            D.IsInsert = S.IsInsert;
            D.IsMenu = S.IsMenu;
            D.IsUpdate = S.IsUpdate;
            return D;
        }

        public static DAL.UserTypeFormDetail ToMap(this BLL.UserTypeFormDetail S, DAL.UserTypeFormDetail D)
        {
            D.Description = S.Description;
            D.FormName = S.FormName;
            D.FormType = S.FormType;
            D.Id = S.Id;
            D.IsActive = S.IsActive;
            D.IsDelete = S.IsDelete;
            D.IsInsert = S.IsInsert;
            D.IsMenu = S.IsMenu;
            D.IsUpdate = S.IsUpdate;
            return D;
        }

        public static BLL.UserAccount ToMap(this DAL.UserAccount S, BLL.UserAccount D)
        {
            D.Id = S.Id;
            D.LoginId = S.LoginId;
            D.Password = S.Password;
            D.UserName = S.UserName;
            D.UserTypeId = D.UserTypeId;

            return D;
        }

        public static DAL.UserAccount ToMap(this BLL.UserAccount S, DAL.UserAccount D)
        {
            D.Id = S.Id;
            D.LoginId = S.LoginId;
            D.Password = S.Password;
            D.UserName = S.UserName;
            D.UserTypeId = D.UserTypeId;

            return D;
        }
    }
}