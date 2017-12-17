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
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void mediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            mediaElement.Position = TimeSpan.FromMilliseconds(1);
        }
    }
}
