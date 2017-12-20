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
    /// Interaction logic for frmReceiptSearch.xaml
    /// </summary>
    public partial class frmReceiptSearch : MetroWindow
    {
        decimal amtFrom = 0, amtTo = 99999999;
        public frmReceiptSearch()
        {
            InitializeComponent();
      
            dtpDateFrom.SelectedDate = DateTime.Now;
            dtpDateTo.SelectedDate = DateTime.Now;
         
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           

            var d = BLL.Receipt.ToList((int?)cmbAccountName.SelectedValue,  dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value, txtEntryNo.Text, cmbstatus.Text, amtFrom, amtTo);
            dgvDetails.ItemsSource = d;
            lblTotal.Content = string.Format("Total :{0:N2}", d.Sum(x => x.Amount).ToString());
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            
            if (txtAmtFrom.Text != "")
            {
                amtFrom = Convert.ToDecimal(txtAmtFrom.Text.ToString());
            }
            if (txtAmtTo.Text != "")
            {
                amtTo = Convert.ToDecimal(txtAmtTo.Text.ToString());
            }
            var d = BLL.Receipt.ToList((int?)cmbAccountName.SelectedValue,  dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value, txtEntryNo.Text, cmbstatus.Text, amtFrom, amtTo);
            //dgvDetails.ItemsSource = BLL.Receipt.ToList((int?)cmbAccountName.SelectedValue, dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value, cmbstatus.Text);
            dgvDetails.ItemsSource = d;
            lblTotal.Content = string.Format("Total :{0:N2}", d.Sum(x => x.Amount));
        }

        private void cmbAccountName_Loaded(object sender, RoutedEventArgs e)
        {

            cmbAccountName.ItemsSource = BLL.Ledger.toList.ToList();
            cmbAccountName.DisplayMemberPath = "AccountName";
            cmbAccountName.SelectedValuePath = "Id";

        }

        private void dgvDetails_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var rp = dgvDetails.SelectedItem as BLL.Receipt;
            if (rp != null)
            {
                Transaction.frmReceipt f = App.frmHome.ShowForm(Common.Forms.frmReceipt) as frmReceipt;
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
