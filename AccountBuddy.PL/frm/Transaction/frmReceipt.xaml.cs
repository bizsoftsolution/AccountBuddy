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

namespace AccountBuddy.PL.frm.Transaction
{
    /// <summary>
    /// Interaction logic for frmReceipt.xaml
    /// </summary>
    public partial class frmReceipt : UserControl
    {
        BLL.Receipt data = new BLL.Receipt();
        public frmReceipt()
        {
            InitializeComponent();
            this.DataContext = data;
            data.Clear();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (data.RDetail.LedgerId == 0)
            {
                MessageBox.Show("Enter LedgerName");
            }
            else if (data.RDetail.Amount == 0)
            {
                MessageBox.Show("Enter Amount");
            }
            else
            {
                data.SaveDetail();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (data.EntryNo == null)
            {
                MessageBox.Show("Enter Entry No");
            }
            else if (data.LedgerId == 0)
            {
                MessageBox.Show("Enter LedgerName");
            }
            else if (data.ReceiptMode == null)
            {
                MessageBox.Show("select Paymode");
            }

            else if (data.RDetails.Count == 0)
            {
                MessageBox.Show("Enter Receipt");
            }
            else if (data.FindRefNo() == false)
            {
                var rv = data.Save();
                if (rv == true)
                {
                    MessageBox.Show("Saved");
                    data.Clear();
                }
            }
            else
            {
                MessageBox.Show("RefNo Already Exist");

            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Do you want to delete?", "DELETE", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var rv = data.Delete();
                if (rv == true)
                {
                    MessageBox.Show("Deleted");
                    data.Clear();
                }
            }

        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            data.Clear();
        }

        private void btnsearch_Click(object sender, RoutedEventArgs e)
        {
            var rv = data.Find();
            if (rv == false) MessageBox.Show(String.Format("Data Not Found"));
        }

        private void dgvDetails_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                BLL.ReceiptDetail pod = dgvDetails.SelectedItem as BLL.ReceiptDetail;
                pod.toCopy<BLL.ReceiptDetail>(data.RDetail);
            }
            catch (Exception ex) { }

        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            frm.Vouchers.frmQuickReceipt f = new Vouchers.frmQuickReceipt();

            f.LoadReport(data, txtReceivedFrom.Text.ToString());
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
                if (MessageBox.Show("do you want to delete this detail?", "Delete", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Button btn = (Button)sender;
                    data.DeleteDetail((int)btn.Tag);
                }
            }
            catch (Exception ex) { }

        }

        private void btnDClear_Click(object sender, RoutedEventArgs e)
        {
            data.ClearDetail();
        }

    }
}
