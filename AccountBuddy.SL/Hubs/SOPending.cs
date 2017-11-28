using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        public List<BLL.SOPending> SOPending_List(DateTime dtFrom, DateTime dtTo)
        {
            List<BLL.SOPending> lstSOPending = new List<BLL.SOPending>();
            BLL.SOPending tb = new BLL.SOPending();

            var lstLedger = Caller.DB.Ledgers.Where(x =>x.AccountGroup.GroupName==BLL.DataKeyValue.SundryDebtors_Key || x.AccountGroup.GroupName == BLL.DataKeyValue.BranchDivisions_Key && x.AccountGroup.CompanyId == Caller.CompanyId).ToList();

            foreach (var l in lstLedger)
            {
                foreach (var pd in l.SalesOrders.Where(x => x.SODate >= dtFrom && x.SODate <= dtTo && x.Ledger.AccountGroup.CompanyId==Caller.CompanyId).ToList())
                {

                    var po = l.PurchaseOrders.FirstOrDefault();
                    tb = new BLL.SOPending();
                    tb.Ledger = LedgerDAL_BLL(l);

                    tb.EntryNo = pd.RefNo;
                    tb.Amount = pd.TotalAmount;
                    tb.SODate = pd.SODate;
                    tb.Status = pd.SalesOrderDetails.FirstOrDefault().SalesDetails.Count() > 0 ? "Sold" : "Pending";
                    lstSOPending.Add(tb);
                }



            }



            return lstSOPending;
        }

    }
}