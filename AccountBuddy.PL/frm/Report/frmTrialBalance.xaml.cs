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
                ReportDataSource data = new ReportDataSource("TrialBalance", BLL.TrialBalance.toList);
                ReportDataSource data1 = new ReportDataSource("CompanyDetail", BLL.CompanyDetail.toList.Where(x => x.Id == BLL.UserAccount.Company.Id).ToList());
                rptTrialBalance.LocalReport.DataSources.Add(data);
                rptTrialBalance.LocalReport.DataSources.Add(data1);
                rptTrialBalance.LocalReport.ReportPath = @"rpt\master\rptTrialBalance.rdlc";

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
            DateTime dtFrom = dtpDateFrom.SelectedDate.Value;
            DateTime dtTo = dtpDateTo.SelectedDate.Value;

            dgvTrialBalance.ItemsSource = BLL.TrialBalance.toList.Where(x => x.VoucherPayDate >= dtFrom && x.VoucherPayDate <= dtTo || x.VoucherRecDate >= dtFrom && x.VoucherRecDate <= dtTo);

        }
    }
}
