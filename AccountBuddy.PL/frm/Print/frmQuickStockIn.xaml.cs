using System;
using System.Collections.Generic;
using System.Data;
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
using MahApps.Metro.Controls;
using Microsoft.Reporting.WinForms;

namespace AccountBuddy.PL.frm.Print
{
    /// <summary>
    /// Interaction logic for frmQuickStockIn.xaml
    /// </summary>
    public partial class frmQuickStockIn: MetroWindow
    {
        public frmQuickStockIn()
        {
            InitializeComponent();
            rptViewer.SetDisplayMode(DisplayMode.PrintLayout);
        }
        public void LoadReport(BLL.StockIn data)
        {
            try
            {

                List<BLL.StockIn> STList = new List<BLL.StockIn>();
                List<BLL.StockInDetail> STDList = new List<BLL.StockInDetail>();
                List<BLL.CompanyDetail> CList = new List<BLL.CompanyDetail>();
                List<BLL.Ledger> SList = new List<BLL.Ledger>();

                STList.Add(data);
                STDList.AddRange(data.STInDetails);
                CList.Add(BLL.UserAccount.User.UserType.Company);


                rptViewer.Reset();
                ReportDataSource data1 = new ReportDataSource("StockIn", STList);
                ReportDataSource data2 = new ReportDataSource("StockInDetail", GetDetails(data));
                ReportDataSource data3 = new ReportDataSource("CompanyDetail", CList);
                ReportDataSource data4 = new ReportDataSource("Ledger", BLL.Ledger.toList.Where(x => x.Id == data.LedgerId).ToList());

                rptViewer.LocalReport.DataSources.Add(data1);
                rptViewer.LocalReport.DataSources.Add(data2);
                rptViewer.LocalReport.DataSources.Add(data3);
                rptViewer.LocalReport.DataSources.Add(data4);
                rptViewer.LocalReport.ReportPath = @"rpt\Transaction\rptStockIn.rdlc";

                rptViewer.RefreshReport();

            }
            catch (Exception ex)
            {

            }
        }
        public DataTable GetDetails(BLL.StockIn data)
        {
            int NoRecPerPage = 20;
            var dataSet = new DataSet();
            DataTable dt = new DataTable();
            dataSet.Tables.Add(dt);

            dt.Columns.Add("ProductName");
            dt.Columns.Add("Quantity");
            dt.Columns.Add("UnitPrice");
            dt.Columns.Add("UOMName");
            dt.Columns.Add("Amount");
            dt.Columns.Add("Id");

            var newRow = dt.NewRow();


            int n = 0;
            foreach (var element in data.STInDetails)
            {
                newRow = dt.NewRow();
                n = n + 1;
                // fill the properties into the cells
                newRow["ProductName"] = element.ProductName;
                newRow["Quantity"] = element.Quantity == 0 ? "" : element.Quantity.ToString();
                newRow["UnitPrice"] = element.UnitPrice == 0 ? "" : element.UnitPrice.ToString();
                newRow["UOMName"] = element.UOMName;
                newRow["Amount"] = element.Amount;
                newRow["Id"] = n.ToString();

                dt.Rows.Add(newRow);
            }

            newRow = dt.NewRow();
            newRow["ProductName"] = string.Format("----------- No Of Products {0}----------", data.STInDetails.Count());
            newRow["Quantity"] = "";
            newRow["UnitPrice"] = "";
            newRow["Amount"] = "";
            newRow["Id"] = "";
            dt.Rows.Add(newRow);

            for (int i = 0; i < NoRecPerPage - data.STInDetails.Count(); i++)
            {
                newRow = dt.NewRow();

                // fill the properties into the cells
                newRow["ProductName"] = "";
                newRow["Quantity"] = "";
                newRow["UnitPrice"] = "";
                newRow["Amount"] = "";
                newRow["Id"] = "";

                dt.Rows.Add(newRow);
            }
            return dt;

        }

    }
}
