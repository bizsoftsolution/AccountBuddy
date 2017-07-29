﻿using System;
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
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                Common.AppLib.WriteLog("Application Startup");
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
