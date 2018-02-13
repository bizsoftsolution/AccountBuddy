﻿using MahApps.Metro.Controls;
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
using AccountBuddy.BLL;
using System.IO;

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
            Common.AppLib.WriteLog("frmLogin_Init");
        }

     

        #region Button Events

        #region company 
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            Common.AppLib.WriteLog("frmLogin_btnLogin_Click_Start");
            var tv = trvCompany.SelectedItem as TreeViewItem;
            if (tv == null)
            {
                MessageBox.Show("Select Company!");
            }
            else if (tv.Tag.ToString() == "")
            {
                MessageBox.Show(Message.PL.Login_CompanyName_Validation);
            }
            else if (txtLoginId.Text == "")
            {
                MessageBox.Show(Message.PL.Login_UserName_validation);
                txtLoginId.Focus();
            }
            else if (txtPassword.Password == "")
            {
                MessageBox.Show(Message.PL.Login_Password_Validation);
                txtPassword.Focus();
            }
            else
            {
                string RValue = BLL.UserAccount.Login("", tv.Header.ToString(), txtLoginId.Text, txtPassword.Password);

                if (RValue == "")
                {
                    Common.AppLib.WriteLog("Login Succeed");
                    App.frmHome = new frmHome();
                    App.frmHome.Title = String.Format("{0} - {1}", BLL.UserAccount.User.UserName, BLL.UserAccount.User.UserType.Company.CompanyName);
                    App.frmLogin.Hide();
                    App.frmHome.Show();
                    var s = BLL.CompanyDetail.ToList.Where(x => x.Id == BLL.UserAccount.User.UserType.CompanyId).FirstOrDefault();
                    byte[] ic= s.CFiles.Where(x => x.AttchmentCode == DataKeyValue.Logo_Key).FirstOrDefault().Image;
                    App.frmHome.Icon = Common.AppLib.ViewImage(ic);
                    frmWelcome frm = new frmWelcome();
                    byte[] v = s.CFiles.Where(x => x.AttchmentCode == DataKeyValue.BackGround_Key).FirstOrDefault().Image;
                    if (v != null)
                    {
                        frm.imgBackground.ImageSource = Common.AppLib.ViewImage(v);
                        frm.img.Source = Common.AppLib.ViewImage(v);
                        frm.img.Tag = v;
                    }

                }

                else
                {
                    Common.AppLib.WriteLog("Login Failed");
                    MessageBox.Show(RValue);
                    ClearForm();
                }
            }
            Common.AppLib.WriteLog("frmLogin_btnLogin_Click_End");
        }

        private void btnSignup_Click(object sender, RoutedEventArgs e)
        {
            Common.AppLib.WriteLog("frmLogin_btnSignup_Click_Start");
            frmCompanySignup f = new frmCompanySignup();
            f.data.Clear();
            f.data.CompanyType = "Company";
            f.ShowDialog();
            ClearForm();
            Common.AppLib.WriteLog("frmLogin_btnSignup_Click_End");
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {

            Common.AppLib.WriteLog("frmLogin_btnClear_Click_Start");
            ClearForm();
            Common.AppLib.WriteLog("frmLogin_btnClear_Click_End");
        }

        #endregion


        #endregion

        #region Events
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

            if (!e.Cancel) System.Windows.Application.Current.Shutdown();
        }

        #endregion

        #region Methods
        public void ClearForm()
        {
            txtLoginId.Text = "";
            txtPassword.Password = "";
            var lstCompany = BLL.CompanyDetail.ToList.Where(x => x.IsActive == true && x.CompanyType == "Company").ToList();

            trvCompany.Items.Clear();
            if (lstCompany.Count() == 0)
            {
                trvCompany.Visibility = Visibility.Hidden;
            }
            else
            {
                trvCompany.Visibility = Visibility.Visible;
                foreach (var cm in lstCompany)
                {
                    var lstWarehouse = BLL.CompanyDetail.ToList.Where(x => x.IsActive == true && x.UnderCompanyId == cm.Id && x.CompanyType == "Warehouse").ToList();
                    var lstDealer = BLL.CompanyDetail.ToList.Where(x => x.IsActive == true && x.UnderCompanyId == cm.Id && x.CompanyType == "Dealer").ToList();

                    TreeViewItem tvi = new TreeViewItem();
                    tvi.Header = cm.CompanyName;
                    tvi.Tag = cm.CompanyType;
                    if (lstWarehouse.Count() == 0 && lstDealer.Count() == 0)
                    {
                        TreeViewItem tviWH = new TreeViewItem();
                        tviWH.Header = " ";
                        tviWH.Tag = "";
                        tvi.Items.Add(tviWH);
                    }
                    if (lstWarehouse.Count() > 0)
                    {
                        TreeViewItem tviWH = new TreeViewItem();
                        tviWH.Header = "WareHouse";
                        tviWH.Tag = "";
                        foreach (var wh in lstWarehouse)
                        {
                            TreeViewItem tviWHCH = new TreeViewItem();
                            tviWHCH.Header = wh.CompanyName;
                            tviWHCH.Tag = wh.CompanyType;
                            tviWH.Items.Add(tviWHCH);
                        }

                        tvi.Items.Add(tviWH);
                    }

                    if (lstDealer.Count() > 0)
                    {
                        TreeViewItem tviDL = new TreeViewItem();
                        tviDL.Header = "Dealer";
                        tviDL.Tag = "";
                        foreach (var dl in lstDealer)
                        {
                            TreeViewItem tviDLCH = new TreeViewItem();
                            tviDLCH.Header = dl.CompanyName;
                            tviDLCH.Tag = dl.CompanyType;
                            tviDL.Items.Add(tviDLCH);
                        }

                        tvi.Items.Add(tviDL);
                    }
                    trvCompany.Items.Add(tvi);
                }
            }
        }

        #endregion

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ClearForm();
            Common.AppLib.WriteLog("frmLogin_Loaded");
        }

        private void MetroWindow_Activated(object sender, EventArgs e)
        {
            Common.AppLib.WriteLog("frmLogin_Activated");
            ClearForm();
        }

        private void MetroWindow_Deactivated(object sender, EventArgs e)
        {
            Common.AppLib.WriteLog("frmLogin_Deactivated");
        }


        

    }
}
