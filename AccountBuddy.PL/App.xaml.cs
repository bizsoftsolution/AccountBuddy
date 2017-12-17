using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Transports;

namespace AccountBuddy.PL
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static frmInit _frmInit;
        private static frmLogin _frmLogin;
        private static frmHome _frmHome;
        
        public static frmInit frmInit
        {
            get
            {
                if (_frmInit == null) _frmInit = new frmInit();
                return _frmInit;
            }
            set
            {
                _frmInit = value;
            }
        }
        public static frmLogin frmLogin
        {
            get
            {
                if (_frmLogin == null) _frmLogin = new frmLogin();
                return _frmLogin;
            }
            set
            {
                _frmLogin = value;
            }
        }
        public static frmHome frmHome
        {
            get
            {
                if (_frmHome == null) _frmHome = new frmHome();
                return _frmHome;
            }
            set
            {
                _frmHome = value;
            }
        }


        private static HubConnection _hubCon;
        private static IHubProxy _hubReceiver;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                
                Common.AppLib.WriteLog("Application Startup");

                Common.AppLib.SLPath= ConfigurationManager.AppSettings["SLPath"];
                Common.AppLib.AppIdKey = ConfigurationManager.AppSettings["DSAppKey"];
                if(string.IsNullOrWhiteSpace(Common.AppLib.SLPath))
                {
                    string str = "SLPath is Empty on Config";
                    Common.AppLib.WriteLog(str);
                    MessageBox.Show(str);
                }
                else if (string.IsNullOrWhiteSpace(Common.AppLib.AppIdKey))
                {
                    string str = "AppKey is Empty on Config";
                    Common.AppLib.WriteLog(str);
                    MessageBox.Show(str);
                }
                else
                {
                    frmInit.Show();
                    
                    System.Windows.Forms.Application.DoEvents();
                    while (true)
                    {
                        if (HubConnect()) break;
                        else if (MessageBox.Show("Could not Connect to server. do you wish to try again?","Connecting...",MessageBoxButton.YesNo) != MessageBoxResult.Yes) break;
                    }
                    if (_hubCon.State != Microsoft.AspNet.SignalR.Client.ConnectionState.Connected)
                    {
                        frmInit.Close();
                    }
                    else
                    {

                        Common.AppLib.AppIdValue = Environment.GetEnvironmentVariable(Common.AppLib.AppIdKey, EnvironmentVariableTarget.Machine);
                        if (Common.AppLib.AppIdValue == null)
                        {
                            try
                            {
                                Common.AppLib.AppIdValue = BLL.FMCGHubClient.HubCaller.Invoke<string>("GetNewAppId").Result;
                                Environment.SetEnvironmentVariable(Common.AppLib.AppIdKey, Common.AppLib.AppIdValue, EnvironmentVariableTarget.Machine);
                            }
                            catch (Exception ex)
                            {
                                var str = ex.Message;
                            }

                        }

                        Common.AppLib.IsAppApproved = BLL.FMCGHubClient.HubCaller.Invoke<bool>("SystemLogin", Common.AppLib.AppIdValue, Environment.MachineName, Environment.UserName, Environment.UserDomainName).Result;
                        if (Common.AppLib.IsAppApproved)
                        {
                            frmInit.Hide();
                            frmLogin.Show();
                        }
                        else
                        {
                            System.Windows.Forms.Application.DoEvents();
                            frmInit.Title = "Waiting for approval";
                            frmLogin.Hide();    
                        }

                        ClientEvents();
                    }                    
                }
                
            }
           catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex.Message);
            }

        }

        #region Method
        private static void Hubcon_StateChanged(StateChange obj)
        {
            Common.AppLib.WriteLog(string.Format("Hubcon_StateChanged=>NewState:{0}, OldState:{1}", obj.NewState, obj.OldState));
        }

        private static void Hubcon_Reconnecting()
        {
            Common.AppLib.WriteLog("Hubcon_Reconnecting");
        }

        private static void Hubcon_Reconnected()
        {
            Common.AppLib.WriteLog("Hubcon_Reconnected");
        }

        private static void Hubcon_Received(string obj)
        {
            Common.AppLib.WriteLog(string.Format("Hubcon_Received=>{0}", obj));
        }

        private static void Hubcon_Error(Exception obj)
        {
            Common.AppLib.WriteLog(string.Format("Hubcon_Error=>Message:{0}, StackTrace:{1}", obj.Message, obj.StackTrace));
        }

        private static void Hubcon_ConnectionSlow()
        {
            Common.AppLib.WriteLog("Hubcon_ConnectionSlow");
        }

        private static void Hubcon_Closed()
        {
            Common.AppLib.WriteLog("HubCon_Closed");
        }

        public static bool HubConnect()
        {
            try
            {
                Common.AppLib.WriteLog(string.Format("SLPath:{0}", Common.AppLib.SLPath));
                _hubCon = new HubConnection(Common.AppLib.SLPath);
                Common.AppLib.WriteLog("SL Connected");
                BLL.FMCGHubClient.HubCaller = _hubCon.CreateHubProxy("ABServerHub");
                _hubReceiver = _hubCon.CreateHubProxy("ABServerHub");
                Common.AppLib.WriteLog("Hub Created");

                
                _hubCon.Closed += Hubcon_Closed;
                _hubCon.ConnectionSlow += Hubcon_ConnectionSlow;
                _hubCon.Error += Hubcon_Error;
                _hubCon.Received += Hubcon_Received;
                _hubCon.Reconnected += Hubcon_Reconnected;
                _hubCon.Reconnecting += Hubcon_Reconnecting;
                _hubCon.StateChanged += Hubcon_StateChanged;

                _hubCon.Start(new LongPollingTransport()).Wait();
                Common.AppLib.WriteLog("Hub Started");
                if(_hubCon.State==Microsoft.AspNet.SignalR.Client.ConnectionState.Connected)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog("Could Not Start Service", ex);
                return false;
            }
        }

        public static void HubDisconnect()
        {
            _hubCon.Stop();
        }
        #endregion

        void ClientEvents()
        {
            BLL.FMCGHubClient.HubCaller.On("AppApproved_Changed", (IsApproved) =>
            {
                try
                {

                    this.Dispatcher.Invoke(() =>
                    {                        
                        Common.AppLib.IsAppApproved = IsApproved;
                        if (IsApproved)
                        {
                            App.frmInit.Hide();
                            if (BLL.UserAccount.User.Id == 0)
                            {
                                App.frmLogin.Show();
                            }
                            else
                            {
                                App.frmHome.ShowDialog();
                                if (Common.AppLib.IsAppApproved)
                                {
                                    BLL.UserAccount.User = new BLL.UserAccount();
                                    frmLogin.ClearForm();
                                    frmLogin.Show();
                                }
                            }
                        }
                        else
                        {
                            App.frmInit.Show();
                            if (BLL.UserAccount.User.Id == 0)
                            {
                                App.frmLogin.Hide();
                            }
                            else
                            {
                                App.frmHome.Hide();
                            }
                        }
                    });

                }
                catch (Exception ex)
                {
                    var str = ex.Message;
                }


            });

            BLL.FMCGHubClient.HubCaller.On<BLL.CompanyDetail>("CompanyDetail_Save", (cs) =>
            {

                this.Dispatcher.Invoke(() =>
                {
                    cs.Save(true);

                });

            });
        }
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            if(_hubCon.State==Microsoft.AspNet.SignalR.Client.ConnectionState.Connected) HubDisconnect();
            Common.AppLib.WriteLog("Application Exit");
        }
    }
}
