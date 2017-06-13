using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        public List<BLL.GeneralStock> GeneralStock_List(int ProductId, DateTime dtFrom, DateTime dtTo)
        {
            List<BLL.GeneralStock> lstGeneralStock = new List<BLL.GeneralStock>();


            BLL.GeneralStock gl = new BLL.GeneralStock();

            var lstProduct = DB.Products.Where(x => x.StockGroup.CompanyDetail.Id == Caller.CompanyId && x.Id == ProductId).ToList();


            #region Ledger

            decimal opqty,sqty, prqty, srqty, pqty, BalQty, Inwards,Outwards;
            BalQty = 0;
            foreach (var P in lstProduct)
            {
                gl = new BLL.GeneralStock();

                gl.Product = Product_DALtoBLL(P);

                opqty = (decimal)P.OpeningStock;

                pqty =(decimal) P.PurchaseDetails.Sum(x => x.Quantity);
                srqty = (decimal)P.SalesReturnDetails.Sum(x => x.Quantity);
                sqty = (decimal)P.SalesDetails.Sum(x => x.Quantity);
                prqty = (decimal)P.PurchaseReturnDetails.Sum(x => x.Quantity);

              

                //gl.Inwards = pqty+srqty;
                //gl.Outwards = sqty+prqty;


                // BalQty += (gl.Inwards - gl.Outwards)+opqty;
                BalQty += opqty;
                gl.BalStock = Math.Abs(BalQty);

                gl.Ledger = new BLL.Ledger();
                gl.Ledger.LedgerName = string.Format("Balance {0}", P.ProductName);
                lstGeneralStock.Add(gl);

                foreach (var pd in P.PurchaseDetails.Where(x => x.Purchase.PurchaseDate >= dtFrom && x.Purchase.PurchaseDate <= dtTo).ToList())
                {
                    gl = new BLL.GeneralStock();
                    gl.Product = new BLL.Product();
                    gl.Product = Product_DALtoBLL(pd.Product);

                    gl.EId = pd.Purchase.Id;
                    gl.EType = "P";
                    gl.TType = pd.Purchase.TransactionType.Type;
                    gl.EDate = pd.Purchase.PurchaseDate;
                    gl.Ledger = new BLL.Ledger();
                    gl.Ledger = LedgerDAL_BLL(pd.Purchase.Ledger);
                    gl.EntryNo = pd.Purchase.RefNo;
                    gl.Inwards =(decimal) pd.Quantity;
                    gl.Outwards = 0;
                    BalQty += (gl.Inwards - gl.Outwards);
                    gl.BalStock = Math.Abs(BalQty);
                    lstGeneralStock.Add(gl);
                }


        

                foreach (var s in P.SalesDetails.Where(x => x.Sale.SalesDate >= dtFrom && x.Sale.SalesDate <= dtTo).ToList())
                {
                   
                        gl = new BLL.GeneralStock();
                        gl.Ledger = new BLL.Ledger();
                        gl.Ledger = LedgerDAL_BLL(s.Sale.Ledger);
                        gl.EId = s.Id;
                        gl.EType = "S";
                        gl.EDate = s.Sale.SalesDate;
                        gl.TType = s.Sale.TransactionType.Type;
                        gl.EntryNo = s.Sale.RefNo;
                        gl.Outwards =(decimal) s.Quantity;
                        gl.Inwards = 0;
                        BalQty += (gl.Inwards - gl.Outwards);
                        gl.BalStock = Math.Abs(BalQty);
                        lstGeneralStock.Add(gl);
                    

                }
                foreach (var pr in P.PurchaseReturnDetails.Where(x => x.PurchaseReturn.PRDate >= dtFrom && x.PurchaseReturn.PRDate <= dtTo).ToList())
                {
                    gl = new BLL.GeneralStock();
                    gl.Ledger = new BLL.Ledger();
                    gl.Ledger = LedgerDAL_BLL(pr.PurchaseReturn.Ledger);
                    
                    gl.EId = pr.PurchaseReturn.Id;
                    gl.EType = "PR";
                    gl.EDate = pr.PurchaseReturn.PRDate;
                    gl.TType = pr.PurchaseReturn.TransactionType.Type;
                    gl.EntryNo = pr.PurchaseReturn.RefNo;
                    gl.Inwards = 0;
                    gl.Outwards = (decimal)pr.Quantity;
                    BalQty += (gl.Inwards - gl.Outwards);
                    gl.BalStock = Math.Abs(BalQty);
                    lstGeneralStock.Add(gl);
                }

                foreach (var sr in P.SalesReturnDetails.Where(x => x.SalesReturn.SRDate >= dtFrom && x.SalesReturn.SRDate <= dtTo).ToList())
                {
                    gl = new BLL.GeneralStock();
                    gl.Ledger = new BLL.Ledger();

                    gl.Ledger = LedgerDAL_BLL(sr.SalesReturn.Ledger);
                
                    gl.EId = sr.SalesReturn.Id;
                    gl.EType = "SR";
                    gl.EDate = sr.SalesReturn.SRDate;

                    gl.EntryNo = sr.SalesReturn.RefNo;
                    gl.TType = sr.SalesReturn.TransactionType.Type;
                    gl.Inwards = (decimal)sr.Quantity;
                    gl.Outwards = 0;
                    BalQty += (gl.Inwards- gl.Outwards);
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