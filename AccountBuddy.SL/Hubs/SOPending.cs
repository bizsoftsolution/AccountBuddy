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

            var lstLedger = DB.Ledgers.Where(x => x.AccountGroup.CompanyId == Caller.CompanyId).ToList();

            foreach (var l in lstLedger)
            {
                if (l.SalesOrders.Where(x => x.SODate >= dtFrom && x.SODate <= dtTo).Count() != 0)
                {
                    var po = l.SalesOrders.FirstOrDefault();
                    tb = new BLL.SOPending();
                    tb.Ledger = LedgerDAL_BLL(l);

                    tb.EntryNo = po.RefNo;
                    tb.Amount = po.TotalAmount;
                    tb.SODate = po.SODate;
                    tb.Status = po.SalesOrderDetails.FirstOrDefault().SalesDetails.Count() > 0 ? "Purchased" : "Pending";
                    lstSOPending.Add(tb);
                }




            }



            return lstSOPending;
        }

    }
}