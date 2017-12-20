using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing.Printing;
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

namespace AccountBuddy.PL.frm.Report
{
    /// <summary>
    /// Interaction logic for frmPaymentReceipt.xaml
    /// </summary>
    public partial class frmPaymentReceipt : UserControl
    {
        private int m_currentPageIndex;
        private IList<Stream> m_streams;

        public frmPaymentReceipt()
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
            cmbAccountName.ItemsSource = BLL.Ledger.toList.ToList();
            cmbAccountName.DisplayMemberPath = "AccountName";
            cmbAccountName.SelectedValuePath = "Id";


        }


        private void LoadReport()
        {
            try
            {
                List<BLL.ReceiptAndPayment> list = dgvReceiptAndPayment.ItemsSource as List<BLL.ReceiptAndPayment>;
                    list = list.Select(x => new BLL.ReceiptAndPayment()
                { AccountName = x.AccountName, PayTo=x.PayTo, TType = x.TType, Particular =x.Particular, Amount = x.Amount, EDate = x.EDate, EntryNo = x.EntryNo, EType = x.EType, Ledger = x.Ledger, RefNo = x.RefNo }).ToList();

                try
                {
                    rptViewer.Reset();
                    ReportDataSource data = new ReportDataSource("ReceiptAndPayment", list);
                    ReportDataSource data1 = new ReportDataSource("CompanyDetail", BLL.CompanyDetail.toList.Where(x => x.Id == BLL.UserAccount.User.UserType.CompanyId).ToList());
                    rptViewer.LocalReport.DataSources.Add(data);
                    rptViewer.LocalReport.DataSources.Add(data1);
                    rptViewer.LocalReport.ReportPath = @"rpt\Report\rptPaymentReceipt.rdlc";

                    ReportParameter[] par = new ReportParameter[3];
                    par[0] = new ReportParameter("DateFrom", dtpDateFrom.SelectedDate.Value.ToString());
                    par[1] = new ReportParameter("DateTo", dtpDateTo.SelectedDate.Value.ToString());
                    par[2] = new ReportParameter("AmtPrefix", Common.AppLib.CurrencyPositiveSymbolPrefix.ToString());
                    rptViewer.LocalReport.SetParameters(par);

                    rptViewer.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(SetSubDataSource);

                    rptViewer.RefreshReport();

                }
                catch (Exception ex)
                {
                    Common.AppLib.WriteLog(ex);
                }
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex);
            }

        }
        public void SetSubDataSource(object sender, SubreportProcessingEventArgs e)
        {
            e.DataSources.Add(new ReportDataSource("CompanyDetail", BLL.CompanyDetail.toList.Where(x => x.Id == BLL.UserAccount.User.UserType.Company.Id).ToList())); ;
        }
     

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            dgvReceiptAndPayment.ItemsSource = BLL.ReceiptAndPayment.ToList((int?)cmbAccountName.SelectedValue, dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value, txtEntryNo.Text, cmbstatus.Text);
            LoadReport();
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

                //MessageBox.Show("Completed Exporting");
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex);
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
                <PageWidth>8.2in</PageWidth>
                <PageHeight>11.6in</PageHeight>
                <MarginTop>1cm</MarginTop>
                <MarginLeft>1cm</MarginLeft>
                <MarginRight>1cm</MarginRight>
                <MarginBottom>1cm</MarginBottom>
            </DeviceInfo>";
                Warning[] warnings;
                m_streams = new List<Stream>();
                report.Render("Image", deviceInfo, CreateStream,
                  out warnings);
                foreach (Stream stream in m_streams)
                    stream.Position = 0;
            }
            catch (Exception ex)
            { Common.AppLib.WriteLog(ex); }
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
                Common.AppLib.WriteLog(ex);
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
            if (cmbAccountName.Text != "")
            {
                if (dgvReceiptAndPayment.Items.Count != 0)
                {

                    frmPaymentReceiptPrint f = new frmPaymentReceiptPrint();
                    f.LoadReport(dgvReceiptAndPayment.ItemsSource as List<BLL.ReceiptAndPayment>, dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value);
                    f.ShowDialog();
                }
                else
                {
                     MessageBox.Show("No Records To Print");
                }

            }
            else
            {
              
                MessageBox.Show("Enter AccountName");
            }

        }

        #endregion

        private void dgvReceiptAndPayment_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var rp = dgvReceiptAndPayment.SelectedItem as BLL.ReceiptAndPayment;
            if (rp != null)
            {
                if (rp.EType == 'P')
                {
                    Transaction.frmPayment f = App.frmHome.ShowForm(Common.Forms.frmPayment) as Transaction.frmPayment;

                    System.Windows.Forms.Application.DoEvents();
                    f.data.EntryNo = rp.EntryNo;
                    System.Windows.Forms.Application.DoEvents();
                    f.data.Find();
                }
                else if (rp.EType == 'R')
                {
                    Transaction.frmReceipt f = App.frmHome.ShowForm(Common.Forms.frmReceipt) as Transaction.frmReceipt;

                    System.Windows.Forms.Application.DoEvents();
                    f.data.EntryNo = rp.EntryNo;
                    System.Windows.Forms.Application.DoEvents();
                    f.data.Find();
                }
                
            }
        }
    }
}
