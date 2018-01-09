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
using System.Windows.Navigation;
using System.Windows.Shapes;
using AccountBuddy.BLL;
using Microsoft.Reporting.WinForms;
using System.IO;
using Microsoft.Win32;
using System.Drawing.Printing;
using System.Drawing.Imaging;

namespace AccountBuddy.PL.frm.Report
{
    /// <summary>
    /// Interaction logic for frmSalesReportCustomer.xaml
    /// </summary>
    public partial class frmSalesReportCustomer : UserControl
    {

        private int m_currentPageIndex;
        private IList<Stream> m_streams;

        public frmSalesReportCustomer()
        {
            InitializeComponent();

            dtpDateFrom.SelectedDate = DateTime.Today;
            dtpDateTo.SelectedDate = DateTime.Today;
        }

        private void cmbCustomer_Loaded(object sender, RoutedEventArgs e)
        {
            cmbCustomer.ItemsSource = BLL.Ledger.toList.Where(x => x.AccountGroup.GroupName == BLL.DataKeyValue.SundryDebtors_Key).ToList();
            cmbCustomer.DisplayMemberPath = "LedgerName";
            cmbCustomer.SelectedValuePath = "Id";
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            int? s = null;
            if (cmbCustomer.SelectedValue != null) s = (int)cmbCustomer.SelectedValue;

            dgvDetails.ItemsSource = BLL.Sale.tolist(s, dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value, txtEntryNo.Text);


            LoadReport(dgvDetails.ItemsSource as List<BLL.Sale>, dtpDateFrom.SelectedDate, dtpDateTo.SelectedDate);

        }

        private void LoadReport(List<Sale> list, DateTime? selectedDate1, DateTime? selectedDate2)
        {
            try
            {
                rptViewer.Reset();
                ReportDataSource data = new ReportDataSource("Sale", list);
                ReportDataSource data1 = new ReportDataSource("CompanyDetail", BLL.CompanyDetail.toList.Where(x => x.Id == BLL.UserAccount.User.UserType.CompanyId).ToList());
                rptViewer.LocalReport.DataSources.Add(data);
                rptViewer.LocalReport.DataSources.Add(data1);
                rptViewer.LocalReport.ReportPath = @"rpt\Report\rptSalesReportNew.rdlc";

                ReportParameter[] par = new ReportParameter[2];
                par[0] = new ReportParameter("DateFrom", dtpDateFrom.SelectedDate.ToString());
                par[1] = new ReportParameter("DateTo", dtpDateTo.SelectedDate.ToString());
                rptViewer.LocalReport.SetParameters(par);

                rptViewer.RefreshReport();


            }
            catch (Exception ex)
            {

            }

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void btnPrintPreview_Click(object sender, RoutedEventArgs e)
        {
            if (dgvDetails.Items.Count != 0)
            {
                frmSalesReportPrint f = new frmSalesReportPrint();
                f.LoadReport(dgvDetails.ItemsSource as List<BLL.Sale>, dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value);
                f.ShowDialog();
            }
            else
            {
                MessageBox.Show("Enter AccountName");
            }
        }

        private void dgvDetails_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var gl = dgvDetails.SelectedItem as BLL.Sale;

                Transaction.frmSale f = new Transaction.frmSale();
                App.frmHome.ShowForm(f);
                System.Windows.Forms.Application.DoEvents();
                f.data.SearchText = gl.RefNo;
                System.Windows.Forms.Application.DoEvents();
                f.data.Find();
                f.btnPrint.IsEnabled = true;
            }
            catch (Exception ex)
            { }

        }

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



    }
}
