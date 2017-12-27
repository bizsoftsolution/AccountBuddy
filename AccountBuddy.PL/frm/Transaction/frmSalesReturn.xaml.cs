﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AccountBuddy.Common;
using Microsoft.AspNet.SignalR.Client;

namespace AccountBuddy.PL.frm.Transaction
{
    /// <summary>
    /// Interaction logic for frmSalesReturn.xaml
    /// </summary>
    public partial class frmSalesReturn : UserControl
    {
        public BLL.SalesReturn data = new BLL.SalesReturn();
        public String FormName = "Sales Return";
        public frmSalesReturn()
        {
            InitializeComponent();
            this.DataContext = data;


            data.setLabel();
            data.Clear();

            LoadWindow();
        }

        private void LoadWindow()
        {
            cmbPType.ItemsSource = BLL.TransactionType.toList;
            cmbPType.DisplayMemberPath = "Type";
            cmbPType.SelectedValuePath = "Id";

            data.setLabel();
            btnSave.Visibility = (BLL.SalesReturn.UserPermission.AllowInsert || BLL.SalesReturn.UserPermission.AllowUpdate) ? Visibility.Visible : Visibility.Collapsed;
            btnDelete.Visibility = BLL.SalesReturn.UserPermission.AllowDelete ? Visibility.Visible : Visibility.Collapsed;
        }      

        #region Button Events

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var max = BLL.Product.toList.Where(x => x.Id == data.SRDetail.ProductId).Select(x => x.MaxSellingRate).FirstOrDefault();
            var min = BLL.Product.toList.Where(x => x.Id == data.SRDetail.ProductId).Select(x => x.MinSellingRate).FirstOrDefault();

