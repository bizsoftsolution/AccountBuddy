using AccountBuddy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        public List<BLL.GeneralLedger> GeneralLedger_List(int LedgerId,DateTime dtFrom, DateTime dtTo)
        {
            List<BLL.GeneralLedger> lstGeneralLedger = new List<BLL.GeneralLedger>();


            BLL.GeneralLedger gl = new BLL.GeneralLedger();

            var lstLedger = DB.Ledgers.Where(x => x.AccountGroup.CompanyDetail.Id == Caller.CompanyId && x.Id == LedgerId).ToList();
            decimal TotDr = 0, TotCr = 0;
            
            #region Ledger

            decimal OPDr, OPCr, PDr, PCr, RDr, RCr, JDr, JCr,BalAmt;
            BalAmt = 0;
            foreach (var l in lstLedger)
            {
                gl = new BLL.GeneralLedger();
               
                gl.Ledger = LedgerDAL_BLL(l);

                OPDr = l.OPDr ?? 0;
                OPCr = l.OPCr ?? 0;

                PDr = l.PaymentDetails.Where(x => x.Payment.PaymentDate < dtFrom).Sum(x => x.Amount);
                PCr = l.Payments.Where(x => x.PaymentDate < dtFrom).Sum(x => x.Amount);

                RDr = l.Receipts.Where(x => x.ReceiptDate < dtFrom).Sum(x => x.Amount);
                RCr = l.ReceiptDetails.Where(x => x.Receipt.ReceiptDate < dtFrom).Sum(x => x.Amount);

                JDr = l.JournalDetails.Where(x => x.Journal.JournalDate < dtFrom).Sum(x => x.DrAmt);
                JCr = l.JournalDetails.Where(x => x.Journal.JournalDate < dtFrom).Sum(x => x.CrAmt);

                gl.DrAmt = OPDr + PDr + RDr + JDr;
                gl.CrAmt = OPCr + PCr + RCr + JCr;

                if (gl.DrAmt > gl.CrAmt)
                {
                    gl.DrAmt = gl.DrAmt - gl.CrAmt;
                    gl.CrAmt = 0;                    
                }
                else
                {
                    gl.CrAmt = gl.CrAmt - gl.DrAmt;
                    gl.DrAmt = 0;                    
                }
                BalAmt += (gl.DrAmt - gl.CrAmt);
                gl.BalAmt = BalAmt;

                gl.Ledger = new BLL.Ledger();
                gl.Ledger.LedgerName = string.Format("Balance {0}", l.LedgerName);
                lstGeneralLedger.Add(gl);

                foreach(var pd in  l.PaymentDetails.Where(x => x.Payment.PaymentDate >= dtFrom && x.Payment.PaymentDate <= dtTo).ToList())
                {
                    gl = new BLL.GeneralLedger();
                    gl.Ledger = new BLL.Ledger();
                    gl.Ledger = LedgerDAL_BLL(pd.Payment.Ledger);
                    pd.Ledger.toCopy<BLL.Ledger>(gl.Ledger);
                    gl.EId = pd.Payment.Id;
                    gl.EType = 'P';
                    gl.EDate = pd.Payment.PaymentDate;
                    gl.RefNo = pd.Payment.PaymentMode == "Cheque" ? pd.Payment.ChequeNo : pd.Payment.RefNo;
                    gl.EntryNo = pd.Payment.EntryNo;
                    gl.DrAmt = pd.Amount;
                    gl.CrAmt = 0;
                    BalAmt += (gl.DrAmt - gl.CrAmt);
                    gl.BalAmt = BalAmt;
                    lstGeneralLedger.Add(gl);
                }


                foreach (var p in l.Payments.Where(x => x.PaymentDate >= dtFrom && x.PaymentDate <= dtTo).ToList())
                {
                    foreach(var pd in p.PaymentDetails)
                    {
                        gl = new BLL.GeneralLedger();
                        gl.Ledger = new BLL.Ledger();
                        gl.Ledger = LedgerDAL_BLL(pd.Ledger);

                        gl.EId = p.Id;
                        gl.EType = 'P';
                        gl.EDate = p.PaymentDate;
                        gl.RefNo = p.PaymentMode == "Cheque" ? p.ChequeNo : p.RefNo;
                        gl.EntryNo = p.EntryNo;
                        gl.DrAmt = 0;
                        gl.CrAmt = pd.Amount;
                        BalAmt += (gl.DrAmt - gl.CrAmt);
                        gl.BalAmt = BalAmt;
                        lstGeneralLedger.Add(gl);
                    }                    
                }

                foreach (var r in l.Receipts.Where(x => x.ReceiptDate >= dtFrom && x.ReceiptDate <= dtTo).ToList())
                {
                    foreach(var rd in r.ReceiptDetails)
                    {
                        gl = new BLL.GeneralLedger();
                        gl.Ledger = new BLL.Ledger();
                        gl.Ledger = LedgerDAL_BLL(rd.Ledger);

                        gl.EId = r.Id;
                        gl.EType = 'R';
                        gl.EDate = r.ReceiptDate;
                        gl.RefNo = r.ReceiptMode == "Cheque" ? r.ChequeNo : r.RefNo;
                        gl.EntryNo = r.EntryNo;
                        gl.DrAmt = rd.Amount;
                        gl.CrAmt = 0;
                        BalAmt += (gl.DrAmt - gl.CrAmt);
                        gl.BalAmt = BalAmt;
                        lstGeneralLedger.Add(gl);
                    }
                    
                }
                foreach(var rd in l.ReceiptDetails.Where(x => x.Receipt.ReceiptDate >= dtFrom && x.Receipt.ReceiptDate <= dtTo).ToList())
                {
                    gl = new BLL.GeneralLedger();
                    gl.Ledger = new BLL.Ledger();
                    gl.Ledger = LedgerDAL_BLL(rd.Receipt.Ledger);
                    gl.EId = rd.Receipt.Id;
                    gl.EType = 'R';
                    gl.EDate = rd.Receipt.ReceiptDate;
                    gl.RefNo = rd.Receipt.ReceiptMode == "Cheque" ? rd.Receipt.ChequeNo : rd.Receipt.RefNo;
                    gl.EntryNo = rd.Receipt.EntryNo;
                    gl.DrAmt = 0;
                    gl.CrAmt = rd.Amount; 
                    BalAmt += (gl.DrAmt - gl.CrAmt);
                    gl.BalAmt = BalAmt;
                    lstGeneralLedger.Add(gl);
                }

                foreach(var jd in l.JournalDetails.Where(x => x.Journal.JournalDate >= dtFrom && x.Journal.JournalDate <= dtTo).ToList())
                {
                    gl = new BLL.GeneralLedger();
                    gl.Ledger = new BLL.Ledger();
                    
                    gl.Ledger = LedgerDAL_BLL(jd.Ledger);

                    gl.EId = jd.Journal.Id;
                    gl.EType = 'J';
                    gl.EDate = jd.Journal.JournalDate;
                    gl.RefNo = "";
                    gl.EntryNo = jd.Journal.EntryNo;
                    gl.DrAmt = jd.DrAmt;
                    gl.CrAmt = jd.CrAmt;
                    BalAmt += (gl.DrAmt - gl.CrAmt);
                    gl.BalAmt = BalAmt;
                    lstGeneralLedger.Add(gl);
                }                                
                gl = new BLL.GeneralLedger();
                gl.Ledger = new BLL.Ledger();
                gl.Ledger.LedgerName = "Total";
                gl.DrAmt = lstGeneralLedger.Sum(x=> x.DrAmt);
                gl.CrAmt = lstGeneralLedger.Sum(x=>x.CrAmt);
                gl.BalAmt = BalAmt;
                lstGeneralLedger.Add(gl);

            }
            #endregion

            
            return lstGeneralLedger;
        }

    }
}
