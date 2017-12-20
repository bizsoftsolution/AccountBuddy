﻿using Microsoft.Reporting.WinForms;
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
        decimal OPBal = 0, CLBal = 0, EDAmt = 0, BalAmt = 0, DifAmt = 0;

        public frmBankReconcilation()
        {
            InitializeComponent();
            rptViewer.SetDisplayMode(DisplayMode.PrintLayout);

         

            dtpDateFrom.SelectedDate = DateTime.Now;
            dtpDateTo.SelectedDate = DateTime.Now;

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            cmbAccountName.ItemsSource = BLL.Ledger.toList.Where(x => x.AccountGroup.GroupName == "Bank Accounts").ToList();
            cmbAccountName.DisplayMemberPath = "AccountName";
            cmbAccountName.SelectedValuePath = "Id";
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
                if (cmbAccountName.SelectedValue != null)
                {

                    int LedgerId = (int)cmbAccountName.SelectedValue;
                    var l1 = BLL.BankReconcilation.ToList(LedgerId, dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value);
                    dgvBankReconciliation.ItemsSource = l1;
                    OPBal = 0;
                    CLBal = 0;

                    try
                    {
                        OPBal = BLL.TrialBalance.GetLedgerBalance(LedgerId, dtpDateFrom.SelectedDate.Value);
                        CLBal = BLL.TrialBalance.GetLedgerBalance(LedgerId, dtpDateTo.SelectedDate.Value);

                    }
                    catch (Exception ex) { Common.AppLib.WriteLog(ex); }

                }


            }

        }
        void FindBalance()
        {
            EDAmt = string.IsNullOrEmpty(txtEndingBalance.Text) ? 0 : Convert.ToDecimal(txtEndingBalance.Text);
            BalAmt = 0;
            DifAmt = 0;
            try
            {
                var l1 = dgvBankReconciliation.ItemsSource as List<BLL.BankReconcilation>;
                BalAmt = OPBal + Math.Abs(l1.Where(x => x.IsCompleted != true).Sum(x => x.DrAmt) - l1.Where(x => x.IsCompleted != true).Sum(x => x.CrAmt));
                DifAmt = Math.Abs(EDAmt - BalAmt);

            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            lblStatus.Text = string.Format(" Opening Balance : {0:0.00}, Closing Balance : {1:0.00}, Cleared Balance : {2:0.00}, Diffrence : {3:0.00}", OPBal, CLBal, BalAmt, DifAmt);
        }
        private void dgvBankReconciliation_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var gl = dgvBankReconciliation.SelectedItem as BLL.BankReconcilation;
            if (gl != null)
            {
                if (gl.EType == 'P')
                {
                    Transaction.frmPayment f = App.frmHome.ShowForm(Common.Forms.frmPayment) as frmPayment;
                    System.Windows.Forms.Application.DoEvents();
                    f.data.SearchText = gl.EntryNo;
                    f.data.EntryNo = gl.EntryNo;
                    System.Windows.Forms.Application.DoEvents();
                    f.data.Find();
                }
                else if (gl.EType == 'R')
                {
                    Transaction.frmReceipt f = App.frmHome.ShowForm(Common.Forms.frmReceipt) as frmReceipt;
                    System.Windows.Forms.Application.DoEvents();
                    f.data.SearchText = gl.EntryNo;
                    f.data.EntryNo = gl.EntryNo;
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
            Common.AppLib.WriteLog("Bank Receomciliation Print");
            try
            {
                var l1 = dgvBankReconciliation.ItemsSource as List<BLL.BankReconcilation>;
                var l2 = l1.Where(x => x.EType == 'P' && !x.IsCompleted).ToList();
                var l3 = l1.Where(x => x.EType == 'R' && !x.IsCompleted).ToList();
                l2 = l2.Select(x => new BLL.BankReconcilation { PayeeName = x.PayeeName, Amount = x.Amount, EDate = x.EDate, EntryNo = x.EntryNo, RefNo = x.RefNo, VoucherNo = x.VoucherNo }).ToList();
                l3 = l3.Select(x => new BLL.BankReconcilation { PayeeName = x.PayeeName, Amount = x.Amount, EDate = x.EDate, EntryNo = x.EntryNo, RefNo = x.RefNo, VoucherNo = x.VoucherNo }).ToList();

                var lName = "";
                if (cmbAccountName.SelectedValue != null)
                {
                    lName = BLL.Ledger.toList.Where(x => x.Id == (int)cmbAccountName.SelectedValue).FirstOrDefault().LedgerName;
                }
                frmBankReconciliationPrint f = new frmBankReconciliationPrint();
                f.LoadReport(lName, dtpDateTo.SelectedDate.Value, l3, l2, Convert.ToDecimal(txtEndingBalance.Text), Convert.ToDecimal(CLBal.ToString()));
                f.ShowDialog();
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
        }

        #endregion

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

            Save();
        }

        private void Save()
        {
            Common.AppLib.WriteLog("Bank reconciliation Save =>Begins");
            var l1 = dgvBankReconciliation.ItemsSource;
            if (l1 != null)
            {
                foreach (var d in l1)
                {
                    var b = d as BLL.BankReconcilation;
                    if (b != null)
                    {
                        if (b.EType == 'P')
                        {
                            BLL.Payment p = new BLL.Payment();
                            p.SearchText = b.EntryNo;
                            p.EntryNo = b.EntryNo;
                            p.Find();
                            p.Status = b.IsCompleted ? "Completed" : "Proccess";
                            p.Save();
                        }
                        else if (b.EType == 'R')
                        {
                            BLL.Receipt R = new BLL.Receipt();
                            R.SearchText = b.EntryNo;
                            R.EntryNo = b.EntryNo;
                            R.Find();

                            R.Status = b.IsCompleted ? "Completed" : "Proccess";
                            R.Save();
                        }
                    }
                }
                Common.AppLib.WriteLog("Bank reconciliation Saved Successfully");
                MessageBox.Show(Message.PL.Saved_Alert);
                App.frmHome.ShowWelcome();
            }
        }

        private void ckbStatus_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                var b = ((CheckBox)sender).Tag as BLL.BankReconcilation;
                //if (d != null) d.IsCompleted = true;

                // var b = d as BLL.BankReconcilation;
                if (b != null)
                {
                    if (b.EType == 'P')
                    {
                        BLL.Payment p = new BLL.Payment();
                        p.SearchText = b.EntryNo;
                        p.EntryNo = b.EntryNo;
                        p.Find();
                        p.Status = b.IsCompleted ? "Completed" : "Proccess";
                        p.Save();
                    }
                    else if (b.EType == 'R')
                    {
                        BLL.Receipt R = new BLL.Receipt();
                        R.SearchText = b.EntryNo;
                        R.EntryNo = b.EntryNo;
                        R.Find();

                        R.Status = b.IsCompleted ? "Completed" : "Proccess";
                        R.Save();
                    }
                }

            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            FindBalance();
        }

        private void ckbStatus_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                var b = ((CheckBox)sender).Tag as BLL.BankReconcilation;
                //if (d != null) d.IsCompleted = false;
                if (b != null)
                {
                    if (b.EType == 'P')
                    {
                        BLL.Payment p = new BLL.Payment();
                        p.SearchText = b.EntryNo;
                        p.EntryNo = b.EntryNo;
                        p.Find();
                        p.Status = b.IsCompleted ? "Completed" : "Proccess";
                        p.Save();
                    }
                    else if (b.EType == 'R')
                    {
                        BLL.Receipt R = new BLL.Receipt();
                        R.SearchText = b.EntryNo;
                        R.EntryNo = b.EntryNo;
                        R.Find();

                        R.Status = b.IsCompleted ? "Completed" : "Proccess";
                        R.Save();
                    }
                }
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            FindBalance();
        }
    }
}