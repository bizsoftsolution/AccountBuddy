﻿using System;
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

namespace AccountBuddy.PL.frm.Transaction
{
    /// <summary>
    /// Interaction logic for frmStockOut.xaml
    /// </summary>
    public partial class frmStockOut : UserControl
    {
        public AccountBuddy.BLL.StockOut data = new BLL.StockOut();

        public string FormName = "Stock Outward";

        public frmStockOut()
        {
            InitializeComponent();
            this.DataContext = data;
            data.Clear();

        }

        #region Events

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var max = BLL.Product.toList.Where(x => x.Id == data.STOutDetail.ProductId).Select(x => x.MaxSellingRate).FirstOrDefault();
            var min = BLL.Product.toList.Where(x => x.Id == data.STOutDetail.ProductId).Select(x => x.MinSellingRate).FirstOrDefault();
            var av = BLL.Product.toList.Where(x => x.Id == data.STOutDetail.ProductId).Select(x => x.AvailableStock).FirstOrDefault();
            if (data.STOutDetail.ProductId == 0)
            {
                MessageBox.Show(string.Format(Message.PL.Empty_Record, "Product"), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbItem.Focus();
            }
            else if (av <= data.STOutDetail.Quantity)
            {
              
                MessageBox.Show(String.Format(Message.PL.Product_Available_Stock, av), FormName, MessageBoxButton.OK, MessageBoxImage.Error);
                txtQty.Focus();
            }
            else if (min > data.STOutDetail.UnitPrice || max < data.STOutDetail.UnitPrice)
            {
                MessageBox.Show(String.Format(Message.PL.Transaction_Selling_Rate, min, max), FormName, MessageBoxButton.OK, MessageBoxImage.Error);
                txtRate.Focus();
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
            btnPrint.IsEnabled = true;

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(string.Format(Message.PL.Delete_confirmation, data.RefNo), "Delete", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var rv = data.Delete();
                if (rv == true)
                {
                    MessageBox.Show("Deleted");
                    data.Clear();
                }
            }

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (data.RefNo == null)
            {
                MessageBox.Show("Enter Reference No");

            }
            else if (data.LedgerName == null)
            {
                MessageBox.Show("Enter Supplier");

            }

            else if (data.STOutDetails.Count == 0)
            {
                MessageBox.Show("Enter Product Details");
            }
            else if (data.FindRefNo() == false)
            {
                data.Type = "Outward";
                var rv = data.Save();
                if (rv == true)
                {
                    MessageBox.Show(Message.PL.Saved_Alert);
                    data.Clear();
                    if (data.Id != 0)
                    {
                        btnPrint.IsEnabled = true;
                    }
                }
            }
            else
            {
                MessageBox.Show(string.Format(Message.PL.Existing_Data, data.RefNo));

            }
        }

        private void OnDelete(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = (Button)sender;
                data.DeleteDetail(btn.Tag.ToString());
            }
            catch (Exception ex) { }

        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            frmQuickStockOut f = new frmQuickStockOut();
            f.LoadReport(data);
            f.ShowDialog();
        }
       
        private void btnsearch_Click(object sender, RoutedEventArgs e)
        {
            var rv = data.Find();

            if (rv == false) MessageBox.Show(String.Format("{0} is not found", data.SearchText));
            if (data.Id != 0)
            {
                btnPrint.IsEnabled = true;
            }
        }

        private void dgvDetails_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                BLL.StockOutDetail pod = dgvDetails.SelectedItem as BLL.StockOutDetail;
                pod.toCopy<BLL.StockOutDetail>(data.STOutDetail);
            }
            catch (Exception ex) { }

        }

        private void txtBarCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && data.STOutDetail.ProductId != 0)
            {
                var max = BLL.Product.toList.Where(x => x.Id == data.STOutDetail.ProductId).Select(x => x.MaxSellingRate).FirstOrDefault();
                var min = BLL.Product.toList.Where(x => x.Id == data.STOutDetail.ProductId).Select(x => x.MinSellingRate).FirstOrDefault();

                if (data.STOutDetail.ProductId == 0)
                {
                    MessageBox.Show(string.Format(Message.PL.Empty_Record, "Product"), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                    cmbItem.Focus();
                }
                else if (min > data.STOutDetail.UnitPrice || max < data.STOutDetail.UnitPrice)
                {
                    MessageBox.Show(String.Format(Message.PL.Transaction_Selling_Rate, min, max), FormName, MessageBoxButton.OK, MessageBoxImage.Error);
                    txtRate.Focus();
                }
                else
                {
                    data.SaveDetail();
                }
            }
        }


        #endregion

        #region Methods

        #endregion



        private void cmbCustomer_Loaded(object sender, RoutedEventArgs e)
        {
            cmbCustomer.ItemsSource = BLL.Ledger.toList.Where(x => x.AccountGroup.GroupName == BLL.DataKeyValue.SundryDebtors_Key || x.AccountGroup.GroupName == BLL.DataKeyValue.BranchDivisions_Key).ToList();
            cmbCustomer.DisplayMemberPath = "LedgerName";
            cmbCustomer.SelectedValuePath = "Id";
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {


            cmbItem.ItemsSource = BLL.Product.toList.ToList();
            cmbItem.DisplayMemberPath = "ProductName";
            cmbItem.SelectedValuePath = "Id";

            cmbUOM.ItemsSource = BLL.UOM.toList.ToList();
            cmbUOM.DisplayMemberPath = "Symbol";
            cmbUOM.SelectedValuePath = "Id";
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

        private void txtDiscount_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Int32 selectionStart = textBox.SelectionStart;
            Int32 selectionLength = textBox.SelectionLength;
            textBox.Text = AppLib.NumericOnly(txtDiscount.Text);
            textBox.SelectionStart = selectionStart <= textBox.Text.Length ? selectionStart : textBox.Text.Length;

        }
    }
}
