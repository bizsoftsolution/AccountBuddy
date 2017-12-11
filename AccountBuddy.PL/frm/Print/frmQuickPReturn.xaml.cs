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
using System.Data;

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
                ReportDataSource data2 = new ReportDataSource("PurchaseReturnDetail", GetDetails(data));
                ReportDataSource data3 = new ReportDataSource("CompanyDetail", CList);
                ReportDataSource data4 = new ReportDataSource("Ledger", BLL.Ledger.toList.Where(x=>x.Id==data.LedgerId).ToList());

                rptQuickPR.LocalReport.DataSources.Add(data1);
                rptQuickPR.LocalReport.DataSources.Add(data2);
                rptQuickPR.LocalReport.DataSources.Add(data3);
                rptQuickPR.LocalReport.DataSources.Add(data4);
                rptQuickPR.LocalReport.ReportPath = @"rpt\Transaction\rptPurchaseReturn.rdlc";
                rptQuickPR.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(SetSubDataSource);

                rptQuickPR.RefreshReport();

            }
            catch (Exception ex)
            {

            }
        }

        public void SetSubDataSource(object sender, SubreportProcessingEventArgs e)
        {
            e.DataSources.Add(new ReportDataSource("CompanyDetail", BLL.CompanyDetail.toList.Where(x => x.Id == BLL.UserAccount.User.UserType.Company.Id).ToList())); ;
        }
        public DataTable GetDetails(BLL.PurchaseReturn data)
        {
            int NoRecPerPage = 18;
            var dataSet = new DataSet();
            DataTable dt = new DataTable();
            dataSet.Tables.Add(dt);

            dt.Columns.Add("ProductName");
            dt.Columns.Add("Quantity");
            dt.Columns.Add("UnitPrice");
            dt.Columns.Add("UOMName");
            dt.Columns.Add("Amount");
            dt.Columns.Add("Id");
            dt.Columns.Add("DiscountAmount");

            var newRow = dt.NewRow();


            int n = 0;
            foreach (var element in data.PRDetails)
            {
                newRow = dt.NewRow();
                n = n + 1;
                newRow["ProductName"] = element.ProductName;
                newRow["Quantity"] = element.Quantity == 0 ? "" : element.Quantity.ToString();
                newRow["UnitPrice"] = element.UnitPrice == 0 ? "" : String.Format("{0:0.00}", element.UnitPrice);
                newRow["UOMName"] = element.UOMName;
                newRow["Amount"] = String.Format("{0:0.00}", element.Amount);
                newRow["Id"] = n.ToString();
                newRow["DiscountAmount"] = element.DiscountAmount == 0 ? "" : String.Format("{0:0.00}", element.DiscountAmount);

                dt.Rows.Add(newRow);
            }

           
            for (int i = 0; i < NoRecPerPage - data.PRDetails.Count(); i++)
            {
                newRow = dt.NewRow();

                // fill the properties into the cells
                newRow["ProductName"] = "";
                newRow["Quantity"] = "";
                newRow["UnitPrice"] = "";
                newRow["Amount"] = "";
                newRow["Id"] = "";
                newRow["DiscountAmount"] = "";

                dt.Rows.Add(newRow);
            }
            return dt;

        }
    }
}
