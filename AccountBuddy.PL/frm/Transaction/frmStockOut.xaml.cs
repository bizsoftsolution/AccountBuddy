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

        public frmStockOut()
        {
            InitializeComponent();
            this.DataContext = data;
            data.Clear();

        }

        #region Events

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (data.STOutDetail.ProductId == 0)
            {
                MessageBox.Show("Enter Product");
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
                data.SaveDetail();
            }
        }


        #endregion

        #region Methods

        #endregion



        private void cmbCustomer_Loaded(object sender, RoutedEventArgs e)
        {
            cmbCustomer.ItemsSource = BLL.Ledger.toList.Where(x => x.AccountGroup.GroupName == BLL.DataKeyValue.SundryDebtors_Key).ToList();
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
    }
}