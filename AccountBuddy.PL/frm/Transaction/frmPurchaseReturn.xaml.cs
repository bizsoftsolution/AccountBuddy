using AccountBuddy.Common;
using Microsoft.AspNet.SignalR.Client;
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

namespace AccountBuddy.PL.frm.Transaction
{
    /// <summary>
    /// Interaction logic for frmPurchaseReturn.xaml
    /// </summary>
    public partial class frmPurchaseReturn : UserControl
    {
        public BLL.PurchaseReturn data = new BLL.PurchaseReturn();
        public string FormName = "Purchase Return";
        public frmPurchaseReturn()
        {
            InitializeComponent();
            this.DataContext = data;

            cmbPType.ItemsSource = BLL.TransactionType.toList;
            cmbPType.DisplayMemberPath = "Type";
            cmbPType.SelectedValuePath = "Id";

            data.Clear();

            onClientEvents();

        }
        private void onClientEvents()
        {
            BLL.FMCGHubClient.FMCGHub.On<String>("PurchaseReturn_RefNoRefresh", (RefNo) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    if (data.Id == 0) data.RefNo = RefNo;
                });
            });
        }
        #region Button Events

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            BLL.Product.Init();
            if (data.PRDetail.ProductId == 0)
            {
                MessageBox.Show(string.Format(Message.PL.Empty_Record, "Product"), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (BLL.Product.toList.Where(x => x.Id == data.PRDetail.ProductId).Select(x => x.AvailableStock).FirstOrDefault() < data.PRDetail.Quantity)
            {
                var v = BLL.Product.toList.Where(x => x.Id == data.PRDetail.ProductId).Select(x => x.AvailableStock).FirstOrDefault();
                MessageBox.Show(String.Format(Message.PL.Product_Available_Stock, v), FormName, MessageBoxButton.OK, MessageBoxImage.Error);
                txtQty.Focus();
            }
            else if (data.PRDetail.Particulars == null)
            {
                MessageBox.Show("Enter reason for return", FormName, MessageBoxButton.OK, MessageBoxImage.Error);
                txtParticulars.Focus();
            }
            else
            {
                data.SaveDetail();
                ckbIsReSale.IsChecked = false;
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            data.ClearDetail();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {

            data.Clear();
            if (data.Id != 0)
            {
                btnPrint.IsEnabled = true;
            }
            btnSave.IsEnabled = true;
            btnDelete.IsEnabled = true;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(string.Format(Message.PL.Delete_confirmation, data.RefNo), "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var rv = data.Delete();
                if (rv == true)
                {
                    MessageBox.Show(Message.PL.Delete_Alert, FormName, MessageBoxButton.OK, MessageBoxImage.Information);
                    data.Clear();
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (data.Id == 0 && !BLL.UserAccount.AllowInsert(Forms.frmPurchaseReturn))
            {
                MessageBox.Show(string.Format(Message.PL.DenyInsert, FormName), FormName.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (data.Id != 0 && !BLL.UserAccount.AllowUpdate(Forms.frmPurchaseReturn))
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
                MessageBox.Show(string.Format(Message.PL.Transaction_POcode, "SR Code"), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                txtRefNo.Focus();
            }
            else if (data.LedgerId == 0)
            {
                MessageBox.Show(string.Format(Message.PL.Transaction_Empty_Supplier), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbSupplier.Focus();
            }

            else if (data.PRDetails.Count == 0)
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

                    if (ckbAutoPrint.IsChecked == true)
                    {
                        Print();
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
            {
                try
                {
                    Button btn = (Button)sender;
                    data.DeleteDetail(btn.Tag.ToString());
                }
                catch (Exception ex) { }

            }
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            Print();
        }
        void Print()
        {
            frm.Print.frmQuickPReturn f = new Print.frmQuickPReturn();
            f.LoadReport(data);
            f.ShowDialog();
        }
        private void btnsearch_Click(object sender, RoutedEventArgs e)
        {
            frmPurchaseReturnSearch f = new frmPurchaseReturnSearch();
            f.ShowDialog();
            f.Close();
        }

        private void dgvDetails_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                BLL.PurchaseReturnDetail pod = dgvDetails.SelectedItem as BLL.PurchaseReturnDetail;
                pod.toCopy<BLL.PurchaseReturnDetail>(data.PRDetail);
            }
            catch (Exception ex) { }

        }

        private void txtBarCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && data.PRDetail.ProductId != 0)
            {
                if (data.PRDetail.ProductId == 0)
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

        #endregion


        private void cmbSupplier_Loaded(object sender, RoutedEventArgs e)
        {
            cmbSupplier.ItemsSource = BLL.Ledger.toList.Where(x => x.AccountGroup.GroupName == BLL.DataKeyValue.SundryCreditors_Key || x.AccountGroup.GroupName == BLL.DataKeyValue.BranchDivisions_Key).ToList();
            cmbSupplier.DisplayMemberPath = "LedgerName";
            cmbSupplier.SelectedValuePath = "Id";
        }

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

        private void cmbItem_Loaded(object sender, RoutedEventArgs e)
        {
            cmbItem.ItemsSource = BLL.Product.toList.Where(x => x.StockGroup.IsPurchase != false).ToList();
            cmbItem.DisplayMemberPath = "ProductName";
            cmbItem.SelectedValuePath = "Id";

        }

        private void cmbUOM_Loaded(object sender, RoutedEventArgs e)
        {
            cmbUOM.ItemsSource = BLL.UOM.toList.ToList();
            cmbUOM.DisplayMemberPath = "Symbol";
            cmbUOM.SelectedValuePath = "Id";

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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            data.lblDiscount = string.Format("{0}({1})", "Discount Amount", AppLib.CurrencyPositiveSymbolPrefix);
            data.lblExtra = string.Format("{0}({1})", "Extra Amount", AppLib.CurrencyPositiveSymbolPrefix);
            btnSave.Visibility = (BLL.PurchaseReturn.UserPermission.AllowInsert || BLL.PurchaseReturn.UserPermission.AllowUpdate) ? Visibility.Visible : Visibility.Collapsed;
            btnDelete.Visibility = BLL.PurchaseReturn.UserPermission.AllowDelete ? Visibility.Visible : Visibility.Collapsed;
        }

        private void txtChequeNo_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Int32 selectionStart = textBox.SelectionStart;
            Int32 selectionLength = textBox.SelectionLength;
            textBox.Text = AppLib.NumericOnly(txtChequeNo.Text);
            textBox.SelectionStart = selectionStart <= textBox.Text.Length ? selectionStart : textBox.Text.Length;

        }
        private void ckbIsReSale_Checked(object sender, RoutedEventArgs e)
        {
            data.PRDetail.IsResale = true;
        }

        private void ckbIsReSale_Unchecked(object sender, RoutedEventArgs e)
        {
            data.PRDetail.IsResale = false;
        }
    }
}
