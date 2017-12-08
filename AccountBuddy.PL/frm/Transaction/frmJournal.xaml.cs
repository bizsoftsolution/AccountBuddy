using AccountBuddy.Common;
using Microsoft.AspNet.SignalR.Client;
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
    /// Interaction logic for frmJournal.xaml
    /// </summary>
    public partial class frmJournal : UserControl
    {
        public BLL.Journal data = new BLL.Journal();
        public string FormName = "Journal";
        decimal drAmt = 0, crAmt = 0, DiffAmt=0;
        public frmJournal()
        {
            InitializeComponent();
            this.DataContext = data;
            data.Clear();
            onClientEvents();

          
        }

        private void LoadWindow()
        {
            data.Clear();        
            btnPrint.IsEnabled = false;           
            btnSave.Visibility = (BLL.Journal.UserPermission.AllowInsert || BLL.Journal.UserPermission.AllowUpdate) ? Visibility.Visible : Visibility.Collapsed;
            btnDelete.Visibility = BLL.Journal.UserPermission.AllowDelete ? Visibility.Visible : Visibility.Collapsed;

        }

        private void onClientEvents()
        {
            BLL.FMCGHubClient.FMCGHub.On<String>("Journal_RefNoRefresh", (EntryNo) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    if (data.Id != 0) data.EntryNo = EntryNo;
                });
            });
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (data.JDetail.LedgerId == 0)
            {
                MessageBox.Show("Enter LedgerName", FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbDebitAC.Focus();
            }
            else if (data.JDetail.DrAmt == 0 && data.JDetail.CrAmt == 0)
            {
                MessageBox.Show("Enter Amount Dr or Amount Cr", FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                txtAmountCr.Focus();
            }
            else if(data.JDetail.TransactionMode==null)
            {
                MessageBox.Show("Enter Mode of Transaction", FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbMode.Focus();
            }
            else if (data.JDetail.TransactionMode == "Cheque" && (data.JDetail.ChequeDate==null||data.JDetail.ChequeNo==""||cmbCqStatus.Text==""))
            {
                MessageBox.Show("Enter Cheque Details", FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                txtChequeNo.Focus();
            }
            else if (data.JDetail.TransactionMode == "Cheque" && data.JDetail.Status == "Completed" && data.JDetail.ClearDate==null)
            {
                MessageBox.Show("Enter Cheque Collection Date", FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                dtpChequeCollectionDate.Focus();
            }
            else if (data.JDetail.TransactionMode == "Cheque" && data.JDetail.Status == "Returned" && (data.JDetail.ClearDate == null|| data.JDetail.ExtraCharge==null))
            {
                MessageBox.Show("Enter Cheque Returned Date And Extra charge", FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                dtpChequeReturnDate.Focus();
            }
            else if (data.JDetail.TransactionMode == "Online" && (txtOnlineRefno.Text == "" || dtpOnlineTDate.SelectedDate == null ||cmbOnStatus.Text==""))
            {
                MessageBox.Show("Enter Online Details", FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                txtOnlineRefno.Focus();
            }
            else if (data.JDetail.TransactionMode == "TT" && (txtTTRefNo.Text==""||dtpTTDate.SelectedDate==null||cmbTTStatus.Text==""))
            {
                MessageBox.Show("Enter TT Details", FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                txtTTRefNo.Focus();
            }
            else
            {
                data.SaveDetail();
                FindDiff();
               
            }
        }

        private void FindDiff()
        {
            var l1 = data.JDetails;
            drAmt= l1.Sum(x => x.CrAmt);
            crAmt = l1.Sum(x => x.DrAmt);

            lblMsg.Text = string.Format("Total Debit Balance : {0:N2}, Total Credit Balance : {1:N2}, Difference : {2:N2}", crAmt, drAmt, Math.Abs(drAmt - crAmt));
            lblMsg.Foreground = drAmt == crAmt ? new SolidColorBrush(Color.FromRgb(0, 0, 255)) : new SolidColorBrush(Color.FromRgb(255, 0, 0));
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            DiffAmt = Math.Abs(drAmt - crAmt);
            if (data.Id == 0 && !BLL.UserAccount.AllowInsert(Forms.frmJournal))
            {
                MessageBox.Show(string.Format(Message.PL.DenyInsert, FormName), FormName, MessageBoxButton.OK, MessageBoxImage.Warning); ;
            }
            else if (data.Id != 0 && !BLL.UserAccount.AllowUpdate(Forms.frmJournal))
            {
                MessageBox.Show(string.Format(Message.PL.DenyUpdate, FormName), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (data.EntryNo == null)
            {
                MessageBox.Show("Enter Entry No", FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (data.JDetails.Count == 0)
            {
                MessageBox.Show("Enter Details", FormName, MessageBoxButton.OK, MessageBoxImage.Warning);

            }
            else if ( DiffAmt!= 0)
            {
                MessageBox.Show("Difference between Credit and Debit Should be Zero", FormName, MessageBoxButton.OK, MessageBoxImage.Warning);

            }
            else if(data.FindEntryNo())
            {
                MessageBox.Show("Entry No Already Exist!..", FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            else
            {
                var rv = data.Save();
                if (rv == true)
                {
                    MessageBox.Show(Message.PL.Saved_Alert, FormName, MessageBoxButton.OK, MessageBoxImage.Information);
                    if (ckxAutoPrint.IsChecked == true) Print();
                    data.Clear();
                    lblMsg.Text = "";
                  
                }
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if(data.Id!=0)
            {
                if (!BLL.UserAccount.AllowDelete(FormName))
                {
                    MessageBox.Show(string.Format(Message.PL.DenyDelete, FormName), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    if (MessageBox.Show("Do you want to delete?", "DELETE", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        var rv = data.Delete();
                        if (rv == true)
                        {
                            MessageBox.Show(Message.PL.Delete_Alert, FormName, MessageBoxButton.OK, MessageBoxImage.Information);
                            data.Clear();
                            if (data.Id != 0)
                            {
                                btnPrint.IsEnabled = true;
                            }
                            else
                            {
                                btnPrint.IsEnabled = false;
                            }
                        }
                    }
                }
                
            }
            else
            {
                MessageBox.Show(Message.PL.No_Records_Delete, FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            data.Clear();
            if (data.Id != 0)
            {
                btnPrint.IsEnabled = true;
            }
            else
            {
                btnPrint.IsEnabled = false;
            }
            lblMsg.Text = "";
            LoadWindow();
        }

        private void btnsearch_Click(object sender, RoutedEventArgs e)
        {
            frmJournalSearch f = new frmJournalSearch();
            f.ShowDialog();
            f.Close();
        }

        private void dgvDetails_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                BLL.JournalDetail pod = dgvDetails.SelectedItem as BLL.JournalDetail;
                pod.toCopy<BLL.JournalDetail>(data.JDetail);
            }
            catch (Exception ex) { }

        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            Print();
        }

        private void Print()
        {
            frm.Vouchers.frmQuickJournalVoucher f = new Vouchers.frmQuickJournalVoucher();

            f.LoadReport(data);
            f.ShowDialog();
        }
        private void btnEditDetail_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = (Button)sender;
                data.FindDetail((int)btn.Tag);
            }
            catch (Exception ex) { }

        }

        private void btnDeleteDetail_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("do you want to delete this detail?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    Button btn = (Button)sender;
                    data.DeleteDetail((int)btn.Tag);
                    FindDiff();
                }
            }
            catch (Exception ex) { }

        }

        private void btnDClear_Click(object sender, RoutedEventArgs e)
        {
            data.ClearDetail();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            LoadWindow();
        }

        private void txtChequeNo_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Int32 selectionStart = textBox.SelectionStart;
            Int32 selectionLength = textBox.SelectionLength;
            textBox.Text = AppLib.NumericOnly(txtChequeNo.Text);
            textBox.SelectionStart = selectionStart <= textBox.Text.Length ? selectionStart : textBox.Text.Length;

        }

        private void cmbDebitAC_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDebitAc();
        }

        private void LoadDebitAc()
        {
            try
            {
                cmbDebitAC.ItemsSource = BLL.Ledger.toList;
                cmbDebitAC.SelectedValuePath = "Id";
                cmbDebitAC.DisplayMemberPath = "AccountName";
            }
            catch(Exception ex)
            {
                Common.AppLib.WriteLog(string.Format("Jounal Account Name Load_{0}_{1}", ex.Message, ex.InnerException));
            }
        }

        private void cmbDebitAC_DropDownOpened(object sender, EventArgs e)
        {
            LoadDebitAc();
        }

        private void txtAmountDr_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Int32 selectionStart = textBox.SelectionStart;
            Int32 selectionLength = textBox.SelectionLength;
            textBox.Text = AppLib.NumericOnly(txtAmountDr.Text);
            textBox.SelectionStart = selectionStart <= textBox.Text.Length ? selectionStart : textBox.Text.Length;

        }

        private void txtAmountCr_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Int32 selectionStart = textBox.SelectionStart;
            Int32 selectionLength = textBox.SelectionLength;
            textBox.Text = AppLib.NumericOnly(txtAmountCr.Text);
            textBox.SelectionStart = selectionStart <= textBox.Text.Length ? selectionStart : textBox.Text.Length;

        }
    }
}
