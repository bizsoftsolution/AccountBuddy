using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AccountBuddy.SL.Controllers
{
    public  class CompanyController : Controller
    {
        DAL.DBFMCGEntities DB = new DAL.DBFMCGEntities();
        public static string SalRefCode = "", SRRefCode = "", SORefCode = "";
        public static string SalPrefix = "", SRPrefix = "", SOPrefix = "";
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult toList()
        {
            var l1 = DB.CompanyDetails.Where(x => x.IsActive)
                                      .Select(x => new BLL.CompanyDetail()
                                      {
                                          Id = x.Id,
                                          CompanyName = x.CompanyName,
                                          AddressLine1 = x.AddressLine1,
                                          AddressLine2 = x.AddressLine2,
                                          CityName = x.CityName,
                                          PostalCode = x.PostalCode,
                                          EMailId = x.EMailId,
                                          MobileNo = x.MobileNo,
                                          TelephoneNo = x.TelephoneNo,
                                          GSTNo = x.GSTNo,
                                          CompanyType = x.CompanyType,
                                          UnderCompanyId = x.UnderCompanyId.Value }).ToList();

            var data = Json(l1, JsonRequestBehavior.AllowGet);
            data.MaxJsonLength = int.MaxValue;
            return data;
        }

        public JsonResult Login(string CompanyName, String LoginId, String Password)
        {
            int UId = 0;
            DateTime dt = DateTime.Now;
            DAL.UserAccount ua = DB.UserAccounts
                                   .Where(x => x.UserType.CompanyDetail.CompanyName == CompanyName
                                                && x.LoginId == LoginId
                                                && x.Password == Password)
                                   .FirstOrDefault();
            if (ua != null)
            {
                UId = ua.Id;
                SalRefCode = DB.Sales.Where(x => x.SalesDate.Month == DateTime.Now.Month && x.Ledger.AccountGroup.CompanyId == ua.UserType.CompanyId).Max(x => x.RefNo.Substring(x.RefNo.Length - 5, x.RefNo.Length));
                SRRefCode = DB.SalesReturns.Where(x => x.SRDate.Month == DateTime.Now.Month && x.Ledger.AccountGroup.CompanyId == ua.UserType.CompanyId).Max(x => x.RefNo.Substring(x.RefNo.Length - 5, x.RefNo.Length));
                SORefCode = DB.SalesOrders.Where(x => x.SODate.Month == DateTime.Now.Month && x.Ledger.AccountGroup.CompanyId == ua.UserType.CompanyId).Max(x => x.RefNo.Substring(x.RefNo.Length - 5, x.RefNo.Length));

                SalPrefix =  string.Format("{0}{1:yy}{2:X}", BLL.FormPrefix.Sales, dt, dt.Month);
                SRPrefix = string.Format("{0}{1:yy}{2:X}", BLL.FormPrefix.SalesReturn, dt, dt.Month);
                SOPrefix = string.Format("{0}{1:yy}{2:X}", BLL.FormPrefix.SalesOrder, dt, dt.Month);
            }
            return Json(new { UserId = UId, SalRefCode = SalRefCode, SRRefCode = SRRefCode, SORefCode = SORefCode,
                                            SalPrefix =SalPrefix , SRPrefix=SRPrefix, SOPrefix=SOPrefix}, JsonRequestBehavior.AllowGet);
        }
        public static void WriteLogM(String str)
        {
            using (StreamWriter writer = new StreamWriter(Path.GetTempPath() + "FMCG_Mobile_log.txt", true))
            {
                writer.WriteLine(string.Format("{0:dd/MM/yyyy hh:mm:ss} => {1}", DateTime.Now, str));
            }
        }
    }
}