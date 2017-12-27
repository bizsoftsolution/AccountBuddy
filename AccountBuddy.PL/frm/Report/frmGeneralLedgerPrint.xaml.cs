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
    /// Interaction logic for frmGeneralLedgerPrint.xaml
    /// </summary>
    public partial class frmGeneralLedgerPrint : MetroWindow
    {
        public static int yy = BLL.UserAccount.User.UserType.Company.LoginAccYear;

        DateTime? dtFrom = new DateTime(yy, 4, 1);
        DateTime? dtTo = new DateTime(yy + 1, 3, 31);

        public frmGeneralLedgerPrint()
        {
            InitializeComponent();
            RptViewer.SetDisplayMode(DisplayMode.PrintLayout);

          
        }
        public void LoadReport(int? LID, DateTime dtFrom, DateTime dtTo)
        {
            try
            {
                List<BLL.GeneralLedger> list = BLL.GeneralLedger.ToList((int)LID, dtFrom, dtTo);
                list = list.Select(x => new BLL.GeneralLedger()
                { AccountName = x.Ledger.AccountName,Particular=x.Particular, CrAmt = x.CrAmt, DrAmt = x.DrAmt, BalAmt = x.BalAmt, EDate = x.EDate, EntryNo = x.EntryNo, EType = x.EType, Ledger = x.Ledger, RefNo = x.RefNo }).ToList();

                try
                {
                    RptViewer.Reset();
                    ReportDataSource data = new ReportDataSource("GeneralLedger", list);
                    ReportDataSource data1 = new ReportDataSource("CompanyDetail", BLL.CompanyDetail.ToList.Where(x => x.Id == BLL.UserAccount.User.UserType.CompanyId).ToList());
                    RptViewer.LocalReport.DataSources.Add(data);
                    RptViewer.LocalReport.DataSources.Add(data1);
                    RptViewer.LocalReport.ReportPath = @"rpt\Report\rptGeneralLedger.rdlc";

                    ReportParameter[] par = new ReportParameter[3];
                    par[0] = new ReportParameter("DateFrom", dtFrom.ToString());
                    par[1] = new ReportParameter("DateTo", dtTo.ToString());
                    par[2] = new ReportParameter("AmtPrefix", Common.AppLib.CurrencyPositiveSymbolPrefix);
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
            e.DataSources.Add(new ReportDataSource("CompanyDetail", BLL.CompanyDetail.ToList.Where(x => x.Id == BLL.UserAccount.User.UserType.Company.Id).ToList())); ;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
