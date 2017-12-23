using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Transports;
using AccountBuddy.BLL;
using AccountBuddy.Common;

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

                AppLib.WriteLog("Application Startup");

                AppLib.SLPath= ConfigurationManager.AppSettings["SLPath"];
                AppLib.SLTransport = ConfigurationManager.AppSettings["SLTransport"];
                AppLib.AppIdKey = ConfigurationManager.AppSettings["DSAppKey"];
                AppLib.WriteLogState = ConfigurationManager.AppSettings["WriteLogState"];


                if (string.IsNullOrWhiteSpace(AppLib.SLPath))
                {
                    string str = "SLPath is Empty on Config";
                    AppLib.WriteLog(str);
                    MessageBox.Show(str);
                }
                else if (string.IsNullOrWhiteSpace(AppLib.SLTransport))
                {
                    string str = "SLTransport is Empty on Config";
                    AppLib.WriteLog(str);
                    MessageBox.Show(str);
                }
                else if (string.IsNullOrWhiteSpace(AppLib.AppIdKey))
                {
                    string str = "AppKey is Empty on Config";
                    AppLib.WriteLog(str);
                    MessageBox.Show(str);
                }
                else
                {
                    AppLib.WriteLog(string.Format("SLPath: {0},SLTransport: {1}, AppKey: {2}", AppLib.SLPath,AppLib.SLTransport, AppLib.AppIdKey));

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
                        frmInit.Hide();
                    }
                    else
                    {
                                                
                        AppLib.AppIdValue = Environment.GetEnvironmentVariable(AppLib.AppIdKey, EnvironmentVariableTarget.Machine);
                        if (AppLib.AppIdValue == null)
                        {
                            try
                            {
                                Environment.SetEnvironmentVariable(AppLib.AppIdKey, "GetNewAppId", EnvironmentVariableTarget.Machine);
                                AppLib.AppIdValue = Environment.GetEnvironmentVariable(AppLib.AppIdKey, EnvironmentVariableTarget.Machine);
                            }
                            catch (Exception ex)
                            {
                                AppLib.WriteLog(ex);
                            }
                        }

                        if (AppLib.AppIdValue == null)
                        {
                            MessageBox.Show("Please Run as Administrator when First Time is open");
                            Application.Current.Shutdown();
                        }
                        else
                        {
                            if (AppLib.AppIdValue == "GetNewAppId")
                            {
                                AppLib.AppIdValue = FMCGHubClient.HubCaller.Invoke<string>("GetNewAppId").Result;
                                Environment.SetEnvironmentVariable(AppLib.AppIdKey, AppLib.AppIdValue, EnvironmentVariableTarget.Machine);
                            }


                            bool r = FMCGHubClient.HubCaller.Invoke<bool>("SetReceiverConnectionIdToCaller", HubConReceiver.ConnectionId).Result;
                            if (!r)
                            {
                                string str = "SetReceiverConnectionIdToCaller_Failed";
                                MessageBox.Show(str);
                                AppLib.WriteLog(str);
                                Application.Current.Shutdown();
                            }
                            else
                            {
                                AppLib.IsAppApproved = FMCGHubClient.HubCaller.Invoke<bool>("LoginHubCaller", AppLib.AppIdValue, Environment.MachineName, Environment.UserName, Environment.UserDomainName).Result;

                                if (AppLib.IsAppApproved)
                                {
                                    frmInit.Hide();
                                    frmLogin.Show();
                                    frmLogin.ClearForm();
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
                
            }
           catch (Exception ex)
            {
                AppLib.WriteLog(ex.Message);
            }

        }

        #region Method

        #region HubConCallerEvents
        private static void HubConCaller_StateChanged(StateChange obj)
        {
            AppLib.WriteLog(string.Format("HubConCaller_StateChanged=>NewState:{0}, OldState:{1}", obj.NewState, obj.OldState));
        }

        private static void HubConCaller_Reconnecting()
        {
            AppLib.WriteLog("HubConCaller_Reconnecting");
        }

        private static void HubConCaller_Reconnected()
        {
            AppLib.WriteLog("HubConCaller_Reconnected");
        }

        private static void HubConCaller_Received(string obj)
        {
            AppLib.WriteLog(string.Format("HubConCaller_Received=>{0}", obj));
        }

        private static void HubConCaller_Error(Exception obj)
        {
            AppLib.WriteLog(string.Format("HubConCaller_Error=>Message:{0}, StackTrace:{1}", obj.Message, obj.StackTrace));
        }

        private static void HubConCaller_ConnectionSlow()
        {
            AppLib.WriteLog("HubConCaller_ConnectionSlow");
        }

        private static void HubConCaller_Closed()
        {
            AppLib.WriteLog("HubConCaller_Closed");
        }
        #endregion

        public static bool HubConCallerConnect()
        {
            try
            {
                
                HubConCaller = new HubConnection(AppLib.SLPath);
                AppLib.WriteLog("HubConCallerConnect_HubConnectionInit");

                FMCGHubClient.HubCaller = HubConCaller.CreateHubProxy("ABServerHub");
                AppLib.WriteLog("HubConCallerConnect_CreateHubProxy");

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

                if (AppLib.SLTransport == "LongPollingTransport")
                {
                    tp = new LongPollingTransport();
                }
                else if (AppLib.SLTransport == "WebSocketTransport")
                {
                    tp = new WebSocketTransport();
                }
                else if (AppLib.SLTransport == "ServerSentEventsTransport")
                {
                    tp = new ServerSentEventsTransport();
                }
                else
                {
                    tp = new LongPollingTransport();
                }
                #endregion

                HubConCaller.Start(tp).Wait();                
                AppLib.WriteLog("HubConCallerConnect_Started");
                return HubConCaller.State == Microsoft.AspNet.SignalR.Client.ConnectionState.Connected;
            }
            catch (Exception ex)
            {
                AppLib.WriteLog(ex);
                return false;
            }
        }

        public static void HubConCallerDisconnect()
        {
            HubConCaller.Stop();
            AppLib.WriteLog("HubConCallerDisconnect_Stoped");
        }

        #region HubConReceiverEvents
        private static void HubConReceiver_StateChanged(StateChange obj)
        {
            AppLib.WriteLog(string.Format("HubConReceiver_StateChanged=>NewState:{0}, OldState:{1}", obj.NewState, obj.OldState));
        }

        private static void HubConReceiver_Reconnecting()
        {
            AppLib.WriteLog("HubConReceiver_Reconnecting");
        }

        private static void HubConReceiver_Reconnected()
        {
            AppLib.WriteLog("HubConReceiver_Reconnected");
        }

        private static void HubConReceiver_Received(string obj)
        {
            AppLib.WriteLog(string.Format("HubConReceiver_Received=>{0}", obj));
        }

        private static void HubConReceiver_Error(Exception obj)
        {
            AppLib.WriteLog(string.Format("HubConReceiver_Error=>Message:{0}, StackTrace:{1}", obj.Message, obj.StackTrace));
        }

        private static void HubConReceiver_ConnectionSlow()
        {
            AppLib.WriteLog("HubConReceiver_ConnectionSlow");
        }

        private static void HubConReceiver_Closed()
        {
            AppLib.WriteLog("HubConReceiver_Closed");
        }
        #endregion

        public static bool HubConReceiverConnect()
        {
            try
            {

                HubConReceiver = new HubConnection(AppLib.SLPath);
                AppLib.WriteLog("HubConReceiverConnect_HubConnectionInit");

                HubReceiver = HubConReceiver.CreateHubProxy("ABServerHub");
                AppLib.WriteLog("HubConReceiverConnect_CreateHubProxy");

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

                if (AppLib.SLTransport == "LongPollingTransport")
                {
                    tp = new LongPollingTransport();
                }
                else if (AppLib.SLTransport == "WebSocketTransport")
                {
                    tp = new WebSocketTransport();
                }
                else if (AppLib.SLTransport == "ServerSentEventsTransport")
                {
                    tp = new ServerSentEventsTransport();
                }
                else
                {
                    tp = new LongPollingTransport();
                }
                #endregion

                HubConReceiver.Start(tp).Wait();

                AppLib.WriteLog("HubConReceiverConnect_Started");
                return HubConReceiver.State == Microsoft.AspNet.SignalR.Client.ConnectionState.Connected;
            }
            catch (Exception ex)
            {
                AppLib.WriteLog(ex);
                return false;
            }
        }

        public static void HubConReceiverDisconnect()
        {
            HubConReceiver.Stop();
            AppLib.WriteLog("HubConReceiverDisconnect_Stoped");
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
                        AppLib.IsAppApproved = IsApproved;

                        if (IsApproved)
                        {
                            frmInit.Hide();
                            if (UserAccount.User.Id == 0)
                            {
                                App.frmLogin.Show();
                            }
                            else
                            {
                                App.frmHome.Show();                            
                            }
                        }
                        else
                        {
                            frmInit.Show();
                            if (UserAccount.User.Id == 0)
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

            HubReceiver.On<CompanyDetail>("CompanyDetail_Save", (cs) =>
            {

                this.Dispatcher.Invoke(() =>
                {
                    cs.Save(true);
                    var f = frmHome.GetForm(Forms.frmCompanySetting);
                    if (f != null)
                    {
                        var frm = f.Content as frm.Master.frmCompanySetting;
                        frm.Grid_Refresh();
                    }

                });

            });

            #region Account Group
            HubReceiver.On<AccountGroup>("AccountGroup_Save", (Account) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    Account.Save(true);
                });
            });
            HubReceiver.On("AccountGroup_Delete", (Action<int>)((pk) =>
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    AccountGroup agp = new AccountGroup();
                    agp.Find((int)pk);
                    agp.Delete((bool)true);
                }));

            }));
            #endregion

            #region Bank
            HubReceiver.On<Bank>("Bank_Save", (Cus) => {

                this.Dispatcher.Invoke(() =>
                {
                    Cus.Save(true);
                });

            });
          
            HubReceiver.On("Bank_Delete", (Action<int>)((pk) => {
                this.Dispatcher.Invoke((Action)(() => {
                    Bank led = new Bank();
                    led.Find((int)pk);
                    led.Delete((bool)true);
                }));

            }));
            #endregion

            #region Customer
            HubReceiver.On<Customer>("Customer_Save", (Cus) => {

                this.Dispatcher.Invoke(() =>
                {
                    Cus.Save(true);
                });

            });
            HubReceiver.On("Customer_Delete", (Action<int>)((pk) => {
                this.Dispatcher.Invoke((Action)(() => {
                    Customer led = new Customer();
                    led.Find((int)pk);
                    led.Delete((bool)true);
                }));

            }));
            #endregion

            #region Custom Setting
            HubReceiver.On<CustomFormat>("CustomFormat_Save", (cs) =>
            {

                this.Dispatcher.Invoke(() =>
                {
                    cs.Save(true);
                });

            });

            #endregion

            HubReceiver.On<UserAccount>("UserAccount_Save", (ua) =>
            {

                this.Dispatcher.Invoke(() =>
                {
                    UserAccount u = new UserAccount();
                    ua.ToMap(u);
                    UserAccount.toList.Add(u);
                });

            });
            
            #region Department
            HubReceiver.On<Department>("Department_Save", (dept) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    dept.Save(true);
                });

            });

            HubReceiver.On("Department_Delete", (Action<int>)((pk) =>
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    Department agp = new Department();
                    agp.Find((int)pk);
                    agp.Delete((bool)true);
                }));

            }));

            HubReceiver.On<BLL.JobWorker>("JobWorker_Save", (Cus) =>
            {

                this.Dispatcher.Invoke(() =>
                {
                    Cus.Save(true);
                });

            });

            HubReceiver.On("JobWorker_Delete", (Action<int>)((pk) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    BLL.JobWorker led = new BLL.JobWorker();
                    led.Find((int)pk);
                    led.Delete((bool)true);
                });

            }));

            HubReceiver.On<BLL.Ledger>("Ledger_Save", (led) =>
            {

                this.Dispatcher.Invoke(() =>
                {
                    led.Save(true);
                });

            });

            HubReceiver.On("Ledger_Delete", (Action<int>)((pk) =>
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    BLL.Ledger led = new BLL.Ledger();
                    led.Find((int)pk);
                    led.Delete((bool)true);
                }));

            }));

            HubReceiver.On<BLL.Product>("Product_Save", (led) => {

                this.Dispatcher.Invoke(() =>
                {
                    led.Save(true);
                });

            });

            HubReceiver.On("Product_Delete", (Action<int>)((pk) => {
                this.Dispatcher.Invoke((Action)(() => {
                    BLL.Product led = new BLL.Product();
                    led.Find((int)pk);
                    led.Delete((bool)true);
                }));

            }));

            HubReceiver.On<BLL.Staff>("Staff_Save", (Cus) =>
            {

                this.Dispatcher.Invoke(() =>
                {
                    Cus.Save(true);
                });

            });

            HubReceiver.On("Staff_Delete", (Action<int>)((pk) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    BLL.Staff led = new BLL.Staff();
                    led.Find((int)pk);
                    led.Delete((bool)true);
                });

            }));
            HubReceiver.On<BLL.StockGroup>("StockGroup_Save", (sgp) =>
            {

                this.Dispatcher.Invoke(() =>
                {
                    sgp.Save(true);
                });

            });
            HubReceiver.On("StockGroup_Delete", (Action<int>)((pk) =>
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    BLL.StockGroup agp = new BLL.StockGroup();
                    agp.Find((int)pk);
                    agp.Delete((bool)true);
                }));

            }));
            HubReceiver.On<BLL.Supplier>("Supplier_Save", (Cus) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    Cus.Save(true);
                });

            });
            HubReceiver.On("Supplier_Delete", (Action<int>)((pk) =>
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    BLL.Supplier led = new BLL.Supplier();
                    led.Find((int)pk);
                    led.Delete((bool)true);
                }));

            }));
            HubReceiver.On<BLL.UOM>("UOM_Save", (uom) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    uom.Save(true);
                });
            });
            HubReceiver.On("UOM_Delete", (Action<int>)((pk) =>
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    BLL.UOM agp = new BLL.UOM();
                    agp.Find((int)pk);
                    agp.Delete((bool)true);
                }));

            }));
          
            #endregion

            #region Transaction
            HubReceiver.On<string>("StockSeperated_RefNoRefresh", (RefNo) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        var f = frmHome.GetForm(Forms.frmStockSeparated);
                        if (f != null)
                        {
                            var frm = f.Content as frm.Transaction.frmStockSeparated;
                            if (frm.data.Id == 0) frm.data.RefNo = RefNo;
                        }
                    }
                    catch(Exception ex) { AppLib.WriteLog(ex); }                    
                });
            });
            HubReceiver.On<String>("StockOut_RefNoRefresh", (RefNo) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        var f = frmHome.GetForm(Forms.frmStockOut);
                        if (f != null)
                        {
                            var frm = f.Content as frm.Transaction.frmStockOut;
                            if (frm.data.Id == 0) frm.data.RefNo = RefNo;
                        }
                    }
                    catch (Exception ex) { AppLib.WriteLog(ex); }
                });
            });
            HubReceiver.On<string>("StockInProcess_RefNoRefresh", (RefNo) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        var f = frmHome.GetForm(Forms.frmStockInProcess);
                        if (f != null)
                        {
                            var frm = f.Content as frm.Transaction.frmStockInProcess;
                            if (frm.data.Id == 0) frm.data.RefNo = RefNo;
                        }
                    }
                    catch (Exception ex) { AppLib.WriteLog(ex); }
                });
            });
            HubReceiver.On<String>("StockIn_RefNoRefresh", (RefNo) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        var f = frmHome.GetForm(Forms.frmStockInProcess);
                        if (f != null)
                        {
                            var frm = f.Content as frm.Transaction.frmStockInProcess;
                            if (frm.data.Id == 0) frm.data.RefNo = RefNo;
                        }
                    }
                    catch (Exception ex) { AppLib.WriteLog(ex); }
                });
            });
            HubReceiver.On<String>("SalesReturn_RefNoRefresh", (RefNo) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        var f = frmHome.GetForm(Forms.frmSalesReturn);
                        if (f != null)
                        {
                            var frm = f.Content as frm.Transaction.frmSalesReturn;
                            if (frm.data.Id == 0) frm.data.RefNo = RefNo;
                        }
                    }
                    catch (Exception ex) { AppLib.WriteLog(ex); }
                });
            });
            HubReceiver.On<String>("SalesOrder_RefNoRefresh", (RefNo) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        var f = frmHome.GetForm(Forms.frmSalesOrder);
                        if (f != null)
                        {
                            var frm = f.Content as frm.Transaction.frmSalesOrder;
                            if (frm.data.Id == 0) frm.data.RefNo = RefNo;
                        }
                    }
                    catch (Exception ex) { AppLib.WriteLog(ex); }
                });
            });

            HubReceiver.On<String>("Sales_RefNoRefresh", (RefNo) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        var f = frmHome.GetForm(Forms.frmSales);
                        if (f != null)
                        {
                            var frm = f.Content as frm.Transaction.frmSale;
                            if (frm.data.Id == 0) frm.data.RefNo = RefNo;
                        }
                    }
                    catch (Exception ex) { AppLib.WriteLog(ex); }
                });
            });

            HubReceiver.On<String>("Receipt_RefNoRefresh", (EntryNo) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        var f = frmHome.GetForm(Forms.frmReceipt);
                        if (f != null)
                        {
                            var frm = f.Content as frm.Transaction.frmReceipt;
                            if (frm.data.Id == 0) frm.data.RefNo = EntryNo;
                        }
                    }
                    catch (Exception ex) { AppLib.WriteLog(ex); }
                });
            });

            HubReceiver.On<String>("PurchaseReturn_RefNoRefresh", (RefNo) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        var f = frmHome.GetForm(Forms.frmPurchaseReturn);
                        if (f != null)
                        {
                            var frm = f.Content as frm.Transaction.frmPurchaseReturn;
                            if (frm.data.Id == 0) frm.data.RefNo = RefNo;
                        }
                    }
                    catch (Exception ex) { AppLib.WriteLog(ex); }
                });
            });
            HubReceiver.On<String>("PurchaseRequest_RefNoRefresh", (RefNo) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        var f = frmHome.GetForm(Forms.frmPurchaseRequest);
                        if (f != null)
                        {
                            var frm = f.Content as frm.Transaction.frmPurchaseRequest;
                            if (frm.data.Id == 0) frm.data.RefNo = RefNo;
                        }
                    }
                    catch (Exception ex) { AppLib.WriteLog(ex); }
                });
            });
            HubReceiver.On<String>("PurchaseOrder_RefNoRefresh", (RefNo) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        var f = frmHome.GetForm(Forms.frmPurchaseOrder);
                        if (f != null)
                        {
                            var frm = f.Content as frm.Transaction.frmPurchaseOrder;
                            if (frm.data.Id == 0) frm.data.RefNo = RefNo;
                        }
                    }
                    catch (Exception ex) { AppLib.WriteLog(ex); }
                });
            });
            HubReceiver.On<String>("Purchase_RefNoRefresh", (RefNo) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        var f = frmHome.GetForm(Forms.frmPurchase);
                        if (f != null)
                        {
                            var frm = f.Content as frm.Transaction.frmPurchase;
                            if (frm.data.Id == 0) frm.data.RefNo = RefNo;
                        }
                    }
                    catch (Exception ex) { AppLib.WriteLog(ex); }
                });
            });

            HubReceiver.On<String>("Payment_RefNoRefresh", (EntryNo) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        var f = frmHome.GetForm(Forms.frmPayment);
                        if (f != null)
                        {
                            var frm = f.Content as frm.Transaction.frmPayment;
                            if (frm.data.Id == 0) frm.data.RefNo = EntryNo;
                        }
                    }
                    catch (Exception ex) { AppLib.WriteLog(ex); }
                });
            });

            HubReceiver.On<String>("Journal_RefNoRefresh", (EntryNo) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        var f = frmHome.GetForm(Forms.frmJournal);
                        if (f != null)
                        {
                            var frm = f.Content as frm.Transaction.frmJournal;
                            if (frm.data.Id == 0) frm.data.EntryNo = EntryNo;
                        }
                    }
                    catch (Exception ex) { AppLib.WriteLog(ex); }
                });
            });
            HubReceiver.On<string>("JobOrderReceived _RefNoRefresh", (RefNo) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        var f = frmHome.GetForm(Forms.frmJobOrderReceived);
                        if (f != null)
                        {
                            var frm = f.Content as frm.Transaction.frmJobOrderReceived;
                            if (frm.data.Id == 0) frm.data.RefNo = RefNo;
                        }
                    }
                    catch (Exception ex) { AppLib.WriteLog(ex); }
                });
            });

            HubReceiver.On<string>("JobOrderIssue_RefNoRefresh", (RefNo) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        var f = frmHome.GetForm(Forms.frmJobOrderIssue);
                        if (f != null)
                        {
                            var frm = f.Content as frm.Transaction.frmJobOrderIssue;
                            if (frm.data.Id == 0) frm.data.RefNo = RefNo;
                        }
                    }
                    catch (Exception ex) { AppLib.WriteLog(ex); }
                });
            });
            #endregion

        }
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            if (HubConCaller.State==Microsoft.AspNet.SignalR.Client.ConnectionState.Connected) HubConCallerDisconnect();
            if (HubConReceiver.State == Microsoft.AspNet.SignalR.Client.ConnectionState.Connected) HubConReceiverDisconnect();
            AppLib.WriteLog("Application Exit");
        }
    }
}
