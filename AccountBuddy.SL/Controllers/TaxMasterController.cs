using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AccountBuddy.SL.Controllers
{
    public class TaxMasterController : Controller
    {
        // GET: TaxMaster
        AccountBuddy.DAL.DBFMCGEntities DB = new DAL.DBFMCGEntities();
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult toList(int DealerId)
        {

            var l1 = DB.TaxMasters.Where(x => x.Ledger.AccountGroup.CompanyId == DealerId)
                                       .ToList();

            BLL.TaxMaster led = new BLL.TaxMaster();
            List<BLL.TaxMaster> TaxList = new List<BLL.TaxMaster>();
            foreach (var x in l1)
            {
                led = new BLL.TaxMaster();
                led.Id = x.Id;
                led.LedgerId = x.LedgerId;
                led.TaxPercentage = x.TaxPercentage;
                led.TaxName = x.Ledger.LedgerName;
                TaxList.Add(led);
            }

            return Json(TaxList, JsonRequestBehavior.AllowGet);

        }

    }
}