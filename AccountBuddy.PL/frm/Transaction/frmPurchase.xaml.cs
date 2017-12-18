using AccountBuddy.Common;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using AccountBuddy.BLL;
using Microsoft.AspNet.SignalR.Client;

namespace AccountBuddy.PL.frm.Transaction
{
    /// <summary>
    /// Interaction logic for frmPurchase.xaml
    /// </summary>
    public partial class frmPurchase : UserControl
    {
        public AccountBuddy.BLL.Purchase data = new BLL.Purchase();

        public string FormName = "Purchase";

        public frmPurchase()
        {
            InitializeComponent();
            this.DataContext = data;

            cmbPType.ItemsSource = BLL.TransactionType.toList;
            cmbPType.DisplayMemberPath = "Type";
            cmbPType.SelectedValuePath = "Id";

            data.Clear();
            data.setLabel();
            onClientEvents();


        }
        private void onClientEvents()
        {
            BLL.FMCGHubClient.HubCaller.On<String>("Purchase_RefNoRefresh", (RefNo) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    if (data.Id == 0) data.RefNo = RefNo;
                });
            });
        }

        #region  button Events

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (data.PDetail.ProductId == 0)
            {
                MessageBox.Show(string.Format(Message.PL.Empty_Record, "Product"), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbItem.Focus();
            }

            else
            {
                data.SaveDetail();
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            data.ClearDetail();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            data.Clear();
            data.setLabel();
            if (data.Id != 0)
            {
                btnPrint.IsEnabled = true;
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(string.Format(Message.PL.Delete_confirmation, data.RefNo), FormName, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var rv = data.Delete();
                if (rv == true)
                {
                    MessageBox.Show(string.Format(Message.PL.Delete_Alert), FormName, MessageBoxButton.OK, MessageBoxImage.Information);
                    data.Clear();
                }
            }

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (data.Id == 0 && !BLL.UserAccount.AllowInsert(Forms.frmPurchase))
            {
                MessageBox.Show(string.Format(Message.PL.DenyInsert, FormName), FormName.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (data.Id != 0 && !BLL.UserAccount.AllowUpdate(Forms.frmPurchase))
            {
                MessageBox.Show(string.Format(Message.PL.DenyUpdate, FormName), FormName.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (cmbPType.Text == "Cheque" && BLL.Bank.toList.Count == 0)
            {
                MessageBox.Show("Enter Bank Details for check Transaction", FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                App.frmHome.ShowBank();
            }
            else if (data.RefNo == null)
            {
                MessageBox.Show(string.Format(Message.PL.Transaction_POcode, "Invoice No"), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                txtRefNo.Focus();
            }
            else if (data.LedgerId == 0)
            {
                MessageBox.Show(string.Format(Message.PL.Transaction_Empty_Supplier), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbSupplier.Focus();
            }
            else if (data.PDetails.Count == 0)
            {
                MessageBox.Show(string.Format(Message.PL.Transaction_ItemDetails_Validation), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbItem.Focus();
            }
            else if (data.FindRefNo() == false)
            {

                var rv = data.Save();
                if (rv == true)
                {
                    MessageBox.Show(string.Format(Message.PL.Saved_Alert), FormName, MessageBoxButton.OK, MessageBoxImage.Information);
                    if (ckbAutoprint.IsChecked == true)
                    {
                        print();
                    }
                    data.Clear();

                    btnPrint.IsEnabled = false;

                }
            }
            else
            {
                MessageBox.Show(string.Format(Message.PL.Existing_Data, data.RefNo), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                txtRefNo.Focus();
            }
        }

        private void OnDelete(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = (Button)sender;
                data.DeleteDetail((int)btn.Tag);
            }
            catch (Exception ex) { }

        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            print();
        }

        private void btnsearch_Click(object sender, RoutedEventArgs e)
        {
            frmPurchaseSearch f = new frmPurchaseSearch();
            f.ShowDialog();
            f.Close();
        }

        #endregion

        #region Events 

        private void dgvDetails_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                BLL.PurchaseDetail pod = dgvDetails.SelectedItem as BLL.PurchaseDetail;
                pod.toCopy<BLL.PurchaseDetail>(data.PDetail);
            }
            catch (Exception ex) { }

        }

        private void txtBarCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && data.PDetail.ProductId != 0)
            {
                if (data.PDetail.ProductId == 0)
                {
                    MessageBox.Show(string.Format(Message.PL.Empty_Record, "Product"), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                    cmbItem.Focus();
                }

                else
                {
                    data.SaveDetail();
                    cmbItem.Focus();
                }
            }
        }

        #endregion

        #region Methods
        void print()
        {
            frm.Print.frmQuickPurchase f = new Print.frmQuickPurchase();
            f.LoadReport(data);
            f.ShowDialog();
        }

        void Clear()
        {
            data.Clear();
            btnSave.IsEnabled = true;
            btnDelete.IsEnabled = true;
        }
        #endregion

        #region Combo Box Load


        private void cmbSupplier_Loaded(object sender, RoutedEventArgs e)
        {
            cmbSupplier.ItemsSource = BLL.Ledger.toList.Where(x => x.AccountGroup.GroupName == BLL.DataKeyValue.SundryCreditors_Key || x.AccountGroup.GroupName == BLL.DataKeyValue.BranchDivisions_Key).ToList();
            cmbSupplier.DisplayMemberPath = "LedgerName";
            cmbSupplier.SelectedValuePath = "Id";
        }

        private void cmbItem_Loaded(object sender, RoutedEventArgs e)
        {
            cmbItem.ItemsSource = Product.toList.Where(x => x.StockGroup.IsPurchase != false).ToList();
            cmbItem.DisplayMemberPath = "ProductName";
            cmbItem.SelectedValuePath = "Id";
        }

        private void cmbUOM_Loaded(object sender, RoutedEventArgs e)
        {
            cmbUOM.ItemsSource = BLL.UOM.toList.ToList();
            cmbUOM.DisplayMemberPath = "Symbol";
            cmbUOM.SelectedValuePath = "Id";

        }
        #endregion

        #region textchanged

        private void txtDiscountAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Int32 selectionStart = textBox.SelectionStart;
            Int32 selectionLength = textBox.SelectionLength;
            textBox.Text = AppLib.NumericOnly(txtDiscountAmount.Text);
            textBox.SelectionStart = selectionStart <= textBox.Text.Length ? selectionStart : textBox.Text.Length;

        }

        private void txtExtraAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Int32 selectionStart = textBox.SelectionStart;
            Int32 selectionLength = textBox.SelectionLength;
            textBox.Text = AppLib.NumericOnly(txtExtraAmount.Text);
            textBox.SelectionStart = selectionStart <= textBox.Text.Length ? selectionStart : textBox.Text.Length;

        }
        private void txtRate_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Int32 selectionStart = textBox.SelectionStart;
            Int32 selectionLength = textBox.SelectionLength;
            textBox.Text = AppLib.NumericOnly(txtRate.Text);
            textBox.SelectionStart = selectionStart <= textBox.Text.Length ? selectionStart : textBox.Text.Length;

        }

        private void txtQty_TextChanged(object sender, TextChangedEventArgs e)
        {

            TextBox textBox = sender as TextBox;
            Int32 selectionStart = textBox.SelectionStart;
            Int32 selectionLength = textBox.SelectionLength;
            textBox.Text = AppLib.NumericOnly(txtQty.Text);
            textBox.SelectionStart = selectionStart <= textBox.Text.Length ? selectionStart : textBox.Text.Length;

        }

        private void txtDiscount_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Int32 selectionStart = textBox.SelectionStart;
            Int32 selectionLength = textBox.SelectionLength;
            textBox.Text = AppLib.NumericOnly(txtDiscount.Text);
            textBox.SelectionStart = selectionStart <= textBox.Text.Length ? selectionStart : textBox.Text.Length;

        }



        #endregion

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            LoadWindow();
            data.setLabel();
        }

        private void LoadWindow()
        {
            btnSave.Visibility = (BLL.Purchase.UserPermission.AllowInsert || BLL.Purchase.UserPermission.AllowUpdate) ? Visibility.Visible : Visibility.Collapsed;
            btnDelete.Visibility = BLL.Purchase.UserPermission.AllowDelete ? Visibility.Visible : Visibility.Collapsed;

        }

        private void txtChequeNo_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Int32 selectionStart = textBox.SelectionStart;
            Int32 selectionLength = textBox.SelectionLength;
            textBox.Text = AppLib.NumericOnly(txtChequeNo.Text);
            textBox.SelectionStart = selectionStart <= textBox.Text.Length ? selectionStart : textBox.Text.Length;

        }

        private void dgvDetails_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                BLL.PurchaseDetail pod = dgvDetails.SelectedItem as BLL.PurchaseDetail;
                pod.toCopy<BLL.PurchaseDetail>(data.PDetail);
            }
            catch (Exception ex) { }
        }
    }
}
