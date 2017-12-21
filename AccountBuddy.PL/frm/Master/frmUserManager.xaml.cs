﻿using System;
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
using AccountBuddy.Common;
using Microsoft.AspNet.SignalR.Client;
using MahApps.Metro.Controls;

namespace AccountBuddy.PL.frm.Master
{
    /// <summary>
    /// Interaction logic for frmUserManager.xaml
    /// </summary>
    public partial class frmUserManager : MetroWindow
    {
        public int CompanyId;

        public frmUserManager()
        {
            InitializeComponent();
        }


        private void btnNewUser_Click(object sender, RoutedEventArgs e)
        {
           
                frmUser f = new frmUser();
                f.UnderCompanyId = CompanyId;
                f.LoadWindow(CompanyId);
                f.ShowDialog();
                LoadWindow(CompanyId);
                //f.data.UserType.CompanyId = userId;
            


        }

        private void btnEditUser_Click(object sender, RoutedEventArgs e)
        {
            if (BLL.UserAccount.AllowUpdate(Forms.frmUser))
            {
                var u = dgvUsers.SelectedItem as BLL.UserAccount;

                frmUser f = new frmUser();
                f.UnderCompanyId = CompanyId;
                f.LoadWindow(CompanyId);
                u.toCopy<BLL.UserAccount>(f.data);
                f.ShowDialog();
                LoadWindow(CompanyId);
            }
            else
            {
                MessageBox.Show("No Permission to Update", "Users", MessageBoxButton.OK, MessageBoxImage.Warning);
            }



        }

        private void btnDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            var u = dgvUsers.SelectedItem as BLL.UserAccount;
            if (!BLL.UserAccount.AllowDelete(Forms.frmUserType))
            {
                MessageBox.Show("No Permission to Delete", "Users", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            else if (u != null)
            {
                if (BLL.UserAccount.toList.Count() == 1)
                {
                    MessageBox.Show(string.Format("You can not delete this user. atleast one user required"));
                }
                else if (MessageBox.Show("Do you Delete this?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    if (u.Delete() == true)
                    {
                        MessageBox.Show(Message.PL.Delete_Alert);
                        LoadWindow(CompanyId);
                    }
                }
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        public void LoadWindow(int CompanyId)
        {
            try
            {
                dgvUsers.ItemsSource = BLL.UserAccount.toList.Where(x => x.UserType.CompanyId == CompanyId).ToList();
            }
            catch(Exception ex)
            {
                Common.AppLib.WriteLog(ex);
            }
        }

    }
}
