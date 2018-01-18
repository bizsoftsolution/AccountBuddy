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
        public string Payment_NewRefNo()
        {
            DateTime dt = DateTime.Now;
            string Prefix = string.Format("{0}{1:yy}{2:X}", BLL.FormPrefix.Payment, dt, dt.Month);
            long No = 0;

            var d = DB.Payments.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.EntryNo.StartsWith(Prefix))
                                     .OrderByDescending(x => x.EntryNo)
                                     .FirstOrDefault();

            if (d != null) No = Convert.ToInt64(d.EntryNo.Substring(Prefix.Length), 16);

            return string.Format("{0}{1:X5}", Prefix, No + 1);
        }
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

                    foreach (var d_pod in d.PaymentDetails)
                    {
                        BLL.PaymentDetail b_pod = PO.PDetails.Where(x => x.Id == d_pod.Id).FirstOrDefault();
                        if (b_pod == null) d.PaymentDetails.Remove(d_pod);
                    }
                    PO.toCopy<DAL.Payment>(d);

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
                Clients.Clients(OtherLoginClientsOnGroup).Payment_RefNoRefresh(Payment_NewRefNo());
                Journal_SaveByPayment(PO);
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

                DAL.Payment d = DB.Payments.Where(x => x.EntryNo == SearchText && x.Ledger.AccountGroup.CompanyId==Caller.CompanyId).FirstOrDefault();
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
                    var P = Payment_DALtoBLL(d);
                    DB.PaymentDetails.RemoveRange(d.PaymentDetails);
                    DB.Payments.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(P, LogDetailType.DELETE);
                    Journal_DeleteByPayment(P);
                }
                
                return true;
            }
            catch (Exception ex) { }
            return false;
        }
        public BLL.Payment Payment_DALtoBLL(DAL.Payment d)
        {
            BLL.Payment P = d.toCopy<BLL.Payment>(new BLL.Payment());
            foreach (var d_Pd in d.PaymentDetails)
            {
                P.PDetails.Add(d_Pd.toCopy<BLL.PaymentDetail>(new BLL.PaymentDetail()));
            }
            return P;
        }

        public bool Find_EntryNo(string entryNo, BLL.Payment PO)

        {
            DAL.Payment d = DB.Payments.Where(x => x.EntryNo == entryNo & x.Id != PO.Id && x.Ledger.AccountGroup.CompanyId==Caller.CompanyId).FirstOrDefault();
            if (d == null)
            {
                return false;
            }
            else
            {
                return true;
            }

        }
        public List<BLL.Payment> Payment_List(int? SID, DateTime dtFrom, DateTime dtTo, string InvoiceNo, string mode)
        {
            BLL.Payment P = new BLL.Payment();
            List<BLL.Payment> lstPurchase = new List<BLL.Payment>();
            try
            {

                var d = DB.Payments.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId &&
                (SID == null || x.LedgerId == SID) && x.PaymentDate >= dtFrom &&
                (mode == null || x.PaymentMode == mode) &&
                x.PaymentDate <= dtTo &&
                (InvoiceNo == "" || x.RefNo.Contains(InvoiceNo))).ToList();
                foreach (var l in d)
                {
                    P = new BLL.Payment();

                    P.Id = l.Id;
                    P.LedgerName = l.Ledger.LedgerName;
                    P.PaymentDate = l.PaymentDate;
                    P.Amount = l.Amount;
                    P.EntryNo = l.EntryNo;
                    P.PaymentMode = l.PaymentMode;
                    lstPurchase.Add(P);

                }
            }
            catch (Exception ex) { }
            return lstPurchase;
        }



        #endregion
    }
}
