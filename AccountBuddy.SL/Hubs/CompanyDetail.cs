using AccountBuddy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        #region CompanyDetail

        public static List<BLL.CompanyDetail> _listCompany;
        public static List<BLL.CompanyDetail> ListCompany
        {
            get
            {
                if (_listCompany == null)
                {
                    _listCompany = DB.CompanyDetails.Select(x => new BLL.CompanyDetail()
                    {
                        Id = x.Id,
                        CompanyName = x.CompanyName,
                        AddressLine1 = x.AddressLine1,
                        AddressLine2 = x.AddressLine2,
                        CityName = x.CityName,
                        EMailId = x.EMailId,
                        GSTNo = x.GSTNo,
                        Logo = x.Logo,
                        MobileNo = x.MobileNo,
                        PostalCode = x.PostalCode,
                        TelephoneNo = x.TelephoneNo                        
                    }
                    ).ToList();
                }
                return _listCompany;
            }
        }
        
        public List<string> CompanyDetail_AcYearList()
        {
            List<string> AcYearList = new List<string>();

            DateTime d =  DateTime.Now;
            int YearFrom = d.Month < 4 ? d.Year - 1 : d.Year;
            int YearTo = d.Month < 4 ? d.Year : d.Year + 1;

            if (DB.Payments.Count() > 0)
            {
                d = DB.Payments.Min(x => x.PaymentDate);
                int yy = YearFrom = d.Month < 4 ? d.Year - 1 : d.Year;
                YearFrom = yy < YearFrom ? yy : YearFrom;
            }

            if (DB.Receipts.Count() > 0)
            {
                d = DB.Receipts.Min(x => x.ReceiptDate);
                int yy = YearFrom = d.Month < 4 ? d.Year - 1 : d.Year;
                YearFrom = yy < YearFrom ? yy : YearFrom;
            }

            if (DB.Journals.Count() > 0)
            {
                d = DB.Journals.Min(x => x.JournalDate);
                int yy = YearFrom = d.Month < 4 ? d.Year - 1 : d.Year;
                YearFrom = yy < YearFrom ? yy : YearFrom;
            }

            for(int n = YearFrom; n < YearTo; n++)
            {
                AcYearList.Add(string.Format("{0} - {1}",n,n+1));
            }
            
            return AcYearList;
        }



        public List<BLL.CompanyDetail> CompanyDetail_List()
        {
            return ListCompany;
        }

        public int CompanyDetail_Save(BLL.CompanyDetail sgp)
        {
            try
            {

                BLL.CompanyDetail b = ListCompany.Where(x => x.Id == sgp.Id).FirstOrDefault();
                DAL.CompanyDetail d = DB.CompanyDetails.Where(x => x.Id == sgp.Id).FirstOrDefault();

                if (d == null)
                {

                    b = new BLL.CompanyDetail();
                    ListCompany.Add(b);

                    d = new DAL.CompanyDetail();
                    DB.CompanyDetails.Add(d);

                    sgp.toCopy<DAL.CompanyDetail>(d);

                    DAL.UserAccount ua = new DAL.UserAccount();
                    ua.LoginId = sgp.UserId;
                    ua.UserName = sgp.UserId;
                    ua.Password = sgp.Password;
                    ua.UserTypeId = 1;
                    d.UserAccounts.Add(ua);

                    DB.SaveChanges();
                    d.toCopy<BLL.CompanyDetail>(b);
                    sgp.Id = d.Id;                                       

                    // LogDetailStore(sgp, LogDetailType.INSERT);
                }
                else
                {
                    sgp.toCopy<BLL.CompanyDetail>(b);
                    sgp.toCopy<DAL.CompanyDetail>(d);
                    DB.SaveChanges();
                    // LogDetailStore(sgp, LogDetailType.UPDATE);
                }

                Clients.Others.CompanyDetail_Save(sgp);

                return sgp.Id;
            }
            catch (Exception ex) { }
            return 0;
        }

        #endregion
    }
}
