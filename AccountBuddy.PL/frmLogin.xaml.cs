using MahApps.Metro.Controls;
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
using Microsoft.AspNet.SignalR.Client;

namespace AccountBuddy.PL
{
    /// <summary>
    /// Interaction logic for frmLogin.xaml
    /// </summary>
    public partial class frmLogin : MetroWindow
    {

        public frmLogin()
        {
            InitializeComponent();

            onClientEvents();

        }

        private void onClientEvents()
        {
            BLL.FMCGHubClient.FMCGHub.On<BLL.CompanyDetail>("CompanyDetail_Save", (cs) =>
            {

                this.Dispatcher.Invoke(() =>
                {
                    cs.Save(true);

                });

            });
        }

        #region Button Events

        #region company 
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (cmbCompany.Text == "")
            {
                MessageBox.Show(Message.PL.Login_CompanyName_Validation);
                cmbCompany.Focus();
            }
            else if (txtUserId.Text == "")
            {
                MessageBox.Show(Message.PL.Login_UserName_validation);
                txtUserId.Focus();
            }
            else if (txtPassword.Password == "")
            {
                MessageBox.Show(Message.PL.Login_Password_Validation);
                txtPassword.Focus();
            }
            else
            {
                string RValue = BLL.UserAccount.Login("", cmbCompany.Text, txtUserId.Text, txtPassword.Password);

                if (RValue == "")
                {
                    App.frmHome = new frmHome();
                    App.frmHome.Title = String.Format("{0} - {1}", BLL.UserAccount.User.UserName, BLL.UserAccount.User.UserType.Company.CompanyName);
                    this.Hide();
                    cmbCompany.Text = "";
                    txtUserId.Text = "";
                    txtPassword.Password = "";
                    App.frmHome.ShowDialog();
                    this.Show();
                    cmbCompany.Focus();
                }
                else
                {
                    MessageBox.Show(RValue);
                    ClearForm();
                }
            }
            
        }

        private void btnSignup_Click(object sender, RoutedEventArgs e)
        {

            frmCompanySignup f = new frmCompanySignup();
            f.data.CompanyType = "Company";
            f.ShowDialog();

        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();

        }

        #endregion

        #region  Warehouse
        private void btnLoginWarehouse_Click(object sender, RoutedEventArgs e)
        {
            if (cmbCompanyWarehousePrimay.Text == "")
            {
                MessageBox.Show(Message.PL.Login_CompanyName_Validation);
                cmbCompanyWarehousePrimay.Focus();
            }

            else if (cmbCompanyWarehouse.Text == "")
            {
                MessageBox.Show(Message.PL.Login_Warehouse_Validation);
                cmbCompanyWarehouse.Focus();
            }
            else if (txtUserIdWarehouse.Text == "")
            {
                MessageBox.Show(Message.PL.Login_UserName_validation);
                txtUserIdWarehouse.Focus();
            }
            else if (txtPasswordWarehouse.Password == "")
            {
                MessageBox.Show(Message.PL.Login_Password_Validation);
                txtPasswordWarehouse.Focus();
            }
            else
            {
                string RValue = BLL.UserAccount.Login("", cmbCompanyWarehouse.Text, txtUserIdWarehouse.Text, txtPasswordWarehouse.Password);

                if ( RValue == "")
                {
                    App.frmHome = new frmHome();
                    App.frmHome.Title = String.Format("{0} - {1}", BLL.UserAccount.User.UserName, BLL.UserAccount.User.UserType.Company.CompanyName);
                    this.Hide();
                    cmbCompanyWarehousePrimay.Text = "";
                    cmbCompanyWarehouse.Text = "";
                    txtUserIdWarehouse.Text = "";
                    txtPasswordWarehouse.Password = "";
                    App.frmHome.ShowDialog();
                    this.Show();
                    cmbCompanyWarehousePrimay.Focus();
                }
                else
                {
                    MessageBox.Show(RValue);
                    ClearForm();
                }
            }
            
        }

        private void btnClearWarehouse_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        #endregion

