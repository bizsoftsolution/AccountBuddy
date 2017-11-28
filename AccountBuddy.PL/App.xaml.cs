using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace AccountBuddy.PL
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static frmHome frmHome;
       public static frmAccountBuddyHome frmAccountBuddyHome;
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                Common.AppLib.WriteLog("Application Startup");
                BLL.FMCGHubClient.URLPath = ConfigurationManager.AppSettings["SLPath"];
                Window frm = new frmLogin();
                frm.Show();
  
            }
           catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex.Message);
            }

        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {

        }
    }
}
