﻿using System;
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
    /// Interaction logic for frmPRReportPrint.xaml
    /// </summary>
    public partial class frmPRReportPrint : MetroWindow
    {
        public frmPRReportPrint()
        {
            InitializeComponent();
            InitializeComponent();
            RptViewer.SetDisplayMode(DisplayMode.PrintLayout);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {


        }
        public void LoadReport(DateTime dtFrom, DateTime dtTo)
        {
            try
            {
                List<BLL.POPending> list = BLL.POPending.ToList(dtFrom, dtTo);
                list = list.Select(x => new BLL.POPending()
                { AccountName = x.Ledger.AccountName, Amount = x.Amount, EntryNo = x.EntryNo, Ledger = x.Ledger, PODate = x.PODate, Status = x.Status }).ToList();

                try
                {
                    RptViewer.Reset();
                    ReportDataSource data = new ReportDataSource("POPending", list);
                    ReportDataSource data1 = new ReportDataSource("CompanyDetail", BLL.CompanyDetail.toList.Where(x => x.Id == BLL.UserAccount.User.UserType.CompanyId).ToList());
                    RptViewer.LocalReport.DataSources.Add(data);
                    RptViewer.LocalReport.DataSources.Add(data1);
                    RptViewer.LocalReport.ReportPath = @"rpt\Report\rptPurchaseRequest.rdlc";

                    ReportParameter[] par = new ReportParameter[2];
                    par[0] = new ReportParameter("DateFrom", dtFrom.ToString());
                    par[1] = new ReportParameter("DateTo", dtTo.ToString());
                    RptViewer.LocalReport.SetParameters(par);

                    RptViewer.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(GetSubReportData);

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
        private void GetSubReportData(object sender, SubreportProcessingEventArgs e)
        {
            e.DataSources.Add(new ReportDataSource("CompanyDetail", BLL.CompanyDetail.toList.Where(x => x.Id == BLL.UserAccount.User.UserType.CompanyId).ToList()));
        }

    }
}
