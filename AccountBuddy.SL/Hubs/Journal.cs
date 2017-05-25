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

        #region list
        public static List<BLL.Journal> _JPendingList;
        public static List<BLL.Journal> JPendingList
        {
            get
            {
                if (_JPendingList == null)
                {
                    _JPendingList = new List<BLL.Journal>();
                    foreach (var d1 in DB.Journals.OrderBy(x => x.EntryNo).ToList())
                    {
                        BLL.Journal d2 = new BLL.Journal();
                        d1.toCopy<BLL.Journal>(d2);

                        d2.JDetails = new ObservableCollection<BLL.JournalDetail>();
                        foreach(var jdFrom in d1.JournalDetails)
                        {
                            BLL.JournalDetail jdTo = new BLL.JournalDetail();
                            jdFrom.toCopy<BLL.JournalDetail>(jdTo);
                            jdTo.LedgerName = jdFrom.Ledger.LedgerName;
                            jdFrom.Ledger.toCopy<BLL.Ledger>(jdTo.JLedger);
                            d2.JDetails.Add(jdTo);
                        }
                        _JPendingList.Add(d2);
                    }

                }
                return _JPendingList;
            }
            set
            {
                _JPendingList = value;
            }
        }
        #endregion


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
                    PO.toCopy<DAL.Journal>(d);

                    //foreach (var d_pod in d.JournalDetails)
                    //{
                    //    BLL.JournalDetail b_pod = PO.JDetails.Where(x => x.Id == d_pod.Id).FirstOrDefault();
                    //    if (b_pod == null) d.JournalDetails.Remove(d_pod);
                    //}

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

                BLL.Journal B_PO = JPendingList.Where(x => x.Id == PO.Id).FirstOrDefault();

                if (B_PO == null)
                {
                    B_PO = new BLL.Journal();
                    JPendingList.Add(B_PO);
                }

                PO.toCopy<BLL.Journal>(B_PO);
                Clients.Clients(OtherLoginClientsOnGroup).Journal_POPendingSave(B_PO);

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

                DAL.Journal d = DB.Journals.Where(x => x.EntryNo == SearchText).FirstOrDefault();
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
                    BLL.Journal PO = new BLL.Journal();
                    d.toCopy<BLL.Journal>(PO);
                    foreach (var d_pod in d.JournalDetails)
                    {
                        BLL.JournalDetail b_pod = new BLL.JournalDetail();
                        d_pod.toCopy<BLL.JournalDetail>(b_pod);
                        PO.JDetails.Add(b_pod);

                    }
                    DB.JournalDetails.RemoveRange(d.JournalDetails);
                    DB.Journals.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(PO, LogDetailType.DELETE);

                    BLL.Journal B_PO = JPendingList.Where(x => x.Id == PO.Id).FirstOrDefault();
                    if (B_PO != null)
                    {
                        Clients.Clients(OtherLoginClientsOnGroup).Journal_POPendingDelete(B_PO.Id);
                        JPendingList.Remove(B_PO);
                    }
                }
                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public List<BLL.Journal> Journal_JPendingList()
        {
            return JPendingList.Where(x => x.JDetail.JLedger.AccountGroup.Company.Id == Caller.CompanyId).ToList();
        }

        public bool Find_JEntryNo(string entryNo, BLL.Payment PO)

        {
            DAL.Journal d = DB.Journals.Where(x => x.EntryNo == entryNo & x.Id != PO.Id).FirstOrDefault();
            if (d == null)
            {
                return false;
            }
            else
            {
                return true;
            }

        }



        #endregion
    }
}
