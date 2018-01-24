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

                    PO.ToMap(d);

                    foreach (var b_pod in PO.JDetails)
                    {
                        DAL.JournalDetail d_pod = new DAL.JournalDetail();
                        b_pod.ToMap(d_pod);
                        d.JournalDetails.Add(d_pod);
                    }
                    DB.SaveChanges();
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
                    DB.JournalDetails.RemoveRange(d.JournalDetails.Where(x => x.JournalId == pd).ToList());


                    PO.ToMap(d);

                    foreach (var b_pod in PO.JDetails)
                    {
                        //DAL.JournalDetail d_pod = d.JournalDetails.Where(x => x.Id == b_pod.Id).FirstOrDefault();
                        //if (d_pod == null)
                        //{
                        DAL.JournalDetail d_pod = new DAL.JournalDetail();
                        d.JournalDetails.Add(d_pod);
                        //}
                        b_pod.ToMap(d_pod);
                    }
                    DB.SaveChanges();
                    LogDetailStore(PO, LogDetailType.UPDATE);
                }

                if (OtherClientsOnGroup.Count > 0) Clients.Clients(OtherClientsOnGroup).Journal_RefNoRefresh(Journal_NewRefNo());
                return true;
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return false;
        }

        public BLL.Journal Journal_Find(string EntryNo)
        {
            BLL.Journal PO = new BLL.Journal();
            try
            {

                DAL.Journal d = DB.Journals.Where(x => x.EntryNo == EntryNo && x.JournalDetails.FirstOrDefault().Ledger.AccountGroup.CompanyId == Caller.CompanyId).FirstOrDefault();
                DB.Entry(d).Reload();
                if (d != null)
                {

                    d.ToMap(PO);
                    int i = 0;
                    foreach (var d_pod in d.JournalDetails)
                    {
                        BLL.JournalDetail b_pod = new BLL.JournalDetail();
                        d_pod.ToMap(b_pod);
                        PO.JDetails.Add(b_pod);
                        b_pod.SNo = ++i;
                        b_pod.LedgerName = (d_pod.Ledger ?? DB.Ledgers.Find(d_pod.LedgerId) ?? new DAL.Ledger()).LedgerName;
                    }

                }
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return PO;
        }
        public BLL.Journal Journal_FindById(int id)
        {
            BLL.Journal PO = new BLL.Journal();
            try
            {

                DAL.Journal d = DB.Journals.Where(x => x.Id == id && x.JournalDetails.FirstOrDefault().Ledger.AccountGroup.CompanyId == Caller.CompanyId).FirstOrDefault();
                DB.Entry(d).Reload();
                if (d != null)
                {

                    d.ToMap(PO);
                    foreach (var d_pod in d.JournalDetails)
                    {
                        BLL.JournalDetail b_pod = new BLL.JournalDetail();
                        d_pod.ToMap(b_pod);
                        PO.JDetails.Add(b_pod);
                        b_pod.LedgerName = (d_pod.Ledger ?? DB.Ledgers.Find(d_pod.LedgerId) ?? new DAL.Ledger()).LedgerName;
                    }

                }
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
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
                if (OtherClientsOnGroup.Count > 0) Clients.Clients(OtherClientsOnGroup).Journal_RefNoRefresh(Journal_NewRefNo());

                return true;
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return false;
        }

        public BLL.Journal Journal_DALtoBLL(DAL.Journal d)
        {
            BLL.Journal J = d.ToMap(new BLL.Journal());
            foreach (var d_Jd in d.JournalDetails)
            {
                J.JDetails.Add(d_Jd.ToMap(new BLL.JournalDetail()));
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

        public int LedgerIdByKeyAndCompany(string key, int CompanyId)
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

        int TaxIdByCompany_LedgerId(int CompanyId, int LId)
        {
            var l = DB.TaxMasters.Where(x => x.LedgerId == LId && x.Ledger.AccountGroup.CompanyId == CompanyId).FirstOrDefault();
            return l == null ? 0 : l.Id;
        }

        decimal TaxPercentByCompany_LedgerId(int CompanyId, int LId)
        {
            var l = DB.TaxMasters.Where(x => x.LedgerId == LId && x.Ledger.AccountGroup.CompanyId == CompanyId).FirstOrDefault();
            return l == null ? 0 : l.TaxPercentage;
        }
        #region Purchase
        void Journal_SaveByPurchase(DAL.Purchase P, List<BLL.TaxMaster> TaxDetail)
        {
            try
            {
                string RefCode = string.Format("{0}{1}", BLL.FormPrefix.Purchase, P.Id);
                var CId = DB.Ledgers.Where(X => X.Id == P.LedgerId).FirstOrDefault().AccountGroup.CompanyId;
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
                DAL.Journal j = DB.Journals.Where(x => x.RefCode == RefCode).FirstOrDefault();
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
                            DrAmt = P.TotalAmount,
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
                            LedgerId = DB.Banks.FirstOrDefault().LedgerId,
                            DrAmt = P.TotalAmount,
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
                        CrAmt = P.ItemAmount - P.DiscountAmount + P.ExtraAmount,
                        TransactionMode = Mode,
                        Particulars = P.Narration,
                        ChequeDate = P.ChequeDate,
                        ChequeNo = P.ChequeNo,
                        Status = status
                    });
                    foreach (var t in TaxDetail)
                    {
                        j.JournalDetails.Add(new DAL.JournalDetail()
                        {
                            LedgerId = t.Ledger.Id,
                            CrAmt = t.TaxAmount,
                            TransactionMode = Mode,
                            ChequeDate = P.ChequeDate,
                            ChequeNo = P.ChequeNo,
                            Status = status,
                            Particulars = P.Narration
                        });
                    }
                    DB.Journals.Add(j);
                }
                else
                {
                    if (j.JournalDetails.Count != TaxDetail.Count)
                    {
                        foreach (var s in j.JournalDetails.ToList())
                        {
                            decimal pd = j.JournalDetails.Select(X => X.JournalId).FirstOrDefault();
                            DB.JournalDetails.RemoveRange(j.JournalDetails.Where(x => x.JournalId == pd).ToList());
                        }
                        if (P.TransactionTypeId == 1)
                        {
                            j.JournalDetails.Add(new DAL.JournalDetail()
                            {
                                LedgerId = LedgerIdByKeyAndCompany(BLL.DataKeyValue.CashLedger_Key, CId),
                                DrAmt = P.TotalAmount,
                                Particulars = P.Narration,
                                TransactionMode = "Cash"
                            });
                        }
                        else if (P.TransactionTypeId == 2)
                        {
                            j.JournalDetails.Add(new DAL.JournalDetail()
                            {
                                LedgerId = P.LedgerId,
                                DrAmt = P.TotalAmount,
                                Particulars = P.Narration,
                                TransactionMode = "Credit"
                            });
                        }
                        else
                        {
                            j.JournalDetails.Add(new DAL.JournalDetail()
                            {
                                LedgerId = DB.Banks.FirstOrDefault().LedgerId,
                                DrAmt = P.TotalAmount,
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
                            CrAmt = P.ItemAmount - P.DiscountAmount + P.ExtraAmount,
                            TransactionMode = Mode,
                            Particulars = P.Narration,
                            ChequeDate = P.ChequeDate,
                            ChequeNo = P.ChequeNo,
                            Status = status
                        });
                        foreach (var t in TaxDetail)
                        {
                            j.JournalDetails.Add(new DAL.JournalDetail()
                            {
                                LedgerId = t.Ledger.Id,
                                CrAmt = t.TaxAmount,
                                TransactionMode = Mode,
                                ChequeDate = P.ChequeDate,
                                ChequeNo = P.ChequeNo,
                                Status = status,
                                Particulars = P.Narration
                            });
                        }
                        DB.SaveChanges();
                    }
                    else
                    {
                        foreach (var jd in j.JournalDetails)
                        {
                            jd.Particulars = P.Narration;
                            if (jd.DrAmt != 0)
                            {
                                if (P.TransactionTypeId == 1)
                                {
                                    jd.LedgerId = LedgerIdByKeyAndCompany(BLL.DataKeyValue.CashLedger_Key, CId);
                                    jd.DrAmt = P.TotalAmount;
                                    jd.TransactionMode = "Cash";
                                    jd.Particulars = P.Narration;
                                }
                                else if (P.TransactionTypeId == 2)
                                {
                                    jd.LedgerId = P.LedgerId;
                                    jd.DrAmt = P.TotalAmount;
                                    jd.TransactionMode = "Credit";
                                    jd.Particulars = P.Narration;
                                }
                                else
                                {
                                    jd.LedgerId = DB.Banks.FirstOrDefault().LedgerId;
                                    jd.DrAmt = P.TotalAmount;
                                    jd.TransactionMode = "Cheque";
                                    jd.ChequeDate = P.ChequeDate;
                                    jd.ChequeNo = P.ChequeNo;
                                    jd.Status = status;
                                    jd.Particulars = P.Narration;
                                }
                            }
                            else
                            {
                                if (jd.LedgerId == LedgerIdByKeyAndCompany(BLL.DataKeyValue.PurchaseAccount_Key, CId))
                                {
                                    jd.CrAmt = P.ItemAmount - P.DiscountAmount + P.ExtraAmount;
                                }
                                else
                                {
                                    if (P.LedgerId == TaxIdByCompany_LedgerId(Caller.CompanyId, jd.LedgerId))
                                    {
                                        jd.CrAmt = TaxDetail.Where(x => x.Ledger.Id == P.LedgerId).Select(x => x.TaxAmount).FirstOrDefault();
                                    }

                                }
                                jd.TransactionMode = Mode;
                                jd.ChequeDate = P.ChequeDate;
                                jd.ChequeNo = P.ChequeNo;
                                jd.Status = status;
                                jd.Particulars = P.Narration;
                            }
                        }
                    }
                }
                j.JournalDate = P.PurchaseDate;
                DB.SaveChanges();
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex);
            }
        }
        void Journal_DeleteByPurchase(BLL.Purchase P)
        {
            string RefCode = string.Format("{0}{1}", BLL.FormPrefix.Purchase, P.Id);
            DAL.Journal j = DB.Journals.Where(x => x.RefCode == RefCode).FirstOrDefault();
            if (j != null) Journal_Delete(j.Id);
        }

        #endregion

        #region sales return 
        public void Journal_SaveBySalesReturn(DAL.SalesReturn SR, List<BLL.TaxMaster> TaxDetail)
        {
            string RefCode = string.Format("{0}{1}", BLL.FormPrefix.SalesReturn, SR.Id);
            string Mode, status = null;
            if (SR.Ledger == null)
            {
                SR.Ledger = DB.Ledgers.Where(x => x.Id == SR.LedgerId).FirstOrDefault();
            }
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
            DAL.Journal j = DB.Journals.Where(x => x.EntryNo == SR.RefNo).FirstOrDefault();
            if (j == null)
            {
                j = new DAL.Journal();
                j.EntryNo = SR.RefNo;//Journal_NewRefNoByCompanyId(CId);
                j.RefCode = RefCode;
                j.Particular = SR.Narration;

                if (SR.TransactionTypeId == 1)
                {
                    j.JournalDetails.Add(new DAL.JournalDetail()
                    {
                        LedgerId = LedgerIdByKeyAndCompany(BLL.DataKeyValue.CashLedger_Key, CId),
                        DrAmt = SR.TotalAmount,
                        Particulars = SR.Narration,
                        TransactionMode = "Cash"
                    });
                }
                else if (SR.TransactionTypeId == 2)
                {
                    j.JournalDetails.Add(new DAL.JournalDetail()
                    {
                        LedgerId = SR.LedgerId,
                        DrAmt = SR.TotalAmount,
                        Particulars = SR.Narration,
                        TransactionMode = "Credit"
                    });
                }
                else
                {
                    j.JournalDetails.Add(new DAL.JournalDetail()
                    {
                        LedgerId = DB.Banks.FirstOrDefault().LedgerId,
                        DrAmt = SR.TotalAmount,
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
                    CrAmt = SR.ItemAmount - SR.DiscountAmount + SR.ExtraAmount,
                    TransactionMode = Mode,
                    Particulars = SR.Narration,
                    ChequeDate = SR.ChequeDate,
                    ChequeNo = SR.ChequeNo,
                    Status = status
                });
                foreach (var t in TaxDetail)
                {
                    j.JournalDetails.Add(new DAL.JournalDetail()
                    {
                        LedgerId = t.Ledger.Id,
                        CrAmt = t.TaxAmount,
                        TransactionMode = Mode,
                        ChequeDate = SR.ChequeDate,
                        ChequeNo = SR.ChequeNo,
                        Status = status,
                        Particulars = SR.Narration
                    });
                }

                DB.Journals.Add(j);
            }
            else
            {
                if (j.JournalDetails.Count != TaxDetail.Count)
                {
                    foreach (var s in j.JournalDetails.ToList())
                    {
                        decimal pd = j.JournalDetails.Select(X => X.JournalId).FirstOrDefault();
                        DB.JournalDetails.RemoveRange(j.JournalDetails.Where(x => x.JournalId == pd).ToList());
                    }
                    if (SR.TransactionTypeId == 1)
                    {
                        j.JournalDetails.Add(new DAL.JournalDetail()
                        {
                            LedgerId = LedgerIdByKeyAndCompany(BLL.DataKeyValue.CashLedger_Key, CId),
                            DrAmt = SR.TotalAmount,
                            Particulars = SR.Narration,
                            TransactionMode = "Cash"
                        });
                    }
                    else if (SR.TransactionTypeId == 2)
                    {
                        j.JournalDetails.Add(new DAL.JournalDetail()
                        {
                            LedgerId = SR.LedgerId,
                            DrAmt = SR.TotalAmount,
                            Particulars = SR.Narration,
                            TransactionMode = "Credit"
                        });
                    }
                    else
                    {
                        j.JournalDetails.Add(new DAL.JournalDetail()
                        {
                            LedgerId = DB.Banks.FirstOrDefault().LedgerId,
                            DrAmt = SR.TotalAmount,
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
                        CrAmt = SR.ItemAmount - SR.DiscountAmount + SR.ExtraAmount,
                        TransactionMode = Mode,
                        Particulars = SR.Narration,
                        ChequeDate = SR.ChequeDate,
                        ChequeNo = SR.ChequeNo,
                        Status = status
                    });
                    foreach (var t in TaxDetail)
                    {
                        j.JournalDetails.Add(new DAL.JournalDetail()
                        {
                            LedgerId = t.Ledger.Id,
                            CrAmt = t.TaxAmount,
                            TransactionMode = Mode,
                            ChequeDate = SR.ChequeDate,
                            ChequeNo = SR.ChequeNo,
                            Status = status,
                            Particulars = SR.Narration
                        });
                    }
                    DB.SaveChanges();
                }
                else
                {
                    foreach (var jd in j.JournalDetails)
                    {
                        jd.Particulars = SR.Narration;
                        if (jd.DrAmt != 0)
                        {
                            if (SR.TransactionTypeId == 1)
                            {
                                jd.LedgerId = LedgerIdByKeyAndCompany(BLL.DataKeyValue.CashLedger_Key, CId);
                                jd.DrAmt = SR.TotalAmount;
                                jd.TransactionMode = "Cash";
                                jd.Particulars = SR.Narration;
                            }
                            else if (SR.TransactionTypeId == 2)
                            {
                                jd.LedgerId = SR.LedgerId;
                                jd.DrAmt = SR.TotalAmount;
                                jd.TransactionMode = "Credit";
                                jd.Particulars = SR.Narration;
                            }
                            else
                            {
                                jd.LedgerId = DB.Banks.FirstOrDefault().LedgerId;
                                jd.DrAmt = SR.TotalAmount;
                                jd.TransactionMode = "Cheque";
                                jd.ChequeDate = SR.ChequeDate;
                                jd.ChequeNo = SR.ChequeNo;
                                jd.Status = status;
                                jd.Particulars = SR.Narration;
                            }
                        }
                        else
                        {
                            if (jd.LedgerId == LedgerIdByKeyAndCompany(BLL.DataKeyValue.Sales_Return_Ledger_Key, CId))
                            {
                                jd.CrAmt = SR.ItemAmount - SR.DiscountAmount + SR.ExtraAmount;
                            }
                            else
                            {
                                if (SR.LedgerId == TaxIdByCompany_LedgerId(Caller.CompanyId, jd.LedgerId))
                                {
                                    jd.CrAmt = TaxDetail.Where(x => x.Ledger.Id == SR.LedgerId).Select(x => x.TaxAmount).FirstOrDefault();
                                }
                            }
                            jd.TransactionMode = Mode;
                            jd.ChequeDate = SR.ChequeDate;
                            jd.ChequeNo = SR.ChequeNo;
                            jd.Status = status;
                            jd.Particulars = SR.Narration;
                        }
                    }
                }
            }
            j.JournalDate = SR.SRDate;
            DB.SaveChanges();
        }
        void Journal_DeleteBySalesReturn(BLL.SalesReturn SR)
        {
            DAL.Journal j = DB.Journals.Where(x => x.EntryNo == SR.RefNo).FirstOrDefault();
            if (j != null) Journal_Delete(j.Id);
        }
        #endregion

        #region Sales

        public void Journal_SaveBySales(DAL.Sale S, List<BLL.TaxMaster> TaxDetails)
        {
            string RefCode = string.Format("{0}{1}", BLL.FormPrefix.Sales, S.Id);
            string Mode = null, status = null;
            var CId = DB.Ledgers.Where(x => x.Id == S.LedgerId).FirstOrDefault().AccountGroup.CompanyId; ;
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
            DAL.Journal j = DB.Journals.Where(x => x.RefCode == RefCode).FirstOrDefault();
            if (j == null)
            {
                j = new DAL.Journal();
                j.EntryNo = S.RefNo;//Journal_NewRefNoByCompanyId(CId);
                j.RefCode = RefCode;
                j.JournalDate = S.SalesDate;
                if (S.TransactionTypeId == 1)
                {
                    j.JournalDetails.Add(new DAL.JournalDetail()
                    {
                        LedgerId = LedgerIdByKeyAndCompany(BLL.DataKeyValue.CashLedger_Key, CId),
                        CrAmt = S.TotalAmount,
                        TransactionMode = "Cash",
                        Particulars = S.Narration
                    });
                }
                else if (S.TransactionTypeId == 2)
                {
                    j.JournalDetails.Add(new DAL.JournalDetail()
                    {
                        LedgerId = S.LedgerId,
                        CrAmt = S.TotalAmount,
                        TransactionMode = "Credit",
                        Particulars = S.Narration
                    });
                }
                else
                {
                    j.JournalDetails.Add(new DAL.JournalDetail()
                    {
                        LedgerId = DB.Banks.FirstOrDefault().LedgerId,
                        CrAmt = S.TotalAmount,
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
                    DrAmt = S.ItemAmount - S.DiscountAmount + S.ExtraAmount,
                    TransactionMode = Mode,
                    ChequeDate = S.ChequeDate,
                    ChequeNo = S.ChequeNo,
                    Status = status,
                    Particulars = S.Narration
                });

                foreach (var t in TaxDetails)
                {
                    j.JournalDetails.Add(new DAL.JournalDetail()
                    {
                        LedgerId = t.Ledger.Id,
                        DrAmt = t.TaxAmount,
                        TransactionMode = Mode,
                        ChequeDate = S.ChequeDate,
                        ChequeNo = S.ChequeNo,
                        Status = status,
                        Particulars = S.Narration
                    });

                }

                DB.Journals.Add(j);
                DB.SaveChanges();
            }
            else
            {
                if (j.JournalDetails.Count != TaxDetails.Count)
                {
                    foreach (var s in j.JournalDetails.ToList())
                    {
                        decimal pd = j.JournalDetails.Select(X => X.JournalId).FirstOrDefault();
                        DB.JournalDetails.RemoveRange(j.JournalDetails.Where(x => x.JournalId == pd).ToList());
                    }
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
                            LedgerId = DB.Banks.FirstOrDefault().LedgerId,
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
                    foreach (var t in TaxDetails)
                    {
                        j.JournalDetails.Add(new DAL.JournalDetail()
                        {
                            LedgerId = t.Ledger.Id,
                            CrAmt = t.TaxAmount,
                            TransactionMode = Mode,
                            ChequeDate = S.ChequeDate,
                            ChequeNo = S.ChequeNo,
                            Status = status,
                            Particulars = S.Narration
                        });

                    }
                    DB.SaveChanges();
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
                                jd.LedgerId = DB.Banks.FirstOrDefault().LedgerId;
                                jd.DrAmt = S.TotalAmount;
                                jd.TransactionMode = "Cheque";
                                jd.Particulars = S.Narration;
                                jd.ChequeDate = S.ChequeDate;
                                jd.ChequeNo = S.ChequeNo;
                                jd.Status = status;
                            }
                        }
                        if (jd.CrAmt != 0)
                        {
                            if (jd.LedgerId == LedgerIdByKeyAndCompany(BLL.DataKeyValue.SalesAccount_Ledger_Key, CId))
                            {
                                jd.CrAmt = S.ItemAmount - S.DiscountAmount + S.ExtraAmount;
                            }
                            else
                            {
                                if (S.LedgerId == TaxIdByCompany_LedgerId(Caller.CompanyId, jd.LedgerId))
                                {
                                    jd.CrAmt = TaxDetails.Where(x => x.Ledger.Id == S.LedgerId).Select(x => x.TaxAmount).FirstOrDefault();
                                }

                            }
                            jd.TransactionMode = Mode;
                            jd.ChequeDate = S.ChequeDate;
                            jd.ChequeNo = S.ChequeNo;
                            jd.Status = status;
                            jd.Particulars = S.Narration;
                        }
                    }
                    j.JournalDate = S.SalesDate;
                    DB.SaveChanges();
                }
            }
        }
        void Journal_DeleteBySales(BLL.Sale S)
        {
            string RefCode = string.Format("{0}{1}", BLL.FormPrefix.Sales, S.Id);

            DAL.Journal j = DB.Journals.Where(x => x.RefCode == RefCode).FirstOrDefault();
            if (j != null) Journal_Delete(j.Id);
        }
        #endregion

        #region Purchase Return
        public void Journal_SaveByPurchaseReturn(DAL.PurchaseReturn PR, List<BLL.TaxMaster> TaxDetails)
        {
            string RefCode = string.Format("{0}{1}", BLL.FormPrefix.PurchaseReturn, PR.Id);
            string Mode, status = null;

            var CId = DB.Ledgers.Where(x => x.Id == PR.LedgerId).FirstOrDefault().AccountGroup.CompanyId;
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
            DAL.Journal j = DB.Journals.Where(x => x.EntryNo == PR.RefNo).FirstOrDefault();
            if (j == null)
            {
                j = new DAL.Journal();
                j.EntryNo = PR.RefNo;//Journal_NewRefNoByCompanyId(CId);
                j.RefCode = RefCode;
                j.Particular = PR.Narration;
                if (PR.TransactionTypeId == 1)
                {
                    j.JournalDetails.Add(new DAL.JournalDetail()
                    {
                        LedgerId = LedgerIdByKeyAndCompany(BLL.DataKeyValue.CashLedger_Key, CId),
                        CrAmt = PR.TotalAmount,
                        Particulars = PR.Narration,
                        TransactionMode = "Cash"
                    });
                }
                else if (PR.TransactionTypeId == 2)
                {
                    j.JournalDetails.Add(new DAL.JournalDetail()
                    {
                        LedgerId = PR.LedgerId,
                        CrAmt = PR.TotalAmount,
                        Particulars = PR.Narration,
                        TransactionMode = "Credit"
                    });
                }
                else
                {
                    j.JournalDetails.Add(new DAL.JournalDetail()
                    {
                        LedgerId = DB.Banks.FirstOrDefault().LedgerId,
                        CrAmt = PR.TotalAmount,
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
                    DrAmt = PR.ItemAmount - PR.DiscountAmount + PR.ExtraAmount,
                    Particulars = PR.Narration,
                    TransactionMode = Mode,
                    ChequeDate = PR.ChequeDate,
                    ChequeNo = PR.ChequeNo,
                    Status = status

                });

                foreach (var t in TaxDetails)
                {
                    j.JournalDetails.Add(new DAL.JournalDetail()
                    {
                        LedgerId = t.Ledger.Id,
                        DrAmt = t.TaxAmount,
                        TransactionMode = Mode,
                        ChequeDate = PR.ChequeDate,
                        ChequeNo = PR.ChequeNo,
                        Status = status,
                        Particulars = PR.Narration
                    });
                }

                DB.Journals.Add(j);
            }
            else
            {
                if (j.JournalDetails.Count != TaxDetails.Count)
                {
                    foreach (var s in j.JournalDetails.ToList())
                    {
                        decimal pd = j.JournalDetails.Select(X => X.JournalId).FirstOrDefault();
                        DB.JournalDetails.RemoveRange(j.JournalDetails.Where(x => x.JournalId == pd).ToList());
                    }
                    if (PR.TransactionTypeId == 1)
                    {
                        j.JournalDetails.Add(new DAL.JournalDetail()
                        {
                            LedgerId = LedgerIdByKeyAndCompany(BLL.DataKeyValue.CashLedger_Key, CId),
                            CrAmt = PR.TotalAmount,
                            TransactionMode = "Cash",
                            Particulars = PR.Narration
                        });
                    }
                    else if (PR.TransactionTypeId == 2)
                    {
                        j.JournalDetails.Add(new DAL.JournalDetail()
                        {
                            LedgerId = PR.LedgerId,
                            CrAmt = PR.TotalAmount,
                            TransactionMode = "Credit",
                            Particulars = PR.Narration
                        });
                    }
                    else
                    {
                        j.JournalDetails.Add(new DAL.JournalDetail()
                        {
                            LedgerId = DB.Banks.FirstOrDefault().LedgerId,
                            CrAmt = PR.TotalAmount,
                            Particulars = PR.Narration,
                            TransactionMode = "Cheque",
                            ChequeDate = PR.ChequeDate,
                            ChequeNo = PR.ChequeNo,
                            Status = "Process"
                        });
                    }
                    j.JournalDetails.Add(new DAL.JournalDetail()
                    {
                        LedgerId = LedgerIdByKeyAndCompany(BLL.DataKeyValue.Purchase_Return_Ledger_Key, CId),
                        DrAmt = PR.ItemAmount - PR.DiscountAmount + PR.ExtraAmount,
                        TransactionMode = Mode,
                        ChequeDate = PR.ChequeDate,
                        ChequeNo = PR.ChequeNo,
                        Status = status,
                        Particulars = PR.Narration
                    });
                    foreach (var t in TaxDetails)
                    {
                        j.JournalDetails.Add(new DAL.JournalDetail()
                        {
                            LedgerId = t.Ledger.Id,
                            DrAmt = t.TaxAmount,
                            TransactionMode = Mode,
                            ChequeDate = PR.ChequeDate,
                            ChequeNo = PR.ChequeNo,
                            Status = status,
                            Particulars = PR.Narration
                        });

                    }
                    DB.SaveChanges();
                }
                else
                {
                    foreach (var jd in j.JournalDetails)
                    {
                        jd.Particulars = PR.Narration;
                        if (jd.CrAmt != 0)
                        {
                            if (PR.TransactionTypeId == 1)
                            {
                                jd.LedgerId = LedgerIdByKeyAndCompany(BLL.DataKeyValue.CashLedger_Key, CId);
                                jd.CrAmt = PR.TotalAmount;
                                jd.TransactionMode = "Cash";
                                jd.Particulars = PR.Narration;
                            }
                            else if (PR.TransactionTypeId == 2)
                            {
                                jd.LedgerId = PR.LedgerId;
                                jd.CrAmt = PR.TotalAmount;
                                jd.TransactionMode = "Credit";
                                jd.Particulars = PR.Narration;
                            }
                            else
                            {
                                jd.LedgerId = DB.Banks.FirstOrDefault().LedgerId;
                                jd.CrAmt = PR.TotalAmount;
                                jd.TransactionMode = "Cheque";
                                jd.Particulars = PR.Narration;
                                jd.ChequeDate = PR.ChequeDate;
                                jd.ChequeNo = PR.ChequeNo;
                                jd.Status = status;
                            }
                        }
                        if (jd.DrAmt != 0)
                        {
                            if (jd.LedgerId == LedgerIdByKeyAndCompany(BLL.DataKeyValue.Purchase_Return_Ledger_Key, CId))
                            {
                                jd.DrAmt = PR.ItemAmount - PR.DiscountAmount + PR.ExtraAmount;
                            }
                            else
                            {
                                if (PR.LedgerId == TaxIdByCompany_LedgerId(Caller.CompanyId, jd.LedgerId))
                                {
                                    jd.DrAmt = TaxDetails.Where(x => x.Ledger.Id == PR.LedgerId).Select(x => x.TaxAmount).FirstOrDefault();
                                }

                            }
                            jd.TransactionMode = Mode;
                            jd.ChequeDate = PR.ChequeDate;
                            jd.ChequeNo = PR.ChequeNo;
                            jd.Status = status;
                            jd.Particulars = PR.Narration;
                        }
                    }
                }
            }
            j.JournalDate = PR.PRDate;
            DB.SaveChanges();
        }
        void Journal_DeleteByPurchaseReturn(BLL.PurchaseReturn PR)
        {

            DAL.Journal j = DB.Journals.Where(x => x.EntryNo == PR.RefNo).FirstOrDefault();
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
                    var CId = CompanyIdByLedgerName(ld.LedgerName);
                    j = new DAL.Journal();
                    j.EntryNo = Journal_NewRefNoByCompanyId(CId);
                    j.JournalDate = P.PaymentDate;
                    j.RefCode = P.EntryNo;
                    if (CId != 0)
                    {
                        var LName = LedgerNameByCompanyId(Caller.CompanyId);

                        j.JournalDetails.Add(new DAL.JournalDetail()
                        {
                            LedgerId = LedgerIdByKeyAndCompany(BLL.DataKeyValue.CashLedger_Key, CId),
                            DrAmt = P.Amount,
                            TransactionMode = "Cash",
                            Particulars = P.Particulars
                        });
                        j.JournalDetails.Add(new DAL.JournalDetail()
                        {

                            LedgerId = LedgerIdByCompany(LName, CId),
                            CrAmt = P.Amount,
                            TransactionMode = "Cash",
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
            DAL.Journal j = DB.Journals.Where(x => x.EntryNo == P.EntryNo).FirstOrDefault();
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
                    var CId = CompanyIdByLedgerName(ld.LedgerName);
                    j = new DAL.Journal();
                    j.EntryNo = Journal_NewRefNoByCompanyId(CId);
                    j.JournalDate = R.ReceiptDate;
                    j.RefCode = R.EntryNo;
                    if (CId != 0)
                    {
                        var LName = LedgerNameByCompanyId(Caller.CompanyId);

                        j.JournalDetails.Add(new DAL.JournalDetail()
                        {
                            LedgerId = LedgerIdByKeyAndCompany(BLL.DataKeyValue.CashLedger_Key, CId),
                            CrAmt = R.Amount,
                            TransactionMode = "Cash",
                            Particulars = R.Particulars
                        });
                        j.JournalDetails.Add(new DAL.JournalDetail()
                        {

                            LedgerId = LedgerIdByCompany(LName, CId),
                            DrAmt = R.Amount,
                            TransactionMode = "Cash",
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
            DAL.Journal j = DB.Journals.Where(x => x.EntryNo == P.EntryNo).FirstOrDefault();
            if (j != null) Journal_Delete(j.Id);
        }
        #endregion

        #region Stockout

        void Journal_SaveByStockOut(DAL.StockOut STout)
        {
            try
            {
                string RefCode = string.Format("{0}{1}", BLL.FormPrefix.StockOut, STout.Id);
                if (STout.Ledger == null)
                {
                    STout.Ledger = DB.Ledgers.Where(x => x.Id == STout.LedgerId).FirstOrDefault();
                }
                var CId = STout.Ledger.AccountGroup.CompanyId;

                DAL.Journal j = DB.Journals.Where(x => x.RefCode == RefCode).FirstOrDefault();
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
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex);
            }
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

            try
            {
                string RefCode = string.Format("{0}{1}", BLL.FormPrefix.StockIn, STIn.Id);
                if (STIn.Ledger == null)
                {
                    STIn.Ledger = DB.Ledgers.Where(x => x.Id == STIn.LedgerId).FirstOrDefault();
                }
                var CId = STIn.Ledger.AccountGroup.CompanyId;


                DAL.Journal j = DB.Journals.Where(x => x.RefCode == RefCode).FirstOrDefault();
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
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex);
            }
        }
        void Journal_DeleteByStockIn(BLL.StockIn P)
        {

            string RefCode = string.Format("{0}{1}", BLL.FormPrefix.StockIn, P.Id);
            DAL.Journal j = DB.Journals.Where(x => x.RefCode == RefCode).FirstOrDefault();
            if (j != null) Journal_Delete(j.Id);
        }
        #endregion

        #region JobOrderIssue
        void Journal_SaveByJobOrderIssue(DAL.JobOrderIssue P, List<BLL.TaxMaster> TaxDetail)
        {

            try
            {
                string RefCode = string.Format("{0}{1}", BLL.FormPrefix.JobOrderIssue, P.Id);

                if (P.JobWorker == null)
                {
                    P.JobWorker = DB.JobWorkers.Where(x => x.Id == P.JobWorkerId).FirstOrDefault();
                }
                var CId = P.JobWorker.Ledger.AccountGroup.CompanyId;

                DAL.Journal j = DB.Journals.Where(x => x.RefCode == RefCode).FirstOrDefault();
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

                    foreach (var t in TaxDetail)
                    {
                        j.JournalDetails.Add(new DAL.JournalDetail()
                        {
                            LedgerId = t.Ledger.Id,
                            DrAmt = t.TaxAmount,
                            TransactionMode = "Cash",
                            Particulars = P.Narration
                        });

                    }
                    DB.Journals.Add(j);
                    DB.SaveChanges();
                }
                else
                {
                    if (j.JournalDetails.Count != TaxDetail.Count)
                    {
                        foreach (var s in j.JournalDetails.ToList())
                        {
                            decimal pd = j.JournalDetails.Select(X => X.JournalId).FirstOrDefault();
                            DB.JournalDetails.RemoveRange(j.JournalDetails.Where(x => x.JournalId == pd).ToList());
                        }
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

                        foreach (var t in TaxDetail)
                        {
                            j.JournalDetails.Add(new DAL.JournalDetail()
                            {
                                LedgerId = t.Ledger.Id,
                                DrAmt = t.TaxAmount,
                                TransactionMode = "Cash",
                                Particulars = P.Narration
                            });
                        }
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
                                if (jd.LedgerId == LedgerIdByKeyAndCompany(BLL.DataKeyValue.JobOrderIssued_Ledger_Key, CId))
                                {
                                    jd.DrAmt = P.ItemAmount - P.DiscountAmount + P.Extras;
                                }
                                else
                                {
                                    if (jd.LedgerId == TaxIdByCompany_LedgerId(Caller.CompanyId, jd.LedgerId))
                                    {
                                        jd.CrAmt = TaxDetail.Where(x => x.Ledger.Id == jd.LedgerId).Select(x => x.TaxAmount).FirstOrDefault();
                                    }

                                }
                            }
                        }
                    }
                    j.JournalDate = P.JODate;
                    DB.SaveChanges();
                }
            }

            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex);
            }
        }
        void Journal_DeleteByJobOrderIssue(BLL.JobOrderIssue P)
        {
            try
            {
                string RefCode = string.Format("{0}{1}", BLL.FormPrefix.JobOrderIssue, P.Id);
                DAL.Journal j = DB.Journals.Where(x => x.RefCode == RefCode).FirstOrDefault();
                if (j != null) Journal_Delete(j.Id);
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex);
            }
        }

        #endregion

        #region JobOrderReceived

        void Journal_SaveByJobOrderReceived(DAL.JobOrderReceived S, List<BLL.TaxMaster> TaxDetail)
        {

            try
            {
                string RefCode = string.Format("{0}{1}", BLL.FormPrefix.JobOrderReceived, S.Id);
                if (S.JobWorker == null)
                {
                    S.JobWorker = DB.JobWorkers.Where(x => x.Id == S.JobWorkerId).FirstOrDefault();
                }
                var CId = S.JobWorker.Ledger.AccountGroup.CompanyId;

                DAL.Journal j = DB.Journals.Where(x => x.RefCode == RefCode).FirstOrDefault();
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

                    foreach (var t in TaxDetail)
                    {
                        j.JournalDetails.Add(new DAL.JournalDetail()
                        {
                            LedgerId = t.Ledger.Id,
                            DrAmt = t.TaxAmount,
                            TransactionMode = "Cash",
                            Particulars = S.Narration
                        });
                    }
                    DB.Journals.Add(j);
                    DB.SaveChanges();
                }
                else
                {
                    if (j.JournalDetails.Count != TaxDetail.Count)
                    {
                        foreach (var s in j.JournalDetails.ToList())
                        {
                            decimal pd = j.JournalDetails.Select(X => X.JournalId).FirstOrDefault();
                            DB.JournalDetails.RemoveRange(j.JournalDetails.Where(x => x.JournalId == pd).ToList());
                        }
                        j.JournalDetails.Add(new DAL.JournalDetail()
                        {
                            LedgerId = S.JobWorker.Ledger.Id,
                            CrAmt = S.TotalAmount,
                            Particulars = S.Narration
                        });

                        j.JournalDetails.Add(new DAL.JournalDetail()
                        {
                            LedgerId = LedgerIdByKeyAndCompany(BLL.DataKeyValue.JobOrderIssued_Ledger_Key, CId),
                            DrAmt = S.ItemAmount - S.DiscountAmount + S.ExtraAmount,
                            Particulars = S.Narration
                        });

                        foreach (var t in TaxDetail)
                        {
                            j.JournalDetails.Add(new DAL.JournalDetail()
                            {
                                LedgerId = t.Ledger.Id,
                                DrAmt = t.TaxAmount,
                                TransactionMode = "Cash",
                                Particulars = S.Narration
                            });
                        }
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
                                if (jd.LedgerId == LedgerIdByKeyAndCompany(BLL.DataKeyValue.JobOrderReceived_Ledger_Key, CId))
                                {
                                    jd.DrAmt = S.ItemAmount - S.DiscountAmount + S.ExtraAmount;
                                }
                                else if (jd.LedgerId == TaxIdByCompany_LedgerId(Caller.CompanyId, jd.LedgerId))
                                {
                                    jd.DrAmt = TaxDetail.Where(x => x.Ledger.Id == jd.LedgerId).Select(x => x.TaxAmount).FirstOrDefault();
                                }
                            }
                        }

                    }
                }
                j.JournalDate = S.JRDate;
                DB.SaveChanges();
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex);
            }
        }
        void Journal_DeleteByJobOrderReceived(BLL.JobOrderReceived S)
        {
            try
            {
                string RefCode = string.Format("{0}{1}", BLL.FormPrefix.JobOrderReceived, S.Id);

                DAL.Journal j = DB.Journals.Where(x => x.RefCode == RefCode).FirstOrDefault();
                if (j != null) Journal_Delete(j.Id);
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex);
            }
        }
        #endregion

        #region StockInProcess

        void Journal_SaveByStockInProcess(DAL.StockInProcess S)
        {
            try
            {
                string RefCode = string.Format("{0}{1}", BLL.FormPrefix.StockInProcess, S.Id);
                if (S.Staff == null)
                {
                    S.Staff = DB.Staffs.Where(x => x.Id == S.StaffId).FirstOrDefault();
                }
                var CId = S.Staff.Ledger.AccountGroup.CompanyId;

                DAL.Journal j = DB.Journals.Where(x => x.RefCode == RefCode).FirstOrDefault();
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

                    DB.Journals.Add(j);
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
                DB.SaveChanges();
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex);
            }
        }
        void Journal_DeleteByStockInProcess(BLL.StockInProcess S)
        {
            string RefCode = string.Format("{0}{1}", BLL.FormPrefix.StockInProcess, S.Id);

            DAL.Journal j = DB.Journals.Where(x => x.RefCode == RefCode).FirstOrDefault();
            if (j != null) Journal_Delete(j.Id);
        }
        #endregion

        #region StockSeparated

        void Journal_SaveByStockSeparated(DAL.StockSeparated S)
        {
            try
            {
                string RefCode = string.Format("{0}{1}", BLL.FormPrefix.StockSeparated, S.Id);
                if (S.Staff == null)
                {
                    S.Staff = DB.Staffs.Where(x => x.Id == S.StaffId).FirstOrDefault();
                }
                var CId = S.Staff.Ledger.AccountGroup.CompanyId;

                DAL.Journal j = DB.Journals.Where(x => x.RefCode == RefCode).FirstOrDefault();
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

                    DB.Journals.Add(j);
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
                DB.SaveChanges();
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex);
            }
        }
        void Journal_DeleteByStockSeparated(BLL.StockSeperated S)
        {

            try
            {
                string RefCode = string.Format("{0}{1}", BLL.FormPrefix.StockSeparated, S.Id);

                DAL.Journal j = DB.Journals.Where(x => x.RefCode == RefCode).FirstOrDefault();
                if (j != null) Journal_Delete(j.Id);
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex);
            }
        }
        #endregion

        public List<BLL.Journal> Journal_List(int? LedgerId, DateTime dtFrom, DateTime dtTo, string EntryNo, string Status, decimal amtFrom, decimal amtTo)
        {

            List<BLL.Journal> lstJournal = new List<BLL.Journal>();
            BLL.Journal rp = new BLL.Journal();
            try
            {
                foreach (var l1 in DB.JournalDetails.
                      Where(x => x.Journal.JournalDate >= dtFrom && x.Journal.JournalDate <= dtTo
                      && (x.LedgerId == LedgerId || LedgerId == null)
                      && (EntryNo == "" || x.Journal.EntryNo == EntryNo)
                      && (Status == "" || x.Status == Status) &&
                      ((x.CrAmt <= amtTo && x.CrAmt >= amtFrom)
                     || (x.DrAmt <= amtTo && x.DrAmt >= amtFrom)) &&
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
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return lstJournal;
        }


        #endregion
    }
}
