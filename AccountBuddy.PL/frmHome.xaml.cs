using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AccountBuddy.Common;

namespace AccountBuddy.PL
{
    /// <summary>
    /// Interaction logic for frmHome.xaml
    /// </summary>
    public partial class frmHome : MetroWindow
    {
        public bool IsForcedClose = false;
        List<NavMenuItem> lstActiveForms = new List<NavMenuItem>();
        public frmHome()
        {
            InitializeComponent();
            ShowWelcome();
            IsForcedClose = false;
            var l1 = BLL.UserAccount.User.UserType.UserTypeDetails.Where(x => x.IsViewForm &&
                                                                              x.UserTypeFormDetail.IsMenu &&
                                                                              x.UserTypeFormDetail.IsActive &&
                                                                              x.UserTypeFormDetail.FormType == "Master")
                                                                  .Select(x => new Common.NavMenuItem()
                                                                  {
                                                                      MenuName = x.UserTypeFormDetail.Description,
                                                                      FormName = x.UserTypeFormDetail.FormName
                                                                  })
                                                                  .ToList();
            lstMaster.ItemsSource = l1;

            var l2 = BLL.UserAccount.User.UserType.UserTypeDetails.Where(x => x.IsViewForm &&
                                                                              x.UserTypeFormDetail.IsMenu &&
                                                                              x.UserTypeFormDetail.IsActive &&
                                                                              x.UserTypeFormDetail.FormType == "Transaction")
                                                                  .Select(x => new Common.NavMenuItem()
                                                                  {
                                                                      MenuName = x.UserTypeFormDetail.Description,
                                                                      FormName = x.UserTypeFormDetail.FormName
                                                                  })
                                                                  .ToList();
            lstTransaction.ItemsSource = l2;

            var l3 = BLL.UserAccount.User.UserType.UserTypeDetails.Where(x => x.IsViewForm &&
                                                                              x.UserTypeFormDetail.IsMenu &&
                                                                              x.UserTypeFormDetail.IsActive &&
                                                                              x.UserTypeFormDetail.FormType == "Report")
                                                                  .Select(x => new Common.NavMenuItem()
                                                                  {
                                                                      MenuName = x.UserTypeFormDetail.Description,
                                                                      FormName = x.UserTypeFormDetail.FormName
                                                                  })
                                                                  .ToList();
            lstReport.ItemsSource = l3;
            Common.AppLib.WriteLog("frmHome_Init");
        }
        private bool Menu_Filter(object obj)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text)) return true;
            try
            {
                var mnu = obj as Common.NavMenuItem;
                return mnu.MenuName.ToLower().Contains(txtSearch.Text.ToLower());
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return false;
        }
        public void ShowWelcome()
        {
            lstActiveForms.Clear();
            ShowForm(Common.Forms.frmWelcome);
        }
        public void ShowBank()
        {
            ccContent.Content = new frm.Master.frmBank();
        }
        public object ShowForm(String Formname)
        {
            var f = lstActiveForms.Where(x => x.FormName == Formname).FirstOrDefault();
            if (f == null)
            {
                f = new NavMenuItem();
                f.FormName = Formname;
                f.Content = Activator.CreateInstance(Type.GetType(Formname));
                lstActiveForms.Add(f);
            }

            ccContent.Content = f.Content;
            Common.AppLib.WriteLog(string.Format("frmHome_ShowForm_{0}",Formname));
            return f.Content;
        }
        public bool CloseForm()
        {
            var n = lstActiveForms.Count();
            var f = lstActiveForms.LastOrDefault();
            if (n == 1)
            {
                if (f.FormName == Common.Forms.frmWelcome)
                {
                    return true;
                }else
                {
                    ShowWelcome();
                    return false;
                }                
            }
            else
            {
                lstActiveForms.RemoveAt(n - 1);
                f = lstActiveForms.LastOrDefault();
                ccContent.Content = f.Content;
                return false;
            }
        }
        

        private void ListBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var dependencyObject = Mouse.Captured as DependencyObject;
                while (dependencyObject != null)
                {
                    if (dependencyObject is ScrollBar) return;
                    dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
                }


                ListBox lb = sender as ListBox;
                Common.NavMenuItem mi = lb.SelectedItem as Common.NavMenuItem;
                lstActiveForms.Clear();
                ShowForm(mi.FormName);
                txtSearch.Text = "";
                

            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            MenuToggleButton.IsChecked = false;
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!IsForcedClose) if (MessageBox.Show("Are you sure to Close?", "Close", MessageBoxButton.YesNo) == MessageBoxResult.Yes) e.Cancel = !CloseForm(); else e.Cancel = true;
            if (!e.Cancel) { App.frmLogin.Show(); App.frmLogin.ClearForm(); Common.AppLib.WriteLog("frmHome_Closed"); }
        }
        
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lstMaster.Items).Filter = Menu_Filter;
            CollectionViewSource.GetDefaultView(lstTransaction.Items).Filter = Menu_Filter;
            CollectionViewSource.GetDefaultView(lstReport.Items).Filter = Menu_Filter;

        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            Common.AppLib.WriteLog("frmHome_btnRefresh_Click_Start");
            BLL.UserAccount.Re_Login();
            Common.AppLib.WriteLog("frmHome_btnRefresh_Click_End");
        }

        private void MetroWindow_Activated(object sender, EventArgs e)
        {
            Common.AppLib.WriteLog("frmHome_Activated");
        }

        private void MetroWindow_Deactivated(object sender, EventArgs e)
        {
            Common.AppLib.WriteLog("frmHome_Deactivated");
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Common.AppLib.WriteLog("frmHome_Loaded");
        }
    }
}