            if (data.SRDetail.ProductId == 0)
            {
                MessageBox.Show(string.Format(Message.PL.Empty_Record, "Product"), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (min > data.SRDetail.UnitPrice || max < data.SRDetail.UnitPrice)
            {
                MessageBox.Show(String.Format(Message.PL.Transaction_Selling_Rate, min, max), FormName, MessageBoxButton.OK, MessageBoxImage.Error);
                txtRate.Focus();
            }
            else if (data.SRDetail.Particulars == null)
            {
                MessageBox.Show("Enter Reason for Resale", FormName, MessageBoxButton.OK, MessageBoxImage.Error);
                txtRate.Focus();
            }
            else
            {
                data.SaveDetail();
                ckbIsReSale.IsChecked = false;
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            data.ClearDetail();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            data.Clear();
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
            if (data.Id == 0 && !BLL.UserAccount.AllowInsert(Forms.frmSalesReturn))
            {
                MessageBox.Show(string.Format(Message.PL.DenyInsert, FormName), FormName.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (data.Id != 0 && !BLL.UserAccount.AllowUpdate(Forms.frmSalesReturn))
            {
                MessageBox.Show(string.Format(Message.PL.DenyUpdate, FormName), FormName.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (cmbPType.Text == "Cheque" && BLL.Bank.toList.Count == 0)
            {
                MessageBox.Show("Enter Bank Details for check Transaction", FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                App.frmHome.ShowBank();
            }
            else if (data.RefNo == null)
            {
                MessageBox.Show(string.Format(Message.PL.Transaction_POcode, "SR Code"), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                txtRefNo.Focus();
            }
            else if (data.LedgerId == 0)
            {
                MessageBox.Show(string.Format(Message.PL.Transaction_Empty_Customer), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbCustomer.Focus();
            }

            else if (data.SRDetails.Count == 0)
            {
                MessageBox.Show(string.Format(Message.PL.Transaction_ItemDetails_Validation), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbItem.Focus();
            }
            else if (cmbPType.Text == "Cheque" && txtChequeNo.Text == "")
            {
                MessageBox.Show("Enter cheque No", FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                txtChequeNo.Focus();
            }
            else if (cmbPType.Text == "Cheque" && dtpChequeDate.Text == "")
            {
                MessageBox.Show("Enter cheque Date", FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                dtpChequeDate.Focus();
            }
            else if (cmbPType.Text == "Cheque" && txtBankName.Text == "")
            {
                MessageBox.Show("Enter Bank Name", FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                txtBankName.Focus();
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
            frm.Print.frmQuickSReturn f = new Print.frmQuickSReturn();
            f.LoadReport(data);
            f.ShowDialog();
        }
        private void btnsearch_Click(object sender, RoutedEventArgs e)
        {
            frmSalesReturnSearch f = new frmSalesReturnSearch();
            f.ShowDialog();
            f.Close();
        }

        private void dgvDetails_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                BLL.SalesReturnDetail pod = dgvDetails.SelectedItem as BLL.SalesReturnDetail;
                pod.ToMap<BLL.SalesReturnDetail>(data.SRDetail);
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }

        }

        private void txtBarCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && data.SRDetail.ProductId != 0)
            {
                var max = BLL.Product.toList.Where(x => x.Id == data.SRDetail.ProductId).Select(x => x.MaxSellingRate).FirstOrDefault();
                var min = BLL.Product.toList.Where(x => x.Id == data.SRDetail.ProductId).Select(x => x.MinSellingRate).FirstOrDefault();

                if (data.SRDetail.ProductId == 0)
                {
                    MessageBox.Show(string.Format(Message.PL.Empty_Record, "Product"), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                    cmbItem.Focus();
                }
                else if (min > data.SRDetail.UnitPrice || max < data.SRDetail.UnitPrice)
                {
                    MessageBox.Show(String.Format(Message.PL.Transaction_Selling_Rate, min, max), FormName, MessageBoxButton.OK, MessageBoxImage.Error);
                    txtRate.Focus();
                }
                else if (data.SRDetail.Particulars == null)
                {
                    MessageBox.Show("Enter reason for return", FormName, MessageBoxButton.OK, MessageBoxImage.Error);
                    txtParticulars.Focus();
                }
                else
                {
                    data.SaveDetail();
                    ckbIsReSale.IsChecked = false;
                }
            }
        }

        #endregion

        #region Combobox Load

        private void cmbCustomer_Loaded(object sender, RoutedEventArgs e)
        {
            cmbCustomer.ItemsSource = BLL.Ledger.toList.Where(x => x.AccountGroup.GroupName == BLL.DataKeyValue.SundryDebtors_Key || x.AccountGroup.GroupName == BLL.DataKeyValue.BranchDivisions_Key).ToList(); ;
            cmbCustomer.DisplayMemberPath = "LedgerName";
            cmbCustomer.SelectedValuePath = "Id";

        }

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

        #endregion

        #region TextChanged
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

        private void txtDiscount_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Int32 selectionStart = textBox.SelectionStart;
            Int32 selectionLength = textBox.SelectionLength;
            textBox.Text = AppLib.NumericOnly(txtDiscount.Text);
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

        private void txtRate_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Int32 selectionStart = textBox.SelectionStart;
            Int32 selectionLength = textBox.SelectionLength;
            textBox.Text = AppLib.NumericOnly(txtRate.Text);
            textBox.SelectionStart = selectionStart <= textBox.Text.Length ? selectionStart : textBox.Text.Length;

        }

        #endregion

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
        private void ckbIsReSale_Checked(object sender, RoutedEventArgs e)
        {
            data.SRDetail.IsResale = true;
        }

        private void ckbIsReSale_Unchecked(object sender, RoutedEventArgs e)
        {
            data.SRDetail.IsResale = false;
        }

        private void dgvDetails_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            try
            {
                BLL.SalesReturnDetail pod = dgvDetails.SelectedItem as BLL.SalesReturnDetail;
                pod.ToMap<BLL.SalesReturnDetail>(data.SRDetail);
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
        }
    }
}
