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
using Microsoft.Reporting.WinForms;

namespace AccountBuddy.PL.frm.Report
{
    /// <summary>
    /// Interaction logic for frmPRReportPrint.xaml
    /// </summary>
    public partial class frmPRReportPrint : MetroWindow
    {
        public frmPRReportPrint()
        {
            InitializeComponent();
            InitializeComponent();
            RptViewer.SetDisplayMode(DisplayMode.PrintLayout);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {


        }
        public void LoadReport(DateTime dtFrom, DateTime dtTo)
        {
            try
            {
                List<BLL.PurchaseRequestReport> list = BLL.PurchaseRequestReport.ToList(dtFrom, dtTo);
                var list1 = list.Where(x => x.IsReject == false && x.IsApproval == false).ToList();
                List<BLL.RequestReport> l = new List<BLL.RequestReport>();
                BLL.RequestReport bl = new BLL.RequestReport();
                foreach (var l1 in list1)
                {
                    bl = new BLL.RequestReport();
                    bl.Amount = l1.Amount;
                    bl.Department = l1.Department;
                    bl.Particulars = l1.Particulars;
                    bl.RequestAt = l1.RequestAt;
                    bl.RequestBy = l1.RequestBy;
                    bl.RequestTo = l1.RequestTo;
                    bl.Status = l1.Status;
                    l.Add(bl);

                }
                try
                {
                    RptViewer.Reset();
                    ReportDataSource data = new ReportDataSource("RequestReport", l);
                    RptViewer.LocalReport.DataSources.Add(data);
                    RptViewer.LocalReport.ReportPath = @"rpt\Report\rptRequestReport.rdlc";

                    ReportParameter[] par = new ReportParameter[3];
                    par[0] = new ReportParameter("DateFrom", dtFrom.ToString());
                    par[1] = new ReportParameter("DateTo", dtTo.ToString());
                    par[2] = new ReportParameter("Title", "Request Pending");
                    RptViewer.LocalReport.SetParameters(par);

                    RptViewer.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(GetSubReportData);

                    RptViewer.RefreshReport();

                }
                catch (Exception ex)
                {

                }
            }
            catch (Exception ex)
            {

            }

        }
        private void GetSubReportData(object sender, SubreportProcessingEventArgs e)
        {
            e.DataSources.Add(new ReportDataSource("CompanyDetail", BLL.CompanyDetail.toList.Where(x => x.Id == BLL.UserAccount.User.UserType.CompanyId).ToList()));
        }

        public void LoadReport_Rq_Rejected(DateTime dtFrom, DateTime dtTo)
        {
            try
            {
                List<BLL.PurchaseRequestReport> list = BLL.PurchaseRequestReport.ToList(dtFrom, dtTo);
                var list1 = list.Where(x => x.IsReject == true).ToList();
                List<BLL.RequestReport> l = new List<BLL.RequestReport>();
                BLL.RequestReport bl = new BLL.RequestReport();
                foreach (var l1 in list1)
                {
                    bl = new BLL.RequestReport();
                    bl.Amount = l1.Amount;
                    bl.Department = l1.Department;
                    bl.Particulars = l1.Particulars;
                    bl.RequestAt = l1.RequestAt;
                    bl.RequestBy = l1.RequestBy;
                    bl.RequestTo = l1.RequestTo;
                    bl.Status = l1.Status;
                    l.Add(bl);

                }
                try
                {
                    RptViewer.Reset();
                    ReportDataSource data = new ReportDataSource("RequestReport", l);
                    RptViewer.LocalReport.DataSources.Add(data);
                    RptViewer.LocalReport.ReportPath = @"rpt\Report\rptRequestReport.rdlc";

                    ReportParameter[] par = new ReportParameter[3];
                    par[0] = new ReportParameter("DateFrom", dtFrom.ToString());
                    par[1] = new ReportParameter("DateTo", dtTo.ToString());
                    par[2] = new ReportParameter("Title", "Request Rejected");
                    RptViewer.LocalReport.SetParameters(par);

                    RptViewer.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(GetSubReportData);

                    RptViewer.RefreshReport();

                }
                catch (Exception ex)
                {

                }

            }
            catch (Exception ex)
            {
            }
        }



        public void LoadReport_Rq_Approved(DateTime dtFrom, DateTime dtTo)
        {
            try
            {
                List<BLL.PurchaseRequestReport> list = BLL.PurchaseRequestReport.ToList(dtFrom, dtTo);
                var list1 = list.Where(x => x.IsApproval == true).ToList();
                List<BLL.RequestReport> l = new List<BLL.RequestReport>();
                BLL.RequestReport bl = new BLL.RequestReport();
                foreach (var l1 in list1)
                {
                    bl = new BLL.RequestReport();
                    bl.Amount = l1.Amount;
                    bl.Department = l1.Department;
                    bl.Particulars = l1.Particulars;
                    bl.RequestAt = l1.RequestAt;
                    bl.RequestBy = l1.RequestBy;
                    bl.RequestTo = l1.RequestTo;
                    bl.Status = l1.Status;
                    l.Add(bl);

                }
                try
                {
                    RptViewer.Reset();
                    ReportDataSource data = new ReportDataSource("RequestReport", l);
                    RptViewer.LocalReport.DataSources.Add(data);
                    RptViewer.LocalReport.ReportPath = @"rpt\Report\rptRequestReport.rdlc";

                    ReportParameter[] par = new ReportParameter[3];
                    par[0] = new ReportParameter("DateFrom", dtFrom.ToString());
                    par[1] = new ReportParameter("DateTo", dtTo.ToString());
                    par[2] = new ReportParameter("Title", "Approved Request");
                    RptViewer.LocalReport.SetParameters(par);

                    RptViewer.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(GetSubReportData);

                    RptViewer.RefreshReport();

                }
                catch (Exception ex)
                {

                }

            }
            catch (Exception ex)
            {
            }
        }

        public void LoadReport_Budget(DateTime dtFrom, DateTime dtTo)
        {
            try
            {
          
                var list1 = BLL.PurchaseRequestBudgetReport.ToList();
                List<BLL.Budget> l = new List<BLL.Budget>();
                BLL.Budget bl = new BLL.Budget();
                foreach (var l1 in list1)
                {
                    bl = new BLL.Budget();
                    bl.ApprovedAmount = l1.ApprovedAmount;
                    bl.Department = l1.Department;
                    bl.Balance = l1.BalanceAmount;
                    bl.Budgets = l1.BudgetAmount;
                    bl.Remaining = l1.RemainingAmount;
                    bl.Request = l1.RequestAmount;
                    

                    l.Add(bl);

                }
                try
                {
                    RptViewer.Reset();
                    ReportDataSource data = new ReportDataSource("Budget", l);
                    RptViewer.LocalReport.DataSources.Add(data);
                    RptViewer.LocalReport.ReportPath = @"rpt\Report\rptBudgetReport.rdlc";

                    ReportParameter[] par = new ReportParameter[3];
                    par[0] = new ReportParameter("DateFrom", dtFrom.ToString());
                    par[1] = new ReportParameter("DateTo", dtTo.ToString());
                    par[2] = new ReportParameter("Title", "Budget");
                    RptViewer.LocalReport.SetParameters(par);

                    RptViewer.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(GetSubReportData);

                    RptViewer.RefreshReport();

                }
                catch (Exception ex)
                {

                }

            }
            catch (Exception ex)
            {
            }
        }
    }
}
