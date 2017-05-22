using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AccountBuddy.Common;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        #region Balance_Sheet

        public List<BLL.BalanceSheet> Balancesheet_List(DateTime dtFrom, DateTime dtTo)
        {
            List<BLL.BalanceSheet> lstBalanceSheet = new List<BLL.BalanceSheet>();


            BLL.BalanceSheet tb = new BLL.BalanceSheet();


            var lstLedgerAssets = DB.Ledgers.Where(x => x.CompanyId == Caller.CompanyId && x.AccountGroup.GroupName=="Assets").ToList();
            var lstLedgerLiability= DB.Ledgers.Where(x => x.CompanyId == Caller.CompanyId && x.AccountGroup.GroupName == "Liabilities").ToList();
            decimal TotAssDr = 0, TotAssCr = 0, TotLiDr=0, TotLiCr=0;

            #region Assets

            tb = new BLL.BalanceSheet();
            tb.LedgerList = new BLL.Ledger();
            tb.LedgerList.LedgerName = "  Assets";
            tb.CrAmt = null;
            tb.DrAmt = null;
            tb.CrAmtOP = null;
            tb.DrAmtOP = null;

            lstBalanceSheet.Add(tb);


            #region Ledger

            decimal OPDr, OPCr, PDr, PCr, RDr, RCr, JDr, JCr;

            foreach (var l in lstLedgerAssets)
            {
                tb = new BLL.BalanceSheet();
                tb.LedgerList = new BLL.Ledger();

                l.toCopy<BLL.Ledger>(tb.LedgerList);
                tb.LedgerList.GroupCode = l.AccountGroup.GroupCode;
                tb.LedgerList.GroupName = l.AccountGroup.GroupName;

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
                    tb.CrAmt = null;
                }
                else
                {
                    tb.CrAmt = tb.CrAmt - tb.DrAmt;
                    tb.DrAmt = null;
                }

                if (tb.DrAmt != 0 || tb.CrAmt != 0)
                {
                    lstBalanceSheet.Add(tb);
                    TotAssDr += tb.DrAmt==null?0:tb.DrAmt.Value;
                    TotAssCr += tb.CrAmt==null?0:tb.CrAmt.Value;
                }
            }
            #endregion

            tb = new BLL.BalanceSheet();
            tb.LedgerList = new BLL.Ledger();
            tb.LedgerList.LedgerName = "  Total Assets";
            tb.DrAmt = TotAssDr;
            tb.CrAmt = TotAssCr;
            lstBalanceSheet.Add(tb);

            #endregion


            #region Liabilities

            tb = new BLL.BalanceSheet();
            tb.LedgerList = new BLL.Ledger();
            tb.LedgerList.LedgerName = "  Liablities";
            tb.CrAmt = null;
            tb.DrAmt = null;
            tb.CrAmtOP = null;
            tb.DrAmtOP = null;

            lstBalanceSheet.Add(tb);
           
            #region Ledger

            decimal OPInDr, OPInCr, PInDr, PInCr, RInDr, RInCr, JInDr, JInCr;

            foreach (var l in lstLedgerLiability)
            {
                tb = new BLL.BalanceSheet();
                tb.LedgerList = new BLL.Ledger();

                l.toCopy<BLL.Ledger>(tb.LedgerList);
                tb.LedgerList.GroupCode = l.AccountGroup.GroupCode;
                tb.LedgerList.GroupName = l.AccountGroup.GroupName;

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
                    tb.DrAmt = tb.DrAmt - tb.CrAmt;
                    tb.CrAmt = null;
                }
                else
                {
                    tb.CrAmt = tb.CrAmt - tb.DrAmt;
                    tb.DrAmt = null;
                }

                if (tb.DrAmt != 0 || tb.CrAmt != 0)
                {
                    lstBalanceSheet.Add(tb);
                    TotLiDr += tb.DrAmt==null?0:(decimal)tb.DrAmt;
                    TotLiCr += tb.CrAmt==null ? 0 :(decimal) tb.DrAmt;
                }
            }
            #endregion


            tb = new BLL.BalanceSheet();
            tb.LedgerList = new BLL.Ledger();
            tb.LedgerList.LedgerName =  "   Total Liabilities";
            tb.DrAmt =TotLiDr;
            tb.CrAmt = TotLiCr;
            lstBalanceSheet.Add(tb);

            #endregion


            tb = new BLL.BalanceSheet();
            tb.LedgerList = new BLL.Ledger();
            tb.LedgerList.LedgerName = "Total";
            tb.DrAmt = TotLiDr+TotAssDr;
            tb.CrAmt = TotLiCr+TotAssCr;
            lstBalanceSheet.Add(tb);



            return lstBalanceSheet;
        }

        #endregion
    }
}