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
    /// Interaction logic for frmStockOut.xaml
    /// </summary>
    public partial class frmStockOut : UserControl
    {
        public AccountBuddy.BLL.StockOut data = new BLL.StockOut();

        public string FormName = "Stock Outward";

        public frmStockOut()
        {
            InitializeComponent();
            this.DataContext = data;
            data.Clear();
           onClientEvents();
        }
        private void onClientEvents()
        {
            BLL.FMCGHubClient.HubCaller.On<String>("StockOut_RefNoRefresh", (RefNo) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    if (data.Id == 0) data.RefNo = RefNo;
                });
            });
        }

        #region Events

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var max = data.STOutDetail.Product.MaxSellingRate;
            var min = data.STOutDetail.Product.MinSellingRate;
            var av = data.STOutDetail.Product.AvailableStock;
            if (data.STOutDetail.ProductId == 0)
            {
                MessageBox.Show(string.Format(Message.PL.Empty_Record, "Product"), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbItem.Focus();
            }
            else if (av < data.STOutDetail.Quantity)
            {

                MessageBox.Show(String.Format(Message.PL.Product_Available_Stock, av), FormName, MessageBoxButton.OK, MessageBoxImage.Error);
                txtQty.Focus();
            }
            else if (min > data.STOutDetail.UnitPrice || max < data.STOutDetail.UnitPrice)
            {
                MessageBox.Show(String.Format(Message.PL.Transaction_Selling_Rate, min, max), FormName, MessageBoxButton.OK, MessageBoxImage.Error);
                txtRate.Focus();
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
            clear();
        }
        void clear()
        {
            data.Clear();
            btnPrint.IsEnabled = false;
            btnSave.IsEnabled = true;
            btnDelete.IsEnabled = true;
        }
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(string.Format(Message.PL.Delete_confirmation, data.RefNo), FormName, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var rv = data.Delete();
                if (rv == true)
                {
                    MessageBox.Show(Message.PL.Delete_Alert, FormName, MessageBoxButton.OK, MessageBoxImage.Information);
                    data.Clear(); clear();
                }
            }

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (data.Id == 0 && !BLL.UserAccount.AllowInsert(Forms.frmStockOut))
            {
                MessageBox.Show(string.Format(Message.PL.DenyInsert, FormName), FormName.ToString(), MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (data.Id != 0 && !BLL.UserAccount.AllowUpdate(Forms.frmStockOut))
            {
                MessageBox.Show(string.Format(Message.PL.DenyUpdate, FormName), FormName.ToString(), MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (data.RefNo == null)
            {
                MessageBox.Show("Enter Reference No", FormName, MessageBoxButton.YesNo, MessageBoxImage.Warning);
            }
            else if (data.LedgerName == null)
            {
                MessageBox.Show("Enter Supplier", FormName, MessageBoxButton.YesNo, MessageBoxImage.Warning);
            }
            else if (data.STOutDetails.Count == 0)
            {
                MessageBox.Show("Enter Product Details", FormName, MessageBoxButton.YesNo, MessageBoxImage.Warning);
            }
            else if (data.FindRefNo() == false)
            {
                data.Type = "Outward";
                var rv = data.Save();
                if (rv == true)
                {
                    MessageBox.Show(Message.PL.Saved_Alert, FormName, MessageBoxButton.YesNo, MessageBoxImage.Information);
                    if (ckbAutoPrint.IsChecked==true)
                    {
                        Print();
                    }
                    data.Clear();
                    clear();
                }
            }
            else
            {
                MessageBox.Show(string.Format(Message.PL.Existing_Data, data.RefNo), FormName, MessageBoxButton.YesNo, MessageBoxImage.Information);

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
            Print();
        }
        void Print()
        {
            frmQuickStockOut f = new frmQuickStockOut();
            f.LoadReport(data);
            f.ShowDialog();
        }
        private void btnsearch_Click(object sender, RoutedEventArgs e)
        {
            frmStockOutwardSearch f = new frmStockOutwardSearch();
            f.ShowDialog();
            f.Close();
        }

        private void dgvDetails_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                BLL.StockOutDetail pod = dgvDetails.SelectedItem as BLL.StockOutDetail;
                pod.toCopy<BLL.StockOutDetail>(data.STOutDetail);
            }
            catch (Exception ex) { }

        }

        private void txtBarCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && data.STOutDetail.ProductId != 0)
            {
                var max = data.STOutDetail.Product.MaxSellingRate;
                var min = data.STOutDetail.Product.MinSellingRate;

                if (data.STOutDetail.ProductId == 0)
                {
                    MessageBox.Show(string.Format(Message.PL.Empty_Record, "Product"), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                    cmbItem.Focus();
                }
                else if (min > data.STOutDetail.UnitPrice || max < data.STOutDetail.UnitPrice)
                {
                    MessageBox.Show(String.Format(Message.PL.Transaction_Selling_Rate, min, max), FormName, MessageBoxButton.OK, MessageBoxImage.Error);
                    txtRate.Focus();
                }
                else
                {
                    data.SaveDetail();
                }
            }
        }


        #endregion

        #region Methods

        #endregion

        private void cmbCustomer_Loaded(object sender, RoutedEventArgs e)
        {
            cmbCustomer.ItemsSource = BLL.Ledger.toList.Where(x => x.AccountGroup.GroupName == BLL.DataKeyValue.SundryDebtors_Key || x.AccountGroup.GroupName == BLL.DataKeyValue.BranchDivisions_Key).ToList();
            cmbCustomer.DisplayMemberPath = "LedgerName";
            cmbCustomer.SelectedValuePath = "Id";
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            btnSave.Visibility = (BLL.StockOut.UserPermission.AllowInsert || BLL.StockOut.UserPermission.AllowUpdate) ? Visibility.Visible : Visibility.Collapsed;
            btnDelete.Visibility = BLL.StockOut.UserPermission.AllowDelete ? Visibility.Visible : Visibility.Collapsed;
      
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

        private void cmbItem_Loaded(object sender, RoutedEventArgs e)
        {
            cmbItem.ItemsSource = BLL.Product.toList.ToList();
            cmbItem.DisplayMemberPath = "ProductName";
            cmbItem.SelectedValuePath = "Id";
        }

        private void cmbUOM_Loaded(object sender, RoutedEventArgs e)
        {
            cmbUOM.ItemsSource = BLL.UOM.toList.ToList();
            cmbUOM.DisplayMemberPath = "Symbol";
            cmbUOM.SelectedValuePath = "Id";
        }

        private void dgvDetails_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                BLL.StockOutDetail pod = dgvDetails.SelectedItem as BLL.StockOutDetail;
                pod.toCopy<BLL.StockOutDetail>(data.STOutDetail);
            }
            catch (Exception ex) { }
        }
    }
}
