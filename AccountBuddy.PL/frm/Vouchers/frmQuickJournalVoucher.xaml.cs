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
    /// Interaction logic for frmQuickJournalVoucher.xaml
    /// </summary>
    public partial class frmQuickJournalVoucher : MetroWindow
    {
        public frmQuickJournalVoucher()
        {
            InitializeComponent();
            rptQuickJournal.SetDisplayMode(DisplayMode.PrintLayout);
        }
        public void LoadReport(BLL.Journal data)
        {
            try
            {

                List<BLL.Journal> JList = new List<BLL.Journal>();
                List<BLL.CompanyDetail> CList = new List<BLL.CompanyDetail>();
                List<BLL.JournalDetail> JDList = new List<BLL.JournalDetail>();

                JList.Add(data);
                JDList.Add(data.JDetail);
                CList.Add(BLL.UserAccount.User.UserType.Company);


                rptQuickJournal.Reset();
                ReportDataSource data1 = new ReportDataSource("Journal", JList);
                ReportDataSource data2 = new ReportDataSource("CompanyDetail", CList);
                ReportDataSource data3 = new ReportDataSource("JDetails", data.JDetails);

                rptQuickJournal.LocalReport.DataSources.Add(data1);
                rptQuickJournal.LocalReport.DataSources.Add(data2);
                rptQuickJournal.LocalReport.DataSources.Add(data3);
                rptQuickJournal.LocalReport.ReportPath = @"rpt\Transaction\rptJournalReceipt.rdlc";
                rptQuickJournal.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(SetSubDataSource);

                rptQuickJournal.RefreshReport();

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
    }
}
