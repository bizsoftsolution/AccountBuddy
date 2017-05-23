using Microsoft.Reporting.WinForms;
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

namespace AccountBuddy.PL.frm.Transaction
{
    /// <summary>
    /// Interaction logic for frmLedgerOpening.xaml
    /// </summary>
    public partial class frmLedgerOpening : UserControl
    {
        List<BLL.Ledger> lstLedgerOld = new List<BLL.Ledger>();
        public frmLedgerOpening()
        {
            InitializeComponent();
            RptLedger.SetDisplayMode(DisplayMode.PrintLayout);
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabControl tc = sender as TabControl;

            if (tc.SelectedIndex == 1)
            {
                LoadReport();
            }        
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dgvLedger.ItemsSource = BLL.Ledger.toList;
            lstLedgerOld = BLL.Ledger.toList.Select(x=> new BLL.Ledger() {Id= x.Id,OPDr=x.OPDr,OPCr=x.OPCr }).ToList();
            BLL.Ledger data = new BLL.Ledger();
            CollectionViewSource.GetDefaultView(dgvLedger.ItemsSource).Filter = Ledger_Filter;
            CollectionViewSource.GetDefaultView(dgvLedger.ItemsSource).SortDescriptions.Add(new System.ComponentModel.SortDescription(nameof(data.AccountName), System.ComponentModel.ListSortDirection.Ascending));
            FindDiff();
        }
        private bool Ledger_Filter(object obj)
        {
            bool RValue = false;
            var d = obj as BLL.Ledger;

            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                string strSearch = cbxCase.IsChecked == true ? txtSearch.Text : txtSearch.Text.ToLower();
                string strValue = "";

                foreach (var p in d.GetType().GetProperties())
                {
                    if (p.Name.ToLower().Contains("id") || p.GetValue(d) == null) continue;
                    strValue = p.GetValue(d).ToString();
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

        private void Grid_Refresh()
        {
            try
            {
                CollectionViewSource.GetDefaultView(dgvLedger.ItemsSource).Refresh();
                FindDiff();
            }
            catch (Exception ex) { };

        }

        private void LoadReport()
        {
            try
            {
                RptLedger.Reset();
                ReportDataSource data = new ReportDataSource("Ledger", BLL.Ledger.toList.Where(x => Ledger_Filter(x)).ToList());
                ReportDataSource data1 = new ReportDataSource("CompanyDetail", BLL.CompanyDetail.toList.Where(x => x.Id == BLL.UserAccount.Company.Id).ToList());
                RptLedger.LocalReport.DataSources.Add(data);
                RptLedger.LocalReport.DataSources.Add(data1);
                RptLedger.LocalReport.ReportPath = @"rpt\Transaction\rptLedgerOpening.rdlc";

                RptLedger.RefreshReport();

            }
            catch (Exception ex)
            {

            }


        }
        private void FindDiff()
        {
            var l1 = BLL.Ledger.toList.Where(x => Ledger_Filter(x)).ToList();
            decimal drAmt = l1.Sum(x => x.OPDr ?? 0);
            decimal crAmt = l1.Sum(x => x.OPCr ?? 0);

            lblMsg.Text = string.Format("Total Debit Balance : {0:N2}, Total Credit Balance : {1:N2}, Difference : {2:N2}", drAmt, crAmt, Math.Abs(drAmt - crAmt));
            lblMsg.Foreground = drAmt==crAmt? new SolidColorBrush(Color.FromRgb( 0,0,255)):new SolidColorBrush(Color.FromRgb(255, 0, 0));
        }
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Grid_Refresh();
        }

        private void cbxCase_Checked(object sender, RoutedEventArgs e)
        {
            Grid_Refresh();
        }

        private void rptStartWith_Checked(object sender, RoutedEventArgs e)
        {
            Grid_Refresh();
        }

        private void cbxCase_Unchecked(object sender, RoutedEventArgs e)
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

        private void dgvLedger_CurrentCellChanged(object sender, EventArgs e)
        {
            FindDiff();
        }

        private void dgvLedger_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            FindDiff();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            foreach(var l1 in BLL.Ledger.toList)
            {
                var l2 = lstLedgerOld.Where(x => x.Id == l1.Id).FirstOrDefault();
                if(l1.OPDr != l2.OPDr || l1.OPCr != l2.OPCr)
                {
                    l1.Save();
                }
            }
            MessageBox.Show(Message.PL.Saved_Alert);
            App.frmHome.ShowWelcome();
        }

        private void dgvLedger_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            FindDiff();
        }
    }
}
