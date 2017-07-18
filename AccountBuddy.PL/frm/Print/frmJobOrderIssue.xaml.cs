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

               


                rptQuickJOIssue.RefreshReport();

            }
            catch (Exception ex)
            {

            }
        }

        public DataTable GetDetails(BLL.JobOrderIssue data)
        {
            int NoRecPerPage = 12;
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
