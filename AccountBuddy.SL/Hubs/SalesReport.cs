using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        public List<BLL.SalesReport> SalesReport_List(DateTime dtFrom,DateTime dtTo,bool isMonthly,string ReportType)
        {
            BLL.SalesReport sr = new BLL.SalesReport();

            List<BLL.SalesReport> rv = new List<BLL.SalesReport>();
            
            var l1 = DB.SalesDetails.Where(x => x.Sale.Ledger.AccountGroup.CompanyDetail.CompanyType == "Dealer" && 
                                                x.Sale.Ledger.AccountGroup.CompanyDetail.UnderCompanyId==Caller.CompanyId && 
                                                x.Sale.SalesDate >= dtFrom && 
                                                x.Sale.SalesDate <= dtTo)
                                    .Select(x=> new {
                                                        x.ProductId,
                                                        x.Product.ProductName,
                                                        x.Sale.Ledger.AccountGroup.CompanyId,
                                                        x.Sale.Ledger.AccountGroup.CompanyDetail.CompanyName,
                                                        x.Sale.SalesDate,
                                                        x.Amount
                                                    })
                                    .ToList();

            int n = Math.Abs((dtTo.Year * 12 + (dtTo.Month - 1)) - (dtFrom.Year * 12 + (dtFrom.Month - 1)));
            if (isMonthly == false) n =n/2;
            if (n > 12) n = 12;

            if (l1.Count() > 0)
            {
                #region DealerWise
                if (ReportType == "DealerWise")
                {
                    var l2 = l1.GroupBy(x => x.CompanyName).ToList();

                    decimal[] gtamt = new decimal[12];

                    foreach (var d1 in l2)
                    {
                        var l3 = d1.GroupBy(x => x.ProductName).ToList();
                        decimal[] tamt = new decimal[12];


                        sr = new BLL.SalesReport();
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

                            sr = new BLL.SalesReport();
                            sr.Amount = amt.Sum();

                            if (sr.Amount > 0)
                            {
                                sr.Description = string.Format("   {0}", d2.Key);
                                sr.M1 = amt[0];
                                sr.M2 = amt[1];
                                sr.M3 = amt[2];
                                sr.M4 = amt[3];
                                sr.M5 = amt[4];
                                sr.M6 = amt[5];
                                sr.M7 = amt[6];
                                sr.M8 = amt[7];
                                sr.M9 = amt[8];
                                sr.M10 = amt[9];
                                sr.M11 = amt[10];
                                sr.M12 = amt[11];
                                rv.Add(sr);
                            }

                            for (int i = 0; i < 12; i++)
                            {
                                tamt[i] += amt[i];
                            }

                        }
                        sr = new BLL.SalesReport();
                        sr.Amount = tamt.Sum();

                        if (sr.Amount == 0)
                        {
                            rv.RemoveAt(rv.Count() - 1);
                        }
                        else
                        {
                            sr.Description = string.Format("Total {0}", d1.Key);
                            sr.M1 = tamt[0];
                            sr.M2 = tamt[1];
                            sr.M3 = tamt[2];
                            sr.M4 = tamt[3];
                            sr.M5 = tamt[4];
                            sr.M6 = tamt[5];
                            sr.M7 = tamt[6];
                            sr.M8 = tamt[7];
                            sr.M9 = tamt[8];
                            sr.M10 = tamt[9];
                            sr.M11 = tamt[10];
                            sr.M12 = tamt[11];
                            rv.Add(sr);

                            rv.Add(new BLL.SalesReport());
                        }

                        for (int i = 0; i < 12; i++)
                        {
                            gtamt[i] += tamt[i];
                        }
                    }
                    sr = new BLL.SalesReport();
                    sr.Amount = gtamt.Sum();

                    if (sr.Amount != 0)
                    {
                        sr.Description = string.Format("Grand Total");
                        sr.M1 = gtamt[0];
                        sr.M2 = gtamt[1];
                        sr.M3 = gtamt[2];
                        sr.M4 = gtamt[3];
                        sr.M5 = gtamt[4];
                        sr.M6 = gtamt[5];
                        sr.M7 = gtamt[6];
                        sr.M8 = gtamt[7];
                        sr.M9 = gtamt[8];
                        sr.M10 = gtamt[9];
                        sr.M11 = gtamt[10];
                        sr.M12 = gtamt[11];
                        rv.Add(sr);

                        rv.Add(new BLL.SalesReport());
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
                        var l3 = d1.GroupBy(x => x.CompanyName).ToList();
                        decimal[] tamt = new decimal[12];


                        sr = new BLL.SalesReport();
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

                            sr = new BLL.SalesReport();
                            sr.Amount = amt.Sum();

                            if (sr.Amount > 0)
                            {
                                sr.Description = string.Format("   {0}", d2.Key);
                                sr.M1 = amt[0];
                                sr.M2 = amt[1];
                                sr.M3 = amt[2];
                                sr.M4 = amt[3];
                                sr.M5 = amt[4];
                                sr.M6 = amt[5];
                                sr.M7 = amt[6];
                                sr.M8 = amt[7];
                                sr.M9 = amt[8];
                                sr.M10 = amt[9];
                                sr.M11 = amt[10];
                                sr.M12 = amt[11];
                                rv.Add(sr);
                            }

                            for (int i = 0; i < 12; i++)
                            {
                                tamt[i] += amt[i];
                            }

                        }
                        sr = new BLL.SalesReport();
                        sr.Amount = tamt.Sum();

                        if (sr.Amount == 0)
                        {
                            rv.RemoveAt(rv.Count() - 1);
                        }
                        else
                        {
                            sr.Description = string.Format("Total {0}", d1.Key);
                            sr.M1 = tamt[0];
                            sr.M2 = tamt[1];
                            sr.M3 = tamt[2];
                            sr.M4 = tamt[3];
                            sr.M5 = tamt[4];
                            sr.M6 = tamt[5];
                            sr.M7 = tamt[6];
                            sr.M8 = tamt[7];
                            sr.M9 = tamt[8];
                            sr.M10 = tamt[9];
                            sr.M11 = tamt[10];
                            sr.M12 = tamt[11];
                            rv.Add(sr);

                            rv.Add(new BLL.SalesReport());
                        }

                        for (int i = 0; i < 12; i++)
                        {
                            gtamt[i] += tamt[i];
                        }
                    }
                    sr = new BLL.SalesReport();
                    sr.Amount = gtamt.Sum();

                    if (sr.Amount != 0)
                    {
                        sr.Description = string.Format("Grand Total");
                        sr.M1 = gtamt[0];
                        sr.M2 = gtamt[1];
                        sr.M3 = gtamt[2];
                        sr.M4 = gtamt[3];
                        sr.M5 = gtamt[4];
                        sr.M6 = gtamt[5];
                        sr.M7 = gtamt[6];
                        sr.M8 = gtamt[7];
                        sr.M9 = gtamt[8];
                        sr.M10 = gtamt[9];
                        sr.M11 = gtamt[10];
                        sr.M12 = gtamt[11];
                        rv.Add(sr);

                        rv.Add(new BLL.SalesReport());
                    }

                }
                #endregion

                #region DealerSummary
                if (ReportType == "DealerSummary")
                {
                    var l2 = l1.GroupBy(x => x.CompanyName).ToList();

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

                        sr = new BLL.SalesReport();
                        sr.Amount = tamt.Sum();

                        if (sr.Amount > 0)
                        {
                            sr.Description = string.Format("{0}", d1.Key);
                            sr.M1 = tamt[0];
                            sr.M2 = tamt[1];
                            sr.M3 = tamt[2];
                            sr.M4 = tamt[3];
                            sr.M5 = tamt[4];
                            sr.M6 = tamt[5];
                            sr.M7 = tamt[6];
                            sr.M8 = tamt[7];
                            sr.M9 = tamt[8];
                            sr.M10 = tamt[9];
                            sr.M11 = tamt[10];
                            sr.M12 = tamt[11];
                            rv.Add(sr);
                        }
                        
                        for (int i = 0; i < 12; i++)
                        {
                            gtamt[i] += tamt[i];
                        }
                    }
                    sr = new BLL.SalesReport();
                    sr.Amount = gtamt.Sum();

                    if (sr.Amount != 0)
                    {
                        sr.Description = string.Format("Total");
                        sr.M1 = gtamt[0];
                        sr.M2 = gtamt[1];
                        sr.M3 = gtamt[2];
                        sr.M4 = gtamt[3];
                        sr.M5 = gtamt[4];
                        sr.M6 = gtamt[5];
                        sr.M7 = gtamt[6];
                        sr.M8 = gtamt[7];
                        sr.M9 = gtamt[8];
                        sr.M10 = gtamt[9];
                        sr.M11 = gtamt[10];
                        sr.M12 = gtamt[11];
                        rv.Add(sr);

                        rv.Add(new BLL.SalesReport());
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

                        sr = new BLL.SalesReport();
                        sr.Amount = tamt.Sum();

                        if (sr.Amount > 0)
                        {
                            sr.Description = string.Format("{0}", d1.Key);
                            sr.M1 = tamt[0];
                            sr.M2 = tamt[1];
                            sr.M3 = tamt[2];
                            sr.M4 = tamt[3];
                            sr.M5 = tamt[4];
                            sr.M6 = tamt[5];
                            sr.M7 = tamt[6];
                            sr.M8 = tamt[7];
                            sr.M9 = tamt[8];
                            sr.M10 = tamt[9];
                            sr.M11 = tamt[10];
                            sr.M12 = tamt[11];
                            rv.Add(sr);
                        }

                        for (int i = 0; i < 12; i++)
                        {
                            gtamt[i] += tamt[i];
                        }
                    }
                    sr = new BLL.SalesReport();
                    sr.Amount = gtamt.Sum();

                    if (sr.Amount != 0)
                    {
                        sr.Description = string.Format("Total");
                        sr.M1 = gtamt[0];
                        sr.M2 = gtamt[1];
                        sr.M3 = gtamt[2];
                        sr.M4 = gtamt[3];
                        sr.M5 = gtamt[4];
                        sr.M6 = gtamt[5];
                        sr.M7 = gtamt[6];
                        sr.M8 = gtamt[7];
                        sr.M9 = gtamt[8];
                        sr.M10 = gtamt[9];
                        sr.M11 = gtamt[10];
                        sr.M12 = gtamt[11];
                        rv.Add(sr);

                        rv.Add(new BLL.SalesReport());
                    }

                }
                #endregion

            }



            return rv;
        }

        public List<BLL.SalesReport> SalesReport_ListCustomerWise(DateTime dtFrom, DateTime dtTo, bool isMonthly, string ReportType)
        {
            BLL.SalesReport sr = new BLL.SalesReport();

            List<BLL.SalesReport> rv = new List<BLL.SalesReport>();

            var l1 = DB.SalesDetails.Where(x => x.Sale.Ledger.AccountGroup.GroupName==BLL.DataKeyValue.SundryDebtors_Key && 
                                                x.Sale.Ledger.AccountGroup.CompanyDetail.Id == Caller.CompanyId &&

                                                x.Sale.SalesDate >= dtFrom &&
                                                x.Sale.SalesDate <= dtTo)
                                    .Select(x => new {
                                        x.ProductId,
                                        x.Product.ProductName,
                                        x.Sale.Ledger.AccountGroup.CompanyId,
                                        x.Sale.Ledger.AccountGroup.CompanyDetail.CompanyName,
                                        x.Sale.SalesDate,
                                        x.Amount, 
                                        x.Sale.Ledger.LedgerName
                                    })
                                    .ToList();

            int n = Math.Abs((dtTo.Year * 12 + (dtTo.Month - 1)) - (dtFrom.Year * 12 + (dtFrom.Month - 1)));
            if (isMonthly == false) n = n / 2;
            if (n > 12) n = 12;

            if (l1.Count() > 0)
            {
                #region CustomeWise
                if (ReportType == "CustomerWise")
                {
                    var l2 = l1.GroupBy(x => x.LedgerName).ToList();

                    decimal[] gtamt = new decimal[12];

                    foreach (var d1 in l2)
                    {
                        var l3 = d1.GroupBy(x => x.ProductName).ToList();
                        decimal[] tamt = new decimal[12];


                        sr = new BLL.SalesReport();
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

                            sr = new BLL.SalesReport();
                            sr.Amount = amt.Sum();

                            if (sr.Amount > 0)
                            {
                                sr.Description = string.Format("   {0}", d2.Key);
                                sr.M1 = amt[0];
                                sr.M2 = amt[1];
                                sr.M3 = amt[2];
                                sr.M4 = amt[3];
                                sr.M5 = amt[4];
                                sr.M6 = amt[5];
                                sr.M7 = amt[6];
                                sr.M8 = amt[7];
                                sr.M9 = amt[8];
                                sr.M10 = amt[9];
                                sr.M11 = amt[10];
                                sr.M12 = amt[11];
                                rv.Add(sr);
                            }

                            for (int i = 0; i < 12; i++)
                            {
                                tamt[i] += amt[i];
                            }

                        }
                        sr = new BLL.SalesReport();
                        sr.Amount = tamt.Sum();

                        if (sr.Amount == 0)
                        {
                            rv.RemoveAt(rv.Count() - 1);
                        }
                        else
                        {
                            sr.Description = string.Format("Total {0}", d1.Key);
                            sr.M1 = tamt[0];
                            sr.M2 = tamt[1];
                            sr.M3 = tamt[2];
                            sr.M4 = tamt[3];
                            sr.M5 = tamt[4];
                            sr.M6 = tamt[5];
                            sr.M7 = tamt[6];
                            sr.M8 = tamt[7];
                            sr.M9 = tamt[8];
                            sr.M10 = tamt[9];
                            sr.M11 = tamt[10];
                            sr.M12 = tamt[11];
                            rv.Add(sr);

                            rv.Add(new BLL.SalesReport());
                        }

                        for (int i = 0; i < 12; i++)
                        {
                            gtamt[i] += tamt[i];
                        }
                    }
                    sr = new BLL.SalesReport();
                    sr.Amount = gtamt.Sum();

                    if (sr.Amount != 0)
                    {
                        sr.Description = string.Format("Grand Total");
                        sr.M1 = gtamt[0];
                        sr.M2 = gtamt[1];
                        sr.M3 = gtamt[2];
                        sr.M4 = gtamt[3];
                        sr.M5 = gtamt[4];
                        sr.M6 = gtamt[5];
                        sr.M7 = gtamt[6];
                        sr.M8 = gtamt[7];
                        sr.M9 = gtamt[8];
                        sr.M10 = gtamt[9];
                        sr.M11 = gtamt[10];
                        sr.M12 = gtamt[11];
                        rv.Add(sr);

                        rv.Add(new BLL.SalesReport());
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


                        sr = new BLL.SalesReport();
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

                            sr = new BLL.SalesReport();
                            sr.Amount = amt.Sum();

                            if (sr.Amount > 0)
                            {
                                sr.Description = string.Format("   {0}", d2.Key);
                                sr.M1 = amt[0];
                                sr.M2 = amt[1];
                                sr.M3 = amt[2];
                                sr.M4 = amt[3];
                                sr.M5 = amt[4];
                                sr.M6 = amt[5];
                                sr.M7 = amt[6];
                                sr.M8 = amt[7];
                                sr.M9 = amt[8];
                                sr.M10 = amt[9];
                                sr.M11 = amt[10];
                                sr.M12 = amt[11];
                                rv.Add(sr);
                            }

                            for (int i = 0; i < 12; i++)
                            {
                                tamt[i] += amt[i];
                            }

                        }
                        sr = new BLL.SalesReport();
                        sr.Amount = tamt.Sum();

                        if (sr.Amount == 0)
                        {
                            rv.RemoveAt(rv.Count() - 1);
                        }
                        else
                        {
                            sr.Description = string.Format("Total {0}", d1.Key);
                            sr.M1 = tamt[0];
                            sr.M2 = tamt[1];
                            sr.M3 = tamt[2];
                            sr.M4 = tamt[3];
                            sr.M5 = tamt[4];
                            sr.M6 = tamt[5];
                            sr.M7 = tamt[6];
                            sr.M8 = tamt[7];
                            sr.M9 = tamt[8];
                            sr.M10 = tamt[9];
                            sr.M11 = tamt[10];
                            sr.M12 = tamt[11];
                            rv.Add(sr);

                            rv.Add(new BLL.SalesReport());
                        }

                        for (int i = 0; i < 12; i++)
                        {
                            gtamt[i] += tamt[i];
                        }
                    }
                    sr = new BLL.SalesReport();
                    sr.Amount = gtamt.Sum();

                    if (sr.Amount != 0)
                    {
                        sr.Description = string.Format("Grand Total");
                        sr.M1 = gtamt[0];
                        sr.M2 = gtamt[1];
                        sr.M3 = gtamt[2];
                        sr.M4 = gtamt[3];
                        sr.M5 = gtamt[4];
                        sr.M6 = gtamt[5];
                        sr.M7 = gtamt[6];
                        sr.M8 = gtamt[7];
                        sr.M9 = gtamt[8];
                        sr.M10 = gtamt[9];
                        sr.M11 = gtamt[10];
                        sr.M12 = gtamt[11];
                        rv.Add(sr);

                        rv.Add(new BLL.SalesReport());
                    }

                }
                #endregion

                #region CustomeSummary
                if (ReportType == "CustomerSummary")
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

                        sr = new BLL.SalesReport();
                        sr.Amount = tamt.Sum();

                        if (sr.Amount > 0)
                        {
                            sr.Description = string.Format("{0}", d1.Key);
                            sr.M1 = tamt[0];
                            sr.M2 = tamt[1];
                            sr.M3 = tamt[2];
                            sr.M4 = tamt[3];
                            sr.M5 = tamt[4];
                            sr.M6 = tamt[5];
                            sr.M7 = tamt[6];
                            sr.M8 = tamt[7];
                            sr.M9 = tamt[8];
                            sr.M10 = tamt[9];
                            sr.M11 = tamt[10];
                            sr.M12 = tamt[11];
                            rv.Add(sr);
                        }

                        for (int i = 0; i < 12; i++)
                        {
                            gtamt[i] += tamt[i];
                        }
                    }
                    sr = new BLL.SalesReport();
                    sr.Amount = gtamt.Sum();

                    if (sr.Amount != 0)
                    {
                        sr.Description = string.Format("Total");
                        sr.M1 = gtamt[0];
                        sr.M2 = gtamt[1];
                        sr.M3 = gtamt[2];
                        sr.M4 = gtamt[3];
                        sr.M5 = gtamt[4];
                        sr.M6 = gtamt[5];
                        sr.M7 = gtamt[6];
                        sr.M8 = gtamt[7];
                        sr.M9 = gtamt[8];
                        sr.M10 = gtamt[9];
                        sr.M11 = gtamt[10];
                        sr.M12 = gtamt[11];
                        rv.Add(sr);

                        rv.Add(new BLL.SalesReport());
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

                        sr = new BLL.SalesReport();
                        sr.Amount = tamt.Sum();

                        if (sr.Amount > 0)
                        {
                            sr.Description = string.Format("{0}", d1.Key);
                            sr.M1 = tamt[0];
                            sr.M2 = tamt[1];
                            sr.M3 = tamt[2];
                            sr.M4 = tamt[3];
                            sr.M5 = tamt[4];
                            sr.M6 = tamt[5];
                            sr.M7 = tamt[6];
                            sr.M8 = tamt[7];
                            sr.M9 = tamt[8];
                            sr.M10 = tamt[9];
                            sr.M11 = tamt[10];
                            sr.M12 = tamt[11];
                            rv.Add(sr);
                        }

                        for (int i = 0; i < 12; i++)
                        {
                            gtamt[i] += tamt[i];
                        }
                    }
                    sr = new BLL.SalesReport();
                    sr.Amount = gtamt.Sum();

                    if (sr.Amount != 0)
                    {
                        sr.Description = string.Format("Total");
                        sr.M1 = gtamt[0];
                        sr.M2 = gtamt[1];
                        sr.M3 = gtamt[2];
                        sr.M4 = gtamt[3];
                        sr.M5 = gtamt[4];
                        sr.M6 = gtamt[5];
                        sr.M7 = gtamt[6];
                        sr.M8 = gtamt[7];
                        sr.M9 = gtamt[8];
                        sr.M10 = gtamt[9];
                        sr.M11 = gtamt[10];
                        sr.M12 = gtamt[11];
                        rv.Add(sr);

                        rv.Add(new BLL.SalesReport());
                    }

                }
                #endregion

            }



            return rv;

        }
        public List<BLL.SalesReport> SalesReport_ListProductWise(DateTime dt)
        {
            List<BLL.SalesReport> rv = new List<BLL.SalesReport>();


            int CompanyId;
            try
            {
                if (Caller.CompanyType == "Company")
                {
                    CompanyId = Caller.CompanyId;
                }
                else if (Caller.CompanyType == "Warehouse")
                {
                    CompanyId = Caller.UnderCompanyId.Value;
                }
                else
                {
                    CompanyId = DB.CompanyDetails.Where(x => x.Id == Caller.UnderCompanyId).FirstOrDefault().UnderCompanyId.Value;
                }
                var lstProduct = DB.Products.Where(x => x.StockGroup.CompanyId == CompanyId).ToList();
                var lstWarehouse = DB.CompanyDetails.Where(x => x.UnderCompanyId == CompanyId).ToList();
                decimal GTotM1 = 0, GTotM2 = 0, GTotM3 = 0, GTotM4 = 0, GTotM5 = 0, GTotM6 = 0, GTot = 0;
                BLL.SalesReport sr = new BLL.SalesReport();

                foreach(var p in lstProduct)
                {
                    sr = new BLL.SalesReport();
                    sr.Description = p.ProductName;
                    rv.Add(sr);

                    foreach (var wh in lstWarehouse)
                    {
                        var lstDealer = DB.CompanyDetails.Where(x => x.UnderCompanyId == wh.Id).ToList();

                        decimal TotM1 = 0, TotM2 = 0, TotM3 = 0, TotM4 = 0, TotM5 = 0, TotM6 = 0, Tot = 0;
                        foreach (var dl in lstDealer)
                        {
                                                        
                            sr = new BLL.SalesReport();

                            sr.Description = String.Format("  {0}", dl.CompanyName);

                            sr.M1 = GetSalesAmount(dl.Id, p.Id, dt.AddMonths(-5));
                            sr.M2 = GetSalesAmount(dl.Id, p.Id, dt.AddMonths(-4));
                            sr.M3 = GetSalesAmount(dl.Id, p.Id, dt.AddMonths(-3));
                            sr.M4 = GetSalesAmount(dl.Id, p.Id, dt.AddMonths(-2));
                            sr.M5 = GetSalesAmount(dl.Id, p.Id, dt.AddMonths(-1));
                            sr.M6 = GetSalesAmount(dl.Id, p.Id, dt);

                            sr.Amount = sr.M1 + sr.M2 + sr.M3 + sr.M4 + sr.M5 + sr.M6;
                            TotM1 += sr.M1.Value;
                            TotM2 += sr.M2.Value;
                            TotM3 += sr.M3.Value;
                            TotM4 += sr.M4.Value;
                            TotM5 += sr.M5.Value;
                            TotM6 += sr.M6.Value;
                            Tot += sr.Amount.Value;

                            if (sr.Amount != 0) rv.Add(sr);                         

                        }

                        sr = new BLL.SalesReport();
                        sr.Description = "";
                        sr.M1 = TotM1;
                        sr.M2 = TotM2;
                        sr.M3 = TotM3;
                        sr.M4 = TotM4;
                        sr.M5 = TotM5;
                        sr.M6 = TotM6;
                        sr.Amount = Tot;
                        if (Tot != 0) rv.Add(sr);

                        sr = new BLL.SalesReport();
                        if (Tot != 0) rv.Add(sr);

                        if (Tot == 0)
                        {
                            rv.RemoveAt(rv.Count() - 1);
                        }
                        GTotM1 += TotM1;
                        GTotM2 += TotM2;
                        GTotM3 += TotM3;
                        GTotM4 += TotM4;
                        GTotM5 += TotM5;
                        GTotM6 += TotM6;
                        GTot += Tot;
                    }
                }

                

                sr = new BLL.SalesReport();
                if (sr.Amount != 0) rv.Add(sr);

                sr = new BLL.SalesReport();
                sr.Description = "Grand Total";
                sr.M1 = GTotM1;
                sr.M2 = GTotM2;
                sr.M3 = GTotM3;
                sr.M4 = GTotM4;
                sr.M5 = GTotM5;
                sr.M6 = GTotM6;
                sr.Amount = GTot;
                if (sr.Amount != 0) rv.Add(sr);


            }

            catch (Exception ex) { }


            return rv;

        }

        decimal GetSalesAmount(int CompanyId,int ProductId,DateTime dt)
        {
            decimal rv = 0;
            try
            {
                rv = DB.SalesDetails.Where(x => x.ProductId == ProductId && x.Sale.Ledger.AccountGroup.CompanyId == CompanyId && x.Sale.SalesDate.Month == dt.Month && x.Sale.SalesDate.Year == dt.Year).Sum(x => x.Amount);
            }catch(Exception ex) { }
            return rv;
        }
    }
}