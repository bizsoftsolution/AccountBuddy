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
    /// Interaction logic for frmReorderLevelReportPrint.xaml
    /// </summary>
    public partial class frmReorderLevelReportPrint : MetroWindow
    {
        public frmReorderLevelReportPrint()
        {
            InitializeComponent();
            RptViewer.SetDisplayMode(DisplayMode.PrintLayout);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {


        }
        public void LoadReport(string Product)
        {
            try
            {
                List<BLL.Product> list = new List<BLL.Product>();
                if (Product != "")
                {
                    list = BLL.Product.toList.Where(x => x.ProductName.ToLower().Contains(Product.ToLower()) && x.IsReOrderLevel == true).ToList();

                }
                else
                {
                    list = BLL.Product.toList.Where(x=> x.IsReOrderLevel == true).ToList();
                }
              
                try
                {
                    RptViewer.Reset();
                    ReportDataSource data = new ReportDataSource("Product", list);
                    ReportDataSource data1 = new ReportDataSource("CompanyDetail", BLL.CompanyDetail.toList.Where(x => x.Id == BLL.UserAccount.User.UserType.CompanyId).ToList());
                    RptViewer.LocalReport.DataSources.Add(data);
                    RptViewer.LocalReport.DataSources.Add(data1);
                    RptViewer.LocalReport.ReportPath = @"rpt\Report\rptReorderLevelReport.rdlc";



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
