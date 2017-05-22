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
    /// Interaction logic for frmProfitLoss.xaml
    /// </summary>
    public partial class frmProfitLoss : UserControl
    {
        public frmProfitLoss()
        {
            InitializeComponent();
            rptProfitLoss.SetDisplayMode(DisplayMode.PrintLayout);

            int yy = BLL.UserAccount.Company.LoginAccYear;

            DateTime? dtFrom = new DateTime(yy, 4, 1);
            DateTime? dtTo = new DateTime(yy + 1, 3, 31);

            dtpDateFrom.SelectedDate = dtFrom;
            dtpDateTo.SelectedDate = dtTo;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dgvProfitLoss.ItemsSource = BLL.ProfitLoss.ToList(dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value);
        }


        private void LoadReport()
        {
            try
            {
                rptProfitLoss.Reset();
                ReportDataSource data = new ReportDataSource("ProfitLoss", dgvProfitLoss.ItemsSource);
                ReportDataSource data1 = new ReportDataSource("CompanyDetail", BLL.CompanyDetail.toList.Where(x => x.Id == BLL.UserAccount.Company.Id).ToList());
                rptProfitLoss.LocalReport.DataSources.Add(data);
                rptProfitLoss.LocalReport.DataSources.Add(data1);
                rptProfitLoss.LocalReport.ReportPath = @"rpt\Report\rptProfitLoss.rdlc";

                ReportParameter[] par = new ReportParameter[2];
                par[0] = new ReportParameter("DateFrom", dtpDateFrom.SelectedDate.Value.ToString());
                par[1] = new ReportParameter("DateTo", dtpDateTo.SelectedDate.Value.ToString());
                rptProfitLoss.LocalReport.SetParameters(par);


                rptProfitLoss.RefreshReport();

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
            dgvProfitLoss.ItemsSource = BLL.ProfitLoss.ToList(dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value);
        }

    }
}
