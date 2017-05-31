using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AccountBuddy.Common;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        #region Profit_Loss

        public List<BLL.ProfitLoss> ProfitLoss_List(DateTime dtFrom, DateTime dtTo)
        {
            List<BLL.ProfitLoss> lstProfitLoss = new List<BLL.ProfitLoss>();


            BLL.ProfitLoss tb = new BLL.ProfitLoss();


            var lstLedgerIncome = DB.Ledgers.Where(x => x.AccountGroup.CompanyId == Caller.CompanyId && x.AccountGroup.GroupName == "Income").ToList();
            var lstLedgerLiability = DB.Ledgers.Where(x => x.AccountGroup.CompanyId == Caller.CompanyId && x.AccountGroup.GroupName == "Expenses").ToList();
            decimal  TotIn=0, TotEx=0;

            #region Income

            tb = new BLL.ProfitLoss();
            tb.Ledger = new BLL.Ledger();
            tb.Ledger.AccountName = "  Income";
            if (tb.Amt == 0)
            {
                tb.Amt = null;
            }
            lstProfitLoss.Add(tb);


            #region Ledger

            decimal OPDr, OPCr, PDr, PCr, RDr, RCr, JDr, JCr;

            foreach (var l in lstLedgerIncome)
            {
                tb = new BLL.ProfitLoss();
                tb.Ledger = new BLL.Ledger();

                //l.toCopy<BLL.Ledger>(tb.Ledger);
                tb.Ledger = LedgerDAL_BLL(l);

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
                    tb.Amt= tb.DrAmt - tb.CrAmt; 
                }
                else
                {
                    tb.CrAmt = tb.CrAmt - tb.DrAmt;
                    tb.DrAmt = 0;
                    tb.Amt = tb.CrAmt - tb.DrAmt;
                }

                if (tb.DrAmt != 0 || tb.CrAmt != 0)
                {
                    lstProfitLoss.Add(tb);
                    TotIn += tb.Amt.Value;
                   
                }
            }
            #endregion

            tb = new BLL.ProfitLoss();
            tb.Ledger = new BLL.Ledger();
            tb.Ledger.AccountName = "  Total Income";
            tb.Amt = TotIn;
           
            lstProfitLoss.Add(tb);

            #endregion


            #region Expenses

            tb = new BLL.ProfitLoss();
            tb.Ledger = new BLL.Ledger();
            tb.Ledger.AccountName = "  Expenses";
            if(tb.Amt==0)
            {
                tb.Amt = null;
            }
            lstProfitLoss.Add(tb);

            #region Ledger

            decimal OPInDr, OPInCr, PInDr, PInCr, RInDr, RInCr, JInDr, JInCr;

            foreach (var l in lstLedgerLiability)
            {
                tb = new BLL.ProfitLoss();
                tb.Ledger = new BLL.Ledger();

                tb.Ledger= LedgerDAL_BLL(l);

                OPInDr = l.OPDr ?? 0;
                OPInCr = l.OPCr ?? 0;

                PInDr = l.PaymentDetails.Where(x => x.Payment.PaymentDate < dtFrom).Sum(x => x.Amount);
                PInCr = l.Payments.Where(x => x.PaymentDate < dtFrom).Sum(x => x.Amount);

                RInDr = l.Receipts.Where(x => x.ReceiptDate < dtFrom).Sum(x => x.Amount);
                RInCr = l.ReceiptDetails.Where(x => x.Receipt.ReceiptDate < dtFrom).Sum(x => x.Amount);

                JInDr = l.JournalDetails.Where(x => x.Journal.JournalDate < dtFrom).Sum(x => x.DrAmt);
                JInCr = l.JournalDetails.Where(x => x.Journal.JournalDate < dtFrom).Sum(x => x.CrAmt);

                tb.DrAmtOP = OPInDr + PInDr + RInDr + JInDr;
                tb.CrAmtOP = OPInCr + PInCr + RInCr + JInCr;


                PInDr = l.PaymentDetails.Where(x => x.Payment.PaymentDate >= dtFrom && x.Payment.PaymentDate <= dtTo).Sum(x => x.Amount);
                PInCr = l.Payments.Where(x => x.PaymentDate >= dtFrom && x.PaymentDate <= dtTo).Sum(x => x.Amount);

                RInDr = l.Receipts.Where(x => x.ReceiptDate >= dtFrom && x.ReceiptDate <= dtTo).Sum(x => x.Amount);
                RInCr = l.ReceiptDetails.Where(x => x.Receipt.ReceiptDate >= dtFrom && x.Receipt.ReceiptDate <= dtTo).Sum(x => x.Amount);

                JInDr = l.JournalDetails.Where(x => x.Journal.JournalDate >= dtFrom && x.Journal.JournalDate <= dtTo).Sum(x => x.DrAmt);
                JInCr = l.JournalDetails.Where(x => x.Journal.JournalDate >= dtFrom && x.Journal.JournalDate <= dtTo).Sum(x => x.CrAmt);



                tb.DrAmt = tb.DrAmtOP + PInDr + RInDr + JInDr;
                tb.CrAmt = tb.CrAmtOP + PInCr + RInCr + JInCr;


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
                    tb.Amt = tb.DrAmt - tb.CrAmt;
                   
                }
                else
                {
                    tb.Amt = tb.CrAmt - tb.DrAmt;
                    
                }

                if (tb.DrAmt != 0 || tb.CrAmt != 0)
                {
                    lstProfitLoss.Add(tb);
                    TotEx +=tb.Amt.Value;

                    
                }
            }
            #endregion


            tb = new BLL.ProfitLoss();
            tb.Ledger = new BLL.Ledger();
            tb.Ledger.AccountName = "   Total Expenses";
            tb.Amt = TotEx;
           
            lstProfitLoss.Add(tb);

            #endregion


            tb = new BLL.ProfitLoss();
            tb.Ledger = new BLL.Ledger();
            tb.Ledger.AccountName        = "Surplus/Deficit";
           if(TotIn>TotEx)
            {
                tb.Amt = TotIn - TotEx;
            }
           else

            {
                tb.Amt =  TotEx-TotIn;
            }

            lstProfitLoss.Add(tb);



            return lstProfitLoss;
        }

        #endregion
    }
}