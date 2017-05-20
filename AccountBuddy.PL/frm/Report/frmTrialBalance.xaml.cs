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
    /// Interaction logic for frmTrialBalance.xaml
    /// </summary>
    public partial class frmTrialBalance : UserControl
    {
        public frmTrialBalance()
        {
            InitializeComponent();
            rptTrialBalance.SetDisplayMode(DisplayMode.PrintLayout);

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dgvTrialBalance.ItemsSource = BLL.TrialBalance.toList;
        }


        private void LoadReport()
        {
            try
            {
                rptTrialBalance.Reset();
                ReportDataSource data = new ReportDataSource("TrialBalance", dgvTrialBalance.ItemsSource);
                ReportDataSource data1 = new ReportDataSource("CompanyDetail", BLL.CompanyDetail.toList.Where(x => x.Id == BLL.UserAccount.Company.Id).ToList());
                rptTrialBalance.LocalReport.DataSources.Add(data);
                rptTrialBalance.LocalReport.DataSources.Add(data1);
                rptTrialBalance.LocalReport.ReportPath = @"rpt\Report\rptTrialBalance.rdlc";

                ReportParameter[] par = new ReportParameter[2];
                par[0] = new ReportParameter("DateFrom", dtpDateFrom.SelectedDate.Value.ToString());
                par[1] = new ReportParameter("DateTo", dtpDateTo.SelectedDate.Value.ToString());
                rptTrialBalance.LocalReport.SetParameters(par);


                rptTrialBalance.RefreshReport();

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

            if (dtpDateFrom.SelectedDate != null && dtpDateTo.SelectedDate != null)
            {
                DateTime dtFrom = dtpDateFrom.SelectedDate.Value;
                DateTime dtTo = dtpDateTo.SelectedDate.Value;

                var list1 = BLL.TrialBalance.toList.Where(x => x.VoucherPayDate >= dtFrom || x.VoucherRecDate >= dtFrom).ToList();
                var list2 = list1.Where(x => x.VoucherPayDate <= dtTo || x.VoucherRecDate <= dtTo).ToList();
                dgvTrialBalance.ItemsSource = list2;
            }
            else
            {
                dgvTrialBalance.ItemsSource = BLL.TrialBalance.toList;
            }

        }
    }
}
