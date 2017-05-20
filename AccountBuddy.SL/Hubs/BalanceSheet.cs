using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        #region Balance_Sheet

        public List<BLL.BalanceSheet> Balance_List()
        {
            List<BLL.BalanceSheet> lstBalanceSheet = new List<BLL.BalanceSheet>();
            BLL.BalanceSheet bal = new BLL.BalanceSheet();

            #region Assets
            var l2 = DB.Ledgers.Where(x => x.CompanyId == Caller.CompanyId && x.AccountGroup.GroupName == "Assets").ToList();
          
            decimal ToAssDr = 0, ToAssCr = 0;
            bal.LedgerName = "Assets";
            lstBalanceSheet.Add(bal);

            #region Ledger
            foreach (var l in l2)
            {
                bal = new BLL.BalanceSheet();

                bal.LedgerName = string.Format("     {0}", l.LedgerName);
                bal.GroupName = l.AccountGroup == null ? null : l.AccountGroup.GroupName;
                bal.DrAmt = l.Payments.Sum(x => x.Amount);
                bal.CrAmt = l.Receipts.Sum(x => x.Amount);
                if (bal.DrAmt > bal.CrAmt)
                {
                    bal.DrAmt = bal.DrAmt - bal.CrAmt;
                    bal.CrAmt = 0;
                }
                else
                {
                    bal.CrAmt = bal.CrAmt - bal.DrAmt;
                    bal.DrAmt = 0;
                }

                if (bal.DrAmt != 0 || bal.CrAmt != 0)
                {
                    lstBalanceSheet.Add(bal);
                    ToAssDr += bal.DrAmt.Value;
                    ToAssCr += bal.CrAmt.Value;
                }
            }
            #endregion


            bal = new BLL.BalanceSheet();
            bal.LedgerName = "Total Assets";
            bal.DrAmt = ToAssDr;
            bal.CrAmt = ToAssCr;
            lstBalanceSheet.Add(bal);

            #endregion

            #region Liabilities
            var l1 = DB.Ledgers.Where(x => x.CompanyId == Caller.CompanyId && x.AccountGroup.GroupName == "Liabilities" ).ToList();
          
            decimal TotDr = 0, TotCr = 0;
            bal = new BLL.BalanceSheet();
            bal.LedgerName = "Liabilities";

            lstBalanceSheet.Add(bal);

            #region Ledger
            foreach (var l in l1)
            {
                bal = new BLL.BalanceSheet();

                bal.LedgerName = string.Format("     {0}", l.LedgerName);
                bal.GroupName = l.AccountGroup == null ? null : l.AccountGroup.GroupName;
                bal.DrAmt = l.Payments.Sum(x => x.Amount);
                bal.CrAmt = l.Receipts.Sum(x => x.Amount);
                if (bal.DrAmt > bal.CrAmt)
                {
                    bal.DrAmt = bal.DrAmt - bal.CrAmt;
                    bal.CrAmt = 0;
                }
                else
                {
                    bal.CrAmt = bal.CrAmt - bal.DrAmt;
                    bal.DrAmt = 0;
                }

                if (bal.DrAmt != 0 || bal.CrAmt != 0)
                {
                    lstBalanceSheet.Add(bal);
                    TotDr += bal.DrAmt.Value;
                    TotCr += bal.CrAmt.Value;
                }
            }
            #endregion



            bal = new BLL.BalanceSheet();
            bal.LedgerName = "Total Liabilities";
            bal.DrAmt = TotDr;
            bal.CrAmt = TotCr;
            lstBalanceSheet.Add(bal);

            bal = new BLL.BalanceSheet();
            bal.LedgerName = "Total";
            bal.DrAmt = TotDr + ToAssDr;
            bal.CrAmt = TotCr + ToAssCr;
            lstBalanceSheet.Add(bal);

            #endregion

            return lstBalanceSheet;
        }

        #endregion
    }
}