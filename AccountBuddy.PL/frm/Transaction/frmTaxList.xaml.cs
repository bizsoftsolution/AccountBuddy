using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for frmTaxList.xaml
    /// </summary>
    public partial class frmTaxList :MetroWindow
    {

        public decimal ItemAmount = 0;
        public decimal DiscountAmount = 0;

        public frmTaxList()
        {
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void ckbStatus_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = e.Source as CheckBox;
            var t = cb.Tag as BLL.TaxMaster;
            t.Status = true;
            t.TaxAmount = BLL.TaxMaster.GetGST(ItemAmount, DiscountAmount, t.TaxPercentage);
            ShowGSTAmount();
        }

        private void ckbStatus_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = e.Source as CheckBox;
            var t = cb.Tag as BLL.TaxMaster;
            t.Status = false;
            t.TaxAmount = 0;
            ShowGSTAmount();
        }

        void ShowGSTAmount()
        {
           
            try
            {
                var l1 = dgvTax.ItemsSource as ObservableCollection<BLL.TaxMaster>;
                var amt = l1.Sum(x => x.TaxAmount);
                lblGSTAmount.Content = string.Format("{0:N2}", amt);
                lblDiscountAmount.Content = string.Format("{0:N2}", DiscountAmount);
                lblItemAmount.Content = string.Format("{0:N2}", ItemAmount);
            }

            catch (Exception ex)
            { Common.AppLib.WriteLog(ex); }
        }
    }
}
