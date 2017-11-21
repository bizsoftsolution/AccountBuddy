using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Reporting.WinForms;
using Microsoft.Win32;
using System.Drawing.Printing;
using System.Drawing.Imaging;

namespace AccountBuddy.PL.frm.Report
{
    /// <summary>
    /// Interaction logic for frmPurchaseRequestReport.xaml
    /// </summary>
    public partial class frmPurchaseRequestReport : UserControl
    {
        private int m_currentPageIndex;
        private IList<Stream> m_streams;

        public frmPurchaseRequestReport()
        {
            InitializeComponent();
            rptViewer.SetDisplayMode(DisplayMode.PrintLayout);

            int yy = BLL.UserAccount.User.UserType.Company.LoginAccYear;

            DateTime? dtFrom = new DateTime(yy, 4, 1);
            DateTime? dtTo = new DateTime(yy + 1, 3, 31);

            dtpDateFrom.SelectedDate = dtFrom;
            dtpDateTo.SelectedDate = dtTo;
        }


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dgvDetails.ItemsSource = BLL.POPending.ToList_PR(dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value);
            LoadReport();
        }


        private void LoadReport()
        {
            try
            {
                List<BLL.POPending> list = BLL.POPending.ToList_PR(dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value);
                list = list.Select(x => new BLL.POPending()
                { AccountName = x.Ledger.AccountName, Amount = x.Amount, EntryNo = x.EntryNo, Ledger = x.Ledger, PODate = x.PODate, Status = x.Status }).ToList();

                try
                {
                    rptViewer.Reset();
                    ReportDataSource data = new ReportDataSource("POPending", list);
                    ReportDataSource data1 = new ReportDataSource("CompanyDetail", BLL.CompanyDetail.toList.Where(x => x.Id == BLL.UserAccount.User.UserType.CompanyId).ToList());
                    rptViewer.LocalReport.DataSources.Add(data);
                    rptViewer.LocalReport.DataSources.Add(data1);
                    rptViewer.LocalReport.ReportPath = @"rpt\Report\rptPurchaseRequest.rdlc";

                    ReportParameter[] par = new ReportParameter[2];
                    par[0] = new ReportParameter("DateFrom", dtpDateFrom.SelectedDate.Value.ToString());
                    par[1] = new ReportParameter("DateTo", dtpDateTo.SelectedDate.Value.ToString());
                    rptViewer.LocalReport.SetParameters(par);

                    rptViewer.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(GetSubReportData);

                    rptViewer.RefreshReport();

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
            List<BLL.CompanyDetail> CList = new List<BLL.CompanyDetail>();
            CList.Add(BLL.UserAccount.User.UserType.Company);

            e.DataSources.Add(new ReportDataSource("CompanyDetail", CList));
        }
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            dgvDetails.ItemsSource = BLL.POPending.ToList_PR(dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value).ToList();
            LoadReport();
        }

        private void dgvDetails_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var po = dgvDetails.SelectedItem as BLL.POPending;
                Transaction.frmPurchaseOrder f = new Transaction.frmPurchaseOrder();
                App.frmHome.ShowForm(f);
                System.Windows.Forms.Application.DoEvents();
                f.data.SearchText = po.EntryNo;
                System.Windows.Forms.Application.DoEvents();
                f.data.Find();
                f.btnMakepurchase.IsEnabled = f.data.Status == "Pending" ? true : false; if (f.data.Id != 0)
                    if (f.data.RefCode != null)
                    {
                        f.btnMakepurchase.IsEnabled = false;
                        f.btnSave.IsEnabled = false;
                        f.btnDelete.IsEnabled = true;

                    }
                if (f.data.RefCode != null)
                {
                    f.btnSave.IsEnabled = false;
                    f.btnDelete.IsEnabled = false;
                }
            }
            catch (Exception ex) { }
        }
        #region Button Events
        private Stream CreateStream(string name,
  string fileNameExtension, Encoding encoding,
  string mimeType, bool willSeek)
        {
            Stream stream = new MemoryStream();
            m_streams.Add(stream);
            return stream;
        }
        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string extension;

                byte[] bytes = rptViewer.LocalReport.Render(
                   "PDF", null, out mimeType, out encoding,
                    out extension,
                   out streamids, out warnings);

                SaveFileDialog SaveFileDialog1 = new SaveFileDialog();

                SaveFileDialog1.ShowDialog();
                string file = string.Format(@"{0}.pdf", SaveFileDialog1.FileName);
                FileStream fs = new FileStream(file,
                   FileMode.Create);
                fs.Write(bytes, 0, bytes.Length);
                fs.Close();

                // MessageBox.Show("Completed Exporting");
            }
            catch (Exception ex)
            {
            }

        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            Export(rptViewer.LocalReport);
            Print();
        }

        private void Export(LocalReport report)
        {
            try
            {
                string deviceInfo =
             @"<DeviceInfo>
                <OutputFormat>EMF</OutputFormat>
                <PageWidth>11.6in</PageWidth>
                <PageHeight>8.2</PageHeight>
                <MarginTop>0.7in</MarginTop>
                <MarginLeft>0.7in</MarginLeft>
                <MarginRight>0.7in</MarginRight>
                <MarginBottom>0.7in</MarginBottom>
            </DeviceInfo>";
                Warning[] warnings;
                m_streams = new List<Stream>();
                report.Render("Image", deviceInfo, CreateStream,
                  out warnings);
                foreach (Stream stream in m_streams)
                    stream.Position = 0;
            }
            catch (Exception ex)
            { }
        }

        private void Print()
        {
            try
            {
                if (m_streams == null || m_streams.Count == 0)
                    throw new Exception("Error: no stream to print.");
                PrintDocument printDoc = new PrintDocument();
                if (!printDoc.PrinterSettings.IsValid)
                {
                    throw new Exception("Error: cannot find the default printer.");
                }
                else
                {
                    printDoc.PrintPage += new PrintPageEventHandler(PrintPage);
                    m_currentPageIndex = 0;
                    printDoc.DefaultPageSettings.Landscape = true;
                    printDoc.Print();
                }
            }
            catch (Exception ex)
            {

            }

        }
        private void PrintPage(object sender, PrintPageEventArgs ev)
        {
            Metafile pageImage = new
           Metafile(m_streams[m_currentPageIndex]);

            // Adjust rectangular area with printer margins.
            System.Drawing.Rectangle adjustedRect = new System.Drawing.Rectangle(
            ev.PageBounds.Left - (int)ev.PageSettings.HardMarginX,
            ev.PageBounds.Top - (int)ev.PageSettings.HardMarginY,
            ev.PageBounds.Width,
            ev.PageBounds.Height);

            // Draw a white background for the report
            ev.Graphics.FillRectangle(System.Drawing.Brushes.White, adjustedRect);

            // Draw the report content
            ev.Graphics.DrawImage(pageImage, adjustedRect);

            // Prepare for the next page. Make sure we haven't hit the end.
            m_currentPageIndex++;
            ev.HasMorePages = (m_currentPageIndex < m_streams.Count);
        }

        private void btnPrintPreview_Click(object sender, RoutedEventArgs e)
        {
            frmPRReportPrint f = new frmPRReportPrint();
            f.LoadReport(dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value);
            f.ShowDialog();
        }

        #endregion

        private void btnReject_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnApproval_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
