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

        public  string Journal_NewRefNoByCompanyId(int CompanyId)
        {
            DateTime dt = DateTime.Now;
            string Prefix = string.Format("{0}{1:yy}{2:X}", BLL.FormPrefix.Journal, dt, dt.Month);
            long No = 0;

            var d = Caller.DB.Journals.Where(x => x.JournalDetails.FirstOrDefault().Ledger.AccountGroup.CompanyId == CompanyId && x.EntryNo.StartsWith(Prefix))
                                     .OrderByDescending(x => x.EntryNo)
                                     .FirstOrDefault();

            if (d != null) No = Convert.ToInt64(d.EntryNo.Substring(Prefix.Length), 16);

            return string.Format("{0}{1:X5}", Prefix, No + 1);
        }

        public bool Journal_Save(BLL.Journal PO)
        {
            try
            {

                DAL.Journal d = Caller.DB.Journals.Where(x => x.Id == PO.Id).FirstOrDefault();

                if (d == null)
                {

                    d = new DAL.Journal();
                    Caller.DB.Journals.Add(d);
                    
                    PO.toCopy<DAL.Journal>(d);
                  
                    foreach (var b_pod in PO.JDetails)
                    {
                        DAL.JournalDetail d_pod = new DAL.JournalDetail();
                        b_pod.toCopy<DAL.JournalDetail>(d_pod);
                        d.JournalDetails.Add(d_pod);
                    }
                    Caller.DB.SaveChanges();
                    PO.Id = d.Id;
                    LogDetailStore(PO, LogDetailType.INSERT);
                }
                else
                {

                    //foreach (var d_SOd in d.JournalDetails.ToList())
                    //{
                    //    BLL.JournalDetail b_SOd = PO.JDetails.Where(x => x.Id == d_SOd.Id).FirstOrDefault();
                    //    if (b_SOd == null) d.JournalDetails.Remove(d_SOd);
                    //}

                    decimal pd = PO.JDetails.Select(X => X.JournalId).FirstOrDefault();
                    Caller.DB.JournalDetails.RemoveRange(d.JournalDetails.Where(x => x.JournalId == pd).ToList());


                    PO.toCopy<DAL.Journal>(d);

                    foreach (var b_pod in PO.JDetails)
                    {
                        //DAL.JournalDetail d_pod = d.JournalDetails.Where(x => x.Id == b_pod.Id).FirstOrDefault();
                        //if (d_pod == null)
                        //{
                        DAL.JournalDetail d_pod = new DAL.JournalDetail();
                            d.JournalDetails.Add(d_pod);
                        //}
                        b_pod.toCopy<DAL.JournalDetail>(d_pod);
                    }
                    Caller.DB.SaveChanges();
                    LogDetailStore(PO, LogDetailType.UPDATE);
                }

                Clients.Clients(OtherLoginClients).Journal_RefNoRefresh(Journal_NewRefNo());
                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public BLL.Journal Journal_Find(string EntryNo)
        {
            BLL.Journal PO = new BLL.Journal();
            try
            {

                DAL.Journal d = Caller.DB.Journals.Where(x => x.EntryNo == EntryNo && x.JournalDetails.FirstOrDefault().Ledger.AccountGroup.CompanyId == Caller.CompanyId).FirstOrDefault();
                Caller.DB.Entry(d).Reload();
                if (d != null)
                {

                    d.toCopy<BLL.Journal>(PO);
                    foreach (var d_pod in d.JournalDetails)
                    {
                        BLL.JournalDetail b_pod = new BLL.JournalDetail();
                        d_pod.toCopy<BLL.JournalDetail>(b_pod);
                        PO.JDetails.Add(b_pod);
                        b_pod.LedgerName = (d_pod.Ledger ?? Caller.DB.Ledgers.Find(d_pod.LedgerId) ?? new DAL.Ledger()).LedgerName;
                    }

                }
            }
            catch (Exception ex) { }
            return PO;
        }
        public BLL.Journal Journal_FindById(int id)
        {
            BLL.Journal PO = new BLL.Journal();
            try
            {

                DAL.Journal d = Caller.DB.Journals.Where(x => x.Id == id && x.JournalDetails.FirstOrDefault().Ledger.AccountGroup.CompanyId == Caller.CompanyId).FirstOrDefault();
                Caller.DB.Entry(d).Reload();
                if (d != null)
                {

                    d.toCopy<BLL.Journal>(PO);
                    foreach (var d_pod in d.JournalDetails)
                    {
                        BLL.JournalDetail b_pod = new BLL.JournalDetail();
                        d_pod.toCopy<BLL.JournalDetail>(b_pod);
                        PO.JDetails.Add(b_pod);
                        b_pod.LedgerName = (d_pod.Ledger ?? Caller.DB.Ledgers.Find(d_pod.LedgerId) ?? new DAL.Ledger()).LedgerName;
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
                DAL.Journal d = Caller.DB.Journals.Where(x => x.Id == pk).FirstOrDefault();

                if (d != null)
                {
                    Caller.DB.JournalDetails.RemoveRange(d.JournalDetails);
                    Caller.DB.Journals.Remove(d);
                    Caller.DB.SaveChanges();
                    LogDetailStore(Journal_DALtoBLL(d), LogDetailType.DELETE);
                }
                Clients.Clients(OtherLoginClients).Journal_RefNoRefresh(Journal_NewRefNo());

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
            DAL.Journal d = Caller.DB.Journals.Where(x => x.EntryNo == entryNo & x.Id != PO.Id && x.JournalDetails.FirstOrDefault().Ledger.AccountGroup.CompanyId == Caller.CompanyId).FirstOrDefault();
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

        public  int LedgerIdByKeyAndCompany(string key, int CompanyId)
        {
            return Caller.DB.DataKeyValues.Where(x => x.CompanyId == CompanyId && x.DataKey == key).FirstOrDefault().DataValue;
        }
        int LedgerIdByCompany(string LName, int CompanyId)
        {
            var l = Caller.DB.Ledgers.Where(x => x.LedgerName == LName && x.AccountGroup.CompanyId == CompanyId).FirstOrDefault();
            return l == null ? 0 : l.Id;
        }
        int CompanyIdByLedgerName(string LedgerName)
        {
            var CName = LedgerName.Substring(3);
            var cm = Caller.DB.CompanyDetails.Where(x => x.CompanyName == CName).FirstOrDefault();
            return cm == null ? 0 : cm.Id;
        }
        string LedgerNameByCompanyId(int CompanyId)
        {
            var cm = Caller.DB.CompanyDetails.Where(x => x.Id == CompanyId).FirstOrDefault();
            return string.Format("{0}-{1}", cm.CompanyType == "Company" ? "CM" : (cm.CompanyType == "Warehouse" ? "WH" : "DL"), cm.CompanyName);
        }

        #region Purchase
        void Journal_SaveByPurchase(DAL.Purchase P)
        {
            string RefCode = string.Format("{0}{1}", BLL.FormPrefix.Purchase, P.Id);
            var CId = P.Ledger.AccountGroup.CompanyId;
            string Mode = null, status = null;
            if (P.TransactionTypeId == 1)
            {
                Mode = "Cash";

            }
            else if (P.TransactionTypeId == 1)
            {
                Mode = "Credit";
            }
            else
            {
                Mode = "Cheque";
                status = "Process";
            }
            DAL.Journal j = Caller.DB.Journals.Where(x => x.RefCode == RefCode).FirstOrDefault();
            if (j == null)
            {
                j = new DAL.Journal();
                j.EntryNo = P.RefNo;// Journal_NewRefNoByCompanyId(CId);
                j.RefCode = RefCode;
                if (P.TransactionTypeId == 1)
                {
                    j.JournalDetails.Add(new DAL.JournalDetail()
                    {
                        LedgerId = LedgerIdByKeyAndCompany(BLL.DataKeyValue.CashLedger_Key, CId),
                        CrAmt = P.TotalAmount,
                        Particulars = P.Narration,
                        TransactionMode = "Cash"
                    });
                }
                else if (P.TransactionTypeId == 2)
                {
                    j.JournalDetails.Add(new DAL.JournalDetail()
                    {
                        LedgerId = P.LedgerId,
                        CrAmt = P.TotalAmount,
                        Particulars = P.Narration,
                        TransactionMode = "Credit"
                    });
                }
                else
                {
                    j.JournalDetails.Add(new DAL.JournalDetail()
                    {
                        LedgerId = Caller.DB.Banks.FirstOrDefault().LedgerId,
                        CrAmt = P.TotalAmount,
                        TransactionMode = "Cheque",
                        Particulars = P.Narration,
                        ChequeDate = P.ChequeDate,
                        ChequeNo = P.ChequeNo,
                        Status = "Process"

                    });
                }

                j.JournalDetails.Add(new DAL.JournalDetail()
                {
                    LedgerId = LedgerIdByKeyAndCompany(BLL.DataKeyValue.PurchaseAccount_Ledger_Key, CId),
                    DrAmt = P.ItemAmount - P.DiscountAmount + P.ExtraAmount,
                    TransactionMode = Mode,
                    Particulars = P.Narration,
                    ChequeDate = P.ChequeDate,
                    ChequeNo = P.ChequeNo,
                    Status = status
                });

                j.JournalDetails.Add(new DAL.JournalDetail()
                {
                    LedgerId = LedgerIdByKeyAndCompany(BLL.DataKeyValue.Output_Tax_Ledger_Key, CId),
                    DrAmt = P.GSTAmount,
                    TransactionMode = Mode,
                    Particulars = P.Narration,
                    ChequeDate = P.ChequeDate,
                    ChequeNo = P.ChequeNo,
                    Status = status
                });

                Caller.DB.Journals.Add(j);
            }
            else
            {
                foreach (var jd in j.JournalDetails)
                {
                    jd.Particulars = P.Narration;
                    if (jd.CrAmt != 0)
                    {
                        if (P.TransactionTypeId == 1)
                        {
                            jd.LedgerId = LedgerIdByKeyAndCompany(BLL.DataKeyValue.CashLedger_Key, CId);
                            jd.CrAmt = P.TotalAmount;
                            jd.TransactionMode = "Cash";
                            jd.Particulars = P.Narration;
                        }
                        else if (P.TransactionTypeId == 2)
                        {
                            jd.LedgerId = P.LedgerId;
                            jd.CrAmt = P.TotalAmount;
                            jd.TransactionMode = "Credit";
                            jd.Particulars = P.Narration;
                        }
                        else
                        {
                            jd.LedgerId = Caller.DB.Banks.FirstOrDefault().LedgerId;
                            jd.CrAmt = P.TotalAmount;
                            jd.TransactionMode = "Cheque";
                            jd.ChequeDate = P.ChequeDate;
                            jd.ChequeNo = P.ChequeNo;
                            jd.Status = status;
                            jd.Particulars = P.Narration;
                        }
                    }
                    else
                    {
                        jd.DrAmt = jd.LedgerId == LedgerIdByKeyAndCompany(BLL.DataKeyValue.PurchaseAccount_Ledger_Key, CId) ? P.ItemAmount - P.DiscountAmount + P.ExtraAmount : P.GSTAmount;
                        jd.TransactionMode = Mode;
                        jd.ChequeDate = P.ChequeDate;
                        jd.ChequeNo = P.ChequeNo;
                        jd.Status = status;
                        jd.Particulars = P.Narration;
                    }
                }
            }

            j.JournalDate = P.PurchaseDate;
            Caller.DB.SaveChanges();
        }
        void Journal_DeleteByPurchase(BLL.Purchase P)
        {
            string RefCode = string.Format("{0}{1}", BLL.FormPrefix.Purchase, P.Id);
            DAL.Journal j = Caller.DB.Journals.Where(x => x.RefCode == RefCode).FirstOrDefault();
            if (j != null) Journal_Delete(j.Id);
        }

        #endregion

        #region sales return 
        public  void Journal_SaveBySalesReturn(DAL.SalesReturn SR)
        {
            string RefCode = string.Format("{0}{1}", BLL.FormPrefix.SalesReturn, SR.Id);
            string Mode, status = null;
            var CId = SR.Ledger.AccountGroup.CompanyId;
            if (SR.TransactionTypeId == 1)
            {
                Mode = "Cash";
               
            }
            else if (SR.TransactionTypeId == 1)
            {
                Mode = "Credit";
            }
            else
            {
                Mode = "Cheque";
                status = "Process";
            }
            DAL.Journal j = Caller.DB.Journals.Where(x => x.RefCode == RefCode).FirstOrDefault();
            if (j == null)
            {
                j = new DAL.Journal();
                j.EntryNo = SR.RefNo;//Journal_NewRefNoByCompanyId(CId);
                j.RefCode = RefCode;
               
                if (SR.TransactionTypeId == 1)
                {
                    j.JournalDetails.Add(new DAL.JournalDetail()
                    {
                        LedgerId = LedgerIdByKeyAndCompany(BLL.DataKeyValue.CashLedger_Key, CId),
                        CrAmt = SR.TotalAmount,
                        Particulars = SR.Narration,
                        TransactionMode = "Cash"
                    });
                }
                else if (SR.TransactionTypeId == 2)
                {
                    j.JournalDetails.Add(new DAL.JournalDetail()
                    {
                        LedgerId = SR.LedgerId,
                        CrAmt = SR.TotalAmount,
                        Particulars = SR.Narration,
                        TransactionMode = "Credit"
                    });
                }
                else
                {
                    j.JournalDetails.Add(new DAL.JournalDetail()
                    {
                        LedgerId = Caller.DB.Banks.FirstOrDefault().LedgerId,
                        CrAmt = SR.TotalAmount,
                        TransactionMode = "Cheque",
                        Particulars = SR.Narration,
                        ChequeDate = SR.ChequeDate,
                        ChequeNo = SR.ChequeNo,
                        Status = "Process"

                    });
                }
                j.JournalDetails.Add(new DAL.JournalDetail()
                {
                    LedgerId = LedgerIdByKeyAndCompany(BLL.DataKeyValue.Sales_Return_Ledger_Key, CId),
                    DrAmt = SR.ItemAmount - SR.DiscountAmount + SR.ExtraAmount,
                    Particulars = SR.Narration,
                    TransactionMode = Mode,
                    ChequeDate = SR.ChequeDate,
                    ChequeNo = SR.ChequeNo,
                    Status=status

                });

                j.JournalDetails.Add(new DAL.JournalDetail()
                {
                    LedgerId = LedgerIdByKeyAndCompany(BLL.DataKeyValue.Output_Tax_Ledger_Key, CId),
                    DrAmt = SR.GSTAmount,
                    Particulars = SR.Narration,
                    TransactionMode = Mode,
                    ChequeDate = SR.ChequeDate,
                    ChequeNo = SR.ChequeNo,
                    Status = status
                });

                Caller.DB.Journals.Add(j);
            }
            else
            {
                foreach (var jd in j.JournalDetails)
                {
                    jd.Particulars = SR.Narration;
                    if (jd.CrAmt != 0)
                    {
                        if (SR.TransactionTypeId == 1)
                        {
                            jd.LedgerId = LedgerIdByKeyAndCompany(BLL.DataKeyValue.CashLedger_Key, CId);
                            jd.CrAmt = SR.TotalAmount;
                            jd.TransactionMode = "Cash";
                            jd.Particulars = SR.Narration;
                        }
                        else if (SR.TransactionTypeId == 2)
                        {
                            jd.LedgerId = SR.LedgerId;
                            jd.CrAmt = SR.TotalAmount;
                            jd.TransactionMode = "Credit";
                            jd.Particulars = SR.Narration;
                        }
                        else
                        {
                            jd.LedgerId = Caller.DB.Banks.FirstOrDefault().LedgerId;
                            jd.CrAmt = SR.TotalAmount;
                            jd.TransactionMode = "Cheque";
                            jd.ChequeDate = SR.ChequeDate;
                            jd.ChequeNo = SR.ChequeNo;
                            jd.Status = status;
                            jd.Particulars = SR.Narration;
                        }
                    }
                    else
                    {
                        jd.DrAmt = jd.LedgerId == LedgerIdByKeyAndCompany(BLL.DataKeyValue.Sales_Return_Ledger_Key, CId) ? SR.ItemAmount - SR.DiscountAmount + SR.ExtraAmount : SR.GSTAmount;
                        jd.TransactionMode = Mode;
                        jd.ChequeDate = SR.ChequeDate;
                        jd.ChequeNo = SR.ChequeNo;
                        jd.Status = status;
                        jd.Particulars = SR.Narration;
                    }
                }
            }
            j.JournalDate = SR.SRDate;
            Caller.DB.SaveChanges();
        }
        void Journal_DeleteBySalesReturn(BLL.SalesReturn SR)
        {
            string RefCode = string.Format("{0}{1}", BLL.FormPrefix.SalesReturn, SR.Id);
            DAL.Journal j = Caller.DB.Journals.Where(x => x.RefCode == RefCode).FirstOrDefault();
            if (j != null) Journal_Delete(j.Id);
        }
        #endregion

        #region Sales

        public  void Journal_SaveBySales(DAL.Sale S)
        {
            string RefCode = string.Format("{0}{1}", BLL.FormPrefix.Sales, S.Id);
            string Mode=null, status = null;
            var l = Caller.DB.Ledgers.Where(x => x.Id == S.LedgerId).FirstOrDefault();
            var CId = l.AccountGroup.CompanyId;
            if (S.TransactionTypeId == 1)
            {
                Mode = "Cash";
               
            }
            else if (S.TransactionTypeId == 1)
            {
                Mode = "Credit";
            }
            else
            {
                Mode = "Cheque";
                status = "Process";
            }
            DAL.Journal j = Caller.DB.Journals.Where(x => x.RefCode == RefCode).FirstOrDefault();
            if (j == null)
            {
                j = new DAL.Journal();
                j.EntryNo = S.RefNo;//Journal_NewRefNoByCompanyId(CId);
                j.RefCode = RefCode;
               
                if (S.TransactionTypeId == 1)
                {
                    j.JournalDetails.Add(new DAL.JournalDetail()
                    {
                        LedgerId = LedgerIdByKeyAndCompany(BLL.DataKeyValue.CashLedger_Key, CId),
                        DrAmt = S.TotalAmount,
                        TransactionMode = "Cash",
                        Particulars = S.Narration
                    });
                }
                else if (S.TransactionTypeId == 2)
                {
                    j.JournalDetails.Add(new DAL.JournalDetail()
                    {
                        LedgerId = S.LedgerId,
                        DrAmt = S.TotalAmount,
                        TransactionMode = "Credit",
                        Particulars = S.Narration
                    });
                }
                else
                {
                    j.JournalDetails.Add(new DAL.JournalDetail()
                    {
                        LedgerId = Caller.DB.Banks.FirstOrDefault().LedgerId,
                        DrAmt = S.TotalAmount,
                        Particulars = S.Narration,
                        TransactionMode = "Cheque",
                        ChequeDate = S.ChequeDate,
                        ChequeNo = S.ChequeNo,
                        Status = "Process"
                    });
                }
                j.JournalDetails.Add(new DAL.JournalDetail()
                {
                    LedgerId = LedgerIdByKeyAndCompany(BLL.DataKeyValue.SalesAccount_Ledger_Key, CId),
                    CrAmt = S.ItemAmount - S.DiscountAmount + S.ExtraAmount,
                    TransactionMode = Mode,
                    ChequeDate = S.ChequeDate,
                    ChequeNo = S.ChequeNo,
                    Status = status,
                    Particulars = S.Narration
                });

                j.JournalDetails.Add(new DAL.JournalDetail()
                {
                    LedgerId = LedgerIdByKeyAndCompany(BLL.DataKeyValue.Input_Tax_Ledger_Key, CId),
                    CrAmt = S.GSTAmount,
                    TransactionMode = Mode,
                    ChequeDate = S.ChequeDate,
                    ChequeNo = S.ChequeNo,
                    Status = status,
                    Particulars = S.Narration
                });

                Caller.DB.Journals.Add(j);
            }
            else
            {
                foreach (var jd in j.JournalDetails)
                {
                    jd.Particulars = S.Narration;
                    if (jd.DrAmt != 0)
                    {
                        if (S.TransactionTypeId == 1)
                        {
                            jd.LedgerId = LedgerIdByKeyAndCompany(BLL.DataKeyValue.CashLedger_Key, CId);
                            jd.DrAmt = S.TotalAmount;
                            jd.TransactionMode = "Cash";
                            jd.Particulars = S.Narration;
                        }
                        else if (S.TransactionTypeId == 2)
                        {
                            jd.LedgerId = S.LedgerId;
                            jd.DrAmt = S.TotalAmount;
                            jd.TransactionMode = "Credit";
                            jd.Particulars = S.Narration;
                        }
                        else
                        {
                            jd.LedgerId = Caller.DB.Banks.FirstOrDefault().LedgerId;
                            jd.DrAmt = S.TotalAmount;
                            jd.TransactionMode = "Cheque";
                            jd.Particulars = S.Narration;
                            jd.ChequeDate = S.ChequeDate;
                            jd.ChequeNo = S.ChequeNo;
                            jd.Status = status;
                        }
                    }
                    else
                    {
                        jd.CrAmt = jd.LedgerId == LedgerIdByKeyAndCompany(BLL.DataKeyValue.SalesAccount_Ledger_Key, CId) ? S.ItemAmount - S.DiscountAmount + S.ExtraAmount : S.GSTAmount;
                       
                        jd.TransactionMode = Mode;
                        jd.ChequeDate = S.ChequeDate;
                        jd.ChequeNo = S.ChequeNo;
                        jd.Status = status;
                        jd.Particulars = S.Narration;
                    }
                }
            }
            j.JournalDate = S.SalesDate;
            Caller.DB.SaveChanges();
        }
        void Journal_DeleteBySales(BLL.Sale S)
        {
            string RefCode = string.Format("{0}{1}", BLL.FormPrefix.Sales, S.Id);

            DAL.Journal j = Caller.DB.Journals.Where(x => x.RefCode == RefCode).FirstOrDefault();
            if (j != null) Journal_Delete(j.Id);
        }
        #endregion

        #region Purchase Return
        public   void Journal_SaveByPurchaseReturn(DAL.PurchaseReturn PR)
        {
            string RefCode = string.Format("{0}{1}", BLL.FormPrefix.PurchaseReturn, PR.Id);
            string Mode, status = null;
            var CId = PR.Ledger.AccountGroup.CompanyId;
            if (PR.TransactionTypeId == 1)
            {
                Mode = "Cash";

            }
            else if (PR.TransactionTypeId == 1)
            {
                Mode = "Credit";
            }
            else
            {
                Mode = "Cheque";
                status = "Process";
            }
            DAL.Journal j = Caller.DB.Journals.Where(x => x.EntryNo == RefCode).FirstOrDefault();
            if (j == null)
            {
                j = new DAL.Journal();
                j.EntryNo = PR.RefNo;//Journal_NewRefNoByCompanyId(CId);
                j.RefCode = RefCode;
                if (PR.TransactionTypeId == 1)
                {
                    j.JournalDetails.Add(new DAL.JournalDetail()
                    {
                        LedgerId = LedgerIdByKeyAndCompany(BLL.DataKeyValue.CashLedger_Key, CId),
                        DrAmt = PR.TotalAmount,
                        Particulars = PR.Narration,
                        TransactionMode = "Cash"
                    });
                }
                else if (PR.TransactionTypeId == 2)
                {
                    j.JournalDetails.Add(new DAL.JournalDetail()
                    {
                        LedgerId = PR.LedgerId,
                        DrAmt = PR.TotalAmount,
                        Particulars = PR.Narration,
                        TransactionMode = "Credit"
                    });
                }
                else
                {
                    j.JournalDetails.Add(new DAL.JournalDetail()
                    {
                        LedgerId = Caller.DB.Banks.FirstOrDefault().LedgerId,
                        DrAmt = PR.TotalAmount,
                        TransactionMode = "Cheque",
                        Particulars = PR.Narration,
                        ChequeDate = PR.ChequeDate,
                        ChequeNo = PR.ChequeNo,
                        Status = "Process"

                    });
                }
                j.JournalDetails.Add(new DAL.JournalDetail()
                {
                    LedgerId = LedgerIdByKeyAndCompany(BLL.DataKeyValue.Purchase_Return_Ledger_Key, CId),
                    CrAmt = PR.ItemAmount - PR.DiscountAmount + PR.ExtraAmount,
                    Particulars = PR.Narration,
                    TransactionMode = Mode,
                    ChequeDate = PR.ChequeDate,
                    ChequeNo = PR.ChequeNo,
                    Status = status

                });

                j.JournalDetails.Add(new DAL.JournalDetail()
                {
                    LedgerId = LedgerIdByKeyAndCompany(BLL.DataKeyValue.Input_Tax_Ledger_Key, CId),
                    CrAmt = PR.GSTAmount,
                    Particulars = PR.Narration,
                    TransactionMode = Mode,
                    ChequeDate = PR.ChequeDate,
                    ChequeNo = PR.ChequeNo,
                    Status = status
                });


                Caller.DB.Journals.Add(j);
            }
            else
            {
                foreach (var jd in j.JournalDetails)
                {
                    jd.Particulars = PR.Narration;
                    if (jd.DrAmt != 0)
                    {
                        if (PR.TransactionTypeId == 1)
                        {
                            jd.LedgerId = LedgerIdByKeyAndCompany(BLL.DataKeyValue.CashLedger_Key, CId);
                            jd.DrAmt = PR.TotalAmount;
                            jd.TransactionMode = "Cash";
                            jd.Particulars = PR.Narration;
                        }
                        else if (PR.TransactionTypeId == 2)
                        {
                            jd.LedgerId = PR.LedgerId;
                            jd.DrAmt = PR.TotalAmount;
                            jd.TransactionMode = "Credit";
                            jd.Particulars = PR.Narration;
                        }
                        else
                        {
                            jd.LedgerId = Caller.DB.Banks.FirstOrDefault().LedgerId;
                            jd.DrAmt = PR.TotalAmount;
                            jd.TransactionMode = "Cheque";
                            jd.Particulars = PR.Narration;
                            jd.ChequeDate = PR.ChequeDate;
                            jd.ChequeNo = PR.ChequeNo;
                            jd.Status = status;
                        }
                    }
                    else
                    {
                        jd.CrAmt = jd.LedgerId == LedgerIdByKeyAndCompany(BLL.DataKeyValue.Purchase_Return_Ledger_Key, CId) ?PR.ItemAmount - PR.DiscountAmount + PR.ExtraAmount : PR.GSTAmount;

                        jd.TransactionMode = Mode;
                        jd.ChequeDate = PR.ChequeDate;
                        jd.ChequeNo = PR.ChequeNo;
                        jd.Status = status;
                        jd.Particulars = PR.Narration;
                    }
                }
            }
            j.JournalDate = PR.PRDate;
            Caller.DB.SaveChanges();
        }
        void Journal_DeleteByPurchaseReturn(BLL.PurchaseReturn PR)
        {

            string RefCode = string.Format("{0}{1}", BLL.FormPrefix.PurchaseReturn, PR.Id);
            DAL.Journal j = Caller.DB.Journals.Where(x => x.RefCode == RefCode).FirstOrDefault();
            if (j != null) Journal_Delete(j.Id);
        }
        #endregion

        #region Payment
        void Journal_SaveByPayment(BLL.Payment P)
        {
            var EntryNo = string.Format("PMT-{0}", P.Id);

            DAL.Journal j = Caller.DB.Journals.Where(x => x.EntryNo == EntryNo).FirstOrDefault();
            if (j == null)
            {
                var pd = P.PDetails.FirstOrDefault();
                var ld = Caller.DB.Ledgers.Where(x => x.Id == pd.LedgerId).FirstOrDefault();

                if (ld.LedgerName.StartsWith("CM-") || ld.LedgerName.StartsWith("WH-") || ld.LedgerName.StartsWith("DL-"))
                {
                    j = new DAL.Journal();
                    j.EntryNo = P.EntryNo;
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
                        Caller.DB.Journals.Add(j);
                        Caller.DB.SaveChanges();
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
                Caller.DB.SaveChanges();
            }

        }
        void Journal_DeleteByPayment(BLL.Payment P)
        {
            var EntryNo = string.Format("PMT-{0}", P.Id);
            DAL.Journal j = Caller.DB.Journals.Where(x => x.EntryNo == EntryNo).FirstOrDefault();
            if (j != null) Journal_Delete(j.Id);
        }

        #endregion

        #region Receipt
        void Journal_SaveByReceipt(BLL.Receipt R)
        {
            var EntryNo = string.Format("RPT-{0}", R.Id);

            DAL.Journal j = Caller.DB.Journals.Where(x => x.EntryNo == EntryNo).FirstOrDefault();
            if (j == null)
            {
                var pd = R.RDetails.FirstOrDefault();
                var ld = Caller.DB.Ledgers.Where(x => x.Id == pd.LedgerId).FirstOrDefault();

                if (ld.LedgerName.StartsWith("CM-") || ld.LedgerName.StartsWith("WH-") || ld.LedgerName.StartsWith("DL-"))
                {
                    j = new DAL.Journal();
                    j.EntryNo = R.EntryNo;
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
                        Caller.DB.Journals.Add(j);
                        Caller.DB.SaveChanges();
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
                Caller.DB.SaveChanges();
            }

        }
        void Journal_DeleteByReceipt(BLL.Receipt P)
        {
            var EntryNo = string.Format("RPT-{0}", P.Id);
            DAL.Journal j = Caller.DB.Journals.Where(x => x.EntryNo == EntryNo).FirstOrDefault();
            if (j != null) Journal_Delete(j.Id);
        }
        #endregion

        #region Stockout

        void Journal_SaveByStockOut(DAL.StockOut STout)
        {
            string RefCode = string.Format("{0}{1}", BLL.FormPrefix.StockOut, STout.Id);
            var CId = STout.Ledger.AccountGroup.CompanyId;

            DAL.Journal j = Caller.DB.Journals.Where(x => x.RefCode == RefCode).FirstOrDefault();
            if (j == null)
            {
                j = new DAL.Journal();
                j.EntryNo = STout.RefNo;// Journal_NewRefNoByCompanyId(CId);
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

                Caller.DB.Journals.Add(j);
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
            Caller.DB.SaveChanges();
        }
        void Journal_DeleteByStockOut(BLL.StockOut P)
        {

            string RefCode = string.Format("{0}{1}", BLL.FormPrefix.StockOut, P.Id);
            DAL.Journal j = Caller.DB.Journals.Where(x => x.RefCode == RefCode).FirstOrDefault();
            if (j != null) Journal_Delete(j.Id);
        }

        #endregion

        #region Stock In
        void Journal_SaveByStockIn(DAL.StockIn STIn)
        {
            string RefCode = string.Format("{0}{1}", BLL.FormPrefix.StockIn, STIn.Id);
            var CId = STIn.Ledger.AccountGroup.CompanyId;


            DAL.Journal j = Caller.DB.Journals.Where(x => x.RefCode == RefCode).FirstOrDefault();
            if (j == null)
            {
                j = new DAL.Journal();
                j.EntryNo = STIn.RefNo;// Journal_NewRefNoByCompanyId(CId);
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

                Caller.DB.Journals.Add(j);
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
            Caller.DB.SaveChanges();
        }
        void Journal_DeleteByStockIn(BLL.StockIn P)
        {

            string RefCode = string.Format("{0}{1}", BLL.FormPrefix.StockIn, P.Id);
            DAL.Journal j = Caller.DB.Journals.Where(x => x.RefCode == RefCode).FirstOrDefault();
            if (j != null) Journal_Delete(j.Id);
        }
        #endregion

        #region JobOrderIssue
        void Journal_SaveByJobOrderIssue(DAL.JobOrderIssue P)
        {
            string RefCode = string.Format("{0}{1}", BLL.FormPrefix.JobOrderIssue, P.Id);

            var LId = Caller.DB.JobWorkers.Where(x => x.Id == P.JobWorkerId).FirstOrDefault().Ledger;
             var CId = LId.AccountGroup.CompanyId;

            DAL.Journal j = Caller.DB.Journals.Where(x => x.RefCode == RefCode).FirstOrDefault();
            if (j == null)
            {
                j = new DAL.Journal();
                j.EntryNo = P.RefNo;// Journal_NewRefNoByCompanyId(CId);
                j.RefCode = RefCode;
                j.JournalDetails.Add(new DAL.JournalDetail()
                {
                    LedgerId = P.JobWorker.Ledger.Id,
                    CrAmt = P.TotalAmount,
                    Particulars = P.Narration
                });

                j.JournalDetails.Add(new DAL.JournalDetail()
                {
                    LedgerId = LedgerIdByKeyAndCompany(BLL.DataKeyValue.JobOrderIssued_Ledger_Key, CId),
                    DrAmt = P.ItemAmount - P.DiscountAmount + P.Extras,
                    Particulars = P.Narration
                });

                j.JournalDetails.Add(new DAL.JournalDetail()
                {
                    LedgerId = LedgerIdByKeyAndCompany(BLL.DataKeyValue.Output_Tax_Ledger_Key, CId),
                    DrAmt = P.GSTAmount,
                    Particulars = P.Narration
                });

                Caller.DB.Journals.Add(j);
            }
            else
            {
                foreach (var jd in j.JournalDetails.ToList())
                {
                    jd.Particulars = P.Narration;
                    if (jd.CrAmt != 0)
                    {
                        jd.LedgerId = P.JobWorker.Ledger.Id;
                        jd.CrAmt = P.TotalAmount;
                    }
                    else
                    {
                        jd.DrAmt = jd.LedgerId == LedgerIdByKeyAndCompany(BLL.DataKeyValue.JobOrderIssued_Ledger_Key, CId) ? P.ItemAmount - P.DiscountAmount + P.Extras : P.GSTAmount;
                    }
                }
            }

            j.JournalDate = P.JODate;
            Caller.DB.SaveChanges();
        }
        void Journal_DeleteByJobOrderIssue(BLL.JobOrderIssue P)
        {
            string RefCode = string.Format("{0}{1}", BLL.FormPrefix.JobOrderIssue, P.Id);
            DAL.Journal j = Caller.DB.Journals.Where(x => x.RefCode == RefCode).FirstOrDefault();
            if (j != null) Journal_Delete(j.Id);
        }

        #endregion

        #region JobOrderReceived

        void Journal_SaveByJobOrderReceived(DAL.JobOrderReceived S)
        {
            string RefCode = string.Format("{0}{1}", BLL.FormPrefix.JobOrderReceived, S.Id);

            var CId = S.JobWorker.Ledger.AccountGroup.CompanyId;

            DAL.Journal j = Caller.DB.Journals.Where(x => x.RefCode == RefCode).FirstOrDefault();
            if (j == null)
            {
                j = new DAL.Journal();
                j.EntryNo = S.RefNo;// Journal_NewRefNoByCompanyId(CId);
                j.RefCode = RefCode;
                j.JournalDetails.Add(new DAL.JournalDetail()
                {
                    LedgerId = S.JobWorker.Ledger.Id,
                    CrAmt = S.TotalAmount,
                    Particulars = S.Narration
                });

                j.JournalDetails.Add(new DAL.JournalDetail()
                {
                    LedgerId = LedgerIdByKeyAndCompany(BLL.DataKeyValue.JobOrderReceived_Ledger_Key, CId),
                    DrAmt = S.ItemAmount - S.DiscountAmount + S.ExtraAmount,
                    Particulars = S.Narration
                });

                j.JournalDetails.Add(new DAL.JournalDetail()
                {
                    LedgerId = LedgerIdByKeyAndCompany(BLL.DataKeyValue.Output_Tax_Ledger_Key, CId),
                    DrAmt = S.GSTAmount,
                    Particulars = S.Narration
                });

                Caller.DB.Journals.Add(j);
            }
            else
            {
                foreach (var jd in j.JournalDetails)
                {
                    jd.Particulars = S.Narration;
                    if (jd.DrAmt != 0)
                    {
                        jd.LedgerId = S.JobWorker.Ledger.Id;
                        jd.CrAmt = S.TotalAmount;
                    }
                    else
                    {
                        jd.DrAmt = jd.LedgerId == LedgerIdByKeyAndCompany(BLL.DataKeyValue.JobOrderReceived_Ledger_Key, CId) ? S.ItemAmount - S.DiscountAmount + S.ExtraAmount : S.GSTAmount;
                    }
                }
            }
            j.JournalDate = S.JRDate;
            Caller.DB.SaveChanges();
        }
        void Journal_DeleteByJobOrderReceived(BLL.JobOrderReceived S)
        {
            string RefCode = string.Format("{0}{1}", BLL.FormPrefix.JobOrderReceived, S.Id);

            DAL.Journal j = Caller.DB.Journals.Where(x => x.RefCode == RefCode).FirstOrDefault();
            if (j != null) Journal_Delete(j.Id);
        }
        #endregion

        #region StockInProcess

        void Journal_SaveByStockInProcess(DAL.StockInProcess S)
        {
            string RefCode = string.Format("{0}{1}", BLL.FormPrefix.StockInProcess, S.Id);

            var CId = S.Staff.Ledger.AccountGroup.CompanyId;

            DAL.Journal j = Caller.DB.Journals.Where(x => x.RefCode == RefCode).FirstOrDefault();
            if (j == null)
            {
                j = new DAL.Journal();
                j.EntryNo = S.RefNo;// Journal_NewRefNoByCompanyId(CId);
                j.RefCode = RefCode;
                j.JournalDetails.Add(new DAL.JournalDetail()
                {
                    LedgerId = S.Staff.Ledger.Id,
                    DrAmt = S.TotalAmount,
                    Particulars = S.Narration
                });

                j.JournalDetails.Add(new DAL.JournalDetail()
                {
                    LedgerId = LedgerIdByKeyAndCompany(BLL.DataKeyValue.StockInProcess_Ledger_Key, CId),
                    CrAmt = S.ItemAmount - S.DiscountAmount + S.Extras,
                    Particulars = S.Narration
                });

                j.JournalDetails.Add(new DAL.JournalDetail()
                {
                    LedgerId = LedgerIdByKeyAndCompany(BLL.DataKeyValue.Input_Tax_Ledger_Key, CId),
                    CrAmt = S.GSTAmount,
                    Particulars = S.Narration
                });

                Caller.DB.Journals.Add(j);
            }
            else
            {
                foreach (var jd in j.JournalDetails)
                {
                    jd.Particulars = S.Narration;
                    if (jd.DrAmt != 0)
                    {
                        jd.LedgerId = S.Staff.Ledger.Id;
                        jd.DrAmt = S.TotalAmount;
                    }
                    else
                    {
                        jd.CrAmt = jd.LedgerId == LedgerIdByKeyAndCompany(BLL.DataKeyValue.StockInProcess_Ledger_Key, CId) ? S.ItemAmount - S.DiscountAmount + S.Extras : S.GSTAmount;
                    }
                }
            }
            j.JournalDate = S.SPDate;
            Caller.DB.SaveChanges();
        }
        void Journal_DeleteByStockInProcess(BLL.StockInProcess S)
        {
            string RefCode = string.Format("{0}{1}", BLL.FormPrefix.StockInProcess, S.Id);

            DAL.Journal j = Caller.DB.Journals.Where(x => x.RefCode == RefCode).FirstOrDefault();
            if (j != null) Journal_Delete(j.Id);
        }
        #endregion
        #region StockSeparated

        void Journal_SaveByStockSeparated(DAL.StockSeparated S)
        {
            string RefCode = string.Format("{0}{1}", BLL.FormPrefix.StockSeparated, S.Id);

            var CId = S.Staff.Ledger.AccountGroup.CompanyId;

            DAL.Journal j = Caller.DB.Journals.Where(x => x.RefCode == RefCode).FirstOrDefault();
            if (j == null)
            {
                j = new DAL.Journal();
                j.EntryNo = S.RefNo;// Journal_NewRefNoByCompanyId(CId);
                j.RefCode = RefCode;
                j.JournalDetails.Add(new DAL.JournalDetail()
                {
                    LedgerId = S.Staff.Ledger.Id,
                    DrAmt = S.TotalAmount,
                    Particulars = S.Narration
                });

                j.JournalDetails.Add(new DAL.JournalDetail()
                {
                    LedgerId = LedgerIdByKeyAndCompany(BLL.DataKeyValue.StockSeperated_Ledger_Key, CId),
                    CrAmt = S.ItemAmount - S.DiscountAmount + S.ExtraAmount,
                    Particulars = S.Narration
                });

                j.JournalDetails.Add(new DAL.JournalDetail()
                {
                    LedgerId = LedgerIdByKeyAndCompany(BLL.DataKeyValue.Input_Tax_Ledger_Key, CId),
                    CrAmt = S.GSTAmount,
                    Particulars = S.Narration
                });

                Caller.DB.Journals.Add(j);
            }
            else
            {
                foreach (var jd in j.JournalDetails)
                {
                    jd.Particulars = S.Narration;
                    if (jd.DrAmt != 0)
                    {
                        jd.LedgerId = S.Staff.Ledger.Id;
                        jd.DrAmt = S.TotalAmount;
                    }
                    else
                    {
                        jd.CrAmt = jd.LedgerId == LedgerIdByKeyAndCompany(BLL.DataKeyValue.StockInProcess_Ledger_Key, CId) ? S.ItemAmount - S.DiscountAmount + S.ExtraAmount : S.GSTAmount;
                    }
                }
            }
            j.JournalDate = S.Date;
            Caller.DB.SaveChanges();
        }
        void Journal_DeleteByStockSeparated(BLL.StockSeperated S)
        {
            string RefCode = string.Format("{0}{1}", BLL.FormPrefix.StockSeparated, S.Id);

            DAL.Journal j = Caller.DB.Journals.Where(x => x.RefCode == RefCode).FirstOrDefault();
            if (j != null) Journal_Delete(j.Id);
        }
        #endregion

        public List<BLL.Journal> Journal_List(int? LedgerId, DateTime dtFrom, DateTime dtTo, string EntryNo, string Status, decimal amtFrom, decimal amtTo)
        {
            Caller.DB = new DAL.DBFMCGEntities();
            List<BLL.Journal> lstJournal = new List<BLL.Journal>();
            BLL.Journal rp = new BLL.Journal();
            try
            {
                foreach (var l1 in Caller.DB.JournalDetails.
                      Where(x => x.Journal.JournalDate >= dtFrom && x.Journal.JournalDate <= dtTo
                      && (x.LedgerId == LedgerId || LedgerId == null)
                      && (EntryNo == "" || x.Journal.EntryNo == EntryNo)
                      && (Status == "" || x.Status == Status) &&
                      ((x.CrAmt <= amtTo && x.CrAmt >= amtFrom)
                     || (x.DrAmt <= amtTo && x.DrAmt >= amtFrom))&&
                      x.Ledger.AccountGroup.CompanyId == Caller.CompanyId).ToList())
                {
                    rp = new BLL.Journal();
                    rp.JDetail.CrAmt = l1.CrAmt;
                    rp.JDetail.DrAmt = l1.DrAmt;
                    rp.EntryNo = l1.Journal.EntryNo;
                    rp.Id = l1.Journal.Id;
                    rp.JDetail.LedgerId = l1.LedgerId;
                    rp.JDetail.LedgerName = string.Format("{0}-{1}", l1.Ledger.AccountGroup.GroupCode, l1.Ledger.LedgerName);
                    rp.Particular = l1.Journal.Particular;
                    rp.JournalDate = l1.Journal.JournalDate;
                    rp.JDetail.TransactionMode = l1.TransactionMode;
                    rp.RefCode = l1.Journal.RefCode;
                    rp.VoucherNo = l1.Journal.VoucherNo;
                    lstJournal.Add(rp);

                    lstJournal = lstJournal.OrderBy(x => x.JournalDate).ToList();
                }

            }
            catch (Exception ex) { }
            return lstJournal;
        }


        #endregion
    }
}
