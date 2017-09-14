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
using Microsoft.Win32;

namespace AccountBuddy.PL.frm.Master
{
    /// <summary>
    /// Interaction logic for frmCompanySetting.xaml
    /// </summary>
    public partial class frmCompanySetting : UserControl
    {
        BLL.CompanyDetail data = new BLL.CompanyDetail();
        public string FormName = "CompanySetting";
        public frmCompanySetting()
        {
            InitializeComponent();
            this.DataContext = data;

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

            BLL.FMCGHubClient.FMCGHub.On<BLL.UserAccount>("UserAccount_Save", (ua) =>
            {

                this.Dispatcher.Invoke(() =>
                {
                    BLL.UserAccount u = new BLL.UserAccount();
                    ua.toCopy<BLL.UserAccount>(u);
                    BLL.UserAccount.toList.Add(u);
                });

            });
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            BLL.CompanyDetail.Init();
            data.Find(BLL.UserAccount.User.UserType.Company.Id);
            iProductImage.Source = AppLib.ViewImage(data.Logo);
            iProductImage.Tag = data.Logo;
            iBanner1Image.Source = AppLib.ViewImage(data.Banner1);
            iBanner1Image.Tag = data.Banner1;
            iBanner2Image.Source = AppLib.ViewImage(data.Banner2);
            iBanner2Image.Tag = data.Banner2;

            //var u = BLL.UserAccount.toList.Where(x => x.UserType.CompanyId == BLL.UserAccount.User.UserType.Company.Id).FirstOrDefault();
            //txtUserName.Text = u.LoginId;
            //txtPassword.Password = u.Password;
            //data.UserId = u.LoginId;

            cmbState.ItemsSource = BLL.StateDetail.toList;
            cmbState.DisplayMemberPath = "StateName";
            cmbState.SelectedValuePath = "Id";
        }
    
        #region ButtonEvents

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

            if (!BLL.UserAccount.AllowInsert(FormName))
            {
                MessageBox.Show(string.Format(Message.PL.DenyInsert, FormName));
            }
            else if (!BLL.UserAccount.AllowUpdate(FormName))
            {
                MessageBox.Show(string.Format(Message.PL.DenyUpdate, FormName));
            }
            else if (txtMail.Text != "" && !Common.AppLib.IsValidEmailAddress(txtMail.Text))
            {
                MessageBox.Show("Please Enter the Valid Email or Leave Empty");

            }
            else
            {
                if (data.Save() == true)
                {
                    MessageBox.Show(Message.PL.Saved_Alert);
                    App.frmHome.ShowWelcome();
                }
            }

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (!BLL.CompanyDetail.UserPermission.AllowDelete)
                MessageBox.Show(string.Format(Message.PL.DenyDelete, lblHead.Text));
            //    else if (MessageBox.Show(Message.PL.Delete_confirmation, "", MessageBoxButton.YesNo) != MessageBoxResult.No)
            else if (MessageBox.Show(Message.PL.Delete_confirmation, "", MessageBoxButton.YesNo) != MessageBoxResult.No)
            {
                frmDeleteConfirmation frm = new frmDeleteConfirmation();
                frm.ShowDialog();
                if (frm.RValue == true)
                {
                    if (data.Delete() == true)
                    {
                        MessageBox.Show(Message.PL.Delete_Alert);
                        App.frmHome.IsForcedClose = true;
                        App.frmHome.Close();
                    }
                }
                else
                {
                    MessageBox.Show(Message.PL.Cant_Delete_Alert);

                }


            }

        }
    
        private void btnImage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog OpenDialogBox = new OpenFileDialog();
                OpenDialogBox.DefaultExt = "Images (*.BMP;*.JPG;*.GIF,*.PNG,*.TIFF)|*.BMP;*.JPG;*.GIF;*.PNG;*.TIFF|All files (*.*)|*.*";
                OpenDialogBox.Filter = "Images (*.BMP;*.JPG;*.GIF,*.PNG,*.TIFF)|*.BMP;*.JPG;*.GIF;*.PNG;*.TIFF|All files (*.*)|*.*";

