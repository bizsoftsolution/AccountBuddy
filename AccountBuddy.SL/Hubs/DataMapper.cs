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
    }
}