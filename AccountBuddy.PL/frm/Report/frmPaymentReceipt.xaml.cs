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
            int yy = BLL.UserAccount.Company.LoginAccYear;

            DateTime? dtFrom = new DateTime(yy, 4, 1);
            DateTime? dtTo = new DateTime(yy + 1, 3, 31);

            dtpDateFrom.SelectedDate = dtFrom;
            dtpDateTo.SelectedDate = dtTo;

            cmbAccountName.ItemsSource = BLL.Ledger.toList;
            cmbAccountName.DisplayMemberPath = "AccountName";
            cmbAccountName.SelectedValuePath = "Id";
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }


        private void LoadReport()
        {
            try
            {
                List<BLL.ReceiptAndPayment> list = BLL.ReceiptAndPayment.ToList((int?)cmbAccountName.SelectedValue, dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value);
                list = list.Select(x => new BLL.ReceiptAndPayment()
                { AccountName = x.Ledger.AccountName, Amount = x.Amount , EDate=x.EDate, EntryNo=x.EntryNo, EType=x.EType, Ledger=x.Ledger, RefNo=x.RefNo}).ToList();

                try
                {
                    rptPaymentReceipt.Reset();
                    ReportDataSource data = new ReportDataSource("PaymentAndReceipt", list);
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
            catch(Exception ex)
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
            dgvReceiptAndPayment.ItemsSource = BLL.ReceiptAndPayment.ToList((int?)cmbAccountName.SelectedValue, dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value);
        }


    }
}
