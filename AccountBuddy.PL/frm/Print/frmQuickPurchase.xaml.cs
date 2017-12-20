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
    /// Interaction logic for frmQuickPurchase.xaml
    /// </summary>
    public partial class frmQuickPurchase : MetroWindow
    {
        public frmQuickPurchase()
        {
            InitializeComponent();
            rptQuickPurchase.SetDisplayMode(DisplayMode.PrintLayout);
        }
        public void LoadReport(BLL.Purchase data)
        {
            try
            {

                List<BLL.Purchase> POList = new List<BLL.Purchase>();
                List<BLL.PurchaseDetail> PODList = new List<BLL.PurchaseDetail>();
                List<BLL.CompanyDetail> CList = new List<BLL.CompanyDetail>();
                List<BLL.Ledger> SList = new List<BLL.Ledger>();
                List<BLL.Sale> slist = new List<BLL.Sale>();
                


                POList.Add(data);
                PODList.AddRange(data.PDetails);
                DataTable dt = GetDetails(data);
                CList.Add(BLL.UserAccount.User.UserType.Company);
                
                rptQuickPurchase.Reset();
                ReportDataSource data1 = new ReportDataSource("Purchase", POList);
                ReportDataSource data2 = new ReportDataSource("PurchaseDetail", dt);
                ReportDataSource data3 = new ReportDataSource("CompanyDetail", CList);
                ReportDataSource data4 = new ReportDataSource("Ledger", BLL.Ledger.toList.Where(x=>x.Id==data.LedgerId).ToList());
                ReportDataSource data5 = new ReportDataSource("Sale", slist);


                rptQuickPurchase.LocalReport.DataSources.Add(data1);
                rptQuickPurchase.LocalReport.DataSources.Add(data2);
                rptQuickPurchase.LocalReport.DataSources.Add(data3);
                rptQuickPurchase.LocalReport.DataSources.Add(data4);
                rptQuickPurchase.LocalReport.DataSources.Add(data5);
                rptQuickPurchase.LocalReport.ReportPath = @"rpt\Transaction\rptPurchase.rdlc";

                ReportParameter[] rp = new ReportParameter[6];
                rp[0] = new ReportParameter("AmtPrefix", AppLib.CurrencyPositiveSymbolPrefix);
                rp[1] = new ReportParameter("ItemAmount", string.Format("{0} {1:N2}", AppLib.CurrencyPositiveSymbolPrefix, data.ItemAmount));
                rp[2] = new ReportParameter("DiscountAmount", string.Format("{0} {1:N2}", AppLib.CurrencyPositiveSymbolPrefix, data.DiscountAmount));
                rp[3] = new ReportParameter("Extra", string.Format("{0} {1:N2}", AppLib.CurrencyPositiveSymbolPrefix, data.ExtraAmount));
                rp[4] = new ReportParameter("GST", string.Format("{0} {1:N2}", AppLib.CurrencyPositiveSymbolPrefix, data.GSTAmount));
                rp[5] = new ReportParameter("BillAmount", string.Format("{0} {1:N2}", AppLib.CurrencyPositiveSymbolPrefix, data.TotalAmount));


                rptQuickPurchase.LocalReport.SetParameters(rp);

                rptQuickPurchase.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(SetSubDataSource);


                rptQuickPurchase.RefreshReport();

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
        public DataTable GetDetails(BLL.Purchase data)
        {
            int NoRecPerPage = 22;
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
            foreach (var element in data.PDetails)
            {
                newRow = dt.NewRow();
                n = n + 1;
                newRow["ProductName"] = element.ProductName;
                newRow["Quantity"] = element.Quantity == 0 ? "0.00" : element.Quantity.ToString();
                newRow["UnitPrice"] = element.UnitPrice == 0 ? "0.00" : String.Format("{0:0.00}", element.UnitPrice);
                newRow["UOMName"] = element.UOMName;
                newRow["Amount"] = String.Format("{0:0.00}", element.Amount);
                newRow["Id"] = n.ToString();
                newRow["DiscountAmount"] = element.DiscountAmount == 0 ? "0.00" : String.Format("{0:0.00}", element.DiscountAmount);

                dt.Rows.Add(newRow);
            }
            if (NoRecPerPage < data.PDetails.Count)
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
            for (int i = 0; i < NoRecPerPage - data.PDetails.Count(); i++)
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
