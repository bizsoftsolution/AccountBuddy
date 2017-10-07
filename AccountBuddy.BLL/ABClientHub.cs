using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Transports;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBuddy.BLL
{
    public static class FMCGHubClient
    {
        #region Field
        private static HubConnection _hubCon;
        private static IHubProxy _fmcgHub;
        #endregion

        #region Property
        public static HubConnection Hubcon
        {
            get
            {
                if (_hubCon == null) HubConnect();
                return _hubCon;
            }
            set
            {
                _hubCon = value;
            }
        }

        public static IHubProxy FMCGHub
        {
            get
            {
                if (_fmcgHub == null) HubConnect();
                if (Hubcon.State != ConnectionState.Connected) HubConnect();
                return _fmcgHub;
            }
            set
            {
                _fmcgHub = value;
            }
        }
        #endregion

        #region Method
        public static void HubConnect()
        {
           //string URLPath = "http://ubs3/fmcg/SignalR";
            string URLPath = "http://localhost:51068"; 
         //string URLPath = "http://localhost/fmcg";
        //string URLPath = "http://192.168.1.170/fmcg/SignalR"; 
            try
            {
                AccountBuddy.Common.AppLib.WriteLog(URLPath);
                AccountBuddy.Common.AppLib.WriteLog("Service Starting...");
                _hubCon = new HubConnection(URLPath);
                AccountBuddy.Common.AppLib.WriteLog("Service Started");
                _fmcgHub = _hubCon.CreateHubProxy("ABServerHub");
                AccountBuddy.Common.AppLib.WriteLog("Hub Created");
                _hubCon.Start(new LongPollingTransport()).Wait();
                AccountBuddy.Common.AppLib.WriteLog("Hub Started");

            }
            catch (Exception ex)
            {
                AccountBuddy.Common.AppLib.WriteLog("Could Not Start Service");

            }
            // _hubCon = new HubConnection("http://110.4.40.46/fmcgsl/SignalR");
            // _hubCon = new HubConnection("http://ubs3/fmcg/SignalR");

        }

        public static void HubDisconnect()
        {
            _hubCon.Stop();
        }
        #endregion
    }
}
