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

            var lstLedger = DB.Ledgers.Where(x => x.AccountGroup.CompanyId == Caller.CompanyId).ToList();
            decimal TotAmt=0;

            foreach (var l in lstLedger)
            {
                tb = new BLL.Payable();
                tb.Ledger = LedgerDAL_BLL(l);

                decimal OP=0, Dr = 0;
               // LedgerBalance(l, dt, ref OPDr, ref OPCr, ref Dr, ref Cr);

                tb.Amount =l.OPCr!=0||l.OPCr!=null? 0:l.OPCr.Value;
                if(l.Purchases != null) tb.Amount += l.Purchases.Where(x => x.TransactionType.Type == "Credit" && x.PurchaseDate <= dt).Sum(x => x.TotalAmount)  ;
                if (l.SalesReturns != null) tb.Amount += l.SalesReturns.Where(x => x.TransactionType.Type == "Credit" && x.SRDate <= dt).Sum(x => x.TotalAmount);
               
                if(tb.Amount!=0)
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