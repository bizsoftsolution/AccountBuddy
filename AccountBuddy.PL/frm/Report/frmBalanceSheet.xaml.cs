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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Reporting.WinForms;

namespace AccountBuddy.PL.frm.Report
{
    /// <summary>
    /// Interaction logic for frmBalanceSheet.xaml
    /// </summary>
    public partial class frmBalanceSheet : UserControl
    {
        public frmBalanceSheet()
        {
            InitializeComponent();
            rptBalanceSheet.SetDisplayMode(DisplayMode.PrintLayout);

            int yy = BLL.UserAccount.User.UserType.Company.LoginAccYear;

            DateTime? dtFrom = new DateTime(yy, 4, 1);
            DateTime? dtTo = new DateTime(yy + 1, 3, 31);

            dtpDateFrom.SelectedDate = dtFrom;
            dtpDateTo.SelectedDate = dtTo;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dgvBalanceSheet.ItemsSource = BLL.BalanceSheet.ToList(dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value);
        }


        private void LoadReport()
        {
            List<BLL.BalanceSheet> list = BLL.BalanceSheet.ToList(dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value);
            list = list.Select(x => new BLL.BalanceSheet()
            { Ledger = x.LedgerList.AccountName, CrAmt = x.CrAmt, DrAmt = x.DrAmt, CrAmtOP = x.CrAmtOP, DrAmtOP = x.DrAmtOP }).ToList();
            try
            {
                rptBalanceSheet.Reset();
                ReportDataSource data = new ReportDataSource("BalanceSheet", list);
                ReportDataSource data1 = new ReportDataSource("CompanyDetail", BLL.CompanyDetail.toList.Where(x => x.Id == BLL.UserAccount.User.UserType.Company.Id).ToList());
                rptBalanceSheet.LocalReport.DataSources.Add(data);
                rptBalanceSheet.LocalReport.DataSources.Add(data1);
                rptBalanceSheet.LocalReport.ReportPath = @"rpt\Report\rptBalanceSheet.rdlc";

                ReportParameter[] par = new ReportParameter[2];
                par[0] = new ReportParameter("DateFrom", dtpDateFrom.Text);
                par[1] = new ReportParameter("DateTo", dtpDateTo.Text);
                rptBalanceSheet.LocalReport.SetParameters(par);


                rptBalanceSheet.RefreshReport();

            }
            catch (Exception ex)
            {

            }


        }


        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabControl tc = sender as TabControl;

            if (tc.SelectedIndex == 1)
            {
                LoadReport();
            }

        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            dgvBalanceSheet.ItemsSource = BLL.BalanceSheet.ToList(dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value);
        }
    }
}
