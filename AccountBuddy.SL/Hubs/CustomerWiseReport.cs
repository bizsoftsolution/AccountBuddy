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
            tb.Month3 = (dtFrom.Month - 4).ToString();
            tb.Month4 = (dtFrom.Month - 3).ToString();
            tb.Month5 = (dtFrom.Month - 2).ToString();
            tb.Month6 = (dtFrom.Month - 1).ToString();
            foreach (var l in lstLedger)
            {
               

                foreach (var pd in l.Sales.FirstOrDefault().SalesDetails.Where(x => x.Sale.SalesDate >= dtFrom).ToList())
                {
                    tb = new BLL.CustomerWiseReport();
                    tb.Ledger = LedgerDAL_BLL(l);
                    tb.ProductName = pd.Product.ProductName;
                    tb.Amount = pd.Sale.TotalAmount;
                    tb.M1 = (decimal)l.Sales.FirstOrDefault().SalesDetails.Where(x => x.Sale.SalesDate.Month == dtFrom.Month).Sum(x => x.Quantity);
                    tb.M2 = (decimal)l.Sales.FirstOrDefault().SalesDetails.Where(x => x.Sale.SalesDate.Month - 5 == dtFrom.Month - 5).Sum(x => x.Quantity);
                    tb.M3 = (decimal)l.Sales.FirstOrDefault().SalesDetails.Where(x => x.Sale.SalesDate.Month - 4 == dtFrom.Month - 4).Sum(x => x.Quantity);
                    tb.M4 = (decimal)l.Sales.FirstOrDefault().SalesDetails.Where(x => x.Sale.SalesDate.Month - 3 == dtFrom.Month - 3).Sum(x => x.Quantity);

                    tb.M5 = (decimal)l.Sales.FirstOrDefault().SalesDetails.Where(x => x.Sale.SalesDate.Month - 2 == dtFrom.Month - 2).Sum(x => x.Quantity);
                    tb.M6 = (decimal)l.Sales.FirstOrDefault().SalesDetails.Where(x => x.Sale.SalesDate.Month - 1 == dtFrom.Month - 1).Sum(x => x.Quantity);

                    lstCustomerWise.Add(tb);
                }

               
            }
            return lstCustomerWise;


        }
        #endregion

    }
}