using AccountBuddy.Common;
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
        public BLL.Product Product_DALtoBLL(DAL.Product ProductsFrom, int CompanyId)
        {
            BLL.Product ProductsTo = ProductsFrom.toCopy<BLL.Product>(new BLL.Product());
            var pd = ProductsFrom.ProductDetails.Where(x => x.CompanyId == CompanyId).FirstOrDefault();
            if (pd == null) pd = new DAL.ProductDetail();
            ProductsTo.OpeningStock = pd.OpeningStock;
            ProductsTo.ReOrderLevel = pd.ReorderLevel;
           
            ProductsTo.POQty = ProductsFrom.PurchaseOrderDetails.Where(x => x.PurchaseOrder.Ledger.AccountGroup.CompanyId == CompanyId).Sum(x => x.Quantity);
            ProductsTo.PQty = ProductsFrom.PurchaseDetails.Where(x => x.Purchase.Ledger.AccountGroup.CompanyId == CompanyId).Sum(x => x.Quantity);
            ProductsTo.PRQty = ProductsFrom.PurchaseReturnDetails.Where(x => x.PurchaseReturn.Ledger.AccountGroup.CompanyId == CompanyId).Sum(x => x.Quantity);
            ProductsTo.SOQty = ProductsFrom.SalesOrderDetails.Where(x => x.SalesOrder.Ledger.AccountGroup.CompanyId == CompanyId).Sum(x => x.Quantity);
            ProductsTo.SQty = ProductsFrom.SalesDetails.Where(x => x.Sale.Ledger.AccountGroup.CompanyId == CompanyId).Sum(x => x.Quantity);
            ProductsTo.SRQty = ProductsFrom.SalesReturnDetails.Where(x => x.SalesReturn.Ledger.AccountGroup.CompanyId == CompanyId).Sum(x => x.Quantity);
            ProductsTo.SInQty = ProductsFrom.StockInDetails.Where(x => x.StockIn.Ledger.AccountGroup.CompanyId == CompanyId).Sum(x => x.Quantity);
            ProductsTo.SOutQty = ProductsFrom.StockOutDetails.Where(x => x.StockOut.Ledger.AccountGroup.CompanyId == CompanyId).Sum(x => x.Quantity);
            ProductsTo.SRQtyForSales = ProductsFrom.SalesReturnDetails.Where(x => x.SalesReturn.Ledger.AccountGroup.CompanyId == CompanyId && x.IsResale == true).Sum(x => x.Quantity);
            ProductsTo.SRQtyNotForSales = ProductsFrom.SalesReturnDetails.Where(x => x.SalesReturn.Ledger.AccountGroup.CompanyId == CompanyId && x.IsResale == false).Sum(x => x.Quantity);

            return ProductsTo;
        }
        public JsonResult toList(int DealerId)
        {
            var c = DB.CompanyDetails.Where(x => x.Id == DealerId).FirstOrDefault();
            if (c != null)
            {
                int CompanyId = (int)c.UnderCompanyId;
                var l1 = DB.Products.Where(x => x.StockGroup.CompanyId == CompanyId)
                                    .ToList()
                                    .ToList().Select(x => Product_DALtoBLL(x, c.Id))
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
                                        OpeningStock = x.StockLeftForSales,
                                        
                                        SRQtyForSales = x.SRQtyForSales,
                                        SRQtyNotForSales = x.SRQtyNotForSales,
                                    })
                                    .ToList();
                return Json(l1, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { HasError = true }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}