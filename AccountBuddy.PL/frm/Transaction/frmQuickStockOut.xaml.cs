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
using AccountBuddy.Common;

namespace AccountBuddy.PL.frm.Transaction
{
    /// <summary>
    /// Interaction logic for frmQuickStockOut.xaml
    /// </summary>
    public partial class frmQuickStockOut : MetroWindow
    {
        public frmQuickStockOut()
        {
            InitializeComponent();
            rptViewer.SetDisplayMode(DisplayMode.PrintLayout);
        }
        public void LoadReport(BLL.StockOut data)
        {
            try
            {

                List<BLL.StockOut> STList = new List<BLL.StockOut>();
                List<BLL.StockOutDetail> STDList = new List<BLL.StockOutDetail>();
                List<BLL.CompanyDetail> CList = new List<BLL.CompanyDetail>();
                List<BLL.Ledger> SList = new List<BLL.Ledger>();

                STList.Add(data);
                STDList.AddRange(data.STOutDetails);
                CList.Add(BLL.UserAccount.User.UserType.Company);


                rptViewer.Reset();
                ReportDataSource data1 = new ReportDataSource("StockOut", STList);
                ReportDataSource data2 = new ReportDataSource("StockOutDetail", GetDetails(data));
                ReportDataSource data3 = new ReportDataSource("CompanyDetail", CList);
                ReportDataSource data4 = new ReportDataSource("Ledger", BLL.Ledger.toList.Where(x => x.Id == data.LedgerId).ToList());

                rptViewer.LocalReport.DataSources.Add(data1);
                rptViewer.LocalReport.DataSources.Add(data2);
                rptViewer.LocalReport.DataSources.Add(data3);
                rptViewer.LocalReport.DataSources.Add(data4);
                rptViewer.LocalReport.ReportPath = @"rpt\Transaction\rptStockOut.rdlc";

                ReportParameter[] rp = new ReportParameter[2];
                rp[0] = new ReportParameter("AmtPrefix", AppLib.CurrencyPositiveSymbolPrefix);
                rp[1] = new ReportParameter("ItemAmount", string.Format("{0} {1:N2}", AppLib.CurrencyPositiveSymbolPrefix, data.ItemAmount));
                rptViewer.LocalReport.SetParameters(rp);

                rptViewer.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(SetSubDataSource);

                rptViewer.RefreshReport();

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
        public DataTable GetDetails(BLL.StockOut data)
        {
            int NoRecPerPage = 22;
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
            foreach (var element in data.STOutDetails)
            {
                newRow = dt.NewRow();
                n = n + 1;
                newRow["ProductName"] = element.ProductName;
                newRow["Quantity"] = element.Quantity == 0 ? "" : element.Quantity.ToString();
                newRow["UnitPrice"] = element.UnitPrice == 0 ? "" : String.Format("{0:0.00}",element.UnitPrice
                    );
                newRow["UOMName"] = element.UOMName;
                newRow["Amount"] = String.Format("{0:0.00}", element.Amount);
                newRow["Id"] = n.ToString();
                newRow["DiscountAmount"] = element.DiscountAmount == 0 ? "" : String.Format("{0:0.00}", element.DiscountAmount);

                dt.Rows.Add(newRow);
            }
            if (NoRecPerPage < data.STOutDetails.Count)
            {

                for (int i = 0; i < 33; i++)
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

            for (int i = 0; i < NoRecPerPage - data.STOutDetails.Count(); i++)
            {
                newRow = dt.NewRow();

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
