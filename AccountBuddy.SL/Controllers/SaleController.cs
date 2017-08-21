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
                sal.GSTAmount = sal.ItemAmount * 6 /100;
                sal.TotalAmount = sal.ItemAmount + sal.GSTAmount;
                sal.TransactionTypeId = 1;
                sal.RefNo = Hubs.ABServerHub.Sales_NewRefNoByCompanyId( db.Ledgers.Where(x => x.Id == LedgerId).FirstOrDefault().AccountGroup.CompanyId);
                
                db.Sales.Add(sal);
                db.SaveChanges();

                Hubs.ABServerHub.Journal_SaveBySales(sal);
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
               
                var lst1 = from s in db.Sales
                          join sd in db.SalesDetails on s.Id equals sd.SalesId
                          where s.Ledger.AccountGroup.CompanyId == DealerId && s.SalesDate>= DateFrom && s.SalesDate <= DateTo
                          select new { sd.Product.ProductName, sd.Amount };
                var lst2 = lst1.GroupBy(x => x.ProductName).Select(x => new { ProductName = x.Key, Amount = x.Sum(y => y.Amount) }).ToList();

                
                return Json(new { HasError = false, Datas= lst2}, JsonRequestBehavior.AllowGet);
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

               var lst1 = from s in db.Sales                           
                           where s.Ledger.AccountGroup.CompanyId == DealerId && s.SalesDate >= DateFrom && s.SalesDate <= DateTo
                           select new { s.Ledger.LedgerName, s.TotalAmount};
                var lst2 = lst1.GroupBy(x => x.LedgerName).Select(x => new { LedgerName = x.Key, Amount = x.Sum(y => y.TotalAmount) }).ToList();


                return Json(new { HasError = false, Datas = lst2 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { HasError = true, ErrMsg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}