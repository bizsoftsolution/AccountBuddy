﻿using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using Microsoft.Reporting.WinForms;
using Microsoft.Win32;

namespace AccountBuddy.PL.frm.Transaction
{
    /// <summary>
    /// Interaction logic for frmPurchaseSearch.xaml
    /// </summary>
    public partial class frmPurchaseSearch : MetroWindow
    {
        private int m_currentPageIndex;
        private IList<Stream> m_streams;

        public frmPurchaseSearch()
        {
            InitializeComponent();
            rptViewer.SetDisplayMode(DisplayMode.PrintLayout);
            int yy = BLL.UserAccount.User.UserType.Company.LoginAccYear;

            DateTime? dtFrom = DateTime.Now;
            DateTime? dtTo = DateTime.Now;

            dtpDateFrom.SelectedDate = dtFrom;
            dtpDateTo.SelectedDate = dtTo;


        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            cmbSupplierName.ItemsSource = BLL.Ledger.toList.Where(x => x.AccountGroup.GroupName == BLL.DataKeyValue.SundryCreditors_Key).ToList();
            cmbSupplierName.DisplayMemberPath = "LedgerName";
            cmbSupplierName.SelectedValuePath = "Id";
            dgvReceiptAndPayment.ItemsSource = BLL.Purchase.tolist((int?)cmbSupplierName.SelectedValue, dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value, txtEntryNo.Text);
            LoadReport();
        }

        private void LoadReport()
        {
            try
            {
                List<BLL.Purchase> list = BLL.Purchase.tolist((int?)cmbSupplierName.SelectedValue, dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value, txtEntryNo.Text);
                list = list.Select(x => new BLL.Purchase()
                { LedgerName = x.LedgerName, TotalAmount = x.TotalAmount, PurchaseDate = x.PurchaseDate, RefNo = x.RefNo }).ToList();

                try
                {
                    rptViewer.Reset();
                    ReportDataSource data = new ReportDataSource("Purchase", list);
                    ReportDataSource data1 = new ReportDataSource("CompanyDetail", BLL.CompanyDetail.toList.Where(x => x.Id == BLL.UserAccount.User.UserType.CompanyId).ToList());
                    rptViewer.LocalReport.DataSources.Add(data);
                    rptViewer.LocalReport.DataSources.Add(data1);
                    rptViewer.LocalReport.ReportPath = @"rpt\Transaction\rptPurchaseReport.rdlc";


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

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            dgvReceiptAndPayment.ItemsSource = BLL.Purchase.tolist((int?)cmbSupplierName.SelectedValue, dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value, txtEntryNo.Text);
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

        #endregion

        private void dgvReceiptAndPayment_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var rp = dgvReceiptAndPayment.SelectedItem as BLL.Purchase;
            if (rp != null)
            {

                Transaction.frmPurchase f = new Transaction.frmPurchase();
                App.frmHome.ShowForm(f);
                System.Windows.Forms.Application.DoEvents();
                f.data.RefNo = rp.RefNo;
              
                System.Windows.Forms.Application.DoEvents();
                f.data.Find();
                f.data.SetAmount();
                f.btnPrint.IsEnabled = true;
                this.Close();

            }
        }

        private void btnExcel_Click(object sender, RoutedEventArgs e)
        {
            //Microsoft.Office.Interop.Excel._Application excel = new Microsoft.Office.Interop.Excel.Application();
            //Microsoft.Office.Interop.Excel._Workbook workbook = excel.Workbooks.Add(Type.Missing);
            //Microsoft.Office.Interop.Excel._Worksheet worksheet = null;

            //try
            //{

            //    worksheet = workbook.ActiveSheet;

            //    worksheet.Name = "ExportedFromDatGrid";

            //    int cellRowIndex = 1;
            //    int cellColumnIndex = 1;
            //    DataTable dt = new DataTable();
            //    var r = dgvReceiptAndPayment.ItemsSource;
            //    foreach(var d in r)
            //    {
            //        dt = new DataTable();
            //        dt.Ite
            //    }
                
            //    //Loop through each row and read value from each column. 
            //    for (int i = 0; i < dgvReceiptAndPayment.Items.Count - 1; i++)
            //    {
            //        for (int j = 0; j < dgvReceiptAndPayment.Columns.Count; j++)
            //        {
            //            // Excel index starts from 1,1. As first Row would have the Column headers, adding a condition check. 
            //            if (cellRowIndex == 1)
            //            {
            //                worksheet.Cells[cellRowIndex, cellColumnIndex] = dgvReceiptAndPayment.Columns[j].Header;
            //            }
            //            else
            //            {
            //                worksheet.Cells[cellRowIndex, cellColumnIndex] = dgvReceiptAndPayment.Rows[i].Cells[j].Value.ToString();
            //            }
            //            cellColumnIndex++;
            //        }
            //        cellColumnIndex = 1;
            //        cellRowIndex++;
            //    }

            //    //Getting the location and file name of the excel to save from user. 
            //    SaveFileDialog saveDialog = new SaveFileDialog();
            //    saveDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            //    saveDialog.FilterIndex = 2;

            //    if (saveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //    {
            //        workbook.SaveAs(saveDialog.FileName);
            //        MessageBox.Show("Export Successful");
            //    }
            //}
            //catch (System.Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
            //finally
            //{
            //    excel.Quit();
            //    workbook = null;
            //    excel = null;
            //}


        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadReport();
        }
    }
}