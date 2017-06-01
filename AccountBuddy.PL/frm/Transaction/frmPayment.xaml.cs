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
            data.Clear();
          
            
        }

       

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (data.PDetail.LedgerId == 0)
            {
                MessageBox.Show("Enter LedgerName");
            }
            else if (data.PDetail.Amount == 0)
            {
                MessageBox.Show("Enter Amount");
            }
            else
            {
                data.SaveDetail();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (data.Id == 0 && !BLL.UserAccount.AllowInsert(FormName))
            {
                MessageBox.Show(string.Format(Message.PL.DenyInsert, FormName));
            }
            else if (data.Id != 0 && !BLL.UserAccount.AllowUpdate(FormName))
            {
                MessageBox.Show(string.Format(Message.PL.DenyUpdate, FormName));
            }
            else if (data.EntryNo == null)
            {
                MessageBox.Show("Enter Entry No");
            }
            else if (data.LedgerId == 0)
            {
                MessageBox.Show("Enter LedgerName");
            }
            else if (data.PaymentMode == null)
            {
                MessageBox.Show("select Paymode");
            }
            
            else if (data.PDetails.Count == 0)
            {
                MessageBox.Show("Enter Payment");
            }
            else if (data.FindEntryNo())
            {
                MessageBox.Show("Entry No Already Exist");
            }
            else
            {
                var rv = data.Save();
                if (rv == true)
                {

                    MessageBox.Show(Message.PL.Saved_Alert);
                    if (ckxAutoPrint.IsChecked == true) Print();
                    data.Clear();
                }
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (!BLL.UserAccount.AllowDelete(FormName))
            {
                MessageBox.Show(string.Format(Message.PL.DenyDelete, FormName));
            }

            else
            {
                if (MessageBox.Show("Do you want to delete?", "DELETE", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    var rv = data.Delete();
                    if (rv == true)
                    {
                        MessageBox.Show(Message.PL.Delete_Alert);
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
            var rv = data.Find();
            if (data.Id != 0) 
            {
                btnPrint.IsEnabled = true;
            }
            else
            {
                btnPrint.IsEnabled = false;
            }
            if (rv == false) MessageBox.Show(String.Format("Data Not Found"));
        }

        private void dgvDetails_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                BLL.PaymentDetail pod = dgvDetails.SelectedItem as BLL.PaymentDetail;
                pod.toCopy<BLL.PaymentDetail>(data.PDetail);
            }
            catch (Exception ex) { }

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
                data.FindDetail((int)btn.Tag);
            }
            catch (Exception ex) { }

        }

        private void btnDeleteDetail_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("do you want to delete this detail?", "Delete", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Button btn = (Button)sender;
                    data.DeleteDetail((int)btn.Tag);
                }
            }
            catch (Exception ex) { }

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

            cmbCreditAC.ItemsSource = BLL.Ledger.toList.Where(x => x.AccountGroup.GroupName == "Cash-in-Hand" || x.AccountGroup.GroupName == "Bank Accounts");
            cmbCreditAC.SelectedValuePath = "Id";
            cmbCreditAC.DisplayMemberPath = "AccountName";

            cmbDebitAC.ItemsSource = BLL.Ledger.toList.Where(x => x.AccountGroup.GroupName != "Primary" && x.AccountGroup.GroupName != "Cash-in-Hand" && x.AccountGroup.GroupName != "Bank Accounts");
            cmbDebitAC.SelectedValuePath = "Id";
            cmbDebitAC.DisplayMemberPath = "AccountName";

            btnSave.Visibility = (BLL.CompanyDetail.UserPermission.AllowInsert || BLL.CompanyDetail.UserPermission.AllowUpdate) ? Visibility.Visible : Visibility.Collapsed;
            btnDelete.Visibility = BLL.CompanyDetail.UserPermission.AllowDelete ? Visibility.Visible : Visibility.Collapsed;

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
