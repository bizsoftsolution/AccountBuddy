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
using MahApps.Metro.Controls;
using Microsoft.Reporting.WinForms;

namespace AccountBuddy.PL.frm.Vouchers
{
    /// <summary>
    /// Interaction logic for frmQuickReceipt.xaml
    /// </summary>
    public partial class frmQuickReceipt : MetroWindow
    {
        public frmQuickReceipt()
        {
            InitializeComponent();
            rptQuickReceipt.SetDisplayMode(DisplayMode.PrintLayout);
        }
        public void LoadReport(BLL.Receipt data, string ReceivedFrom)
        {
            try
            {

                List<BLL.Receipt> RList = new List<BLL.Receipt>();
                List<BLL.CompanyDetail> CList = new List<BLL.CompanyDetail>();
                List<BLL.ReceiptDetail> RDList = new List<BLL.ReceiptDetail>();

                RList.Add(data);
                RDList.Add(data.RDetail);
                CList.Add(BLL.UserAccount.Company);


                rptQuickReceipt.Reset();
                ReportDataSource data1 = new ReportDataSource("Receipt", RList);
                ReportDataSource data2 = new ReportDataSource("CompanyDetail", CList);
                ReportDataSource data3 = new ReportDataSource("ReceiptDetail", RDList);

                rptQuickReceipt.LocalReport.DataSources.Add(data1);
                rptQuickReceipt.LocalReport.DataSources.Add(data2);
                rptQuickReceipt.LocalReport.DataSources.Add(data3);
                rptQuickReceipt.LocalReport.ReportPath = @"rpt\Transaction\rptReceiptVoucher.rdlc";

                ReportParameter[] par = new ReportParameter[1];
                par[0] = new ReportParameter("ReceivedFrom", ReceivedFrom);
                rptQuickReceipt.LocalReport.SetParameters(par);



                rptQuickReceipt.RefreshReport();

            }
            catch (Exception ex)
            {

            }
        }
    }
}
