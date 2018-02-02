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
                var p1 = l.PaymentDetails.Where(x => x.Payment.PaymentDate >= dtFrom && x.Payment.PaymentDate <= dtTo).ToList();

                var p2 = l.Payments.Where(x => x.PaymentDate >= dtFrom && x.PaymentDate <= dtTo).ToList();

                var r1 = l.Receipts.Where(x => x.ReceiptDate >= dtFrom && x.ReceiptDate <= dtTo).ToList();

                var r2 = l.ReceiptDetails.Where(x => x.Receipt.ReceiptDate >= dtFrom && x.Receipt.ReceiptDate <= dtTo).ToList();

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

                foreach (var pd in p1)
                {
                    gl = new BLL.GeneralLedger();
                    gl.Ledger = new BLL.Ledger();
                    gl.Ledger = LedgerDAL_BLL(pd.Payment.Ledger);
                    gl.Particular = pd.Particular;
                    gl.EId = pd.Payment.Id;
                    gl.EType = BLL.FormPrefix.Payment;
                    gl.TType = pd.Payment.PaymentMode;
                    gl.EDate = pd.Payment.PaymentDate;
                    gl.RefNo = pd.Payment.PaymentMode == "Cheque" ? pd.Payment.ChequeNo : pd.Payment.RefNo;
                    gl.EntryNo = pd.Payment.EntryNo;
                    gl.DrAmt = pd.Amount;
                    gl.CrAmt = 0;
                    BalAmt += (gl.DrAmt - gl.CrAmt);
                    gl.BalAmt = Math.Abs(BalAmt);
                    lstGeneralLedger.Add(gl);
                }


                foreach (var pd in p2)
                {
                    gl = new BLL.GeneralLedger();
                    gl.Ledger = new BLL.Ledger();
                    //gl.Ledger = LedgerDAL_BLL(pd.Ledger);
                    gl.Particular = pd.Particulars;
                    gl.EId = pd.Id;
                    gl.EType = BLL.FormPrefix.Payment;
                    gl.TType = pd.PaymentMode;
                    gl.EDate = pd.PaymentDate;
                    gl.RefNo = pd.PaymentMode == "Cheque" ? pd.ChequeNo : pd.RefNo;
                    gl.EntryNo = pd.EntryNo;
                    gl.DrAmt = 0;
                    gl.CrAmt = pd.Amount;
                    gl.Ledger.AccountName = String.Join("\n", pd.PaymentDetails.Select(x => LedgerDAL_BLL(x.Ledger).AccountName).ToList());
                    BalAmt += (gl.DrAmt - gl.CrAmt);
                    gl.BalAmt = Math.Abs(BalAmt);
                    lstGeneralLedger.Add(gl);
                }


                foreach (var r in r1)
                {

                    gl = new BLL.GeneralLedger();
                    gl.Ledger = new BLL.Ledger();
                    //gl.Ledger = LedgerDAL_BLL(r.Ledger);
                    gl.Particular = r.Particulars;
                    gl.EId = r.Id;
                    gl.EType = BLL.FormPrefix.Receipt;
                    gl.TType = r.ReceiptMode;
                    gl.EDate = r.ReceiptDate;
                    gl.RefNo = r.ReceiptMode == "Cheque" ? r.ChequeNo : r.RefNo;
                    gl.EntryNo = r.EntryNo;
                    gl.DrAmt = r.Amount;
                    gl.CrAmt = 0;
                    gl.Ledger.AccountName = String.Join("\n", r.ReceiptDetails.Select(x => LedgerDAL_BLL(x.Ledger).AccountName).ToList());
                    BalAmt += (gl.DrAmt - gl.CrAmt);
                    gl.BalAmt = Math.Abs(BalAmt);
                    lstGeneralLedger.Add(gl);


                }
                foreach (var rd in r2)
                {
                    gl = new BLL.GeneralLedger();
                    gl.Ledger = new BLL.Ledger();
                    gl.Ledger = LedgerDAL_BLL(rd.Receipt.Ledger);
                    gl.Particular = rd.Particulars;
                    gl.EId = rd.Receipt.Id;
                    gl.EType = BLL.FormPrefix.Receipt;
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
                    try
                    {
                        gl = new BLL.GeneralLedger();
                        gl.Ledger = new BLL.Ledger();

                        gl.Ledger = LedgerDAL_BLL(jd.Journal.JournalDetails.Where(x => (jd.DrAmt != 0 && x.CrAmt != 0) || (jd.CrAmt != 0 && x.DrAmt != 0)).FirstOrDefault().Ledger);
                        gl.Particular = jd.Particulars;
                        gl.EId = jd.Journal.Id;
                        gl.RefCode = jd.Journal.EntryNo;
                        //if (jd.Journal.RefCode != null)
                        //{
                        //gl.RefCode = jd.Journal.RefCode;
                        gl.RefCode = jd.Journal.EntryNo;
                        if (jd.Journal.EntryNo.StartsWith(BLL.FormPrefix.Sales))
                        {
                            gl.EType = BLL.FormPrefix.Sales;

                            gl.RefEntryNo = string.Format("{0}", jd.Journal.EntryNo.Remove(0, 2));
                        }
                        else if (jd.Journal.EntryNo.StartsWith(BLL.FormPrefix.Purchase))
                        {
                            gl.EType = BLL.FormPrefix.Purchase;

                            gl.RefEntryNo = string.Format("{0}", jd.Journal.EntryNo.Remove(0, 2));
                        }
                        else if (jd.Journal.EntryNo.StartsWith(BLL.FormPrefix.SalesReturn))
                        {
                            gl.EType = BLL.FormPrefix.SalesReturn;

                            gl.RefEntryNo = string.Format("{0}", jd.Journal.EntryNo.Remove(0, 2));
                        }
                        else if (jd.Journal.EntryNo.StartsWith(BLL.FormPrefix.PurchaseReturn))
                        {
                            gl.EType = BLL.FormPrefix.PurchaseReturn;

                            gl.RefEntryNo = string.Format("{0}", jd.Journal.EntryNo.Remove(0, 2));
                        }
                        else if (jd.Journal.EntryNo.StartsWith(BLL.FormPrefix.StockIn))
                        {
                            gl.EType = BLL.FormPrefix.StockIn;

                            gl.RefEntryNo = string.Format("{0}", jd.Journal.EntryNo.Remove(0, 2));
                        }
                        else if (jd.Journal.EntryNo.StartsWith(BLL.FormPrefix.StockOut))
                        {
                            gl.EType = BLL.FormPrefix.StockOut;

                            gl.RefEntryNo = string.Format("{0}", jd.Journal.EntryNo.Remove(0, 2));
                        }
                        else if (jd.Journal.EntryNo.StartsWith(BLL.FormPrefix.JobOrderIssue))
                        {
                            gl.EType = BLL.FormPrefix.JobOrderIssue;

                            gl.RefEntryNo = string.Format("{0}", jd.Journal.EntryNo.Remove(0, 2));
                        }
                        else if (jd.Journal.EntryNo.StartsWith(BLL.FormPrefix.JobOrderReceived))
                        {
                            gl.EType = BLL.FormPrefix.JobOrderReceived;

                            gl.RefEntryNo = string.Format("{0}", jd.Journal.EntryNo.Remove(0, 2));
                        }
                        else if (jd.Journal.EntryNo.StartsWith(BLL.FormPrefix.StockInProcess))
                        {
                            gl.EType = BLL.FormPrefix.StockInProcess;

                            gl.RefEntryNo = string.Format("{0}", jd.Journal.EntryNo.Remove(0, 2));
                        }
                        else if (jd.Journal.EntryNo.StartsWith(BLL.FormPrefix.StockSeparated))
                        {
                            gl.EType = BLL.FormPrefix.StockSeparated;

                            gl.RefEntryNo = string.Format("{0}", jd.Journal.EntryNo.Remove(0, 2));
                        }
                        else if (jd.Journal.EntryNo.StartsWith(BLL.FormPrefix.Payment))
                        {
                            gl.EType = BLL.FormPrefix.Payment;

                            gl.RefEntryNo = string.Format("{0}", jd.Journal.EntryNo.Remove(0, 2));
                        }
                        else if (jd.Journal.EntryNo.StartsWith(BLL.FormPrefix.Receipt))
                        {
                            gl.EType = BLL.FormPrefix.Receipt;

                            gl.RefEntryNo = string.Format("{0}", jd.Journal.EntryNo.Remove(0, 2));
                        }

                        //}
                        else
                        {
                            gl.EType = BLL.FormPrefix.Journal;
                            gl.RefEntryNo = gl.EntryNo;
                        }

                        gl.EDate = jd.Journal.JournalDate;
                        if (jd.TransactionMode == "Cheque")
                        {
                            gl.RefNo = jd.ChequeNo;
                        }
                        else if (jd.TransactionMode == "Online" || jd.TransactionMode == "TT")
                        {
                            gl.RefNo = jd.RefNo;
                        }
                        else
                        {
                            gl.RefNo = "";
                        }
                        gl.EntryNo = jd.Journal.EntryNo;
                        gl.DrAmt = jd.DrAmt;
                        gl.CrAmt = jd.CrAmt;
                        gl.TType = jd.TransactionMode;
                        BalAmt += (gl.DrAmt - gl.CrAmt);
                        gl.BalAmt = Math.Abs(BalAmt);
                        lstGeneralLedger.Add(gl);
                    }
                    catch (Exception ex)
                    { Common.AppLib.WriteLog(ex); }
                }

                lstGeneralLedger = lstGeneralLedger.OrderBy(x => x.EDate).ToList();
                BalAmt = 0;
                foreach (var lg in lstGeneralLedger)
                {
                    BalAmt += (lg.DrAmt - lg.CrAmt);
                    lg.BalAmt = Math.Abs(BalAmt);
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
