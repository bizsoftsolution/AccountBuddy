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
using Microsoft.Win32;
using AccountBuddy.Common;
using System.IO;

namespace AccountBuddy.PL
{
    /// <summary>
    /// Interaction logic for frmCompanySignup.xaml
    /// </summary>
    public partial class frmCompanySignup : MetroWindow
    {
        public BLL.CompanyDetail data = new BLL.CompanyDetail();
        public bool IsForcedClose = false;
        public frmCompanySignup()
        {
            InitializeComponent();
            this.DataContext = data;
            IsForcedClose = false;
            Common.AppLib.WriteLog("frmSignup_Init");
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Common.AppLib.WriteLog("frmCompanySignup_btnBack_Click_Start");
            this.Close();
            Common.AppLib.WriteLog("frmCompanySignup_btnBack_Click_End");
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            Common.AppLib.WriteLog("frmCompanySignup_btnClear_Click_Start");
            data.Clear();
            txtPassword.Password = "";
            iLogoImage.Tag = "";
            Common.AppLib.WriteLog("frmCompanySignup_btnClear_Click_End");
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

            Common.AppLib.WriteLog("frmCompanySignup_btnSave_Click_Start");
            if (data.Save() == true)
            {
                MessageBox.Show(Message.PL.Saved_Alert, this.Title.ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
                BLL.UserType.Init();
                BLL.UserAccount.Init();
                IsForcedClose = true;
                Close();
            }
            else
            {
                MessageBox.Show(string.Join("\n", data.lstValidation.Select(x => x.Message).ToList()));
            }
            Common.AppLib.WriteLog("frmCompanySignup_btnSave_Click_End");
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

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (IsForcedClose == false && MessageBox.Show("Are you sure to close the signup?", "Close", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                e.Cancel = true;
            }
            else{
                Common.AppLib.WriteLog("frmSignup_Closed");
            }
        }

        private void txtMail_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtMail.Text != "" && !Common.AppLib.IsValidEmailAddress(txtMail.Text)) MessageBox.Show("Please Enter the Valid Email or Leave Empty");
        }

        private void btnImage_Click(object sender, RoutedEventArgs e)
        {
            Common.AppLib.WriteLog("frmCompanySignup_btnImage_Click_Start");
            try
            {
                OpenFileDialog OpenDialogBox = new OpenFileDialog();
                OpenDialogBox.DefaultExt = "Images (*.BMP;*.JPG;*.GIF,*.PNG,*.TIFF)|*.BMP;*.JPG;*.GIF;*.PNG;*.TIFF|All files (*.*)|*.*";
                OpenDialogBox.Filter = "Images (*.BMP;*.JPG;*.GIF,*.PNG,*.TIFF)|*.BMP;*.JPG;*.GIF;*.PNG;*.TIFF|All files (*.*)|*.*";

                var browsefile = OpenDialogBox.ShowDialog();
                var fileLength = new FileInfo(OpenDialogBox.FileName).Length;
            
                if (browsefile == true)
                {
                    string sFileName = OpenDialogBox.FileName.ToString();
                    if (!string.IsNullOrEmpty(sFileName))
                    {
                        ImageSource imageSource = new BitmapImage(new Uri(sFileName));

                        iLogoImage.Source = imageSource;
                        iLogoImage.Tag = AppLib.ReadImageFile(sFileName);
                    }

                }
            }
            catch (Exception ex)
            { Common.AppLib.WriteLog(ex); }

            Common.AppLib.WriteLog("frmCompanySignup_btnImage_Click_End");

        }

        private void MetroWindow_Activated(object sender, EventArgs e)
        {
            Common.AppLib.WriteLog("frmSignup_Activated");
        }

        private void MetroWindow_Deactivated(object sender, EventArgs e)
        {
            Common.AppLib.WriteLog("frmSignup_Deactivated");
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Common.AppLib.WriteLog("frmSignup_Loaded");
        }
    }
}
