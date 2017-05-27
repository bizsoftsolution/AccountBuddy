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
        //public static string FormName = nameof(frmCompanySetting);
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
                    ua.Save(true);
                });

            });
        }


        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (data.Id == 0 && !BLL.CompanyDetail.UserPermission.AllowInsert)
            {
                MessageBox.Show(string.Format(Message.PL.DenyInsert, lblHead.Text));
            }
            else if (data.Id != 0 && !BLL.CompanyDetail.UserPermission.AllowUpdate)
            {
                MessageBox.Show(string.Format(Message.PL.DenyUpdate, lblHead.Text));
            }
            else if (data.Save() == true)
            {
                MessageBox.Show(Message.PL.Saved_Alert);
                App.frmHome.ShowWelcome();
            }
            else
            {
                MessageBox.Show(string.Join("\n", data.lstValidation.Select(x => x.Message).ToList()));
            }
        }

        

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            data.Find(BLL.UserAccount.User.UserType.Company.Id);

            var lstUser = BLL.UserAccount.toList;
            dgvUsers.ItemsSource = lstUser;
            
            if (BLL.UserAccount.UserPermission.IsViewForm)
            {
                gbxLoginUser.Visibility = Visibility.Visible;
                btnNewUser.Visibility = BLL.UserAccount.UserPermission.AllowInsert ? Visibility.Visible : Visibility.Collapsed;
                dgvUsers.Columns[0].Visibility = BLL.UserAccount.UserPermission.AllowUpdate ? Visibility.Visible : Visibility.Collapsed;
                dgvUsers.Columns[1].Visibility = BLL.UserAccount.UserPermission.AllowDelete ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                gbxLoginUser.Visibility = Visibility.Collapsed;
            }
            btnSave.Visibility = (BLL.CompanyDetail.UserPermission.AllowInsert || BLL.CompanyDetail.UserPermission.AllowUpdate) ? Visibility.Visible : Visibility.Collapsed;
            btnDelete.Visibility = BLL.CompanyDetail.UserPermission.AllowDelete ? Visibility.Visible : Visibility.Collapsed;            
        }

        private void btnNewUser_Click(object sender, RoutedEventArgs e)
        {
            frmUser f = new frmUser();
            f.ShowDialog();            
        }
        
        private void btnEditUser_Click(object sender, RoutedEventArgs e)
        {
            var u = dgvUsers.SelectedItem as BLL.UserAccount;

            frmUser f = new frmUser();
            u.toCopy<BLL.UserAccount>(f.data);
            f.ShowDialog();
        }

        private void btnDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            var u = dgvUsers.SelectedItem as BLL.UserAccount;
            if (u != null)
            {
                if(BLL.UserAccount.toList.Count() == 1)
                {
                    MessageBox.Show(string.Format("You can not delete this user. atleast one user required"));
                }
                else if (MessageBox.Show("Do you Delete this?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    if (u.Delete() == true) MessageBox.Show(Message.PL.Delete_Alert);
                }
            }
            
        }

        private void NumericOnly(System.Object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = Common.AppLib.IsTextNumeric(e.Text);

        }

        private void txtMail_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Int32 selectionStart = textBox.SelectionStart;
            Int32 selectionLength = textBox.SelectionLength;
            String newText = String.Empty;

            int AtCount = 0;
            foreach (Char c in textBox.Text.ToCharArray())
            {
                if (Char.IsLetterOrDigit(c) || Char.IsControl(c) || (c == '.' || c == '_') || (c == '@' && AtCount == 0))
                {
                    newText += c;
                    if (c == '@') AtCount += 1;
                }
            }
            textBox.Text = newText;
            textBox.SelectionStart = selectionStart <= textBox.Text.Length ? selectionStart : textBox.Text.Length;
        }

        private void txtMail_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtMail.Text != "" && !Common.AppLib.IsValidEmailAddress(txtMail.Text)) MessageBox.Show("Please Enter the Valid Email or Leave Empty");

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (!BLL.CompanyDetail.UserPermission.AllowDelete)
                MessageBox.Show(string.Format(Message.PL.DenyDelete, lblHead.Text));
            else if (MessageBox.Show(Message.PL.Delete_confirmation, "", MessageBoxButton.YesNo) != MessageBoxResult.No)
                if (data.Delete() == true)
                {
                    MessageBox.Show(Message.PL.Delete_Alert);
                    App.frmHome.IsForcedClose = true;
                    App.frmHome.Close();
                }
        }
    }
}
