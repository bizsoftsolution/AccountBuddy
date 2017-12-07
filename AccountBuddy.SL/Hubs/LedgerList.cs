using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
      
        public List<BLL.LedgerList> LedgerList()
        {
            var rv = Caller.DB.Ledgers.Where(x => x.AccountGroup.CompanyDetail.Id == Caller.CompanyId).ToList();
            List<BLL.LedgerList> l1 = new List<BLL.LedgerList>();
            BLL.LedgerList l2 = new BLL.LedgerList();
            foreach(var r in rv)
            {
                l2 = new BLL.LedgerList();
                l2.AccountGroup = AccountGroupDAL_BLL(r.AccountGroup);
                l2.Ledger = LedgerDAL_BLL(r);
                l2.AccountName = l2.Ledger.AccountName;
                l2.Id = r.Id;
                l1.Add(l2);
            }

            return l1;

        }

    }
}