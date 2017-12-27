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
using AccountBuddy.Common;
using Microsoft.AspNet.SignalR.Client;

namespace AccountBuddy.PL.frm.Transaction
{
    /// <summary>
    /// Interaction logic for frmPurchaseRequest.xaml
    /// </summary>
    public partial class frmPurchaseRequest : UserControl
    {
        public BLL.PurchaseRequest data = new BLL.PurchaseRequest();
        public string FormName = "Purchase Request";

        public frmPurchaseRequest()
        {
            InitializeComponent();            
            this.DataContext = data;
            CultureInfo cultureInfo = new CultureInfo("en-US");

            data.Clear();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-MY");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-MY");
            DateTimeFormatInfo dateInfo = new DateTimeFormatInfo();
            dateInfo.ShortDatePattern = "MM/yyyy";
            cultureInfo.DateTimeFormat = dateInfo;
       
            lblDiscountAmount.Text = string.Format("{0}({1})", "Discount Amount", AppLib.CurrencyPositiveSymbolPrefix);
            lblExtraAmount.Text = string.Format("{0}({1})", "Extra Amount", AppLib.CurrencyPositiveSymbolPrefix);
                 }
       

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (data.PRDetail.ProductId == 0)
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
            Clear();
        }

        void Clear()
        {
            data.Clear();

            //btnMakepurchase.IsEnabled = false;
            btnPrint.IsEnabled = false;
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
                    MessageBox.Show(string.Format(Message.PL.Delete_Alert), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                    data.Clear(); Clear();
                }
            }

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

            if (data.Id == 0 && !BLL.PurchaseRequest.UserPermission.AllowInsert)
            {
                MessageBox.Show(string.Format(Message.PL.DenyInsert, FormName), FormName.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (data.Id != 0 && !BLL.PurchaseRequest.UserPermission.AllowUpdate)
            {
                MessageBox.Show(string.Format(Message.PL.DenyUpdate, FormName), FormName.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (data.RefNo == null)
            {
                MessageBox.Show(string.Format(Message.PL.Transaction_POcode, "PO Code"), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                txtRefNo.Focus();
            }
            else if (data.LedgerId == 0)
            {
                MessageBox.Show(string.Format(Message.PL.Transaction_Empty_Supplier), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbSupplier.Focus();
            }
            else if (data.RequestTo == 0)
            {
                MessageBox.Show(string.Format(Message.PL.Empty_Record,"Request To"), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
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
                    if (ckbAutoprint.IsChecked == true)
                    {
                        LoadReport();
                    }
                    data.Clear();
                    Clear();
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
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }

        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            LoadReport();
        }

        private void btnsearch_Click(object sender, RoutedEventArgs e)
        {

            ViewForm();
        }
        public void ViewForm()
        {
            var rv = data.Find();

            if (data.Id != 0)
            {
                //btnMakepurchase.IsEnabled = data.Status == "Pending" ? true : false;
                btnPrint.IsEnabled = true;
            }
            if (data.RefCode != null)
            {
                btnSave.IsEnabled = true;
                btnDelete.IsEnabled = true;
            }
            if (rv == false) MessageBox.Show(string.Format(Message.PL.Transaction_Not_Fount, data.SearchText), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
            //btnMakepurchase.Visibility = (BLL.CompanyDetail.UserPermission.AllowInsert || BLL.CompanyDetail.UserPermission.AllowUpdate) ? Visibility.Visible : Visibility.Collapsed;
            //btnMakepurchase.IsEnabled = false;
        }


        private void dgvDetails_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                BLL.PurchaseRequestDetail pod = dgvDetails.SelectedItem as BLL.PurchaseRequestDetail;
                pod.ToMap<BLL.PurchaseRequestDetail>(data.PRDetail);
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }

        }

        private void txtBarCode_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Return && data.PRDetail.ProductId != 0)
            {
                if (data.PRDetail.ProductId== 0)
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

            frm.Print.frmQuickPurchaseRq f = new Print.frmQuickPurchaseRq();
            f.LoadReport(data);
            f.ShowDialog();
        }

        private void cmbSupplier_Loaded(object sender, RoutedEventArgs e)
        { try
            {
                cmbSupplier.ItemsSource = BLL.Ledger.toList.Where(x => x.AccountGroup.GroupName == BLL.DataKeyValue.SundryCreditors_Key || x.AccountGroup.GroupName == BLL.DataKeyValue.BranchDivisions_Key).ToList();
                cmbSupplier.DisplayMemberPath = "LedgerName";
                cmbSupplier.SelectedValuePath = "Id";
            }
            catch (Exception ex)
            { Common.AppLib.WriteLog(ex); }
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
            btnSave.Visibility = (BLL.PurchaseRequest.UserPermission.AllowInsert || BLL.PurchaseRequest.UserPermission.AllowUpdate) ? Visibility.Visible : Visibility.Collapsed;
            btnDelete.Visibility = BLL.PurchaseRequest.UserPermission.AllowDelete ? Visibility.Visible : Visibility.Collapsed;

        }

        private void cmbRequestTo_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                cmbRequestTo.ItemsSource = BLL.Staff.RequestToList;
                cmbRequestTo.DisplayMemberPath = "Ledger.LedgerName";
                cmbRequestTo.SelectedValuePath = "Id";
            }catch(Exception ex) { Common.AppLib.WriteLog(ex); }
        }
    }
}
