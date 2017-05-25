﻿using AccountBuddy.Common;
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

        private BLL.Ledger LedterDAL_BLL(DAL.Ledger ledgerFrom)
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
                             .Select(x => LedterDAL_BLL(x)).ToList();
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

                return led.Id=d.Id;
            }
            catch (Exception ex) { }
            return 0;
        }

        public void Ledger_Delete(int pk)
        {
            try
            {
                var d = DB.Ledgers.Where(x => x.Id == pk).FirstOrDefault();
                if (d != null)
                {                    
                    DB.Ledgers.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(LedterDAL_BLL(d) , LogDetailType.DELETE);
                }

                Clients.Clients(OtherLoginClientsOnGroup).Ledger_Delete(pk);
                Clients.All.delete(pk);
            }
            catch (Exception ex) { }
        }
        
        #endregion
    }
}
