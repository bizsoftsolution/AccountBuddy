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
using System.Windows.Shapes;
using MahApps.Metro.Controls;

namespace AccountBuddy.PL.frm.Master
{
    /// <summary>
    /// Interaction logic for frmDeleteConfirmation.xaml
    /// </summary>
    public partial class frmDeleteConfirmation : MetroWindow
    {

        public bool RValue { get; private set; }

        public frmDeleteConfirmation()
        {
            InitializeComponent();
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }
        private void txtPassword_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Cut ||
                e.Command == ApplicationCommands.Copy ||
                e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true;
            }
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            var r = BLL.UserAccount.Admin_Authentication(BLL.UserAccount.User.UserType.Company.CompanyName, txtLoginId.Text, txtPassword.Password);
            if (r == true)
            {
                MessageBox.Show("Verified", "", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                RValue = true;
                Close();
            }
            else
            {
                MessageBox.Show("Wrong User", "", MessageBoxButton.OK, MessageBoxImage.Error);
                RValue = false;
                Close();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
            
        }

       
    }
}
