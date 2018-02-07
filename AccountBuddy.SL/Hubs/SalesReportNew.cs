using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        public List<BLL.SalesReportNew> SalesReportNew_ToList(DateTime dtFrom, DateTime dtTo, bool isMonthly, bool isDate, bool isYear, String ReportType)
        {
            List<BLL.SalesReportNew> list = new List<BLL.SalesReportNew>();
            List<DAL.Sale> l1 = new List<DAL.Sale>();
            if (ReportType == "Dealer")
            {
                 l1 = DB.Sales.Where(x => x.SalesDate >= dtFrom &&
                                                x.SalesDate <= dtTo &&
                                                x.Ledger.AccountGroup.CompanyId == Caller.CompanyId &&
                                                x.Ledger.LedgerName.StartsWith("DL-")
                                                ).ToList();
            }
            else
            {
                l1 = DB.Sales.Where(x => x.SalesDate >= dtFrom &&
                                               x.SalesDate <= dtTo &&
                                               x.Ledger.AccountGroup.CompanyId == Caller.CompanyId &&
                                               !x.Ledger.LedgerName.StartsWith("DL-")
                                               && !x.Ledger.LedgerName.StartsWith("WH-")
                                               ).ToList();
            }
            var m = "";
            foreach (var s in l1)
            {
                foreach (var sd in s.SalesDetails)
                {
                    if (isMonthly == true)
                    {
                        m = string.Format("{0:MMM-yyyy}", s.SalesDate);

                    }
                    else if (isDate == true)
                    {
                        m = string.Format("{0:dd-MM-yyyy}", s.SalesDate);

                    }
                    else if (isYear == true)
                    {
                        m = string.Format("{0:yyyy}", s.SalesDate);
                    }



                    list.Add(new BLL.SalesReportNew()
                    {
                        CustomerName = s.Ledger.LedgerName,
                        ProductName = sd.Product.ProductName,
                        Amount = sd.Amount,
                        Month = m
                    });
                }
            }

            return list;
        }

        public List<BLL.SalesReportNew> PurchaseReport_ToList(DateTime dtFrom, DateTime dtTo, bool isMonthly, bool isDate, bool isYear, String ReportType)
        {
            List<BLL.SalesReportNew> list = new List<BLL.SalesReportNew>();
            List<DAL.Purchase> l1 = new List<DAL.Purchase>();
            if (ReportType == "Dealer")
            {
                l1 = DB.Purchases.Where(x => x.PurchaseDate >= dtFrom &&
                                               x.PurchaseDate <= dtTo &&
                                               x.Ledger.AccountGroup.CompanyId == Caller.CompanyId &&
                                               x.Ledger.LedgerName.StartsWith("DL-")
                                               ).ToList();
            }
            else
            {
                l1 = DB.Purchases.Where(x => x.PurchaseDate >= dtFrom &&
                                               x.PurchaseDate <= dtTo &&
                                               x.Ledger.AccountGroup.CompanyId == Caller.CompanyId &&
                                               !x.Ledger.LedgerName.StartsWith("DL-")
                                               && !x.Ledger.LedgerName.StartsWith("WH-")
                                               ).ToList();
            }
            var m = "";
            foreach (var s in l1)
            {
                foreach (var sd in s.PurchaseDetails)
                {
                    if (isMonthly == true)
                    {
                        m = string.Format("{0:MMM-yyyy}", s.PurchaseDate);

                    }
                    else if (isDate == true)
                    {
                        m = string.Format("{0:dd-MM-yyyy}", s.PurchaseDate);

                    }
                    else if (isYear == true)
                    {
                        m = string.Format("{0:yyyy}", s.PurchaseDate);
                    }
                    
                    list.Add(new BLL.SalesReportNew()
                    {
                        CustomerName = s.Ledger.LedgerName,
                        ProductName = sd.Product.ProductName,
                        Amount = sd.Amount,
                        Month = m
                    });
                }
            }

            return list;
        }

    }
}
