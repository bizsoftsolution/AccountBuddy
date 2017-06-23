using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        public List<BLL.SalesReport> SalesReport_ListCustomerWise(DateTime dt)
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
                foreach (var wh in lstWarehouse)
                {
                    var lstDealer = DB.CompanyDetails.Where(x => x.UnderCompanyId == wh.Id).ToList();
                    foreach(var dl in lstDealer)
                    {

                        sr = new BLL.SalesReport();
                        sr.Description = dl.CompanyName;
                        rv.Add(sr);
                        decimal TotM1=0, TotM2 = 0, TotM3 = 0, TotM4 = 0, TotM5 = 0, TotM6 = 0, Tot = 0;

                        foreach (var p in lstProduct)
                        {
                            sr = new BLL.SalesReport();

                            sr.Description =String.Format("  {0}", p.ProductName);

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

                            if (sr.Amount!=0) rv.Add(sr);
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
                        if(Tot!=0) rv.Add(sr);

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
                if(sr.Amount!=0) rv.Add(sr);


            }

            catch(Exception ex) { }
            


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