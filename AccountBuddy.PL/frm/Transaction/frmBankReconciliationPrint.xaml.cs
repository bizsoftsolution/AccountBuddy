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

namespace AccountBuddy.PL.frm.Transaction
{
    /// <summary>
    /// Interaction logic for frmBankReconciliationPrint.xaml
    /// </summary>
    public partial class frmBankReconciliationPrint : MetroWindow
    {
        public frmBankReconciliationPrint()
        {
            InitializeComponent();
            RptViewer.SetDisplayMode(DisplayMode.PrintLayout);
        }
        public void LoadReport(string lName, DateTime dt, List<BLL.BankReconcilation> l1, List<BLL.BankReconcilation> l2, decimal endBal, decimal CLBal)
        {
            try
            {

                try
                {
                    Common.AppLib.WriteLog("Bank Receonciliation Print");
                    RptViewer.Reset();
                    ReportDataSource data1 = new ReportDataSource("BankReconciliation1", l1);
                    ReportDataSource data2 = new ReportDataSource("BankReconciliation2", l2);
                    RptViewer.LocalReport.DataSources.Add(data1);
                    RptViewer.LocalReport.DataSources.Add(data2);
                    RptViewer.LocalReport.ReportPath = @"rpt\Transaction\rptBankReconciliationReport.rdlc";

                    ReportParameter[] par = new ReportParameter[8];
                    par[0] = new ReportParameter("Company", BLL.UserAccount.User.UserType.Company.CompanyName);
                    par[1] = new ReportParameter("EndingBalance", string.Format("{0} {1:N2}", Common.AppLib.CurrencyPositiveSymbolPrefix,endBal));
                    par[2] = new ReportParameter("BankName", lName);
                    par[3] = new ReportParameter("ReportMonth", dt.Date.ToString("MMMM-yy"));
                    par[4] = new ReportParameter("RAmount",string.Format("{0} {1:N2}", Common.AppLib.CurrencyPositiveSymbolPrefix, l1.Sum(x => x.Amount)));
                    par[5] = new ReportParameter("PAmount", string.Format("{0} {1:N2}", Common.AppLib.CurrencyPositiveSymbolPrefix, l2.Sum(x => x.Amount)));
                    par[6] = new ReportParameter("CLBAl", string.Format("{0} {1:N2}", Common.AppLib.CurrencyPositiveSymbolPrefix, CLBal));
                    par[7] = new ReportParameter("AmtPrefix", Common.AppLib.CurrencyPositiveSymbolPrefix);
                    RptViewer.LocalReport.SetParameters(par);

                    RptViewer.RefreshReport();

                }
                catch (Exception ex)
                {
                    Common.AppLib.WriteLog(string.Format("could not load report Bank Receomciliation {0}", ex.InnerException));

                }
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex);
            }

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
