using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AccountBuddy.Common;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        #region ProfitLoss

        public List<BLL.ProfitLoss> ProfitLoss_List(DateTime dtFrom, DateTime dtTo)
        {
            List<BLL.ProfitLoss> lstProfitLoss = new List<BLL.ProfitLoss>();
            var l1 = DB.AccountGroups.Where(x => x.CompanyId == Caller.CompanyId && (x.GroupName == "Assets" || x.GroupName == "Liabilities")).ToList();

            foreach (var ag in l1)
            {
                decimal TotalDr = 0, TotalCr = 0, TotalDrOP = 0, TotalCrOP = 0;
                lstProfitLoss.AddRange(ProfitLossByGroupName(ag, dtFrom, dtTo, "", ref TotalDr, ref TotalCr, ref TotalDrOP, ref TotalCrOP));
            }

            return lstProfitLoss;
        }

        List<BLL.ProfitLoss> ProfitLossByGroupName(DAL.AccountGroup ag, DateTime dtFrom, DateTime dtTo, string Prefix, ref decimal TotalDr, ref decimal TotalCr, ref decimal TotalDrOP, ref decimal TotalCrOP)
        {
            List<BLL.ProfitLoss> lstProfitLoss = new List<BLL.ProfitLoss>();
            if (ag.AccountGroup1.Count() == 0 && ag.Ledgers.Count() == 0) return lstProfitLoss;

            BLL.ProfitLoss tb = new BLL.ProfitLoss();

            tb = new BLL.ProfitLoss();
            tb.Ledger = new BLL.Ledger();
            tb.Ledger .AccountName = Prefix + ag.GroupName;
            tb.CrAmt = null;
            tb.DrAmt = null;
            tb.CrAmtOP = null;
            tb.DrAmtOP = null;

            lstProfitLoss.Add(tb);


            foreach (var uag in ag.AccountGroup1)
            {
                lstProfitLoss.AddRange(ProfitLossByGroupName(uag, dtFrom, dtTo, Prefix + "     ", ref TotalDr, ref TotalCr, ref TotalDrOP, ref TotalCrOP));
            }

            decimal OPDr, OPCr, PDr, PCr, RDr, RCr, JDr, JCr;

            foreach (var l in ag.Ledgers)
            {
                tb = new BLL.ProfitLoss();
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
                    tb.CrAmt = null;
                }
                else
                {
                    tb.CrAmt = tb.CrAmt - tb.DrAmt;
                    tb.DrAmt = null;
                }

                if (tb.DrAmt != 0 || tb.CrAmt != 0)
                {
                    tb.Ledger.AccountGroup.GroupCode = Prefix + "     " + tb.Ledger.AccountGroup.GroupCode;
                    lstProfitLoss.Add(tb);
                    TotalDr += tb.DrAmt ?? 0;
                    TotalCr += tb.CrAmt ?? 0;

                    TotalDrOP += tb.DrAmtOP ?? 0;
                    TotalCrOP += tb.CrAmtOP ?? 0;

                }
            }

            if (TotalDr > TotalCr)
            {
                TotalDr = Math.Abs(TotalDr - TotalCr);
                TotalCr = 0;
            }
            else
            {
                TotalCr = Math.Abs(TotalDr - TotalCr);
                TotalDr = 0;
            }

            if (TotalDrOP > TotalCrOP)
            {
                TotalDrOP = Math.Abs(TotalDrOP - TotalCrOP);
                TotalCrOP = 0;
            }
            else
            {
                TotalCrOP = Math.Abs(TotalDrOP - TotalCrOP);
                TotalDrOP = 0;
            }


            tb = new BLL.ProfitLoss();
            tb.Ledger= new BLL.Ledger();
            tb.Ledger.AccountName = Prefix + "Total " + ag.GroupName;
            tb.CrAmt = TotalCr;
            tb.DrAmt = TotalDr;
            tb.CrAmtOP = TotalCrOP;
            tb.DrAmtOP = TotalDrOP;


            lstProfitLoss.Add(tb);

            return lstProfitLoss;
        }

        #endregion
    }
}