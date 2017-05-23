using AccountBuddy.Common;
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

        #region list
        public static List<BLL.Ledger> _ledgerList;
        public static List<BLL.Ledger> ledgerList
        {
            get
            {
                if (_ledgerList == null)
                {
                    _ledgerList = new List<BLL.Ledger>();
                    foreach (var d1 in DB.Ledgers
                        .Select(x => new BLL.Ledger()
                        {
                            Id = x.Id,
                            TelephoneNo = x.TelephoneNo,
                            PersonIncharge = x.PersonIncharge,
                            MobileNo = x.MobileNo,
                            AccountGroupId = x.AccountGroupId == null ? 0 : (int)x.AccountGroupId,
                            
                            GroupCode = x.AccountGroup==null? "" : x.AccountGroup.GroupCode,
                            GroupName = x.AccountGroup == null ? "" : x.AccountGroup.GroupName,
                            AddressLine1 = x.AddressLine1,
                            AddressLine2 = x.AddressLine2,
                            CompanyId = x.CompanyId == null ? 0 : (int)x.CompanyId,
                            CreditAmount = x.CreditAmount == null ? 0 : (double)x.CreditAmount,
                            CreditLimit = x.CreditLimit == null ? (short)0 : x.CreditLimit.Value,
                            CreditLimitTypeId = x.CreditLimitTypeId == null ? 0 : (int)x.CreditLimitTypeId,
                            EMailId = x.EMailId,
                            GSTNo = x.GSTNo,
                            LedgerName = x.LedgerName,
                            LedgerCode = x.LedgerCode,
                            CityName = x.CityName,
                            CreditLimitType = x.CreditLimitType.LimitType,
                            OPDr = x.OPDr,
                            OPCr = x.OPCr

                        })
                        .OrderBy(x => x.LedgerName).ToList())
                    {
                        BLL.Ledger d2 = new BLL.Ledger();
                        d1.toCopy<BLL.Ledger>(d2);
                        _ledgerList.Add(d2);
                    }

                }
                return _ledgerList;
            }
            set
            {
                _ledgerList = value;
            }

        }
        #endregion

        public List<BLL.Ledger> Ledger_List()
        {
            return ledgerList.Where(x=> x.CompanyId==Caller.CompanyId).ToList();
        }

        public int Ledger_Save(BLL.Ledger led)
        {
            try
            {

                BLL.Ledger b = ledgerList.Where(x => x.Id == led.Id).FirstOrDefault();
                DAL.Ledger d = DB.Ledgers.Where(x => x.Id == led.Id).FirstOrDefault();

                if (d == null)
                {

                    b = new BLL.Ledger();
                    ledgerList.Add(b);

                    d = new DAL.Ledger();
                    DB.Ledgers.Add(d);

                    led.toCopy<DAL.Ledger>(d);
                    d.CompanyId = Caller.CompanyId;
                    DB.SaveChanges();
                    d.toCopy<BLL.Ledger>(b);

                    DB.SaveChanges();
                    led.Id = d.Id;
                    LogDetailStore(led, LogDetailType.INSERT);
                }
                else
                {
                    led.toCopy<BLL.Ledger>(b);
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
                BLL.Ledger b = ledgerList.Where(x => x.Id == pk).FirstOrDefault();
                if (b != null)
                {
                    var d = DB.Ledgers.Where(x => x.Id == pk).FirstOrDefault();

                    DB.Ledgers.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(b, LogDetailType.DELETE);
                    ledgerList.Remove(b);
                }

                Clients.Clients(OtherLoginClientsOnGroup).Ledger_Delete(pk);
                Clients.All.delete(pk);
            }
            catch (Exception ex) { }
        }


        #endregion
    }
}
