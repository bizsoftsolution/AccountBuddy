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
        #region Receipt
        public string Receipt_NewRefNo()
        {
            DateTime dt = DateTime.Now;
            string Prefix = string.Format("{0}{1:yy}{2:X}", BLL.FormPrefix.Receipt, dt, dt.Month);
            long No = 0;

            var d = DB.Receipts.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.EntryNo.StartsWith(Prefix))
                                     .OrderByDescending(x => x.EntryNo)
                                     .FirstOrDefault();

            if (d != null) No = Convert.ToInt64(d.EntryNo.Substring(Prefix.Length ), 16);

            return string.Format("{0}{1:X5}", Prefix, No + 1);
        }

        public bool Receipt_Save(BLL.Receipt PO)
        {
            try
            {

                DAL.Receipt d = DB.Receipts.Where(x => x.Id == PO.Id).FirstOrDefault();

                if (d == null)
                {

                    d = new DAL.Receipt();
                    DB.Receipts.Add(d);

                    PO.toCopy<DAL.Receipt>(d);

                    foreach (var b_pod in PO.RDetails)
                    {
                        DAL.ReceiptDetail d_pod = new DAL.ReceiptDetail();
                        b_pod.toCopy<DAL.ReceiptDetail>(d_pod);
                        d.ReceiptDetails.Add(d_pod);
                    }
                    DB.SaveChanges();
                    PO.Id = d.Id;
                    LogDetailStore(PO, LogDetailType.INSERT);
                }
                else
                {

                    //foreach (var d_pod in d.ReceiptDetails.ToList())
                    //{
                    //    BLL.ReceiptDetail b_pod = PO.RDetails.Where(x => x.Id == d_pod.Id).FirstOrDefault();
                    //    if (b_pod == null) d.ReceiptDetails.Remove(d_pod);
                    //}
                    decimal rd = PO.RDetails.Select(X => X.ReceiptId).FirstOrDefault();
                    DB.ReceiptDetails.RemoveRange(d.ReceiptDetails.Where(x => x.ReceiptId == rd).ToList());

                    PO.toCopy<DAL.Receipt>(d);

                    foreach (var b_pod in PO.RDetails)
                    {
                        //DAL.ReceiptDetail d_pod = d.ReceiptDetails.Where(x => x.Id == b_pod.Id).FirstOrDefault();
                        //if (d_pod == null)
                        // {
                        DAL.ReceiptDetail d_pod = new DAL.ReceiptDetail();
                            d.ReceiptDetails.Add(d_pod);
                      //  }
                        b_pod.toCopy<DAL.ReceiptDetail>(d_pod);
                    }
                    DB.SaveChanges();
                   
                    LogDetailStore(PO, LogDetailType.UPDATE);
                    
                }
                if (OtherClientsOnGroup.Count > 0) Clients.Clients(OtherClientsOnGroup).Receipt_RefNoRefresh(Receipt_NewRefNo());
                Journal_SaveByReceipt(PO);
                return true;
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return false;
        }

        public BLL.Receipt Receipt_Find(string EntryNo)
        {
            BLL.Receipt PO = new BLL.Receipt();
            try
            {

                DAL.Receipt d = DB.Receipts.Where(x => x.EntryNo == EntryNo && x.Ledger.AccountGroup.CompanyId == Caller.CompanyId).FirstOrDefault();
                DB.Entry(d).Reload();
                if (d != null)
                {

                    d.toCopy<BLL.Receipt>(PO);
                    PO.LedgerName = (d.Ledger ?? DB.Ledgers.Find(d.LedgerId) ?? new DAL.Ledger()).LedgerName;
                    int i = 0;
                    foreach (var d_pod in d.ReceiptDetails)
                    {
                        BLL.ReceiptDetail b_pod = new BLL.ReceiptDetail();
                        d_pod.toCopy<BLL.ReceiptDetail>(b_pod);
                        PO.RDetails.Add(b_pod);
                        b_pod.SNo = ++i;
                       
                        b_pod.LedgerId = d_pod.LedgerId;
                        b_pod.LedgerName = (d_pod.Ledger ?? DB.Ledgers.Find(d_pod.LedgerId) ?? new DAL.Ledger()).LedgerName;
                    }

                }
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return PO;
        }

        public bool Receipt_Delete(long pk)
        {
            try
            {
                DAL.Receipt d = DB.Receipts.Where(x => x.Id == pk).FirstOrDefault();

                if (d != null)
                {
                    var P = Receipt_DALtoBLL(d);
                    DB.ReceiptDetails.RemoveRange(d.ReceiptDetails);
                    DB.Receipts.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(P, LogDetailType.DELETE);
                    Journal_DeleteByReceipt(P);
                    if (OtherClientsOnGroup.Count > 0) Clients.Clients(OtherClientsOnGroup).Receipt_RefNoRefresh(Receipt_NewRefNo());
                }
                return true;
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return false;
        }


        public BLL.Receipt Receipt_DALtoBLL(DAL.Receipt d)
        {
            BLL.Receipt R = d.toCopy<BLL.Receipt>(new BLL.Receipt());
            foreach (var d_Pd in d.ReceiptDetails)
            {
                R.RDetails.Add(d_Pd.toCopy<BLL.ReceiptDetail>(new BLL.ReceiptDetail()));
            }
            return R;
        }

        public bool Find_REntryNo(string entryNo, BLL.Receipt PO)

        {
            DAL.Receipt d = DB.Receipts.Where(x => x.EntryNo == entryNo & x.Id != PO.Id && x.Ledger.AccountGroup.CompanyId == Caller.CompanyId).FirstOrDefault();
            if (d == null)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        public List<BLL.Receipt> Receipt_List(int? LedgerId, DateTime dtFrom, DateTime dtTo, string EntryNo, string Status, decimal amtFrom, decimal amtTo)
        {
            
            List<BLL.Receipt> lstReceipt = new List<BLL.Receipt>();
            BLL.Receipt rp = new BLL.Receipt();
            try
            {
                foreach (var l in DB.Receipts.
                      Where(x => x.ReceiptDate >= dtFrom && x.ReceiptDate <= dtTo
                      && (x.LedgerId == LedgerId || LedgerId == null)
                      && (EntryNo == "" || x.EntryNo == EntryNo)
                      && (Status == "" || x.Status == Status)
                      && (x.Amount >= amtFrom && x.Amount <= amtTo) &&
                      x.Ledger.AccountGroup.CompanyId == Caller.CompanyId).ToList())
                {
                    rp = new BLL.Receipt();
                    rp.Amount = l.Amount;
                    rp.ChequeDate = l.ChequeDate;
                    rp.ChequeNo = l.ChequeNo;
                    rp.CleareDate = l.CleareDate;
                    rp.EntryNo = l.EntryNo;
                    rp.ExtraCharge = l.Extracharge;
                    rp.Id = l.Id;
                    rp.LedgerId = l.LedgerId;
                    rp.LedgerName = string.Format("{0}-{1}", l.Ledger.AccountGroup.GroupCode, l.Ledger.LedgerName);
                    rp.Particulars = l.Particulars;
                    rp.ReceiptDate = l.ReceiptDate;
                    rp.ReceiptMode = l.ReceiptMode;
                    rp.ReceivedFrom = l.ReceivedFrom;
                    rp.RefCode = l.RefCode;
                    rp.RefNo = l.RefNo;
                    rp.Status = l.Status;
                    rp.VoucherNo = l.VoucherNo;
                    lstReceipt.Add(rp);
                    lstReceipt = lstReceipt.OrderBy(x => x.ReceiptDate).ToList();
                }

            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return lstReceipt;
        }

        #endregion
    }
}
