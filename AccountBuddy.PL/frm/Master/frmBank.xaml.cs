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
    /// Interaction logic for frmBank.xaml
    /// </summary>
    public partial class frmBank : UserControl
    {

        #region Field

        public static string FormName = "Bank";
        BLL.Bank data = new BLL.Bank();

        #endregion

        #region Constructor

        public frmBank()
        {
            InitializeComponent();
            this.DataContext = data;
            data.Clear();
            rptBank.SetDisplayMode(DisplayMode.PrintLayout);
            onClientEvents();
        }

        #endregion

        #region Events

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            BLL.Bank.Init();
            dgvBank.ItemsSource = BLL.Bank.toList;

            CollectionViewSource.GetDefaultView(dgvBank.ItemsSource).Filter = Bank_Filter;
            CollectionViewSource.GetDefaultView(dgvBank.ItemsSource).SortDescriptions.Add(new System.ComponentModel.SortDescription(nameof(data.Ledger.AccountName), System.ComponentModel.ListSortDirection.Ascending));

            cmbAccountType.ItemsSource = BLL.Ledger.ACTypeList;

            btnSave.Visibility = (BLL.CompanyDetail.UserPermission.AllowInsert || BLL.CompanyDetail.UserPermission.AllowUpdate) ? Visibility.Visible : Visibility.Collapsed;
            btnDelete.Visibility = BLL.CompanyDetail.UserPermission.AllowDelete ? Visibility.Visible : Visibility.Collapsed;

            cmbState.ItemsSource = BLL.StateDetail.toList;
            cmbState.DisplayMemberPath = "StateName";
            cmbState.SelectedValuePath = "Id";
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (data.Ledger.LedgerName == null)
            {
                MessageBox.Show(string.Format(Message.PL.Empty_Record, "LedgerName"));
            }
            else if (data.AccountNo == null)
            {
                MessageBox.Show(string.Format(Message.PL.Empty_Record, "AccountNo"));
            }
            else if (data.BankAccountName == null)
            {
                MessageBox.Show(string.Format(Message.PL.Empty_Record, "BankAccountName"));
            }
            else if (txtMail.Text != "" && !Common.AppLib.IsValidEmailAddress(txtMail.Text))
            {
                MessageBox.Show("Please Enter the Valid Email or Leave Empty");

            }
            else if (data.Id == 0 && !BLL.UserAccount.AllowInsert(FormName))
            {
                MessageBox.Show(string.Format(Message.PL.DenyInsert, FormName));
            }
            else if (data.Id != 0 && !BLL.UserAccount.AllowUpdate(FormName))
            {
                MessageBox.Show(string.Format(Message.PL.DenyUpdate, FormName));
            }
            else
            {
                if (data.Save() == true)
                {
                    MessageBox.Show(string.Format(Message.PL.Saved_Alert),FormName, MessageBoxButton.OK, MessageBoxImage.Information);
                    data.Clear();
                    Grid_Refresh();
                }

                else
                {
                    MessageBox.Show(string.Format(Message.PL.Existing_Data, data.Ledger.LedgerName), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (data.Id != 0)
            {
                if (!BLL.UserAccount.AllowDelete(FormName))
                {
                    MessageBox.Show(string.Format(Message.PL.DenyDelete, FormName));
                }
                else
                {
                    if (MessageBox.Show(Message.PL.Delete_confirmation, "", MessageBoxButton.YesNo) != MessageBoxResult.No)
                    {
                        if (data.Delete() == true)
                        {
                            MessageBox.Show(Message.PL.Delete_Alert, FormName, MessageBoxButton.OK, MessageBoxImage.Information);
                            data.Clear();
                            Grid_Refresh();
                        }
                        else
                        {
                            MessageBox.Show(Message.PL.Cant_Delete_Alert, FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("No Records to Delete");
            }


        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            data.Clear();
        }

        private void dgvBank_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var d = dgvBank.SelectedItem as BLL.Bank;
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

        private bool Bank_Filter(object obj)
        {
            bool RValue = false;
            

            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                string strSearch = cbxCase.IsChecked == true ? txtSearch.Text : txtSearch.Text.ToLower();
                string strValue = "";

                var d1 = obj as BLL.Bank;
              
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

                var d = d1.Ledger;

                foreach (var p in d.GetType().GetProperties())
                {
                    if (p.Name.ToLower().Contains("id") ||
                        p.GetValue(d) == null ||
                        p.PropertyType.Namespace != "System"
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
                CollectionViewSource.GetDefaultView(dgvBank.ItemsSource).Refresh();
            }
            catch (Exception ex) { };

        }

        private void LoadReport()
        {
            try
            {
                rptBank.Reset();
                ReportDataSource data = new ReportDataSource("Bank", BLL.Bank.toList.Where(x => Bank_Filter(x)).Select(x => new { AccountName= x.BankAccountName,GroupCode=x.AccountNo,x.Ledger.LedgerName, x.Ledger.PersonIncharge, x.Ledger.AddressLine1, x.Ledger.AddressLine2, x.Ledger.CityName, x.Ledger.CreditAmount, x.Ledger.CreditLimit, CreditLimitTypeName = x.Ledger.CreditLimitType.LimitType, x.Ledger.OPCr, x.Ledger.OPDr }).OrderBy(x => x.AccountName).ToList());
                ReportDataSource data1 = new ReportDataSource("CompanyDetail", BLL.CompanyDetail.toList.Where(x => x.Id == BLL.UserAccount.User.UserType.Company.Id).ToList());
                rptBank.LocalReport.DataSources.Add(data);
                rptBank.LocalReport.DataSources.Add(data1);
                rptBank.LocalReport.ReportPath = @"rpt\master\rptBank.rdlc";

                rptBank.RefreshReport();

            }
            catch (Exception ex)
            {

            }


        }

        private void onClientEvents()
        {
            BLL.FMCGHubClient.FMCGHub.On<BLL.Bank>("Bank_Save", (Cus) => {

                this.Dispatcher.Invoke(() =>
                {
                    Cus.Save(true);
                });

            });

            BLL.FMCGHubClient.FMCGHub.On("Bank_Delete", (Action<int>)((pk) => {
                this.Dispatcher.Invoke((Action)(() => {
                    BLL.Bank led = new BLL.Bank();
                    led.Find((int)pk);
                    led.Delete((bool)true);
                }));

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

        private void txtLedgerOP_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Int32 selectionStart = textBox.SelectionStart;
            Int32 selectionLength = textBox.SelectionLength;
            textBox.Text = AppLib.NumericOnly(txtLedgerOP.Text);
            textBox.SelectionStart = selectionStart <= textBox.Text.Length ? selectionStart : textBox.Text.Length;

        }

        private void dgvBank_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var d = dgvBank.SelectedItem as BLL.Bank;
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
            if (txtMail.Text != "" && !Common.AppLib.IsValidEmailAddress(txtMail.Text)) MessageBox.Show("Please Enter the Valid Email or Leave Empty");

        }


    }
}
