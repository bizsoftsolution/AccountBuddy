using MahApps.Metro.Controls;
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
using System.Windows.Shapes;

namespace AccountBuddy.PL.frm.Print
{
    /// <summary>
    /// Interaction logic for frmQuickPayment.xaml
    /// </summary>
    public partial class frmQuickPayment : MetroWindow
    {
        public frmQuickPayment()
        {
            InitializeComponent();
            rptQuickPayment.SetDisplayMode(DisplayMode.PrintLayout);
        }
        public void LoadReport(BLL.Payment data,string Payto, string ChequeNo, string AmtInWords)
        {
            try
            {

                List<BLL.Payment> POList = new List<BLL.Payment>();
                List<BLL.CompanyDetail> CList = new List<BLL.CompanyDetail>();

                POList.Add(data);
                CList.Add(BLL.UserAccount.User.UserType.Company);


                rptQuickPayment.Reset();
                ReportDataSource data1 = new ReportDataSource("Payment", POList);
                ReportDataSource data2 = new ReportDataSource("CompanyDetail", CList);
              
                rptQuickPayment.LocalReport.DataSources.Add(data1);
                rptQuickPayment.LocalReport.DataSources.Add(data2);
                rptQuickPayment.LocalReport.ReportPath = @"rpt\Transaction\rptPaymentvoucher.rdlc";

                ReportParameter[] par = new ReportParameter[3];
                par[0] = new ReportParameter("PayTo", Payto.ToUpper());
                par[1] = new ReportParameter("ChequeNo", ChequeNo);
                par[2] = new ReportParameter("AmtInwords", AmtInWords.ToUpper());
               rptQuickPayment.LocalReport.SetParameters(par);



                rptQuickPayment.RefreshReport();

            }
            catch (Exception ex)
            {

            }
        }
    }
}
