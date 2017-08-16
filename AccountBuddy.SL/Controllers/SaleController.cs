using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AccountBuddy.SL.Controllers
{
    public class SaleController : Controller
    {
        // GET: Sale
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult Save(int LedgerId, string SaleDetails)
        {
            try
            {
                DAL.DBFMCGEntities db = new DAL.DBFMCGEntities();

                dynamic l1 = JsonConvert.DeserializeObject(SaleDetails);
                DAL.Sale sal = new DAL.Sale();
                foreach (dynamic d1 in l1)
                {
                    DAL.SalesDetail sd = new DAL.SalesDetail()
                    {
                        ProductId = d1.ProductId,
                        Quantity = d1.Qty,
                        UnitPrice = d1.Rate,
                        Amount = d1.Amount,
                        UOMId = d1.UOMId
                    };
                    sal.SalesDetails.Add(sd);
                }
                sal.LedgerId = LedgerId;
                sal.SalesDate = DateTime.Now;
                sal.ItemAmount = sal.SalesDetails.Sum(x => x.Amount);
                sal.TransactionTypeId = 1;
                sal.RefNo = Hubs.ABServerHub.Sales_NewRefNoByCompanyId( db.Ledgers.Where(x => x.Id == LedgerId).FirstOrDefault().AccountGroup.CompanyId);
                
                db.Sales.Add(sal);
                db.SaveChanges();
                return Json(new { Id = sal.Id, HasError = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Id = 0, HasError = true, ErrMsg = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult ProductReport(int DealerId,DateTime DateFrom,DateTime DateTo)
        {
            try
            {
                DAL.DBFMCGEntities db = new DAL.DBFMCGEntities();                

                var l1 = db.Products.Select(x => new { ProductName = x.ProductName, Amount = x.SalesDetails.Where(y => y.Sale.SalesDate >= DateFrom && y.Sale.SalesDate <= DateTo && y.Sale.Ledger.AccountGroup.CompanyId == DealerId).Sum(y => y.Amount) }).ToList();

                return Json(new { HasError = false, Datas=l1}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { HasError = true, ErrMsg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult CustomerReport(int DealerId, DateTime DateFrom, DateTime DateTo)
        {
            try
            {
                DAL.DBFMCGEntities db = new DAL.DBFMCGEntities();

                var l1 = db.Customers.Where(x=> x.Ledger.AccountGroup.CompanyId==DealerId).Select(x => new { CustomerName = x.Ledger.LedgerName, Amount = x.Ledger.Sales.Where(y => y.SalesDate >= DateFrom && y.SalesDate <= DateTo ).Sum(y => y.TotalAmount) }).ToList();

                return Json(new { HasError = false, Datas = l1 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { HasError = true, ErrMsg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}