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
using Microsoft.AspNet.SignalR.Client;

namespace AccountBuddy.PL.frm.Master
{
    /// <summary>
    /// Interaction logic for frmCustomFormat.xaml
    /// </summary>
    public partial class frmCustomFormat : UserControl
    {
        BLL.CustomFormat data = new BLL.CustomFormat();
        public string FormName = "CustomFormat";
        int number1 = 123456789;
        int number2 = 10;
        string words = "";
        public frmCustomFormat()
        {
            InitializeComponent();
            this.DataContext = data;

            onClientEvents();
        }

        private void onClientEvents()
        {
            BLL.FMCGHubClient.FMCGHub.On<BLL.CustomFormat>("CustomFormat_Save", (cs) =>
            {

                this.Dispatcher.Invoke(() =>
                {
                    cs.Save(true);
                });

            });


        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            BLL.CustomFormat.Init();

            data.Find(BLL.UserAccount.User.UserType.CompanyId);
            txtSampleCurrencySymbol.Text = string.Format("{0} {1}", txtCurrencySymbol.Text, "123456789.10");
            setSample();
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
            if (!BLL.CustomFormat.UserPermission.AllowDelete)
                MessageBox.Show(string.Format(Message.PL.DenyDelete, lblHead.Text));
            else if (MessageBox.Show(Message.PL.Delete_confirmation, "", MessageBoxButton.YesNo) != MessageBoxResult.No)

                if (data.Delete() == true)
                {
                    MessageBox.Show(Message.PL.Delete_Alert);

                }
        }



        #endregion

        private void txtCurrencySymbol_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtSampleCurrencySymbol.Text = string.Format("{0} {1}", txtCurrencySymbol.Text, "123456789.10");
        }

        private void txtCurrencyName1_TextChanged(object sender, TextChangedEventArgs e)
        {
            setSample();
        }

        private void setSample()
        {
            if (data.IsPrefix == true)
            {
                words = string.Format("{0}{1} {2} ", txtCurrencyName1.Text, number1 > 1 ? "S" : "", "One Hundred And Twenty Three Million Four Hundred And Fifty Six Thousand Seven Hundred And Eighty Nine");

            }
            else
            {
                words = string.Format("{0} {1}{2} ", "One Hundred And Twenty Three Million Four Hundred And Fifty Six Thousand Seven Hundred And Eighty Nine", txtCurrencyName1.Text, number1 > 1 ? "S" : "");

            }
            if (number2 > 0) words = string.Format("{0} AND {1} {2} {3}", words, "Ten", txtCurrencyName2.Text, number2 > 1 ? "s" : "");
            words = string.Format("{0} ONLY", words).ToUpper();

            txtSampleCurrencyName1.Text = words;

        }

        private void txtCurrencyName2_TextChanged(object sender, TextChangedEventArgs e)
        {
            setSample();
        }
    }
}
