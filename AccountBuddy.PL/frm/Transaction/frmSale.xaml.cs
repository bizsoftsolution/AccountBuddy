using AccountBuddy.Common;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Drawing;
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
    /// Interaction logic for frmSale.xaml
    /// </summary>
    public partial class frmSale : UserControl
    {
        public BLL.Sale data = new BLL.Sale();
        string TextToPrint = "";
        public string FormName = "Sales";
        public frmSale()
        {
            InitializeComponent();
            this.DataContext = data;

            data.Clear();
            onClientEvents();


        }
        private void onClientEvents()
        {
            BLL.FMCGHubClient.FMCGHub.On<String>("Sales_RefNoRefresh", (RefNo) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    if (data.Id == 0) data.RefNo = RefNo;
                });
            });
        }

        #region Button Events

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            BLL.Product.Init();

            var max = data.SDetail.Product.MaxSellingRate;
            var min = data.SDetail.Product.MinSellingRate;

            if (data.SDetail.ProductId == 0)
            {
                MessageBox.Show(string.Format(Message.PL.Empty_Record, "Product"), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbItem.Focus();
            }
            else if (data.SDetail.Product.AvailableStock < data.SDetail.Quantity)
            {
                var v = BLL.Product.toList.Where(x => x.Id == data.SDetail.ProductId).Select(x => x.AvailableStock).FirstOrDefault();
                MessageBox.Show(String.Format(Message.PL.Product_Available_Stock, v), FormName, MessageBoxButton.OK, MessageBoxImage.Error);
                txtQty.Focus();
            }
            else if (min > data.SDetail.UnitPrice || max < data.SDetail.UnitPrice)
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
            if (data.Id != 0)
            {
                btnPrint.IsEnabled = true;
            }
            btnSave.IsEnabled = true;
            btnDelete.IsEnabled = true;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (data.Id != 0)
            {
                if (MessageBox.Show(string.Format(Message.PL.Delete_confirmation, data.RefNo), "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    var rv = data.Delete();
                    if (rv == true)
                    {
                        MessageBox.Show(string.Format(Message.PL.Delete_Alert), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                        data.Clear();
                        btnPrint.IsEnabled = false;
                    }
                }
            }
            else
            {
                MessageBox.Show("No Records to Delete", FormName, MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (data.Id == 0 && !BLL.UserAccount.AllowInsert(Forms.frmSales))
            {
                MessageBox.Show(string.Format(Message.PL.DenyInsert, FormName), FormName.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (data.Id != 0 && !BLL.UserAccount.AllowUpdate(Forms.frmSales))
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
                MessageBox.Show(string.Format(Message.PL.Transaction_POcode, "PO Code"), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                txtRefNo.Focus();
            }
            else if (data.LedgerId == 0)
            {
                MessageBox.Show(string.Format(Message.PL.Transaction_Empty_Customer), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbCustomer.Focus();
            }
            else if (data.SDetails.Count == 0)
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
                        PrintBill();
                    }

                    data.Clear();
                    btnPrint.IsEnabled = false;
                }
                else
                { }
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
            catch (Exception ex) { }

        }

        private void btnsearch_Click(object sender, RoutedEventArgs e)
        {
            

            frmSalesSearch f = new frmSalesSearch();
            f.ShowDialog();
            f.Close();
        }

        #endregion

        #region Print
        enum PrintTextAlignType
        {
            Left,
            Center,
            Right
        }
        int PrintNoOfCharPerLine = 27;
        String PrintLine(string Text, PrintTextAlignType AlignType)
        {

            String RValue = "";
            if (AlignType == PrintTextAlignType.Left)
            {
                RValue = Text;
            }
            else if (AlignType == PrintTextAlignType.Center)
            {
                RValue = new string(' ', Math.Abs(PrintNoOfCharPerLine - Text.Length) / 2) + Text;
            }
            else
            {
                RValue = new string(' ', Math.Abs(PrintNoOfCharPerLine - Text.Length)) + Text;
            }
            return RValue + System.Environment.NewLine;
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            PrintBill();
        }
        void PrintBill()
        {
            if (rdbBillPrint.IsChecked == true)
            {

                System.Drawing.Printing.PrintDocument prnPurchaseOrder = new System.Drawing.Printing.PrintDocument();
                prnPurchaseOrder.PrintPage += PrnPurchaseOrder_PrintPage;
                PrintDialog pd = new PrintDialog();
                pd.ShowDialog();
                prnPurchaseOrder.DefaultPageSettings.PrinterSettings.PrinterName = prnPurchaseOrder.DefaultPageSettings.PrinterSettings.PrinterName;
                //prnPurchaseOrder.DefaultPageSettings.PrinterSettings.PrinterName = @"\\192.168.43.212\POS-58";
                prnPurchaseOrder.PrintController = new System.Drawing.Printing.StandardPrintController();

                TextToPrint = PrintLine(BLL.UserAccount.User.UserType.Company.CompanyName, PrintTextAlignType.Center);
                TextToPrint += PrintLine(string.Format("{0}", BLL.UserAccount.User.UserType.Company.AddressLine1), PrintTextAlignType.Center);
                TextToPrint += PrintLine(string.Format("{0}", BLL.UserAccount.User.UserType.Company.AddressLine2), PrintTextAlignType.Center);
                TextToPrint += PrintLine(string.Format("{0}", BLL.UserAccount.User.UserType.Company.CityName), PrintTextAlignType.Center);

                TextToPrint += PrintLine(string.Format("{0}CASH BILL{0}", new string('-', 8)), PrintTextAlignType.Center);

                TextToPrint += PrintLine(string.Format("Dt: {0:dd/MM/yyyy hh:mm tt}", DateTime.Now), PrintTextAlignType.Left);
                TextToPrint += PrintLine(string.Format("Bill No: {0}", data.RefNo), PrintTextAlignType.Left);

                TextToPrint += PrintLine(string.Format("{0}", new string('-', PrintNoOfCharPerLine)), PrintTextAlignType.Center);
                TextToPrint += PrintLine(string.Format("{0,3} {1,-11} {2,10}", "SNo", "Particulars", "Amount"), PrintTextAlignType.Left);
                TextToPrint += PrintLine(string.Format("{0}", new string('-', PrintNoOfCharPerLine)), PrintTextAlignType.Center);

                int sno = 0;

                foreach (var data in data.SDetails)
                {
                    TextToPrint += PrintLine(string.Format("{0,3} {1,-11} {2,10:0.00}", ++sno, data.ProductName, data.UnitPrice * (decimal)data.Quantity), PrintTextAlignType.Left);
                    TextToPrint += PrintLine(string.Format("{0,3} [Rs. {1} x {2} {3}]", "", data.UnitPrice, data.Quantity, data.UOMName), PrintTextAlignType.Left);
                }
                TextToPrint += PrintLine(string.Format("{0}", new string('-', PrintNoOfCharPerLine)), PrintTextAlignType.Center);
                TextToPrint += PrintLine(string.Format("Total   : {0,10:0.00}", Convert.ToDouble(data.ItemAmount)), PrintTextAlignType.Right);
                TextToPrint += PrintLine(string.Format("Discount: {0,10:0.00}", Convert.ToDouble(data.DiscountAmount)), PrintTextAlignType.Right);
                TextToPrint += PrintLine(string.Format("GST: {0,10:0.00}", Convert.ToDouble(data.GSTAmount)), PrintTextAlignType.Right);
                TextToPrint += PrintLine(string.Format("Extra: {0,10:0.00}", Convert.ToDouble(data.ExtraAmount)), PrintTextAlignType.Right);
                TextToPrint += PrintLine("", PrintTextAlignType.Left);
                TextToPrint += PrintLine(string.Format("Bill Amount : RM .{0:0.00}", Convert.ToDouble(data.TotalAmount)), PrintTextAlignType.Center);
                TextToPrint += PrintLine("", PrintTextAlignType.Left);
                TextToPrint += PrintLine("", PrintTextAlignType.Left);
                TextToPrint += PrintLine("", PrintTextAlignType.Left);


                prnPurchaseOrder.Print();
            }
            else
            {
                frm.Print.FrmQuickSales f = new Print.FrmQuickSales();
                f.LoadReport(data);
                f.ShowDialog();
            }
        }
        private void PrnPurchaseOrder_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            System.Drawing.Font textfont = new System.Drawing.Font("Courier New", 8, System.Drawing.FontStyle.Regular);

            int currentChar = 0;
            int w = 0, h = 0, left = 0, top = 0;
            System.Drawing.Rectangle b = new System.Drawing.Rectangle(left, top, w, h);
            StringFormat format = new StringFormat(StringFormatFlags.LineLimit);
            int line = 0, chars = 0;

            e.Graphics.MeasureString(TextToPrint, textfont, new System.Drawing.SizeF(0, 0), format, out chars, out line);
            e.Graphics.DrawString(TextToPrint.Substring(currentChar, chars), textfont, System.Drawing.Brushes.Black, b, format);

            currentChar = currentChar + chars;
            if (currentChar < TextToPrint.Length)
            {
                e.HasMorePages = true;
            }
            else
            {
                e.HasMorePages = false;
                currentChar = 0;
            }

        }
        #endregion Print

        #region Events
        private void dgvDetails_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                BLL.SalesDetail pod = dgvDetails.SelectedItem as BLL.SalesDetail;
                pod.toCopy<BLL.SalesDetail>(data.SDetail);
            }
            catch (Exception ex) { }

        }

        private void txtBarCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && data.SDetail.ProductId != 0)
            {
                var max = BLL.Product.toList.Where(x => x.Id == data.SDetail.ProductId).Select(x => x.MaxSellingRate).FirstOrDefault();
                var min = BLL.Product.toList.Where(x => x.Id == data.SDetail.ProductId).Select(x => x.MinSellingRate).FirstOrDefault();

                if (data.SDetail.ProductId == 0)
                {
                    MessageBox.Show(string.Format(Message.PL.Empty_Record, "Product"), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
                    cmbItem.Focus();
                }
                else if (min > data.SDetail.UnitPrice || max < data.SDetail.UnitPrice)
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

        #region Combobox Load


        private void cmbPType_Loaded(object sender, RoutedEventArgs e)
        {
            cmbPType.ItemsSource = BLL.TransactionType.toList;
            cmbPType.DisplayMemberPath = "Type";
            cmbPType.SelectedValuePath = "Id";
        }

        private void cmbCustomer_DropDownOpened(object sender, EventArgs e)
        {
            LoadCustomer();
        }

        private void LoadCustomer()
        {
            BLL.Ledger.toList = null;
            try
            {
                cmbCustomer.ItemsSource = BLL.Ledger.toList.Where(x => x.AccountGroup.GroupName == BLL.DataKeyValue.SundryDebtors_Key || x.AccountGroup.GroupName == BLL.DataKeyValue.BranchDivisions_Key).ToList();
                cmbCustomer.DisplayMemberPath = "LedgerName";
                cmbCustomer.SelectedValuePath = "Id";
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(string.Format("Sales Customer Combo box = {0}", ex.Message));
            }
        }

        private void cmbItem_DropDownOpened(object sender, EventArgs e)
        {
            try
            {
                BLL.Product.toList = null;
                cmbItem.ItemsSource = BLL.Product.toList.Where(x => x.StockGroup.IsSale != false).ToList();
                cmbItem.DisplayMemberPath = "ProductName";
                cmbItem.SelectedValuePath = "Id";
            }
            catch(Exception ex)
            {
                Common.AppLib.WriteLog(string.Format("Sales Product List_{0}_{1}", ex.Message, ex.InnerException));
            }
        }

        private void cmbUOM_DropDownOpened(object sender, EventArgs e)
        {
            BLL.UOM.toList = null;
            cmbUOM.ItemsSource = BLL.UOM.toList.ToList();
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

        #endregion

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // lblDiscountAmount.Text = string.Format("{0}({1})", "Discount Amount", AppLib.CurrencyPositiveSymbolPrefix);
            // lblExtraAmount.Text = string.Format("{0}({1})", "Extra Amount", AppLib.CurrencyPositiveSymbolPrefix);

            data.lblDiscount = string.Format("{0}({1})", "Discount Amount", AppLib.CurrencyPositiveSymbolPrefix);
            data.lblExtra = string.Format("{0}({1})", "Extra Amount", AppLib.CurrencyPositiveSymbolPrefix);

            btnSave.Visibility = (BLL.Sale.UserPermission.AllowInsert || BLL.Sale.UserPermission.AllowUpdate) ? Visibility.Visible : Visibility.Collapsed;
            btnDelete.Visibility = BLL.Sale.UserPermission.AllowDelete ? Visibility.Visible : Visibility.Collapsed;
        }

        private void txtChequeNo_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Int32 selectionStart = textBox.SelectionStart;
            Int32 selectionLength = textBox.SelectionLength;
            textBox.Text = AppLib.NumericOnly(txtChequeNo.Text);
            textBox.SelectionStart = selectionStart <= textBox.Text.Length ? selectionStart : textBox.Text.Length;
        }

        private void cmbUOM_Loaded(object sender, RoutedEventArgs e)
        {
            BLL.UOM.toList = null;
            cmbUOM.ItemsSource = BLL.UOM.toList.ToList();
            cmbUOM.DisplayMemberPath = "Symbol";
            cmbUOM.SelectedValuePath = "Id";
        }

        private void cmbCustomer_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCustomer();
        }

        private void dgvDetails_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                BLL.SalesDetail pod = dgvDetails.SelectedItem as BLL.SalesDetail;
                pod.toCopy<BLL.SalesDetail>(data.SDetail);
            }
            catch (Exception ex) { }
        }
    }
}
