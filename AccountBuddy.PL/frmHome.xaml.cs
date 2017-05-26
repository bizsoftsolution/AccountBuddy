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
        }
        public void ShowWelcome()
        {
            ccContent.Content = new frmWelcome();
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
                if (!BLL.UserAccount.AllowFormShow(mi.FormName))
                {

                    MessageBox.Show(string.Format(Message.PL.DenyFormShow, mi.MenuName));
                }
                else
                {
                    ccContent.Content = mi.Content;
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
    }
}
