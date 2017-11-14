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
    /// Interaction logic for frmStaff.xaml
    /// </summary>
    public partial class frmStaff : UserControl
    {
        #region Field

        public static string FormName = "Staff";
        BLL.Staff data = new BLL.Staff();

        #endregion

        #region Constructor

        public frmStaff()
        {
            InitializeComponent();
            this.DataContext = data;
            data.Clear();
            rptStaff.SetDisplayMode(DisplayMode.PrintLayout);
            onClientEvents();
            

        }

        #endregion

        #region Events

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            BLL.Staff.Init();

            dgvStaff.ItemsSource = BLL.Staff.toList;
            cmbDepartment.ItemsSource = BLL.Department.toList;
            cmbDesignation.ItemsSource = BLL.Staff.toList.Select(x=> x.Designation).Distinct().ToList();
            cmbLoginId.ItemsSource = BLL.UserAccount.toList;

            CollectionViewSource.GetDefaultView(dgvStaff.ItemsSource).Filter = Staff_Filter;
            CollectionViewSource.GetDefaultView(dgvStaff.ItemsSource).SortDescriptions.Add(new System.ComponentModel.SortDescription(nameof(data.Ledger.LedgerName), System.ComponentModel.ListSortDirection.Ascending));

            rptContain.IsChecked = true;


            btnSave.Visibility = (BLL.Staff.UserPermission.AllowInsert || BLL.Staff.UserPermission.AllowUpdate) ? Visibility.Visible : Visibility.Collapsed;
            btnDelete.Visibility = BLL.Staff.UserPermission.AllowDelete ? Visibility.Visible : Visibility.Collapsed;

            btnAddUser.Visibility = BLL.UserAccount.UserPermission.AllowInsert  ? Visibility.Visible : Visibility.Collapsed;
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (data.Ledger.LedgerName == null)
            {
                MessageBox.Show(string.Format(Message.PL.Empty_Record, "Staff Name"), FormName.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if ( string.IsNullOrWhiteSpace( data.Designation) )
            {
                MessageBox.Show(string.Format(Message.PL.Empty_Record, "Designation"), FormName.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (data.Salary == 0)
            {
                MessageBox.Show(string.Format(Message.PL.Empty_Record, "Salary"), FormName.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (data.DepartmentId == 0)
            {
                MessageBox.Show(string.Format(Message.PL.Empty_Record, "Department"), FormName.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (data.LoginId == 0)
            {
                MessageBox.Show(string.Format(Message.PL.Empty_Record, "Login Id"), FormName.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (data.DOB == null)
            {
                MessageBox.Show(string.Format("Enter Date Of Birth.."), FormName.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (data.DOJ == null)
            {
                MessageBox.Show(string.Format("Enter Date Of Join.."), FormName.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (txtMail.Text != "" && !Common.AppLib.IsValidEmailAddress(txtMail.Text))
            {
                MessageBox.Show("Please Enter the Valid Email or Leave Empty");

            }
            else if (data.Id == 0 && !BLL.Staff.UserPermission.AllowInsert)
            {
                MessageBox.Show(string.Format(Message.PL.DenyInsert, FormName), FormName.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (data.Id != 0 && !BLL.Staff.UserPermission.AllowUpdate)
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
                    cmbDesignation.ItemsSource = BLL.Staff.toList.Select(x => x.Designation).Distinct().ToList();
                }

                else
                {
                    MessageBox.Show(string.Format(Message.PL.Existing_Data, data.Ledger.LedgerName), FormName.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (data.Id != 0)
            {
                if (!BLL.Staff.UserPermission.AllowDelete)
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
                            cmbDesignation.ItemsSource = BLL.Staff.toList.Select(x => x.Designation).Distinct().ToList();
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


        private void dgvStaff_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var d = dgvStaff.SelectedItem as BLL.Staff;
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

        private bool Staff_Filter(object obj)
        {
            bool RValue = false;
            var d1 = obj as BLL.Staff;
            var d = d1.Ledger;
            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                string strSearch = cbxCase.IsChecked == true ? txtSearch.Text : txtSearch.Text.ToLower();
                string strValue = "";

                foreach (var p in d.GetType().GetProperties())
                {
                    if (p.Name.ToLower().Contains("id") ||
                         p.GetValue(d) == null ||p.PropertyType.Namespace!="System")continue;
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
                CollectionViewSource.GetDefaultView(dgvStaff.ItemsSource).Refresh();
            }
            catch (Exception ex) { };

        }

        private void LoadReport()
        {
            try
            {
                rptStaff.Reset();
                ReportDataSource data = new ReportDataSource("Ledger", BLL.Staff.toList.Where(x => Staff_Filter(x)).Select(x => new { x.Ledger.LedgerName, AccountName=x.Designation, x.Ledger.AddressLine1, x.Ledger.AddressLine2, x.Ledger.CityName, x.Ledger.TelephoneNo, x.Ledger.MobileNo,x.Ledger.EMailId,  OPCr=x.Salary }).OrderBy(x => x.LedgerName).ToList());
                ReportDataSource data1 = new ReportDataSource("CompanyDetail", BLL.CompanyDetail.toList.Where(x => x.Id == BLL.UserAccount.User.UserType.Company.Id).ToList());
                rptStaff.LocalReport.DataSources.Add(data);
                rptStaff.LocalReport.DataSources.Add(data1);

                rptStaff.LocalReport.ReportPath = @"rpt\master\rptStaff.rdlc";
                rptStaff.RefreshReport();

            }
            catch (Exception ex)
            {

            }


        }

        private void onClientEvents()
        {
            BLL.FMCGHubClient.FMCGHub.On<BLL.Staff>("Staff_Save", (Cus) =>
            {

                this.Dispatcher.Invoke(() =>
                {
                    Cus.Save(true);
                });

            });

            BLL.FMCGHubClient.FMCGHub.On("Staff_Delete", (Action<int>)((pk) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    BLL.Staff led = new BLL.Staff();
                    led.Find((int)pk);
                    led.Delete((bool)true);
                });

            }));
        }

        #endregion
    
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

       

        private void dgvStaff_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var d = dgvStaff.SelectedItem as BLL.Supplier;
            if (d != null)
            {
                data.Find(d.Id);
            }
        }

        private void txtMail_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Int32 selectionStart = textBox.SelectionStart;
            Int32 selectionLength = textBox.SelectionLength;
            String newText = String.Empty;

            int AtCount = 0;
            foreach (Char c in textBox.Text.ToCharArray())
            {
                if (Char.IsLetterOrDigit(c) || Char.IsControl(c) || (c == '.' || c == '_') || (c == '@' && AtCount == 0))
                {
                    newText += c;
                    if (c == '@') AtCount += 1;
                }
            }
            textBox.Text = newText;
            textBox.SelectionStart = selectionStart <= textBox.Text.Length ? selectionStart : textBox.Text.Length;
        }

        private void txtMail_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtMail.Text != "" && !Common.AppLib.IsValidEmailAddress(txtMail.Text))
            {
                MessageBox.Show("Please Enter the Valid Email or Leave Empty");
              
            }

        }

        private void btnAddUser_Click(object sender, RoutedEventArgs e)
        {
            frmUser frm = new frmUser();
            frm.LoadWindow( BLL.UserAccount.User.UserType.CompanyId);
            frm.ShowDialog();
            if (frm.LastCreateUserId != 0)
            {
                data.LoginId = frm.LastCreateUserId;
                cmbLoginId.SelectedValue = frm.LastCreateUserId;
            }
        }
    }
}
