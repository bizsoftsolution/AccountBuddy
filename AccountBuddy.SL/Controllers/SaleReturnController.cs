using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AccountBuddy.SL.Controllers
{
    public class SaleReturnController : Controller
    {
        // GET: SaleReturn
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Save(int LedgerId,string PayMode, string SaleReturnDetails, bool IsGST)
        {
            try
            {
                DAL.DBFMCGEntities db = new DAL.DBFMCGEntities();

                dynamic l1 = JsonConvert.DeserializeObject(SaleReturnDetails);
                DAL.SalesReturn sal = new DAL.SalesReturn();
                foreach (dynamic d1 in l1)
                {
                    DAL.SalesReturnDetail sd = new DAL.SalesReturnDetail()
                    {
                        ProductId = d1.ProductId,
                        Quantity = d1.Qty,
                        UnitPrice = d1.Rate,
                        Amount = d1.Amount,
                        UOMId = d1.UOMId
                    };
                    sal.SalesReturnDetails.Add(sd);
                }
                sal.LedgerId = LedgerId;
                sal.SRDate = DateTime.Now;
                sal.ItemAmount = sal.SalesReturnDetails.Sum(x => x.Amount.Value);
                if (IsGST == true)
                {
                    sal.GSTAmount = sal.ItemAmount * 6 / 100;
                    sal.TotalAmount = sal.ItemAmount + sal.GSTAmount;
                }
                else
                {
                    sal.GSTAmount = 0;
                    sal.TotalAmount = sal.ItemAmount;
                }
                sal.TransactionTypeId = 1;
                sal.Narration = PayMode;
                sal.RefNo = Hubs.ABServerHub.Sales_NewRefNoByCompanyId(db.Ledgers.Where(x => x.Id == LedgerId).FirstOrDefault().AccountGroup.CompanyId);

                db.SalesReturns.Add(sal);
                db.SaveChanges();

                Hubs.ABServerHub.Journal_SaveBySalesReturn(sal);
                return Json(new { Id = sal.Id, HasError = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Id = 0, HasError = true, ErrMsg = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

    }
}