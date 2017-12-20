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
    /// Interaction logic for frmPOPendingPrint.xaml
    /// </summary>
    public partial class frmPOPendingPrint : MetroWindow
    {
        public static int yy = BLL.UserAccount.User.UserType.Company.LoginAccYear;

        DateTime? dtFrom = new DateTime(yy, 4, 1);
        DateTime? dtTo = new DateTime(yy + 1, 3, 31);

        public frmPOPendingPrint()
        {
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
                List<BLL.POPending> list = BLL.POPending.ToList(dtFrom, dtTo);
                list = list.Select(x => new BLL.POPending()
                { AccountName = x.Ledger.AccountName, Amount = x.Amount, EntryNo = x.EntryNo, Ledger = x.Ledger, PODate = x.PODate, Status = x.Status }).ToList();

                try
                {
                    RptViewer.Reset();
                    ReportDataSource data = new ReportDataSource("POPending", list);
                    ReportDataSource data1 = new ReportDataSource("CompanyDetail", BLL.CompanyDetail.toList.Where(x => x.Id == BLL.UserAccount.User.UserType.CompanyId).ToList());
                    RptViewer.LocalReport.DataSources.Add(data);
                    RptViewer.LocalReport.DataSources.Add(data1);
                    RptViewer.LocalReport.ReportPath = @"rpt\Report\rptPOPendingReport.rdlc";

                    ReportParameter[] par = new ReportParameter[3];
                    par[0] = new ReportParameter("DateFrom", dtFrom.ToString());
                    par[1] = new ReportParameter("DateTo", dtTo.ToString());
                    par[2] = new ReportParameter("AmtPrefix", Common.AppLib.CurrencyPositiveSymbolPrefix.ToString());
                    RptViewer.LocalReport.SetParameters(par);
                    RptViewer.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(SetSubDataSource);
                    RptViewer.RefreshReport();

                }
                catch (Exception ex)
                {
                    Common.AppLib.WriteLog(ex);
                }
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex);
            }

        }
        public void SetSubDataSource(object sender, SubreportProcessingEventArgs e)
        {
            e.DataSources.Add(new ReportDataSource("CompanyDetail", BLL.CompanyDetail.toList.Where(x => x.Id == BLL.UserAccount.User.UserType.Company.Id).ToList())); ;
        }
    }
}
