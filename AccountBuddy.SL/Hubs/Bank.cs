using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AccountBuddy.Common;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {

        private BLL.Bank Bank_DALtoBLL(DAL.Bank BankFrom)
        {
            BLL.Bank BankTo = BankFrom.toCopy<BLL.Bank>(new BLL.Bank());

            BankTo.Ledger = LedgerDAL_BLL(BankFrom.Ledger);


            return BankTo;
        }

        public List<BLL.Bank> Bank_List()
        {
            return DB.Banks.Where(x => x.Ledger.AccountGroup.CompanyDetail.Id == Caller.CompanyId).ToList()
                             .Select(x => Bank_DALtoBLL(x)).ToList();
        }

        public int Bank_Save(BLL.Bank cus)
        {
            try
            {
                DAL.Bank d = DB.Banks.Where(x => x.Id == cus.Id).FirstOrDefault();
                if (d == null)
                {

                    d = new DAL.Bank();
                    d.LedgerId = Ledger_Save(cus.Ledger);
                    if (d.LedgerId != 0)
                    {
                        DB.Banks.Add(d);
                        DB.SaveChanges();
                        cus.Id = d.Id;
                        LogDetailStore(cus, LogDetailType.INSERT);
                    }
                }
                else
                {
                    cus.toCopy<DAL.Bank>(d);
                    Ledger_Save(cus.Ledger);
                    DB.SaveChanges();
                    LogDetailStore(cus, LogDetailType.UPDATE);
                }

                Clients.Clients(OtherLoginClientsOnGroup).Bank_Save(cus);

                return d.Id;
            }
            catch (Exception ex) { }
            return 0;
        }

        public bool Bank_Delete(int pk)
        {
            var rv = false;
            try
            {
                var d = DB.Banks.Where(x => x.Id == pk).FirstOrDefault();
                if (d != null && Ledger_CanDelete(d.Ledger))
                {
                    DB.Banks.Remove(d);
                    Ledger_Delete((int)d.LedgerId);
                    DB.SaveChanges();
                    LogDetailStore(Bank_DALtoBLL(d), LogDetailType.DELETE);
                }

                Clients.Clients(OtherLoginClientsOnGroup).Bank_Delete(pk);
                Clients.All.delete(pk);

                rv = true;

            }
            catch (Exception ex)
            {
                rv = false;
            }
            return rv;
        }

    }
}