using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AccountBuddy.SL.Controllers
{
    public class SaleOrderController : Controller
    {
        // GET: SaleOrder
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult Save(int LedgerId,string RefCode, string SaleOrderDetails, bool IsGST, decimal DiscountAmount)
        {
            try
            {
                DAL.DBFMCGEntities DB = new DAL.DBFMCGEntities();

                dynamic l1 = JsonConvert.DeserializeObject(SaleOrderDetails);
                DAL.SalesOrder sal = new DAL.SalesOrder();
                foreach (dynamic d1 in l1)
                {
                    DAL.SalesOrderDetail sd = new DAL.SalesOrderDetail()
                    {
                        ProductId = d1.ProductId,
                        Quantity = d1.Qty,
                        UnitPrice = d1.Rate,
                        Amount = d1.Amount,
                        UOMId = d1.UOMId,
                     DiscountAmount = d1.DiscountAmount
                    };
                    sal.SalesOrderDetails.Add(sd);
                }
                sal.LedgerId = LedgerId;
                sal.SODate = DateTime.Now;
                sal.ItemAmount = sal.SalesOrderDetails.Sum(x => x.Amount);
                sal.DiscountAmount = DiscountAmount;
                sal.RefCode = RefCode;
                if (IsGST == true)
                {
                    sal.GSTAmount = sal.ItemAmount * 6 / 100;
                    sal.TotalAmount = sal.ItemAmount + sal.GSTAmount-DiscountAmount;
                }
                else
                {
                    sal.GSTAmount = 0;
                    sal.TotalAmount = sal.ItemAmount - DiscountAmount; ;
                }
                Hubs.ABServerHub ab = new Hubs.ABServerHub();
                sal.RefNo = ab.SalesOrder_NewRefNoByCompanyId(DB.Ledgers.Where(x => x.Id == LedgerId).FirstOrDefault().AccountGroup.CompanyId);
                
                DB.SalesOrders.Add(sal);
                DB.SaveChanges();
                return Json(new { Id = sal.Id, HasError = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Id = 0, HasError = true, ErrMsg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}