using AccountBuddy.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        #region Journal        


        public bool Journal_Save(BLL.Journal PO)
        {
            try
            {

                DAL.Journal d = DB.Journals.Where(x => x.Id == PO.Id).FirstOrDefault();

                if (d == null)
                {

                    d = new DAL.Journal();
                    DB.Journals.Add(d);

                    PO.toCopy<DAL.Journal>(d);

                    foreach (var b_pod in PO.JDetails)
                    {
                        DAL.JournalDetail d_pod = new DAL.JournalDetail();
                        b_pod.toCopy<DAL.JournalDetail>(d_pod);
                        d.JournalDetails.Add(d_pod);
                    }
                    DB.SaveChanges();
                    PO.Id = d.Id;
                    LogDetailStore(PO, LogDetailType.INSERT);
                }
                else
                {

                    foreach (var d_SOd in d.JournalDetails)
                    {
                        BLL.JournalDetail b_SOd = PO.JDetails.Where(x => x.Id == d_SOd.Id).FirstOrDefault();
                        if (b_SOd == null) d.JournalDetails.Remove(d_SOd);
                    }

                    PO.toCopy<DAL.Journal>(d);

                    foreach (var b_pod in PO.JDetails)
                    {
                        DAL.JournalDetail d_pod = d.JournalDetails.Where(x => x.Id == b_pod.Id).FirstOrDefault();
                        if (d_pod == null)
                        {
                            d_pod = new DAL.JournalDetail();
                            d.JournalDetails.Add(d_pod);
                        }
                        b_pod.toCopy<DAL.JournalDetail>(d_pod);
                    }
                    DB.SaveChanges();
                    LogDetailStore(PO, LogDetailType.UPDATE);
                }

                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public BLL.Journal Journal_Find(string SearchText)
        {
            BLL.Journal PO = new BLL.Journal();
            try
            {

                DAL.Journal d = DB.Journals.Where(x => x.EntryNo == SearchText && x.JournalDetails.FirstOrDefault().Ledger.AccountGroup.CompanyId == Caller.CompanyId).FirstOrDefault();
                DB.Entry(d).Reload();
                if (d != null)
                {

                    d.toCopy<BLL.Journal>(PO);
                    foreach (var d_pod in d.JournalDetails)
                    {
                        BLL.JournalDetail b_pod = new BLL.JournalDetail();
                        d_pod.toCopy<BLL.JournalDetail>(b_pod);
                        PO.JDetails.Add(b_pod);
                        b_pod.LedgerName = (d_pod.Ledger ?? DB.Ledgers.Find(d_pod.LedgerId) ?? new DAL.Ledger()).LedgerName;
                    }

                }
            }
            catch (Exception ex) { }
            return PO;
        }

        public bool Journal_Delete(long pk)
        {
            try
            {
                DAL.Journal d = DB.Journals.Where(x => x.Id == pk).FirstOrDefault();

                if (d != null)
                {
                    DB.JournalDetails.RemoveRange(d.JournalDetails);
                    DB.Journals.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(Journal_DALtoBLL(d), LogDetailType.DELETE);
                }
                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public BLL.Journal Journal_DALtoBLL(DAL.Journal d)
        {
            BLL.Journal J = d.toCopy<BLL.Journal>(new BLL.Journal());
            foreach (var d_Jd in d.JournalDetails)
            {
                J.JDetails.Add(d_Jd.toCopy<BLL.JournalDetail>(new BLL.JournalDetail()));
            }
            return J;
        }

        public bool Find_JEntryNo(string entryNo, BLL.Payment PO)

        {
            DAL.Journal d = DB.Journals.Where(x => x.EntryNo == entryNo & x.Id != PO.Id && x.JournalDetails.FirstOrDefault().Ledger.AccountGroup.CompanyId == Caller.CompanyId).FirstOrDefault();
            if (d == null)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        int LedgerIdByKey(string key)
        {
            return LedgerIdByKeyAndCompany(key, Caller.CompanyId);
        }

        int LedgerIdByKeyAndCompany(string key, int CompanyId)
        {
            return DB.DataKeyValues.Where(x => x.CompanyId == CompanyId && x.DataKey == key).FirstOrDefault().DataValue;
        }
        int LedgerIdByCompany(string LName, int CompanyId)
        {
            var l = DB.Ledgers.Where(x => x.LedgerName == LName && x.AccountGroup.CompanyId == CompanyId).FirstOrDefault();
            return l == null ? 0 : l.Id;
        }
        int CompanyIdByLedgerName(string LedgerName)
        {
            var CName = LedgerName.Substring(3);
            var cm = DB.CompanyDetails.Where(x => x.CompanyName == CName).FirstOrDefault();
            return cm == null ? 0 : cm.Id;
        }
        string LedgerNameByCompanyId(int CompanyId)
        {
            var cm = DB.CompanyDetails.Where(x => x.Id == CompanyId).FirstOrDefault();
            return string.Format("{0}-{1}", cm.CompanyType == "Company" ? "CM" : (cm.CompanyType == "Warehouse" ? "WH" : "DL"), cm.CompanyName);
        }

        void Journal_SaveByPurchase(BLL.Purchase P)
        {
            var EntryNo = string.Format("PUR-{0}", P.Id);

            DAL.Journal j = DB.Journals.Where(x => x.EntryNo == EntryNo).FirstOrDefault();
            if (j == null)
            {
                j = new DAL.Journal();
                j.EntryNo = EntryNo;

                j.JournalDetails.Add(new DAL.JournalDetail()
                {
                    LedgerId = P.TransactionType == "Cash" ? LedgerIdByKey(BLL.DataKeyValue.CashLedger_Key) : P.LedgerId,
                    CrAmt = P.TotalAmount,
                    Particulars = P.Narration
                });

                j.JournalDetails.Add(new DAL.JournalDetail()
                {
                    LedgerId = LedgerIdByKey(BLL.DataKeyValue.PurchaseAccount_Ledger_Key),
                    DrAmt = P.ItemAmount - P.DiscountAmount + P.ExtraAmount,
                    Particulars = P.Narration
                });

                j.JournalDetails.Add(new DAL.JournalDetail()
                {
                    LedgerId = LedgerIdByKey(BLL.DataKeyValue.Output_Tax_Ledger_Key),
                    DrAmt = P.GSTAmount,
                    Particulars = P.Narration
                });

                DB.Journals.Add(j);
            }
            else
            {
                foreach (var jd in j.JournalDetails)
                {
                    jd.Particulars = P.Narration;
                    if (jd.CrAmt != 0)
                    {
                        jd.LedgerId = P.TransactionType == "Cash" ? LedgerIdByKey(BLL.DataKeyValue.CashLedger_Key) : P.LedgerId;
                        jd.CrAmt = P.TotalAmount;
                    }
                    else
                    {
                        jd.DrAmt = jd.LedgerId == LedgerIdByKey(BLL.DataKeyValue.PurchaseAccount_Ledger_Key) ? P.ItemAmount - P.DiscountAmount + P.ExtraAmount : P.GSTAmount;
                    }
                }
            }

            j.JournalDate = P.PurchaseDate;
            DB.SaveChanges();
        }
        void Journal_DeleteByPurchase(BLL.Purchase P)
        {
            var EntryNo = string.Format("PUR-{0}", P.Id);
            DAL.Journal j = DB.Journals.Where(x => x.EntryNo == EntryNo).FirstOrDefault();
            if (j != null) Journal_Delete(j.Id);
        }
        void Journal_SaveBySalesReturn(BLL.SalesReturn SR)
        {
            var EntryNo = string.Format("SRN-{0}", SR.Id);

            DAL.Journal j = DB.Journals.Where(x => x.EntryNo == EntryNo).FirstOrDefault();
            if (j == null)
            {
                j = new DAL.Journal();
                j.EntryNo = EntryNo;

                j.JournalDetails.Add(new DAL.JournalDetail()
                {
                    LedgerId = SR.TransactionType == "Cash" ? LedgerIdByKey(BLL.DataKeyValue.CashLedger_Key) : SR.LedgerId,
                    CrAmt = SR.TotalAmount,
                    Particulars = SR.Narration
                });

                j.JournalDetails.Add(new DAL.JournalDetail()
                {
                    LedgerId = LedgerIdByKey(BLL.DataKeyValue.Sales_Return_Ledger_Key),
                    DrAmt = SR.ItemAmount - SR.DiscountAmount + SR.ExtraAmount,
                    Particulars = SR.Narration
                });

                j.JournalDetails.Add(new DAL.JournalDetail()
                {
                    LedgerId = LedgerIdByKey(BLL.DataKeyValue.Output_Tax_Ledger_Key),
                    DrAmt = SR.GSTAmount,
                    Particulars = SR.Narration
                });

                DB.Journals.Add(j);
            }
            else
            {
                foreach (var jd in j.JournalDetails)
                {
                    jd.Particulars = SR.Narration;
                    if (jd.CrAmt != 0)
                    {
                        jd.LedgerId = SR.TransactionType == "Cash" ? LedgerIdByKey(BLL.DataKeyValue.CashLedger_Key) : SR.LedgerId;
                        jd.CrAmt = SR.TotalAmount;
                    }
                    else
                    {
                        jd.DrAmt = jd.LedgerId == LedgerIdByKey(BLL.DataKeyValue.Sales_Return_Ledger_Key) ? SR.ItemAmount - SR.DiscountAmount + SR.ExtraAmount : SR.GSTAmount;
                    }
                }
            }
            j.JournalDate = SR.SRDate;
            DB.SaveChanges();
        }
        void Journal_DeleteBySalesReturn(BLL.SalesReturn SR)
        {
            var EntryNo = string.Format("SRN-{0}", SR.Id);

            DAL.Journal j = DB.Journals.Where(x => x.EntryNo == EntryNo).FirstOrDefault();
            if (j == null) Journal_Delete(j.Id);
        }
        void Journal_SaveBySales(BLL.Sale S)
        {
            var EntryNo = string.Format("SAL-{0}", S.Id);

            DAL.Journal j = DB.Journals.Where(x => x.EntryNo == EntryNo).FirstOrDefault();
            if (j == null)
            {
                j = new DAL.Journal();
                j.EntryNo = EntryNo;

                j.JournalDetails.Add(new DAL.JournalDetail()
                {
                    LedgerId = S.TransactionType == "Cash" ? LedgerIdByKey(BLL.DataKeyValue.CashLedger_Key) : S.LedgerId,
                    DrAmt = S.TotalAmount,
                    Particulars = S.Narration
                });

                j.JournalDetails.Add(new DAL.JournalDetail()
                {
                    LedgerId = LedgerIdByKey(BLL.DataKeyValue.SalesAccount_Ledger_Key),
                    CrAmt = S.ItemAmount - S.DiscountAmount + S.ExtraAmount,
                    Particulars = S.Narration
                });

                j.JournalDetails.Add(new DAL.JournalDetail()
                {
                    LedgerId = LedgerIdByKey(BLL.DataKeyValue.Input_Tax_Ledger_Key),
                    CrAmt = S.GSTAmount,
                    Particulars = S.Narration
                });

                DB.Journals.Add(j);
            }
            else
            {
                foreach (var jd in j.JournalDetails)
                {
                    jd.Particulars = S.Narration;
                    if (jd.DrAmt != 0)
                    {
                        jd.LedgerId = S.TransactionType == "Cash" ? LedgerIdByKey(BLL.DataKeyValue.CashLedger_Key) : S.LedgerId;
                        jd.DrAmt = S.TotalAmount;
                    }
                    else
                    {
                        jd.CrAmt = jd.LedgerId == LedgerIdByKey(BLL.DataKeyValue.SalesAccount_Ledger_Key) ? S.ItemAmount - S.DiscountAmount + S.ExtraAmount : S.GSTAmount;
                    }
                }
            }
            j.JournalDate = S.SalesDate;
            DB.SaveChanges();
        }
        void Journal_DeleteBySales(BLL.Sale S)
        {
            var EntryNo = string.Format("SAL-{0}", S.Id);

            DAL.Journal j = DB.Journals.Where(x => x.EntryNo == EntryNo).FirstOrDefault();
            if (j == null) Journal_Delete(j.Id);
        }
        void Journal_SaveByPurchaseReturn(BLL.PurchaseReturn PR)
        {
            var EntryNo = string.Format("PRN-{0}", PR.Id);

            DAL.Journal j = DB.Journals.Where(x => x.EntryNo == EntryNo).FirstOrDefault();
            if (j == null)
            {
                j = new DAL.Journal();
                j.EntryNo = EntryNo;

                j.JournalDetails.Add(new DAL.JournalDetail()
                {
                    LedgerId = PR.TransactionType == "Cash" ? LedgerIdByKey(BLL.DataKeyValue.CashLedger_Key) : PR.LedgerId,
                    DrAmt = PR.TotalAmount,
                    Particulars = PR.Narration
                });

                j.JournalDetails.Add(new DAL.JournalDetail()
                {
                    LedgerId = LedgerIdByKey(BLL.DataKeyValue.Purchase_Return_Ledger_Key),
                    CrAmt = PR.ItemAmount - PR.DiscountAmount + PR.ExtraAmount,
                    Particulars = PR.Narration
                });

                j.JournalDetails.Add(new DAL.JournalDetail()
                {
                    LedgerId = LedgerIdByKey(BLL.DataKeyValue.Input_Tax_Ledger_Key),
                    CrAmt = PR.GSTAmount,
                    Particulars = PR.Narration
                });

                DB.Journals.Add(j);
            }
            else
            {
                foreach (var jd in j.JournalDetails)
                {
                    jd.Particulars = PR.Narration;
                    if (jd.DrAmt != 0)
                    {
                        jd.LedgerId = PR.TransactionType == "Cash" ? LedgerIdByKey(BLL.DataKeyValue.CashLedger_Key) : PR.LedgerId;
                        jd.DrAmt = PR.TotalAmount;
                    }
                    else
                    {
                        jd.CrAmt = jd.LedgerId == LedgerIdByKey(BLL.DataKeyValue.Purchase_Return_Ledger_Key) ? PR.ItemAmount - PR.DiscountAmount + PR.ExtraAmount : PR.GSTAmount;
                    }
                }
            }
            j.JournalDate = PR.PRDate;
            DB.SaveChanges();
        }
        void Journal_DeleteByPurchaseReturn(BLL.PurchaseReturn PR)
        {
            var EntryNo = string.Format("PRN-{0}", PR.Id);

            DAL.Journal j = DB.Journals.Where(x => x.EntryNo == EntryNo).FirstOrDefault();
            if (j == null) Journal_Delete(j.Id);
        }


        void Journal_SaveByPayment(BLL.Payment P)
        {
            var EntryNo = string.Format("PMT-{0}", P.Id);

            DAL.Journal j = DB.Journals.Where(x => x.EntryNo == EntryNo).FirstOrDefault();
            if (j == null)
            {
                var pd = P.PDetails.FirstOrDefault();
                var ld = DB.Ledgers.Where(x => x.Id == pd.LedgerId).FirstOrDefault();

                if (ld.LedgerName.StartsWith("CM-") || ld.LedgerName.StartsWith("WH-") || ld.LedgerName.StartsWith("DL-"))
                {
                    j = new DAL.Journal();
                    j.EntryNo = EntryNo;
                    j.JournalDate = P.PaymentDate;

                    var CId = CompanyIdByLedgerName(ld.LedgerName);
                    if (CId != 0)
                    {
                        var LName = LedgerNameByCompanyId(Caller.CompanyId);

                        j.JournalDetails.Add(new DAL.JournalDetail()
                        {
                            LedgerId = LedgerIdByKeyAndCompany(BLL.DataKeyValue.CashLedger_Key, CId),
                            DrAmt = P.Amount,
                            Particulars = P.Particulars
                        });
                        j.JournalDetails.Add(new DAL.JournalDetail()
                        {

                            LedgerId = LedgerIdByCompany(LName, CId),
                            CrAmt = P.Amount,
                            Particulars = P.Particulars
                        });
                        DB.Journals.Add(j);
                        DB.SaveChanges();
                    }


                }
            }
            else
            {
                j.JournalDate = P.PaymentDate;
                foreach (var jd in j.JournalDetails)
                {
                    if (jd.CrAmt != 0) jd.CrAmt = P.Amount;
                    if (jd.DrAmt != 0) jd.DrAmt = P.Amount;
                    jd.Particulars = P.Particulars;
                }
                DB.SaveChanges();
            }

        }
        void Journal_DeleteByPayment(BLL.Payment P)
        {
            var EntryNo = string.Format("PMT-{0}", P.Id);
            DAL.Journal j = DB.Journals.Where(x => x.EntryNo == EntryNo).FirstOrDefault();
            if (j != null) Journal_Delete(j.Id);
        }

        void Journal_SaveByReceipt(BLL.Receipt R)
        {
            var EntryNo = string.Format("RPT-{0}", R.Id);

            DAL.Journal j = DB.Journals.Where(x => x.EntryNo == EntryNo).FirstOrDefault();
            if (j == null)
            {
                var pd = R.RDetails.FirstOrDefault();
                var ld = DB.Ledgers.Where(x => x.Id == pd.LedgerId).FirstOrDefault();

                if (ld.LedgerName.StartsWith("CM-") || ld.LedgerName.StartsWith("WH-") || ld.LedgerName.StartsWith("DL-"))
                {
                    j = new DAL.Journal();
                    j.EntryNo = EntryNo;
                    j.JournalDate = R.ReceiptDate;

                    var CId = CompanyIdByLedgerName(ld.LedgerName);
                    if (CId != 0)
                    {
                        var LName = LedgerNameByCompanyId(Caller.CompanyId);

                        j.JournalDetails.Add(new DAL.JournalDetail()
                        {
                            LedgerId = LedgerIdByKeyAndCompany(BLL.DataKeyValue.CashLedger_Key, CId),
                            DrAmt = R.Amount,
                            Particulars = R.Particulars
                        });
                        j.JournalDetails.Add(new DAL.JournalDetail()
                        {

                            LedgerId = LedgerIdByCompany(LName, CId),
                            CrAmt = R.Amount,
                            Particulars = R.Particulars
                        });
                        DB.Journals.Add(j);
                        DB.SaveChanges();
                    }


                }
            }
            else
            {

                j.JournalDate = R.ReceiptDate;
                foreach (var jd in j.JournalDetails)
                {
                    if (jd.CrAmt != 0) jd.CrAmt = R.Amount;
                    if (jd.DrAmt != 0) jd.DrAmt = R.Amount;
                    jd.Particulars = R.Particulars;
                }
                DB.SaveChanges();
            }

        }
        void Journal_DeleteByReceipt(BLL.Receipt P)
        {
            var EntryNo = string.Format("RPT-{0}", P.Id);
            DAL.Journal j = DB.Journals.Where(x => x.EntryNo == EntryNo).FirstOrDefault();
            if (j != null) Journal_Delete(j.Id);
        }

        void Journal_SaveByStockIn(BLL.StockIn P)
        {
            var RefNo = string.Format("STIN-{0}", P.Id);
            DAL.Journal j = DB.Journals.Where(x => x.EntryNo == RefNo).FirstOrDefault();
            if (j == null)
            {
                j = new DAL.Journal();
                j.EntryNo = P.RefNo;

                j.JournalDetails.Add(new DAL.JournalDetail()
                {
                    LedgerId = P.LedgerId,
                    CrAmt = P.ItemAmount,
                    Particulars = P.Narration
                });

                j.JournalDetails.Add(new DAL.JournalDetail()
                {
                    LedgerId = LedgerIdByKey(BLL.DataKeyValue.Stock_Inward_Ledger_Key),
                    DrAmt = P.ItemAmount,
                    Particulars = P.Narration
                });



                DB.Journals.Add(j);
            }
            else
            {
                foreach (var jd in j.JournalDetails)
                {
                    jd.Particulars = P.Narration;
                    if (jd.CrAmt != 0)
                    {
                        jd.LedgerId = P.LedgerId;
                        jd.CrAmt = P.ItemAmount;
                    }
                    else
                    {
                        jd.DrAmt = P.ItemAmount;
                    }
                }
            }

             j.JournalDate = P.Date;
            DB.SaveChanges();
        }
        void Journal_DeleteByStockIn(BLL.StockIn P)
        {
            var RefNo = string.Format("STIN-{0}", P.Id);
            DAL.Journal j = DB.Journals.Where(x => x.EntryNo == RefNo).FirstOrDefault();
            if (j != null) Journal_Delete(j.Id);
        }

        #region Stock Out
        void Journal_DeleteByStockOut(BLL.StockOut P)
        {
            var RefNo = string.Format("STOUT-{0}", P.Id);
            DAL.Journal j = DB.Journals.Where(x => x.EntryNo == P.RefNo).FirstOrDefault();
            if (j != null) Journal_Delete(j.Id);
        }
        void Journal_SaveByStockOut(BLL.StockOut P)
        {

            DAL.Journal j = DB.Journals.Where(x => x.EntryNo == P.RefNo).FirstOrDefault();
            if (j == null)
            {
                j = new DAL.Journal();
                j.EntryNo = P.RefNo;

                j.JournalDetails.Add(new DAL.JournalDetail()
                {
                    LedgerId = P.LedgerId,
                    CrAmt = P.ItemAmount,
                    Particulars = P.Narration
                });

                j.JournalDetails.Add(new DAL.JournalDetail()
                {
                    LedgerId = LedgerIdByKey(BLL.DataKeyValue.Stock_Outward_Ledger_Key),
                    DrAmt = P.ItemAmount,
                    Particulars = P.Narration
                });



                DB.Journals.Add(j);
            }
            else
            {
                foreach (var jd in j.JournalDetails)
                {
                    jd.Particulars = P.Narration;
                    if (jd.CrAmt != 0)
                    {
                        jd.LedgerId = P.LedgerId;
                        jd.CrAmt = P.ItemAmount;
                    }
                    else
                    {
                        jd.DrAmt =  P.ItemAmount;
                    }
                }
            }

            j.JournalDate = P.Date;
            DB.SaveChanges();
        }
        #endregion

        #endregion
    }
}
