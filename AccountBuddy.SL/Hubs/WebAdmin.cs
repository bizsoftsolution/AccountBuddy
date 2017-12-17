using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        static int WebAdminCompanyId = -123;
       
        public bool WebLogin(string LoginId,string Password)
        {
            var d = DB.WebAdmins.Where(x => x.LoginId == LoginId && x.Password == Password).FirstOrDefault();
            var app = DB.AppConnections.Where(x => x.ConnectionId == Caller.ConnectionId).ToList().LastOrDefault();
            if (d == null)
            {
                if (app != null)
                {
                    app.AppConnectionLoginFaileds.Add(new DAL.AppConnectionLoginFailed() {
                        CompanyName = "WebAdmin",
                        LoginId = LoginId,
                        Password = Password,
                        FailedAt = DateTime.Now                
                    });
                    DB.SaveChanges();
                }
                 
                return false;
            }
            else
            {
                Caller.CompanyId = WebAdminCompanyId;
                Caller.CompanyType = "WebAdmim";
                Caller.UserId = d.Id;

                if (app != null)
                {
                    app.AppConnectionLoginWebAdmins.Add(new DAL.AppConnectionLoginWebAdmin() {
                        WebAdminId=d.Id,
                        LoginAt =DateTime.Now
                    });
                    DB.SaveChanges();
                }

                return true;
            }
        }
    }
}