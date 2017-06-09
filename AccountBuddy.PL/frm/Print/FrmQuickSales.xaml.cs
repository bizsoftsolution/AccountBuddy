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
    /// Interaction logic for FrmQuickSales.xaml
    /// </summary>
    public partial class FrmQuickSales : MetroWindow
    {
        public FrmQuickSales()
        {
            InitializeComponent();
          rptQuickSales.SetDisplayMode(DisplayMode.PrintLayout);
        }
        public void LoadReport(BLL.Sale data)
        {
            try
            {

                List<BLL.Sale> POList = new List<BLL.Sale>();
                List<BLL.SalesDetail> PODList = new List<BLL.SalesDetail>();
                List<BLL.CompanyDetail> CList = new List<BLL.CompanyDetail>();
                List<BLL.Ledger> CustomerList = new List<BLL.Ledger>();

                POList.Add(data);
                PODList.AddRange(data.SDetails);
                CList.Add(BLL.UserAccount.User.UserType.Company);
                CustomerList.Add(BLL.Ledger.toList.Where(x => x.Id == data.LedgerId).FirstOrDefault());


                rptQuickSales.Reset();
                ReportDataSource data1 = new ReportDataSource("Sale", POList);
                ReportDataSource data2 = new ReportDataSource("SalesDetail", PODList);
                ReportDataSource data3 = new ReportDataSource("CompanyDetail", CList);
                ReportDataSource data4 = new ReportDataSource("Ledger", CustomerList);


                rptQuickSales.LocalReport.DataSources.Add(data1);
                rptQuickSales.LocalReport.DataSources.Add(data2);
                rptQuickSales.LocalReport.DataSources.Add(data3);
                rptQuickSales.LocalReport.DataSources.Add(data4);
                rptQuickSales.LocalReport.ReportPath = @"rpt\Transaction\rptQuickSales.rdlc";

                rptQuickSales.RefreshReport();

            }
            catch (Exception ex)
            {

            }
        }

    }
}
