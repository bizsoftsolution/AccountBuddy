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
    /// Interaction logic for frmStockInProcessSearch.xaml
    /// </summary>
    public partial class frmStockInProcessSearch : MetroWindow
    {
        decimal amtfrom = 0, amtTo = 99999999;

        public frmStockInProcessSearch()
        {
            InitializeComponent();
            dtpDateFrom.SelectedDate = DateTime.Now;
            dtpDateTo.SelectedDate = DateTime.Now;
        }

        private void cmbStaff_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                cmbStaff.ItemsSource = BLL.Staff.toList.Where(x => x.Ledger.AccountGroup.CompanyId == BLL.UserAccount.User.UserType.CompanyId).ToList();
                cmbStaff.DisplayMemberPath = "Ledger.LedgerName";
                cmbStaff.SelectedValuePath = "Id";
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(string.Format("Stock In Process Staff List_{0}_{1}", ex.Message, ex.InnerException));
            }
        }
        private void dgvDetails_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var rp = dgvDetails.SelectedItem as BLL.StockInProcess;
            if (rp != null)
            {
                Transaction.frmStockInProcess f = App.frmHome.ShowForm(Common.Forms.frmStockInProcess) as Transaction.frmStockInProcess;

                System.Windows.Forms.Application.DoEvents();
                f.data.RefNo = rp.RefNo;
                f.data.SearchText = rp.RefNo;
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var d = BLL.StockInProcess.ToList((int?)cmbStaff.SelectedValue, dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value, txtAmtFrom.Text, amtfrom, amtTo);
                dgvDetails.ItemsSource = d;
                lblTotal.Content = string.Format("Total :{0:N2}", d.Sum(x => x.ItemAmount));
            }
            catch (Exception ex)
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
            var d = BLL.StockInProcess.ToList((int?)cmbStaff.SelectedValue, dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value, txtBillNo.Text, amtfrom, amtTo);
            dgvDetails.ItemsSource = d;
            lblTotal.Content = string.Format("Total :{0:N2}", d.Sum(x => x.ItemAmount));
        }
    }
}
