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
    /// Interaction logic for frmQuickSReturn.xaml
    /// </summary>
    public partial class frmQuickSReturn : MetroWindow
    {
        public frmQuickSReturn()
        {
            InitializeComponent();
            rptQuickSalesReturn.SetDisplayMode(DisplayMode.PrintLayout);
        }
        public void LoadReport(BLL.SalesReturn data)
        {
            try
            {

                List<BLL.SalesReturn> POList = new List<BLL.SalesReturn>();
                List<BLL.SalesReturnDetail> PODList = new List<BLL.SalesReturnDetail>();
                List<BLL.CompanyDetail> CList = new List<BLL.CompanyDetail>();

                POList.Add(data);
                PODList.AddRange(data.SRDetails);
                CList.Add(BLL.UserAccount.User.UserType.Company);


                rptQuickSalesReturn.Reset();
                ReportDataSource data1 = new ReportDataSource("SalesReturn", POList);
                ReportDataSource data2 = new ReportDataSource("SalesReturnDetail", GetDetails(data));
                ReportDataSource data3 = new ReportDataSource("CompanyDetail", CList);
                ReportDataSource data4 = new ReportDataSource("Ledger", BLL.Ledger.toList.Where(x => x.Id == data.LedgerId).ToList());
                

                rptQuickSalesReturn.LocalReport.DataSources.Add(data1);
                rptQuickSalesReturn.LocalReport.DataSources.Add(data2);
                rptQuickSalesReturn.LocalReport.DataSources.Add(data3);
                rptQuickSalesReturn.LocalReport.DataSources.Add(data4);
                rptQuickSalesReturn.LocalReport.ReportPath = @"rpt\Transaction\rptSalesReturn.rdlc";

                rptQuickSalesReturn.RefreshReport();

            }
            catch (Exception ex)
            {

            }
        }
        public DataTable GetDetails(BLL.SalesReturn data)
        {
            int NoRecPerPage =12;
            var dataSet = new DataSet();
            DataTable dt = new DataTable();
            dataSet.Tables.Add(dt);

            dt.Columns.Add("ProductName");
            dt.Columns.Add("Quantity");
            dt.Columns.Add("UnitPrice");
            dt.Columns.Add("UOMName");
            dt.Columns.Add("Amount");
            dt.Columns.Add("Id");

            var newRow = dt.NewRow();


            int n = 0;
            foreach (var element in data.SRDetails)
            {
                newRow = dt.NewRow();
                n = n + 1;
                // fill the properties into the cells
                newRow["ProductName"] = element.ProductName;
                newRow["Quantity"] = element.Quantity == 0 ? "" : element.Quantity.ToString();
                newRow["UnitPrice"] = element.UnitPrice == 0 ? "" : element.UnitPrice.ToString();
                newRow["UOMName"] = element.UOMName;
                newRow["Amount"] = element.Amount;
                newRow["Id"] = n.ToString();

                dt.Rows.Add(newRow);
            }



            for (int i = 0; i < NoRecPerPage - data.SRDetails.Count(); i++)
            {
                newRow = dt.NewRow();

                // fill the properties into the cells
                newRow["ProductName"] = "";
                newRow["Quantity"] = "";
                newRow["UnitPrice"] = "";
                newRow["Amount"] = "";
                newRow["Id"] = "";

                dt.Rows.Add(newRow);
            }
            return dt;

        }

    }
}
