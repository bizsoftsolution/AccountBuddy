using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AccountBuddy.Common;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        #region Trial_Balance

        public List<BLL.TrialBalance> TrialBalance_List(DateTime dtFrom, DateTime dtTo)
        {
            List<BLL.TrialBalance> lstTrialBalance = new List<BLL.TrialBalance>();


            BLL.TrialBalance tb = new BLL.TrialBalance();

            var lstLedger = DB.Ledgers.Where(x => x.AccountGroup.CompanyId == Caller.CompanyId).ToList();
            decimal TotDr = 0, TotCr = 0, TotOPCr = 0, TotOPDr = 0;



            #region Ledger

            decimal OPDr, OPCr, PDr, PCr, RDr, RCr, JDr, JCr;

            foreach (var l in lstLedger)
            {
                tb = new BLL.TrialBalance();
                tb.Ledger = new BLL.Ledger();
                
                 l.toCopy<BLL.Ledger>(tb.Ledger);
                
                OPDr = l.OPDr ?? 0;
                OPCr = l.OPCr ?? 0;

                PDr = l.PaymentDetails.Where(x => x.Payment.PaymentDate < dtFrom).Sum(x => x.Amount);
                PCr = l.Payments.Where(x => x.PaymentDate < dtFrom).Sum(x => x.Amount);

                RDr = l.Receipts.Where(x => x.ReceiptDate < dtFrom).Sum(x => x.Amount);
                RCr = l.ReceiptDetails.Where(x => x.Receipt.ReceiptDate < dtFrom).Sum(x => x.Amount);

                JDr = l.JournalDetails.Where(x => x.Journal.JournalDate < dtFrom).Sum(x => x.DrAmt);
                JCr = l.JournalDetails.Where(x => x.Journal.JournalDate < dtFrom).Sum(x => x.CrAmt);

                tb.DrAmtOP = OPDr + PDr + RDr + JDr;
                tb.CrAmtOP = OPCr + PCr + RCr + JCr;


                PDr = l.PaymentDetails.Where(x => x.Payment.PaymentDate >= dtFrom && x.Payment.PaymentDate <= dtTo).Sum(x => x.Amount);
                PCr = l.Payments.Where(x => x.PaymentDate >= dtFrom && x.PaymentDate <= dtTo).Sum(x => x.Amount);

                RDr = l.Receipts.Where(x => x.ReceiptDate >= dtFrom && x.ReceiptDate <= dtTo).Sum(x => x.Amount);
                RCr = l.ReceiptDetails.Where(x => x.Receipt.ReceiptDate >= dtFrom && x.Receipt.ReceiptDate <= dtTo).Sum(x => x.Amount);

                JDr = l.JournalDetails.Where(x => x.Journal.JournalDate >= dtFrom && x.Journal.JournalDate <= dtTo).Sum(x => x.DrAmt);
                JCr = l.JournalDetails.Where(x => x.Journal.JournalDate >= dtFrom && x.Journal.JournalDate <= dtTo).Sum(x => x.CrAmt);



                tb.DrAmt = tb.DrAmtOP + PDr + RDr + JDr;
                tb.CrAmt = tb.CrAmtOP + PCr + RCr + JCr;


                if (tb.DrAmtOP > tb.CrAmtOP)
                {
                    tb.DrAmtOP = tb.DrAmtOP - tb.CrAmtOP;
                    tb.CrAmtOP = 0;
                }
                else
                {
                    tb.CrAmtOP = tb.CrAmtOP - tb.DrAmtOP;
                    tb.DrAmtOP = 0;
                }

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
                    TotDr += tb.DrAmt;
                    TotCr += tb.CrAmt;
                    TotOPCr += tb.CrAmtOP;
                    TotOPDr += tb.DrAmtOP;

                }
            }
            #endregion

            tb = new BLL.TrialBalance();
            tb.Ledger = new BLL.Ledger();
            tb.Ledger.AccountName = "Total";
            tb.DrAmt = TotDr;
            tb.CrAmt = TotCr;
            tb.CrAmtOP = TotOPCr;
            tb.DrAmtOP = TotOPDr;
            lstTrialBalance.Add(tb);

            return lstTrialBalance;
        }

        #endregion


    }
}