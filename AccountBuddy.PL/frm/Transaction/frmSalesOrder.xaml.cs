using AccountBuddy.Common;
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
    /// Interaction logic for frmSalesOrder.xaml
    /// </summary>
    public partial class frmSalesOrder : UserControl
    {
        public BLL.SalesOrder data = new BLL.SalesOrder();
        public string FormName = "Sales Order";

        public frmSalesOrder()
        {
            InitializeComponent();
            this.DataContext = data;

            data.Clear();

        }

        #region Button Events


        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var max = BLL.Product.toList.Where(x => x.Id == data.SODetail.ProductId).Select(x => x.MaxSellingRate).FirstOrDefault();
            var min = BLL.Product.toList.Where(x => x.Id == data.SODetail.ProductId).Select(x => x.MinSellingRate).FirstOrDefault();

            if (data.SODetail.ProductId == null)
            {
                MessageBox.Show(string.Format(Message.PL.Empty_Record, "Product"), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (min > data.SODetail.UnitPrice || max < data.SODetail.UnitPrice)
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
            data.Clear();
            btnMakesales.IsEnabled = false;
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
            if (data.RefNo == null)
            {
                MessageBox.Show(string.Format(Message.PL.Transaction_POcode, "SO Code"), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                txtRefNo.Focus();
            }
            else if (data.LedgerId == 0)
            {
                MessageBox.Show(string.Format(Message.PL.Transaction_Empty_Customer), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbCustomer.Focus();
            }

            else if (data.SODetails.Count == 0)
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
                    data.Clear();
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
                data.DeleteDetail(btn.Tag.ToString());
            }
            catch (Exception ex) { }

        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            frm.Print.frmQuickSO f = new Print.frmQuickSO();
            f.LoadReport(data);
            f.ShowDialog();
        }

        private void btnsearch_Click(object sender, RoutedEventArgs e)
        {
            var rv = data.Find();
            if(data.Id!=0)
            {
                btnMakesales.IsEnabled = data.Status== "Pending" ? true: false;
            }
            if (rv == false) MessageBox.Show(string.Format(Message.PL.Transaction_Not_Fount, data.SearchText), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void btnMakesales_Click(object sender, RoutedEventArgs e)
        {
            if (data.MakeSales())
            {
                MessageBox.Show(string.Format(Message.PL.Transaction_Make_Sales), FormName, MessageBoxButton.OK, MessageBoxImage.Information);
                data.Clear();
                btnMakesales.IsEnabled = false;
            }
        }

        #endregion 

        #region events

        private void dgvDetails_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                BLL.SalesOrderDetail pod = dgvDetails.SelectedItem as BLL.SalesOrderDetail;
                pod.toCopy<BLL.SalesOrderDetail>(data.SODetail);
            }
            catch (Exception ex) { }

        }

        private void txtBarCode_KeyDown(object sender, KeyEventArgs e)
        {
           
            if (e.Key == Key.Return && data.SODetail.ProductId != null)
            {
                var max = BLL.Product.toList.Where(x => x.Id == data.SODetail.ProductId).Select(x => x.MaxSellingRate).FirstOrDefault();
                var min = BLL.Product.toList.Where(x => x.Id == data.SODetail.ProductId).Select(x => x.MinSellingRate).FirstOrDefault();
                if (data.SODetail.ProductId == 0)
                {
                    MessageBox.Show(string.Format(Message.PL.Empty_Record, "Product"), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                    cmbItem.Focus();
                }
                else if (min > data.SODetail.UnitPrice || max < data.SODetail.UnitPrice)
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

        #region Textchanged
        private void txtDiscountAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Int32 selectionStart = textBox.SelectionStart;
            Int32 selectionLength = textBox.SelectionLength;
            textBox.Text = AppLib.NumericOnly(txtdiscountAmount.Text);
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

        private void txtdiscountAmount_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Int32 selectionStart = textBox.SelectionStart;
            Int32 selectionLength = textBox.SelectionLength;
            textBox.Text = AppLib.NumericOnly(txtdiscountAmount.Text);
            textBox.SelectionStart = selectionStart <= textBox.Text.Length ? selectionStart : textBox.Text.Length;

        }

        #endregion

        #region combo box loading
        private void cmbItem_Loaded(object sender, RoutedEventArgs e)
        {
            cmbItem.ItemsSource = BLL.Product.toList;
            cmbItem.DisplayMemberPath = "ProductName";
            cmbItem.SelectedValuePath = "Id";

        }

        private void cmbUOM_Loaded(object sender, RoutedEventArgs e)
        {

            cmbUOM.ItemsSource = BLL.UOM.toList;
            cmbUOM.DisplayMemberPath = "Symbol";
            cmbUOM.SelectedValuePath = "Id";

        }
        private void cmbCustomer_Loaded(object sender, RoutedEventArgs e)
        {
            cmbCustomer.ItemsSource = BLL.Ledger.toList.Where(x => x.AccountGroup.GroupName == BLL.DataKeyValue.SundryDebtors_Key).ToList(); ;
            cmbCustomer.DisplayMemberPath = "LedgerName";
            cmbCustomer.SelectedValuePath = "Id";


        }
        #endregion

    }
}
