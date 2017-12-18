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
    /// Interaction logic for frmStockSeperated.xaml
    /// </summary>
    public partial class frmStockSeparated : UserControl
    {
        public BLL.StockSeperated data = new BLL.StockSeperated();
        public string FormName = "Stock Process Received";

        public frmStockSeparated()
        {
            InitializeComponent();
            this.DataContext = data;
            data.setLabel();
            data.Clear();
            onClientEvents();
        }
        private void onClientEvents()
        {
            BLL.FMCGHubClient.HubCaller.On<string>("StockSeperated_RefNoRefresh", (RefNo) =>
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

            if (data.SSDetail.ProductId == null)
            {
                MessageBox.Show(string.Format(Message.PL.Empty_Record, "Product"), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
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
            if (data.Id == 0 && !BLL.UserAccount.AllowInsert(Forms.frmStockSeparated))
            {
                MessageBox.Show(string.Format(Message.PL.DenyInsert, FormName), FormName.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (data.Id != 0 && !BLL.UserAccount.AllowUpdate(Forms.frmStockSeparated))
            {
                MessageBox.Show(string.Format(Message.PL.DenyUpdate, FormName), FormName.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (data.RefNo == null)
            {
                MessageBox.Show(string.Format(Message.PL.Transaction_POcode, "SP Code"), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                txtRefNo.Focus();
            }
            else if (data.StaffId == null)
            {
                MessageBox.Show(string.Format("Enter Staff ...."), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbStaff.Focus();
            }

            else if (data.SSDetails.Count == 0)
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
            //frm.Print.frmStockSeperated f = new Print.frmStockSeperated();
            //f.LoadReport(data);
            //f.ShowDialog();
        }
        private void btnsearch_Click(object sender, RoutedEventArgs e)
        {
            frmStockSeperatedSearch f = new frmStockSeperatedSearch();
            f.ShowDialog();
            f.Close();


        }



        #endregion

        #region events

        private void dgvDetails_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                BLL.StockSeperatedDetail pod = dgvDetails.SelectedItem as BLL.StockSeperatedDetail;
                pod.toCopy<BLL.StockSeperatedDetail>(data.SSDetail);
            }
            catch (Exception ex) { }

        }

        private void txtBarCode_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Return && data.SSDetail.ProductId != null)
            {
                if (data.SSDetail.ProductId == 0)
                {
                    MessageBox.Show(string.Format(Message.PL.Empty_Record, "Product"), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                    cmbItem.Focus();
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

        private void txtdiscountAmount_TextChanged(object sender, TextChangedEventArgs e)
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
            cmbItem.ItemsSource = BLL.Product.toList.Where(x => x.StockGroup.IsSale != false).ToList();
            cmbItem.DisplayMemberPath = "ProductName";
            cmbItem.SelectedValuePath = "Id";

        }

        private void cmbUOM_Loaded(object sender, RoutedEventArgs e)
        {

            cmbUOM.ItemsSource = BLL.UOM.toList;
            cmbUOM.DisplayMemberPath = "Symbol";
            cmbUOM.SelectedValuePath = "Id";

        }



        #endregion

        private void cmbStaff_Loaded(object sender, RoutedEventArgs e)
        {
            cmbStaff.ItemsSource = BLL.Staff.toList.Where(x => x.Ledger.AccountGroup.CompanyId == BLL.UserAccount.User.UserType.CompanyId).ToList();
            cmbStaff.DisplayMemberPath = "Ledger.LedgerName";
            cmbStaff.SelectedValuePath = "Id";


        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            data.setLabel();
            btnSave.Visibility = (BLL.StockSeperated.UserPermission.AllowInsert || BLL.StockSeperated.UserPermission.AllowUpdate) ? Visibility.Visible : Visibility.Collapsed;
            btnDelete.Visibility = BLL.StockSeperated.UserPermission.AllowDelete ? Visibility.Visible : Visibility.Collapsed;
        }

        private void dgvDetails_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                BLL.StockSeperatedDetail pod = dgvDetails.SelectedItem as BLL.StockSeperatedDetail;
                pod.toCopy<BLL.StockSeperatedDetail>(data.SSDetail);
            }
            catch (Exception ex) { }
        }
    }
}