                var browsefile = OpenDialogBox.ShowDialog();
                if (browsefile == true)
                {
                    string sFileName = OpenDialogBox.FileName.ToString();
                    if (!string.IsNullOrEmpty(sFileName))
                    {
                        ImageSource imageSource = new BitmapImage(new Uri(sFileName));
                        iProductImage.Source = imageSource;
                        iProductImage.Tag = AppLib.ReadImageFile(sFileName);
                    }

                }
            }
            catch (Exception ex)
            { }

        }
        #endregion

        private void txtPassword_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Cut ||
                e.Command == ApplicationCommands.Copy ||
                e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true;
            }
        }

        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            data.Password = txtPassword.Password;
        }
      
        #region Numeric Only
        private void NumericOnly(System.Object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = Common.AppLib.IsTextNumeric(e.Text);
        }
        #endregion

        #region Mail Validation
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





        #endregion
     
        private void btnSetting_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                frmCustomSetting f = new frmCustomSetting();
                f.LoadWindow();
                f.ShowDialog();
            }
            catch (Exception ex) { }
        }

        private void txtCGSTAmount_TextChanged(object sender, TextChangedEventArgs e)
        {

            TextBox textBox = sender as TextBox;
            Int32 selectionStart = textBox.SelectionStart;
            Int32 selectionLength = textBox.SelectionLength;
            textBox.Text = AppLib.NumericOnly(txtCGSTAmount.Text);
            textBox.SelectionStart = selectionStart <= textBox.Text.Length ? selectionStart : textBox.Text.Length;


        }

        private void txtSGSTAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Int32 selectionStart = textBox.SelectionStart;
            Int32 selectionLength = textBox.SelectionLength;
            textBox.Text = AppLib.NumericOnly(txtSGSTAmount.Text);
            textBox.SelectionStart = selectionStart <= textBox.Text.Length ? selectionStart : textBox.Text.Length;

        }

        private void txtIGSTAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Int32 selectionStart = textBox.SelectionStart;
            Int32 selectionLength = textBox.SelectionLength;
            textBox.Text = AppLib.NumericOnly(txtIGSTAmount.Text);
            textBox.SelectionStart = selectionStart <= textBox.Text.Length ? selectionStart : textBox.Text.Length;

        }

        private void btnBanner_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog OpenDialogBox = new OpenFileDialog();
                OpenDialogBox.DefaultExt = "Images (*.BMP;*.JPG;*.GIF,*.PNG,*.TIFF)|*.BMP;*.JPG;*.GIF;*.PNG;*.TIFF|All files (*.*)|*.*";
                OpenDialogBox.Filter = "Images (*.BMP;*.JPG;*.GIF,*.PNG,*.TIFF)|*.BMP;*.JPG;*.GIF;*.PNG;*.TIFF|All files (*.*)|*.*";

                var browsefile = OpenDialogBox.ShowDialog();
                if (browsefile == true)
                {
                    string sFileName = OpenDialogBox.FileName.ToString();
                    if (!string.IsNullOrEmpty(sFileName))
                    {
                        ImageSource imageSource = new BitmapImage(new Uri(sFileName));
                        iBanner1Image.Source = imageSource;
                        iBanner1Image.Tag = AppLib.ReadImageFile(sFileName);
                    }

                }
            }
            catch (Exception ex)
            { }
        }

        private void btnBanner2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog OpenDialogBox = new OpenFileDialog();
                OpenDialogBox.DefaultExt = "Images (*.BMP;*.JPG;*.GIF,*.PNG,*.TIFF)|*.BMP;*.JPG;*.GIF;*.PNG;*.TIFF|All files (*.*)|*.*";
                OpenDialogBox.Filter = "Images (*.BMP;*.JPG;*.GIF,*.PNG,*.TIFF)|*.BMP;*.JPG;*.GIF;*.PNG;*.TIFF|All files (*.*)|*.*";

                var browsefile = OpenDialogBox.ShowDialog();
                if (browsefile == true)
                {
                    string sFileName = OpenDialogBox.FileName.ToString();
                    if (!string.IsNullOrEmpty(sFileName))
                    {
                        ImageSource imageSource = new BitmapImage(new Uri(sFileName));
                        iBanner2Image.Source = imageSource;
                        iBanner2Image.Tag = AppLib.ReadImageFile(sFileName);
                    }

                }
            }
            catch (Exception ex)
            { }
        }
    }
}
