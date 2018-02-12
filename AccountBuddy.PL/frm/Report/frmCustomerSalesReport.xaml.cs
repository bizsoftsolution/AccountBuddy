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
    /// Interaction logic for frmCustomerSalesReport.xaml
    /// </summary>
    public partial class frmCustomerSalesReport : UserControl
    {
        public List<BLL.SalesReportNew> s = new List<BLL.SalesReportNew>();
        bool isMonthly = true, isDate = false, isYear = false;

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
            //SetHeading(dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value);
            s = BLL.SalesReportNew.ToList(dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value, (bool)rdbMonthlyWise.IsChecked, (bool)rdbDayWise.IsChecked, (bool)rdbYearWise.IsChecked, "Customer").ToList();
            LoadReport();

        }

        public void LoadReport()
        {
            try
            {
                rptViewer.Reset();

                ReportDataSource data = new ReportDataSource("SalesReportNew", s);
                ReportDataSource data1 = new ReportDataSource("CompanyDetail", BLL.CompanyDetail.ToList.Where(x => x.Id == BLL.UserAccount.User.UserType.CompanyId).ToList());
                rptViewer.LocalReport.DataSources.Add(data);
                rptViewer.LocalReport.DataSources.Add(data1);
                rptViewer.LocalReport.ReportPath = @"rpt\Report\rptSalesReport.rdlc";
                ReportParameter[] rp = new ReportParameter[9];

                if (rdbCustomerSummary.IsChecked == true)
                {
                    rp[0] = new ReportParameter("Customer", false.ToString());
                    rp[1] = new ReportParameter("Product", true.ToString());
                    rp[2] = new ReportParameter("CustomerWise", true.ToString());
                    rp[3] = new ReportParameter("ProductWise", true.ToString());

                }
                else if (rdbProductSummary.IsChecked == true)
                {
                    rp[0] = new ReportParameter("Customer", true.ToString());
                    rp[1] = new ReportParameter("Product", false.ToString());
                    rp[2] = new ReportParameter("CustomerWise", true.ToString());
                    rp[3] = new ReportParameter("ProductWise", true.ToString());

                }
                else if (rdbProductWise.IsChecked == true)
                {
                    rp[0] = new ReportParameter("Customer", true.ToString());
                    rp[1] = new ReportParameter("Product", true.ToString());
                    rp[2] = new ReportParameter("CustomerWise", true.ToString());
                    rp[3] = new ReportParameter("ProductWise", false.ToString());

                }
                else if (rdbCustomerWise.IsChecked == true)
                {
                    rp[0] = new ReportParameter("Customer", true.ToString());
                    rp[1] = new ReportParameter("Product", true.ToString());
                    rp[2] = new ReportParameter("CustomerWise", false.ToString());
                    rp[3] = new ReportParameter("ProductWise", true.ToString());
                }
                rp[4] = new ReportParameter("DateFrom", dtpDateFrom.SelectedDate.ToString());
                rp[5] = new ReportParameter("DateTo", dtpDateTo.SelectedDate.ToString());
                rp[6] = new ReportParameter("Title", lblHeader.Content.ToString());
                rp[7] = new ReportParameter("AmtPrefix", Common.AppLib.CurrencyPositiveSymbolPrefix);
                rp[8] = new ReportParameter("ColumnName", "Customer Name");

                rptViewer.LocalReport.SetParameters(rp);
                rptViewer.RefreshReport();
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex);
            }

        }

        void SetHeading(DateTime dtFrom, DateTime dtTo)
        {

            //for (int i = 1; i <= 12; i++)
            //{
            //    dgvDetails.Columns[i].Visibility = Visibility.Hidden;
            //}


            //int n = Math.Abs((dtTo.Year * 12 + (dtTo.Month - 1)) - (dtFrom.Year * 12 + (dtFrom.Month - 1)));
            //if (rdbYearWise.IsChecked == true) n = n / 12;
            //if (n > 12) n = 12;
            //for (int i = 0; i <= n; i++)
            //{
            //    dgvDetails.Columns[i + 1].Header = rdbMonthlyWise.IsChecked == true ? string.Format("{0:MMM-yyyy}", dtFrom.AddMonths(i)) : string.Format("{0:yyyy}", dtFrom.AddYears(i));
            //    dgvDetails.Columns[i + 1].Visibility = Visibility.Visible;
            //}

        }


        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            //SetHeading(dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value);
            //bool isMonthly = rdbMonthlyWise.IsChecked == true;

            s = BLL.SalesReportNew.ToList(dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value, (bool)rdbMonthlyWise.IsChecked, (bool)rdbDayWise.IsChecked, (bool)rdbYearWise.IsChecked, "Customer").ToList();
            LoadReport();
        }

        private void rdbCustomerWise_Checked(object sender, RoutedEventArgs e)
        {
            LoadReport();
        }

        private void rdbCustomerWise_Unchecked(object sender, RoutedEventArgs e)
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

        private void rdbCustomerSummary_Checked(object sender, RoutedEventArgs e)
        {
            LoadReport();
        }

        private void rdbCustomerSummary_Unchecked(object sender, RoutedEventArgs e)
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

        private void rdbDayWise_Checked(object sender, RoutedEventArgs e)
        {
            isDate = true; LoadReport1();
        }

        private void LoadReport1()
        {
            if (dtpDateFrom.SelectedDate != null || dtpDateTo.SelectedDate != null)
            {
                s = BLL.SalesReportNew.ToList(dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value, isMonthly, isDate, isYear, "Customer").ToList();
                LoadReport();
            }


        }

        private void rdbDayWise_Unchecked(object sender, RoutedEventArgs e)
        {
            isDate = false; LoadReport1();
        }

        private void rdbMonthlyWise_Checked(object sender, RoutedEventArgs e)
        {
            isMonthly = true; LoadReport1();
        }

        private void rdbMonthlyWise_Unchecked(object sender, RoutedEventArgs e)
        {
            isMonthly = false; LoadReport1();
        }

        private void rdbYearWise_Checked(object sender, RoutedEventArgs e)
        {
            isYear = true; LoadReport1();
        }

        private void rdbYearWise_Unchecked(object sender, RoutedEventArgs e)
        {
            isYear = false; LoadReport1();
        }
    }
}
