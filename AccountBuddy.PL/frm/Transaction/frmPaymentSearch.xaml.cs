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
    /// Interaction logic for frmPaymentSearch.xaml
    /// </summary>
    public partial class frmPaymentSearch : MetroWindow
    {
        decimal amtfrom = 0, amtTo = 99999999;
        public frmPaymentSearch()
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

          

            var d = BLL.Payment.ToList((int?)cmbAccountName.SelectedValue, dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value, txtEntryNo.Text, cmbstatus.Text, amtfrom, amtTo);
            dgvDetails.ItemsSource = d;
            lblTotal.Content = string.Format("Total :{0:N2}", d.Sum(x => x.Amount));
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
           
            if (txtAmtFrom.Text != "")
            {
                amtfrom = Convert.ToDecimal(txtAmtFrom.Text.ToString());
            }
            if (txtAmtTo.Text != "")
            {
                amtTo = Convert.ToDecimal(txtAmtTo.Text.ToString());
            }
            var d = BLL.Payment.ToList((int?)cmbAccountName.SelectedValue,dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value, txtEntryNo.Text, cmbstatus.Text, amtfrom, amtTo);
            dgvDetails.ItemsSource = d;
            lblTotal.Content = string.Format("Total :{0:N2}", d.Sum(x => x.Amount));
        }
        private void dgvDetails_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var rp = dgvDetails.SelectedItem as BLL.Payment;
            if (rp != null)
            {
                Transaction.frmPayment f = App.frmHome.ShowForm(Common.Forms.frmPayment) as Transaction.frmPayment;

                System.Windows.Forms.Application.DoEvents();
                f.data.EntryNo = rp.EntryNo;
                f.data.Find();
                f.btnPrint.IsEnabled = true;
               
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
