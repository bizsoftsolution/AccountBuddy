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

namespace AccountBuddy.PL.frm.Master
{
    /// <summary>
    /// Interaction logic for frmUser.xaml
    /// </summary>
    public partial class frmUser : MetroWindow
    {
        public BLL.UserAccount data = new BLL.UserAccount();
        public int UnderCompanyId;
        public int LastCreateUserId;
        public frmUser()
        {
            InitializeComponent();
            this.DataContext = data;
        }
        public void LoadWindow(int CompanyId)
        {
            //btnUserTypeSetting.Visibility = BLL.UserType.UserPermission.IsViewForm ? Visibility.Visible : Visibility.Collapsed;
            cmbUserType.ItemsSource = BLL.UserType.toList.Where(x => x.CompanyId == CompanyId).ToList();
            UnderCompanyId = CompanyId;
            cmbUserType.DisplayMemberPath = "TypeOfUser";
            cmbUserType.SelectedValuePath = "Id";
        }
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            data.Clear();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            LastCreateUserId = 0;
            if (data.Id == 0 && BLL.UserAccount.UserPermission.AllowInsert == false)
            {
                MessageBox.Show("No Permission to Insert", "New User", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (data.Id != 0 && BLL.UserAccount.UserPermission.AllowUpdate == false)
            {
                MessageBox.Show("No Permission to Update", "New User", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (data.Save() == true)
            {
                MessageBox.Show(Message.PL.Saved_Alert);
                if (BLL.UserAccount.User.Id == data.Id)
                {
                    BLL.UserAccount.User.UserName = data.UserName;
                    App.frmHome.Title = String.Format("{0} - {1}", BLL.UserAccount.User.UserName, BLL.UserAccount.User.UserType.Company.CompanyName);
                }
                LastCreateUserId = data.Id;
                this.Close();
            }
            else
            {
                String str = String.Join("\n", data.lstValidation.Select(x => x.Message).ToList());
                MessageBox.Show(str);
            }
        }

        private void btnUserTypeSetting_Click(object sender, RoutedEventArgs e)
        {
           
                frmUserType frm = new frmUserType();
                frm.UnderCompanyId = UnderCompanyId;
                frm.LoadWindow(UnderCompanyId);
                frm.ShowDialog();
           
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            btnSave.Visibility = (BLL.CompanyDetail.UserPermission.AllowInsert || BLL.CompanyDetail.UserPermission.AllowUpdate) ? Visibility.Visible : Visibility.Collapsed;

            //if (BLL.UserAccount.AllowInsert(Common.Forms.frmUserType)|| BLL.UserAccount.AllowUpdate(Common.Forms.frmUserType))
            //{
            //    btnUserTypeSetting.Visibility = Visibility.Visible;
            //}
            //else
            //{
            //    btnUserTypeSetting.Visibility = Visibility.Collapsed;
            //}

        }
    }
}
