using Microsoft.AspNet.SignalR.Client;
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

namespace AccountBuddy.PL.frm.Master
{
    /// <summary>
    /// Interaction logic for frmAccountGroup.xaml
    /// </summary>
    public partial class frmAccountGroup : UserControl
    {
        #region Field

        public static string FormName = "Account Group";
        BLL.AccountGroup data = new BLL.AccountGroup();

        #endregion

        #region Constructor

        public frmAccountGroup()
        {
            InitializeComponent();
            this.DataContext = data;
            data.Clear();
            RptAccount.SetDisplayMode(DisplayMode.PrintLayout);

            onClientEvents();

        }

        #endregion

        #region Events

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            BLL.AccountGroup.Init();
            //dgvAccount.ItemsSource = BLL.AccountGroup.toList;

           
            //CollectionViewSource.GetDefaultView(dgvAccount.ItemsSource).Filter = AccountGroup_Filter;
            //CollectionViewSource.GetDefaultView(dgvAccount.ItemsSource).SortDescriptions.Add(new System.ComponentModel.SortDescription(nameof(data.GroupCode), System.ComponentModel.ListSortDirection.Ascending));

            rptContain.IsChecked = true;
            btnSave.Visibility = (BLL.AccountGroup.UserPermission.AllowInsert || BLL.AccountGroup.UserPermission.AllowUpdate) ? Visibility.Visible : Visibility.Collapsed;
            btnDelete.Visibility = BLL.AccountGroup.UserPermission.AllowDelete ? Visibility.Visible : Visibility.Collapsed;

            clear();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (data.GroupName == null)
            {
                MessageBox.Show(String.Format(Message.BLL.Required_Data, "Group Name"),FormName, MessageBoxButton.OK, MessageBoxImage.Asterisk );
            }
            else if (data.Id == 0 && !BLL.UserAccount.AllowInsert(Common.Forms.frmAccountGroup))
            {
                MessageBox.Show(string.Format(Message.PL.DenyInsert, FormName), FormName, MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            else if (data.Id != 0 && !BLL.UserAccount.AllowUpdate(Common.Forms.frmAccountGroup))
            {
                MessageBox.Show(string.Format(Message.PL.DenyUpdate, FormName), FormName, MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            else
            {
                if (data.Save() == true)
                {
                    MessageBox.Show(Message.PL.Saved_Alert, FormName, MessageBoxButton.OK, MessageBoxImage.Information);
                    clear();
                    Grid_Refresh();
                }
                else
                {
                    MessageBox.Show(string.Format(Message.PL.Existing_Data, data.GroupName), FormName, MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (data.Id != 0)
            {
                if (!BLL.UserAccount.AllowDelete(FormName))
                {
                    MessageBox.Show(string.Format(Message.PL.DenyDelete, FormName), FormName, MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    if (MessageBox.Show(Message.PL.Delete_confirmation,FormName, MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.No)
                    {
                        if (data.Delete() == true)
                        {
                            MessageBox.Show(Message.PL.Delete_Alert, FormName, MessageBoxButton.OK, MessageBoxImage.Information);
                            clear();
                            Grid_Refresh();
                        }
                        else
                        {
                            MessageBox.Show(Message.PL.Cant_Delete_Alert, FormName, MessageBoxButton.OK, MessageBoxImage.Information);
                            clear();
                        }

                    }
                }
            }
            else
            {
                MessageBox.Show("No Records to Delete", FormName, MessageBoxButton.OK, MessageBoxImage.Information);
            }


        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            data.Clear();
        }

        private void dgvAccount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BLL.AccountGroup.UserPermission.AllowUpdate)
            {
                var d = dgvAccount.SelectedItem as BLL.AccountGroup;
                if (d != null)
                {
                    data.Find(d.Id);
                }
            }
            else
            {
                MessageBox.Show(string.Format("No Permission to edit this item"), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
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

        private bool AccountGroup_Filter(object obj)
        {
            bool RValue = false;
            var d = obj as BLL.AccountGroup;

            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                string strSearch = cbxCase.IsChecked == true ? txtSearch.Text : txtSearch.Text.ToLower();
                string strValue = "";

                foreach (var p in d.GetType().GetProperties())
                {
                    if (p.Name.ToLower().Contains("id") ||
                            p.GetValue(d) == null ||
                            (p.Name != nameof(d.GroupName) &&
                                p.Name != nameof(d.UnderAccountGroup.GroupName) &&
                                p.Name != nameof(d.GroupCode)


                             )
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

        public void Grid_Refresh()
        {
            try
            {
               // CollectionViewSource.GetDefaultView(dgvAccount.ItemsSource).Refresh();
                CollectionViewSource.GetDefaultView(trvAccount.ItemsSource).Refresh();
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); };

        }

        private void LoadReport()
        {
            try
            {
                RptAccount.Reset();
                ReportDataSource data = new ReportDataSource("AccountGroup", BLL.AccountGroup.toList.Where(x => AccountGroup_Filter(x)).Select(x => new { x.GroupCode, x.GroupName, underGroupName =x.UnderAccountGroup==null?"": x.UnderAccountGroup.GroupName }).OrderBy(x => x.GroupCode).ToList());
                ReportDataSource data1 = new ReportDataSource("CompanyDetail", BLL.CompanyDetail.toList.Where(x => x.Id == BLL.UserAccount.User.UserType.Company.Id).ToList());
                RptAccount.LocalReport.DataSources.Add(data);
                RptAccount.LocalReport.DataSources.Add(data1);
                RptAccount.LocalReport.ReportPath = @"rpt\master\rptAccountGroup.rdlc";

                RptAccount.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(SetSubDataSource);

                RptAccount.RefreshReport();

            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex);
            }


        }
        public  void SetSubDataSource(object sender, SubreportProcessingEventArgs e)
        {
            e.DataSources.Add(new ReportDataSource("CompanyDetail", BLL.CompanyDetail.toList.Where(x => x.Id == BLL.UserAccount.User.UserType.Company.Id).ToList())); ;
        }
       

        private void onClientEvents()
        {     }

        #endregion

        private void dgvAccount_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (BLL.AccountGroup.UserPermission.AllowUpdate)
            {
                var d = dgvAccount.SelectedItem as BLL.AccountGroup;
                if (d != null)
                {
                    data.Find(d.Id);
                }
            }
            else
            {
                MessageBox.Show(string.Format("No Permission to edit this item"), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void trvAccount_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var d = trvAccount.SelectedItem as BLL.AccountGroup;
            if (d != null)
            {
                data.Find(d.Id);
            }
        }
        void clear()
        {
            data.Clear();
           trvAccount.ItemsSource = BLL.AccountGroup.toGroup(null);
           //trvAccount.ItemsSource = BLL.AccountGroup.toGroup(BLL.DataKeyValue.Primary_Value);
        }
        private void cmbUnder_GotFocus(object sender, RoutedEventArgs e)
        {
            var LAGIds = BLL.Ledger.toList.Select(x => x.AccountGroupId).ToList();
            cmbUnder.ItemsSource = BLL.AccountGroup.toList.Where(x => !LAGIds.Contains(x.Id)).ToList();
            cmbUnder.SelectedValuePath = "Id";
            cmbUnder.DisplayMemberPath = "GroupNameWithCode";
        }
    }
}
