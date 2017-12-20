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
using AccountBuddy.Common;

namespace AccountBuddy.PL.frm.Print
{
    /// <summary>
    /// Interaction logic for frmQuickSO.xaml
    /// </summary>
    public partial class frmQuickSO : MetroWindow
    {
        public frmQuickSO()
        {
            InitializeComponent();
            rptQuickSalesOrder.SetDisplayMode(DisplayMode.PrintLayout);
        }
        public void LoadReport(BLL.SalesOrder data)
        {
            try
            {

                List<BLL.SalesOrder> POList = new List<BLL.SalesOrder>();
                List<BLL.SalesOrderDetail> PODList = new List<BLL.SalesOrderDetail>();
                List<BLL.CompanyDetail> CList = new List<BLL.CompanyDetail>();
                List<BLL.Ledger> CustomerList = new List<BLL.Ledger>();

                POList.Add(data);
                PODList.AddRange(data.SODetails);
                CList.Add(BLL.UserAccount.User.UserType.Company);
                CustomerList.Add(BLL.Ledger.toList.Where(x => x.Id == data.LedgerId).FirstOrDefault());


                rptQuickSalesOrder.Reset();
                ReportDataSource data1 = new ReportDataSource("SalesOrder", POList);
                ReportDataSource data2 = new ReportDataSource("SalesOrderDetail", GetDetails(data));
                ReportDataSource data3 = new ReportDataSource("CompanyDetail", CList);
                ReportDataSource data4 = new ReportDataSource("Ledger", CustomerList);

                rptQuickSalesOrder.LocalReport.DataSources.Add(data1);
                rptQuickSalesOrder.LocalReport.DataSources.Add(data2);
                rptQuickSalesOrder.LocalReport.DataSources.Add(data3);
                rptQuickSalesOrder.LocalReport.DataSources.Add(data4);
                rptQuickSalesOrder.LocalReport.ReportPath = @"rpt\Transaction\rptQuickSalesOrder.rdlc";

                ReportParameter[] rp = new ReportParameter[6];
                rp[0] = new ReportParameter("AmtPrefix", AppLib.CurrencyPositiveSymbolPrefix);
                rp[1] = new ReportParameter("ItemAmount", string.Format("{0} {1:N2}", AppLib.CurrencyPositiveSymbolPrefix, data.ItemAmount));
                rp[2] = new ReportParameter("DiscountAmount", string.Format("{0} {1:N2}", AppLib.CurrencyPositiveSymbolPrefix, data.DiscountAmount));
                rp[3] = new ReportParameter("Extra", string.Format("{0} {1:N2}", AppLib.CurrencyPositiveSymbolPrefix, data.ExtraAmount));
                rp[4] = new ReportParameter("GST", string.Format("{0} {1:N2}", AppLib.CurrencyPositiveSymbolPrefix, data.GSTAmount));
                rp[5] = new ReportParameter("BillAmount", string.Format("{0} {1:N2}", AppLib.CurrencyPositiveSymbolPrefix, data.TotalAmount));


                rptQuickSalesOrder.LocalReport.SetParameters(rp);


                rptQuickSalesOrder.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(SetSubDataSource);



                rptQuickSalesOrder.RefreshReport();

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
        public DataTable GetDetails(BLL.SalesOrder data)
        {
            int NoRecPerPage =22;
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
            foreach (var element in data.SODetails)
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
            if (NoRecPerPage < data.SODetails.Count)
            {

                for (int i = 0; i < 35; i++)
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
            }


            for (int i = 0; i < NoRecPerPage - data.SODetails.Count(); i++)
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
