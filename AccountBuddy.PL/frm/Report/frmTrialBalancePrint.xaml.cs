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
    /// Interaction logic for frmTrialBalancePrint.xaml
    /// </summary>
    public partial class frmTrialBalancePrint : MetroWindow
    {
        public static int yy = BLL.UserAccount.User.UserType.Company.LoginAccYear;

        DateTime? dtFrom = new DateTime(yy, 4, 1);
        DateTime? dtTo = new DateTime(yy + 1, 3, 31);

        public frmTrialBalancePrint()
        {
            InitializeComponent();
            rptViewer.SetDisplayMode(DisplayMode.PrintLayout);

            LoadReport(Convert.ToDateTime(dtFrom), Convert.ToDateTime(dtTo)); ;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadReport(Convert.ToDateTime(dtFrom), Convert.ToDateTime(dtTo)); ;

        }

        public void LoadReport(DateTime dtFrom, DateTime dtTo)
        {
            List<BLL.TrialBalance> list = BLL.TrialBalance.ToList(dtFrom, dtTo);
            list = list.Select(x => new BLL.TrialBalance()
            { AccountName = x.Ledger.AccountName, CrAmt = x.CrAmt, DrAmt = x.DrAmt, CrAmtOP = x.CrAmtOP, DrAmtOP = x.DrAmtOP }).ToList();

            try
            {
                rptViewer.Reset();
                ReportDataSource data = new ReportDataSource("TrialBalance", list);
                ReportDataSource data1 = new ReportDataSource("CompanyDetail", BLL.CompanyDetail.toList.Where(x => x.Id == BLL.UserAccount.User.UserType.Company.Id).ToList());
                rptViewer.LocalReport.DataSources.Add(data);
                rptViewer.LocalReport.DataSources.Add(data1);
                rptViewer.LocalReport.ReportPath = @"rpt\Report\rptTrialBalance.rdlc";

                ReportParameter[] par = new ReportParameter[2];
                par[0] = new ReportParameter("DateFrom", dtFrom.ToString());
                par[1] = new ReportParameter("DateTo", dtTo.ToString());
                rptViewer.LocalReport.SetParameters(par);

                rptViewer.RefreshReport();

            }
            catch (Exception ex)
            {

            }


        }

    }
}
