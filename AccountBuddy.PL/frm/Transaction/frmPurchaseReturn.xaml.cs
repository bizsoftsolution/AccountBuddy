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
        BLL.PurchaseReturn data = new BLL.PurchaseReturn();

        public frmPurchaseReturn()
        {
            InitializeComponent();
            this.DataContext = data;

            //cmbSupplier.ItemsSource = BLL.Supplier.toList;
            //cmbSupplier.DisplayMemberPath = "SupplierName";
            //cmbSupplier.SelectedValuePath = "Id";

            //cmbPType.ItemsSource = BLL.TransactionType.toList;
            //cmbPType.DisplayMemberPath = "Type";
            //cmbPType.SelectedValuePath = "Id";

            //cmbItem.ItemsSource = BLL.Product.toList;
            //cmbItem.DisplayMemberPath = "ProductName";
            //cmbItem.SelectedValuePath = "Id";

            //cmbUOM.ItemsSource = BLL.UOM.toList;
            //cmbUOM.DisplayMemberPath = "Symbol";
            //cmbUOM.SelectedValuePath = "Id";

            //cmbPORefNo.ItemsSource = BLL.PurchaseReturn.;
            //cmbPORefNo.DisplayMemberPath = "RefNo";
            //cmbPORefNo.SelectedValuePath = "Id";

            //CollectionViewSource.GetDefaultView(cmbPORefNo.ItemsSource).Filter = PORefNo_Filter;


           // data.Clear();

        }

        #region Events

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            //if (data.PRDetail.ProductId == null)
            //{
            //    MessageBox.Show(Message.PL.Transaction_Empty_Product);
            //}
            //else if (data.PRDetail.Quantity > BLL.Product.toList.Where(x => x.Id == data.PRDetail.ProductId).Select(x => x.AvailableStock).FirstOrDefault())
            //{
            //    var avst = BLL.Product.toList.Where(x => x.Id == data.PRDetail.ProductId).Select(x => x.AvailableStock).FirstOrDefault();
            //    MessageBox.Show(Message.PL.Product_Not_In_Stock, avst.ToString());
            //}
            //else
            //{
            //    data.SaveDetail();
            //}
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
           // data.ClearDetail();
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
            //else if (data.SupplierName == null)
            //{
            //    MessageBox.Show(Message.PL.Transaction_Supplier_Validation);
            //}
            //else if (data.TransactionTypeId == null)
            //{
            //    MessageBox.Show(Message.PL.Transaction_Type_Validation);
            //}
            //else if (data.PRDetails.Count == 0)
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
        //{
        //    try
        //    {
        //        Button btn = (Button)sender;
        //        data.DeleteDetail(btn.Tag.ToString());
        //    }
        //    catch (Exception ex) { }

        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            //frm.Print.frmQuickPReturn f = new Print.frmQuickPReturn();
            //f.LoadReport(data);
            //f.ShowDialog();
        }

        private void btnsearch_Click(object sender, RoutedEventArgs e)
        {
            //    var rv = data.Find();
            //    if (rv == false) MessageBox.Show(String.Format("{0} is not found", data.SearchText));
            //
        }

        private void dgvDetails_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //try
            //{
            //    BLL.PurchaseReturnDetail pod = dgvDetails.SelectedItem as BLL.PurchaseReturnDetail;
            //    pod.toCopy<BLL.PurchaseReturnDetail>(data.PRDetail);
            //}
            //catch (Exception ex) { }

        }

        private void txtBarCode_KeyDown(object sender, KeyEventArgs e)
        {
        //    if (e.Key == Key.Return && data.PRDetail.ProductId != null)
        //    {
        //        data.SaveDetail();
        //    }
        }

        private void cmbSupplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           // PORefNo_Refresh();
        }

        private void cmbPORefNo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //try
            //{
            //    BLL.PurchaseReturn PO = cmbPORefNo.SelectedItem as BLL.PurchaseReturn;
            //    PO.SearchText = PO.RefNo;
            //    if (PO.Find())
            //    {
            //        foreach (var pod in PO.PRDetails)
            //        {
            //            data.PRDetail.PDId = pod.Id;
            //            data.PRDetail.ProductId = pod.ProductId.Value;
            //            data.PRDetail.Quantity = pod.Quantity.Value;
            //            data.PRDetail.UnitPrice = pod.UnitPrice;

            //            data.SaveDetail();
            //        }
            //    }

            //}
            //catch (Exception EX) { }
        }

        #endregion

        #region Methods
        //public bool PORefNo_Filter(object obj)
        //{
        //    //try
        //    //{
        //    //    BLL.PurchaseReturn PO = obj as BLL.PurchaseReturn;
        //    //    BLL.Supplier S = cmbSupplier.SelectedItem as BLL.Supplier;

        //    //    return PO.SupplierId == S.Id;
        //    //}
        //    //catch (Exception ex) { }
        //    //return false;

        //}
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
