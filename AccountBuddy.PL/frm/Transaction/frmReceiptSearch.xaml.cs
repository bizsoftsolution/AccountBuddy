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
    /// Interaction logic for frmReceiptSearch.xaml
    /// </summary>
    public partial class frmReceiptSearch : MetroWindow
    {
        
        public frmReceiptSearch()
        {
            InitializeComponent();
            rptViewer.SetDisplayMode(DisplayMode.PrintLayout);
           
            dtpDateFrom.SelectedDate = DateTime.Now;
            dtpDateTo.SelectedDate = DateTime.Now; 
            
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadWindow();
        }

        private void LoadReport()
        {
            try
            {
                List<BLL.Receipt> list = dgvDetail.ItemsSource as List<BLL.Receipt>;

                list = list.Select(x => new BLL.Receipt()
                { LedgerName = x.LedgerName, Amount = x.Amount, ReceiptDate = x.ReceiptDate,      EntryNo = x.EntryNo, ReceiptMode = x.ReceiptMode }).ToList();

                try
                {
                    rptViewer.Reset();
                    ReportDataSource data = new ReportDataSource("Receipt", list);
                    ReportDataSource data1 = new ReportDataSource("CompanyDetail", BLL.CompanyDetail.toList.Where(x => x.Id == BLL.UserAccount.User.UserType.CompanyId).ToList());
                    rptViewer.LocalReport.DataSources.Add(data);
                    rptViewer.LocalReport.DataSources.Add(data1);
                    rptViewer.LocalReport.ReportPath = @"rpt\Transaction\rptReceiptReport.rdlc";


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
            string p ;
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
            dgvDetail.ItemsSource = BLL.Receipt.List(c, dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value, txtEntryNo.Text, p);
            LoadReport();

        }
        
        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //LoadReport();
        }
        
        private void cmbType_Loaded(object sender, RoutedEventArgs e)
        {
            cmbType.ItemsSource = BLL.TransactionType.toList;
            cmbType.DisplayMemberPath = "Type";
            cmbType.SelectedValuePath = "Id";

        }

        private void dgvDetail_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var rp = dgvDetail.SelectedItem as BLL.Receipt;
            if (rp != null)
            {

                Transaction.frmReceipt f = new Transaction.frmReceipt();
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
