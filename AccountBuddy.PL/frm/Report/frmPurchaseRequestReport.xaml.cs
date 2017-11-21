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
            LoadReport();
        }

        private bool RequestOnHold_Filter(object obj)
        {
            bool RValue = false;

            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                string strSearch = cbxCase.IsChecked == true ? txtSearch.Text : txtSearch.Text.ToLower();
                string strValue = "";

                var d1 = obj as BLL.PurchaseRequestReport;

                if (d1.IsReject || d1.IsApproval) return false;

                foreach (var p in d1.GetType().GetProperties())
                {
                    if (p.Name.ToLower().Contains("id") ||
                        p.GetValue(d1) == null ||
                        p.PropertyType.Namespace != "System"
                            ) continue;
                    strValue = p.GetValue(d1).ToString();
                    if (cbxCase.IsChecked == false)
                    {
                        strValue = strValue.ToLower();
                    }
                    if (rptStartWith.IsChecked == true && strValue.StartsWith(strSearch))
                    {
                        RValue = true;
                        break;
                    }
                    else if (rptContain.IsChecked == true && strValue.Contains(strSearch))
                    {
                        RValue = true;
                        break;
                    }
                    else if (rptEndWith.IsChecked == true && strValue.EndsWith(strSearch))
                    {
                        RValue = true;
                        break;
                    }
                }                
            }
            else
            {
                RValue = true;
            }
            return RValue;
        }
        private bool RequestOnReject_Filter(object obj)
        {
            bool RValue = false;

            if (!string.IsNullOrEmpty(txtSearchReject.Text))
            {
                string strSearch = cbxCaseReject.IsChecked == true ? txtSearchReject.Text : txtSearchReject.Text.ToLower();
                string strValue = "";

                var d1 = obj as BLL.PurchaseRequestReport;

                if (!d1.IsReject) return false;

                foreach (var p in d1.GetType().GetProperties())
                {
                    if (p.Name.ToLower().Contains("id") ||
                        p.GetValue(d1) == null ||
                        p.PropertyType.Namespace != "System"
                            ) continue;
                    strValue = p.GetValue(d1).ToString();
                    if (cbxCaseReject.IsChecked == false)
                    {
                        strValue = strValue.ToLower();
                    }
                    if (rptStartWithReject.IsChecked == true && strValue.StartsWith(strSearch))
                    {
                        RValue = true;
                        break;
                    }
                    else if (rptContainReject.IsChecked == true && strValue.Contains(strSearch))
                    {
                        RValue = true;
                        break;
                    }
                    else if (rptEndWithReject.IsChecked == true && strValue.EndsWith(strSearch))
                    {
                        RValue = true;
                        break;
                    }
                }
            }
            else
            {
                RValue = true;
            }
            return RValue;
        }  
        private bool RequestOnApproval_Filter(object obj)
        {
            bool RValue = false;

            if (!string.IsNullOrEmpty(txtSearchApproved.Text))
            {
                string strSearch = cbxCaseApproved.IsChecked == true ? txtSearchApproved.Text : txtSearchApproved.Text.ToLower();
                string strValue = "";

                var d1 = obj as BLL.PurchaseRequestReport;

                if (!d1.IsApproval) return false;

                foreach (var p in d1.GetType().GetProperties())
                {
                    if (p.Name.ToLower().Contains("id") ||
                        p.GetValue(d1) == null ||
                        p.PropertyType.Namespace != "System"
                            ) continue;
                    strValue = p.GetValue(d1).ToString();
                    if (cbxCaseApproved.IsChecked == false)
                    {
                        strValue = strValue.ToLower();
                    }
                    if (rptStartWithApproved.IsChecked == true && strValue.StartsWith(strSearch))
                    {
                        RValue = true;
                        break;
                    }
                    else if (rptContainApproved.IsChecked == true && strValue.Contains(strSearch))
                    {
                        RValue = true;
                        break;
                    }
                    else if (rptEndWithApproved.IsChecked == true && strValue.EndsWith(strSearch))
                    {
                        RValue = true;
                        break;
                    }
                }
            }
            else
            {
                RValue = true;
            }
            return RValue;
        }
        private bool Budget_Filter(object obj)
        {
            bool RValue = false;

            if (!string.IsNullOrEmpty(txtSearchBudget.Text))
            {
                string strSearch = cbxCaseBudget.IsChecked == true ? txtSearchBudget.Text : txtSearchBudget.Text.ToLower();
                string strValue = "";

                var d1 = obj as BLL.PurchaseRequestReport;

                foreach (var p in d1.GetType().GetProperties())
                {
                    if (p.Name.ToLower().Contains("id") ||
                        p.GetValue(d1) == null ||
                        p.PropertyType.Namespace != "System"
                            ) continue;
                    strValue = p.GetValue(d1).ToString();
                    if (cbxCaseBudget.IsChecked == false)
                    {
                        strValue = strValue.ToLower();
                    }
                    if (rptStartWithBudget.IsChecked == true && strValue.StartsWith(strSearch))
                    {
                        RValue = true;
                        break;
                    }
                    else if (rptContainBudget.IsChecked == true && strValue.Contains(strSearch))
                    {
                        RValue = true;
                        break;
                    }
                    else if (rptEndWithBudget.IsChecked == true && strValue.EndsWith(strSearch))
                    {
                        RValue = true;
                        break;
                    }
                }
            }
            else
            {
                RValue = true;
            }
            return RValue;
        }

        private void Grid_Refresh()
        {
            try
            {
                CollectionViewSource.GetDefaultView(dgvDetailsOnHold.ItemsSource).Refresh();
                CollectionViewSource.GetDefaultView(dgvDetailsOnReject.ItemsSource).Refresh();
                CollectionViewSource.GetDefaultView(dgvDetailsOnApproved.ItemsSource).Refresh();
                CollectionViewSource.GetDefaultView(dgvDetailsOnBudget.ItemsSource).Refresh();
            }
            catch (Exception ex) { };

        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Grid_Refresh();
        }

        private void cbxCase_Checked(object sender, RoutedEventArgs e)
        {
            Grid_Refresh();
        }
        private void cbxCase_Unchecked(object sender, RoutedEventArgs e)
        {
            Grid_Refresh();
        }

        private void rptStartWith_Checked(object sender, RoutedEventArgs e)
        {
            Grid_Refresh();
        }


        private void rptContain_Checked(object sender, RoutedEventArgs e)
        {
            Grid_Refresh();
        }

        private void rptEndWith_Checked(object sender, RoutedEventArgs e)
        {
            Grid_Refresh();
        }

        private void rptStartWith_Unchecked(object sender, RoutedEventArgs e)
        {
            Grid_Refresh();
        }

        private void rptContain_Unchecked(object sender, RoutedEventArgs e)
        {
            Grid_Refresh();
        }

        private void rptEndWith_Unchecked(object sender, RoutedEventArgs e)
        {
            Grid_Refresh();
        }
        private void LoadReport()
        {
            try
            {
                List<BLL.PurchaseRequestReport> list = BLL.PurchaseRequestReport.ToList(dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value);

                dgvDetailsOnHold.ItemsSource = list.Where(x=> x.IsReject==false && x.IsApproval==false).ToList();
                dgvDetailsOnReject.ItemsSource = list.Where(x => x.IsReject == true).ToList();
                dgvDetailsOnApproved.ItemsSource = list.Where(x => x.IsApproval== true).ToList();
                dgvDetailsOnBudget.ItemsSource = BLL.PurchaseRequestBudgetReport.ToList();

                CollectionViewSource.GetDefaultView(dgvDetailsOnHold.ItemsSource).Filter = RequestOnHold_Filter;
                CollectionViewSource.GetDefaultView(dgvDetailsOnHold.ItemsSource).Filter = RequestOnReject_Filter;
                CollectionViewSource.GetDefaultView(dgvDetailsOnHold.ItemsSource).Filter = RequestOnApproval_Filter;
                CollectionViewSource.GetDefaultView(dgvDetailsOnHold.ItemsSource).Filter = Budget_Filter;
                Grid_Refresh();
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
            if (MessageBox.Show("Are you Reject this Request?","Purchase Request", MessageBoxButton.YesNo, MessageBoxImage.Question)==MessageBoxResult.Yes)
            {
                var btn = sender as Button;
                var d = btn.Tag as BLL.PurchaseRequestReport;
                if (BLL.PurchaseRequest.Reject(d.PurchaseRequestStatusDetailsId))
                {
                    MessageBox.Show("Requestion Reject Successfull!");
                    LoadReport();
                }
            }            
        }

        private void btnApproval_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you Approval this Request?", "Purchase Request", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var btn = sender as Button;
                var d = btn.Tag as BLL.PurchaseRequestReport;
                if (BLL.PurchaseRequest.Approval(d.PurchaseRequestStatusDetailsId))
                {
                    MessageBox.Show("Requestion Approval Successfull!");
                    LoadReport();
                }
            }
        }

        private void btnDetail_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var d = btn.Tag as BLL.PurchaseRequestReport;
            Transaction.frmPurchaseRequest frm = new Transaction.frmPurchaseRequest();            
            App.frmHome.ShowForm(frm);
            System.Windows.Forms.Application.DoEvents();
            frm.data.SearchText = d.PurchaseRequestRefNo;
            System.Windows.Forms.Application.DoEvents();
            frm.data.Find();
            System.Windows.Forms.Application.DoEvents();
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
