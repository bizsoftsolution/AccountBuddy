﻿using AccountBuddy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        public List<BLL.ReceiptAndPayment> ReceiptAndPayment_List(int? LedgerId, DateTime dtFrom, DateTime dtTo)
        {
            List<BLL.ReceiptAndPayment> lstReceiptAndPayment = new List<BLL.ReceiptAndPayment>();


            BLL.ReceiptAndPayment rp = new BLL.ReceiptAndPayment();

            var lstLedger = DB.Ledgers.Where(x => x.AccountGroup.CompanyId == Caller.CompanyId && (LedgerId==null || x.Id == LedgerId)).ToList();
            
            #region Ledger

            foreach (var l in lstLedger)
            {
                foreach (var pd in l.PaymentDetails.Where(x => x.Payment.PaymentDate >= dtFrom && x.Payment.PaymentDate <= dtTo).ToList())
                {
                    rp = new BLL.ReceiptAndPayment();
                    rp.Ledger = new BLL.Ledger();
                    // pd.Ledger.toCopy<BLL.Ledger>(rp.Ledger);

                    rp.Ledger = LedgerDAL_BLL(pd.Ledger);
                    rp.EId = pd.Payment.Id;
                    rp.EType = 'P';
                    rp.EDate = pd.Payment.PaymentDate;
                    rp.RefNo = pd.Payment.PaymentMode == "Cheque" ? pd.Payment.ChequeNo : pd.Payment.RefNo;
                    rp.EntryNo = pd.Payment.EntryNo;
                    rp.Amount = pd.Amount;
                    lstReceiptAndPayment.Add(rp);
                }


                foreach (var p in l.Payments.Where(x => x.PaymentDate >= dtFrom && x.PaymentDate <= dtTo).ToList())
                {
                    foreach(var pd in p.PaymentDetails)
                    {
                        rp = new BLL.ReceiptAndPayment();
                        rp.Ledger = new BLL.Ledger();
                        //p.Ledger.toCopy<BLL.Ledger>(rp.Ledger);

                        rp.Ledger = LedgerDAL_BLL(pd.Ledger);
                        rp.EId = p.Id;
                        rp.EType = 'P';
                        rp.EDate = p.PaymentDate;
                        rp.RefNo = p.PaymentMode == "Cheque" ? p.ChequeNo : p.RefNo;
                        rp.EntryNo = p.EntryNo;
                        rp.Amount = pd.Amount;
                        lstReceiptAndPayment.Add(rp);
                    }                    
                }

                foreach (var r in l.Receipts.Where(x => x.ReceiptDate >= dtFrom && x.ReceiptDate <= dtTo).ToList())
                {
                    foreach(var rd in r.ReceiptDetails)
                    {
                        rp = new BLL.ReceiptAndPayment();
                        rp.Ledger = new BLL.Ledger();
                        rp.Ledger = LedgerDAL_BLL(rd.Ledger);
                        rp.EId = r.Id;
                        rp.EType = 'R';
                        rp.EDate = r.ReceiptDate;
                        rp.RefNo = r.ReceiptMode == "Cheque" ? r.ChequeNo : r.RefNo;
                        rp.EntryNo = r.EntryNo;
                        rp.Amount = rd.Amount;
                        lstReceiptAndPayment.Add(rp);
                    }
                    
                }
                foreach (var rd in l.ReceiptDetails.Where(x => x.Receipt.ReceiptDate >= dtFrom && x.Receipt.ReceiptDate <= dtTo).ToList())
                {
                    rp = new BLL.ReceiptAndPayment();
                    rp.Ledger = new BLL.Ledger();
                    rp.Ledger = LedgerDAL_BLL(rd.Receipt.Ledger);

                    rp.EId = rd.Receipt.Id;
                    rp.EType = 'R';
                    rp.EDate = rd.Receipt.ReceiptDate;
                    rp.RefNo = rd.Receipt.ReceiptMode == "Cheque" ? rd.Receipt.ChequeNo : rd.Receipt.RefNo;
                    rp.EntryNo = rd.Receipt.EntryNo;
                    rp.Amount = rd.Amount;
                    lstReceiptAndPayment.Add(rp);
                }
                
            }
            #endregion


            return lstReceiptAndPayment;
        }

    }
}
