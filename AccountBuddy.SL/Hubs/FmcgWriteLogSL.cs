using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        public static void WriteLog(String str, int UserId,  int CompanyId, string ErDescription)
        {

            using (StreamWriter writer = new StreamWriter(Path.GetTempPath() + "FMCG_log_SL.txt", true))
            {
                writer.WriteLine(string.Format("{0:dd/MM/yyyy hh:mm:ss} => {1}:CompanyId={2},  UserId={3}, Error Description=>{4}", DateTime.Now, str, CompanyId,  UserId, ErDescription));
            }
        }
        public static void WriteErrorLog(String str,string FnName,  int UserId, int CompanyId, string ErDescription)
        {

            using (StreamWriter writer = new StreamWriter(Path.GetTempPath() + "FMCG_Errorlog_SL.txt", true))
            {
                writer.WriteLine(string.Format("{0:dd/MM/yyyy hh:mm:ss} => {1}:FunctionName={2}, CompanyId={3},  UserId={4}, Error Description=>{5}", DateTime.Now, str,FnName, CompanyId,  UserId, ErDescription));
            }
        }
    }
}