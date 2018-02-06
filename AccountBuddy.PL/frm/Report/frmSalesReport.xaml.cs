using Microsoft.Reporting.WinForms;
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
    /// Interaction logic for frmSalesReport.xaml
    /// </summary>
    public partial class frmSalesReport : UserControl
    {
        public frmSalesReport()
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
            for (int i = 0; i <= n; i++)
            {
                dgvDetails.Columns[i + 1].Header = rdbMonthlyWise.IsChecked == true ? string.Format("{0:MMM-yyyy}", dtFrom.AddMonths(i)) : string.Format("{0:yyyy}", dtFrom.AddYears(i));
                dgvDetails.Columns[i + 1].Visibility = Visibility.Visible;
            }
            LoadReport();
        }

        private void LoadReport()
        {
            try
            {
                rptViewer.Reset();

                List<BLL.SalesReportNew> s = BLL.SalesReportNew.ToList(dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value, (bool)rdbMonthlyWise.IsChecked, "DealerWise").ToList();

                ReportDataSource data = new ReportDataSource("SalesReportNew", s);
                //ReportDataSource data1 = new ReportDataSource("CompanyDetail", BLL.CompanyDetail.toList.Where(x => x.Id == BLL.UserAccount.User.UserType.CompanyId).ToList());
                rptViewer.LocalReport.DataSources.Add(data);
                //rptViewer.LocalReport.DataSources.Add(data1);
                rptViewer.LocalReport.ReportPath = @"rpt\Report\rptSalesReport.rdlc";
                ReportParameter[] rp = new ReportParameter[4];

                if (rdbDealerSummary.IsChecked == true)
                {
                    rp[0] = new ReportParameter("Customer", true.ToString());
                    rp[1] = new ReportParameter("Product", false.ToString());
                    rp[2] = new ReportParameter("CustomerWise", false.ToString());
                    rp[3] = new ReportParameter("ProductWise", false.ToString());

                }
                else if (rdbProductSummary.IsChecked == true)
                {
                    rp[0] = new ReportParameter("Customer", false.ToString());
                    rp[1] = new ReportParameter("Product", true.ToString());
                    rp[2] = new ReportParameter("CustomerWise", false.ToString());
                    rp[3] = new ReportParameter("ProductWise", false.ToString());

                }
                else if (rdbProductWise.IsChecked == true)
                {
                    rp[0] = new ReportParameter("Customer", false.ToString());
                    rp[1] = new ReportParameter("Product", false.ToString());
                    rp[2] = new ReportParameter("CustomerWise", false.ToString());
                    rp[3] = new ReportParameter("ProductWise", true.ToString());

                }
                else if (rdbDealerWise.IsChecked == true)
                {
                    rp[0] = new ReportParameter("Customer", false.ToString());
                    rp[1] = new ReportParameter("Product", false.ToString());
                    rp[2] = new ReportParameter("CustomerWise", true.ToString());
                    rp[3] = new ReportParameter("ProductWise", false.ToString());
                }

                rptViewer.LocalReport.SetParameters(rp);
                rptViewer.RefreshReport();
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex);
            }

        }


        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            SetHeading(dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value);
            bool isMonthly = rdbMonthlyWise.IsChecked == true;

            string ReportType = "";
            if (rdbDealerWise.IsChecked == true) ReportType = "DealerWise";
            if (rdbDealerSummary.IsChecked == true) ReportType = "DealerSummary";
            if (rdbProductWise.IsChecked == true) ReportType = "ProductWise";
            if (rdbProductSummary.IsChecked == true) ReportType = "ProductSummary";

            dgvDetails.ItemsSource = BLL.SalesReport.ToList(dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value, isMonthly, ReportType);
            LoadReport();

        }

        private void rdbDealerWise_Checked(object sender, RoutedEventArgs e)
        {
            LoadReport();
        }

        private void rdbDealerWise_Unchecked(object sender, RoutedEventArgs e)
        {
            LoadReport();
        }

        private void rdbProductWise_Checked(object sender, RoutedEventArgs e)
        {
            LoadReport();
        }

        private void rdbProductWise_Unchecked(object sender, RoutedEventArgs e)
        {
            LoadReport();
        }

        private void rdbDealerSummary_Checked(object sender, RoutedEventArgs e)
        {
            LoadReport();
        }

        private void rdbDealerSummary_Unchecked(object sender, RoutedEventArgs e)
        {
            LoadReport();
        }

        private void rdbProductSummary_Checked(object sender, RoutedEventArgs e)
        {
            LoadReport();
        }

        private void rdbProductSummary_Unchecked(object sender, RoutedEventArgs e)
        {
            LoadReport();
        }
    }
}
