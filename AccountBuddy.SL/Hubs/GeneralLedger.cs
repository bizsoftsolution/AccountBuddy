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
        public List<BLL.GeneralLedger> GeneralLedger_List(int LedgerId, DateTime dtFrom, DateTime dtTo)
        {
            List<BLL.GeneralLedger> lstGeneralLedger = new List<BLL.GeneralLedger>();


            BLL.GeneralLedger gl = new BLL.GeneralLedger();

            var lstLedger = DB.Ledgers.Where(x => x.AccountGroup.CompanyDetail.Id == Caller.CompanyId && x.Id == LedgerId).ToList();


            #region Ledger

            decimal OPDr, OPCr, PDr, PCr, RDr, RCr, JDr, JCr, BalAmt;
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
                gl.BalAmt = Math.Abs(BalAmt);

                gl.Ledger = new BLL.Ledger();
                gl.Particular = string.Format("Balance {0}", l.LedgerName);
                lstGeneralLedger.Add(gl);

                foreach (var pd in l.PaymentDetails.Where(x => x.Payment.PaymentDate >= dtFrom && x.Payment.PaymentDate <= dtTo).ToList())
                {
                    gl = new BLL.GeneralLedger();
                    gl.Ledger = new BLL.Ledger();
                    gl.Ledger = LedgerDAL_BLL(pd.Payment.Ledger);
                    gl.Particular = pd.Particular;
                    gl.EId = pd.Payment.Id;
                    gl.EType = BLL.FormPrefix.Payment;
                    gl.EDate = pd.Payment.PaymentDate;
                    gl.RefNo = pd.Payment.PaymentMode == "Cheque" ? pd.Payment.ChequeNo : pd.Payment.RefNo;
                    gl.EntryNo = pd.Payment.EntryNo;
                    gl.DrAmt = pd.Amount;
                    gl.CrAmt = 0;
                    BalAmt += (gl.DrAmt - gl.CrAmt);
                    gl.BalAmt = Math.Abs(BalAmt);
                    lstGeneralLedger.Add(gl);
                }


                foreach (var p in l.Payments.Where(x => x.PaymentDate >= dtFrom && x.PaymentDate <= dtTo).ToList())
                {
                    foreach (var pd in p.PaymentDetails)
                    {
                        gl = new BLL.GeneralLedger();
                        gl.Ledger = new BLL.Ledger();
                        gl.Ledger = LedgerDAL_BLL(pd.Ledger);
                        gl.Particular = pd.Particular;
                        gl.EId = p.Id;
                        gl.EType = BLL.FormPrefix.Payment;
                        gl.EDate = p.PaymentDate;
                        gl.RefNo = p.PaymentMode == "Cheque" ? p.ChequeNo : p.RefNo;
                        gl.EntryNo = p.EntryNo;
                        gl.DrAmt = 0;
                        gl.CrAmt = pd.Amount;
                        BalAmt += (gl.DrAmt - gl.CrAmt);
                        gl.BalAmt = Math.Abs(BalAmt);
                        lstGeneralLedger.Add(gl);
                    }
                }

                foreach (var r in l.Receipts.Where(x => x.ReceiptDate >= dtFrom && x.ReceiptDate <= dtTo).ToList())
                {
                    foreach (var rd in r.ReceiptDetails)
                    {
                        gl = new BLL.GeneralLedger();
                        gl.Ledger = new BLL.Ledger();
                        gl.Ledger = LedgerDAL_BLL(rd.Ledger);
                        gl.Particular = rd.Particulars;
                        gl.EId = r.Id;
                        gl.EType = BLL.FormPrefix.Receipt;
                        gl.EDate = r.ReceiptDate;
                        gl.RefNo = r.ReceiptMode == "Cheque" ? r.ChequeNo : r.RefNo;
                        gl.EntryNo = r.EntryNo;
                        gl.DrAmt = rd.Amount;
                        gl.CrAmt = 0;
                        BalAmt += (gl.DrAmt - gl.CrAmt);
                        gl.BalAmt = Math.Abs(BalAmt);
                        lstGeneralLedger.Add(gl);
                    }

                }
                foreach (var rd in l.ReceiptDetails.Where(x => x.Receipt.ReceiptDate >= dtFrom && x.Receipt.ReceiptDate <= dtTo).ToList())
                {
                    gl = new BLL.GeneralLedger();
                    gl.Ledger = new BLL.Ledger();
                    gl.Ledger = LedgerDAL_BLL(rd.Receipt.Ledger);
                    gl.Particular = rd.Particulars;
                    gl.EId = rd.Receipt.Id;
                    gl.EType =BLL.FormPrefix.Receipt;
                    gl.EDate = rd.Receipt.ReceiptDate;
                    gl.RefNo = rd.Receipt.ReceiptMode == "Cheque" ? rd.Receipt.ChequeNo : rd.Receipt.RefNo;
                    gl.EntryNo = rd.Receipt.EntryNo;
                    gl.DrAmt = 0;
                    gl.CrAmt = rd.Amount;
                    BalAmt += (gl.DrAmt - gl.CrAmt);
                    gl.BalAmt = Math.Abs(BalAmt);
                    lstGeneralLedger.Add(gl);
                }

                foreach (var jd in l.JournalDetails.Where(x => x.Journal.JournalDate >= dtFrom && x.Journal.JournalDate <= dtTo).ToList())
                {
                    gl = new BLL.GeneralLedger();
                    gl.Ledger = new BLL.Ledger();

                    gl.Ledger = LedgerDAL_BLL(jd.Journal.JournalDetails.Where(x => (jd.DrAmt != 0 && x.CrAmt != 0) || (jd.CrAmt != 0 && x.DrAmt != 0)).FirstOrDefault().Ledger);
                    gl.Particular = jd.Particulars;
                    gl.EId = jd.Journal.Id;
                    if (jd.Journal.RefCode != null)
                    {
                        gl.RefCode = jd.Journal.RefCode;

                        if (jd.Journal.RefCode.StartsWith(BLL.FormPrefix.Sales))
                        {
                            gl.EType = BLL.FormPrefix.Sales;
                            gl.RefEntryNo = string.Format("{0}", jd.Journal.RefCode.Remove(0, 2));
                        }
                        else if (jd.Journal.RefCode.StartsWith(BLL.FormPrefix.Purchase))
                        {
                            gl.EType = BLL.FormPrefix.Purchase;
                            gl.RefEntryNo = string.Format("{0}", jd.Journal.RefCode.Remove(0, 2));
                        }
                        else if (jd.Journal.RefCode.StartsWith(BLL.FormPrefix.SalesReturn))
                        {
                            gl.EType = BLL.FormPrefix.SalesReturn;
                            gl.RefEntryNo = string.Format("{0}", jd.Journal.RefCode.Remove(0, 2));
                        }
                        else if (jd.Journal.RefCode.StartsWith(BLL.FormPrefix.PurchaseReturn))
                        {
                            gl.EType = BLL.FormPrefix.PurchaseReturn;
                            gl.RefEntryNo = string.Format("{0}", jd.Journal.RefCode.Remove(0, 2));
                        }
                        else if (jd.Journal.RefCode.StartsWith(BLL.FormPrefix.StockIn))
                        {
                            gl.EType = BLL.FormPrefix.StockIn;
                            gl.RefEntryNo = string.Format("{0}", jd.Journal.RefCode.Remove(0, 2));
                        }
                        else if (jd.Journal.RefCode.StartsWith(BLL.FormPrefix.StockOut))
                        {
                            gl.EType = BLL.FormPrefix.StockOut;
                            gl.RefEntryNo = string.Format("{0}", jd.Journal.RefCode.Remove(0, 2));
                        }
                        else if (jd.Journal.RefCode.StartsWith(BLL.FormPrefix.JobOrderIssue))
                        {
                            gl.EType = BLL.FormPrefix.JobOrderIssue;
                            gl.RefEntryNo = string.Format("{0}", jd.Journal.RefCode.Remove(0, 2));
                        }
                        else if (jd.Journal.RefCode.StartsWith(BLL.FormPrefix.JobOrderReceived))
                        {
                            gl.EType = BLL.FormPrefix.JobOrderReceived;
                            gl.RefEntryNo = string.Format("{0}", jd.Journal.RefCode.Remove(0, 2));
                        }
                        else if (jd.Journal.RefCode.StartsWith(BLL.FormPrefix.StockInProcess))
                        {
                            gl.EType = BLL.FormPrefix.StockInProcess;
                            gl.RefEntryNo = string.Format("{0}", jd.Journal.RefCode.Remove(0, 2));
                        }
                        else if (jd.Journal.RefCode.StartsWith(BLL.FormPrefix.StockSeparated))
                        {
                            gl.EType = BLL.FormPrefix.StockSeparated;
                            gl.RefEntryNo = string.Format("{0}", jd.Journal.RefCode.Remove(0, 2));
                        }
                        else if (jd.Journal.RefCode.StartsWith(BLL.FormPrefix.Payment))
                        {
                            gl.EType = BLL.FormPrefix.Payment;
                            gl.RefEntryNo = string.Format("{0}", jd.Journal.RefCode.Remove(0, 2));
                        }
                        else if(jd.Journal.RefCode.StartsWith(BLL.FormPrefix.Receipt))
                        {
                            gl.EType = BLL.FormPrefix.Receipt;
                            gl.RefEntryNo = string.Format("{0}", jd.Journal.RefCode.Remove(0, 2));
                        }
                      
                    }
                    else
                    {
                        gl.EType = BLL.FormPrefix.Journal;
                        gl.RefEntryNo = gl.EntryNo;
                    }

                    gl.EDate = jd.Journal.JournalDate;
                    gl.RefNo = "";
                    gl.EntryNo = jd.Journal.EntryNo;
                    gl.DrAmt = jd.DrAmt;
                    gl.CrAmt = jd.CrAmt;
                    BalAmt += (gl.DrAmt - gl.CrAmt);
                    gl.BalAmt = Math.Abs(BalAmt);
                    lstGeneralLedger.Add(gl);
                }
                gl = new BLL.GeneralLedger();
                gl.Ledger = new BLL.Ledger();
                gl.Particular = "Total";
                gl.DrAmt = lstGeneralLedger.Sum(x => x.DrAmt);
                gl.CrAmt = lstGeneralLedger.Sum(x => x.CrAmt);
                gl.BalAmt = Math.Abs(BalAmt);
                lstGeneralLedger.Add(gl);

            }
            #endregion


            return lstGeneralLedger;
        }

    }
}
