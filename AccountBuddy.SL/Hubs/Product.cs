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
            BLL.Product ProductsTo = ProductsFrom.ToMap(new BLL.Product());
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
                ProductsTo.PSpec = ProductsFrom.Product_Spec_Process.Where(x => x.Product.StockGroup.CompanyId == Caller.CompanyId).Sum(x => x.Qty);
                ProductsTo.PSpecP = ProductsFrom.Product_Spec_Process_Detail.Where(x => x.Product.StockGroup.CompanyId == Caller.CompanyId).Sum(x => x.Qty);
                ProductsTo.SRQtyForSales = ProductsFrom.SalesReturnDetails.Where(x => x.SalesReturn.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.IsResale == true).Sum(x => x.Quantity);
                ProductsTo.SRQtyNotForSales = ProductsFrom.SalesReturnDetails.Where(x => x.SalesReturn.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.IsResale == false).Sum(x => x.Quantity);

                return ProductsTo;
            }
            catch (Exception ex)
            { Common.AppLib.WriteLog(ex); }
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
            DAL.Product d = DB.Products.Where(x => x.Id == pro.Id).FirstOrDefault();
            try
            {

                if (d == null)
                {
                    d = new DAL.Product();
                    DB.Products.Add(d);

                    pro.ToMap(d);

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
                    pro.ToMap(d);
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
                if (OtherClientsOnGroup.Count > 0) Clients.Clients(OtherClientsOnGroup).Product_Save(Product_DALtoBLL(d));

                if (d.Id != 0)
                {
                    var p = Product_DALtoBLL(d);

                    return p;
                }

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

                if (OtherClientsOnGroup.Count > 0) Clients.Clients(OtherClientsOnGroup).Customer_Delete(pk);

                rv = true;

            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex);
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

        public double StockBalance(DAL.Product p)
        {

            return StockBalance(p, DateTime.Now);
        }

        public double StockBalance(DAL.Product p, DateTime dt)
        {

            var OPT = p.ProductDetails.Where(x => x.CompanyId == Caller.CompanyId).FirstOrDefault();
            var OP = OPT == null ? 0 : OPT.OpeningStock;

            #region Inward

            var Pur = p.PurchaseDetails.Where(x => x.Purchase.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.Purchase.PurchaseDate < dt).Sum(x => x.Quantity);
            var SR = p.SalesReturnDetails.Where(x => x.SalesReturn.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.SalesReturn.SRDate < dt).Sum(x => x.Quantity);
            var SIn = p.StockInDetails.Where(x => x.StockIn.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.StockIn.Date < dt).Sum(x => x.Quantity);
            var PSD = p.Product_Spec_Process_Detail.Where(x => x.Product.ProductDetails.FirstOrDefault().CompanyId == Caller.CompanyId && x.Product_Spec_Process.Date < dt).Sum(x => x.Qty);
            var JoR = p.JobOrderReceivedDetails.Where(x => x.JobOrderReceived.JobWorker.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.JobOrderReceived.JRDate < dt).Sum(x => x.Quantity);
            var SInPro = p.StockInProcessDetails.Where(x => x.StockInProcess.Staff.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.StockInProcess.SPDate < dt).Sum(x => x.Quantity);
           var SRQtyForSales = p.SalesReturnDetails.Where(x => x.SalesReturn.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.IsResale == true && x.SalesReturn.SRDate < dt).Sum(x => x.Quantity);
           
            #endregion

            #region Outward
            var Sal = p.SalesDetails.Where(x => x.Sale.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.Sale.SalesDate < dt).Sum(x => x.Quantity);
            var PR = p.PurchaseReturnDetails.Where(x => x.PurchaseReturn.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.PurchaseReturn.PRDate < dt).Sum(x => x.Quantity);
            var SOut = p.StockOutDetails.Where(x => x.StockOut.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.StockOut.Date < dt).Sum(x => x.Quantity);
            var PSP = p.Product_Spec_Process.Where(x => x.Product.ProductDetails.FirstOrDefault().CompanyId == Caller.CompanyId && x.Date < dt).Sum(x => x.Qty);
            var JOI = p.JobOrderIssueDetails.Where(x => x.JobOrderIssue.JobWorker.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.JobOrderIssue.JODate < dt).Sum(x => x.Quantity);
            var SOutPro = p.StockSeperatedDetails.Where(x => x.StockSeparated.Staff.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.StockSeparated.Date < dt).Sum(x => x.Quantity);
            var  SRQtyNotForSales = p.SalesReturnDetails.Where(x => x.SalesReturn.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.IsResale == false && x.SalesReturn.SRDate < dt).Sum(x => x.Quantity);

            #endregion
            var avl = OP + (Pur + SR + SIn + PSD + JoR + SInPro + SRQtyForSales) - (Sal + PR + SOut + PSP + JOI + SOutPro + SRQtyNotForSales);

            return avl;
        }

        public List<BLL.StockReport> StockReport_List(int? PID,DateTime dtFrom, DateTime dtTo)
        {
            List<BLL.StockReport> rv = new List<BLL.StockReport>();

            var l1 = DB.Products.Where(x => x.StockGroup.CompanyId == (Caller.CompanyType == "Company" ? Caller.CompanyId : Caller.UnderCompanyId)&& (PID==null||x.Id==PID)).ToList();

            foreach (var p in l1)
            {
                var op = StockBalance(p, dtFrom);

                #region Inward

                var Pur = p.PurchaseDetails.Where(x => x.Purchase.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.Purchase.PurchaseDate >= dtFrom && x.Purchase.PurchaseDate <= dtTo).Sum(x => x.Quantity);
                var SR = p.SalesReturnDetails.Where(x => x.SalesReturn.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.SalesReturn.SRDate >= dtFrom && x.SalesReturn.SRDate <= dtTo).Sum(x => x.Quantity);
                var SIn = p.StockInDetails.Where(x => x.StockIn.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.StockIn.Date >= dtFrom && x.StockIn.Date <= dtTo).Sum(x => x.Quantity);
                var PSD = p.Product_Spec_Process_Detail.Where(x => x.Product.ProductDetails.FirstOrDefault().CompanyId == Caller.CompanyId && x.Product_Spec_Process.Date >= dtFrom && x.Product_Spec_Process.Date <= dtTo).Sum(x => x.Qty);
                var JoR = p.JobOrderReceivedDetails.Where(x => x.JobOrderReceived.JobWorker.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.JobOrderReceived.JRDate >= dtFrom && x.JobOrderReceived.JRDate <= dtTo).Sum(x => x.Quantity);
                var SInPro = p.StockInProcessDetails.Where(x => x.StockInProcess.Staff.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.StockInProcess.SPDate >= dtFrom && x.StockInProcess.SPDate <= dtTo).Sum(x => x.Quantity);
                var SRQtyForSales = p.SalesReturnDetails.Where(x => x.SalesReturn.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.IsResale == true && x.SalesReturn.SRDate > dtFrom&&x.SalesReturn.SRDate<=dtTo).Sum(x => x.Quantity);

                #endregion

                #region Outward
                var Sal = p.SalesDetails.Where(x => x.Sale.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.Sale.SalesDate >= dtFrom && x.Sale.SalesDate <= dtTo).Sum(x => x.Quantity);
                var PR = p.PurchaseReturnDetails.Where(x => x.PurchaseReturn.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.PurchaseReturn.PRDate >= dtFrom && x.PurchaseReturn.PRDate <= dtTo).Sum(x => x.Quantity);
                var SOut = p.StockOutDetails.Where(x => x.StockOut.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.StockOut.Date >= dtFrom && x.StockOut.Date <= dtTo).Sum(x => x.Quantity);
                var PSP = p.Product_Spec_Process.Where(x => x.Product.ProductDetails.FirstOrDefault().CompanyId == Caller.CompanyId && x.Date >= dtFrom && x.Date <= dtTo).Sum(x => x.Qty);
                var JOI = p.JobOrderIssueDetails.Where(x => x.JobOrderIssue.JobWorker.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.JobOrderIssue.JODate >= dtFrom && x.JobOrderIssue.JODate <= dtTo).Sum(x => x.Quantity);
                var SOutPro = p.StockSeperatedDetails.Where(x => x.StockSeparated.Staff.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.StockSeparated.Date >= dtFrom && x.StockSeparated.Date <= dtTo).Sum(x => x.Quantity);
                var SRQtyNotForSales = p.SalesReturnDetails.Where(x => x.SalesReturn.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.IsResale == false && x.SalesReturn.SRDate > dtFrom && x.SalesReturn.SRDate <= dtTo).Sum(x => x.Quantity);

                #endregion

                var avl = op + (Pur + SR + SIn + PSD + JoR + SInPro+SRQtyForSales) - (Sal + PR + SOut + PSP + JOI + SOutPro+SRQtyNotForSales);

                if (op != 0)
                {
                    rv.Add(new BLL.StockReport()
                    {
                        ProductName = p.ProductName,
                        TransactionType = "01-Opening",
                        Qty = op
                    });
                }

                if (Pur != 0)
                {
                    rv.Add(new BLL.StockReport()
                    {
                        ProductName = p.ProductName,
                        TransactionType = "02-Purchase",
                        Qty = Pur
                    });
                }
                if (Sal != 0)
                {
                    rv.Add(new BLL.StockReport()
                    {
                        ProductName = p.ProductName,
                        TransactionType = "03-Sales",
                        Qty = Sal
                    });
                }
                if (PR != 0)
                {
                    rv.Add(new BLL.StockReport()
                    {
                        ProductName = p.ProductName,
                        TransactionType = "04-Purchase Return",
                        Qty = SR
                    });
                }
                if (SR != 0)
                {
                    rv.Add(new BLL.StockReport()
                    {
                        ProductName = p.ProductName,
                        TransactionType = "05-Sales Return",
                        Qty = SR
                    });
                }
                if (SRQtyForSales != 0)
                {
                    rv.Add(new BLL.StockReport()
                    {
                        ProductName = p.ProductName,
                        TransactionType = "06-Sales Return(For Sales)",
                        Qty = SRQtyForSales
                    });
                }
                if (SRQtyNotForSales != 0)
                {
                    rv.Add(new BLL.StockReport()
                    {
                        ProductName = p.ProductName,
                        TransactionType = "07-Sales Return(Not For Sales)",
                        Qty = SRQtyNotForSales
                    });
                }
                if (SIn != 0)
                {
                    rv.Add(new BLL.StockReport()
                    {
                        ProductName = p.ProductName,
                        TransactionType = "08-Stock Inwards",
                        Qty = SIn
                    });
                }
                if (SOut != 0)
                {
                    rv.Add(new BLL.StockReport()
                    {
                        ProductName = p.ProductName,
                        TransactionType = "09-Stock Outwards",
                        Qty = SOut
                    });
                }
                if (JoR != 0)
                {
                    rv.Add(new BLL.StockReport()
                    {
                        ProductName = p.ProductName,
                        TransactionType = "10-Job Order Received",
                        Qty = JOI
                    });
                }
                if (JOI != 0)
                {
                    rv.Add(new BLL.StockReport()
                    {
                        ProductName = p.ProductName,
                        TransactionType = "11-Job Order Issued",
                        Qty = JOI
                    });
                }
                if (SInPro != 0)
                {
                    rv.Add(new BLL.StockReport()
                    {
                        ProductName = p.ProductName,
                        TransactionType = "12-Stock In Process",
                        Qty = SInPro
                    });
                }
                if (SOutPro != 0)
                {
                    rv.Add(new BLL.StockReport()
                    {
                        ProductName = p.ProductName,
                        TransactionType = "13-Stock Out Process",
                        Qty = SOutPro
                    });
                }

                if (avl != 0)
                {
                    rv.Add(new BLL.StockReport()
                    {
                        ProductName = p.ProductName,
                        TransactionType = "14-Available",
                        Qty = avl
                    });
                }

            }

            return rv;
        }

        #endregion
    }
}