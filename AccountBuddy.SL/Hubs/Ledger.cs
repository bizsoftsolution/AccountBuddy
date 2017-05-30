using AccountBuddy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {

        #region Ledger

        private BLL.Ledger LedgerDAL_BLL(DAL.Ledger ledgerFrom)
        {
            BLL.Ledger ledgerTo = ledgerFrom.toCopy<BLL.Ledger>(new BLL.Ledger());

            ledgerTo.AccountGroup = new BLL.AccountGroup();
            ledgerTo.CreditLimitType = new BLL.CreditLimitType();

            ledgerFrom.AccountGroup.toCopy<BLL.AccountGroup>(ledgerTo.AccountGroup);
            ledgerFrom.CreditLimitType.toCopy<BLL.CreditLimitType>(ledgerTo.CreditLimitType);

            return ledgerTo;
        }

        public List<BLL.Ledger> Ledger_List()
        {
            return DB.Ledgers.Where(x => x.AccountGroup.CompanyDetail.Id == Caller.CompanyId).ToList()
                             .Select(x => LedgerDAL_BLL(x)).ToList();
        }

        public List<BLL.Ledger> CashLedger_List()
        {
            return DB.Ledgers.Where(x => x.AccountGroup.CompanyDetail.Id == Caller.CompanyId && x.AccountGroup.GroupName == "Bank Accounts" || x.AccountGroup.GroupName == "Cash-in-Hand").ToList()
                             .Select(x => LedgerDAL_BLL(x)).ToList();
        }

        public int Ledger_Save(BLL.Ledger led)
        {
            try
            {
                DAL.Ledger d = DB.Ledgers.Where(x => x.Id == led.Id).FirstOrDefault();
                if (d == null)
                {
                    d = new DAL.Ledger();
                    DB.Ledgers.Add(d);

                    led.toCopy<DAL.Ledger>(d);

                    DB.SaveChanges();
                    led.Id = d.Id;

                    LogDetailStore(led, LogDetailType.INSERT);
                }
                else
                {
                    led.toCopy<DAL.Ledger>(d);
                    DB.SaveChanges();
                    LogDetailStore(led, LogDetailType.UPDATE);
                }

                Clients.Clients(OtherLoginClientsOnGroup).Ledger_Save(led);

                return led.Id = d.Id;
            }
            catch (Exception ex) { }
            return 0;
        }

        public bool Ledger_Delete(int pk)
        {
            var rv = false;
            try
            {
                var d = DB.Ledgers.Where(x => x.Id == pk).FirstOrDefault();
                if (d.Payments != null && d.PaymentDetails != null && d.Receipts != null && d.ReceiptDetails != null && d.JournalDetails != null)
                {
                    if (d != null)
                    {
                        DB.Ledgers.Remove(d);
                        DB.SaveChanges();
                        LogDetailStore(LedgerDAL_BLL(d), LogDetailType.DELETE);
                    }

                    Clients.Clients(OtherLoginClientsOnGroup).Ledger_Delete(pk);
                    Clients.All.delete(pk);

                    rv = true;
                }
                else
                {
                    rv = false;
                }

            }
            catch (Exception ex)
            {
                rv = false;
            }
            return rv;
        }

        #endregion
    }
}
