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


namespace FMCG.PL.frm.Report
{
    /// <summary>
    /// Interaction logic for frmPOPending.xaml
    /// </summary>
    public partial class frmPOPending : UserControl
    {
        public List<POPending> list = new List<POPending>();

        public frmPOPending()
        {
            InitializeComponent();
            list.Add(new POPending { PONo = "PO001", RefNo = "REF001", Date = "25-04-2017", Product = "Biscuit", PurQty = "10", PenQty = "5" });
            list.Add(new POPending { PONo = "PO003", RefNo = "REF003", Date = "27-04-2017", Product = "Chocolate", PurQty = "100", PenQty = "50" });
            dgvDetails.ItemsSource = list;
        }
    }

    public class POPending
    {
        public string PONo { get; set; }
        public string RefNo { get; set; }
        public string Date { get; set; }
        public string Product { get; set; }
        public string PurQty { get; set; }
        public string PenQty { get; set; }

    }
}
