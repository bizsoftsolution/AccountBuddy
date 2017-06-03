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
            data.Find(BLL.UserAccount.User.UserType.Company.Id);
            iProductImage.Source = AppLib.ViewImage(data.Logo);
            iProductImage.Tag = data.Logo;

            var lstCompany = BLL.CompanyDetail.toList.Where(x => x.CompanyType == "Warehouse" && x.UnderCompanyId == BLL.UserAccount.User.UserType.Company.Id);
            dgvWarehouse.ItemsSource = lstCompany;

            var lstWareHouse = lstCompany.Where(x => x.UnderCompanyId == BLL.UserAccount.User.UserType.CompanyId);
            dgvDealer.ItemsSource = lstWareHouse;
        }
        private void Grid_Refresh()
        {
            try
            {
                CollectionViewSource.GetDefaultView(dgvWarehouse.ItemsSource).Refresh();
                CollectionViewSource.GetDefaultView(dgvDealer.ItemsSource).Refresh();
            }
            catch (Exception ex) { };

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
            else
            {
                if (data.Save() == true)
                {
                    MessageBox.Show(Message.PL.Saved_Alert);
                    //   App.frmHome.ShowWelcome();
                }
            }

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


        private void btnNewWareHouse_Click(object sender, RoutedEventArgs e)
        {
            frmCompanySignup f = new frmCompanySignup();
            f.data.UnderCompanyId = BLL.UserAccount.User.UserType.Company.Id;
            f.data.CompanyType = "Warehouse";
            f.ShowDialog();
            var lstCompany = BLL.CompanyDetail.toList.Where(x => x.CompanyType == "Warehouse" && x.UnderCompanyId == BLL.UserAccount.User.UserType.Company.Id);
            dgvWarehouse.ItemsSource = lstCompany;
        }

        private void btnNewDealer_Click(object sender, RoutedEventArgs e)
        {
            var cm = dgvWarehouse.SelectedItem as BLL.CompanyDetail;
            if (cm != null)
            {
                frmCompanySignup f = new frmCompanySignup();
                f.data.UnderCompanyId = cm.Id;
                f.data.CompanyType = "Dealer";
                f.ShowDialog();
                List<BLL.CompanyDetail> lstCompany = new List<BLL.CompanyDetail>();
                if (cm != null)
                {
                    lstCompany = BLL.CompanyDetail.toList.Where(x => x.CompanyType == "Dealer" && x.UnderCompanyId == cm.Id).ToList();
                }
                dgvDealer.ItemsSource = lstCompany;
            }
        }

        private void btnEditWarehouse_Click(object sender, RoutedEventArgs e)
        {
            var cm = dgvWarehouse.SelectedItem as BLL.CompanyDetail;

            frmCompanySignup f = new frmCompanySignup();
            cm.toCopy<BLL.CompanyDetail>(f.data);
            f.data.UnderCompanyId = BLL.UserAccount.User.UserType.Company.Id;
            f.data.CompanyType = "Warehouse";
            f.ShowDialog();
            var lstCompany = BLL.CompanyDetail.toList.Where(x => x.CompanyType == "Warehouse" && x.UnderCompanyId == BLL.UserAccount.User.UserType.Company.Id);
            dgvWarehouse.ItemsSource = lstCompany;
        }

        private void btnDeleteWarehouse_Click(object sender, RoutedEventArgs e)
        {
            var d = dgvWarehouse.SelectedItem as BLL.CompanyDetail;
            data = d;
            if (d.Id != 0)
            {
                if (!BLL.UserAccount.AllowDelete(FormName))
                {
                    MessageBox.Show(string.Format(Message.PL.DenyDelete, FormName));
                }
                else
                {
                    if (MessageBox.Show(Message.PL.Delete_confirmation, "DELETE", MessageBoxButton.YesNo) != MessageBoxResult.No)
                    {
                        if (data.DeleteWareHouse(d.Id))
                        {
                            MessageBox.Show(Message.PL.Delete_Alert);
                            Grid_Refresh();
                        }


                    }
                }
            }
            else
            {
                MessageBox.Show("No Records to Delete");
            }

        }

        private void btnEditDealer_Click(object sender, RoutedEventArgs e)
        {
            var cm = dgvWarehouse.SelectedItem as BLL.CompanyDetail;
            var cmd = dgvDealer.SelectedItem as BLL.CompanyDetail;
            if (cm != null)
            {
                frmCompanySignup f = new frmCompanySignup();
                cmd.toCopy<BLL.CompanyDetail>(f.data);
                f.data.UnderCompanyId = cm.Id;
                f.data.CompanyType = "Dealer";
                f.ShowDialog();
                List<BLL.CompanyDetail> lstCompany = new List<BLL.CompanyDetail>();
                if (cm != null)
                {
                    lstCompany = BLL.CompanyDetail.toList.Where(x => x.CompanyType == "Dealer" && x.UnderCompanyId == cm.Id).ToList();
                }
                dgvDealer.ItemsSource = lstCompany;
            }
        }

        private void btnDeleteDealer_Click(object sender, RoutedEventArgs e)
        {
            var d = dgvDealer.SelectedItem as BLL.CompanyDetail;
            data = d;
            if (d.Id != 0)
            {
                if (!BLL.UserAccount.AllowDelete(FormName))
                {
                    MessageBox.Show(string.Format(Message.PL.DenyDelete, FormName));
                }
                else
                {
                    if (MessageBox.Show(Message.PL.Delete_confirmation, "DELETE", MessageBoxButton.YesNo) != MessageBoxResult.No)
                    {
                        if (data.DeleteWareHouse(d.Id))
                        {
                            MessageBox.Show(Message.PL.Delete_Alert);
                            Grid_Refresh();
                        }


                    }
                }
            }
            else
            {
                MessageBox.Show("No Records to Delete");
            }
        }

        #endregion

        #region Events

        private void dgvWarehouse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cm = dgvWarehouse.SelectedItem as BLL.CompanyDetail;
            List<BLL.CompanyDetail> lstCompany = new List<BLL.CompanyDetail>();
            if (cm != null)
            {
                lstCompany = BLL.CompanyDetail.toList.Where(x => x.CompanyType == "Dealer" && x.UnderCompanyId == cm.Id).ToList();
            }
            dgvDealer.ItemsSource = lstCompany;
        }

        #endregion

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
    }
}
