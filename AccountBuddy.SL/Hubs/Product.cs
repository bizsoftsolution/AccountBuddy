using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AccountBuddy.Common;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {

        #region Products

        public BLL.Product Product_DALtoBLL(DAL.Product ProductsFrom)
        {
            BLL.Product ProductsTo = ProductsFrom.toCopy<BLL.Product>(new BLL.Product());
            try
            {            
                ProductsTo.StockGroup = StockGroup_DALtoBLL(ProductsFrom.StockGroup);
                var pd = ProductsFrom.ProductDetails.Where(x => x.CompanyId == Caller.CompanyId).FirstOrDefault();
                if (pd == null) pd = new DAL.ProductDetail();

                ProductsTo.UOM = ProductsFrom.UOM == null ? null : UOM_DALtoBLL(ProductsFrom.UOM);
                ProductsTo.OpeningStock = pd.OpeningStock;
                ProductsTo.ReOrderLevel = pd.ReorderLevel;
                ProductsTo.POQty = ProductsFrom.PurchaseOrderDetails.Where(x => x.PurchaseOrder.Ledger.AccountGroup.CompanyId == Caller.CompanyId).Sum(x => x.Quantity);
                ProductsTo.PQty = ProductsFrom.PurchaseDetails.Where(x => x.Purchase.Ledger.AccountGroup.CompanyId == Caller.CompanyId).Sum(x => x.Quantity);
                ProductsTo.PRQty = ProductsFrom.PurchaseReturnDetails.Where(x => x.PurchaseReturn.Ledger.AccountGroup.CompanyId == Caller.CompanyId).Sum(x => x.Quantity);
                ProductsTo.SOQty = ProductsFrom.SalesOrderDetails.Where(x => x.SalesOrder.Ledger.AccountGroup.CompanyId == Caller.CompanyId).Sum(x => x.Quantity);
                ProductsTo.SQty = ProductsFrom.SalesDetails.Where(x => x.Sale.Ledger.AccountGroup.CompanyId == Caller.CompanyId).Sum(x => x.Quantity);
                ProductsTo.SRQty = ProductsFrom.SalesReturnDetails.Where(x => x.SalesReturn.Ledger.AccountGroup.CompanyId == Caller.CompanyId).Sum(x => x.Quantity);
                ProductsTo.SInQty = ProductsFrom.StockInDetails.Where(x => x.StockIn.Ledger.AccountGroup.CompanyId == Caller.CompanyId).Sum(x => x.Quantity);
                ProductsTo.SOutQty = ProductsFrom.StockOutDetails.Where(x => x.StockOut.Ledger.AccountGroup.CompanyId == Caller.CompanyId).Sum(x => x.Quantity);
                ProductsTo.JOQty = ProductsFrom.JobOrderIssueDetails.Where(x => x.JobOrderIssue.JobWorker.Ledger.AccountGroup.CompanyId == Caller.CompanyId).Sum(x => x.Quantity);
                ProductsTo.JRQty = ProductsFrom.JobOrderReceivedDetails.Where(x => x.JobOrderReceived.JobWorker.Ledger.AccountGroup.CompanyId == Caller.CompanyId).Sum(x => x.Quantity);
                ProductsTo.SSQty = ProductsFrom.StockSeperatedDetails.Where(x => x.StockSeparated.Staff.Ledger.AccountGroup.CompanyId == Caller.CompanyId).Sum(x => x.Quantity);
                ProductsTo.SPQty = ProductsFrom.StockInProcessDetails.Where(x => x.StockInProcess.Staff.Ledger.AccountGroup.CompanyId == Caller.CompanyId).Sum(x => x.Quantity);

                ProductsTo.SRQtyForSales = ProductsFrom.SalesReturnDetails.Where(x => x.SalesReturn.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.IsResale == true).Sum(x => x.Quantity);
                ProductsTo.SRQtyNotForSales = ProductsFrom.SalesReturnDetails.Where(x => x.SalesReturn.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.IsResale == false).Sum(x => x.Quantity);

                return ProductsTo;
            }
            catch(Exception ex)
            { }
            return ProductsTo;
        }

        public List<BLL.Product> Product_List()
        {

            if (Caller.CompanyType == "Company")
            {
                return DB.Products.Where(x => x.StockGroup.CompanyDetail.Id == Caller.CompanyId).ToList()
                             .Select(x => Product_DALtoBLL(x)).ToList();
            }
            else
            {
                return DB.Products.Where(x => x.StockGroup.CompanyDetail.Id == Caller.UnderCompanyId).ToList()
                           .Select(x => Product_DALtoBLL(x)).ToList();

            }
        }

        public BLL.Product Product_Save(BLL.Product pro)
        {
            try
            {

                DAL.Product d = DB.Products.Where(x => x.Id == pro.Id).FirstOrDefault();
                if (d == null)
                {
                    d = new DAL.Product();
                    DB.Products.Add(d);

                    pro.toCopy<DAL.Product>(d);

                    DAL.ProductDetail pd = new DAL.ProductDetail();

                    d.ProductDetails.Add(pd);
                    pd.CompanyId = Caller.CompanyId;
                    pd.OpeningStock = pro.OpeningStock;
                    pd.ReorderLevel = pro.ReOrderLevel;


                    DB.SaveChanges();
                    pro.Id = d.Id;
                    LogDetailStore(pro, LogDetailType.INSERT);
                }
                else
                {
                    pro.toCopy<DAL.Product>(d);
                    //Ledger_Save(pro.Ledger);
                    var pd = d.ProductDetails.Where(x => x.CompanyId == Caller.CompanyId).FirstOrDefault();
                    if (pd == null)
                    {
                        pd = new DAL.ProductDetail();
                        d.ProductDetails.Add(pd);
                        pd.CompanyId = Caller.CompanyId;
                    }
                    pd.OpeningStock = pro.OpeningStock;
                    pd.ReorderLevel = pro.ReOrderLevel;

                    DB.SaveChanges();
                }
                if (d.Id != 0)
                {
                    var p = Product_DALtoBLL(d);
                    
                    return p;
                }
                Clients.Clients(OtherLoginClientsOnGroup).Product_Save(Product_DALtoBLL(d));

            }
            catch (Exception ex)
            {
                WriteErrorLog("Product", "Product_Save", BLL.UserAccount.User.Id, Caller.CompanyId, ex.Message);
            }
            return new BLL.Product();
        }

        public bool Product_Delete(int pk)
        {
            var rv = false;
            try
            {
                var d = DB.Products.Where(x => x.Id == pk).FirstOrDefault();
                if (d != null)
                {
                    var p = Product_DALtoBLL(d);
                    DB.ProductDetails.RemoveRange(d.ProductDetails);
                    DB.Products.Remove(d);
                    //Ledger_Delete((int)d.LedgerId);
                    DB.SaveChanges();
                    LogDetailStore(p, LogDetailType.DELETE);
                }

                Clients.Clients(OtherLoginClientsOnGroup).Customer_Delete(pk);

                rv = true;

            }
            catch (Exception ex)
            {
                rv = false;
            }
            return rv;
        }

        public bool Product_CanDelete(DAL.Product p)
        {
            bool rv = (p == null) ? false : p.PurchaseOrderDetails.Count() == 0 &&
                   p.PurchaseDetails.Count() == 0 &&
                   p.PurchaseReturnDetails.Count() == 0 &&
                   p.SalesOrderDetails.Count() == 0 &&
                   p.SalesDetails.Count() == 0 &&
                   p.SalesReturnDetails.Count() == 0;

            return rv;
        }

        public bool Product_CanDeleteById(int Id)
        {
            return Product_CanDelete(DB.Products.Where(x => x.Id == Id).FirstOrDefault());
        }

        #endregion
    }
}