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
using AccountBuddy.BLL;
using System.IO;

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
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{try
			{
				BLL.CompanyDetail.Init();

				data.Find(BLL.UserAccount.User.UserType.Company.Id);
                
                if (data.CFiles.Count != 0)
				{
                    var i=data.CFiles.Where(x => x.AttchmentCode == DataKeyValue.Logo_Key).FirstOrDefault();
                    if (i!=null)
                    {
                        iLogoImage.Source = AppLib.ViewImage(i.Image);
                        iLogoImage.Tag = i.Image;
                        
                    }
                   var b = data.CFiles.Where(x => x.AttchmentCode == DataKeyValue.BackGround_Key).FirstOrDefault();
                    if (b != null)
                    {
                        iBImage.Source = AppLib.ViewImage(b.Image);
                        iBImage.Tag = b.Image;
                        frmWelcome frm = new frmWelcome();
                        frm.imgBackground.ImageSource = Common.AppLib.ViewImage(b.Image);
                    }

                }

				if (data.CompanyType == "Company")
				{
					var lstCompany = BLL.CompanyDetail.ToList.Where(x => x.CompanyType == "Warehouse" && x.UnderCompanyId == BLL.UserAccount.User.UserType.Company.Id && x.IsActive == true);
					dgvWarehouse.ItemsSource = lstCompany;
					dgvDealer.ItemsSource = BLL.CompanyDetail.ToList.Where(x => x.CompanyType == "Dealer" && x.UnderCompanyId == BLL.UserAccount.User.UserType.Company.Id && x.IsActive == true).ToList();
				}
				else
				{
					gbxWareHouse.Visibility = Visibility.Collapsed;
					gbxDealer.Visibility = Visibility.Collapsed;
					btnUser.Visibility = Visibility.Collapsed;
					btnDelete.Visibility = Visibility.Collapsed;
				}

				var l = BLL.Ledger.toList.Where(x => x.AccountGroup.GroupName == BLL.DataKeyValue.BankAccounts_Key).ToList();
				cmbBank.ItemsSource = l;
				cmbBank.DisplayMemberPath = "LedgerName";
				cmbBank.SelectedValuePath = "Id";
				cmbBank.SelectedItem = l.FirstOrDefault();
				btnSave.Visibility = (BLL.CompanyDetail.UserPermission.AllowInsert || BLL.CompanyDetail.UserPermission.AllowUpdate) ? Visibility.Visible : Visibility.Collapsed;
				btnDelete.Visibility = BLL.CompanyDetail.UserPermission.AllowDelete ? Visibility.Visible : Visibility.Collapsed;
			}
			catch (Exception ex)
			{
				Common.AppLib.WriteLog(ex);
			}
		}
        public static BitmapImage BitmapImageFromBytes(byte[] bytes)
        {
            BitmapImage image = null;
            MemoryStream stream = null;
            try
            {
                stream = new MemoryStream(bytes);
                stream.Seek(0, SeekOrigin.Begin);
                System.Drawing.Image img = System.Drawing.Image.FromStream(stream);
                image = new BitmapImage();
                image.BeginInit();
                MemoryStream ms = new MemoryStream();
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                ms.Seek(0, SeekOrigin.Begin);
                image.StreamSource = ms;
                image.StreamSource.Seek(0, SeekOrigin.Begin);
                image.EndInit();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
            return image;
        }


        public void Grid_Refresh()
		{
			try
			{
				CollectionViewSource.GetDefaultView(dgvWarehouse.ItemsSource).Refresh();
				CollectionViewSource.GetDefaultView(dgvDealer.ItemsSource).Refresh();

				dgvDealer.ItemsSource = BLL.CompanyDetail.ToList.Where(x => x.CompanyType == "Dealer" && x.UnderCompanyId == BLL.UserAccount.User.UserType.Company.Id && x.IsActive == true).ToList();
				dgvWarehouse.ItemsSource = BLL.CompanyDetail.ToList.Where(x => x.CompanyType == "Warehouse" && x.UnderCompanyId == BLL.UserAccount.User.UserType.Company.Id && x.IsActive == true).ToList();

			}
			catch (Exception ex) { Common.AppLib.WriteLog(ex); };

		}


		#region ButtonEvents

		private void btnSave_Click(object sender, RoutedEventArgs e)
		{

			if (data.Id == 0 && !BLL.UserAccount.AllowInsert(Forms.frmCompanySetting))
			{
				MessageBox.Show(string.Format(Message.PL.DenyInsert, FormName), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
			}
			else if (data.Id != 0 && !BLL.UserAccount.AllowUpdate(Forms.frmCompanySetting))
			{
				MessageBox.Show(string.Format(Message.PL.DenyUpdate, FormName), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
			}
			else if (txtMail.Text != "" && !Common.AppLib.IsValidEmailAddress(txtMail.Text))
			{
				MessageBox.Show("Please Enter the Valid Email or Leave Empty", FormName, MessageBoxButton.OK, MessageBoxImage.Warning);

			}
			else
			{
				if (data.Save() == true)
				{
					MessageBox.Show(Message.PL.Saved_Alert, FormName, MessageBoxButton.OK, MessageBoxImage.Information);
					App.frmHome.ShowWelcome();
				}
			}

		}

		private void btnDelete_Click(object sender, RoutedEventArgs e)
		{
			if (!BLL.CompanyDetail.UserPermission.AllowDelete)
				MessageBox.Show(string.Format(Message.PL.DenyDelete, lblHead.Text));
			//    else if (MessageBox.Show(Message.PL.Delete_confirmation, "", MessageBoxButton.YesNo) != MessageBoxResult.No)
			else if (MessageBox.Show(Message.PL.Delete_confirmation, FormName, MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.No)
			{
				frmDeleteConfirmation frm = new frmDeleteConfirmation();
				frm.ShowDialog();
				if (frm.RValue == true)
				{
					if (data.Delete() == true)
					{
						MessageBox.Show(Message.PL.Delete_Alert, FormName, MessageBoxButton.OK, MessageBoxImage.Information);
						App.frmHome.IsForcedClose = true;
						App.frmHome.Close();
					}
				}
				else
				{
					MessageBox.Show(Message.PL.Cant_Delete_Alert, FormName, MessageBoxButton.OK, MessageBoxImage.Warning);

				}


			}

		}


		private void btnNewWareHouse_Click(object sender, RoutedEventArgs e)
		{
			if (!BLL.UserAccount.AllowInsert(Forms.frmCompanySetting))
			{
				MessageBox.Show("No Permission to insert new warehouse", "New Warehouse", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
			else
			{
				frmCompanySignup f = new frmCompanySignup();
				f.data.Clear();
				f.data.UnderCompanyId = BLL.UserAccount.User.UserType.Company.Id;
				f.data.CompanyType = "Warehouse";
				f.Title = "New Warehouse";
				f.ShowDialog();
				List<BLL.CompanyDetail> lstCompany = new List<BLL.CompanyDetail>();
				lstCompany = BLL.CompanyDetail.ToList.Where(x => x.CompanyType == "Warehouse" && x.UnderCompanyId == BLL.UserAccount.User.UserType.Company.Id && x.IsActive == true).ToList();
				dgvWarehouse.ItemsSource = lstCompany;
			}

		}

		private void btnNewDealer_Click(object sender, RoutedEventArgs e)
		{

			if (!BLL.UserAccount.AllowInsert(Forms.frmCompanySetting))
			{
				MessageBox.Show("No Permission to insert new dealer", "New Dealer", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
			else
			{
				frmCompanySignup f = new frmCompanySignup();
				f.data.Clear();
				f.data.UnderCompanyId = BLL.UserAccount.User.UserType.Company.Id;
				f.data.CompanyType = "Dealer";
				f.Title = "New Dealer";
				f.ShowDialog();
				List<BLL.CompanyDetail> lstCompany = new List<BLL.CompanyDetail>();

				lstCompany = BLL.CompanyDetail.ToList.Where(x => x.CompanyType == "Dealer" && x.UnderCompanyId == BLL.UserAccount.User.UserType.Company.Id && x.IsActive == true).ToList();

				dgvDealer.ItemsSource = lstCompany;
				Grid_Refresh();
			}
		}

		private void btnEditWarehouse_Click(object sender, RoutedEventArgs e)
		{

			if (!BLL.UserAccount.AllowUpdate(Forms.frmCompanySetting))
			{
				MessageBox.Show("No Permission to Update", FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
			}
			else
			{
				var cm = dgvWarehouse.SelectedItem as BLL.CompanyDetail;
				frmCompanySignup f = new frmCompanySignup();
				cm.ToMap(f.data);
				f.data.UnderCompanyId = BLL.UserAccount.User.UserType.Company.Id;
				f.data.CompanyType = "Warehouse";
				f.iLogoImage.Source = AppLib.ViewImage(cm.Logo);
				f.iLogoImage.Tag = cm.Logo;
				f.Title = "Edit Warehouse";
				f.gbxLogin.Visibility = Visibility.Collapsed;
				f.ShowDialog();
				var lstCompany = BLL.CompanyDetail.ToList.Where(x => x.CompanyType == "Warehouse" && x.UnderCompanyId == BLL.UserAccount.User.UserType.Company.Id && x.IsActive == true);
				dgvWarehouse.ItemsSource = lstCompany;
			}

		}

		private void btnDeleteWarehouse_Click(object sender, RoutedEventArgs e)
		{
			var d = dgvWarehouse.SelectedItem as BLL.CompanyDetail;
			data = d;
			if (!BLL.UserAccount.AllowDelete(Forms.frmCompanySetting))
			{
				MessageBox.Show("No Permission to Delete", FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
			}
			else if (d.Id != 0)
			{
				if (!BLL.UserAccount.AllowDelete(FormName))
				{
					MessageBox.Show(string.Format(Message.PL.DenyDelete, FormName), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
				}
				else if (MessageBox.Show(Message.PL.Delete_confirmation, FormName, MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.No)
				{
					frmDeleteConfirmation frm = new frmDeleteConfirmation();
					frm.ShowDialog();
					if (frm.RValue == true)
					{

						if (data.DeleteWareHouse(d.Id))
						{
							MessageBox.Show(Message.PL.Delete_Alert, FormName, MessageBoxButton.OK, MessageBoxImage.Information);
							Grid_Refresh();
						}
						else
						{

							MessageBox.Show(Message.PL.Cant_Delete_Alert, FormName, MessageBoxButton.OK, MessageBoxImage.Information);
						}
					}
				}
				else
				{

				}
			}
			else
			{
				MessageBox.Show("No Records to Delete", FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
			}

		}

		private void btnEditDealer_Click(object sender, RoutedEventArgs e)
		{
			if (!BLL.UserAccount.AllowUpdate(Forms.frmCompanySetting))
			{
				MessageBox.Show("No Permission to Update", FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
			}
			else
			{
				var cm = dgvDealer.SelectedItem as BLL.CompanyDetail;

				frmCompanySignup f = new frmCompanySignup();
				cm.ToMap(f.data);
				f.data.UnderCompanyId = BLL.UserAccount.User.UserType.Company.Id;
				f.data.CompanyType = "Dealer";
				f.iLogoImage.Source = AppLib.ViewImage(cm.Logo);
				f.iLogoImage.Tag = cm.Logo;
				f.Title = "Edit Dealer";
				f.gbxLogin.Visibility = Visibility.Collapsed;
				f.ShowDialog();
				var lstCompany = BLL.CompanyDetail.ToList.Where(x => x.CompanyType == "Dealer" && x.UnderCompanyId == BLL.UserAccount.User.UserType.Company.Id && x.IsActive == true);
				dgvDealer.ItemsSource = lstCompany;

			}
		}

		private void btnDeleteDealer_Click(object sender, RoutedEventArgs e)
		{
			var d = dgvDealer.SelectedItem as BLL.CompanyDetail;
			data = d;
			if (!BLL.UserAccount.AllowDelete(Forms.frmCompanySetting))
			{
				MessageBox.Show("No Permission to Delete", FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
			}
			else if (d.Id != 0)
			{
				if (d.Id != 0)
				{
					if (!BLL.UserAccount.AllowDelete(FormName))
					{
						MessageBox.Show(string.Format(Message.PL.DenyDelete, FormName), FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
					}

					else if (MessageBox.Show(Message.PL.Delete_confirmation, FormName, MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.No)
					{
						frmDeleteConfirmation frm = new frmDeleteConfirmation();
						frm.ShowDialog();
						if (frm.RValue == true)
						{

							if (data.DeleteWareHouse(d.Id))
							{
								MessageBox.Show(Message.PL.Delete_Alert);
								Grid_Refresh();
							}
							else
							{

								MessageBox.Show(Message.PL.Cant_Delete_Alert);
							}
						}
					}
				}
				else
				{
					MessageBox.Show("No Records to Delete", FormName, MessageBoxButton.OK, MessageBoxImage.Warning);
				}
			}
		}

		private void btnUserWarehouse_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				var cm = dgvWarehouse.SelectedItem as BLL.CompanyDetail;
				frmUserManager f = new frmUserManager();
				f.LoadWindow(cm.Id);
				f.CompanyId = cm.Id;
				f.Title = string.Format("Login Users - {0}", cm.CompanyName);
				f.ShowDialog();
			}
			catch (Exception ex) { Common.AppLib.WriteLog(ex); }
		}

		private void btnImage_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				string sFileName = FileName();
				if (!string.IsNullOrEmpty(sFileName))
				{

					ImageSource imageSource = new BitmapImage(new Uri(sFileName));
iLogoImage.Source = imageSource;
					iLogoImage.Tag = AppLib.ReadImageFile(sFileName);
					
					data.AddFiles(AppLib.ReadImageFile(sFileName), DataKeyValue.Logo_Key);
				}


			}
			catch (Exception ex)
			{ Common.AppLib.WriteLog(ex); }

		}

		private string FileName()
		{
			string sFileName = null;
			OpenFileDialog OpenDialogBox = new OpenFileDialog();
			OpenDialogBox.DefaultExt = "Images (*.BMP;*.JPG;*.GIF,*.PNG,*.TIFF)|*.BMP;*.JPG;*.GIF;*.PNG;*.TIFF|All files (*.*)|*.*";
			OpenDialogBox.Filter = "Images (*.BMP;*.JPG;*.GIF,*.PNG,*.TIFF)|*.BMP;*.JPG;*.GIF;*.PNG;*.TIFF|All files (*.*)|*.*";

			var browsefile = OpenDialogBox.ShowDialog();
			if (browsefile == true)
			{
				sFileName = OpenDialogBox.FileName.ToString();

			}
			return sFileName;
		}
		#endregion

		#region Events

		private void dgvWarehouse_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var cm = dgvWarehouse.SelectedItem as BLL.CompanyDetail;
			List<BLL.CompanyDetail> lstCompany = new List<BLL.CompanyDetail>();
			if (cm != null)
			{

			};
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

		private void btnUserDealer_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				var cm = dgvDealer.SelectedItem as BLL.CompanyDetail;
				frmUserManager f = new frmUserManager();
				f.LoadWindow(cm.Id);
				f.CompanyId = cm.Id;
				f.Title = string.Format("Login Users - {0}", cm.CompanyName);
				f.ShowDialog();
			}
			catch (Exception ex) { Common.AppLib.WriteLog(ex); }
		}

		private void btnUser_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				frmUserManager f = new frmUserManager();
				f.LoadWindow(BLL.UserAccount.User.UserType.CompanyId);
				f.CompanyId = BLL.UserAccount.User.UserType.CompanyId;
				f.Title = string.Format("Login Users - {0}", BLL.UserAccount.User.UserType.Company.CompanyName);
				f.ShowDialog();
			}
			catch (Exception ex) { Common.AppLib.WriteLog(ex); }
		}

		private void btnSetting_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				frmCustomSetting f = new frmCustomSetting();
				f.LoadWindow();
				f.ShowDialog();
			}
			catch (Exception ex) { Common.AppLib.WriteLog(ex); }
		}

		private void UserControl_Unloaded(object sender, RoutedEventArgs e)
		{

		}

		private void cmbBank_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			try
			{
				if (BLL.Bank.toList.Count != 0)
				{
					var b = cmbBank.SelectedItem as BLL.Bank;
					AppLib.BankId = b.LedgerId;
					AppLib.BankName = b.Ledger.LedgerName;
				}
			}
			catch (Exception ex)
			{
				Common.AppLib.WriteLog(ex);
			}

		}

		private void btnClearImage_Click(object sender, RoutedEventArgs e)
		{
			try
			{
                data.RemoveFiles(DataKeyValue.Logo_Key);
                iLogoImage.Source = null;
				iLogoImage.Tag = null;
                
            }
			catch (Exception ex)
			{
				Common.AppLib.WriteLog(ex);
			}
		}

		private void btnBGClearImage_Click(object sender, RoutedEventArgs e)
		{
			try
            {
                data.RemoveFiles(DataKeyValue.BackGround_Key);

                iBImage.Source = null;
				iBImage.Tag = null;
			}
			catch (Exception ex)
			{
				Common.AppLib.WriteLog(ex);
			}
		}

		private void btnBackImage_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				string sFileName = FileName();
				if (!string.IsNullOrEmpty(sFileName))
				{
					ImageSource imageSource = new BitmapImage(new Uri(sFileName));
					iBImage.Source = imageSource;
					iBImage.Tag = AppLib.ReadImageFile(sFileName);
					
					data.AddFiles(AppLib.ReadImageFile(sFileName), DataKeyValue.BackGround_Key);
				}


			}
			catch (Exception ex)
			{ Common.AppLib.WriteLog(ex); }

		}
	}
}
