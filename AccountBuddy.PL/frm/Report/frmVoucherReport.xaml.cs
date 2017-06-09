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
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace FMCG.PL.frm.Report
{
    /// <summary>
    /// Interaction logic for frmVoucherReport.xaml
    /// </summary>
    public partial class frmVoucherReport : UserControl
    {
        public static ObservableCollection<Payable> list = new ObservableCollection<Payable>();
        public frmVoucherReport()
        {
            InitializeComponent();
            list.Add(new Payable { VoucherName = "Test", Amount = 1000, Date = DateTime.Today });
            list.Add(new Payable { VoucherName = "TestSupplier", Amount = 2000, Date = DateTime.Today });
            dgvPayableSupplier.ItemsSource = list;

        }
    }
    public class Payable

    {
        public string VoucherName { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date
        {
            get; set;
        }
    }
}
