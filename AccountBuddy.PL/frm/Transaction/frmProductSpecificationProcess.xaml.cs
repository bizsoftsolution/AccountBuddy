using AccountBuddy.BLL;
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
    /// Interaction logic for frmProductSpecificationProcess.xaml
    /// </summary>
    public partial class frmProductSpecificationProcess : UserControl
    {

        public AccountBuddy.BLL.Product_Spec_Process data = new BLL.Product_Spec_Process();

        public string FormName = "Product Specification Process";

        public frmProductSpecificationProcess()
        {
            InitializeComponent();
            this.DataContext = data;

            data.Clear();
        }

        #region  button Events

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (data.PDetail.ProductId == 0)
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

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (data.Id != 0)
            {
                if (MessageBox.Show(string.Format(Message.PL.Delete_confirmation, data.Id), FormName, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    var rv = data.Delete();
                    if (rv == true)
                    {
                        MessageBox.Show(string.Format(Message.PL.Delete_Alert), FormName, MessageBoxButton.OK, MessageBoxImage.Information);
                        data.Clear();
                    }
                }
            }

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (data.Id == 0 && !BLL.UserAccount.AllowInsert(Forms.frmProductSpecificationProcess))
            {
                MessageBox.Show(string.Format(Message.PL.DenyInsert, FormName), FormName.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (data.Id != 0 && !BLL.UserAccount.AllowUpdate(Forms.frmProductSpecificationProcess))
            {
                MessageBox.Show(string.Format(Message.PL.DenyUpdate, FormName), FormName.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }

            else if (data.ProductId == 0)
            {
                MessageBox.Show(string.Format(Message.PL.Transaction_Empty_Product), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbItemMaster.Focus();
            }
            else if (data.PDetails.Count == 0)
            {
                MessageBox.Show(string.Format(Message.PL.Transaction_ItemDetails_Validation), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbItem.Focus();
            }
            else
            {
                var rv = data.Save();
                if (rv == true)
                {
                    MessageBox.Show(string.Format(Message.PL.Saved_Alert), FormName, MessageBoxButton.OK, MessageBoxImage.Information);
                    data.Clear();
                }
                else
                {
                    MessageBox.Show("Could Not Save Record", FormName, MessageBoxButton.OK, MessageBoxImage.Warning);

                }
            }


        }

        private void OnDelete(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = (Button)sender;
                data.DeleteDetail((int)btn.Tag);
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }

        }



        private void btnsearch_Click(object sender, RoutedEventArgs e)
        {
            // frmPurchaseSearch f = new frmPurchaseSearch();
            // f.ShowDialog();
            // f.Close();
        }

        #endregion

        #region Events 

        private void dgvDetails_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                BLL.Product_Spec_Process_Detail pod = dgvDetails.SelectedItem as BLL.Product_Spec_Process_Detail;
                pod.ToMap(data.PDetail);
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }

        }

        private void txtBarCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && data.PDetail.ProductId != 0)
            {
                if (data.PDetail.ProductId == 0)
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

        #endregion

        #region Methods

        void Clear()
        {
            data.Clear();
            Button_Visibility();
        }
        #endregion

        #region Combo Box Load

        private void cmbItem_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var p = Product_Spec_master.toList.Select(x => x.Product).ToList();
                List<Product> s = new List<Product>();
                foreach (var p1 in p)
                {
                    BLL.Product s1 = new Product();
                    s1.ProductName = p1.ProductName;
                    s1.Id = p1.Id;
                    s.Add(s1);
                }
                cmbItem.ItemsSource = s;
                cmbItem.DisplayMemberPath = "ProductName";
                cmbItemMaster.SelectedValuePath = "Id";
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex);
            }
        }


        #endregion

        #region textchanged





        private void txtQty_TextChanged(object sender, TextChangedEventArgs e)
        {

            TextBox textBox = sender as TextBox;
            Int32 selectionStart = textBox.SelectionStart;
            Int32 selectionLength = textBox.SelectionLength;
            textBox.Text = AppLib.NumericOnly(txtQty.Text);
            textBox.SelectionStart = selectionStart <= textBox.Text.Length ? selectionStart : textBox.Text.Length;

        }

        #endregion

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadWindow();
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex);
            }

        }

        private void LoadWindow()
        {
            Button_Visibility();
        }

        private void Button_Visibility()
        {
            btnSave.Visibility = (BLL.Product_Spec_master.UserPermission.AllowInsert || BLL.Product_Spec_master.UserPermission.AllowUpdate) ? Visibility.Visible : Visibility.Collapsed;
            btnDelete.Visibility = BLL.Product_Spec_master.UserPermission.AllowDelete ? Visibility.Visible : Visibility.Collapsed;

        }

        private void dgvDetails_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                BLL.Product_Spec_Process_Detail pod = dgvDetails.SelectedItem as BLL.Product_Spec_Process_Detail;
                pod.ToMap(data.PDetail);
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
        }


        private void cmbItemMaster_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var p = Product_Spec_master.toList.Select(x => x.PDetails).ToList();
                List<Product> s = new List<Product>();
                foreach (var p1 in p)
                {
                    BLL.Product s1 = new Product();
                    s1.ProductName = p1.FirstOrDefault().ProductName;
                    s1.Id = p1.FirstOrDefault().ProductId;
                    s.Add(s1);
                }
                cmbItemMaster.ItemsSource = s;
                cmbItemMaster.DisplayMemberPath = "ProductName";
                cmbItemMaster.SelectedValuePath = "Id";
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex);
            }
        }
    }
}

