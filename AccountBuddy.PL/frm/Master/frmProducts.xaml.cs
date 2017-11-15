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
using Microsoft.Reporting.WinForms;

namespace AccountBuddy.PL.frm.Master
{
    /// <summary>
    /// Interaction logic for frmProducts.xaml
    /// </summary>
    public partial class frmProducts : UserControl
    {
        #region Field

        public static string FormName = "Products";
        BLL.Product data = new BLL.Product();

        #endregion

        #region Constructor

        public frmProducts()
        {
            InitializeComponent();
            this.DataContext = data;
            data.Clear();
            rptProduct.SetDisplayMode(DisplayMode.PrintLayout);
            onClientEvents();
        }

        #endregion

        #region Events

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            BLL.Product.Init();
            dgvProduct.ItemsSource = BLL.Product.toList;

            CollectionViewSource.GetDefaultView(dgvProduct.ItemsSource).Filter = Product_Filter;
            CollectionViewSource.GetDefaultView(dgvProduct.ItemsSource).SortDescriptions.Add(new System.ComponentModel.SortDescription(nameof(data.ProductName), System.ComponentModel.ListSortDirection.Ascending));

            var SUIds = BLL.StockGroup.toList.Select(x => x.UnderGroupId).ToList();
            cmbStockGroupId.ItemsSource = BLL.StockGroup.toList.Where(x=> !SUIds.Contains(x.Id)).ToList();
            cmbStockGroupId.DisplayMemberPath = "StockGroupName";
            cmbStockGroupId.SelectedValuePath = "Id";

            cmbUOM.ItemsSource = BLL.UOM.toList;
            cmbUOM.SelectedValuePath = "Id";
            cmbUOM.DisplayMemberPath = "Symbol";

