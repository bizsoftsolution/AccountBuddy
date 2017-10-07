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

namespace AccountBuddy.PL
{
    /// <summary>
    /// Interaction logic for frmHome.xaml
    /// </summary>
    public partial class frmHome : MetroWindow
    {
        public bool IsForcedClose = false;
        public frmHome()
        {
            InitializeComponent();
            ShowWelcome();
            onClientEvents();
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
        }
        private bool Menu_Filter(object obj)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text)) return true;
            try
            {
                var mnu = obj as Common.NavMenuItem;
                return mnu.MenuName.ToLower().Contains(txtSearch.Text.ToLower());
            }
            catch (Exception ex) { }
            return false;
        }
        public void ShowWelcome()
        {
            ccContent.Content = new frmWelcome();
        }
        public void ShowBank()
        {
            ccContent.Content = new frm.Master.frmBank();
        }
        public void ShowForm(object o)
        {
            ccContent.Content = o;
        }

        private void onClientEvents()
        {
            
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
                if (mi.Content == null)
                {
                    object obj = Activator.CreateInstance(Type.GetType(mi.FormName));
                    mi.Content = obj;
                }
                if (mi.Content != null) ccContent.Content = mi.Content;
                
                else
                {
                    ccContent.Content = mi.Content;
                    txtSearch.Text = "";
                }
            }
            catch (Exception ex) { }
            MenuToggleButton.IsChecked = false;
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!IsForcedClose && MessageBox.Show("Are you sure to Exit?", "Exit", MessageBoxButton.YesNo) != MessageBoxResult.Yes) e.Cancel = true;
        }

        private void Menu_CleanUpVirtualizedItem(object sender, CleanUpVirtualizedItemEventArgs e)
        {

        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lstMaster.Items).Filter = Menu_Filter;
            CollectionViewSource.GetDefaultView(lstTransaction.Items).Filter = Menu_Filter;
            CollectionViewSource.GetDefaultView(lstReport.Items).Filter = Menu_Filter;

        }
    }
}