        #region Dealer
        private void btnLoginDealer_Click(object sender, RoutedEventArgs e)
        {
            if (cmbCompanyDealerPrimay.Text == "")
            {
                MessageBox.Show(Message.PL.Login_CompanyName_Validation);
                cmbCompanyDealerPrimay.Focus();
            }

            else if (cmbCompanyDealer.Text == "")
            {
                MessageBox.Show(Message.PL.Login_Dealer_Validation);
                cmbCompanyDealer.Focus();
            }
            else if (txtUserIdDealer.Text == "")
            {
                MessageBox.Show(Message.PL.Login_UserName_validation);
                txtUserIdDealer.Focus();
            }
            else if (txtPasswordDealer.Password == "")
            {
                MessageBox.Show(Message.PL.Login_Password_Validation);
                txtPasswordDealer.Focus();
            }
            else
            {
                string RValue = BLL.UserAccount.Login("", cmbCompanyDealer.Text, txtUserIdDealer.Text, txtPasswordDealer.Password);
                if (RValue == "" )
                {
                    App.frmHome = new frmHome();
                    App.frmHome.Title = String.Format("{0} - {1}", BLL.UserAccount.User.UserName, BLL.UserAccount.User.UserType.Company.CompanyName);
                    this.Hide();
                    cmbCompanyDealerPrimay.Text = "";
                    cmbCompanyDealerWarehouse.Text = "";
                    cmbCompanyDealer.Text = "";

                    txtUserIdDealer.Text = "";
                    txtPasswordDealer.Password = "";
                    App.frmHome.ShowDialog();
                    this.Show();
                    cmbCompanyDealerPrimay.Focus();
                }
                else
                {
                    MessageBox.Show(RValue);
                    ClearForm();
                }
            }
            
        }

        private void btnClearDealer_Click(object sender, RoutedEventArgs e)
        {

            ClearForm();
        }

        #endregion
        #endregion


        #region Events
        private void cmbCompanyWarehouse_GotFocus(object sender, RoutedEventArgs e)
        {
            var cm = cmbCompanyWarehousePrimay.SelectedItem as BLL.CompanyDetail;
            List<BLL.CompanyDetail> lstCom = new List<BLL.CompanyDetail>();
            if (cm != null)
            {
                lstCom = BLL.CompanyDetail.toList.Where(x => x.CompanyType == "Warehouse" && x.UnderCompanyId == cm.Id).ToList();
            }
            cmbCompanyWarehouse.ItemsSource = lstCom;
            cmbCompanyWarehouse.SelectedValuePath = "Id";
            cmbCompanyWarehouse.DisplayMemberPath = "CompanyName";
        }

        private void cmbCompanyDealerWarehouse_GotFocus(object sender, RoutedEventArgs e)
        {
            var cm = cmbCompanyDealerPrimay.SelectedItem as BLL.CompanyDetail;
            List<BLL.CompanyDetail> lstCom = new List<BLL.CompanyDetail>();
            if (cm != null)
            {
                lstCom = BLL.CompanyDetail.toList.Where(x => x.CompanyType == "Warehouse" && x.UnderCompanyId == cm.Id).ToList();
            }
            cmbCompanyDealerWarehouse.ItemsSource = lstCom;
            cmbCompanyDealerWarehouse.SelectedValuePath = "Id";
            cmbCompanyDealerWarehouse.DisplayMemberPath = "CompanyName";
        }

        private void cmbCompanyDealer_GotFocus(object sender, RoutedEventArgs e)
        {
            var cm = cmbCompanyDealerWarehouse.SelectedItem as BLL.CompanyDetail;
            List<BLL.CompanyDetail> lstCom = new List<BLL.CompanyDetail>();
            if (cm != null)
            {
                lstCom = BLL.CompanyDetail.toList.Where(x => x.CompanyType == "Dealer" && x.UnderCompanyId == cm.Id).ToList();
            }
            cmbCompanyDealer.ItemsSource = lstCom;
            cmbCompanyDealer.SelectedValuePath = "Id";
            cmbCompanyDealer.DisplayMemberPath = "CompanyName";
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

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Are you sure to Exit?", "Exit", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                e.Cancel = true;
            }
        }

        #endregion

        #region Methods
        private void ClearForm()
        {
            cmbCompany.Text = "";
            txtUserId.Text = "";
            txtPassword.Password = "";

            cmbCompanyDealerPrimay.Text = "";
            cmbCompanyDealerWarehouse.Text = "";
            cmbCompanyDealer.Text = "";
            txtUserIdDealer.Text = "";
            txtPasswordDealer.Password = "";

            cmbCompanyWarehousePrimay.Text = "";
            cmbCompanyWarehouse.Text = "";
            txtUserIdWarehouse.Text = "";
            txtPasswordWarehouse.Password = "";
        }

        #endregion

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var l1 = BLL.CompanyDetail.toList.Where(x=>x.CompanyType=="Company").ToList();
            cmbCompany.ItemsSource = l1;
            cmbCompany.SelectedValuePath = "Id";
            cmbCompany.DisplayMemberPath = "CompanyName";

            cmbCompanyWarehousePrimay.ItemsSource = l1;
            cmbCompanyWarehousePrimay.SelectedValuePath = "Id";
            cmbCompanyWarehousePrimay.DisplayMemberPath = "CompanyName";

            cmbCompanyDealerPrimay.ItemsSource = l1;
            cmbCompanyDealerPrimay.SelectedValuePath = "Id";
            cmbCompanyDealerPrimay.DisplayMemberPath = "CompanyName";


        }

    }
}
