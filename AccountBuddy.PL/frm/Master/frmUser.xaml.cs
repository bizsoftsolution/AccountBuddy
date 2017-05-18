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
        public frmUser()
        {
            InitializeComponent();

            this.DataContext = data;

            cmbUserType.ItemsSource = BLL.UserType.toList;
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
            if (data.Save() == true)
            {
                MessageBox.Show("Saved");
                if (BLL.UserAccount.User.Id == data.Id)
                {
                    BLL.UserAccount.User.UserName = data.UserName;
                    App.frmHome.Title = String.Format("{0} - {1}", BLL.UserAccount.User.UserName, BLL.UserAccount.Company.CompanyName);
                }
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
            frm.ShowDialog();
        }
    }
}
