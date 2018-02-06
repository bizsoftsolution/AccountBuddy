using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        public List<BLL.SalesReportNew> SalesReportNew_ToList(DateTime dtFrom, DateTime dtTo, bool isMonthly, String ReportType)
        {
            List<BLL.SalesReportNew> list = new List<BLL.SalesReportNew>();

            var l1 = DB.Sales.Where(x => x.SalesDate >= dtFrom && x.SalesDate <= dtTo && x.Ledger.AccountGroup.CompanyId == Caller.CompanyId).ToList();

            foreach (var s in l1)
            {
                foreach (var sd in s.SalesDetails)
                {
                    list.Add(new BLL.SalesReportNew()
                    {
                        CustomerName = s.Ledger.LedgerName,
                        ProductName = sd.Product.ProductName,
                        Amount = sd.Amount,
                        Month = isMonthly == true ? string.Format("{0:MMM-yyyy}", s.SalesDate) : string.Format("{0:yyyy}", s.SalesDate)
                    });
                }
            }
            
            return list;
        }


        public List<BLL.SalesReportNew> tempSalesReportNew_ToList(DateTime dtFrom, DateTime dtTo, bool isMonthly, String ReportType)
        {
            List<BLL.SalesReportNew> rv = new List<BLL.SalesReportNew>();
            List<BLL.SalesReportNew> list = new List<BLL.SalesReportNew>();
            BLL.SalesReportNew sr = new BLL.SalesReportNew();
            int SNo = 111; int APref = 111;
            var l1 = DB.SalesDetails.Where(x => x.Sale.Ledger.LedgerName.StartsWith("DL-") &&
                                              x.Sale.Ledger.AccountGroup.CompanyDetail.Id == Caller.CompanyId &&
                                              x.Sale.SalesDate >= dtFrom &&
                                              x.Sale.SalesDate <= dtTo)
                                  .Select(x => new
                                  {
                                      x.ProductId,
                                      x.Product.ProductName,
                                      x.Sale.Ledger.AccountGroup.CompanyId,
                                      x.Sale.Ledger.AccountGroup.CompanyDetail.CompanyName,
                                      x.Sale.Ledger.LedgerName,
                                      x.Sale.SalesDate,
                                      x.Amount
                                  })
                                  .ToList();

            int n = Math.Abs((dtTo.Year * 12 + (dtTo.Month - 1)) - (dtFrom.Year * 12 + (dtFrom.Month - 1)));
            if (isMonthly == false) n = n / 2;
            if (n > 12) n = 12;
            if (l1.Count() > 0)
            {
                #region DealerWise
                if (ReportType == "DealerWise")
                {

                    foreach (var l in l1)
                    {
                        SNo = 111;
                        sr = new BLL.SalesReportNew();
                        sr.Description = string.Format("{0}-{1}", ++SNo, l.LedgerName);
                        sr.Month = isMonthly == true ? string.Format("{1}-{0:MMM-yyyy}", dtFrom.AddMonths(0), ++APref) : string.Format("{1}{0:yyyy}", dtFrom.AddYears(0), ++APref);
                        rv.Add(sr);
                        foreach (var p in l1.GroupBy(x => x.ProductName))
                        {
                            decimal[] amt = new decimal[12];
                            APref = 111;
                            for (int i = 0; i < n; i++)
                            {
                                amt[i] = isMonthly == true ? 0 : 1;
                                sr = new BLL.SalesReportNew();
                                if (isMonthly == true)
                                {
                                    DateTime dt = dtFrom.AddMonths(i);
                                    amt[i] = p.Where(x => x.SalesDate.Year == dt.Year && x.SalesDate.Month == dt.Month).Sum(x => x.Amount);
                                }
                                else
                                {
                                    DateTime dt = dtFrom.AddYears(i);
                                    amt[i] = p.Where(x => x.SalesDate.Year == dt.Year).Sum(x => x.Amount);
                                }
                                sr.Description = string.Format("  {1}-{0}", p.Key, ++SNo);
                                sr.Amount = amt[i];
                                sr.Month = isMonthly == true ? string.Format("{1}-{0:MMM-yyyy}", dtFrom.AddMonths(i), ++APref) : string.Format("{1}{0:yyyy}", dtFrom.AddYears(i), ++APref);
                                rv.Add(sr);
                            }
                        }
                    }


                }
                #endregion

                #region ProductWise
                if (ReportType == "ProductWise")
                {
                    var l2 = l1.GroupBy(x => x.ProductName).ToList();

                    decimal[] gtamt = new decimal[12];

                    foreach (var d1 in l2)
                    {
                        var l3 = d1.GroupBy(x => x.LedgerName).ToList();
                        decimal[] tamt = new decimal[12];


                        sr = new BLL.SalesReportNew();
                        sr.Description = d1.Key;
                        rv.Add(sr);


                        foreach (var d2 in l3)
                        {
                            decimal[] amt = new decimal[12];

                            for (int i = 0; i < n; i++)
                            {

                                amt[i] = isMonthly == true ? 0 : 1;

                                if (isMonthly == true)
                                {
                                    DateTime dt = dtFrom.AddMonths(i);
                                    amt[i] = d2.Where(x => x.SalesDate.Year == dt.Year && x.SalesDate.Month == dt.Month).Sum(x => x.Amount);
                                }
                                else
                                {
                                    DateTime dt = dtFrom.AddYears(i);
                                    amt[i] = d2.Where(x => x.SalesDate.Year == dt.Year).Sum(x => x.Amount);
                                }
                            }

                            sr = new BLL.SalesReportNew();
                            sr.Amount = amt.Sum();

                            if (sr.Amount > 0)
                            {
                                for (int i = 0; i < 12; i++)
                                {
                                    sr = new BLL.SalesReportNew();

                                    sr.Description = string.Format("   {0}", d2.Key);
                                    sr.Amount = amt[i];
                                    sr.Month = isMonthly == true ? string.Format("{1}{0:MMM-yyyy}", dtFrom.AddMonths(i), ++APref) : string.Format("{1}{0:yyyy}", dtFrom.AddYears(i), ++APref);

                                    rv.Add(sr);
                                }

                            }

                            for (int i = 0; i < 12; i++)
                            {
                                tamt[i] += amt[i];
                            }

                        }
                        sr = new BLL.SalesReportNew();
                        sr.Amount = tamt.Sum();

                        if (sr.Amount == 0)
                        {
                            rv.RemoveAt(rv.Count() - 1);
                        }
                        else
                        {
                            for (int i = 0; i < 12; i++)
                            {
                                sr = new BLL.SalesReportNew();

                                sr.Description = string.Format("{1}Total {0}", d1.Key, ++SNo);
                                sr.Amount = tamt[i];
                                sr.Month = isMonthly == true ? string.Format("{1}{0:MMM-yyyy}", dtFrom.AddMonths(i), ++APref) : string.Format("{1}{0:yyyy}", dtFrom.AddYears(i), ++APref);
                                rv.Add(sr);

                                rv.Add(new BLL.SalesReportNew());
                            }
                        }

                        for (int i = 0; i < 12; i++)
                        {
                            gtamt[i] += tamt[i];
                        }
                    }
                    sr = new BLL.SalesReportNew();
                    sr.Amount = gtamt.Sum();

                    if (sr.Amount != 0)
                    {
                        for (int i = 0; i < 12; i++)
                        {
                            sr = new BLL.SalesReportNew();

                            sr.Description = string.Format("{0}Grand Total", ++SNo);
                            sr.Amount = gtamt[i];
                            sr.Month = isMonthly == true ? string.Format("{1}{0:MMM-yyyy}", dtFrom.AddMonths(i), ++APref) : string.Format("{1}{0:yyyy}", dtFrom.AddYears(i), ++APref);
                            rv.Add(sr);

                            rv.Add(new BLL.SalesReportNew());
                        }
                    }

                }
                #endregion

                #region DealerSummary
                if (ReportType == "DealerSummary")
                {
                    var l2 = l1.GroupBy(x => x.LedgerName).ToList();

                    decimal[] gtamt = new decimal[12];

                    foreach (var d1 in l2)
                    {
                        decimal[] tamt = new decimal[12];

                        for (int i = 0; i < n; i++)
                        {

                            tamt[i] = isMonthly == true ? 0 : 1;

                            if (isMonthly == true)
                            {
                                DateTime dt = dtFrom.AddMonths(i);
                                tamt[i] = d1.Where(x => x.SalesDate.Year == dt.Year && x.SalesDate.Month == dt.Month).Sum(x => x.Amount);
                            }
                            else
                            {
                                DateTime dt = dtFrom.AddYears(i);
                                tamt[i] = d1.Where(x => x.SalesDate.Year == dt.Year).Sum(x => x.Amount);
                            }
                        }

                        sr = new BLL.SalesReportNew();
                        sr.Amount = tamt.Sum();

                        if (sr.Amount > 0)
                        {
                            for (int i = 0; i < 12; i++)
                            {
                                sr = new BLL.SalesReportNew();

                                sr.Description = string.Format("{1}{0}", d1.Key, ++SNo);
                                sr.Amount = tamt[i];
                                sr.Month = isMonthly == true ? string.Format("{1}{0:MMM-yyyy}", dtFrom.AddMonths(i), ++APref) : string.Format("{1}{0:yyyy}", dtFrom.AddYears(i), ++APref);

                                rv.Add(sr);
                            }

                        }

                        for (int i = 0; i < 12; i++)
                        {
                            gtamt[i] += tamt[i];
                        }
                    }
                    sr = new BLL.SalesReportNew();
                    sr.Amount = gtamt.Sum();

                    if (sr.Amount != 0)
                    {
                        for (int i = 0; i < 12; i++)
                        {
                            sr = new BLL.SalesReportNew();

                            sr.Description = string.Format("{0}Total", ++SNo);
                            sr.Amount = gtamt[i];
                            sr.Month = isMonthly == true ? string.Format("{1}{0:MMM-yyyy}", dtFrom.AddMonths(i), ++APref) : string.Format("{1}{0:yyyy}", dtFrom.AddYears(i), ++APref);
                            rv.Add(sr);

                            rv.Add(new BLL.SalesReportNew());
                        }
                    }

                }
                #endregion

                #region ProductSummary
                if (ReportType == "ProductSummary")
                {
                    var l2 = l1.GroupBy(x => x.ProductName).ToList();

                    decimal[] gtamt = new decimal[12];

                    foreach (var d1 in l2)
                    {
                        decimal[] tamt = new decimal[12];

                        for (int i = 0; i < n; i++)
                        {

                            tamt[i] = isMonthly == true ? 0 : 1;

                            if (isMonthly == true)
                            {
                                DateTime dt = dtFrom.AddMonths(i);
                                tamt[i] = d1.Where(x => x.SalesDate.Year == dt.Year && x.SalesDate.Month == dt.Month).Sum(x => x.Amount);
                            }
                            else
                            {
                                DateTime dt = dtFrom.AddYears(i);
                                tamt[i] = d1.Where(x => x.SalesDate.Year == dt.Year).Sum(x => x.Amount);
                            }
                        }

                        sr = new BLL.SalesReportNew();
                        sr.Amount = tamt.Sum();

                        if (sr.Amount > 0)
                        {
                            for (int i = 0; i < n; i++)
                            {
                                sr = new BLL.SalesReportNew();

                                sr.Description = string.Format("{1}{0}", d1.Key, ++SNo);
                                sr.Amount = tamt[i];
                                sr.Month = isMonthly == true ? string.Format("{1}{0:MMM-yyyy}", dtFrom.AddMonths(i), ++APref) : string.Format("{1}{0:yyyy}", dtFrom.AddYears(i), APref);
                                rv.Add(sr);
                            }
                        }

                        for (int i = 0; i < 12; i++)
                        {
                            gtamt[i] += tamt[i];
                        }
                    }
                    sr = new BLL.SalesReportNew();
                    sr.Amount = gtamt.Sum();

                    if (sr.Amount != 0)
                    {
                        for (int i = 0; i < 12; i++)
                        {
                            sr = new BLL.SalesReportNew();

                            sr.Description = string.Format("{0}Total", ++SNo);
                            sr.Amount = gtamt[i];
                            sr.Month = isMonthly == true ? string.Format("{1}{0:MMM-yyyy}", dtFrom.AddMonths(i), ++APref) : string.Format("{1}{0:yyyy}", dtFrom.AddYears(i), ++APref);
                            rv.Add(sr);
                            rv.Add(new BLL.SalesReportNew());
                        }
                    }

                }
                #endregion

            }



            return rv;

        }
    }
}