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
using AccountBuddy.Common;
using MahApps.Metro.Controls;
using Microsoft.Reporting.WinForms;

namespace AccountBuddy.PL.frm.Transaction
{
    /// <summary>
    /// Interaction logic for frmLedgerOpeningPrint.xaml
    /// </summary>
    public partial class frmLedgerOpeningPrint : MetroWindow
    {
        public frmLedgerOpeningPrint()
        {
            InitializeComponent();
            RptLedger.SetDisplayMode(DisplayMode.PrintLayout);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadReport();
        }

        public void LoadReport()
        {
            try
            {
                RptLedger.Reset();
                ReportDataSource data = new ReportDataSource("Ledger", BLL.Ledger.toList);
                ReportDataSource data1 = new ReportDataSource("CompanyDetail", BLL.CompanyDetail.toList.Where(x => x.Id == BLL.UserAccount.User.UserType.Company.Id).ToList());
                RptLedger.LocalReport.DataSources.Add(data);
                RptLedger.LocalReport.DataSources.Add(data1);
                RptLedger.LocalReport.ReportPath = @"rpt\Transaction\rptLedgerOpening.rdlc";

                ReportParameter[] rp = new ReportParameter[1];
                rp[0] = new ReportParameter("AmtPrefix", AppLib.CurrencyPositiveSymbolPrefix);
                RptLedger.LocalReport.SetParameters(rp);

                RptLedger.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(SetSubDataSource);


                RptLedger.RefreshReport();

            }
            catch (Exception ex)
            {

            }


        }

        public void SetSubDataSource(object sender, SubreportProcessingEventArgs e)
        {
            e.DataSources.Add(new ReportDataSource("CompanyDetail", BLL.CompanyDetail.toList.Where(x => x.Id == BLL.UserAccount.User.UserType.Company.Id).ToList())); ;
        }
    }
}
