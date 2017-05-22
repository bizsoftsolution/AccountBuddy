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
    /// Interaction logic for frmPaymentReceipt.xaml
    /// </summary>
    public partial class frmPaymentReceipt : UserControl
    {
        public frmPaymentReceipt()
        {
            InitializeComponent();
            rptPaymentReceipt.SetDisplayMode(DisplayMode.PrintLayout);

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            
        }


        private void LoadReport()
        {
            try
            {
                rptPaymentReceipt.Reset();
                ReportDataSource data = new ReportDataSource("PaymentReceipt", dgvPaymentReceipt.ItemsSource);
                ReportDataSource data1 = new ReportDataSource("CompanyDetail", BLL.CompanyDetail.toList.Where(x => x.Id == BLL.UserAccount.Company.Id).ToList());
                rptPaymentReceipt.LocalReport.DataSources.Add(data);
                rptPaymentReceipt.LocalReport.DataSources.Add(data1);
                rptPaymentReceipt.LocalReport.ReportPath = @"rpt\Report\rptPaymentReceipt.rdlc";

                ReportParameter[] par = new ReportParameter[2];
                par[0] = new ReportParameter("DateFrom", dtpDateFrom.SelectedDate.Value.ToString());
                par[1] = new ReportParameter("DateTo", dtpDateTo.SelectedDate.Value.ToString());
                rptPaymentReceipt.LocalReport.SetParameters(par);


                rptPaymentReceipt.RefreshReport();

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

            //if (dtpDateFrom.SelectedDate != null && dtpDateTo.SelectedDate != null)
            //{
            //    DateTime dtFrom = dtpDateFrom.SelectedDate.Value;
            //    DateTime dtTo = dtpDateTo.SelectedDate.Value;

            //    var list1 = BLL.TrialBalance.toList.Where(x => x.VoucherPayDate >= dtFrom || x.VoucherRecDate >= dtFrom).ToList();
            //    var list2 = list1.Where(x => x.VoucherPayDate <= dtTo || x.VoucherRecDate <= dtTo).ToList();
            //    dgvPaymentReceipt.ItemsSource = list2;
            //}
            //else
            //{
            //    dgvPaymentReceipt.ItemsSource = BLL.TrialBalance.toList;
            //}

        }

    }
}
