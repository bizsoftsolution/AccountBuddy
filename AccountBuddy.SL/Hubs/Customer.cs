using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AccountBuddy.Common;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {

        private BLL.Customer Customer_DALtoBLL(DAL.Customer customerFrom)
        {
            BLL.Customer CustomerTo = customerFrom.toCopy<BLL.Customer>(new BLL.Customer());

            CustomerTo.Ledger = LedgerDAL_BLL(customerFrom.Ledger);


            return CustomerTo;
        }

        public List<BLL.Customer> Customer_List()
        {
            return DB.Customers.Where(x => x.Ledger.AccountGroup.CompanyDetail.Id == Caller.CompanyId).ToList()
                             .Select(x => Customer_DALtoBLL(x)).ToList();
        }        

        public int Customer_Save(BLL.Customer cus)
        {
            try
            {
                DAL.Customer d = DB.Customers.Where(x => x.Id == cus.Id).FirstOrDefault();
                if (d == null)
                {                    

                    d = new DAL.Customer();
                    d.LedgerId = Ledger_Save(cus.Ledger);
                    if (d.LedgerId != 0)
                    {
                        DB.Customers.Add(d);
                        DB.SaveChanges();
                        cus.Id = d.Id;
                        LogDetailStore(cus, LogDetailType.INSERT);
                    }
                }
                else
                {
                    cus.toCopy<DAL.Customer>(d);
                    Ledger_Save(cus.Ledger);
                    DB.SaveChanges();
                    LogDetailStore(cus, LogDetailType.UPDATE);
                }

                Clients.Clients(OtherLoginClientsOnGroup).Customer_Save(cus);

                return d.Id;
            }
            catch (Exception ex) { }
            return 0;
        }

        public bool Customer_Delete(int pk)
        {
            var rv = false;
            try
            {
                var d = DB.Customers.Where(x => x.Id == pk).FirstOrDefault();
                if (d != null && Ledger_CanDelete(d.Ledger))
                {
                    DB.Customers.Remove(d);
                    Ledger_Delete(d.LedgerId);
                    DB.SaveChanges();
                    LogDetailStore(Customer_DALtoBLL(d), LogDetailType.DELETE);
                }

                Clients.Clients(OtherLoginClientsOnGroup).Customer_Delete(pk);
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