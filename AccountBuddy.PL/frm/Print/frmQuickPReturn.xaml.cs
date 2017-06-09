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
    /// Interaction logic for frmQuickPReturn.xaml
    /// </summary>
    public partial class frmQuickPReturn : MetroWindow
    {
        public frmQuickPReturn()
        {
            InitializeComponent();
            rptQuickPR.SetDisplayMode(DisplayMode.PrintLayout);
        }
        public void LoadReport(BLL.PurchaseReturn data)
        {
            try
            {

                List<BLL.PurchaseReturn> POList = new List<BLL.PurchaseReturn>();
                List<BLL.PurchaseReturnDetail> PODList = new List<BLL.PurchaseReturnDetail>();
                List<BLL.CompanyDetail> CList = new List<BLL.CompanyDetail>();

                POList.Add(data);
                PODList.AddRange(data.PRDetails);
                CList.Add(BLL.UserAccount.User.UserType.Company);


                rptQuickPR.Reset();
                ReportDataSource data1 = new ReportDataSource("PurchaseReturn", POList);
                ReportDataSource data2 = new ReportDataSource("PurchaseReturnDetail", PODList);
                ReportDataSource data3 = new ReportDataSource("CompanyDetail", CList);

                rptQuickPR.LocalReport.DataSources.Add(data1);
                rptQuickPR.LocalReport.DataSources.Add(data2);
                rptQuickPR.LocalReport.DataSources.Add(data3);
                rptQuickPR.LocalReport.ReportPath = @"rpt\Transaction\rptPurchaseReturn.rdlc";

                rptQuickPR.RefreshReport();

            }
            catch (Exception ex)
            {

            }
        }
    }
}
