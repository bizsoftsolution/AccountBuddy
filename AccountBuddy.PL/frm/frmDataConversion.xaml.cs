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
using MahApps.Metro.Controls;

namespace AccountBuddy.PL.frm
{
    /// <summary>
    /// Interaction logic for frmDataConversion.xaml
    /// </summary>
    public partial class frmDataConversion : MetroWindow
    {
        public frmDataConversion()
        {
            InitializeComponent();
        }

        private void btnLedger_Click(object sender, RoutedEventArgs e)
        {
            BLL.Ledger data = new BLL.Ledger();
            data.SetLedger();
            dgvDetail.ItemsSource = BLL.Ledger.toList.Select(x=>new { LedgerName = x.LedgerName, Fund = Common.AppLib.FundName });
        }

        private void btnAcountGroups_Click(object sender, RoutedEventArgs e)
        {
         
        }
    }
}
