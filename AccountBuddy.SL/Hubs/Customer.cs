using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AccountBuddy.Common;

namespace AccountBuddy.SL.Hubs
{
    #region Customer
    public partial class ABServerHub
    {

        public  BLL.Customer Customer_DALtoBLL(DAL.Customer customerFrom)
        {
            BLL.Customer CustomerTo = customerFrom.toCopy<BLL.Customer>(new BLL.Customer());

            CustomerTo.Ledger = LedgerDAL_BLL(customerFrom.Ledger);


            return CustomerTo;
        }

        public  List<BLL.Customer> Customer_List()
        {
            return Caller.DB.Customers.Where(x => x.Ledger.AccountGroup.CompanyDetail.Id == Caller.CompanyId).ToList()
                             .Select(x => Customer_DALtoBLL(x)).ToList();
        }

        public BLL.Customer Customer_Save(BLL.Customer cus)
        {
            try
            {
                DAL.Customer d = Caller.DB.Customers.Where(x => x.Id == cus.Id).FirstOrDefault();
                if (d == null)
                {

                    d = new DAL.Customer();
                    d.LedgerId = Ledger_Save(cus.Ledger);
                    if (d.LedgerId != 0)
                    {
                        Caller.DB.Customers.Add(d);
                        Caller.DB.SaveChanges();
                        cus.Id = d.Id;
                        LogDetailStore(cus, LogDetailType.INSERT);
                    }
                }
                else
                {
                    cus.toCopy<DAL.Customer>(d);
                    Ledger_Save(cus.Ledger);
                    Caller.DB.SaveChanges();
                    LogDetailStore(cus, LogDetailType.UPDATE);
                }
                var b = Customer_DALtoBLL(d);

                Clients.Clients(OtherLoginClients).Customer_Save(b);
                //Clients.All.Customer_Save(b);
               // WriteLog("Customer_Save", BLL.UserAccount.User.Id,BLL.UserAccount.User.UserType.CompanyId , "Connection Timedout");
                return b;
            }
            catch (Exception ex)
            {
                WriteErrorLog("Customer", "Customer_Save", BLL.UserAccount.User.Id, Caller.CompanyId, ex.Message);
            }
            return new BLL.Customer();
        }

        public bool Customer_Delete(int pk)
        {
            var rv = false;
            try
            {
                var d = Caller.DB.Customers.Where(x => x.Id == pk).FirstOrDefault();
                if (d != null && Ledger_CanDelete(d.Ledger))
                {
                    var b = Customer_DALtoBLL(d);
                    Caller.DB.Customers.Remove(d);
                    Caller.DB.SaveChanges();
                    Ledger_Delete(d.LedgerId);
                    LogDetailStore(b, LogDetailType.DELETE);
                }

                Clients.Clients(OtherLoginClients).Customer_Delete(pk);
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
    #endregion
}