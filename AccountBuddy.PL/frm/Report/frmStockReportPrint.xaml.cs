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

namespace AccountBuddy.PL.frm.Report
{
    /// <summary>
    /// Interaction logic for frmStockReportPrint.xaml
    /// </summary>
    public partial class frmStockReportPrint : MetroWindow
    {


        public frmStockReportPrint()
        {
            InitializeComponent();
            RptViewer.SetDisplayMode(DisplayMode.PrintLayout);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {


        }
        public void LoadReport(List<BLL.StockReport> list, DateTime dtFrom, DateTime dtTo)
        {
            try
            {
                RptViewer.Reset();
                  ReportDataSource data = new ReportDataSource("StockReport", list);
                    ReportDataSource data1 = new ReportDataSource("CompanyDetail", BLL.CompanyDetail.ToList.Where(x => x.Id == BLL.UserAccount.User.UserType.Company.Id).ToList());
                RptViewer.LocalReport.DataSources.Add(data);
                RptViewer.LocalReport.DataSources.Add(data1);
                RptViewer.LocalReport.ReportPath = @"rpt\Report\rptStock.rdlc";

                ReportParameter[] rp = new ReportParameter[2];
                rp[0] = new ReportParameter("DateFrom", dtFrom.ToString());
                rp[1] = new ReportParameter("DateTo", dtTo.ToString());
                RptViewer.LocalReport.SetParameters(rp);


                RptViewer.RefreshReport();

            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex);
            }

        }
        public void SetSubDataSource(object sender, SubreportProcessingEventArgs e)
        {
            e.DataSources.Add(new ReportDataSource("CompanyDetail", BLL.CompanyDetail.ToList.Where(x => x.Id == BLL.UserAccount.User.UserType.Company.Id).ToList())); ;
        }
    }
}

