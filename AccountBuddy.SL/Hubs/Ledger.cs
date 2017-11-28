﻿using AccountBuddy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {

        #region Ledger

        private BLL.Ledger LedgerDAL_BLL(DAL.Ledger ledgerFrom)
        {
            BLL.Ledger ledgerTo = new BLL.Ledger();
            try
            {
                ledgerTo = ledgerFrom.toCopy<BLL.Ledger>(new BLL.Ledger());

                ledgerTo.AccountGroup = AccountGroupDAL_BLL(ledgerFrom.AccountGroup);

                ledgerTo.CreditLimitType = new BLL.CreditLimitType();
                ledgerFrom.CreditLimitType.toCopy<BLL.CreditLimitType>(ledgerTo.CreditLimitType);
                ledgerTo.OPBal = GetLedgerBalance(ledgerFrom);

            }
            catch (Exception ex)
            {

            }
            return ledgerTo;

        }

        public List<BLL.Ledger> Ledger_List()
        {
            return Caller.DB.Ledgers.Where(x => x.AccountGroup.CompanyDetail.Id == Caller.CompanyId).ToList()
                             .Select(x => LedgerDAL_BLL(x)).ToList();
        }

        public List<BLL.Ledger> CashLedger_List()
        {
            return Caller.DB.Ledgers.Where(x => x.AccountGroup.CompanyDetail.Id == Caller.CompanyId && x.AccountGroup.GroupName == "Bank Accounts" || x.AccountGroup.GroupName == "Cash-in-Hand").ToList()
                             .Select(x => LedgerDAL_BLL(x)).ToList();
        }

        public int Ledger_Save(BLL.Ledger led)
        {
            try
            {
                DAL.Ledger d = Caller.DB.Ledgers.Where(x => x.Id == led.Id).FirstOrDefault();
             
                if (d == null)
                {
                    d = new DAL.Ledger(); 
                    Caller.DB.Ledgers.Add(d);

                    led.toCopy<DAL.Ledger>(d);

                   
                    Caller.DB.SaveChanges();
                    led.Id = d.Id;

                    LogDetailStore(led, LogDetailType.INSERT);
                }
                else
                {
                    led.toCopy<DAL.Ledger>(d);
                    Caller.DB.SaveChanges();
                    LogDetailStore(led, LogDetailType.UPDATE);
                }

                Clients.Clients(OtherLoginClientsOnGroup).Ledger_Save(led);

                return led.Id = d.Id;
            }
            catch (Exception ex) { }
            return 0;
        }

        public bool Ledger_Delete(int pk)
        {
            var rv = false;
            try
            {
                var d = Caller.DB.Ledgers.Where(x => x.Id == pk).FirstOrDefault();
                if (d != null && Ledger_CanDelete(d))
                {

                    Caller.DB.Ledgers.Remove(d);
                    Caller.DB.SaveChanges();
                    Clients.Clients(OtherLoginClientsOnGroup).Ledger_Delete(pk);
                    Clients.All.delete(pk);
                    rv = true;

                }
                else
                {
                    rv = false;
                }



                LogDetailStore(LedgerDAL_BLL(d), LogDetailType.DELETE);
            }
            catch (Exception ex)
            {
                rv = false;
            }
            return rv;
        }

        public bool Ledger_CanDelete(DAL.Ledger l)
        {
            bool rv = (l == null) ? false : l.Payments.Count() == 0 &&
                   l.PaymentDetails.Count() == 0 &&
                   l.Receipts.Count() == 0 &&
                   l.ReceiptDetails.Count() == 0 &&
                   l.JournalDetails.Count() == 0 &&
                   l.PurchaseOrders.Count() == 0 &&
                   l.PurchaseRequests.Count() == 0 &&
                   l.PurchaseReturns.Count() == 0 &&
                   l.Purchases.Count() == 0 &&
                   l.Sales.Count() == 0 &&
                   l.SalesOrders.Count() == 0 &&
                   l.SalesReturns.Count() == 0 &&
                   l.StockIns.Count() == 0 &&
                   l.StockOuts.Count() == 0;

            return rv;
        }

        public bool Ledger_CanDeleteById(int Id)
        {
            return Ledger_CanDelete(Caller.DB.Ledgers.Where(x => x.Id == Id).FirstOrDefault());
        }

        public void Existing_Ledger()
        {
            //SqlConnection sqlConnection1 = new SqlConnection(AppLib.conString);

            //SqlCommand cmd = new SqlCommand();
            //SqlDataReader reader;

            //cmd.CommandText = string.Format("select distinct ViewLedgerReport.LedgerName as LedgerName, Fund as Fund from nubebfs.dbo.ViewLedgerReport where Fund='{0}'",AppLib.FundName);
            //cmd.CommandType = CommandType.Text;
            //cmd.Connection = sqlConnection1;
            //sqlConnection1.Close();
            //sqlConnection1.Open();
            //reader = cmd.ExecuteReader();
            //while (reader != null)
            //{
            //    foreach (var i in reader)
            //    {

            //        DAL.Ledger ast = new DAL.Ledger();
            //        ast.LedgerName = reader.GetString(0);
            //        ast.AccountGroupId = BLL.DataKeyValue.Primary_Value;
            //        Caller.DB.Ledgers.Add(ast);
            //        Caller.DB.SaveChanges();
            //    }
            //}

        }

        #endregion
    }
}
