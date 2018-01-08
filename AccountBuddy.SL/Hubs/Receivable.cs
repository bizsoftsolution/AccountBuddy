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

            var lstLedger = DB.Ledgers.Where(x => x.AccountGroup.CompanyId == Caller.CompanyId);
            decimal TotAmt = 0;

            foreach (var l in lstLedger)
            {
                tb = new BLL.Receivable();
                tb.Ledger = LedgerDAL_BLL(l);

                decimal OP = 0, Dr = 0;
                // LedgerBalance(l, dt, ref OPDr, ref OPCr, ref Dr, ref Cr);

                tb.Amount = l.OPDr != 0 || l.OPDr != null ? 0 : l.OPDr.Value;
                if (l.Sales.Count() != 0)
                {
                    var l2 = l.Sales.Where(x => x.TransactionType.Type == "Credit" && x.SalesDate <= dt).GroupBy(x => x.Ledger.LedgerName).ToList();
                    foreach (var l1 in l2)
                    {

                        tb.Amount += l1.Sum(x => x.TotalAmount);

                    }
                }
                if (l.PurchaseReturns.Count() != 0)
                {
                    var l2 = l.PurchaseReturns.Where(x => x.TransactionType.Type == "Credit" && x.PRDate <= dt).GroupBy(x => x.Ledger.LedgerName).ToList();
                    foreach (var l1 in l2)
                    {

                        tb.Amount += l1.Sum(x => x.TotalAmount);
                    }
                }
                if (l.StockOuts.Count() != 0)
                {
                    var l2 = l.StockOuts.Where(x => x.Date <= dt).GroupBy(x => x.Ledger.LedgerName);
                    foreach (var l1 in l2)
                    {

                        tb.Amount += l1.Sum(x => x.ItemAmount);
                    }
                }
                if (l.ReceiptDetails.Count() != 0)
                {
                    var l2 = l.ReceiptDetails.Where(x => x.Receipt.ReceiptDate <= dt).GroupBy(x => x.Ledger.LedgerName).ToList() ;
                    foreach (var l1 in l2)
                    {
                        if (tb.Amount > l1.Sum(x => x.Amount))
                        {
                            tb.Amount = Math.Abs(tb.Amount-l1.Sum(x => x.Amount));
                        }
                        else
                        {
                            tb.Amount = 0;
                        }
                        
                    }
                }

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