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

namespace AccountBuddy.PL.frm.Transaction
{
    /// <summary>
    /// Interaction logic for frmPurchase.xaml
    /// </summary>
    public partial class frmPurchase : UserControl
    {
        AccountBuddy.BLL.Purchase data = new BLL.Purchase();

        public frmPurchase()
        {
            InitializeComponent();
            this.DataContext = data;

            cmbSupplier.ItemsSource = BLL.Ledger.toList;
            cmbSupplier.DisplayMemberPath = "LedgerName";
            cmbSupplier.SelectedValuePath = "Id";

            cmbPType.ItemsSource = BLL.TransactionType.toList;
            cmbPType.DisplayMemberPath = "Type";
            cmbPType.SelectedValuePath = "Id";

            cmbItem.ItemsSource = BLL.Product.toList;
            cmbItem.DisplayMemberPath = "ProductName";
            cmbItem.SelectedValuePath = "Id";

            cmbUOM.ItemsSource = BLL.UOM.toList;
            cmbUOM.DisplayMemberPath = "Symbol";
            cmbUOM.SelectedValuePath = "Id";

            cmbPORefNo.ItemsSource = BLL.PurchaseOrder.POPendingList;
            cmbPORefNo.DisplayMemberPath = "RefNo";
            cmbPORefNo.SelectedValuePath = "Id";

           // CollectionViewSource.GetDefaultView(cmbPORefNo.ItemsSource).Filter = PO;


            data.Clear();

        }

        #region Events

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (data.PDetail.ProductId == 0)
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
            else if (data.TransactionTypeId == 0)
            {
                MessageBox.Show("Enter Transaction Type");
            }
            else if (data.PDetails.Count == 0)
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
            try
            {
                Button btn = (Button)sender;
                data.DeleteDetail(btn.Tag.ToString());
            }
            catch (Exception ex) { }

        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            frm.Print.frmQuickPurchase f = new Print.frmQuickPurchase();
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
                BLL.PurchaseDetail pod = dgvDetails.SelectedItem as BLL.PurchaseDetail;
                pod.toCopy<BLL.PurchaseDetail>(data.PDetail);
            }
            catch (Exception ex) { }

        }

        private void txtBarCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && data.PDetail.ProductId != 0)
            {
                data.SaveDetail();
            }
        }

        private void cmbLedger_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PORefNo_Refresh();
        }

        private void cmbPORefNo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                BLL.PurchaseOrder PO = cmbPORefNo.SelectedItem as BLL.PurchaseOrder;
                PO.SearchText = PO.RefNo;
                if (PO.Find())
                {
                    foreach (var pod in PO.PODetails)
                    {
                        data.PDetail.PODId = pod.Id;
                        data.PDetail.ProductId = pod.ProductId;
                        data.PDetail.Quantity = pod.Quantity;
                        data.PDetail.UnitPrice = pod.UnitPrice;

                        data.SaveDetail();
                    }
                }

            }
            catch (Exception EX) { }
        }

        #endregion

        #region Methods
        public void PORefNo_Refresh()
        {
            try
            {
                CollectionViewSource.GetDefaultView(cmbPORefNo.ItemsSource).Refresh();
            }
            catch (Exception ex) { };
        }
        #endregion

    }
}
