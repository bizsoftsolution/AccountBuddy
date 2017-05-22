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
using Microsoft.Reporting.WinForms;

namespace AccountBuddy.PL.frm.Report
{
    /// <summary>
    /// Interaction logic for frmTrialBalance.xaml
    /// </summary>
    public partial class frmTrialBalance : UserControl
    {
        public frmTrialBalance()
        {
            InitializeComponent();
            rptTrialBalance.SetDisplayMode(DisplayMode.PrintLayout);

            int yy = BLL.UserAccount.Company.LoginAccYear;

            DateTime? dtFrom=new DateTime(yy,4,1);
            DateTime? dtTo =new DateTime(yy+1,3,31);
            
            dtpDateFrom.SelectedDate = dtFrom;
            dtpDateTo.SelectedDate = dtTo;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dgvTrialBalance.ItemsSource = BLL.TrialBalance.ToList(dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value);        
        }


        private void LoadReport()
        {
            List<BLL.TrialBalance> list = BLL.TrialBalance.ToList(dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value);
            list = list.Select(x => new BLL.TrialBalance()
            { AccountName = x.Ledger.AccountName, CrAmt = x.CrAmt, DrAmt = x.DrAmt, CrAmtOP = x.CrAmtOP, DrAmtOP = x.DrAmtOP }).ToList();

            try
            {
                rptTrialBalance.Reset();
                ReportDataSource data = new ReportDataSource("TrialBalance", list);
                ReportDataSource data1 = new ReportDataSource("CompanyDetail", BLL.CompanyDetail.toList.Where(x => x.Id == BLL.UserAccount.Company.Id).ToList());
                rptTrialBalance.LocalReport.DataSources.Add(data);
                rptTrialBalance.LocalReport.DataSources.Add(data1);
                rptTrialBalance.LocalReport.ReportPath = @"rpt\Report\rptTrialBalance.rdlc";

                ReportParameter[] par = new ReportParameter[2];
                par[0] = new ReportParameter("DateFrom", dtpDateFrom.SelectedDate.Value.ToString());
                par[1] = new ReportParameter("DateTo", dtpDateTo.SelectedDate.Value.ToString());
                rptTrialBalance.LocalReport.SetParameters(par);


                rptTrialBalance.RefreshReport();

            }
            catch (Exception ex)
            {

            }


        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabControl tc = sender as TabControl;

            if (tc.SelectedIndex == 1)
            {
                LoadReport();
            }

        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            dgvTrialBalance.ItemsSource = BLL.TrialBalance.ToList(dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value);
        }
    }
}
