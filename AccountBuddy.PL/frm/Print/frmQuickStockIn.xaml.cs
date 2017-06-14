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
                ReportDataSource data2 = new ReportDataSource("StockInDetail", STDList);
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

    }
}
