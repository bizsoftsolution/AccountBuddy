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

namespace AccountBuddy.PL.frm.Master
{
    /// <summary>
    /// Interaction logic for frmProductSpecification.xaml
    /// </summary>
    public partial class frmProductSpecification : UserControl
    {
        public AccountBuddy.BLL.Product_Spec_master data = new BLL.Product_Spec_master();

        public string FormName = "Product Specification";

        public frmProductSpecification()
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
                        Grid_Refresh();
                    }
                }
            }

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (data.Id == 0 && !BLL.UserAccount.AllowInsert(Forms.frmPurchase))
            {
                MessageBox.Show(string.Format(Message.PL.DenyInsert, FormName), FormName.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (data.Id != 0 && !BLL.UserAccount.AllowUpdate(Forms.frmPurchase))
            {
                MessageBox.Show(string.Format(Message.PL.DenyUpdate, FormName), FormName.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
            else if (data.ProductId == 0)
            {
                MessageBox.Show(string.Format(Message.PL.Transaction_Empty_Product), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbItemMaster.Focus();
            }
            else if(BLL.Product_Spec_master.toList.Where(x=>x.ProductId==data.ProductId).Count()>0)
            {
                MessageBox.Show(string.Format("Already Existing Product"), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
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
                    Grid_Refresh();
                }
                else
                {
                    MessageBox.Show("Could Not Save Record", FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                    Grid_Refresh();
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

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            print();
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
                BLL.Product_Spec_Detail pod = dgvDetails.SelectedItem as BLL.Product_Spec_Detail;
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
        void print()
        {
            frm.Print.frmQuickPurchase f = new Print.frmQuickPurchase();
           // f.LoadReport(data);
            f.ShowDialog();
        }

        void Clear()
        {
            data.Clear();
            btnSave.IsEnabled = true;
            btnDelete.IsEnabled = true;
        }
        #endregion

        #region Combo Box Load


      

        private void cmbItem_Loaded(object sender, RoutedEventArgs e)
        {
            cmbItem.ItemsSource = Product.toList.ToList();
            cmbItem.DisplayMemberPath = "ProductName";
            cmbItem.SelectedValuePath = "Id";
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
                dgvProduct.ItemsSource = BLL.Product_Spec_master.toList;
                
                CollectionViewSource.GetDefaultView(dgvProduct.ItemsSource).Filter = Product_Filter;
                CollectionViewSource.GetDefaultView(dgvProduct.ItemsSource).SortDescriptions.Add(new System.ComponentModel.SortDescription(nameof(data.ProductName), System.ComponentModel.ListSortDirection.Ascending));
            }
            catch(Exception ex)
            {
                Common.AppLib.WriteLog(ex);
            }

        }

        private void LoadWindow()
        {
            btnSave.Visibility = (BLL.Product_Spec_master.UserPermission.AllowInsert || BLL.Product_Spec_master.UserPermission.AllowUpdate) ? Visibility.Visible : Visibility.Collapsed;
            btnDelete.Visibility = BLL.Product_Spec_master.UserPermission.AllowDelete ? Visibility.Visible : Visibility.Collapsed;

        }

        private void dgvDetails_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                BLL.Product_Spec_Detail pod = dgvDetails.SelectedItem as BLL.Product_Spec_Detail;
                pod.ToMap(data.PDetail);
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
        }

        private void cmbItemMaster_Loaded(object sender, RoutedEventArgs e)
        {
            cmbItemMaster.ItemsSource = Product.toList.ToList();
            cmbItemMaster.DisplayMemberPath = "ProductName";
            cmbItemMaster.SelectedValuePath = "Id";
        }

        private void dgvProduct_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
          
                try
                {
                    var d = dgvProduct.SelectedItem as BLL.Product_Spec_master;
                    if (d != null)
                    {
                        data.Find(d.Id);
                    }
                }
                catch (Exception ex)
                {
                    Common.AppLib.WriteLog(ex);
                }
            

        }
        private bool Product_Filter(object obj)
        {
            bool RValue = false;
         
            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                string strSearch = cbxCase.IsChecked == true ? txtSearch.Text : txtSearch.Text.ToLower();
                string strValue = "";

                var d1 = obj as BLL.Product_Spec_master;

                foreach (var p in d1.GetType().GetProperties())
                {
                    if (p.Name.ToLower().Contains("id") ||
                        p.GetValue(d1) == null ||
                        p.PropertyType.Namespace != "System"
                            ) continue;
                    strValue = p.GetValue(d1).ToString();
                    if (cbxCase.IsChecked == false)
                    {
                        strValue = strValue.ToLower();
                    }
                    if (rptStartWith.IsChecked == true && strValue.StartsWith(strSearch))
                    {
                        RValue = true;
                        break;
                    }
                    else if (rptContain.IsChecked == true && strValue.Contains(strSearch))
                    {
                        RValue = true;
                        break;
                    }
                    else if (rptEndWith.IsChecked == true && strValue.EndsWith(strSearch))
                    {
                        RValue = true;
                        break;
                    }
                }

           
            }
            else
            {
                RValue = true;
            }
            return RValue;
        }
        private void Grid_Refresh()
        {
            try
            {
                if (dgvProduct != null) CollectionViewSource.GetDefaultView(dgvProduct.ItemsSource).Refresh();

                
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); };

        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Grid_Refresh();
        }

        private void cbxCase_Unchecked(object sender, RoutedEventArgs e)
        {
            Grid_Refresh();
        }

        private void cbxCase_Checked(object sender, RoutedEventArgs e)
        {
            Grid_Refresh();
        }

        private void rptStartWith_Checked(object sender, RoutedEventArgs e)
        {
            Grid_Refresh();
        }

        private void rptStartWith_Unchecked(object sender, RoutedEventArgs e)
        {
            Grid_Refresh();
        }

        private void rptContain_Unchecked(object sender, RoutedEventArgs e)
        {
            Grid_Refresh();
        }

        private void rptContain_Checked(object sender, RoutedEventArgs e)
        {
            Grid_Refresh();
        }

        private void rptEndWith_Unchecked(object sender, RoutedEventArgs e)
        {
            Grid_Refresh();
        }

        private void rptEndWith_Checked(object sender, RoutedEventArgs e)
        {
            Grid_Refresh();
        }
    }
}
