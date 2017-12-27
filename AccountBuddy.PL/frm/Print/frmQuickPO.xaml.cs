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
using Microsoft.Reporting.WinForms;
using MahApps.Metro.Controls;
using System.Collections.ObjectModel;
using System.Data;
using AccountBuddy.Common;

namespace AccountBuddy.PL.frm.Print
{
    /// <summary>
    /// Interaction logic for frmQuickPO.xaml
    /// </summary>
    public partial class frmQuickPO : MetroWindow
    {

        public frmQuickPO()
        {
            InitializeComponent();
            rptQuickPO.SetDisplayMode(DisplayMode.PrintLayout);
        }
        public void LoadReport(BLL.PurchaseOrder data)
        {
            try
            {

                List<BLL.PurchaseOrder> POList = new List<BLL.PurchaseOrder>();
                List<BLL.PurchaseOrderDetail> PODList = new List<BLL.PurchaseOrderDetail>();
                List<BLL.CompanyDetail> CList = new List<BLL.CompanyDetail>();
                ObservableCollection<BLL.Ledger> LedgerDetail = new ObservableCollection<BLL.Ledger>();

                POList.Add(data);
                PODList.AddRange(data.PODetails);
                DataTable dt = GetDetails(data);
                CList.Add(BLL.UserAccount.User.UserType.Company);
                LedgerDetail.Add(BLL.Ledger.toList.Where(x => x.LedgerName == data.LedgerName).FirstOrDefault());

                rptQuickPO.Reset();
                ReportDataSource data1 = new ReportDataSource("PurchaseOrder", POList);
                ReportDataSource data2 = new ReportDataSource("PurchaseOrderDetail", dt);
                ReportDataSource data3 = new ReportDataSource("CompanyDetail", CList);
                ReportDataSource data4 = new ReportDataSource("Ledger", LedgerDetail);


                rptQuickPO.LocalReport.DataSources.Add(data1);
                rptQuickPO.LocalReport.DataSources.Add(data2);
                rptQuickPO.LocalReport.DataSources.Add(data3);
                rptQuickPO.LocalReport.DataSources.Add(data4);
                rptQuickPO.LocalReport.ReportPath = @"rpt\Transaction\rptQuickPurchaseOrder.rdlc";


                ReportParameter[] rp = new ReportParameter[6];
                rp[0] = new ReportParameter("AmtPrefix", AppLib.CurrencyPositiveSymbolPrefix);
                rp[1] = new ReportParameter("ItemAmount", string.Format("{0} {1:N2}", AppLib.CurrencyPositiveSymbolPrefix, data.ItemAmount));
                rp[2] = new ReportParameter("DiscountAmount", string.Format("{0} {1:N2}", AppLib.CurrencyPositiveSymbolPrefix, data.DiscountAmount));
                rp[3] = new ReportParameter("Extra", string.Format("{0} {1:N2}", AppLib.CurrencyPositiveSymbolPrefix, data.Extras));
                rp[4] = new ReportParameter("GST", string.Format("{0} {1:N2}",AppLib.CurrencyPositiveSymbolPrefix, data.GSTAmount));
                rp[5] = new ReportParameter("BillAmount", string.Format("{0} {1:N2}",AppLib.CurrencyPositiveSymbolPrefix, data.TotalAmount));

                rptQuickPO.LocalReport.SetParameters(rp);
                rptQuickPO.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(SetSubDataSource);




                rptQuickPO.RefreshReport();

            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex);
            }
        }
        public void SetSubDataSource(object sender, SubreportProcessingEventArgs e)
        {
            e.DataSources.Add(new ReportDataSource("CompanyDetail", BLL.CompanyDetail.ToList.Where(x => x.Id == BLL.UserAccount.User.UserType.Company.Id).ToList())); ;
        }
        public DataTable GetDetails(BLL.PurchaseOrder data)
        {
            int NoRecPerPage = 21;
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
            foreach (var element in data.PODetails)
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


            if (NoRecPerPage < data.PODetails.Count)
            {

                for (int i = 0; i < 30; i++)
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
            for (int i = 0; i < NoRecPerPage - data.PODetails.Count(); i++)
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

