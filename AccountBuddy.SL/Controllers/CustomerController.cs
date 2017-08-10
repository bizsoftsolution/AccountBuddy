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
                                         Id = x.LedgerId,
                                         LedgerName  = x.Ledger.LedgerName,
                                         AddressLine1 = x.Ledger.AddressLine1,
                                         AddressLine2 = x.Ledger.AddressLine2,
                                         CityName = x.Ledger.CityName,
                                         MobileNo = x.Ledger.MobileNo,
                                         TelephoneNo = x.Ledger.TelephoneNo,
                                         CreditLimitTypeName = x.Ledger.CreditLimitType.LimitType,
                                         CreditLimit = x.Ledger.CreditLimit==null?(short)0:x.Ledger.CreditLimit.Value,
                                         CreditAmount = x.Ledger.CreditAmount==null?0:x.Ledger.CreditAmount.Value,
                                         EMailId = x.Ledger.EMailId,
                                         PersonIncharge = x.Ledger.PersonIncharge,
                                         GSTNo =x.Ledger.GSTNo

                                      })
                                      .ToList();

            return Json(l1, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Save(int DealerId,String LedgerName,String PersonIncharge, String AddressLine1, String AddressLine2, String CityName, String MobileNo,String GSTNo)
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
            catch(Exception ex)
            {
                return Json(new { Id = 0, HasError = true,ErrMsg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            
            
        }
    }
}