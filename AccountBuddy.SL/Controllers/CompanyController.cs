using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AccountBuddy.SL.Controllers
{
    public class CompanyController : Controller
    {
        DAL.DBFMCGEntities DB = new DAL.DBFMCGEntities();
        // GET: Company
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
                                          Logo = x.Logo,
                                          CompanyType = x.CompanyType,
                                          UnderCompanyId = x.UnderCompanyId.Value


                                      })
                                      .ToList();

            return Json(l1, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Login(string CompanyName,String LoginId, String Password)
        {
            int UId = 0;
            DAL.UserAccount ua = DB.UserAccounts
                                   .Where(x => x.UserType.CompanyDetail.CompanyName == CompanyName
                                                && x.LoginId == LoginId
                                                && x.Password == Password)
                                   .FirstOrDefault();
            if (ua != null) UId = ua.Id;
            return Json(new {UserId=UId }, JsonRequestBehavior.AllowGet);
        }
    }
}