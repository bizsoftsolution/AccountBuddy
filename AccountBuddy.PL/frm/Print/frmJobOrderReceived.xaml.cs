﻿using System;
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
    /// Interaction logic for frmJobOrderReceived.xaml
    /// </summary>
    public partial class frmJobOrderReceived : MetroWindow
    {
        public frmJobOrderReceived()
        {
            InitializeComponent();
            rptQuickJOReceived.SetDisplayMode(DisplayMode.PrintLayout);
        }
        public void LoadReport(BLL.JobOrderReceived data)
        {
            try
            {

                List<BLL.JobOrderReceived> JOList = new List<BLL.JobOrderReceived>();
                List<BLL.JobOrderReceivedDetail> JODList = new List<BLL.JobOrderReceivedDetail>();
                List<BLL.CompanyDetail> CList = new List<BLL.CompanyDetail>();
                ObservableCollection<BLL.Ledger> LedgerDetail = new ObservableCollection<BLL.Ledger>();

                JOList.Add(data);
                JODList.AddRange(data.JRDetails);
                DataTable dt = GetDetails(data);
                CList.Add(BLL.UserAccount.User.UserType.Company);
                LedgerDetail.Add(BLL.Ledger.toList.Where(x => x.LedgerName == data.JobWorkerName).FirstOrDefault());

                rptQuickJOReceived.Reset();
                ReportDataSource data1 = new ReportDataSource("JobOrderReceived", JOList);
                ReportDataSource data2 = new ReportDataSource("JobOrderReceivedDetail", dt);
                ReportDataSource data3 = new ReportDataSource("CompanyDetail", CList);
                ReportDataSource data4 = new ReportDataSource("Ledger", LedgerDetail);


                rptQuickJOReceived.LocalReport.DataSources.Add(data1);
                rptQuickJOReceived.LocalReport.DataSources.Add(data2);
                rptQuickJOReceived.LocalReport.DataSources.Add(data3);
                rptQuickJOReceived.LocalReport.DataSources.Add(data4);
                rptQuickJOReceived.LocalReport.ReportPath = @"rpt\Transaction\rptJobOrderReceived.rdlc";



                ReportParameter[] rp = new ReportParameter[6];
                rp[0] = new ReportParameter("AmtPrefix", AppLib.CurrencyPositiveSymbolPrefix);
                rp[1] = new ReportParameter("ItemAmount", string.Format("{0} {1:N2}", AppLib.CurrencyPositiveSymbolPrefix, data.ItemAmount));
                rp[2] = new ReportParameter("DiscountAmount", string.Format("{0} {1:N2}", AppLib.CurrencyPositiveSymbolPrefix, data.DiscountAmount));
                rp[3] = new ReportParameter("Extra", string.Format("{0} {1:N2}", AppLib.CurrencyPositiveSymbolPrefix, data.Extras));
                rp[4] = new ReportParameter("GST", string.Format("{0} {1:N2}", AppLib.CurrencyPositiveSymbolPrefix, data.GSTAmount));
                rp[5] = new ReportParameter("BillAmount", string.Format("{0} {1:N2}", AppLib.CurrencyPositiveSymbolPrefix, data.TotalAmount));


                rptQuickJOReceived.LocalReport.SetParameters(rp);

                rptQuickJOReceived.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(SetSubDataSource);




                rptQuickJOReceived.RefreshReport();

            }
            catch (Exception ex)
            {

            }
        }
        public void SetSubDataSource(object sender, SubreportProcessingEventArgs e)
        {
            e.DataSources.Add(new ReportDataSource("CompanyDetail", BLL.CompanyDetail.toList.Where(x => x.Id == BLL.UserAccount.User.UserType.Company.Id).ToList())); ;
        }
        public DataTable GetDetails(BLL.JobOrderReceived data)
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
            foreach (var element in data.JRDetails)
            {
                newRow = dt.NewRow();
                n = n + 1;
                newRow["ProductName"] = element.ProductName;
                newRow["Quantity"] = element.Quantity == 0 ? "0.00" : element.Quantity.ToString();
                newRow["UnitPrice"] = element.UnitPrice == 0 ? "0.00" : String.Format("{0:0.00}", element.UnitPrice);
                newRow["UOMName"] = element.UOMName;
                newRow["Amount"] = String.Format("{0:0.00}", element.Amount);
                newRow["Id"] = n.ToString();
                newRow["DiscountAmount"] = element.DiscountAmount == 0 ? "0.00 : String.Format("{ 0:0.00}", element.DiscountAmount);

                dt.Rows.Add(newRow);
            }
            if (NoRecPerPage < data.JRDetails.Count)
            {

                for (int i = 0; i < 34; i++)
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
            for (int i = 0; i < NoRecPerPage - data.JRDetails.Count(); i++)
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
