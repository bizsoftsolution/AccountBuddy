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
using AccountBuddy.Common;
using MahApps.Metro.Controls;
using Microsoft.Reporting.WinForms;

namespace AccountBuddy.PL.frm.Print
{
    /// <summary>
    /// Interaction logic for frmJobOrderIssue.xaml
    /// </summary>
    public partial class frmJobOrderIssue : MetroWindow
    {

        public frmJobOrderIssue()
        {
            InitializeComponent();
            rptQuickJOIssue.SetDisplayMode(DisplayMode.PrintLayout);
        }
        public void LoadReport(BLL.JobOrderIssue data)
        {
            try
            {

                List<BLL.JobOrderIssue> JOList = new List<BLL.JobOrderIssue>();
                List<BLL.JobOrderIssueDetail> JODList = new List<BLL.JobOrderIssueDetail>();
                List<BLL.CompanyDetail> CList = new List<BLL.CompanyDetail>();
                ObservableCollection<BLL.Ledger> LedgerDetail = new ObservableCollection<BLL.Ledger>();

                JOList.Add(data);
                JODList.AddRange(data.JODetails);
                DataTable dt = GetDetails(data);
                CList.Add(BLL.UserAccount.User.UserType.Company);
                LedgerDetail.Add(BLL.Ledger.toList.Where(x => x.LedgerName == data.JobWorkerName).FirstOrDefault());

                rptQuickJOIssue.Reset();
                ReportDataSource data1 = new ReportDataSource("JobOrderIssue", JOList);
                ReportDataSource data2 = new ReportDataSource("JobOrderIssueDetail", dt);
                ReportDataSource data3 = new ReportDataSource("CompanyDetail", CList);
                ReportDataSource data4 = new ReportDataSource("Ledger", LedgerDetail);


                rptQuickJOIssue.LocalReport.DataSources.Add(data1);
                rptQuickJOIssue.LocalReport.DataSources.Add(data2);
                rptQuickJOIssue.LocalReport.DataSources.Add(data3);
                rptQuickJOIssue.LocalReport.DataSources.Add(data4);
                rptQuickJOIssue.LocalReport.ReportPath = @"rpt\Transaction\rptJobOrderIssue.rdlc";

                ReportParameter[] rp = new ReportParameter[6];
                rp[0] = new ReportParameter("AmtPrefix", AppLib.CurrencyPositiveSymbolPrefix);
                rp[1] = new ReportParameter("ItemAmount",string.Format("{0:N2} {1}", data.ItemAmount, AppLib.CurrencyPositiveSymbolPrefix));
                rp[2] = new ReportParameter("DiscountAmount", string.Format("{0:N2} {1}", data.DiscountAmount, AppLib.CurrencyPositiveSymbolPrefix));
                rp[3] = new ReportParameter("Extra", string.Format("{0:N2} {1}", data.Extras, AppLib.CurrencyPositiveSymbolPrefix));
                rp[4] = new ReportParameter("GST", string.Format("{0:N2} {1}", data.GSTAmount, AppLib.CurrencyPositiveSymbolPrefix));
                rp[5] = new ReportParameter("BillAmount", string.Format("{0:N2} {1}", data.TotalAmount, AppLib.CurrencyPositiveSymbolPrefix));

                rptQuickJOIssue.LocalReport.SetParameters(rp);

                rptQuickJOIssue.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(SetSubDataSource);





                rptQuickJOIssue.RefreshReport();

            }
            catch (Exception ex)
            {

            }
        }
        public void SetSubDataSource(object sender, SubreportProcessingEventArgs e)
        {
            e.DataSources.Add(new ReportDataSource("CompanyDetail", BLL.CompanyDetail.toList.Where(x => x.Id == BLL.UserAccount.User.UserType.Company.Id).ToList())); ;
        }
        public DataTable GetDetails(BLL.JobOrderIssue data)
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
            foreach (var element in data.JODetails)
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
            if (NoRecPerPage < data.JODetails.Count)
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
            for (int i = 0; i < NoRecPerPage - data.JODetails.Count(); i++)
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
