using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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

namespace AccountBuddy.PL.frm.Print
{
    /// <summary>
    /// Interaction logic for frmQuickPurchaseRq.xaml
    /// </summary>
    public partial class frmQuickPurchaseRq : MetroWindow
    {
        public frmQuickPurchaseRq()
        {
            InitializeComponent();
      
        rptQuickPO.SetDisplayMode(DisplayMode.PrintLayout);
        }
    public void LoadReport(BLL.PurchaseRequest data)
    {
        try
        {

            List<BLL.PurchaseRequest> POList = new List<BLL.PurchaseRequest>();
            List<BLL.PurchaseRequestDetail> PODList = new List<BLL.PurchaseRequestDetail>();
            List<BLL.CompanyDetail> CList = new List<BLL.CompanyDetail>();
            ObservableCollection<BLL.Ledger> LedgerDetail = new ObservableCollection<BLL.Ledger>();

            POList.Add(data);
            PODList.AddRange(data.PRDetails);
            DataTable dt = GetDetails(data);
            CList.Add(BLL.UserAccount.User.UserType.Company);
            LedgerDetail.Add(BLL.Ledger.toList.Where(x => x.LedgerName == data.LedgerName).FirstOrDefault());

            rptQuickPO.Reset();
            ReportDataSource data1 = new ReportDataSource("PurchaseRequest", POList);
            ReportDataSource data2 = new ReportDataSource("PurchaseRequestDetail", dt);
            ReportDataSource data3 = new ReportDataSource("CompanyDetail", CList);
            ReportDataSource data4 = new ReportDataSource("Ledger", LedgerDetail);


            rptQuickPO.LocalReport.DataSources.Add(data1);
            rptQuickPO.LocalReport.DataSources.Add(data2);
            rptQuickPO.LocalReport.DataSources.Add(data3);
            rptQuickPO.LocalReport.DataSources.Add(data4);
            rptQuickPO.LocalReport.ReportPath = @"rpt\Transaction\rptQuickPurchaseRq.rdlc";

            ReportParameter[] par = new ReportParameter[1];
            par[0] = new ReportParameter("AmtInWords", data.AmountInwords);
            rptQuickPO.LocalReport.SetParameters(par);

                rptQuickPO.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(GetSubReportData);

            rptQuickPO.RefreshReport();

        }
        catch (Exception ex)
        {

        }
    }

        private void GetSubReportData(object sender, SubreportProcessingEventArgs e)
        {
            List<BLL.CompanyDetail> CList = new List<BLL.CompanyDetail>();
            CList.Add(BLL.UserAccount.User.UserType.Company);

            e.DataSources.Add(new ReportDataSource ("CompanyDetail",CList));
        }

        public DataTable GetDetails(BLL.PurchaseRequest data)
    {
        int NoRecPerPage = 23;
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
