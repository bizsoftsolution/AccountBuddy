using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        #region Profit_Loss

        public List<BLL.ProfitLoss> PL_List()
        {
            List<BLL.ProfitLoss> lstProfitLoss = new List<BLL.ProfitLoss>();
            BLL.ProfitLoss bal = new BLL.ProfitLoss();



            #region Assets
            var l2 = DB.Ledgers.Where(x => x.CompanyId == Caller.CompanyId && x.AccountGroup.GroupName == "Income").ToList();

            decimal TotInAmt = 0,CrAmt=0, DrAmt=0 ;
            bal.LedgerName = "Income";
            lstProfitLoss.Add(bal);

            #region Ledger
            foreach (var l in l2)
            {
                bal = new BLL.ProfitLoss();

                bal.LedgerName = string.Format("     {0}", l.LedgerName);
                bal.GroupName = l.AccountGroup == null ? null : l.AccountGroup.GroupName;
                DrAmt = l.Payments.Sum(x => x.Amount);
                CrAmt = l.Receipts.Sum(x => x.Amount);

                bal.Amt =Math.Abs(DrAmt-CrAmt);
               

                if (bal.Amt != 0 )
                {
                    lstProfitLoss.Add(bal);
                    TotInAmt += bal.Amt.Value;
                 
                }
            }
            #endregion


            bal = new BLL.ProfitLoss();
            bal.LedgerName = "Total Income";
            bal.Amt = TotInAmt;
             lstProfitLoss.Add(bal);

            #endregion

            #region Expenses
            var l1 = DB.Ledgers.Where(x => x.CompanyId == Caller.CompanyId && x.AccountGroup.GroupName == "Expenses").ToList();

            decimal TotExAmt = 0;
            bal = new BLL.ProfitLoss();
            bal.LedgerName = "Expense";

            lstProfitLoss.Add(bal);

            #region Ledger
            foreach (var l in l1)
            {
                bal = new BLL.ProfitLoss();

                bal.LedgerName = string.Format("     {0}", l.LedgerName);
                bal.GroupName = l.AccountGroup == null ? null : l.AccountGroup.GroupName;
                DrAmt = l.Payments.Sum(x => x.Amount);
               CrAmt = l.Receipts.Sum(x => x.Amount);
                bal.Amt = Math.Abs(DrAmt - CrAmt);
               
                if (bal.Amt != 0 )
                {
                    lstProfitLoss.Add(bal);
                   TotExAmt+= bal.Amt.Value; ;
                }
            }
            #endregion



            bal = new BLL.ProfitLoss();
            bal.LedgerName = "Total Expenses";
            bal.Amt = TotExAmt;
            lstProfitLoss.Add(bal);

            bal = new BLL.ProfitLoss();
            bal.LedgerName = "Surplus/Deficit";
            bal.Amt =  Math.Abs(TotInAmt- TotExAmt);
            lstProfitLoss.Add(bal);

            #endregion

            return lstProfitLoss;
        }

        #endregion
    }
}