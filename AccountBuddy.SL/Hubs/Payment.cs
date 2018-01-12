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

                    AppLib.ToMap(PO, d);

                    foreach (var b_pod in PO.PDetails)
                    {
                        DAL.PaymentDetail d_pod = new DAL.PaymentDetail();
                        AppLib.ToMap(b_pod, d_pod);
                        d.PaymentDetails.Add(d_pod);
                    }
                    DB.SaveChanges();
                    PO.Id = d.Id;
                    LogDetailStore(PO, LogDetailType.INSERT);
                }
                else
                {

                    //foreach (var d_pod in d.PaymentDetails.ToList())
                    //{
                    //    BLL.PaymentDetail b_pod = PO.PDetails.Where(x => x.Id == d_pod.Id).FirstOrDefault();
                    //   if (b_pod == null) d.PaymentDetails.Remove(d_pod);

                    //}
                    decimal pd = PO.PDetails.Select(X => X.PaymentId).FirstOrDefault();
                    DB.PaymentDetails.RemoveRange(d.PaymentDetails.Where(x => x.PaymentId == pd).ToList());

                    AppLib.ToMap(PO, d);

                    foreach (var b_pod in PO.PDetails)
                    {
                        //DAL.PaymentDetail d_pod = d.PaymentDetails.Where(x=> x.Id==b_pod.Id).FirstOrDefault();
                        //if (d_pod == null)
                        // {
                        DAL.PaymentDetail d_pod = new DAL.PaymentDetail();
                            d.PaymentDetails.Add(d_pod);
                        // }
                        AppLib.ToMap(b_pod, d_pod);                        
                    }
                    DB.SaveChanges();
                    LogDetailStore(PO, LogDetailType.UPDATE);
                }
                if (OtherClientsOnGroup.Count > 0) Clients.Clients(OtherClientsOnGroup).Payment_RefNoRefresh(Payment_NewRefNo());
                Journal_SaveByPayment(PO);
                return true;
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return false;
        }

        public BLL.Payment Payment_Find(string EntryNo)
        {
            BLL.Payment PO = new BLL.Payment();
            try
            {

                DAL.Payment d = DB.Payments.Where(x => x.EntryNo == EntryNo && x.Ledger.AccountGroup.CompanyId==Caller.CompanyId).FirstOrDefault();
                DB.Entry(d).Reload();
                if (d != null)
                {

                    AppLib.ToMap(d, PO);
                    PO.LedgerName = (d.Ledger ?? DB.Ledgers.Find(d.LedgerId) ?? new DAL.Ledger()).LedgerName;
                    int i = 0;
                    foreach (var d_pod in d.PaymentDetails)
                    {
                        BLL.PaymentDetail b_pod = new BLL.PaymentDetail();
                        AppLib.ToMap(d_pod, b_pod);
                        PO.PDetails.Add(b_pod);
                        b_pod.SNo = ++i;
                        b_pod.LedgerName = (d_pod.Ledger ?? DB.Ledgers.Find(d_pod.LedgerId) ?? new DAL.Ledger()).LedgerName;
                    }

                }
         
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
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
                    if (OtherClientsOnGroup.Count > 0) Clients.Clients(OtherClientsOnGroup).Payment_RefNoRefresh(Payment_NewRefNo());
                }

                return true;
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return false;
        }
        public BLL.Payment Payment_DALtoBLL(DAL.Payment d)
        {
            BLL.Payment P = AppLib.ToMap(d, new BLL.Payment());
            foreach (var d_Pd in d.PaymentDetails)
            {
                P.PDetails.Add(AppLib.ToMap(d_Pd, new BLL.PaymentDetail()));
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
        public List<BLL.Payment> Payment_List(int? LedgerId,  DateTime dtFrom, DateTime dtTo, string EntryNo, string Status, decimal amtFrom, decimal amtTo)
        {
            
            List<BLL.Payment> lstPayment = new List<BLL.Payment>();
            BLL.Payment rp = new BLL.Payment();
            try
            {
                foreach (var l in DB.Payments.
                      Where(x => x.PaymentDate >= dtFrom && x.PaymentDate <= dtTo
                      && (x.LedgerId == LedgerId || LedgerId == null)
                      && (EntryNo == "" || x.EntryNo == EntryNo)
                      && (Status == "" || x.Status == Status)
                      && (x.Amount >= amtFrom && x.Amount <= amtTo) &&
                      x.Ledger.AccountGroup.CompanyId == Caller.CompanyId).ToList())
                {
                    rp = new BLL.Payment();
                    rp.Amount = l.Amount;
                    rp.ChequeDate = l.ChequeDate;
                    rp.ChequeNo = l.ChequeNo;
                    rp.ClearDate = l.ClearDate;
                    rp.EntryNo = l.EntryNo;
                    rp.ExtraCharge = l.ExtraCharge;
                    rp.Id = l.Id;
                    rp.LedgerId = l.LedgerId;
                    rp.LedgerName = string.Format("{0}-{1}", l.Ledger.AccountGroup.GroupCode, l.Ledger.LedgerName);
                    rp.Particulars = l.Particulars;
                    rp.PaymentDate = l.PaymentDate;
                    rp.PaymentMode = l.PaymentMode;
                    rp.PayTo = l.PayTo;
                    rp.RefCode = l.RefCode;
                    rp.RefNo = l.RefNo;
                    rp.Status = l.Status;
                    rp.VoucherNo = l.VoucherNo;
                    lstPayment.Add(rp);
                    lstPayment = lstPayment.OrderBy(x => x.PaymentDate).ToList();
                }

            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return lstPayment;
        }


        #endregion
    }
}
