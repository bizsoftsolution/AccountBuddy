using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AccountBuddy.SL.Controllers
{
    public class ProductController : Controller
    {
        DAL.DBFMCGEntities DB = new DAL.DBFMCGEntities();
        // GET: Product
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult toList(int CompanyId)
        {

            var l1 = DB.Products.Where(x => x.StockGroup.CompanyId == CompanyId)
                                      .Select(x => new BLL.Product()
                                      {
                                          Id = x.Id,
                                          ProductName = x.ProductName,
                                          ItemCode = x.ItemCode,
                                          PurchaseRate = x.PurchaseRate,
                                          SellingRate = x.SellingRate,
                                          MRP = x.MRP,
                                          StockGroupId = x.StockGroupId,
                                          UOMId = x.UOMId,
                                          UOMName = x.UOM.Symbol                                                             
                                      })
                                      .ToList();

            return Json(l1, JsonRequestBehavior.AllowGet);
        }
    }
}