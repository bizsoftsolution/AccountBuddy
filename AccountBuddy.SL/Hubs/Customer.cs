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
            BLL.Customer CustomerTo = customerFrom.ToMap<BLL.Customer>(new BLL.Customer());

            CustomerTo.Ledger = LedgerDAL_BLL(customerFrom.Ledger);


            return CustomerTo;
        }

        public  List<BLL.Customer> Customer_List()
        {
            return DB.Customers.Where(x => x.Ledger.AccountGroup.CompanyDetail.Id == Caller.CompanyId).ToList()
                             .Select(x => Customer_DALtoBLL(x)).ToList();
        }

        public BLL.Customer Customer_Save(BLL.Customer cus)
        {
            try
            {
                Common.AppLib.WriteLog(string.Format("server=>{0}", cus));
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
                    cus.ToMap<DAL.Customer>(d);
                    Ledger_Save(cus.Ledger);
                    DB.SaveChanges();
                    LogDetailStore(cus, LogDetailType.UPDATE);
                }
                var b = Customer_DALtoBLL(d);

                if (OtherClientsOnGroup.Count > 0) Clients.Clients(OtherClientsOnGroup).Customer_Save(b);
              
                
                //Clients.All.Customer_Save(b);
                                                            
                // WriteLog("Customer_Save", BLL.UserAccount.User.Id,BLL.UserAccount.User.UserType.CompanyId , "Connection Timedout");
                return b;
            }
            catch (Exception ex)
            {
                WriteErrorLog("Customer", "Customer_Save", BLL.UserAccount.User.Id, Caller.CompanyId, ex.Message);
            }
            Common.AppLib.WriteLog(string.Format("return server=>{0}", cus));
            return new BLL.Customer();
        }

        public bool Customer_Delete(int pk)
        {
            var rv = false;
            try
            {
                var d = DB.Customers.Where(x => x.Id == pk).FirstOrDefault();
                if (d != null && Ledger_CanDelete(d.Ledger))
                {
                    var b = Customer_DALtoBLL(d);
                    DB.Customers.Remove(d);
                    DB.SaveChanges();
                    Ledger_Delete(d.LedgerId);
                    LogDetailStore(b, LogDetailType.DELETE);
                }

                if (OtherClientsOnGroup.Count > 0) Clients.Clients(OtherClientsOnGroup).Customer_Delete(pk);
                

                rv = true;

            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex);
                rv = false;
            }
            return rv;
        }

    }
    #endregion
}