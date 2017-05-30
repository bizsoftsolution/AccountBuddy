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

namespace AccountBuddy.PL.frm.Transaction
{
    /// <summary>
    /// Interaction logic for frmBankReconcilation.xaml
    /// </summary>
    public partial class frmBankReconcilation : UserControl
    {
        public frmBankReconcilation()
        {
            InitializeComponent();
          
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (cmbAccountName.SelectedValue != null) dgvDetails.ItemsSource = BLL.BankReconcilation.ToList((int)cmbAccountName.SelectedValue, dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value);

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            int yy = BLL.UserAccount.User.UserType.Company.LoginAccYear;

            DateTime? dtFrom = new DateTime(yy, 4, 1);
            DateTime? dtTo = new DateTime(yy + 1, 3, 31);

            dtpDateFrom.SelectedDate = dtFrom;
            dtpDateTo.SelectedDate = dtTo;

            cmbAccountName.ItemsSource = BLL.Ledger.toList;
            cmbAccountName.DisplayMemberPath = "AccountName";
            cmbAccountName.SelectedValuePath = "Id";
        }
    }
}
