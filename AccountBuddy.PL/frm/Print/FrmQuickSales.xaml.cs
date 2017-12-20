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
using AccountBuddy.Common;

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


                rptQuickSales.Reset();
                ReportDataSource data1 = new ReportDataSource("Sale", POList);
                ReportDataSource data2 = new ReportDataSource("SalesDetail", GetDetails(data));
                ReportDataSource data3 = new ReportDataSource("CompanyDetail", CList);
                ReportDataSource data4 = new ReportDataSource("Ledger", CustomerList);


                rptQuickSales.LocalReport.DataSources.Add(data1);
                rptQuickSales.LocalReport.DataSources.Add(data2);
                rptQuickSales.LocalReport.DataSources.Add(data3);
                rptQuickSales.LocalReport.DataSources.Add(data4);
                rptQuickSales.LocalReport.ReportPath = @"rpt\Transaction\rptQuickSales.rdlc";

                ReportParameter[] rp = new ReportParameter[6];
                rp[0] = new ReportParameter("AmtPrefix", AppLib.CurrencyPositiveSymbolPrefix);
                rp[1] = new ReportParameter("ItemAmount", string.Format("{0} {1:N2}", AppLib.CurrencyPositiveSymbolPrefix, data.ItemAmount));
                rp[2] = new ReportParameter("DiscountAmount", string.Format("{0} {1:N2}", AppLib.CurrencyPositiveSymbolPrefix, data.DiscountAmount));
                rp[3] = new ReportParameter("Extra", string.Format("{0} {1:N2}", AppLib.CurrencyPositiveSymbolPrefix, data.ExtraAmount));
                rp[4] = new ReportParameter("GST", string.Format("{0} {1:N2}", AppLib.CurrencyPositiveSymbolPrefix, data.GSTAmount));
                rp[5] = new ReportParameter("BillAmount", string.Format("{0} {1:N2}", AppLib.CurrencyPositiveSymbolPrefix, data.TotalAmount));

                rptQuickSales.LocalReport.SetParameters(rp);


                rptQuickSales.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(SetSubDataSource);

                rptQuickSales.RefreshReport();

            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex);

            }
        }

        public void SetSubDataSource(object sender, SubreportProcessingEventArgs e)
        {
            e.DataSources.Add(new ReportDataSource("CompanyDetail", BLL.CompanyDetail.toList.Where(x => x.Id == BLL.UserAccount.User.UserType.Company.Id).ToList())); ;
        }
        public DataTable GetDetails(BLL.Sale data)
        {
            int NoRecPerPage = 21;
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
            if(NoRecPerPage<data.SDetails.Count)
            {
              
                for (int i = 0; i <29 ; i++)
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
