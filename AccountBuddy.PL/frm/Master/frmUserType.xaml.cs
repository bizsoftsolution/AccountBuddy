using MahApps.Metro.Controls;
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
using System.Windows.Shapes;

namespace AccountBuddy.PL.frm.Master
{
    /// <summary>
    /// Interaction logic for frmUserType.xaml
    /// </summary>
    public partial class frmUserType : MetroWindow
    {
        #region Field

        public static string FormName = "UserType";
        BLL.UserType data = new BLL.UserType();
        
        #endregion

        #region Constructor

        public frmUserType()
        {
            InitializeComponent();
            this.DataContext = data;

            onClientEvents();
        }

        #endregion

        #region Events

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dgvDetail.ItemsSource = BLL.UserType.toList;
            Clear();      
        }


        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (data.Id == 0 && !BLL.UserAccount.AllowInsert(FormName))
            {
                MessageBox.Show(string.Format(Message.PL.DenyInsert, FormName));
            }
            else if (data.Id != 0 && !BLL.UserAccount.AllowUpdate(FormName))
            {
                MessageBox.Show(string.Format(Message.PL.DenyUpdate, FormName));
            }
            else
            {
                if (data.Save() == true)
                {
                    MessageBox.Show("Saved");
                    this.Close();
                }
            }

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (data.Id == 0) MessageBox.Show("No Records to Delete");
            else
            {
                if (!BLL.UserAccount.AllowDelete(FormName)) MessageBox.Show(string.Format(Message.PL.DenyDelete, FormName));
                else if (MessageBox.Show("Do you want to Delete this record?", "DELETE", MessageBoxButton.YesNo) != MessageBoxResult.No)
                {
                    if (data.Delete() == true)
                    {
                        MessageBox.Show("Deleted");
                        this.Close();
                    }
                }
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            Clear();
        }

        private void dgvDetail_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var d = dgvDetail.SelectedItem as BLL.UserType;
            if (d != null)
            {
                data.Find(d.Id);
            }
        }



        #endregion

        #region Methods

        private void Clear()
        {
            data.Clear();
            ckbAllViewForm.IsChecked = false;
            ckbAllAllowInsert.IsChecked = false;
            ckbAllAllowUpdate.IsChecked = false;
            ckbAllAllowDelete.IsChecked = false;
        }
        private void onClientEvents()
        {
            BLL.ABClientHub.FMCGHub.On<BLL.UserType>("userType_Save", (rv) => {

                this.Dispatcher.Invoke(() =>
                {
                    rv.Save(true);
                });

            });

            BLL.ABClientHub.FMCGHub.On("userType_Delete", (Action<int>)((pk) => {
                this.Dispatcher.Invoke((Action)(() => {
                    BLL.UserType d = new BLL.UserType();
                    d.Find((int)pk);
                    d.Delete((bool)true);
                }));

            }));
        }

        #endregion

        private void ckbAllViewForm_Checked(object sender, RoutedEventArgs e)
        {
            foreach(var d in data.UserTypeDetails) d.IsViewForm = true;
        }

        private void ckbAllViewForm_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var d in data.UserTypeDetails) d.IsViewForm = false;
        }

        private void ckbAllAllowInsert_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var d in data.UserTypeDetails.Where(x=> x.IsNotReport).ToList()) d.AllowInsert = true;
        }

        private void ckbAllAllowInsert_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var d in data.UserTypeDetails.Where(x => x.IsNotReport).ToList()) d.AllowInsert = false;
        }

        private void ckbAllAllowUpdate_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var d in data.UserTypeDetails.Where(x => x.IsNotReport).ToList()) d.AllowUpdate = true;
        }

        private void ckbAllAllowUpdate_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var d in data.UserTypeDetails.Where(x => x.IsNotReport).ToList()) d.AllowUpdate =false;
        }

        private void ckbAllAllowDelete_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var d in data.UserTypeDetails.Where(x => x.IsNotReport).ToList()) d.AllowDelete = true;
        }

        private void ckbAllAllowDelete_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var d in data.UserTypeDetails.Where(x => x.IsNotReport).ToList()) d.AllowDelete = false;
        }

        private void ckbViewForm_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                var d = ((CheckBox) sender).Tag as BLL.UserTypeDetail;
                if (d != null) d.IsViewForm = true;
            }catch(Exception ex) { }
        }

        private void ckbViewForm_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                var d = ((CheckBox)sender).Tag as BLL.UserTypeDetail;
                if (d != null) d.IsViewForm = false;
            }
            catch (Exception ex) { }
        }

        private void ckbAllowInsert_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                var d = ((CheckBox)sender).Tag as BLL.UserTypeDetail;
                if (d != null) d.AllowInsert = true;
            }
            catch (Exception ex) { }
        }

        private void ckbAllowInsert_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                var d = ((CheckBox)sender).Tag as BLL.UserTypeDetail;
                if (d != null) d.AllowInsert = false;
            }
            catch (Exception ex) { }
        }

        private void ckbAllowUpdate_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                var d = ((CheckBox)sender).Tag as BLL.UserTypeDetail;
                if (d != null) d.AllowUpdate= true;
            }
            catch (Exception ex) { }
        }

        private void ckbAllowUpdate_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                var d = ((CheckBox)sender).Tag as BLL.UserTypeDetail;
                if (d != null) d.AllowUpdate = false;
            }
            catch (Exception ex) { }
        }

        private void ckbAllowDelete_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                var d = ((CheckBox)sender).Tag as BLL.UserTypeDetail;
                if (d != null) d.AllowDelete= true;
            }
            catch (Exception ex) { }
        }

        private void ckbAllowDelete_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                var d = ((CheckBox)sender).Tag as BLL.UserTypeDetail;
                if (d != null) d.AllowDelete = false;
            }
            catch (Exception ex) { }
        }
    }
}
