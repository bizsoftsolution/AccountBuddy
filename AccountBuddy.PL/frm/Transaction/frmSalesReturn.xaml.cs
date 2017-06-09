using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AccountBuddy.Common;

namespace AccountBuddy.PL.frm.Transaction
{
    /// <summary>
    /// Interaction logic for frmSalesReturn.xaml
    /// </summary>
    public partial class frmSalesReturn : UserControl
    {
        BLL.SalesReturn data = new BLL.SalesReturn();

        public frmSalesReturn()
        {
            InitializeComponent();
            this.DataContext = data;

            cmbCustomer.ItemsSource = BLL.Customer.toList;
            cmbCustomer.DisplayMemberPath = "CustomerName";
            cmbCustomer.SelectedValuePath = "Id";

            //.ItemsSource = BLL.TransactionType.toList;
//cmbPType.DisplayMemberPath = "Type";
            //cmbPType.SelectedValuePath = "Id";

          //  cmbItem.ItemsSource = BLL.Product.toList;
//cmbItem.DisplayMemberPath = "ProductName";
          //  cmbItem.SelectedValuePath = "Id";

            cmbUOM.ItemsSource = BLL.UOM.toList;
            cmbUOM.DisplayMemberPath = "Symbol";
            cmbUOM.SelectedValuePath = "Id";

            //cmbPORefNo.ItemsSource = BLL.SalesReturn.;
            //cmbPORefNo.DisplayMemberPath = "RefNo";
            //cmbPORefNo.SelectedValuePath = "Id";

            //CollectionViewSource.GetDefaultView(cmbPORefNo.ItemsSource).Filter = PORefNo_Filter;


//data.Clear();

        }

        #region Events

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            //if (data.SRDetail.ProductId == null)
            //{
            //    MessageBox.Show(Message.PL.Transaction_Empty_Product);
            //}
            //else if (data.SRDetail.Quantity > BLL.Product.toList.Where(x => x.Id == data.SRDetail.ProductId).Select(x => x.AvailableStock).FirstOrDefault())
            //{
            //    var avst = BLL.Product.toList.Where(x => x.Id == data.SRDetail.ProductId).Select(x => x.AvailableStock).FirstOrDefault();
            //    MessageBox.Show(Message.PL.Product_Not_In_Stock, avst.ToString());
            //}
            //else
            //{
            //    data.SaveDetail();
            //}
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
//data.ClearDetail();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
//data.Clear();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            //if (MessageBox.Show(string.Format(Message.PL.TransactionDeleteConfirmation, data.RefNo), "Delete", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            //{
            //    var rv = data.Delete();
            //    if (rv == true)
            //    {
            //        MessageBox.Show("Deleted");
            //        data.Clear();
            //    }
            //}

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //if (data.RefNo == null)
            //{
            //    MessageBox.Show(Message.PL.Transaction_RefNo_Validation);
            //}
            //else if (data.CustomerName == null)
            //{
            //    MessageBox.Show(Message.PL.Transaction_Supplier_Validation);
            //}
            //else if (data.TransactionTypeId == null)
            //{
            //    MessageBox.Show(Message.PL.Transaction_Type_Validation);
            //}
            //else if (data.SRDetails.Count == 0)
            //{
            //    MessageBox.Show(Message.PL.Transaction_ItemDetails_Validation);
            //}
            //else if (data.FindRefNo() == false)
            //{
            //    var rv = data.Save();
            //    if (rv == true)
            //    {
            //        MessageBox.Show("Saved");
            //        data.Clear();
            //    }
            //}
            //else
            //{
            //    MessageBox.Show(string.Format(Message.PL.Transaction_RefNo_ExistValidation, data.RefNo));

            //}
        }

        private void OnDelete(object sender, RoutedEventArgs e)
        {
            //try
            //{
            //    Button btn = (Button)sender;
            //    data.DeleteDetail(btn.Tag.ToString());
            //}
            //catch (Exception ex) { }

        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            //frm.Print.frmQuickSReturn f = new Print.frmQuickSReturn();
            //f.LoadReport(data);
            //f.ShowDialog();
        }

        private void btnsearch_Click(object sender, RoutedEventArgs e)
        {
            //    var rv = data.Find();
            //    if (rv == false) MessageBox.Show(String.Format("{0} is not found", data.SearchText));
            //}
        }
        private void dgvDetails_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //try
            //{
            //    BLL.SalesReturnDetail pod = dgvDetails.SelectedItem as BLL.SalesReturnDetail;
            //    pod.toCopy<BLL.SalesReturnDetail>(data.SRDetail);
            //}
            //catch (Exception ex) { }

        }

        private void txtBarCode_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.Key == Key.Return && data.SRDetail.ProductId != null)
            //{
            //    data.SaveDetail();
            //}
        }

        private void cmbCustomer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
          //  PORefNo_Refresh();
        }

        #endregion

        #region Methods
        public bool PORefNo_Filter(object obj)
        {
            try
            {
                BLL.SalesReturn PO = obj as BLL.SalesReturn;
                BLL.Customer S = cmbCustomer.SelectedItem as BLL.Customer;

               // return PO.CustomerId == S.Id;
            }
            catch (Exception ex) { }
            return false;

        }
        #endregion
    }
}
