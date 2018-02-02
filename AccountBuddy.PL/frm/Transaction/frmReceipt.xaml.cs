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
    /// Interaction logic for frmReceipt.xaml
    /// </summary>
    public partial class frmReceipt : UserControl
    {
        public BLL.Receipt data = new BLL.Receipt();
        public String FormName = "Receipt";
        public frmReceipt()
        {
            InitializeComponent();
            this.DataContext = data;
            data.Clear();            
        }
        
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (data.RDetail.LedgerId == 0)
            {
                MessageBox.Show("Enter LedgerName", FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (data.RDetail.Amount == 0)
            {
                MessageBox.Show("Enter Amount", FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                data.SaveDetail();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (data.Id == 0 && !BLL.UserAccount.AllowInsert(Forms.frmReceipt))
            {
                MessageBox.Show(string.Format(Message.PL.DenyInsert, FormName), FormName.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (data.Id != 0 && !BLL.UserAccount.AllowUpdate(Forms.frmReceipt))
            {
                MessageBox.Show(string.Format(Message.PL.DenyUpdate, FormName), FormName.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
            else if(data.EntryNo == null)
            {
                MessageBox.Show("Enter Entry No", FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (data.LedgerId == 0)
            {
                MessageBox.Show("Enter LedgerName", FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (data.ReceiptMode == null)
            {
                MessageBox.Show("select Paymode", FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            else if (data.RDetails.Count == 0)
            {
                MessageBox.Show("Enter Receipt", FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
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
                    MessageBox.Show(string.Format(Message.PL.DenyDelete, FormName));
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

            frmReceiptSearch f = new frmReceiptSearch();
            f.ShowDialog();
            f.Close();
        }

        private void dgvDetails_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                BLL.ReceiptDetail pod = dgvDetails.SelectedItem as BLL.ReceiptDetail;
                pod.ToMap(data.RDetail);
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }

        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {

            Print();
        }

        private void Print()
        {

            frm.Vouchers.frmQuickReceipt f = new Vouchers.frmQuickReceipt();

            f.LoadReport(data);
            f.ShowDialog();
        }
        private void btnEditDetail_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = (Button)sender;
                data.FindDetail((int)btn.Tag);
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }

        }

        private void btnDeleteDetail_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Do you want to delete this detail?",  FormName, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    Button btn = (Button)sender;
                    data.DeleteDetail((int)btn.Tag);
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



            btnSave.Visibility = (BLL.Receipt.UserPermission.AllowInsert || BLL.Receipt.UserPermission.AllowUpdate) ? Visibility.Visible : Visibility.Collapsed;
            btnDelete.Visibility = BLL.Receipt.UserPermission.AllowDelete ? Visibility.Visible : Visibility.Collapsed;

            data.Clear();

            if(data.Id!=0)
            {
                btnPrint.IsEnabled = true;
            }
            else
            {
                btnPrint.IsEnabled = false;
            }
            
        }

        private void cmbReceiptMode_Loaded(object sender, RoutedEventArgs e)
        {

          
        }

        private void cmbDebitAC_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDebitAC();
        }

        private void LoadDebitAC()
        {
            try
            {
                cmbDebitAC.ItemsSource = BLL.Ledger.toList.Where(x => x.AccountGroup.GroupName == "Cash-in-Hand" || x.AccountGroup.GroupName == "Bank Accounts");
                cmbDebitAC.SelectedValuePath = "Id";
                cmbDebitAC.DisplayMemberPath = "AccountName";
            }
            catch(Exception ex)
            {
                Common.AppLib.WriteLog(string.Format("Receipt Combo Box {0}-{1}", ex.Message, ex.InnerException));
            }
        }

        private void cmbCreditAC_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCreditAC();
        }

        private void LoadCreditAC()
        {
            try
            {
                cmbCreditAC.ItemsSource = BLL.Ledger.toList.Where(x => x.AccountGroup.GroupName != "Primary" && x.AccountGroup.GroupName != "Cash-in-Hand" && x.AccountGroup.GroupName != "Bank Accounts");
                cmbCreditAC.SelectedValuePath = "Id";
                cmbCreditAC.DisplayMemberPath = "AccountName";
            }
            catch(Exception ex)
            {
                Common.AppLib.WriteLog(string.Format("Receipt Credit Ac_{0}_{1}", ex.Message, ex.InnerException));
            }
        }

        private void cmbDebitAC_DropDownOpened(object sender, EventArgs e)
        {
            LoadDebitAC();

        }

        private void ckbIsGST_Unchecked(object sender, RoutedEventArgs e)
        {
            data.RDetail.IncludingGST = false;
        }

        private void ckbIsGST_Checked(object sender, RoutedEventArgs e)
        {
            data.RDetail.IncludingGST = true;
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                frmTaxList frm = new frmTaxList();
                frm.dgvTax.ItemsSource = data.RDetail.TaxDetails;
                frm.Amount = data.RDetail.Amount;
                frm.ShowDialog();
                data.RDetail.SetGST();
                frm.Close();
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex);
            }
        }
    }
}
