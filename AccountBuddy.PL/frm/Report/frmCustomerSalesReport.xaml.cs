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

namespace AccountBuddy.PL.frm.Report
{
    /// <summary>
    /// Interaction logic for frmCustomerSalesReport.xaml
    /// </summary>
    public partial class frmCustomerSalesReport : UserControl
    {
        public frmCustomerSalesReport()
        {
            InitializeComponent();
            int yy = BLL.UserAccount.User.UserType.Company.LoginAccYear;

            DateTime? dtFrom = new DateTime(yy, 4, 1);
            DateTime? dtTo = new DateTime(yy + 1, 3, 31);

            dtpDateFrom.SelectedDate = dtFrom;
            dtpDateTo.SelectedDate = dtTo;

        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SetHeading(dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value);
        }
        void SetHeading(DateTime dtFrom, DateTime dtTo)
        {

            for (int i = 1; i <= 12; i++)
            {
                dgvDetails.Columns[i].Visibility = Visibility.Hidden;
            }


            int n = Math.Abs((dtTo.Year * 12 + (dtTo.Month - 1)) - (dtFrom.Year * 12 + (dtFrom.Month - 1)));
            if (rdbYearWise.IsChecked == true) n = n / 12;
            if (n > 12) n = 12;
            for (int i = 0; i < n; i++)
            {
                dgvDetails.Columns[i + 1].Header = rdbMonthlyWise.IsChecked == true ? string.Format("{0:MMM-yyyy}", dtFrom.AddMonths(i)) : string.Format("{0:yyyy}", dtFrom.AddYears(i));
                dgvDetails.Columns[i + 1].Visibility = Visibility.Visible;
            }

        }


        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            SetHeading(dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value);
            bool isMonthly = rdbMonthlyWise.IsChecked == true;

            string ReportType = "";
            if (rdbCustomerWise.IsChecked == true) ReportType = "CustomerWise";
            if (rdbCustomerSummary.IsChecked == true) ReportType = "CustomerSummary";
            if (rdbProductWise.IsChecked == true) ReportType = "ProductWise";
            if (rdbProductSummary.IsChecked == true) ReportType = "ProductSummary";

            dgvDetails.ItemsSource = BLL.SalesReport.ToListCustomerWise(dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value, isMonthly, ReportType);

        }

    }
}
