using AccountBuddy.Common;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Interaction logic for frmPurchaseOrder.xaml
    /// </summary>
    public partial class frmPurchaseOrder : UserControl
    {
        public BLL.PurchaseOrder data = new BLL.PurchaseOrder();
        public string FormName = "Purchase Order";
        public frmPurchaseOrder()
        {
            InitializeComponent();
            this.DataContext = data;


            data.Clear();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-MY");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-MY");

        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (data.PODetail.ProductId == 0)
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
            btnMakepurchase.IsEnabled = false;
          
        }

        private void MaxRefNo()
        {
            
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
           
            if (MessageBox.Show(string.Format(Message.PL.Delete_confirmation, data.RefNo), "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var rv = data.Delete();
                if (rv == true)
                {
                    MessageBox.Show(string.Format(Message.PL.Delete_Alert), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                    data.Clear();
                }
            }

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (data.RefNo == null)
            {
                MessageBox.Show(string.Format(Message.PL.Transaction_POcode,"PO Code"),FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                txtRefNo.Focus();
            }
            else if (data.LedgerId == 0)
            {
                MessageBox.Show(string.Format(Message.PL.Transaction_Empty_Supplier), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbSupplier.Focus();
            }
            else if (data.PODetails.Count == 0)
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
            LoadReport();
        }

        private void btnsearch_Click(object sender, RoutedEventArgs e)
        {
            var rv = data.Find();
            if(data.Id!=0)
            {
               btnMakepurchase.IsEnabled = data.Status == "Pending" ? true:false;
            }
            if (rv == false) MessageBox.Show(string.Format(Message.PL.Transaction_Not_Fount, data.SearchText), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
            

        }
        private void btnMakepurchase_Click(object sender, RoutedEventArgs e)
        {
            if (data.MakePurchase())
            {
                MessageBox.Show(string.Format(Message.PL.Transaction_Make_Purchase), FormName, MessageBoxButton.OK, MessageBoxImage.Information);
                data.Clear();
                btnMakepurchase.IsEnabled = false;
            }
        }

        private void dgvDetails_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                BLL.PurchaseOrderDetail pod = dgvDetails.SelectedItem as BLL.PurchaseOrderDetail;
                pod.toCopy<BLL.PurchaseOrderDetail>(data.PODetail);
            }
            catch (Exception ex) { }

        }

        private void txtBarCode_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Return && data.PODetail.ProductId != 0)
            {
                if (data.PODetail.ProductId == 0)
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

        private void LoadReport()
        {
            frm.Print.frmQuickPO f = new Print.frmQuickPO();
            f.LoadReport(data);
            f.ShowDialog();
        }


       
        private void cmbSupplier_Loaded(object sender, RoutedEventArgs e)
        {
            cmbSupplier.ItemsSource = BLL.Ledger.toList.Where(x => x.AccountGroup.GroupName == BLL.DataKeyValue.SundryCreditors_Key).ToList();
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
    }
}
