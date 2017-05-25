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
    /// Interaction logic for frmGeneralLedger.xaml
    /// </summary>
    public partial class frmGeneralLedger : UserControl
    {
        public frmGeneralLedger()
        {
            InitializeComponent();
            rptGeneralLedger.SetDisplayMode(DisplayMode.PrintLayout);

            int yy = BLL.UserAccount.User.UserType.Company.LoginAccYear;

            DateTime? dtFrom = new DateTime(yy, 4, 1);
            DateTime? dtTo = new DateTime(yy + 1, 3, 31);

            dtpDateFrom.SelectedDate = dtFrom;
            dtpDateTo.SelectedDate = dtTo;

            cmbAccountName.ItemsSource = BLL.Ledger.toList;
            cmbAccountName.DisplayMemberPath = "AccountName";
            cmbAccountName.SelectedValuePath = "Id";
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            
        }


        private void LoadReport()
        {
            List<BLL.GeneralLedger> list = BLL.GeneralLedger.ToList((int)cmbAccountName.SelectedValue, dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value);
            list = list.Select(x => new BLL.GeneralLedger()
            { AccountName = x.Ledger.AccountName, CrAmt = x.CrAmt, DrAmt = x.DrAmt}).ToList();

            try
            {
                rptGeneralLedger.Reset();
                ReportDataSource data = new ReportDataSource("GeneralLedger", list);
                ReportDataSource data1 = new ReportDataSource("CompanyDetail", BLL.CompanyDetail.toList.Where(x => x.Id == BLL.UserAccount.User.UserType.Company.Id).ToList());
                rptGeneralLedger.LocalReport.DataSources.Add(data);
                rptGeneralLedger.LocalReport.DataSources.Add(data1);
                rptGeneralLedger.LocalReport.ReportPath = @"rpt\Report\rptGeneralLedger.rdlc";

                ReportParameter[] par = new ReportParameter[2];
                par[0] = new ReportParameter("DateFrom", dtpDateFrom.SelectedDate.Value.ToString());
                par[1] = new ReportParameter("DateTo", dtpDateTo.SelectedDate.Value.ToString());
                rptGeneralLedger.LocalReport.SetParameters(par);

                rptGeneralLedger.RefreshReport();

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
            if(cmbAccountName.SelectedValue!=null) dgvGeneralLedger.ItemsSource = BLL.GeneralLedger.ToList((int)cmbAccountName.SelectedValue, dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value);
        }

    }
}
