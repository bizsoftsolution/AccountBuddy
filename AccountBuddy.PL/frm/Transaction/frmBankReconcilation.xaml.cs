using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using Microsoft.Win32;
using System.Drawing.Printing;
using System.Drawing.Imaging;


namespace AccountBuddy.PL.frm.Transaction
{
    /// <summary>
    /// Interaction logic for frmBankReconcilation.xaml
    /// </summary>
    public partial class frmBankReconcilation : UserControl
    {
        private int m_currentPageIndex;
        private IList<Stream> m_streams;

        public frmBankReconcilation()
        {
            InitializeComponent();
            rptViewer.SetDisplayMode(DisplayMode.PrintLayout);

            int yy = BLL.UserAccount.User.UserType.Company.LoginAccYear;

            DateTime? dtFrom = new DateTime(yy, 4, 1);
            DateTime? dtTo = new DateTime(yy + 1, 3, 31);

            dtpDateFrom.SelectedDate = dtFrom;
            dtpDateTo.SelectedDate = dtTo;

            cmbAccountName.ItemsSource = BLL.Ledger.toList.Where(x=> x.AccountGroup.GroupName=="Bank Accounts").ToList();
            cmbAccountName.DisplayMemberPath = "AccountName";
            cmbAccountName.SelectedValuePath = "Id";
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            
        }


        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (cmbAccountName.SelectedValue == null)
            {
                MessageBox.Show("Enter Bank Account..");
                cmbAccountName.Focus();
            }
            else
            {
                if (cmbAccountName.SelectedValue != null) dgvBankReconciliation.ItemsSource = BLL.BankReconcilation.ToList((int)cmbAccountName.SelectedValue, dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value);
                //LoadReport();
            }

        }

        private void dgvBankReconciliation_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var gl = dgvBankReconciliation.SelectedItem as BLL.BankReconcilation;
            if (gl != null)
            {
                if (gl.EType == 'P')
                {
                    Transaction.frmPayment f = new Transaction.frmPayment();
                    App.frmHome.ShowForm(f);
                    System.Windows.Forms.Application.DoEvents();
                    f.data.SearchText = gl.EntryNo;
                    System.Windows.Forms.Application.DoEvents();
                    f.data.Find();
                }
                else if (gl.EType == 'R')
                {
                    Transaction.frmReceipt f = new Transaction.frmReceipt();
                    App.frmHome.ShowForm(f);
                    System.Windows.Forms.Application.DoEvents();
                    f.data.SearchText = gl.EntryNo;
                    System.Windows.Forms.Application.DoEvents();
                    f.data.Find();
                }
                else if (gl.EType == 'J')
                {
                    Transaction.frmJournal f = new Transaction.frmJournal();
                    App.frmHome.ShowForm(f);
                    System.Windows.Forms.Application.DoEvents();
                    f.data.SearchText = gl.EntryNo;
                    System.Windows.Forms.Application.DoEvents();
                    f.data.Find();
                }
            }
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
                <PageWidth>8.5in</PageWidth>
                <PageHeight>11in</PageHeight>
                <MarginTop>0in</MarginTop>
                <MarginLeft>0in</MarginLeft>
                <MarginRight>0in</MarginRight>
                <MarginBottom>0in</MarginBottom>
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
            //frmGeneralLedgerPrint f = new frmGeneralLedgerPrint();
            //f.LoadReport((int)cmbAccountName.SelectedValue, dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value);
            //f.ShowDialog();
        }

        #endregion

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var l1 = dgvBankReconciliation.ItemsSource;

            foreach(var d in l1)
            {
                var b = d as BLL.BankReconcilation;
                if (b != null)
                {
                    if (b.EType == 'P')
                    {
                        BLL.Payment p = new BLL.Payment();
                        p.SearchText = b.EntryNo;
                        p.Find();
                        p.Status = b.IsCompleted ? "Completed" : "Proccess";
                        p.Save();
                    }
                    else if (b.EType == 'R')
                    {
                        BLL.Receipt R = new BLL.Receipt();
                        R.SearchText = b.EntryNo;
                        R.Find();
                        R.Status = b.IsCompleted ? "Completed" : "Proccess";
                        R.Save();
                    }
                }
            }
            MessageBox.Show("Saved");
            App.frmHome.ShowWelcome();
        }
    }
}
