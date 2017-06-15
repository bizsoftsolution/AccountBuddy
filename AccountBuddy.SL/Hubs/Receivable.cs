using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {

        public List<BLL.Receivable> Receivable_List(DateTime dt)
        {
            List<BLL.Receivable> lstReceivable = new List<BLL.Receivable>();
            BLL.Receivable tb = new BLL.Receivable();

            var lstLedger = DB.Ledgers.Where(x => x.AccountGroup.CompanyId == Caller.CompanyId).ToList();
            decimal TotAmt = 0;

            foreach (var l in lstLedger)
            {
                tb = new BLL.Receivable();
                tb.Ledger = LedgerDAL_BLL(l);

                decimal OP = 0, Dr = 0;
                // LedgerBalance(l, dt, ref OPDr, ref OPCr, ref Dr, ref Cr);

                tb.Amount = l.OPCr != 0 || l.OPCr != null ? 0 : l.OPCr.Value;

                if (l.Sales != null) tb.Amount += l.Sales.Where(x => x.TransactionType.Type == "Credit" && x.SalesDate <= dt).Sum(x => x.TotalAmount);
                if (l.PurchaseReturns != null) tb.Amount += l.PurchaseReturns.Where(x => x.TransactionType.Type == "Credit" && x.PRDate <= dt).Sum(x => x.TotalAmount);


                if (tb.Amount != 0)
                {
                    lstReceivable.Add(tb);
                    TotAmt += tb.Amount;
                }
            }

            tb = new BLL.Receivable();
            tb.Ledger = new BLL.Ledger();
            tb.Ledger.AccountName = "Total";
            tb.Amount = TotAmt;
            lstReceivable.Add(tb);

            return lstReceivable;
        }
    }
}