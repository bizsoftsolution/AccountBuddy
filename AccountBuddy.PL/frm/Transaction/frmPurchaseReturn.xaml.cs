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
    /// Interaction logic for frmPurchaseReturn.xaml
    /// </summary>
    public partial class frmPurchaseReturn : UserControl
    {
       public BLL.PurchaseReturn data = new BLL.PurchaseReturn();

        public frmPurchaseReturn()
        {
            InitializeComponent();
            this.DataContext = data;

            cmbSupplier.ItemsSource = BLL.Supplier.toList;
            cmbSupplier.DisplayMemberPath = "Ledger.LedgerName";
            cmbSupplier.SelectedValuePath = "Ledger.Id";

            cmbPType.ItemsSource = BLL.TransactionType.toList;
            cmbPType.DisplayMemberPath = "Type";
            cmbPType.SelectedValuePath = "Id";

            cmbItem.ItemsSource = BLL.Product.toList;
            cmbItem.DisplayMemberPath = "ProductName";
            cmbItem.SelectedValuePath = "Id";

            cmbUOM.ItemsSource = BLL.UOM.toList;
            cmbUOM.DisplayMemberPath = "Symbol";
            cmbUOM.SelectedValuePath = "Id";



            data.Clear();

        }

        #region Events

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (data.PRDetail.ProductId == null)
            {
                MessageBox.Show("Enter Product");
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
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(string.Format(Message.PL.Delete_confirmation, data.RefNo), "Delete", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var rv = data.Delete();
                if (rv == true)
                {
                    MessageBox.Show("Deleted");
                    data.Clear();
                }
            }

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (data.RefNo == null)
            {
                MessageBox.Show("Enter Reference No");

            }
            else if (data.LedgerName == null)
            {
                MessageBox.Show("Enter Supplier");

            }
            else if (data.TransactionTypeId == null)
            {
                MessageBox.Show("Enter Transaction Type");
            }
            else if (data.PRDetails.Count == 0)
            {
                MessageBox.Show("Enter Product Details");
            }
            else if (data.FindRefNo() == false)
            {
                var rv = data.Save();
                if (rv == true)
                {
                    MessageBox.Show("Saved Successfully");
                    data.Clear();
                }
            }
            else
            {
                MessageBox.Show(string.Format(Message.PL.Existing_Data, data.RefNo));

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
            frm.Print.frmQuickPReturn f = new Print.frmQuickPReturn();
            f.LoadReport(data);
            f.ShowDialog();
        }

        private void btnsearch_Click(object sender, RoutedEventArgs e)
        {
            var rv = data.Find();
            if (rv == false) MessageBox.Show(String.Format("{0} is not found", data.SearchText));

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
            if (e.Key == Key.Return && data.PRDetail.ProductId != null)
            {
                data.SaveDetail();
            }
        }



        #endregion

        #region Methods

        #endregion


      

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
    }
}
