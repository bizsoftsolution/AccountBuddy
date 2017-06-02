using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AccountBuddy.Common;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {

        #region Customer

        private BLL.Customer CustomerDAL_BLL(DAL.Customer customerFrom)
        {
            BLL.Customer CustomerTo = customerFrom.toCopy<BLL.Customer>(new BLL.Customer());

            CustomerTo.Ledger = LedgerDAL_BLL(customerFrom.Ledger);


            return CustomerTo;
        }

        public List<BLL.Customer> Customer_List()
        {
            return DB.Customers.Where(x => x.Ledger.AccountGroup.CompanyDetail.Id == Caller.CompanyId).ToList()
                             .Select(x => CustomerDAL_BLL(x)).ToList();
        }

        public List<BLL.Customer> CashCustomer_List()
        {
            return DB.Customers.Where(x => x.Ledger.AccountGroup.CompanyDetail.Id == Caller.CompanyId && x.Ledger.AccountGroup.GroupName == "Bank Accounts" || x.Ledger.AccountGroup.GroupName == "Cash-in-Hand").ToList()
                             .Select(x => CustomerDAL_BLL(x)).ToList();
        }

        public int Customer_Save(BLL.Customer cus)
        {
            try
            {
                DAL.Ledger d = DB.Ledgers.Where(x => x.Id == cus.Ledger.Id).FirstOrDefault();
                DAL.Customer CL = DB.Customers.Where(x => x.LedgerId == cus.Id).FirstOrDefault();

                if (d == null)
                {

                    d = new DAL.Ledger();
                    DB.Ledgers.Add(d);
                    d.AccountGroupId = DB.AccountGroups.Where(x => x.GroupName == "Sundry Creditors").Select(x => x.Id).FirstOrDefault();
                    cus.Ledger.toCopy<DAL.Ledger>(d);
                                        
                    cus.Id = d.Id;
                    DB.SaveChanges();

                    CL = new DAL.Customer();
                    CL.LedgerId = d.Id;
                     
                    DB.Customers.Add(CL);
                    DB.SaveChanges();

                  
                    cus.toCopy<DAL.Customer>(CL);



                   



                    LogDetailStore(cus, LogDetailType.INSERT);
                }
                else
                {
                    cus.toCopy<DAL.Ledger>(d);
                    DB.SaveChanges();
                    LogDetailStore(cus, LogDetailType.UPDATE);
                }

                Clients.Clients(OtherLoginClientsOnGroup).Customer_Save(cus);

                return cus.Id = d.Id;
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
                if (d.Ledger.Payments != null && d.Ledger.PaymentDetails != null && d.Ledger.Receipts != null && d.Ledger.ReceiptDetails != null && d.Ledger.JournalDetails != null)
                {
                    if (d != null)
                    {
                        DB.Customers.Remove(d);
                        DB.SaveChanges();
                        LogDetailStore(CustomerDAL_BLL(d), LogDetailType.DELETE);
                    }

                    Clients.Clients(OtherLoginClientsOnGroup).Customer_Delete(pk);
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