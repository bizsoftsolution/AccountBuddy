using MahApps.Metro.Controls;
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
using System.Windows.Shapes;

namespace AccountBuddy.PL.frm.Transaction
{
    /// <summary>
    /// Interaction logic for frmPSSearch.xaml
    /// </summary>
    public partial class frmPSSearch : MetroWindow
    {
        decimal amtfrom = 0, amtTo = 99999999;

        public frmPSSearch()
        {
            InitializeComponent();
            dtpDateFrom.SelectedDate = DateTime.Now;
            dtpDateTo.SelectedDate = DateTime.Now;
        }


        private void dgvDetails_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var rp = dgvDetails.SelectedItem as BLL.Purchase;
            if (rp != null)
            {
                Transaction.frmPurchase f = App.frmHome.ShowForm(Common.Forms.frmPurchase) as Transaction.frmPurchase;

                System.Windows.Forms.Application.DoEvents();
                f.data.RefNo = rp.RefNo;
                f.data.SearchText = rp.RefNo;
                f.btnPrint.IsEnabled = true;
                f.data.Find();
                if (f.data.RefCode != null)
                {
                    f.btnSave.IsEnabled = true;
                    f.btnDelete.IsEnabled = true;
                }

                f.btnPrint.IsEnabled = true;

                System.Windows.Forms.Application.DoEvents();
                this.Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            try
            {
                var d = BLL.Product_Spec_Process.ToList((int?)cmbProduct.SelectedValue, dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value);
                dgvDetails.ItemsSource = d;
            }
            catch (Exception ex)
            { Common.AppLib.WriteLog(ex); }
        }

        private void cmbProduct_Loaded(object sender, RoutedEventArgs e)
        {
            var p = BLL.Product_Spec_Process.toList.Select(x => x.Product).ToList();
            List<BLL.Product> s = new List<BLL.Product>();
            foreach (var p1 in p)
            {
                BLL.Product s1 = new BLL.Product();
                s1.ProductName = p1.ProductName;
                s1.Id = p1.Id;
                s.Add(s1);
            }
            cmbProduct.ItemsSource = s;
            cmbProduct.DisplayMemberPath = "ProductName";
            cmbProduct.SelectedValuePath = "Id";
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {

            var d = BLL.Product_Spec_Process.ToList((int?)cmbProduct.SelectedValue, dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value);
            dgvDetails.ItemsSource = d;
        }

    }
}
