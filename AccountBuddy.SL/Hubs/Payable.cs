using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        public List<BLL.Payable> Payable_List(DateTime dt)
        {
            List<BLL.Payable> lstPayable = new List<BLL.Payable>();
            BLL.Payable tb = new BLL.Payable();

            var lstLedger = Caller.DB.Ledgers.Where(x => x.AccountGroup.CompanyId == Caller.CompanyId);
            decimal TotAmt = 0;

            foreach (var l in lstLedger)
            {
                tb = new BLL.Payable();
                tb.Ledger = LedgerDAL_BLL(l);

                decimal OP = 0, Dr = 0;
                // LedgerBalance(l, dt, ref OPDr, ref OPCr, ref Dr, ref Cr);

                tb.Amount = l.OPDr != 0 || l.OPDr != null ? 0 : l.OPDr.Value;
                if (l.Purchases.Count() != 0)
                {
                    var l2 = l.Purchases.Where(x => x.PurchaseDate <= dt &&( x.TransactionType.Type == "Credit" || x.TransactionType.Type== "PNT(Payment Next Trip)")).GroupBy(x => x.Ledger.LedgerName);
                    foreach (var l1 in l2)
                    {

                        tb.Amount += l1.Sum(x => x.TotalAmount);

                    }
                }
                if (l.SalesReturns.Count() != 0)
                {
                    var l2 = l.SalesReturns.Where(x => (x.TransactionType.Type == "Credit" || x.TransactionType.Type == "PNT(Payment Next Trip)") && x.SRDate <= dt).GroupBy(x => x.Ledger.LedgerName);
                    foreach (var l1 in l2)
                    {

                        tb.Amount += l1.Sum(x => x.TotalAmount);
                    }
                }
                if (l.StockIns.Count() != 0)
                {
                    var l2 = l.StockIns.Where(x =>  x.Date <= dt).GroupBy(x => x.Ledger.LedgerName);
                    foreach (var l1 in l2)
                    {

                        tb.Amount += l1.Sum(x => x.ItemAmount);
                    }
                }
                if (l.PaymentDetails.Count() != 0)
                {
                    var l2 = l.PaymentDetails.Where(x => x.Payment.PaymentDate <= dt).GroupBy(x => x.Ledger.LedgerName);
                    foreach (var l1 in l2)
                    {
                        if (tb.Amount > l1.Sum(x => x.Amount))
                        {
                            tb.Amount = Math.Abs(tb.Amount - l1.Sum(x => x.Amount));
                        }
                        else
                        {
                            tb.Amount = 0;
                        }

                    }
                }

                if (tb.Amount != 0)
                {
                    lstPayable.Add(tb);
                    TotAmt += tb.Amount;
                }
            }

            tb = new BLL.Payable();
            tb.Ledger = new BLL.Ledger();
            tb.Ledger.AccountName = "Total";
            tb.Amount = TotAmt;
            lstPayable.Add(tb);

            return lstPayable;
        }
    }
}