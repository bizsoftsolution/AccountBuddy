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
using AccountBuddy.Common;
using Microsoft.AspNet.SignalR.Client;
using AccountBuddy.BLL;

namespace AccountBuddy.PL.frm.Transaction
{
    /// <summary>
    /// Interaction logic for frmPayment.xaml
    /// </summary>
    public partial class frmPayment : UserControl
    {
        public BLL.Payment data = new BLL.Payment();
        public string FormName = "Payment";
        public frmPayment()
        {
            InitializeComponent();
            this.DataContext = data;

            LoadWindow();
        }

        private void LoadWindow()
        {
            try
            {
                btnSave.Visibility = (BLL.Payment.UserPermission.AllowInsert || BLL.Payment.UserPermission.AllowUpdate) ? Visibility.Visible : Visibility.Collapsed;
                btnDelete.Visibility = BLL.Payment.UserPermission.AllowDelete ? Visibility.Visible : Visibility.Collapsed;

                data.Clear();
                btnPrint.IsEnabled = false;
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }

        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (data.PDetail.LedgerId == 0)
            {
                MessageBox.Show("Enter LedgerName", FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (data.PDetail.Amount == 0)
            {
                MessageBox.Show("Enter Amount", FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (data.PDetail.GSTStatusId == 0)
            {
                MessageBox.Show("Enter GST Type", FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbGST.Focus();
            }
            else
            {
                data.SaveDetail();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (data.Id == 0 && !BLL.UserAccount.AllowInsert(Forms.frmPayment))
            {
                MessageBox.Show(string.Format(Message.PL.DenyInsert, FormName), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (data.Id != 0 && !BLL.UserAccount.AllowUpdate(Forms.frmPayment))
            {
                MessageBox.Show(string.Format(Message.PL.DenyUpdate, FormName), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (data.EntryNo == null)
            {
                MessageBox.Show("Enter Entry No", FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (data.LedgerId == 0)
            {
                MessageBox.Show("Enter LedgerName", FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (data.PaymentMode == null)
            {
                MessageBox.Show("select Paymode", FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            else if (data.PDetails.Count == 0)
            {
                MessageBox.Show("Enter Payment", FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (data.FindEntryNo())
            {
                MessageBox.Show("Entry No Already Exist", FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            else
            {
                var rv = data.Save();
                if (rv == true)
                {

                    MessageBox.Show(Message.PL.Saved_Alert, FormName, MessageBoxButton.OK, MessageBoxImage.Information);
                    if (ckxAutoPrint.IsChecked == true) Print();
                    data.Clear();
                }
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (data.Id != 0)
            {
                if (!BLL.UserAccount.AllowDelete(FormName))
                {
                    MessageBox.Show(string.Format(Message.PL.DenyDelete, FormName), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                else
                {
                    if (MessageBox.Show("Do you want to delete?", FormName, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        var rv = data.Delete();
                        if (rv == true)
                        {
                            MessageBox.Show(Message.PL.Delete_Alert, FormName, MessageBoxButton.OK, MessageBoxImage.Information);
                            data.Clear();
                            if (data.Id != 0)
                            {
                                btnPrint.IsEnabled = true;
                            }
                            else
                            {
                                btnPrint.IsEnabled = false;
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show(Message.PL.No_Records_Delete, FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            data.Clear();
            if (data.Id != 0)
            {
                btnPrint.IsEnabled = true;

            }
            else
            {
                btnPrint.IsEnabled = false;
            }
        }

        private void btnsearch_Click(object sender, RoutedEventArgs e)
        {
            frmPaymentSearch f = new frmPaymentSearch();
            f.ShowDialog();
            f.Close();
        }

        private void dgvDetails_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                BLL.PaymentDetail pod = dgvDetails.SelectedItem as BLL.PaymentDetail;
                pod.ToMap(data.PDetail);
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }

        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            Print();
        }
        private void Print()
        {
            frm.Vouchers.frmQuickPayment f = new Vouchers.frmQuickPayment();

            f.LoadReport(data);
            f.ShowDialog();
        }
        private void btnEditDetail_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = (Button)sender;

                if ((int)btn.Tag != 0)
                {
                    data.FindDetail((int)btn.Tag);
                }
                else
                {
                    MessageBox.Show("Could Not Edit This Item", FormName, MessageBoxButton.OK, MessageBoxImage.Asterisk);
                }
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }

        }
        private void btnDeleteDetail_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                Button btn = (Button)sender;
                if ((int)btn.Tag != 0)
                {
                    if (MessageBox.Show("Do you want to delete this detail?", FormName, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        data.DeleteDetail((int)btn.Tag);
                    }
                }
                else
                {
                    MessageBox.Show("Could Not Delete This Item", FormName, MessageBoxButton.OK, MessageBoxImage.Asterisk);
                }
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }

        }
        private void btnDClear_Click(object sender, RoutedEventArgs e)
        {
            data.ClearDetail();
        }

        private void txtAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Int32 selectionStart = textBox.SelectionStart;
            Int32 selectionLength = textBox.SelectionLength;
            textBox.Text = AppLib.NumericOnly(txtAmount.Text);
            textBox.SelectionStart = selectionStart <= textBox.Text.Length ? selectionStart : textBox.Text.Length;

        }

        private void txtChequeNo_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Int32 selectionStart = textBox.SelectionStart;
            Int32 selectionLength = textBox.SelectionLength;
            textBox.Text = AppLib.NumericOnly(txtChequeNo.Text);
            textBox.SelectionStart = selectionStart <= textBox.Text.Length ? selectionStart : textBox.Text.Length;

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadWindow();
        }

        private void cmbCreditAC_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCreditAC();
        }

        private void cmbDebitAC_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDebitAc();

        }

        private void cmbDebitAC_DropDownOpened(object sender, EventArgs e)
        {
            LoadDebitAc();
        }

        private void LoadDebitAc()
        {
            try
            {
                BLL.Ledger.toList = null;
                cmbDebitAC.ItemsSource = BLL.Ledger.toList.Where(x => x.AccountGroup.GroupName != "Primary" && x.AccountGroup.GroupName != "Cash-in-Hand" && x.AccountGroup.GroupName != "Bank Accounts");
                cmbDebitAC.SelectedValuePath = "Id";
                cmbDebitAC.DisplayMemberPath = "AccountName";
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(string.Format("Payment Debit Ac List {0}-{1}", ex.Message, ex.InnerException));
            }
        }

        private void cmbCreditAC_DropDownOpened(object sender, EventArgs e)
        {
            LoadCreditAC();
        }

        private void LoadCreditAC()
        {
            try
            {
                cmbCreditAC.ItemsSource = BLL.Ledger.toList.Where(x => x.AccountGroup.GroupName == "Cash-in-Hand" || x.AccountGroup.GroupName == "Bank Accounts");
                cmbCreditAC.SelectedValuePath = "Id";
                cmbCreditAC.DisplayMemberPath = "AccountName";
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(string.Format("Payment Credit Ac List {0}-{1}", ex.Message, ex.InnerException));
            }
        }

        private void dgvDetails_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                BLL.PaymentDetail pod = dgvDetails.SelectedItem as BLL.PaymentDetail;
                pod.ToMap(data.PDetail);
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
        }

        private void cmbGST_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                cmbGST.ItemsSource = BLL.TaxType.toList;
                cmbGST.SelectedValuePath = "Id";
                cmbGST.DisplayMemberPath = "Type";
            }
            catch (Exception ex)
            { Common.AppLib.WriteLog(ex); }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                frmTaxList frm = new frmTaxList();
                frm.dgvTax.ItemsSource = data.PDetail.TaxDetails;
                frm.ItemAmount = data.PDetail.Amount;
                frm.lblItemAmount.Content = "Amount";
                frm.lblDiscountAmount.Visibility = Visibility.Hidden;
                frm.lblHDiscountAmount.Visibility = Visibility.Hidden;
                frm.ShowDialog();
                data.PDetail.SetGST();
                frm.Close();
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex);
            }
        }

        private void cmbGST_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var i = cmbGST.SelectedItem as BLL.TaxType;

            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
        }
    }
}
