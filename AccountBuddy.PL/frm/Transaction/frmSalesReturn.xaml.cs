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
       public BLL.SalesReturn data = new BLL.SalesReturn();

        public frmSalesReturn()
        {
            InitializeComponent();
            this.DataContext = data;

        
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
            if (data.SRDetail.ProductId == null)
            {
                MessageBox.Show(Message.PL.Transaction_Empty_Product);
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
                    MessageBox.Show(Message.PL.Delete_confirmation);
                    data.Clear();
                }
            }

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (data.RefNo == null)
            {
                MessageBox.Show("Enter Ref No");
                txtRefNo.Focus();
            }
            else if (data.LedgerName == null)
            {
                MessageBox.Show(Message.PL.Empty_Record,"Customer");
            }
            else if (data.TransactionTypeId == null)
            {
                MessageBox.Show("Enter Transaction Type");
                cmbPType.Focus();
            }
            else if (data.SRDetails.Count == 0)
            {
                MessageBox.Show(Message.PL.Transaction_ItemDetails_Validation);
            }
            else if (data.FindRefNo() == false)
            {
                var rv = data.Save();
                if (rv == true)
                {
                    MessageBox.Show(Message.PL.Saved_Alert);
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
            frm.Print.frmQuickSReturn f = new Print.frmQuickSReturn();
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
                BLL.SalesReturnDetail pod = dgvDetails.SelectedItem as BLL.SalesReturnDetail;
                pod.toCopy<BLL.SalesReturnDetail>(data.SRDetail);
            }
            catch (Exception ex) { }

        }

        private void txtBarCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && data.SRDetail.ProductId != null)
            {
                data.SaveDetail();
            }
        }

        #endregion

        #region Methods

        #endregion

       

        private void cmbCustomer_Loaded(object sender, RoutedEventArgs e)
        {
            cmbCustomer.ItemsSource = BLL.Ledger.toList.Where(x => x.AccountGroup.GroupName == BLL.DataKeyValue.SundryDebtors_Key).ToList(); ;
            cmbCustomer.DisplayMemberPath = "LedgerName";
            cmbCustomer.SelectedValuePath = "Id";

        }
    }
}
