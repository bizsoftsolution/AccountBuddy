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
using MahApps.Metro.Controls;
using Microsoft.Reporting.WinForms;

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
                ReportDataSource data2 = new ReportDataSource("StockOutDetail", STDList);
                ReportDataSource data3 = new ReportDataSource("CompanyDetail", CList);
                ReportDataSource data4 = new ReportDataSource("Ledger", BLL.Ledger.toList.Where(x => x.Id == data.LedgerId).ToList());

                rptViewer.LocalReport.DataSources.Add(data1);
                rptViewer.LocalReport.DataSources.Add(data2);
                rptViewer.LocalReport.DataSources.Add(data3);
                rptViewer.LocalReport.DataSources.Add(data4);
                rptViewer.LocalReport.ReportPath = @"rpt\Transaction\rptStockOut.rdlc";

                rptViewer.RefreshReport();

            }
            catch (Exception ex)
            {

            }
        }

    }
}
