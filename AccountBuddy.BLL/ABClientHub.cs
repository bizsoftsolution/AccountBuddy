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
        
        private static IHubProxy _fmcgHub;
       
        #endregion

        #region Property
        
        public static IHubProxy FMCGHub
        {
            get
            {                             
                return _fmcgHub;
            }
            set
            {
                _fmcgHub = value;
            }
        }
        #endregion
        
    }
}
