using AccountBuddy.Common;
using Microsoft.AspNet.SignalR.Client;
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

namespace AccountBuddy.PL.frm.Master
{
    /// <summary>
    /// Interaction logic for frmCompanySetting.xaml
    /// </summary>
    public partial class frmCompanySetting : UserControl
    {
        BLL.CompanyDetail data = new BLL.CompanyDetail();

        public frmCompanySetting()
        {
            InitializeComponent();
            this.DataContext = data;

            onClientEvents();
        }

        private void onClientEvents()
        {
            BLL.ABClientHub.FMCGHub.On<BLL.CompanyDetail>("CompanyDetail_Save", (cs) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    cs.Save(true);
                });

            });

            BLL.ABClientHub.FMCGHub.On<BLL.UserAccount>("UserAccount_Save", (ua) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    BLL.UserAccount u = new BLL.UserAccount();
                    ua.toCopy<BLL.UserAccount>(u);
                    BLL.UserAccount.toList.Add(u);
                });

            });
        }


        private void btnSave_Click(object sender, RoutedEventArgs e)
        {            
            if (data.Save() == true) MessageBox.Show("Saved");
        }

        

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            data.Find(BLL.UserAccount.Company.Id);

            var lstUser = BLL.UserAccount.toList;
            dgvWarehouse.ItemsSource = lstUser;            
            gbxLoginUser.Visibility = BLL.UserAccount.AllowFormShow("User Account") ? Visibility.Visible : Visibility.Collapsed;
        }

        private void btnNewUser_Click(object sender, RoutedEventArgs e)
        {
            frmUser f = new frmUser();
            f.ShowDialog();            
        }
        
        private void btnEditUser_Click(object sender, RoutedEventArgs e)
        {
            var u = dgvWarehouse.SelectedItem as BLL.UserAccount;

            frmUser f = new frmUser();
            u.toCopy<BLL.UserAccount>(f.data);
            f.ShowDialog();
        }

        private void btnDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            var u = dgvWarehouse.SelectedItem as BLL.UserAccount;
            if (u != null)
            {
                if(u.UserTypeId==1 && BLL.UserAccount.toList.Where(x=> x.UserTypeId == 1).Count() == 1)
                {
                    MessageBox.Show(string.Format("You can not delete this user. atleast one {0} user required",u.UserTypeName));
                }
                else if (MessageBox.Show("Do you Delete this?", "Delete", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {

                    if (u.Delete() == true) MessageBox.Show("Deleted");
                }
            }
            
        }        
    }
}
