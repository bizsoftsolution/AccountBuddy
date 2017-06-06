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
            return DB.Suppliers.Where(x => x.Ledger.AccountGroup.CompanyDetail.Id == Caller.CompanyId).ToList()
                             .Select(x => Supplier_DALtoBLL(x)).ToList();
        }

        public int Supplier_Save(BLL.Supplier cus)
        {
            try
            {
                DAL.Supplier d = DB.Suppliers.Where(x => x.Id == cus.Id).FirstOrDefault();
                if (d == null)
                {

                    d = new DAL.Supplier();
                    d.LedgerId = Ledger_Save(cus.Ledger);
                    if (d.LedgerId != 0)
                    {
                        DB.Suppliers.Add(d);
                        DB.SaveChanges();
                        cus.Id = d.Id;
                        LogDetailStore(cus, LogDetailType.INSERT);
                    }
                }
                else
                {
                    cus.toCopy<DAL.Supplier>(d);
                    Ledger_Save(cus.Ledger);
                    DB.SaveChanges();
                    LogDetailStore(cus, LogDetailType.UPDATE);
                }

                Clients.Clients(OtherLoginClientsOnGroup).Supplier_Save(cus);

                return d.Id;
            }
            catch (Exception ex) { }
            return 0;
        }

        public bool Supplier_Delete(int pk)
        {
            var rv = false;
            try
            {
                var d = DB.Suppliers.Where(x => x.Id == pk).FirstOrDefault();
                if (d != null && Ledger_CanDelete(d.Ledger))
                {
                    DB.Suppliers.Remove(d);
                    Ledger_Delete((int)d.LedgerId);
                    DB.SaveChanges();
                    LogDetailStore(Supplier_DALtoBLL(d), LogDetailType.DELETE);
                }

                Clients.Clients(OtherLoginClientsOnGroup).Supplier_Delete(pk);
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