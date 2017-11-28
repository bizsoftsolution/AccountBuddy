using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        public List<BLL.GeneralStock> GeneralStock_List(int? CompanyId, int ProductId, DateTime dtFrom, DateTime dtTo)
        {
            List<BLL.GeneralStock> lstGeneralStock = new List<BLL.GeneralStock>();


            BLL.GeneralStock gl = new BLL.GeneralStock();

            var lstProduct = Caller.DB.Products.Where(x => x.StockGroup.CompanyDetail.Id == Caller.CompanyId && x.Id == ProductId).ToList();


            #region Ledger

            decimal opqty, sqty, prqty, srqty, pqty, BalQty, StIn, StOut, JOQty, JRQty,SSQty, SPQty;
            BalQty = 0;
            foreach (var P in lstProduct)
            {
                gl = new BLL.GeneralStock();

                gl.Product = Product_DALtoBLL(P);

                opqty = (decimal)P.ProductDetails.FirstOrDefault().OpeningStock;

                pqty = (decimal)P.PurchaseDetails.Where(x => (CompanyId == null || x.Purchase.Ledger.AccountGroup.CompanyId == CompanyId) && x.Purchase.PurchaseDate < dtFrom).Sum(x => x.Quantity);
                srqty = (decimal)P.SalesReturnDetails.Where(x => (CompanyId == null || x.SalesReturn.Ledger.AccountGroup.CompanyId == CompanyId) && x.SalesReturn.SRDate < dtFrom).Sum(x => x.Quantity);
                sqty = (decimal)P.SalesDetails.Where(x => (CompanyId == null || x.Sale.Ledger.AccountGroup.CompanyId == CompanyId) && x.Sale.SalesDate < dtFrom).Sum(x => x.Quantity);
                prqty = (decimal)P.PurchaseReturnDetails.Where(x => (CompanyId == null || x.PurchaseReturn.Ledger.AccountGroup.CompanyId == CompanyId) && x.PurchaseReturn.PRDate < dtFrom).Sum(x => x.Quantity);
                StIn = (decimal)P.StockInDetails.Where(x => (CompanyId == null || x.StockIn.Ledger.AccountGroup.CompanyId == CompanyId) && x.StockIn.Date < dtFrom).Sum(x => x.Quantity);
                StOut = (decimal)P.StockOutDetails.Where(x => (CompanyId == null || x.StockOut.Ledger.AccountGroup.CompanyId == CompanyId) && x.StockOut.Date < dtFrom).Sum(x => x.Quantity);
                JOQty = (decimal)P.JobOrderIssueDetails.Where(x => (CompanyId == null || x.JobOrderIssue.JobWorker.Ledger.AccountGroup.CompanyId == CompanyId) && x.JobOrderIssue.JODate < dtFrom).Sum(x => x.Quantity);
                JRQty = (decimal)P.JobOrderReceivedDetails.Where(x => (CompanyId == null || x.JobOrderReceived.JobWorker.Ledger.AccountGroup.CompanyId == CompanyId) && x.JobOrderReceived.JRDate < dtFrom).Sum(x => x.Quantity);
               SPQty = (decimal)P.StockInProcessDetails.Where(x => (CompanyId == null || x.StockInProcess.Staff.Ledger.AccountGroup.CompanyId == CompanyId) && x.StockInProcess.SPDate < dtFrom).Sum(x => x.Quantity);
                SSQty = (decimal)P.StockSeperatedDetails.Where(x => (CompanyId == null || x.StockSeparated.Staff.Ledger.AccountGroup.CompanyId == CompanyId) && x.StockSeparated.Date < dtFrom).Sum(x => x.Quantity);

                gl.Inwards = pqty + srqty + StIn+JRQty;
                gl.Outwards = sqty + prqty + StOut+JOQty;


                BalQty += ((opqty + gl.Inwards) - gl.Outwards);

                gl.BalStock = Math.Abs(BalQty);

                gl.Ledger = new BLL.Ledger();
                gl.Ledger.LedgerName = string.Format("Balance {0}", P.ProductName);
                lstGeneralStock.Add(gl);

                foreach (var pd in P.PurchaseDetails.Where(x => (CompanyId == null || x.Purchase.Ledger.AccountGroup.CompanyId == CompanyId) && x.Purchase.PurchaseDate >= dtFrom && x.Purchase.PurchaseDate <= dtTo).ToList())
                {
                    gl = new BLL.GeneralStock();
                    gl.Product = new BLL.Product();
                    gl.Product = Product_DALtoBLL(pd.Product);

                    gl.EId = pd.Purchase.Id;
                    gl.EType = "P";
                    gl.TType = string.Format("{0} - Purchase", pd.Purchase.TransactionType.Type);
                    gl.EDate = pd.Purchase.PurchaseDate;
                    gl.Ledger = new BLL.Ledger();
                    gl.Ledger = LedgerDAL_BLL(pd.Purchase.Ledger);
                    gl.EntryNo = pd.Purchase.RefNo;
                    gl.Inwards = (decimal)pd.Quantity;
                    gl.Outwards = 0;
                    BalQty += (gl.Inwards - gl.Outwards);
                    gl.BalStock = Math.Abs(BalQty);
                    lstGeneralStock.Add(gl);
                }




                foreach (var s in P.SalesDetails.Where(x => (CompanyId == null || x.Sale.Ledger.AccountGroup.CompanyId == CompanyId) && x.Sale.SalesDate >= dtFrom && x.Sale.SalesDate <= dtTo).ToList())
                {

                    gl = new BLL.GeneralStock();
                    gl.Ledger = new BLL.Ledger();
                    gl.Ledger = LedgerDAL_BLL(s.Sale.Ledger);
                    gl.EId = s.Id;
                    gl.EType = "S";
                    gl.EDate = s.Sale.SalesDate;
                    gl.TType = string.Format("{0} - Sales", s.Sale.TransactionType.Type);
                    gl.EntryNo = s.Sale.RefNo;
                    gl.Outwards = (decimal)s.Quantity;
                    gl.Inwards = 0;
                    BalQty += (gl.Inwards - gl.Outwards);
                    gl.BalStock = Math.Abs(BalQty);
                    lstGeneralStock.Add(gl);


                }
                foreach (var pr in P.PurchaseReturnDetails.Where(x => (CompanyId == null || x.PurchaseReturn.Ledger.AccountGroup.CompanyId == CompanyId) && x.PurchaseReturn.PRDate >= dtFrom && x.PurchaseReturn.PRDate <= dtTo).ToList())
                {
                    gl = new BLL.GeneralStock();
                    gl.Ledger = new BLL.Ledger();
                    gl.Ledger = LedgerDAL_BLL(pr.PurchaseReturn.Ledger);

                    gl.EId = pr.PurchaseReturn.Id;
                    gl.EType = "PR";
                    gl.EDate = pr.PurchaseReturn.PRDate;
                    gl.TType = string.Format("{0} - Purchase Return", pr.PurchaseReturn.TransactionType.Type);
                    gl.EntryNo = pr.PurchaseReturn.RefNo;
                    gl.Inwards = 0;
                    gl.Outwards = (decimal)pr.Quantity;
                    BalQty += (gl.Inwards - gl.Outwards);
                    gl.BalStock = Math.Abs(BalQty);
                    lstGeneralStock.Add(gl);
                }

                foreach (var sr in P.SalesReturnDetails.Where(x => (CompanyId == null || x.SalesReturn.Ledger.AccountGroup.CompanyId == CompanyId) && x.SalesReturn.SRDate >= dtFrom && x.SalesReturn.SRDate <= dtTo).ToList())
                {
                    gl = new BLL.GeneralStock();
                    gl.Ledger = new BLL.Ledger();

                    gl.Ledger = LedgerDAL_BLL(sr.SalesReturn.Ledger);

                    gl.EId = sr.SalesReturn.Id;
                    gl.EType = "SR";
                    gl.EDate = sr.SalesReturn.SRDate;

                    gl.EntryNo = sr.SalesReturn.RefNo;
                    gl.TType = string.Format("{0} - Sales Return", sr.SalesReturn.TransactionType.Type);
                    gl.Inwards = (decimal)sr.Quantity;
                    gl.Outwards = 0;
                    BalQty += (gl.Inwards - gl.Outwards);
                    gl.BalStock = Math.Abs(BalQty);
                    lstGeneralStock.Add(gl);
                }
                foreach (var sIn in P.StockInDetails.Where(x => (CompanyId == null || x.StockIn.Ledger.AccountGroup.CompanyId == CompanyId) && x.StockIn.Date >= dtFrom && x.StockIn.Date <= dtTo).ToList())
                {
                    gl = new BLL.GeneralStock();
                    gl.Ledger = new BLL.Ledger();

                    gl.Ledger = LedgerDAL_BLL(sIn.StockIn.Ledger);

                    gl.EId = sIn.StockIn.Id;
                    gl.EType = "SIn";
                    gl.EDate = sIn.StockIn.Date;

                    gl.EntryNo = sIn.StockIn.RefNo;
                    gl.TType = string.Format("{0} - Stock Inwards", sIn.StockIn.Type);
                    gl.Inwards = (decimal)sIn.Quantity;
                    gl.Outwards = 0;
                    BalQty += (gl.Inwards - gl.Outwards);
                    gl.BalStock = Math.Abs(BalQty);
                    lstGeneralStock.Add(gl);
                }
                foreach (var sIn in P.StockOutDetails.Where(x => (CompanyId == null || x.StockOut.Ledger.AccountGroup.CompanyId == CompanyId) && x.StockOut.Date >= dtFrom && x.StockOut.Date <= dtTo).ToList())
                {
                    gl = new BLL.GeneralStock();
                    gl.Ledger = new BLL.Ledger();

                    gl.Ledger = LedgerDAL_BLL(sIn.StockOut.Ledger);

                    gl.EId = sIn.StockOut.Id;
                    gl.EType = "SOut";
                    gl.EDate = sIn.StockOut.Date;

                    gl.EntryNo = sIn.StockOut.RefNo;
                    gl.TType = string.Format("{0} - Stock Outwards", sIn.StockOut.Type);
                    gl.Inwards = 0;
                    gl.Outwards = (decimal)sIn.Quantity;
                    BalQty += (gl.Inwards - gl.Outwards);
                    gl.BalStock = Math.Abs(BalQty);
                    lstGeneralStock.Add(gl);
                }
                foreach (var JO in P.JobOrderIssueDetails.Where(x => (CompanyId == null || x.JobOrderIssue.JobWorker.Ledger.AccountGroup.CompanyId == CompanyId) && x.JobOrderIssue.JODate >= dtFrom && x.JobOrderIssue.JODate <= dtTo).ToList())
                {
                    gl = new BLL.GeneralStock();
                    gl.Ledger = new BLL.Ledger();

                    gl.Ledger = LedgerDAL_BLL(JO.JobOrderIssue.JobWorker.Ledger);

                    gl.EId = JO.JobOrderIssue.Id;
                    gl.EType = "JO";
                    gl.EDate = JO.JobOrderIssue.JODate;

                    gl.EntryNo = JO.JobOrderIssue.RefNo;
                    gl.TType = string.Format("{0} - Job Order","Issued");
                    gl.Inwards = 0;
                    gl.Outwards = (decimal)JO.Quantity;
                    BalQty += (gl.Inwards - gl.Outwards);
                    gl.BalStock = Math.Abs(BalQty);
                    lstGeneralStock.Add(gl);
                }
                foreach (var JR in P.JobOrderReceivedDetails.Where(x => (CompanyId == null || x.JobOrderReceived.JobWorker.Ledger.AccountGroup.CompanyId == CompanyId) && x.JobOrderReceived.JRDate >= dtFrom && x.JobOrderReceived.JRDate <= dtTo).ToList())
                {
                    gl = new BLL.GeneralStock();
                    gl.Ledger = new BLL.Ledger();

                    gl.Ledger = LedgerDAL_BLL(JR.JobOrderReceived.JobWorker.Ledger);

                    gl.EId = JR.JobOrderReceived.Id;
                    gl.EType = "JR";
                    gl.EDate = JR.JobOrderReceived.JRDate;

                    gl.EntryNo = JR.JobOrderReceived.RefNo;
                    gl.TType = string.Format("{0} - Job Order", "Received");
                    gl.Inwards = (decimal)JR.Quantity;
                    gl.Outwards = 0;
                    BalQty += (gl.Inwards - gl.Outwards);
                    gl.BalStock = Math.Abs(BalQty);
                    lstGeneralStock.Add(gl);
                }
                foreach (var SP in P.StockInProcessDetails.Where(x => (CompanyId == null || x.StockInProcess.Staff.Ledger.AccountGroup.CompanyId == CompanyId) && x.StockInProcess.SPDate >= dtFrom && x.StockInProcess.SPDate <= dtTo).ToList())
                {
                    gl = new BLL.GeneralStock();
                    gl.Ledger = new BLL.Ledger();

                    gl.Ledger = LedgerDAL_BLL(SP.StockInProcess.Staff.Ledger);

                    gl.EId = SP.StockInProcess.Id;
                    gl.EType = "SP";
                    gl.EDate = SP.StockInProcess.SPDate;

                    gl.EntryNo = SP.StockInProcess.RefNo;
                    gl.TType = string.Format("Stock In Process");
                    gl.Inwards = 0;
                    gl.Outwards = (decimal)SP.Quantity;
                    BalQty += (gl.Inwards - gl.Outwards);
                    gl.BalStock = Math.Abs(BalQty);
                    lstGeneralStock.Add(gl);
                }
                foreach (var SS in P.StockSeperatedDetails.Where(x => (CompanyId == null || x.StockSeparated.Staff.Ledger.AccountGroup.CompanyId == CompanyId) && x.StockSeparated.Date >= dtFrom && x.StockSeparated.Date <= dtTo).ToList())
                {
                    gl = new BLL.GeneralStock();
                    gl.Ledger = new BLL.Ledger();

                    gl.Ledger = LedgerDAL_BLL(SS.StockSeparated.Staff.Ledger);

                    gl.EId = SS.StockSeparated.Id;
                    gl.EType = "SS";
                    gl.EDate = SS.StockSeparated.Date;

                    gl.EntryNo = SS.StockSeparated.RefNo;
                    gl.TType = string.Format("Stock Separated");
                    gl.Inwards = (decimal)SS.Quantity;
                    gl.Outwards =0 ;
                    BalQty += (gl.Inwards - gl.Outwards);
                    gl.BalStock = Math.Abs(BalQty);
                    lstGeneralStock.Add(gl);
                }
                gl = new BLL.GeneralStock();
                gl.Ledger = new BLL.Ledger();
                gl.Ledger.LedgerName = "Total";
                gl.Inwards = lstGeneralStock.Sum(x => x.Inwards);
                gl.Outwards = lstGeneralStock.Sum(x => x.Outwards);
                gl.BalStock = Math.Abs(BalQty);
                lstGeneralStock.Add(gl);

            }
            #endregion


            return lstGeneralStock;
        }

    }
}