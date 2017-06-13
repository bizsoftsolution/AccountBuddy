using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AccountBuddy.SL.Controllers
{
    public class CustomerController : Controller
    {
        DAL.DBFMCGEntities DB = new DAL.DBFMCGEntities();
        // GET: Customer
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult toList(int CompanyId)
        {

            var l1 = DB.Customers.Where(x => x.Ledger.AccountGroup.CompanyId == CompanyId)
                                      .Select(x => new BLL.Ledger()
                                      {
                                         Id = x.Id,
                                         LedgerName  = x.Ledger.LedgerName,
                                         AddressLine1 = x.Ledger.AddressLine1,
                                         AddressLine2 = x.Ledger.AddressLine2,
                                         CityName = x.Ledger.CityName,
                                         MobileNo = x.Ledger.MobileNo,
                                         TelephoneNo = x.Ledger.TelephoneNo,
                                         CreditLimitTypeName = x.Ledger.CreditLimitType.LimitType,
                                         CreditLimit = x.Ledger.CreditLimit.Value,
                                         CreditAmount = x.Ledger.CreditAmount.Value,
                                         EMailId = x.Ledger.EMailId,
                                         PersonIncharge = x.Ledger.PersonIncharge
                                      })
                                      .ToList();

            return Json(l1, JsonRequestBehavior.AllowGet);
        }
    }
}