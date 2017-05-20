using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountBuddy.Common;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        #region Payment

        #region list
        public static List<BLL.Payment> _PPendingList;
        public static List<BLL.Payment> PPendingList
        {
            get
            {
                if (_PPendingList == null)
                {
                    _PPendingList = new List<BLL.Payment>();
                    foreach (var d1 in DB.Payments.OrderBy(x => x.EntryNo).ToList())
                    {
                        BLL.Payment d2 = new BLL.Payment();
                        d1.toCopy<BLL.Payment>(d2);
                        _PPendingList.Add(d2);
                    }

                }
                return _PPendingList;
            }
            set
            {
                _PPendingList = value;
            }
        }
        #endregion


        public bool Payment_Save(BLL.Payment PO)
        {
            try
            {

                DAL.Payment d = DB.Payments.Where(x => x.Id == PO.Id).FirstOrDefault();

                if (d == null)
                {

                    d = new DAL.Payment();
                    DB.Payments.Add(d);

                    PO.toCopy<DAL.Payment>(d);

                    foreach (var b_pod in PO.PDetails)
                    {
                        DAL.PaymentDetail d_pod = new DAL.PaymentDetail();
                        b_pod.toCopy<DAL.PaymentDetail>(d_pod);
                        d.PaymentDetails.Add(d_pod);
                    }
                    DB.SaveChanges();
                    PO.Id = d.Id;
                    LogDetailStore(PO, LogDetailType.INSERT);
                }
                else
                {
                    PO.toCopy<DAL.Payment>(d);

                    //foreach (var d_pod in d.PaymentDetails)
                    //{
                    //    BLL.PaymentDetail b_pod = PO.PDetails.Where(x => x.Id == d_pod.Id).FirstOrDefault();
                    //    if (b_pod == null) d.PaymentDetails.Remove(d_pod);
                    //}

                    foreach (var b_pod in PO.PDetails)
                    {
                        DAL.PaymentDetail d_pod = d.PaymentDetails.Where(x=> x.Id==b_pod.Id).FirstOrDefault();
                        if (d_pod == null)
                        {
                            d_pod = new DAL.PaymentDetail();
                            d.PaymentDetails.Add(d_pod);
                        }
                        b_pod.toCopy<DAL.PaymentDetail>(d_pod);                        
                    }
                    DB.SaveChanges();
                    LogDetailStore(PO, LogDetailType.UPDATE);
                }

                BLL.Payment B_PO = PPendingList.Where(x => x.Id == PO.Id).FirstOrDefault();

                if (B_PO == null)
                {
                    B_PO = new BLL.Payment();
                    PPendingList.Add(B_PO);
                }

                PO.toCopy<BLL.Payment>(B_PO);
                Clients.Clients(OtherLoginClientsOnGroup).Payment_POPendingSave(B_PO);

                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public BLL.Payment Payment_Find(string SearchText)
        {
            BLL.Payment PO = new BLL.Payment();
            try
            {

                DAL.Payment d = DB.Payments.Where(x => x.EntryNo == SearchText).FirstOrDefault();
                DB.Entry(d).Reload();
                if (d != null)
                {

                    d.toCopy<BLL.Payment>(PO);
                    PO.LedgerName = (d.Ledger ?? DB.Ledgers.Find(d.LedgerId) ?? new DAL.Ledger()).LedgerName;
                    foreach (var d_pod in d.PaymentDetails)
                    {
                        BLL.PaymentDetail b_pod = new BLL.PaymentDetail();
                        d_pod.toCopy<BLL.PaymentDetail>(b_pod);
                        PO.PDetails.Add(b_pod);
                        b_pod.LedgerName = (d_pod.Ledger ?? DB.Ledgers.Find(d_pod.LedgerId) ?? new DAL.Ledger()).LedgerName;
                    }

                }
            }
            catch (Exception ex) { }
            return PO;
        }

        public bool Payment_Delete(long pk)
        {
            try
            {
                DAL.Payment d = DB.Payments.Where(x => x.Id == pk).FirstOrDefault();

                if (d != null)
                {
                    BLL.Payment PO = new BLL.Payment();
                    d.toCopy<BLL.Payment>(PO);
                    PO.LedgerName = d.Ledger.LedgerName;
                    foreach (var d_pod in d.PaymentDetails)
                    {
                        BLL.PaymentDetail b_pod = new BLL.PaymentDetail();
                        d_pod.toCopy<BLL.PaymentDetail>(b_pod);
                        PO.PDetails.Add(b_pod);

                    }
                    DB.PaymentDetails.RemoveRange(d.PaymentDetails);
                    DB.Payments.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(PO, LogDetailType.DELETE);

                    BLL.Payment B_PO = PPendingList.Where(x => x.Id == PO.Id).FirstOrDefault();
                    if (B_PO != null)
                    {
                        Clients.Clients(OtherLoginClientsOnGroup).Payment_POPendingDelete(B_PO.Id);
                        PPendingList.Remove(B_PO);
                    }
                }
                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public List<BLL.Payment> Payment_PPendingList()
        {
            return PPendingList.Where(x => x.PLedger.CompanyId== Caller.CompanyId).ToList();
        }

        public bool Find_PORef(string entryNo, BLL.Payment PO)

        {
            DAL.Payment d = DB.Payments.Where(x => x.EntryNo == entryNo & x.Id != PO.Id).FirstOrDefault();
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
