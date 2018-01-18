using MahApps.Metro.Controls;
using Microsoft.Reporting.WinForms;
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

namespace AccountBuddy.PL.frm.Transaction
{
    /// <summary>
    /// Interaction logic for frmPaymentSearch.xaml
    /// </summary>
    public partial class frmPaymentSearch : MetroWindow
    {
        public frmPaymentSearch()
        {
            InitializeComponent();
            rptViewer.SetDisplayMode(DisplayMode.PrintLayout);
          
           dtpDateFrom.SelectedDate = DateTime.Now;
            dtpDateTo.SelectedDate = DateTime.Now;


        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            cmbLedgerName.ItemsSource = BLL.Ledger.toList.ToList();
            cmbLedgerName.DisplayMemberPath = "LedgerName";
            cmbLedgerName.SelectedValuePath = "Id";
            LoadWindow();
        }

        private void LoadReport()
        {
            try
            {
                List<BLL.Payment> list = dgvDetail.ItemsSource as List<BLL.Payment>;

                list = list.Select(x => new BLL.Payment()
                { LedgerName = x.LedgerName, Amount = x.Amount, PaymentDate = x.PaymentDate, EntryNo = x.EntryNo, PaymentMode = x.PaymentMode }).ToList();

                try
                {
                    rptViewer.Reset();
                    ReportDataSource data = new ReportDataSource("Payment", list);
                    ReportDataSource data1 = new ReportDataSource("CompanyDetail", BLL.CompanyDetail.toList.Where(x => x.Id == BLL.UserAccount.User.UserType.CompanyId).ToList());
                    rptViewer.LocalReport.DataSources.Add(data);
                    rptViewer.LocalReport.DataSources.Add(data1);
                    rptViewer.LocalReport.ReportPath = @"rpt\Transaction\rptPaymentReport.rdlc";


                    rptViewer.RefreshReport();

                }
                catch (Exception ex)
                {

                }
            }
            catch (Exception ex)
            {

            }

        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadWindow();
        }

        private void LoadWindow()
        {
            int? c = null;
            string p;
            if (cmbLedgerName.SelectedValue != null)
            {
                c = (int)cmbLedgerName.SelectedValue;
            }
            else
            {
                c = null;
            }
            if (cmbType.SelectedValue != null)

            {
                p = cmbType.Text.ToString();
            }
            else
            {
                p = null;
            }
            dgvDetail.ItemsSource = BLL.Payment.List(c, dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value, txtEntryNo.Text, p);
            LoadReport();

        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //LoadReport();
        }

       

        private void dgvDetail_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var rp = dgvDetail.SelectedItem as BLL.Payment;
            if (rp != null)
            {

                Transaction.frmPayment f = new Transaction.frmPayment();
                App.frmHome.ShowForm(f);
                System.Windows.Forms.Application.DoEvents();
                f.data.EntryNo = rp.EntryNo;

                System.Windows.Forms.Application.DoEvents();
                f.data.Find();

                f.btnPrint.IsEnabled = true;
                this.Close();

            }
        }

        private void cmbLedgerName_Loaded(object sender, RoutedEventArgs e)
        {
            cmbLedgerName.ItemsSource = BLL.Ledger.toList.ToList();
            cmbLedgerName.DisplayMemberPath = "LedgerName";
            cmbLedgerName.SelectedValuePath = "Id";

        }
    }
}
