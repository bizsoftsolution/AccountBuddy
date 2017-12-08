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

namespace AccountBuddy.PL.frm.Transaction
{
    /// <summary>
    /// Interaction logic for frmJournalSearch.xaml
    /// </summary>
    public partial class frmJournalSearch : MetroWindow
    {
        decimal amtFrom = 0, amtTo = 99999999;
        string status = null;
        public frmJournalSearch()
        {
            InitializeComponent();
           
            dtpDateFrom.SelectedDate = DateTime.Now;
            dtpDateTo.SelectedDate = DateTime.Now;

       
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cmbAccountName.ItemsSource = BLL.Ledger.toList.ToList();
            cmbAccountName.DisplayMemberPath = "AccountName";
            cmbAccountName.SelectedValuePath = "Id";

            var d = BLL.Journal.ToList((int?)cmbAccountName.SelectedValue, dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value, status, txtEntryNo.Text, amtFrom, amtTo);
            dgvDetails.ItemsSource = d;
            lblTotal.Content = string.Format("Total CrAmt: {0:N2}, DrAmt: {1:N2}", d.Sum(x => x.JDetail.CrAmt), d.Sum(x => x.JDetail.DrAmt));
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if(cmbstatus.Text!="")
            {
                status = cmbstatus.Text;
            }
            else
            {
                status = null;
            }
           
            if (txtAmtFrom.Text != "")
            {
                amtFrom = Convert.ToDecimal(txtAmtFrom.Text.ToString());
            }
            else
            {
                amtFrom = 0;
            }
            if (txtAmtTo.Text != "")
            {
                amtTo = Convert.ToDecimal(txtAmtTo.Text.ToString());
            }
            else
            {
                amtTo = 99999999;
            }

            var d = BLL.Journal.ToList((int?)cmbAccountName.SelectedValue, dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value, txtEntryNo.Text, status, amtFrom, amtTo);

            dgvDetails.ItemsSource = d;
            lblTotal.Content = string.Format("Total CrAmt: {0:N2}, DrAmt: {1:N2}", d.Sum(x => x.JDetail.CrAmt), d.Sum(x => x.JDetail.DrAmt));
        }
        private void dgvDetails_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var rp = dgvDetails.SelectedItem as BLL.Journal;
            if (rp != null)
            {
                Transaction.frmJournal f = App.frmHome.ShowForm(Common.Forms.frmJournal) as Transaction.frmJournal;

                System.Windows.Forms.Application.DoEvents();
                f.data.EntryNo = rp.EntryNo;
                f.btnPrint.IsEnabled = true;

                f.data.Find();
                if (f.data.RefCode != null)
                {
                    f.btnSave.IsEnabled = true;
                    f.btnDelete.IsEnabled = true;
                }
                System.Windows.Forms.Application.DoEvents();
                this.Close();
            }
        }

    }
}
