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


namespace AccountBuddy.PL
{
    /// <summary>
    /// Interaction logic for frmInit.xaml
    /// </summary>
    public partial class frmInit : MetroWindow
    {
        public frmInit()
        {
            InitializeComponent();
            Common.AppLib.WriteLog("frmInit_Init");
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            Common.AppLib.WriteLog("frmInit_Activated");
            mediaElement.Position = TimeSpan.FromMilliseconds(1);
            mediaElement.Play();
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Common.AppLib.WriteLog("frmInit_Loaded");
        }

        private void mediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            mediaElement.Position = TimeSpan.FromMilliseconds(1);
        }

        private void MetroWindow_Deactivated(object sender, EventArgs e)
        {
            Common.AppLib.WriteLog("frmInit_Deactivated");
            mediaElement.Stop();
        }
    }
}
