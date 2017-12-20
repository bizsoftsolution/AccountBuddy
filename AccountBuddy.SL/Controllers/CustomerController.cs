using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AccountBuddy.BLL;

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
                                       .ToList();

            BLL.Ledger led = new BLL.Ledger();
            List<BLL.Ledger> CustomerList = new List<BLL.Ledger>();
            foreach (var x in l1)
            {
                led = new BLL.Ledger();
                led.Id = x.LedgerId;
                led.LedgerName = x.Ledger.LedgerName;
                led.AddressLine1 = x.Ledger.AddressLine1;
                led.AddressLine2 = x.Ledger.AddressLine2;
                led.CityName = x.Ledger.CityName;
                led.MobileNo = x.Ledger.MobileNo;
                led.TelephoneNo = x.Ledger.TelephoneNo;
                led.CreditLimitTypeName = x.Ledger.CreditLimitType == null ? null : x.Ledger.CreditLimitType.LimitType;
                led.CreditLimit = x.Ledger.CreditLimit == null ? (short)0 : x.Ledger.CreditLimit.Value;
                led.CreditAmount = x.Ledger.CreditAmount == null ? 0 : x.Ledger.CreditAmount.Value;
                led.EMailId = x.Ledger.EMailId;
                led.PersonIncharge = x.Ledger.PersonIncharge;
                led.GSTNo = x.Ledger.GSTNo;
                led.OPBal = AccountBuddy.SL.Hubs.ABServerHub.GetLedgerBalance(x.Ledger);
                CustomerList.Add(led);
            }

            return Json(CustomerList, JsonRequestBehavior.AllowGet);

        }

        public JsonResult Save(int DealerId, String LedgerName, String PersonIncharge, String AddressLine1, String AddressLine2, String CityName, String MobileNo, String GSTNo)
        {
            try
            {
                int AGId = DB.DataKeyValues.Where(x => x.CompanyId == DealerId && x.DataKey == BLL.DataKeyValue.SundryDebtors_Key).FirstOrDefault().DataValue;
                DAL.Customer cus = new DAL.Customer();
                DAL.Ledger led = new DAL.Ledger()
                {
                    LedgerName = LedgerName,
                    PersonIncharge = PersonIncharge,
                    AddressLine1 = AddressLine1,
                    AddressLine2 = AddressLine2,
                    CityName = CityName,
                    MobileNo = MobileNo,
                    AccountGroupId = AGId,
                    GSTNo = GSTNo
                };
                DB.Ledgers.Add(led);
                DB.SaveChanges();
                cus.LedgerId = led.Id;
                DB.Customers.Add(cus);
                DB.SaveChanges();

                return Json(new { Id = cus.LedgerId, HasError = false }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { Id = 0, HasError = true, ErrMsg = ex.Message }, JsonRequestBehavior.AllowGet);
            }


        }
    }
}