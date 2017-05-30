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
                MessageBox.Show("Enter LedgerName");
            }
            else if (data.RDetail.Amount == 0)
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
            else if(data.EntryNo == null)
            {
                MessageBox.Show("Enter Entry No");
            }
            else if (data.LedgerId == 0)
            {
                MessageBox.Show("Enter LedgerName");
            }
            else if (data.ReceiptMode == null)
            {
                MessageBox.Show("select Paymode");
            }

            else if (data.RDetails.Count == 0)
            {
                MessageBox.Show("Enter Receipt");
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
                        MessageBox.Show("Deleted");
                        data.Clear();
                    }
                }
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            data.Clear();
        }

        private void btnsearch_Click(object sender, RoutedEventArgs e)
        {
            var rv = data.Find();
            if (rv == false) MessageBox.Show(String.Format("Data Not Found"));
        }

        private void dgvDetails_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                BLL.ReceiptDetail pod = dgvDetails.SelectedItem as BLL.ReceiptDetail;
                pod.toCopy<BLL.ReceiptDetail>(data.RDetail);
            }
            catch (Exception ex) { }

        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
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
            btnSave.Visibility = (BLL.CompanyDetail.UserPermission.AllowInsert || BLL.CompanyDetail.UserPermission.AllowUpdate) ? Visibility.Visible : Visibility.Collapsed;
            btnDelete.Visibility = BLL.CompanyDetail.UserPermission.AllowDelete ? Visibility.Visible : Visibility.Collapsed;

            data.Clear();
        }
    }
}
