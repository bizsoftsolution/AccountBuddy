using MahApps.Metro.Controls;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data;

namespace AccountBuddy.PL.frm.Print
{
    /// <summary>
    /// Interaction logic for FrmQuickSales.xaml
    /// </summary>
    public partial class FrmQuickSales : MetroWindow
    {
        public FrmQuickSales()
        {
            InitializeComponent();
            rptQuickSales.SetDisplayMode(DisplayMode.PrintLayout);
        }
        public void LoadReport(BLL.Sale data)
        {
            try
            {

                List<BLL.Sale> POList = new List<BLL.Sale>();
                List<BLL.SalesDetail> PODList = new List<BLL.SalesDetail>();
                List<BLL.CompanyDetail> CList = new List<BLL.CompanyDetail>();
                List<BLL.Ledger> CustomerList = new List<BLL.Ledger>();

                POList.Add(data);
                PODList.AddRange(data.SDetails);
                CList.Add(BLL.UserAccount.User.UserType.Company);
                CustomerList.Add(BLL.Ledger.toList.Where(x => x.Id == data.LedgerId).FirstOrDefault());


                var b = BLL.CompanyDetail.toList.Select(x => new BLL.CompanyDetail() { CompanyName = x.CompanyName, Title1 = x.Title1, Title2 = x.Title2, ContactPerson = x.ContactPerson, AddressLine1 = x.AddressLine1, AddressLine2 = x.AddressLine2,CityName=x.CityName, StateName = x.State.StateName,GSTNo=x.GSTNo, Banner1 = x.Banner1, Banner2 = x.Banner2, Logo = x.Logo, MobileNo = x.MobileNo, TelephoneNo = x.TelephoneNo, PostalCode = x.PostalCode }).ToList() ;
                var l= BLL.Ledger.toList.Where(x => x.Id == data.LedgerId).Select(x => new BLL.Ledger() { LedgerName = x.LedgerName, StateName = x.State==null?null: x.State.StateName, StateCode =x.State==null?null:x.State.TINNo, CityName=x.CityName, AccountGroup=x.AccountGroup }).ToList();
               // CustomerList.Add(l);

                rptQuickSales.Reset();
                ReportDataSource data1 = new ReportDataSource("Sale", POList);
                ReportDataSource data2 = new ReportDataSource("SalesDetail", GetDetails(data));
                ReportDataSource data3 = new ReportDataSource("CompanyDetail", b);
                ReportDataSource data4 = new ReportDataSource("Ledger", l);

                rptQuickSales.LocalReport.DataSources.Add(data1);
                rptQuickSales.LocalReport.DataSources.Add(data2);
                rptQuickSales.LocalReport.DataSources.Add(data3);
                rptQuickSales.LocalReport.DataSources.Add(data4);

                rptQuickSales.LocalReport.ReportPath = @"rpt\Transaction\rptQuickSales.rdlc";

                ReportParameter[] rp = new ReportParameter[5];
                rp[0] = new ReportParameter("SalesType", data.TransactionType);
                rp[1] = new ReportParameter("IGST", Common.AppLib.IGSTPer.ToString());
                rp[2] = new ReportParameter("SGST", Common.AppLib.SGSTPer.ToString());
                rp[3] = new ReportParameter("CGST", Common.AppLib.CGSTPer.ToString());
                rp[4] = new ReportParameter("CompanyName", BLL.UserAccount.User.UserType.Company.CompanyName);
                rptQuickSales.LocalReport.SetParameters(rp);

                rptQuickSales.RefreshReport();

            }
            catch (Exception ex)
            {

            }
        }
        public DataTable GetDetails(BLL.Sale data)
        {
            int NoRecPerPage = 13;
            var dataSet = new DataSet();
            DataTable dt = new DataTable();
            dataSet.Tables.Add(dt);

            dt.Columns.Add("ProductName");
            dt.Columns.Add("Quantity");
            dt.Columns.Add("UnitPrice");
            dt.Columns.Add("UOMName");
            dt.Columns.Add("Amount");
            dt.Columns.Add("Id");
            dt.Columns.Add("DiscountAmount");

            var newRow = dt.NewRow();


            int n = 0;
            foreach (var element in data.SDetails)
            {
                newRow = dt.NewRow();
                n = n + 1;
                newRow["ProductName"] = element.ProductName;
                newRow["Quantity"] = element.Quantity == 0 ? "" : element.Quantity.ToString();
                newRow["UnitPrice"] = element.UnitPrice == 0 ? "" : String.Format("{0:0.00}", element.UnitPrice);
                newRow["UOMName"] = element.UOMName;
                newRow["Amount"] = String.Format("{0:0.00}", element.Amount);
                newRow["Id"] = n.ToString();
                newRow["DiscountAmount"] = element.DiscountAmount == 0 ? "" : String.Format("{0:0.00}", element.DiscountAmount);

                dt.Rows.Add(newRow);
            }



            for (int i = 0; i < NoRecPerPage - data.SDetails.Count(); i++)
            {
                newRow = dt.NewRow();

                // fill the properties into the cells
                newRow["ProductName"] = "";
                newRow["Quantity"] = "";
                newRow["UnitPrice"] = "";
                newRow["Amount"] = "";
                newRow["Id"] = "";
                newRow["DiscountAmount"] = "";

                dt.Rows.Add(newRow);
            }
            return dt;

        }
    }
}
