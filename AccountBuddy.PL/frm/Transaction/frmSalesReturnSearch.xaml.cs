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
    /// Interaction logic for frmSalesReturnSearch.xaml
    /// </summary>
    public partial class frmSalesReturnSearch : MetroWindow
    {
        decimal amtfrom = 0, amtTo = 99999999;

        public frmSalesReturnSearch()
        {
            InitializeComponent();
            dtpDateFrom.SelectedDate = DateTime.Now;
            dtpDateTo.SelectedDate = DateTime.Now;
        }

       

        private void dgvDetails_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var rp = dgvDetails.SelectedItem as BLL.SalesReturn;
            if (rp != null)
            {
                Transaction.frmSalesReturn f = App.frmHome.ShowForm(Common.Forms.frmSalesReturn) as Transaction.frmSalesReturn;

                System.Windows.Forms.Application.DoEvents();
                f.data.RefNo = rp.RefNo;
                f.data.SearchText = rp.RefNo;
                f.btnPrint.IsEnabled = true;
                f.data.Find();
                if (f.data.RefCode != null)
                {
                    f.btnSave.IsEnabled = true;
                    f.btnDelete.IsEnabled = true;
                }

                f.btnPrint.IsEnabled = true;

                System.Windows.Forms.Application.DoEvents();
                this.Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            try
            {
                var d = BLL.SalesReturn.ToList((int?)cmbCustomerName.SelectedValue, (int?)cmbTransactionType.SelectedValue, dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value, txtAmtFrom.Text, amtfrom, amtTo);
                dgvDetails.ItemsSource = d;
                lblTotal.Content = string.Format("Total :{0:N2}", d.Sum(x => x.TotalAmount));
            }
            catch (Exception ex)
            { }
        }

        private void cmbTransactionType_Loaded(object sender, RoutedEventArgs e)
        {
            cmbTransactionType.ItemsSource = BLL.TransactionType.toList;
            cmbTransactionType.SelectedValuePath = "Id";
            cmbTransactionType.DisplayMemberPath = "Type";
        }

        private void cmbCustomerName_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                cmbCustomerName.ItemsSource = BLL.Ledger.toList.Where(x => x.AccountGroup.GroupName == BLL.DataKeyValue.SundryDebtors_Key || x.AccountGroup.GroupName == BLL.DataKeyValue.BranchDivisions_Key).ToList(); ;
                cmbCustomerName.DisplayMemberPath = "LedgerName";
                cmbCustomerName.SelectedValuePath = "Id";
            }
            catch(Exception ex)
            { }

        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (txtAmtFrom.Text != "")
            {
                amtfrom = Convert.ToDecimal(txtAmtFrom.Text.ToString());
            }
            else
            {
                amtfrom = 0;
            }
            if (txtAmtTo.Text != "")
            {
                amtTo = Convert.ToDecimal(txtAmtTo.Text.ToString());
            }
            else
            {
                amtTo = 999999999;
            }
            var d = BLL.PurchaseReturn.ToList((int?)cmbCustomerName.SelectedValue, (int?)cmbTransactionType.SelectedValue, dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value, txtBillNo.Text, amtfrom, amtTo);
            dgvDetails.ItemsSource = d;
            lblTotal.Content = string.Format("Total :{0:N2}", d.Sum(x => x.TotalAmount));
        }

    }
}
