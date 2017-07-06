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

namespace AccountBuddy.PL.frm.Report
{
    /// <summary>
    /// Interaction logic for frmCustomerWiseSalesReport.xaml
    /// </summary>
    public partial class frmCustomerWiseSalesReport : MetroWindow
    {

        public frmCustomerWiseSalesReport()
        {
            InitializeComponent();
            RptViewer.SetDisplayMode(DisplayMode.PrintLayout);


        }
        public void LoadReport(DateTime dt)
        {
            try
            {
                //    List<BLL.SalesReport> list = BLL.SalesReport.ToListCustomerWise( dt);

                //    try
                //    {
                //        RptViewer.Reset();
                //        ReportDataSource data = new ReportDataSource("SalesReport", list);
                //        ReportDataSource data1 = new ReportDataSource("CompanyDetail", BLL.CompanyDetail.toList.Where(x => x.Id == BLL.UserAccount.User.UserType.CompanyId).ToList());
                //        RptViewer.LocalReport.DataSources.Add(data);
                //        RptViewer.LocalReport.DataSources.Add(data1);
                //        RptViewer.LocalReport.ReportPath = @"rpt\Report\rptCustomerWiseSalesReport.rdlc";

                //        ReportParameter[] par = new ReportParameter[7];
                //        par[0] = new ReportParameter("Month1", string.Format("{0:MMMM}", dt.AddMonths(-5)));
                //        par[1] = new ReportParameter("Month2", string.Format("{0:MMMM}", dt.AddMonths(-4)));
                //        par[2] = new ReportParameter("Month3", string.Format("{0:MMMM}", dt.AddMonths(-3)));
                //        par[3] = new ReportParameter("Month4", string.Format("{0:MMMM}", dt.AddMonths(-2)));
                //        par[4] = new ReportParameter("Month5", string.Format("{0:MMMM}", dt.AddMonths(-1)));
                //        par[5] = new ReportParameter("Month6", string.Format("{0:MMMM}", dt));

                //        par[6] = new ReportParameter("Title", "Customer Wise Sales Report");

                //        RptViewer.LocalReport.SetParameters(par);

                //        RptViewer.RefreshReport();

            }
            catch (Exception ex)
            {

            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }

}


