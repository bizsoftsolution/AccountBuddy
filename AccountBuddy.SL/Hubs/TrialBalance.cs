using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        #region Trial_Balance

        public List<BLL.TrialBalance> TrialBalance_List()
        {
            List<BLL.TrialBalance> lstTrialBalance = new List<BLL.TrialBalance>();
            BLL.TrialBalance tb = new BLL.TrialBalance();

            var l1 = DB.Ledgers.Where(x => x.CompanyId == Caller.CompanyId).ToList();
             decimal TotDr = 0, TotCr = 0;

            #region Ledger
            foreach (var l in l1)
            {
                tb = new BLL.TrialBalance();

                tb.LedgerName = l.LedgerName;
                tb.GroupName = l.AccountGroup == null ? null : l.AccountGroup.GroupName;
                tb.DrAmt = l.Payments.Sum(x => x.Amount);
                tb.CrAmt = l.Receipts.Sum(x => x.Amount);
                tb.VoucherPayDate = l.Payments.Select(x => x.PaymentDate).FirstOrDefault();
                tb.VoucherRecDate = l.Receipts.Select(X => X.ReceiptDate).FirstOrDefault();


                if (tb.DrAmt > tb.CrAmt)
                {
                    tb.DrAmt = tb.DrAmt - tb.CrAmt;
                    tb.CrAmt = 0;
                }
                else
                {
                    tb.CrAmt = tb.CrAmt - tb.DrAmt;
                    tb.DrAmt = 0;
                }

                if (tb.DrAmt != 0 || tb.CrAmt != 0)
                {
                    lstTrialBalance.Add(tb);
                    TotDr += tb.DrAmt.Value;
                    TotCr += tb.CrAmt.Value;
                }
            }
            #endregion

         

            tb = new BLL.TrialBalance();
            tb.LedgerName = "Total ";
            tb.DrAmt = TotDr;
            tb.CrAmt = TotCr;
            tb.VoucherPayDate = DateTime.Today;
            tb.VoucherRecDate = DateTime.Today;

            lstTrialBalance.Add(tb);


            return lstTrialBalance;
        }

        #endregion


    }
}