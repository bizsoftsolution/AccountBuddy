using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AccountBuddy.SL.Controllers
{
    public class CompanyController : Controller
    {
        // GET: Company
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult toList()
        {
            DAL.DBFMCGEntities DB = new DAL.DBFMCGEntities();
            var l1 = DB.CompanyDetails.Where(x=> x.IsActive)
                                      .Select(x=> new BLL.CompanyDetail()
                                      {
                                          Id=x.Id,
                                          CompanyName = x.CompanyName,
                                          AddressLine1 = x.AddressLine1
                                      })
                                      .ToList();

            return Json(l1, JsonRequestBehavior.AllowGet);
        }
    }
}