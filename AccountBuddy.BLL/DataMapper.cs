using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBuddy.BLL
{
    public static class DataMapper
    {
        public static CompanyDetail ToMap(this CompanyDetail S,CompanyDetail D)
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
            D.IsEnabled = S.IsEnabled;
            D.IsReadOnly = S.IsReadOnly;
            D.LoginAccYear = S.LoginAccYear;
            D.Logo = S.Logo;
            D.MobileNo = S.MobileNo;
            D.Password = S.Password;
            D.PostalCode = S.PostalCode;
            D.TelephoneNo = S.TelephoneNo;
            D.UnderCompanyId = S.UnderCompanyId;
            D.UserId = S.UserId;           

            return D;   
        }
    }
}
