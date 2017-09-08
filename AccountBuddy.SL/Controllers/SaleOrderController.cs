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
        public JsonResult Save(int LedgerId, string SaleOrderDetails, bool IsGST)
        {
            try
            {
                DAL.DBFMCGEntities db = new DAL.DBFMCGEntities();

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
                        UOMId = d1.UOMId
                    };
                    sal.SalesOrderDetails.Add(sd);
                }
                sal.LedgerId = LedgerId;
                sal.SODate = DateTime.Now;
                sal.ItemAmount = sal.SalesOrderDetails.Sum(x => x.Amount);
                if(IsGST==true)
                {
                    sal.GSTAmount = sal.ItemAmount * 6 / 100;
                    sal.TotalAmount = sal.ItemAmount + sal.GSTAmount;
                }
                else
                {
                    sal.GSTAmount = 0;
                        sal.TotalAmount = sal.ItemAmount;
                }
                sal.RefNo = Hubs.ABServerHub.SalesOrder_NewRefNoByCompanyId(db.Ledgers.Where(x => x.Id == LedgerId).FirstOrDefault().AccountGroup.CompanyId);
                
                db.SalesOrders.Add(sal);
                db.SaveChanges();
                return Json(new { Id = sal.Id, HasError = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Id = 0, HasError = true, ErrMsg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}