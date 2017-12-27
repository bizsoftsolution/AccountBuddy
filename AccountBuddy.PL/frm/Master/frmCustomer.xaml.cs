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
    /// Interaction logic for frmCustomer.xaml
    /// </summary>
    public partial class frmCustomer : UserControl
    {
        #region Field

        public static string FormName = "Customer";
        BLL.Customer data = new BLL.Customer();

        #endregion

        #region Constructor

        public frmCustomer()
        {
            InitializeComponent();
            this.DataContext = data;
            data.Clear();
            rptCustomer.SetDisplayMode(DisplayMode.PrintLayout);
            onClientEvents();
        }

        #endregion

        #region Events

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                BLL.Customer.Init();
                dgvCustomer.ItemsSource = BLL.Customer.toList;

                CollectionViewSource.GetDefaultView(dgvCustomer.ItemsSource).Filter = Customer_Filter;
                CollectionViewSource.GetDefaultView(dgvCustomer.ItemsSource).SortDescriptions.Add(new System.ComponentModel.SortDescription(nameof(data.Ledger.LedgerName), System.ComponentModel.ListSortDirection.Ascending));

                rptContain.IsChecked = true;

                cmbCreditLimitTypeId.ItemsSource = BLL.CreditLimitType.toList;
                cmbCreditLimitTypeId.SelectedValuePath = "Id";
                cmbCreditLimitTypeId.DisplayMemberPath = "LimitType";

                cmbAccountType.ItemsSource = BLL.Ledger.ACTypeList;

                btnSave.Visibility = (BLL.Ledger.UserPermission.AllowInsert || BLL.CompanyDetail.UserPermission.AllowUpdate) ? Visibility.Visible : Visibility.Collapsed;
                btnDelete.Visibility = BLL.Ledger.UserPermission.AllowDelete ? Visibility.Visible : Visibility.Collapsed;
            }
            catch(Exception ex)
            { }
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (data.Ledger.LedgerName == null)
            {
                MessageBox.Show(string.Format(Message.PL.Empty_Record, "Customer Name"), FormName.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (data.Id == 0 && !BLL.UserAccount.AllowInsert(Forms.frmCustomer))
            {
                MessageBox.Show(string.Format(Message.PL.DenyInsert, FormName), FormName.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (data.Id != 0 && !BLL.UserAccount.AllowUpdate(Forms.frmCustomer))
            {
                MessageBox.Show(string.Format(Message.PL.DenyUpdate, FormName), FormName.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (txtMail.Text != "" && !Common.AppLib.IsValidEmailAddress(txtMail.Text))
            {
                MessageBox.Show("Please Enter the Valid Email or Leave Empty");

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
                    MessageBox.Show(string.Format(Message.PL.Existing_Data, data.Ledger.LedgerName), FormName.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
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


        private void dgvCustomer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EditItem();
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

        private bool Customer_Filter(object obj)
        {
            bool RValue = false;
            var d1 = obj as BLL.Customer;
            var d = d1.Ledger;
            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                string strSearch = cbxCase.IsChecked == true ? txtSearch.Text : txtSearch.Text.ToLower();
                string strValue = "";

                foreach (var p in d.GetType().GetProperties())
                {
                    if (p.Name.ToLower().Contains("id") ||
                         p.GetValue(d) == null || p.PropertyType.Namespace!="System") continue;
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
                CollectionViewSource.GetDefaultView(dgvCustomer.ItemsSource).Refresh();
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); };

        }

        private void LoadReport()
        {
            try
            {
                rptCustomer.Reset();
                ReportDataSource data = new ReportDataSource("Ledger", BLL.Customer.toList.Where(x => Customer_Filter(x)).Select(x => new { x.Ledger.LedgerName, x.Ledger.PersonIncharge, x.Ledger.AddressLine1, x.Ledger.AddressLine2, x.Ledger.CityName, x.Ledger.CreditAmount, x.Ledger.CreditLimit, CreditLimitTypeName = x.Ledger.CreditLimitType == null ? null : x.Ledger.CreditLimitType.LimitType, x.Ledger.OPCr, x.Ledger.OPDr, x.Ledger.TelephoneNo }).OrderBy(x => x.LedgerName).ToList());
                ReportDataSource data1 = new ReportDataSource("CompanyDetail", BLL.CompanyDetail.ToList.Where(x => x.Id == BLL.UserAccount.User.UserType.Company.Id).ToList());
                rptCustomer.LocalReport.DataSources.Add(data);
                rptCustomer.LocalReport.DataSources.Add(data1);
                
                rptCustomer.LocalReport.ReportPath = @"rpt\master\RptLedger.rdlc";

                ReportParameter[] param = new ReportParameter[2];
                param[0]=new ReportParameter("Title", "CUSTOMER LIST");
                param[1] = new ReportParameter("AmtPrefix", AppLib.CurrencyPositiveSymbolPrefix.ToString());
                rptCustomer.LocalReport.SetParameters(param);

                rptCustomer.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(SetSubDataSource);


                rptCustomer.RefreshReport();

            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex);
            }


        }
        public void SetSubDataSource(object sender, SubreportProcessingEventArgs e)
        {
            e.DataSources.Add(new ReportDataSource("CompanyDetail", BLL.CompanyDetail.ToList.Where(x => x.Id == BLL.UserAccount.User.UserType.Company.Id).ToList())); ;
        }
        private void onClientEvents()
       {
            }
        private void EditItem()
        {
            try
            {
                var d = dgvCustomer.SelectedItem as BLL.Customer;
                if (d != null)
                {
                    data.Find(d.Id);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }           
        }
        #endregion

        private void txtCreditAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Int32 selectionStart = textBox.SelectionStart;
            Int32 selectionLength = textBox.SelectionLength;
            textBox.Text = AppLib.NumericOnly(txtCreditAmount.Text);
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

        private void txtLedgerOP_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Int32 selectionStart = textBox.SelectionStart;
            Int32 selectionLength = textBox.SelectionLength;
            textBox.Text = AppLib.NumericOnly(txtLedgerOP.Text);
            textBox.SelectionStart = selectionStart <= textBox.Text.Length ? selectionStart : textBox.Text.Length;

        }

        private void dgvCustomer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EditItem();
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
