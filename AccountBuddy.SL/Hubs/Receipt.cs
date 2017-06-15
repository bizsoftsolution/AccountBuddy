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

                    foreach (var d_pod in d.ReceiptDetails)
                    {
                        BLL.ReceiptDetail b_pod = PO.RDetails.Where(x => x.Id == d_pod.Id).FirstOrDefault();
                        if (b_pod == null) d.ReceiptDetails.Remove(d_pod);
                    }

                    PO.toCopy<DAL.Receipt>(d);

                    foreach (var b_pod in PO.RDetails)
                    {
                        DAL.ReceiptDetail d_pod = d.ReceiptDetails.Where(x => x.Id == b_pod.Id).FirstOrDefault();
                        if (d_pod == null)
                        {
                            d_pod = new DAL.ReceiptDetail();
                            d.ReceiptDetails.Add(d_pod);
                        }
                        b_pod.toCopy<DAL.ReceiptDetail>(d_pod);
                    }
                    DB.SaveChanges();
                    LogDetailStore(PO, LogDetailType.UPDATE);
                    Journal_SaveByReceipt(PO);
                }
                
                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public BLL.Receipt Receipt_Find(string SearchText)
        {
            BLL.Receipt PO = new BLL.Receipt();
            try
            {

                DAL.Receipt d = DB.Receipts.Where(x => x.EntryNo == SearchText && x.Ledger.AccountGroup.CompanyId == Caller.CompanyId).FirstOrDefault();
                DB.Entry(d).Reload();
                if (d != null)
                {

                    d.toCopy<BLL.Receipt>(PO);
                    PO.LedgerName = (d.Ledger ?? DB.Ledgers.Find(d.LedgerId) ?? new DAL.Ledger()).LedgerName;
                    foreach (var d_pod in d.ReceiptDetails)
                    {
                        BLL.ReceiptDetail b_pod = new BLL.ReceiptDetail();
                        d_pod.toCopy<BLL.ReceiptDetail>(b_pod);
                        PO.RDetails.Add(b_pod);
                        b_pod.LedgerName = (d_pod.Ledger ?? DB.Ledgers.Find(d_pod.LedgerId) ?? new DAL.Ledger()).LedgerName;
                    }

                }
            }
            catch (Exception ex) { }
            return PO;
        }

        public bool Receipt_Delete(long pk)
        {
            try
            {
                DAL.Receipt d = DB.Receipts.Where(x => x.Id == pk).FirstOrDefault();

                if (d != null)
                {
                    DB.ReceiptDetails.RemoveRange(d.ReceiptDetails);
                    DB.Receipts.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(Receipt_DALtoBLL(d), LogDetailType.DELETE);
                }
                return true;
            }
            catch (Exception ex) { }
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


        #endregion
    }
}
