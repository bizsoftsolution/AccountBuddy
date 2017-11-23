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
    /// Interaction logic for frmUOM.xaml
    /// </summary>
    public partial class frmUOM : UserControl
    {
        #region Field

        public static string FormName = "UOM";
        BLL.UOM data = new BLL.UOM();

        #endregion

        #region Constructor

        public frmUOM()
        {
            InitializeComponent();
            this.DataContext = data;
            data.Clear();
            rptUOM.SetDisplayMode(DisplayMode.PrintLayout);

            onClientEvents();

        }

        #endregion

        #region Events

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            BLL.UOM.Init();
            dgvUOM.ItemsSource = BLL.UOM.toList;

            CollectionViewSource.GetDefaultView(dgvUOM.ItemsSource).Filter = UOM_Filter;
            CollectionViewSource.GetDefaultView(dgvUOM.ItemsSource).SortDescriptions.Add(new System.ComponentModel.SortDescription(nameof(data.Symbol), System.ComponentModel.ListSortDirection.Ascending));
           

            btnSave.Visibility = (BLL.UOM.UserPermission.AllowInsert || BLL.UOM.UserPermission.AllowUpdate) ? Visibility.Visible : Visibility.Collapsed;
            btnDelete.Visibility = BLL.UOM.UserPermission.AllowDelete ? Visibility.Visible : Visibility.Collapsed;

            data.Clear();

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (data.Symbol == null)
            {
                MessageBox.Show(String.Format(Message.BLL.Required_Data, "Symbol"), FormName.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            if (data.FormalName == null)
            {
                MessageBox.Show(String.Format(Message.BLL.Required_Data, "Formal Name"), FormName.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (data.Id == 0 && !BLL.UserAccount.AllowInsert(Common.Forms.frmUOM))
            {
                MessageBox.Show(string.Format(Message.PL.DenyInsert, FormName), FormName.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (data.Id != 0 && !BLL.UserAccount.AllowUpdate(Common.Forms.frmUOM))
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
                    MessageBox.Show(string.Format(Message.PL.Existing_Data, data.Symbol), FormName.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void dgvUOM_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var d = dgvUOM.SelectedItem as BLL.UOM;
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

        private bool UOM_Filter(object obj)
        {
            bool RValue = false;
            var d = obj as BLL.UOM;

            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                string strSearch = cbxCase.IsChecked == true ? txtSearch.Text : txtSearch.Text.ToLower();
                string strValue = "";

                foreach (var p in d.GetType().GetProperties())
                {
                    if (p.Name.ToLower().Contains("id") 
                        || p.GetValue(d) == null) continue;
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
                CollectionViewSource.GetDefaultView(dgvUOM.ItemsSource).Refresh();
            }
            catch (Exception ex) { };

        }

        private void LoadReport()
        {
            try
            {
                rptUOM.Reset();
                ReportDataSource data = new ReportDataSource("UOM", BLL.UOM.toList.Where(x => UOM_Filter(x)).Select(x => new { x.Symbol, x.FormalName }).OrderBy(x => x.Symbol).ToList());
                ReportDataSource data1 = new ReportDataSource("CompanyDetail", BLL.CompanyDetail.toList.Where(x => x.Id == BLL.UserAccount.User.UserType.Company.Id).ToList());
                rptUOM.LocalReport.DataSources.Add(data);
                rptUOM.LocalReport.DataSources.Add(data1);
                rptUOM.LocalReport.ReportPath = @"rpt\master\rptUOM.rdlc";

                rptUOM.RefreshReport();

            }
            catch (Exception ex)
            {

            }


        }

        private void onClientEvents()
        {
            BLL.FMCGHubClient.FMCGHub.On<BLL.UOM>("UOM_Save", (uom) =>
            {

                this.Dispatcher.Invoke(() =>
                {
                    uom.Save(true);
                });

            });

            BLL.FMCGHubClient.FMCGHub.On("UOM_Delete", (Action<int>)((pk) =>
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    BLL.UOM agp = new BLL.UOM();
                    agp.Find((int)pk);
                    agp.Delete((bool)true);
                }));

            }));
        }

        #endregion

        private void dgvUOM_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var d = dgvUOM.SelectedItem as BLL.UOM;
            if (d != null)
            {
                data.Find(d.Id);
            }
        }

    }
}
