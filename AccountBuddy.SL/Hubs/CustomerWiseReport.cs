using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        public List<BLL.CustomerWiseReport> CustomerWiseReport_List(int CustomerId, DateTime dtFrom)
        {
            List<BLL.CustomerWiseReport> lstCustomerWise = new List<BLL.CustomerWiseReport>();
            BLL.CustomerWiseReport tb = new BLL.CustomerWiseReport();

            var lstLedger = DB.Ledgers.Where(x => x.AccountGroup.CompanyDetail.Id == Caller.CompanyId && x.Id == CustomerId).ToList();


            #region Ledger

            decimal Qty = 0;
            tb.Month1 = dtFrom.ToString("MMMM");
            tb.Month2 = dtFrom.AddMonths(-1).ToString("MMMM");
            tb.Month3 = dtFrom.AddMonths(-2).ToString("MMMM");
            tb.Month4 = dtFrom.AddMonths(-3).ToString("MMMM");
            tb.Month5 = dtFrom.AddMonths(-4).ToString("MMMM");
            tb.Month6 = dtFrom.AddMonths(-5).ToString("MMMM");
            tb.Ledger = new BLL.Ledger();
            tb.ProductName = "";
            tb.Amount =null;
            lstCustomerWise.Add(tb);
            foreach (var l in lstLedger)
            {
               

                foreach (var pd in l.Sales.FirstOrDefault().SalesDetails.ToList())
                {
                    tb = new BLL.CustomerWiseReport();
                    tb.Ledger = LedgerDAL_BLL(l);
                    tb.ProductName = pd.Product.ProductName;
                    tb.Amount = pd.Sale.TotalAmount;
                    tb.M1 = (decimal)l.Sales.FirstOrDefault().SalesDetails.Where(x => x.Sale.SalesDate.Month == dtFrom.Month).Sum(x => x.Quantity);
                    tb.M2 = (decimal)l.Sales.FirstOrDefault().SalesDetails.Where(x => x.Sale.SalesDate.AddMonths(-5) == dtFrom.AddMonths(-5)).Sum(x => x.Quantity);
                    tb.M3 = (decimal)l.Sales.FirstOrDefault().SalesDetails.Where(x => x.Sale.SalesDate.AddMonths(-4) == dtFrom.AddMonths(-4)).Sum(x => x.Quantity);
                    tb.M4 = (decimal)l.Sales.FirstOrDefault().SalesDetails.Where(x => x.Sale.SalesDate.AddMonths(-3) == dtFrom.AddMonths(-3)).Sum(x => x.Quantity);

                    tb.M5 = (decimal)l.Sales.FirstOrDefault().SalesDetails.Where(x => x.Sale.SalesDate.AddMonths(-2) == dtFrom.AddMonths(-2)).Sum(x => x.Quantity);
                    tb.M6 = (decimal)l.Sales.FirstOrDefault().SalesDetails.Where(x => x.Sale.SalesDate.AddMonths(-1) == dtFrom.AddMonths(-1)).Sum(x => x.Quantity);
                    tb.Month1 = "";
                    tb.Month2 = "";
                    tb.Month3 = "";
                    tb.Month4 = "";
                    tb.Month5 = "";
                    tb.Month6 = "";
                    lstCustomerWise.Add(tb);
                }

               
            }
            return lstCustomerWise;


        }
        #endregion

    }
}