using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AccountBuddy.Common;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {

        private BLL.Supplier Supplier_DALtoBLL(DAL.Supplier SupplierFrom)
        {
            BLL.Supplier SupplierTo = SupplierFrom.toCopy<BLL.Supplier>(new BLL.Supplier());

            SupplierTo.Ledger = LedgerDAL_BLL(SupplierFrom.Ledger);


            return SupplierTo;
        }



        public List<BLL.Supplier> Supplier_List()
        {
            return Caller.DB.Suppliers.Where(x => x.Ledger.AccountGroup.CompanyDetail.Id == Caller.CompanyId).ToList()
                             .Select(x => Supplier_DALtoBLL(x)).ToList();

          

        }

        public BLL.Supplier Supplier_Save(BLL.Supplier sup)
        {
            try
            {
                DAL.Supplier d = Caller.DB.Suppliers.Where(x => x.Id == sup.Id).FirstOrDefault();
                if (d == null)
                {

                    d = new DAL.Supplier();
                    d.LedgerId = Ledger_Save(sup.Ledger);
                    if (d.LedgerId != 0)
                    {
                        Caller.DB.Suppliers.Add(d);
                        Caller.DB.SaveChanges();
                        sup.Id = d.Id;
                        LogDetailStore(sup, LogDetailType.INSERT);
                    }
                }
                else
                {
                    sup.toCopy<DAL.Supplier>(d);
                    Ledger_Save(sup.Ledger);
                    Caller.DB.SaveChanges();
                    LogDetailStore(sup, LogDetailType.UPDATE);
                }
                var s = Supplier_DALtoBLL(d);
                Clients.Clients(OtherLoginClients).Supplier_Save(s);

                return s;
            }
            catch (Exception ex) { }
            return new BLL.Supplier();
        }

        public bool Supplier_Delete(int pk)
        {
            var rv = false;
            try
            {
                var d = Caller.DB.Suppliers.Where(x => x.Id == pk).FirstOrDefault();
                if (d != null && Ledger_CanDelete(d.Ledger))
                {
                    var s = Supplier_DALtoBLL(d);
                    Caller.DB.Suppliers.Remove(d);
                    Caller.DB.SaveChanges();
                    Ledger_Delete((int)d.LedgerId);                    
                    LogDetailStore(s, LogDetailType.DELETE);
                }

                Clients.Clients(OtherLoginClients).Supplier_Delete(pk);
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