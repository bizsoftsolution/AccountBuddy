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

namespace AccountBuddy.PL.frm.Transaction
{
    /// <summary>
    /// Interaction logic for frmJobOrderIssue.xaml
    /// </summary>
    public partial class frmJobOrderIssue : UserControl
    {
        public BLL.JobOrderIssue data = new BLL.JobOrderIssue();
        public string FormName = "Job Order Issue";

        public frmJobOrderIssue()
        {
            InitializeComponent();
            this.DataContext = data;

            data.Clear();       
            data.setLabel();
            LoadWindow();

        }
        
        #region Button Events


        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {

            if (data.JODetail.ProductId == null)
            {
                MessageBox.Show(string.Format(Message.PL.Empty_Record, "Product"), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
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
            btnJobReceived.IsEnabled = false;
            data.setLabel();
            btnPrint.IsEnabled = false;
            btnSave.IsEnabled = true;
            btnDelete.IsEnabled = true;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(string.Format(Message.PL.Delete_confirmation, data.RefNo), FormName, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var rv = data.Delete();
                if (rv == true)
                {
                    MessageBox.Show(Message.PL.Delete_Alert, FormName, MessageBoxButton.OK, MessageBoxImage.Information);
                    data.Clear();
                }
            }

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (data.Id == 0 && !BLL.UserAccount.AllowInsert(Forms.frmJobOrderIssue))
            {
                MessageBox.Show(string.Format(Message.PL.DenyInsert, FormName), FormName.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (data.Id != 0 && !BLL.UserAccount.AllowUpdate(Forms.frmJobOrderIssue))
            {
                MessageBox.Show(string.Format(Message.PL.DenyUpdate, FormName), FormName.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (data.RefNo == null)
            {
                MessageBox.Show(string.Format(Message.PL.Transaction_POcode, "JO Code"), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                txtRefNo.Focus();
            }
            else if (data.JobWorkerId == null)
            {
                MessageBox.Show(string.Format("Enter Job Worker Name.."), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbJobWorker.Focus();
            }

            else if (data.JODetails.Count == 0)
            {
                MessageBox.Show(string.Format(Message.PL.Transaction_ItemDetails_Validation), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbItem.Focus();
            }

            else if (data.FindRefNo() == false)
            {
                var rv = data.Save();
                if (rv == true)
                {
                    MessageBox.Show(string.Format(Message.PL.Saved_Alert), FormName, MessageBoxButton.OK, MessageBoxImage.Information);
                    if (ckbAutoPrint.IsChecked == true)
                    {
                        Print();
                    }

                    data.Clear();

                    btnPrint.IsEnabled = false;

                }
            }
            else
            {
                MessageBox.Show(string.Format(Message.PL.Existing_Data, data.RefNo), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                txtRefNo.Focus();
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
            Print();
        }
        void Print()
        {
            frm.Print.frmJobOrderIssue f = new Print.frmJobOrderIssue();
            f.LoadReport(data);
            f.ShowDialog();
        }
        private void btnsearch_Click(object sender, RoutedEventArgs e)
        {
            frmJobOrderIssueSearch f = new frmJobOrderIssueSearch();
            f.ShowDialog();
            f.Close();
        }



        #endregion

        #region events

        private void dgvDetails_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                BLL.JobOrderIssueDetail pod = dgvDetails.SelectedItem as BLL.JobOrderIssueDetail;
                pod.ToMap<BLL.JobOrderIssueDetail>(data.JODetail);
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }

        }

        private void txtBarCode_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Return && data.JODetail.ProductId != null)
            {
                if (data.JODetail.ProductId == 0)
                {
                    MessageBox.Show(string.Format(Message.PL.Empty_Record, "Product"), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                    cmbItem.Focus();
                }

                else
                {
                    data.SaveDetail();
                }
            }
        }

        #endregion

        #region Textchanged
        private void txtDiscountAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Int32 selectionStart = textBox.SelectionStart;
            Int32 selectionLength = textBox.SelectionLength;
            textBox.Text = AppLib.NumericOnly(txtDiscountAmount.Text);
            textBox.SelectionStart = selectionStart <= textBox.Text.Length ? selectionStart : textBox.Text.Length;

        }

        private void txtExtraAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Int32 selectionStart = textBox.SelectionStart;
            Int32 selectionLength = textBox.SelectionLength;
            textBox.Text = AppLib.NumericOnly(txtExtraAmount.Text);
            textBox.SelectionStart = selectionStart <= textBox.Text.Length ? selectionStart : textBox.Text.Length;

        }

        private void txtRate_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Int32 selectionStart = textBox.SelectionStart;
            Int32 selectionLength = textBox.SelectionLength;
            textBox.Text = AppLib.NumericOnly(txtRate.Text);
            textBox.SelectionStart = selectionStart <= textBox.Text.Length ? selectionStart : textBox.Text.Length;

        }

        private void txtQty_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Int32 selectionStart = textBox.SelectionStart;
            Int32 selectionLength = textBox.SelectionLength;
            textBox.Text = AppLib.NumericOnly(txtQty.Text);
            textBox.SelectionStart = selectionStart <= textBox.Text.Length ? selectionStart : textBox.Text.Length;

        }

        private void txtdiscountAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Int32 selectionStart = textBox.SelectionStart;
            Int32 selectionLength = textBox.SelectionLength;
            textBox.Text = AppLib.NumericOnly(txtdiscountAmount.Text);
            textBox.SelectionStart = selectionStart <= textBox.Text.Length ? selectionStart : textBox.Text.Length;

        }

        #endregion

        #region combo box loading
        private void cmbItem_Loaded(object sender, RoutedEventArgs e)
        {
            cmbItem.ItemsSource = BLL.Product.toList.Where(x => x.StockGroup.IsSale != false).ToList();
            cmbItem.DisplayMemberPath = "ProductName";
            cmbItem.SelectedValuePath = "Id";

        }

        private void cmbUOM_Loaded(object sender, RoutedEventArgs e)
        {

            cmbUOM.ItemsSource = BLL.UOM.toList;
            cmbUOM.DisplayMemberPath = "Symbol";
            cmbUOM.SelectedValuePath = "Id";

        }
        private void cmbJobWorker_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                cmbJobWorker.ItemsSource = BLL.JobWorker.toList.Where(x => x.Ledger.AccountGroup.CompanyId == BLL.UserAccount.User.UserType.CompanyId).ToList();
                cmbJobWorker.DisplayMemberPath = "Ledger.LedgerName";
                cmbJobWorker.SelectedValuePath = "Id";

            }
            catch(Exception ex)
            { Common.AppLib.WriteLog(ex); }
        }
        #endregion

        private void btnJobReceived_Click(object sender, RoutedEventArgs e)
        {
            if (data.MakeReceived())
            {
                MessageBox.Show(string.Format(Message.PL.Transaction_Job_Received), FormName, MessageBoxButton.OK, MessageBoxImage.Information);
                data.Clear();
                btnJobReceived.IsEnabled = false;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadWindow();
            data.setLabel();
        }

        private void LoadWindow()
        {
         
            btnSave.Visibility = (BLL.JobOrderIssue.UserPermission.AllowInsert || BLL.JobOrderIssue.UserPermission.AllowUpdate) ? Visibility.Visible : Visibility.Collapsed;
            btnDelete.Visibility = BLL.JobOrderIssue.UserPermission.AllowDelete ? Visibility.Visible : Visibility.Collapsed;
        }

        private void dgvDetails_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                BLL.JobOrderIssueDetail pod = dgvDetails.SelectedItem as BLL.JobOrderIssueDetail;
                pod.ToMap<BLL.JobOrderIssueDetail>(data.JODetail);
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
        }
    }
}
