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


        private static HubConnection HubConCaller;
        private static HubConnection HubConReceiver;
        public static IHubProxy HubReceiver;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                
                Common.AppLib.WriteLog("Application Startup");

                Common.AppLib.SLPath= ConfigurationManager.AppSettings["SLPath"];
                Common.AppLib.SLTransport = ConfigurationManager.AppSettings["SLTransport"];
                Common.AppLib.AppIdKey = ConfigurationManager.AppSettings["DSAppKey"];


                if(string.IsNullOrWhiteSpace(Common.AppLib.SLPath))
                {
                    string str = "SLPath is Empty on Config";
                    Common.AppLib.WriteLog(str);
                    MessageBox.Show(str);
                }
                else if (string.IsNullOrWhiteSpace(Common.AppLib.SLTransport))
                {
                    string str = "SLTransport is Empty on Config";
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
                    Common.AppLib.WriteLog(string.Format("SLPath: {0},SLTransport: {1}, AppKey: {2}", Common.AppLib.SLPath,Common.AppLib.SLTransport, Common.AppLib.AppIdKey));

                    frmInit.Show();                    
                    System.Windows.Forms.Application.DoEvents();
                    while (true)
                    {
                        if (HubConCallerConnect()) break;
                        else if (MessageBox.Show("Could not Connect to server. do you want to retry?","Connecting...",MessageBoxButton.YesNo) != MessageBoxResult.Yes) break;
                    }
                    HubConReceiverConnect();

                    if (HubConCaller.State != Microsoft.AspNet.SignalR.Client.ConnectionState.Connected || HubConReceiver.State != Microsoft.AspNet.SignalR.Client.ConnectionState.Connected)
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
                        bool r = BLL.FMCGHubClient.HubCaller.Invoke<bool>("SetReceiverConnectionIdToCaller", HubConReceiver.ConnectionId).Result;
                        if (!r)
                        {
                            string str = "SetReceiverConnectionIdToCaller_Failed";
                            MessageBox.Show(str);
                            Common.AppLib.WriteLog(str);
                            frmInit.Close();
                        }
                        else
                        {
                            Common.AppLib.IsAppApproved = BLL.FMCGHubClient.HubCaller.Invoke<bool>("LoginHubCaller", Common.AppLib.AppIdValue, Environment.MachineName, Environment.UserName, Environment.UserDomainName).Result;

                            if (Common.AppLib.IsAppApproved)
                            {
                                frmInit.Hide();
                                frmLogin.Show();
                            }
                            else
                            {                                
                                frmInit.Title = "Waiting for approval";
                                frmLogin.Hide();
                                System.Windows.Forms.Application.DoEvents();
                            }

                            ClientEvents();
                        }
                        
                    }                    
                }
                
            }
           catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex.Message);
            }

        }

        #region Method

        #region HubConCallerEvents
        private static void HubConCaller_StateChanged(StateChange obj)
        {
            Common.AppLib.WriteLog(string.Format("HubConCaller_StateChanged=>NewState:{0}, OldState:{1}", obj.NewState, obj.OldState));
        }

        private static void HubConCaller_Reconnecting()
        {
            Common.AppLib.WriteLog("HubConCaller_Reconnecting");
        }

        private static void HubConCaller_Reconnected()
        {
            Common.AppLib.WriteLog("HubConCaller_Reconnected");
        }

        private static void HubConCaller_Received(string obj)
        {
            Common.AppLib.WriteLog(string.Format("HubConCaller_Received=>{0}", obj));
        }

        private static void HubConCaller_Error(Exception obj)
        {
            Common.AppLib.WriteLog(string.Format("HubConCaller_Error=>Message:{0}, StackTrace:{1}", obj.Message, obj.StackTrace));
        }

        private static void HubConCaller_ConnectionSlow()
        {
            Common.AppLib.WriteLog("HubConCaller_ConnectionSlow");
        }

        private static void HubConCaller_Closed()
        {
            Common.AppLib.WriteLog("HubConCaller_Closed");
        }
        #endregion

        public static bool HubConCallerConnect()
        {
            try
            {
                
                HubConCaller = new HubConnection(Common.AppLib.SLPath);
                Common.AppLib.WriteLog("HubConCallerConnect_HubConnectionInit");

                BLL.FMCGHubClient.HubCaller = HubConCaller.CreateHubProxy("ABServerHub");
                Common.AppLib.WriteLog("HubConCallerConnect_CreateHubProxy");

                #region Events
                HubConCaller.Closed += HubConCaller_Closed;
                HubConCaller.ConnectionSlow += HubConCaller_ConnectionSlow;
                HubConCaller.Error += HubConCaller_Error;
                HubConCaller.Received += HubConCaller_Received;
                HubConCaller.Reconnected += HubConCaller_Reconnected;
                HubConCaller.Reconnecting += HubConCaller_Reconnecting;
                HubConCaller.StateChanged += HubConCaller_StateChanged;
                #endregion

                #region Transport

                IClientTransport tp;

                if (Common.AppLib.SLTransport == "LongPollingTransport")
                {
                    tp = new LongPollingTransport();
                }
                else if (Common.AppLib.SLTransport == "WebSocketTransport")
                {
                    tp = new WebSocketTransport();
                }
                else if (Common.AppLib.SLTransport == "ServerSentEventsTransport")
                {
                    tp = new ServerSentEventsTransport();
                }
                else
                {
                    tp = new LongPollingTransport();
                }
                #endregion

                HubConCaller.Start(tp).Wait();                
                Common.AppLib.WriteLog("HubConCallerConnect_Started");
                return HubConCaller.State == Microsoft.AspNet.SignalR.Client.ConnectionState.Connected;
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex);
                return false;
            }
        }

        public static void HubConCallerDisconnect()
        {
            HubConCaller.Stop();
            Common.AppLib.WriteLog("HubConCallerDisconnect_Stoped");
        }

        #region HubConReceiverEvents
        private static void HubConReceiver_StateChanged(StateChange obj)
        {
            Common.AppLib.WriteLog(string.Format("HubConReceiver_StateChanged=>NewState:{0}, OldState:{1}", obj.NewState, obj.OldState));
        }

        private static void HubConReceiver_Reconnecting()
        {
            Common.AppLib.WriteLog("HubConReceiver_Reconnecting");
        }

        private static void HubConReceiver_Reconnected()
        {
            Common.AppLib.WriteLog("HubConReceiver_Reconnected");
        }

        private static void HubConReceiver_Received(string obj)
        {
            Common.AppLib.WriteLog(string.Format("HubConReceiver_Received=>{0}", obj));
        }

        private static void HubConReceiver_Error(Exception obj)
        {
            Common.AppLib.WriteLog(string.Format("HubConReceiver_Error=>Message:{0}, StackTrace:{1}", obj.Message, obj.StackTrace));
        }

        private static void HubConReceiver_ConnectionSlow()
        {
            Common.AppLib.WriteLog("HubConReceiver_ConnectionSlow");
        }

        private static void HubConReceiver_Closed()
        {
            Common.AppLib.WriteLog("HubConReceiver_Closed");
        }
        #endregion

        public static bool HubConReceiverConnect()
        {
            try
            {

                HubConReceiver = new HubConnection(Common.AppLib.SLPath);
                Common.AppLib.WriteLog("HubConReceiverConnect_HubConnectionInit");

                HubReceiver = HubConReceiver.CreateHubProxy("ABServerHub");
                Common.AppLib.WriteLog("HubConReceiverConnect_CreateHubProxy");

                #region Events
                HubConReceiver.Closed += HubConReceiver_Closed;
                HubConReceiver.ConnectionSlow += HubConReceiver_ConnectionSlow;
                HubConReceiver.Error += HubConReceiver_Error;
                HubConReceiver.Received += HubConReceiver_Received;
                HubConReceiver.Reconnected += HubConReceiver_Reconnected;
                HubConReceiver.Reconnecting += HubConReceiver_Reconnecting;
                HubConReceiver.StateChanged += HubConReceiver_StateChanged;
                #endregion

                #region Transport

                IClientTransport tp;

                if (Common.AppLib.SLTransport == "LongPollingTransport")
                {
                    tp = new LongPollingTransport();
                }
                else if (Common.AppLib.SLTransport == "WebSocketTransport")
                {
                    tp = new WebSocketTransport();
                }
                else if (Common.AppLib.SLTransport == "ServerSentEventsTransport")
                {
                    tp = new ServerSentEventsTransport();
                }
                else
                {
                    tp = new LongPollingTransport();
                }
                #endregion

                HubConReceiver.Start(tp).Wait();

                Common.AppLib.WriteLog("HubConReceiverConnect_Started");
                return HubConReceiver.State == Microsoft.AspNet.SignalR.Client.ConnectionState.Connected;
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex);
                return false;
            }
        }

        public static void HubConReceiverDisconnect()
        {
            HubConReceiver.Stop();
            Common.AppLib.WriteLog("HubConReceiverDisconnect_Stoped");
        }
        #endregion

        void ClientEvents()
        {
            HubReceiver.On("AppApproved_Changed", (IsApproved) =>
            {
                try
                {

                    this.Dispatcher.Invoke(() =>
                    {
                        Common.AppLib.IsAppApproved = BLL.FMCGHubClient.HubCaller.Invoke<bool>("LoginHubCaller", Common.AppLib.AppIdValue, Environment.MachineName, Environment.UserName, Environment.UserDomainName).Result;
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
            if (HubConCaller.State==Microsoft.AspNet.SignalR.Client.ConnectionState.Connected) HubConCallerDisconnect();
            if (HubConReceiver.State == Microsoft.AspNet.SignalR.Client.ConnectionState.Connected) HubConReceiverDisconnect();
            Common.AppLib.WriteLog("Application Exit");
        }
    }
}
