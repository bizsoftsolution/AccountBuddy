using AccountBuddy.Common;
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
        BLL.Sale data = new BLL.Sale();
        string TextToPrint = "";
        public frmSale()
        {
            InitializeComponent();
            this.DataContext = data;

          

            cmbPType.ItemsSource = BLL.TransactionType.toList;
            cmbPType.DisplayMemberPath = "Type";
            cmbPType.SelectedValuePath = "Id";

            cmbItem.ItemsSource = BLL.Product.toList;
            cmbItem.DisplayMemberPath = "ProductName";
            cmbItem.SelectedValuePath = "Id";

            cmbUOM.ItemsSource = BLL.UOM.toList;
            cmbUOM.DisplayMemberPath = "Symbol";
            cmbUOM.SelectedValuePath = "Id";



            data.Clear();

        }

        #region Events

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (data.SDetail.ProductId == 0)
            {
                MessageBox.Show("Empty Record");
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
            else if (data.LedgerId == 0)
            {
                MessageBox.Show("Enter Supplier");

            }
            else if (data.TransactionTypeId == 0)
            {
                MessageBox.Show("Enter Transaction Type");
            }
            else if (data.SDetails.Count == 0)
            {
                MessageBox.Show("Enter Product Details");
            }
            else if (data.FindRefNo() == false)
            {
                var rv = data.Save();
                if (rv == true)
                {
                    MessageBox.Show("Saved Successfully");
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
                TextToPrint += PrintLine(string.Format("{0},", BLL.UserAccount.User.UserType.Company.AddressLine1), PrintTextAlignType.Center);
                TextToPrint += PrintLine(string.Format("{0},", BLL.UserAccount.User.UserType.Company.AddressLine2), PrintTextAlignType.Center);
                TextToPrint += PrintLine(string.Format("{0},", BLL.UserAccount.User.UserType.Company.CityName), PrintTextAlignType.Center);

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

        private void btnsearch_Click(object sender, RoutedEventArgs e)
        {
            var rv = data.Find();
            if (rv == false) MessageBox.Show(String.Format("{0} is not found", data.SearchText));
        }

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
            if (e.Key == Key.Return && data.SDetail.ProductId != null)
            {
                data.SaveDetail();
            }
        }


        #endregion

        #region Methods

        #endregion

        private void cmbCustomer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }


      

        private void cmbCustomer_Loaded(object sender, RoutedEventArgs e)
        {
            cmbCustomer.ItemsSource = BLL.Ledger.toList.Where(x => x.AccountGroup.GroupName == BLL.DataKeyValue.SundryDebtors_Key).ToList(); ;
            cmbCustomer.DisplayMemberPath = "LedgerName";
            cmbCustomer.SelectedValuePath = "Id";

        }
    }
}
