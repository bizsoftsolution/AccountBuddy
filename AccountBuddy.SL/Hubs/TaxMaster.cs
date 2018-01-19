using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        #region TaxMaster

        public BLL.TaxMaster TaxMaster_DALtoBLL(DAL.TaxMaster taxFrom)
        {
            BLL.TaxMaster TaxTo = taxFrom.ToMap(new BLL.TaxMaster());
            TaxTo.Ledger = LedgerDAL_BLL(taxFrom.Ledger);
        
            return TaxTo;
        }

        public List<BLL.TaxMaster> TaxMaster_List()
        {
            return DB.TaxMasters.Where(x => x.Ledger.AccountGroup.CompanyDetail.Id == Caller.CompanyId).ToList()
                             .Select(x => TaxMaster_DALtoBLL(x)).ToList();
        }

        public BLL.TaxMaster TaxMaster_Save(BLL.TaxMaster cus)
        {
            try
            {
                cus.Status = true;
                Common.AppLib.WriteLog(string.Format("server=>{0}", cus));
                DAL.TaxMaster d = DB.TaxMasters.Where(x => x.Id == cus.Id).FirstOrDefault();
                if (d == null)
                {

                    d = new DAL.TaxMaster();
                    cus.ToMap(d);
                    d.LedgerId = Ledger_Save(cus.Ledger);
                   
                    if (d.LedgerId != 0)
                    {
                        DB.TaxMasters.Add(d);
                        DB.SaveChanges();
                        cus.Id = d.Id;
                        LogDetailStore(cus, LogDetailType.INSERT);
                    }
                }
                else
                {
                    cus.ToMap(d);
                    Ledger_Save(cus.Ledger);
                    DB.SaveChanges();
                    LogDetailStore(cus, LogDetailType.UPDATE);
                }
                var b = TaxMaster_DALtoBLL(d);

                if (OtherClientsOnGroup.Count > 0) Clients.Clients(OtherClientsOnGroup).TaxMaster_Save(b);


                //Clients.All.Customer_Save(b);

                // WriteLog("Customer_Save", BLL.UserAccount.User.Id,BLL.UserAccount.User.UserType.CompanyId , "Connection Timedout");
                return b;
            }
            catch (Exception ex)
            {
                WriteErrorLog("TaxMaster", "TaxMaster_Save", BLL.UserAccount.User.Id, Caller.CompanyId, ex.Message);
            }
            Common.AppLib.WriteLog(string.Format("return server=>{0}", cus));
            return new BLL.TaxMaster();
        }

        public bool TaxMaster_Delete(int pk)
        {
            var rv = false;
            try
            {
                var d = DB.TaxMasters.Where(x => x.Id == pk).FirstOrDefault();
                if (d != null && Ledger_CanDelete(d.Ledger))
                {
                    var b = TaxMaster_DALtoBLL(d);
                    DB.TaxMasters.Remove(d);
                    DB.SaveChanges();
                    Ledger_Delete(d.LedgerId);
                    LogDetailStore(b, LogDetailType.DELETE);
                }

                if (OtherClientsOnGroup.Count > 0) Clients.Clients(OtherClientsOnGroup).TaxMaster_Delete(pk);


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
