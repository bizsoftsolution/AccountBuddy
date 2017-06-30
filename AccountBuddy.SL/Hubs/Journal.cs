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
        public string Journal_NewRefNo()
        {
            return Journal_NewRefNoByCompanyId(Caller.CompanyId);
        }

        public string Journal_NewRefNoByCompanyId(int CompanyId)
        {
            DateTime dt = DateTime.Now;
            string Prefix = string.Format("{0}{1:yy}{2:X}", BLL.FormPrefix.Journal, dt, dt.Month);
            long No = 0;

            var d = DB.Journals.Where(x => x.JournalDetails.FirstOrDefault().Ledger.AccountGroup.CompanyId == CompanyId && x.EntryNo.StartsWith(Prefix))
                                     .OrderByDescending(x => x.EntryNo)
                                     .FirstOrDefault();

            if (d != null) No = Convert.ToInt64(d.EntryNo.Substring(Prefix.Length), 16);

            return string.Format("{0}{1:X5}", Prefix, No + 1);
        }

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

                Clients.Clients(OtherLoginClientsOnGroup).Journal_RefNoRefresh(Journal_NewRefNo());
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

        #region Purchase
        void Journal_SaveByPurchase(DAL.Purchase P)
        {
            string RefCode = string.Format("{0}{1}", BLL.FormPrefix.Purchase, P.Id);
            var CId = P.Ledger.AccountGroup.CompanyId;

            DAL.Journal j = DB.Journals.Where(x => x.RefCode == RefCode).FirstOrDefault();
            if (j == null)
            {
                j = new DAL.Journal();
                j.EntryNo = Journal_NewRefNoByCompanyId(CId);
                j.RefCode = RefCode;
                j.JournalDetails.Add(new DAL.JournalDetail()
                {
                    LedgerId = P.TransactionTypeId == 1 ? LedgerIdByKeyAndCompany(BLL.DataKeyValue.CashLedger_Key, CId) : P.LedgerId,
                    CrAmt = P.TotalAmount,
                    Particulars = P.Narration
                });

                j.JournalDetails.Add(new DAL.JournalDetail()
                {
                    LedgerId = LedgerIdByKeyAndCompany(BLL.DataKeyValue.PurchaseAccount_Ledger_Key, CId),
                    DrAmt = P.ItemAmount - P.DiscountAmount + P.ExtraAmount,
                    Particulars = P.Narration
                });

                j.JournalDetails.Add(new DAL.JournalDetail()
                {
                    LedgerId = LedgerIdByKeyAndCompany(BLL.DataKeyValue.Output_Tax_Ledger_Key, CId),
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
                        jd.LedgerId = P.TransactionTypeId == 1 ? LedgerIdByKeyAndCompany(BLL.DataKeyValue.CashLedger_Key, CId) : P.LedgerId;
                        jd.CrAmt = P.TotalAmount;
                    }
                    else
                    {
                        jd.DrAmt = jd.LedgerId == LedgerIdByKeyAndCompany(BLL.DataKeyValue.PurchaseAccount_Ledger_Key, CId) ? P.ItemAmount - P.DiscountAmount + P.ExtraAmount : P.GSTAmount;
                    }
                }
            }

            j.JournalDate = P.PurchaseDate;
            DB.SaveChanges();
        }
        void Journal_DeleteByPurchase(BLL.Purchase P)
        {
            string RefCode = string.Format("{0}{1}", BLL.FormPrefix.Purchase, P.Id);
            DAL.Journal j = DB.Journals.Where(x => x.RefCode == RefCode).FirstOrDefault();
            if (j != null) Journal_Delete(j.Id);
        }

        #endregion

        #region sales return 
        void Journal_SaveBySalesReturn(DAL.SalesReturn SR)
        {
            string RefCode = string.Format("{0}{1}", BLL.FormPrefix.SalesReturn, SR.Id);

            var CId = SR.Ledger.AccountGroup.CompanyId;

            DAL.Journal j = DB.Journals.Where(x => x.RefCode == RefCode).FirstOrDefault();
            if (j == null)
            {
                j = new DAL.Journal();
                j.EntryNo = Journal_NewRefNoByCompanyId(CId);
                j.RefCode = RefCode;

                j.JournalDetails.Add(new DAL.JournalDetail()
                {
                    LedgerId = SR.TransactionTypeId == 1 ? LedgerIdByKeyAndCompany(BLL.DataKeyValue.CashLedger_Key, CId) : SR.LedgerId,
                    CrAmt = SR.TotalAmount,
                    Particulars = SR.Narration
                });

                j.JournalDetails.Add(new DAL.JournalDetail()
                {
                    LedgerId = LedgerIdByKeyAndCompany(BLL.DataKeyValue.Sales_Return_Ledger_Key, CId),
                    DrAmt = SR.ItemAmount - SR.DiscountAmount + SR.ExtraAmount,
                    Particulars = SR.Narration
                });

                j.JournalDetails.Add(new DAL.JournalDetail()
                {
                    LedgerId = LedgerIdByKeyAndCompany(BLL.DataKeyValue.Output_Tax_Ledger_Key, CId),
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
                        jd.LedgerId = SR.TransactionTypeId == 1 ? LedgerIdByKeyAndCompany(BLL.DataKeyValue.CashLedger_Key, CId) : SR.LedgerId;
                        jd.CrAmt = SR.TotalAmount;
                    }
                    else
                    {
                        jd.DrAmt = jd.LedgerId == LedgerIdByKeyAndCompany(BLL.DataKeyValue.Sales_Return_Ledger_Key, CId) ? SR.ItemAmount - SR.DiscountAmount + SR.ExtraAmount : SR.GSTAmount;
                    }
                }
            }
            j.JournalDate = SR.SRDate;
            DB.SaveChanges();
        }
        void Journal_DeleteBySalesReturn(BLL.SalesReturn SR)
        {
            string RefCode = string.Format("{0}{1}", BLL.FormPrefix.SalesReturn,SR.Id);
            DAL.Journal j = DB.Journals.Where(x => x.RefCode == RefCode).FirstOrDefault();
            if (j != null) Journal_Delete(j.Id);
        }
        #endregion

        #region Sales

        void Journal_SaveBySales(DAL.Sale S)
        {
            string RefCode = string.Format("{0}{1}", BLL.FormPrefix.Sales, S.Id);

            var CId = S.Ledger.AccountGroup.CompanyId;

            DAL.Journal j = DB.Journals.Where(x => x.RefCode == RefCode).FirstOrDefault();
            if (j == null)
            {
                j = new DAL.Journal();
                j.EntryNo = Journal_NewRefNoByCompanyId(CId);
                j.RefCode = RefCode;
                j.JournalDetails.Add(new DAL.JournalDetail()
                {
                    LedgerId = S.TransactionTypeId == 1 ? LedgerIdByKeyAndCompany(BLL.DataKeyValue.CashLedger_Key, CId) : S.LedgerId,
                    DrAmt = S.TotalAmount,
                    Particulars = S.Narration
                });

                j.JournalDetails.Add(new DAL.JournalDetail()
                {
                    LedgerId = LedgerIdByKeyAndCompany(BLL.DataKeyValue.SalesAccount_Ledger_Key, CId),
                    CrAmt = S.ItemAmount - S.DiscountAmount + S.ExtraAmount,
                    Particulars = S.Narration
                });

                j.JournalDetails.Add(new DAL.JournalDetail()
                {
                    LedgerId = LedgerIdByKeyAndCompany(BLL.DataKeyValue.Input_Tax_Ledger_Key, CId),
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
                        jd.LedgerId = S.TransactionTypeId == 1 ? LedgerIdByKeyAndCompany(BLL.DataKeyValue.CashLedger_Key, CId) : S.LedgerId;
                        jd.DrAmt = S.TotalAmount;
                    }
                    else
                    {
                        jd.CrAmt = jd.LedgerId == LedgerIdByKeyAndCompany(BLL.DataKeyValue.SalesAccount_Ledger_Key, CId) ? S.ItemAmount - S.DiscountAmount + S.ExtraAmount : S.GSTAmount;
                    }
                }
            }
            j.JournalDate = S.SalesDate;
            DB.SaveChanges();
        }
        void Journal_DeleteBySales(BLL.Sale S)
        {
            string RefCode = string.Format("{0}{1}", BLL.FormPrefix.Sales, S.Id);

            DAL.Journal j = DB.Journals.Where(x => x.RefCode == RefCode).FirstOrDefault();
            if (j != null) Journal_Delete(j.Id);
        }
        #endregion

        #region Purchase Return
        void Journal_SaveByPurchaseReturn(DAL.PurchaseReturn PR)
        {
            string RefCode = string.Format("{0}{1}", BLL.FormPrefix.PurchaseReturn, PR.Id);
            var CId = PR.Ledger.AccountGroup.CompanyId;


            DAL.Journal j = DB.Journals.Where(x => x.EntryNo == RefCode).FirstOrDefault();
            if (j == null)
            {
                j = new DAL.Journal();
                j.EntryNo = Journal_NewRefNoByCompanyId(CId);
                j.RefCode = RefCode;
                j.JournalDetails.Add(new DAL.JournalDetail()
                {
                    LedgerId = PR.TransactionTypeId == 1 ? LedgerIdByKeyAndCompany(BLL.DataKeyValue.CashLedger_Key, CId) : PR.LedgerId,
                    DrAmt = PR.TotalAmount,
                    Particulars = PR.Narration
                });

                j.JournalDetails.Add(new DAL.JournalDetail()
                {
                    LedgerId = LedgerIdByKeyAndCompany(BLL.DataKeyValue.Purchase_Return_Ledger_Key, CId),
                    CrAmt = PR.ItemAmount - PR.DiscountAmount + PR.ExtraAmount,
                    Particulars = PR.Narration
                });

                j.JournalDetails.Add(new DAL.JournalDetail()
                {
                    LedgerId = LedgerIdByKeyAndCompany(BLL.DataKeyValue.Input_Tax_Ledger_Key, CId),
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
                        jd.LedgerId = PR.TransactionTypeId == 1 ? LedgerIdByKeyAndCompany(BLL.DataKeyValue.CashLedger_Key, CId) : PR.LedgerId;
                        jd.DrAmt = PR.TotalAmount;
                    }
                    else
                    {
                        jd.CrAmt = jd.LedgerId == LedgerIdByKeyAndCompany(BLL.DataKeyValue.Purchase_Return_Ledger_Key, CId) ? PR.ItemAmount - PR.DiscountAmount + PR.ExtraAmount : PR.GSTAmount;
                    }
                }
            }
            j.JournalDate = PR.PRDate;
            DB.SaveChanges();
        }
        void Journal_DeleteByPurchaseReturn(BLL.PurchaseReturn PR)
        {

            string RefCode = string.Format("{0}{1}", BLL.FormPrefix.PurchaseReturn, PR.Id);
            DAL.Journal j = DB.Journals.Where(x => x.RefCode == RefCode).FirstOrDefault();
            if (j != null) Journal_Delete(j.Id);
        }
        #endregion

        #region Payment
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

        #endregion

        #region Receipt
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
        #endregion

        #region Stockout

        void Journal_SaveByStockOut(DAL.StockOut STout)
        {
            string RefCode = string.Format("{0}{1}", BLL.FormPrefix.StockOut, STout.Id);
            var CId = STout.Ledger.AccountGroup.CompanyId;

            DAL.Journal j = DB.Journals.Where(x => x.RefCode == RefCode).FirstOrDefault();
            if (j == null)
            {
                j = new DAL.Journal();
                j.EntryNo = Journal_NewRefNoByCompanyId(CId);
                j.RefCode = RefCode;


                j.JournalDetails.Add(new DAL.JournalDetail()
                {
                    LedgerId = LedgerIdByKeyAndCompany(BLL.DataKeyValue.Stock_Outward_Ledger_Key, CId),
                    CrAmt = STout.ItemAmount,
                    Particulars = STout.Narration
                });

                j.JournalDetails.Add(new DAL.JournalDetail()
                {
                    LedgerId = STout.LedgerId,
                    DrAmt = STout.ItemAmount,
                    Particulars = STout.Narration
                });

                DB.Journals.Add(j);
            }
            else
            {
                foreach (var jd in j.JournalDetails)
                {
                    jd.Particulars = STout.Narration;
                    if (jd.CrAmt != 0)
                    {
                        jd.LedgerId = STout.LedgerId;
                        jd.CrAmt = STout.ItemAmount;
                    }
                    else
                    {
                        jd.DrAmt = STout.ItemAmount;
                    }
                }
            }

            j.JournalDate = STout.Date;
            DB.SaveChanges();
        }
        void Journal_DeleteByStockOut(BLL.StockOut P)
        {

            string RefCode = string.Format("{0}{1}", BLL.FormPrefix.StockOut, P.Id);
            DAL.Journal j = DB.Journals.Where(x => x.RefCode == RefCode).FirstOrDefault();
            if (j != null) Journal_Delete(j.Id);
        }

        #endregion

        #region Stock In
        void Journal_SaveByStockIn(DAL.StockIn STIn)
        {
            string RefCode = string.Format("{0}{1}", BLL.FormPrefix.StockOut, STIn.Id);
            var CId = STIn.Ledger.AccountGroup.CompanyId;


            DAL.Journal j = DB.Journals.Where(x => x.RefCode == RefCode).FirstOrDefault();
            if (j == null)
            {
                j = new DAL.Journal();
                j.EntryNo = Journal_NewRefNoByCompanyId(CId);
                j.RefCode = RefCode;


                j.JournalDetails.Add(new DAL.JournalDetail()
                {
                    LedgerId = STIn.LedgerId,
                    CrAmt = STIn.ItemAmount,
                    Particulars = STIn.Narration
                });

                j.JournalDetails.Add(new DAL.JournalDetail()
                {
                    LedgerId = LedgerIdByKeyAndCompany(BLL.DataKeyValue.Stock_Inward_Ledger_Key, CId),
                    DrAmt = STIn.ItemAmount,
                    Particulars = STIn.Narration
                });

                DB.Journals.Add(j);
            }
            else
            {
                foreach (var jd in j.JournalDetails)
                {
                    jd.Particulars = STIn.Narration;
                    if (jd.CrAmt != 0)
                    {
                        jd.LedgerId = STIn.LedgerId;
                        jd.CrAmt = STIn.ItemAmount;
                    }
                    else
                    {
                        jd.DrAmt = STIn.ItemAmount;
                    }
                }
            }

            j.JournalDate = STIn.Date;
            DB.SaveChanges();
        }
        void Journal_DeleteByStockIn(BLL.StockIn P)
        {

            string RefCode = string.Format("{0}{1}", BLL.FormPrefix.StockIn, P.Id);
            DAL.Journal j = DB.Journals.Where(x => x.RefCode == RefCode).FirstOrDefault();
            if (j != null) Journal_Delete(j.Id);
        }
        #endregion

        #endregion
    }
}
