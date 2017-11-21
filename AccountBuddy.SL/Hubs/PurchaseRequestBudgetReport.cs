using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        public List<BLL.PurchaseRequestBudgetReport> PurchaseRequestBudgetReport_List(DateTime dtFrom, DateTime dtTo)
        {
            var lst = new List<BLL.PurchaseRequestBudgetReport>();

            return lst;
        }        
    }
}