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

namespace AccountBuddy.PL.frm.Vouchers
{
    /// <summary>
    /// Interaction logic for frmQuickPayment.xaml
    /// </summary>
    public partial class frmQuickPayment : MetroWindow
    {
        public frmQuickPayment()
        {
            InitializeComponent();
            rptQuickPayment.SetDisplayMode(DisplayMode.PrintLayout);
        }
        public void LoadReport(BLL.Payment data, string Payto)
        {
            try
            {

                List<BLL.Payment> PList = new List<BLL.Payment>();
                List<BLL.CompanyDetail> CList = new List<BLL.CompanyDetail>();
                List<BLL.PaymentDetail> PDList = new List<BLL.PaymentDetail>();

                PList.Add(data);
                PDList.Add(data.PDetail);
                CList.Add(BLL.UserAccount.Company);


                rptQuickPayment.Reset();
                ReportDataSource data1 = new ReportDataSource("Payment", PList);
                ReportDataSource data2 = new ReportDataSource("CompanyDetail", CList);
                ReportDataSource data3 = new ReportDataSource("PaymentDetail", PDList);

                rptQuickPayment.LocalReport.DataSources.Add(data1);
                rptQuickPayment.LocalReport.DataSources.Add(data2);
                rptQuickPayment.LocalReport.DataSources.Add(data3);
                rptQuickPayment.LocalReport.ReportPath = @"rpt\Transaction\rptPaymentVoucher.rdlc";

                ReportParameter[] par = new ReportParameter[1];
                par[0] = new ReportParameter("PayTo", Payto.ToUpper());
                rptQuickPayment.LocalReport.SetParameters(par);



                rptQuickPayment.RefreshReport();

            }
            catch (Exception ex)
            {

            }
        }
    }
}

