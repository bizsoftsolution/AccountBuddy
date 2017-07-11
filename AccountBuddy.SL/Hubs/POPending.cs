using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        public List<BLL.POPending> POPending_List(DateTime dtFrom, DateTime dtTo)
        {
            List<BLL.POPending> lstPOPending = new List<BLL.POPending>();
            BLL.POPending tb = new BLL.POPending();

            var lstLedger = DB.Ledgers.Where(x => x.AccountGroup.GroupName == BLL.DataKeyValue.SundryCreditors_Key || x.AccountGroup.GroupName==BLL.DataKeyValue.BranchDivisions_Key && x.AccountGroup.CompanyId == Caller.CompanyId).ToList();

            foreach (var l in lstLedger)
            {
                foreach (var pd in l.PurchaseOrders.Where(x => x.PODate >= dtFrom && x.PODate <= dtTo && x.Ledger.AccountGroup.CompanyId == Caller.CompanyId).ToList())
                {
                    var po = l.PurchaseOrders.FirstOrDefault();
                    tb = new BLL.POPending();
                    tb.Ledger = LedgerDAL_BLL(l);

                    tb.EntryNo = pd.RefNo;
                    tb.Amount = pd.TotalAmount;
                    tb.PODate = pd.PODate;
                    tb.Status = pd.PurchaseOrderDetails.FirstOrDefault().PurchaseDetails.Count() > 0 ? "Purchased" : "Pending";
                    lstPOPending.Add(tb);
                }

            }
            
            return lstPOPending;
        }

    }
}