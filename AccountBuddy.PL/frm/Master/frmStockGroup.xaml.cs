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
using Microsoft.AspNet.SignalR.Client;
using Microsoft.Reporting.WinForms;

namespace AccountBuddy.PL.frm.Master
{
    /// <summary>
    /// Interaction logic for frmStockGroup.xaml
    /// </summary>
    public partial class frmStockGroup : UserControl
    {
        #region Field

        public static string FormName = "Stock Group";
        BLL.StockGroup data = new BLL.StockGroup();

        #endregion

        #region Constructor

        public frmStockGroup()
        {
            InitializeComponent();
            this.DataContext = data;
            data.Clear();
            rptStockGroup.SetDisplayMode(DisplayMode.PrintLayout);

            onClientEvents();

        }

        #endregion

        #region Events

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            BLL.StockGroup.Init();
            dgvStock.ItemsSource = BLL.StockGroup.toList;

            CollectionViewSource.GetDefaultView(dgvStock.ItemsSource).Filter = StockGroup_Filter;
            CollectionViewSource.GetDefaultView(dgvStock.ItemsSource).SortDescriptions.Add(new System.ComponentModel.SortDescription(nameof(data.GroupCode), System.ComponentModel.ListSortDirection.Ascending));

            trvStock.ItemsSource = BLL.StockGroup.toGroup(null);

            btnSave.Visibility = (BLL.CompanyDetail.UserPermission.AllowInsert || BLL.CompanyDetail.UserPermission.AllowUpdate) ? Visibility.Visible : Visibility.Collapsed;
            btnDelete.Visibility = BLL.CompanyDetail.UserPermission.AllowDelete ? Visibility.Visible : Visibility.Collapsed;

            data.Clear();

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (data.StockGroupName == null)
            {
                MessageBox.Show(String.Format(Message.BLL.Required_Data, "Group Name"), FormName.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (data.Id == 0 && !BLL.UserAccount.AllowInsert(FormName))
            {
                MessageBox.Show(string.Format(Message.PL.DenyInsert, FormName), FormName.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (data.Id != 0 && !BLL.UserAccount.AllowUpdate(FormName))
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
                    MessageBox.Show(string.Format(Message.PL.Existing_Data, data.StockGroupName), FormName.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (data.Id != 0)
            {
                if (!BLL.UserAccount.AllowDelete(FormName))
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

        private void dgvStock_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var d = dgvStock.SelectedItem as BLL.StockGroup;
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

        #endregion

        #region Methods

        private bool StockGroup_Filter(object obj)
        {
            bool RValue = false;
            var d = obj as BLL.StockGroup;

            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                string strSearch = cbxCase.IsChecked == true ? txtSearch.Text : txtSearch.Text.ToLower();
                string strValue = "";

                foreach (var p in d.GetType().GetProperties())
                {
                    if (p.Name.ToLower().Contains("id") ||
                         p.GetValue(d) == null ||
                            (p.Name != nameof(data.StockGroupName) &&
                                p.Name != nameof(data.underStockGroupName) &&
                                p.Name != nameof(data.GroupCode)
 )) continue;
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
                CollectionViewSource.GetDefaultView(dgvStock.ItemsSource).Refresh();
            }
            catch (Exception ex) { };

        }

        private void LoadReport()
        {
            try
            {
                rptStockGroup.Reset();
                ReportDataSource data = new ReportDataSource("StockGroup", BLL.StockGroup.toList.Where(x => StockGroup_Filter(x)).Select(x => new { StockGroupCode = x.GroupCode, StockGroupName = x.StockGroupName, underGroupName = x.UnderStockGroup.StockGroupName }).OrderBy(x => x.StockGroupCode).ToList());
                ReportDataSource data1 = new ReportDataSource("CompanyDetail", BLL.CompanyDetail.toList.Where(x => x.Id == BLL.UserAccount.User.UserType.Company.Id).ToList());
                rptStockGroup.LocalReport.DataSources.Add(data);
                rptStockGroup.LocalReport.DataSources.Add(data1);
                rptStockGroup.LocalReport.ReportPath = @"rpt\master\rptStockGroup.rdlc";

                rptStockGroup.RefreshReport();

            }
            catch (Exception ex)
            {

            }


        }

        private void onClientEvents()
        {
            BLL.FMCGHubClient.FMCGHub.On<BLL.StockGroup>("StockGroup_Save", (sgp) =>
            {

                this.Dispatcher.Invoke(() =>
                {
                    sgp.Save(true);
                });

            });

            BLL.FMCGHubClient.FMCGHub.On("StockGroup_Delete", (Action<int>)((pk) =>
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    BLL.StockGroup agp = new BLL.StockGroup();
                    agp.Find((int)pk);
                    agp.Delete((bool)true);
                }));

            }));
        }

        #endregion

        private void dgvStock_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var d = dgvStock.SelectedItem as BLL.StockGroup;
            if (d != null)
            {
                data.Find(d.Id);
            }
        }

        private void cmbUnder_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void cmbUnder_GotFocus(object sender, RoutedEventArgs e)
        {
            var PSGIds = BLL.Product.toList.Select(x => x.StockGroupId).ToList();
            cmbUnder.ItemsSource = BLL.StockGroup.toList.Where(x => !PSGIds.Contains(x.Id)).ToList();
            cmbUnder.SelectedValuePath = "Id";
            cmbUnder.DisplayMemberPath = "StockGroupName";
        }

        private void trvStock_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var d = trvStock.SelectedItem as BLL.StockGroup;
            if (d != null)
            {
                data.Find(d.Id);
            }

        }
    }
}