            btnSave.Visibility = (BLL.Product.UserPermission.AllowInsert || BLL.Product.UserPermission.AllowUpdate) ? Visibility.Visible : Visibility.Collapsed;
            btnDelete.Visibility = BLL.Product.UserPermission.AllowDelete ? Visibility.Visible : Visibility.Collapsed;

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (data.ProductName == null)
                {
                    MessageBox.Show(string.Format(Message.PL.Empty_Record, "ProductName"), FormName.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (data.ItemCode == null)
                {
                    MessageBox.Show(string.Format(Message.PL.Empty_Record, "Item Code"), FormName.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (data.StockGroupId == 0)
                {
                    MessageBox.Show(string.Format(Message.PL.Empty_Record, "Stock Group"), FormName.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (data.UOMId == 0)
                {
                    MessageBox.Show(string.Format(Message.PL.Empty_Record, "UOM"), FormName.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (data.Id == 0 && !BLL.Product.UserPermission.AllowInsert)
                {
                    MessageBox.Show(string.Format(Message.PL.DenyInsert, FormName), FormName.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (data.Id != 0 && !BLL.Product.UserPermission.AllowUpdate)
                {
                    MessageBox.Show(string.Format(Message.PL.DenyUpdate, FormName), FormName.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    if (data.Save() == true)
                    {
                        MessageBox.Show(Message.PL.Saved_Alert, FormName.ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
                        data.Clear();
                        Grid_Refresh();
                    }
                    else
                    {
                        MessageBox.Show(string.Format(Message.PL.Existing_Data, data.ProductName), FormName.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch(Exception ex) { }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (data.Id != 0)
            {
                if (!BLL.Product.UserPermission.AllowDelete)
                {
                    MessageBox.Show(string.Format(Message.PL.DenyDelete, FormName), FormName.ToString(), MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    if (MessageBox.Show(Message.PL.Delete_confirmation, FormName, MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.No)
                    {
                        if (data.Delete() == true)
                        {
                            MessageBox.Show(Message.PL.Delete_Alert, FormName.ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
                            data.Clear();
                            Grid_Refresh();
                        }
                        else
                        {
                            MessageBox.Show(Message.PL.Cant_Delete_Alert, FormName.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                            data.Clear();
                        }

                    }
                }
            }
            else
            {
                MessageBox.Show("No Records to Delete", FormName.ToString(), MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            data.Clear();
        }

        private void dgvProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var d = dgvProduct.SelectedItem as BLL.Product;
            if (d != null)
            {
                data.Find(d.Id);
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
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

        private void cbxCase_Unchecked(object sender, RoutedEventArgs e)
        {
            Grid_Refresh();
        }

        private void rptContain_Checked(object sender, RoutedEventArgs e)
        {
            Grid_Refresh();
        }

        private void rptEndWith_Checked(object sender, RoutedEventArgs e)
        {
            Grid_Refresh();
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabControl tc = sender as TabControl;

            if (tc.SelectedIndex == 1)
            {
                LoadReport();
            }

        }

        private void NumericOnly(System.Object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = Common.AppLib.IsTextNumeric(e.Text);
        }

        #endregion

        #region Methods

        private bool Product_Filter(object obj)
        {
            bool RValue = false;
            var d = obj as BLL.Product;

            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                string strSearch = cbxCase.IsChecked == true ? txtSearch.Text : txtSearch.Text.ToLower();
                string strValue = "";

                foreach (var p in d.GetType().GetProperties())
                {
                    if (p.Name.ToLower().Contains("id") || p.GetValue(d) == null
                         ||(p.Name != nameof(d.ProductName) &&
                                p.Name != nameof(d.ItemCode) &&
                                p.Name != nameof(d.SellingRate)&&
                                p.Name!=nameof(d.MinSellingRate)&&
                                p.Name!=nameof(d.MaxSellingRate) &&
                                p.Name!=nameof(d.SellingRate))

                        ) continue;
                    strValue = p.GetValue(d).ToString();
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
                CollectionViewSource.GetDefaultView(dgvProduct.ItemsSource).Refresh();
            }
            catch (Exception ex) { };

        }

        private void LoadReport()
        {
            try
            {
                rptProduct.Reset();
                ReportDataSource data = new ReportDataSource("Products", BLL.Product.toList.Where(x => Product_Filter(x)).Select(x => new { x.ProductName, x.StockGroup.StockGroupName, UOMName=x.UOM.Symbol, x.ItemCode, x.PurchaseRate, x.SellingRate,  x.MRP, x.OpeningStock, x.ReOrderLevel }).OrderBy(x => x.ProductName).ToList());
                ReportDataSource data1 = new ReportDataSource("CompanyDetail", BLL.CompanyDetail.toList.Where(x => x.Id == BLL.UserAccount.User.UserType.Company.Id).ToList());
                rptProduct.LocalReport.DataSources.Add(data);
                rptProduct.LocalReport.DataSources.Add(data1);
                rptProduct.LocalReport.ReportPath = @"rpt\master\rptProducts.rdlc";

                rptProduct.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(SetSubDataSource);


                rptProduct.RefreshReport();

            }
            catch (Exception ex)
            {

            }


        }
        private void SetSubDataSource(object sender, SubreportProcessingEventArgs e)
        {
            e.DataSources.Add(new ReportDataSource("CompanyDetail", BLL.CompanyDetail.toList.Where(x => x.Id == BLL.UserAccount.User.UserType.Company.Id).ToList())); ;

        }
        private void onClientEvents()
        {
            BLL.FMCGHubClient.FMCGHub.On<BLL.Product>("Product_Save", (led) => {

                this.Dispatcher.Invoke(() =>
                {
                    led.Save(true);
                });

            });

            BLL.FMCGHubClient.FMCGHub.On("Product_Delete", (Action<int>)((pk) => {
                this.Dispatcher.Invoke((Action)(() => {
                    BLL.Product led = new BLL.Product();
                    led.Find((int)pk);
                    led.Delete((bool)true);
                }));

            }));
        }

        #endregion

        private void txtPurchase_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Int32 selectionStart = textBox.SelectionStart;
            Int32 selectionLength = textBox.SelectionLength;
            textBox.Text = AppLib.NumericOnly(txtPurchaseRate.Text);
            textBox.SelectionStart = selectionStart <= textBox.Text.Length ? selectionStart : textBox.Text.Length;
        }

        private void rptStartWith_Unchecked(object sender, RoutedEventArgs e)
        {
            Grid_Refresh();
        }

        private void rptContain_Unchecked(object sender, RoutedEventArgs e)
        {
            Grid_Refresh();
        }

        private void rptEndWith_Unchecked(object sender, RoutedEventArgs e)
        {
            Grid_Refresh();
        }


        private void dgvProduct_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var d = dgvProduct.SelectedItem as BLL.Product;
            if (d != null)
            {
                data.Find(d.Id);
            }
        }

        private void txtSellingRate_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Int32 selectionStart = textBox.SelectionStart;
            Int32 selectionLength = textBox.SelectionLength;
            textBox.Text = AppLib.NumericOnly(txtSellingRate.Text);
            textBox.SelectionStart = selectionStart <= textBox.Text.Length ? selectionStart : textBox.Text.Length;

        }

        private void txtMRP_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Int32 selectionStart = textBox.SelectionStart;
            Int32 selectionLength = textBox.SelectionLength;
            textBox.Text = AppLib.NumericOnly(txtMRP.Text);
            textBox.SelectionStart = selectionStart <= textBox.Text.Length ? selectionStart : textBox.Text.Length;

        }

        //private void txtGST_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    TextBox textBox = sender as TextBox;
        //    Int32 selectionStart = textBox.SelectionStart;
        //    Int32 selectionLength = textBox.SelectionLength;
        //    textBox.Text = AppLib.NumericOnly(txtGST.Text);
        //    textBox.SelectionStart = selectionStart <= textBox.Text.Length ? selectionStart : textBox.Text.Length;

        //}

        private void txtOpeningStock_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Int32 selectionStart = textBox.SelectionStart;
            Int32 selectionLength = textBox.SelectionLength;
            textBox.Text = AppLib.NumericOnly(txtOpeningStock.Text);
            textBox.SelectionStart = selectionStart <= textBox.Text.Length ? selectionStart : textBox.Text.Length;

        }

        private void txtReOrderLevel_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Int32 selectionStart = textBox.SelectionStart;
            Int32 selectionLength = textBox.SelectionLength;
            textBox.Text = AppLib.NumericOnly(txtReOrderLevel.Text);
            textBox.SelectionStart = selectionStart <= textBox.Text.Length ? selectionStart : textBox.Text.Length;

        }

        //private void txtDiscount_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    TextBox textBox = sender as TextBox;
        //    Int32 selectionStart = textBox.SelectionStart;
        //    Int32 selectionLength = textBox.SelectionLength;
        //    textBox.Text = AppLib.NumericOnly(txtDiscount.Text);
        //    textBox.SelectionStart = selectionStart <= textBox.Text.Length ? selectionStart : textBox.Text.Length;

        //}

        private void txtMaxSellingRate_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Int32 selectionStart = textBox.SelectionStart;
            Int32 selectionLength = textBox.SelectionLength;
            textBox.Text = AppLib.NumericOnly(txtMaxSellingRate.Text);
            textBox.SelectionStart = selectionStart <= textBox.Text.Length ? selectionStart : textBox.Text.Length;

        }

        private void txtMinSellingRate_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Int32 selectionStart = textBox.SelectionStart;
            Int32 selectionLength = textBox.SelectionLength;
            textBox.Text = AppLib.NumericOnly(txtMinSellingRate.Text);
            textBox.SelectionStart = selectionStart <= textBox.Text.Length ? selectionStart : textBox.Text.Length;

        }
    }
}
