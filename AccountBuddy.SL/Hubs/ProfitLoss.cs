using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AccountBuddy.Common;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
           #region Profit_Loss

        public List<BLL.ProfitLoss> ProfitLoss_List(DateTime dtFrom, DateTime dtTo)
        {
            List<BLL.ProfitLoss> lstProfitLoss = new List<BLL.ProfitLoss>();
            var l1 = Caller.DB.AccountGroups.Where(x => x.CompanyId==Caller.CompanyId && ( x.GroupName == "Income" || x.GroupName == "Expenses")).ToList();
            decimal GTotalDr = 0, GTotalCr = 0, GTotalDrOP = 0, GTotalCrOP = 0;
            foreach (var ag in l1)
            {
                decimal TotalDr = 0, TotalCr = 0, TotalDrOP = 0, TotalCrOP = 0;
                lstProfitLoss.AddRange(ProfitLossByGroupName(ag,dtFrom,dtTo,"", ref TotalDr, ref TotalCr, ref TotalDrOP, ref TotalCrOP));
                GTotalDr += TotalDr;
                GTotalCr += TotalCr;
                GTotalDrOP += TotalDrOP;
                GTotalCrOP += TotalCrOP;

            }

            BLL.ProfitLoss tb = new BLL.ProfitLoss();
            tb.Ledger = new BLL.Ledger();
            tb.Ledger.AccountName = "Total ";
            tb.CrAmt = GTotalCr;
            tb.DrAmt = GTotalDr;
            tb.CrAmtOP = GTotalCrOP;
            tb.DrAmtOP = GTotalDrOP;
            lstProfitLoss.Add(tb);



            tb = new BLL.ProfitLoss();
            tb.Ledger = new BLL.Ledger();
            tb.Ledger.AccountName = "Balance";
            tb.CrAmt = GTotalCr > GTotalDr ? Math.Abs(GTotalDr - GTotalCr) : 0;
            tb.DrAmt = GTotalCr < GTotalDr ? Math.Abs(GTotalDr - GTotalCr) : 0; ;
            tb.CrAmtOP = GTotalCrOP > GTotalDrOP ? Math.Abs(GTotalDrOP - GTotalCrOP) : 0;
            tb.DrAmtOP = GTotalCrOP < GTotalDrOP ? Math.Abs(GTotalDrOP - GTotalCrOP) : 0;
            lstProfitLoss.Add(tb);


            return lstProfitLoss;
        }

        List<BLL.ProfitLoss> ProfitLossByGroupName(DAL.AccountGroup ag, DateTime dtFrom,DateTime dtTo,string Prefix,ref decimal TotalDr, ref decimal TotalCr, ref decimal TotalDrOP, ref decimal TotalCrOP)
        {
            decimal GTotalDr = 0, GTotalCr = 0, GTotalDrOP = 0, GTotalCrOP = 0;

            List<BLL.ProfitLoss> lstProfitLoss = new List<BLL.ProfitLoss>();
            decimal total = 0;
            GetLedgerTotal(ag, ref total);
            if (total == 0) return lstProfitLoss;

            BLL.ProfitLoss tb = new BLL.ProfitLoss();

            tb = new BLL.ProfitLoss();
            tb.Ledger = new BLL.Ledger();
            tb.Ledger.AccountName = Prefix + ag.GroupName;
            tb.CrAmt = null;
            tb.DrAmt = null;
            tb.CrAmtOP = null;
            tb.DrAmtOP = null;

            lstProfitLoss.Add(tb);


            foreach (var uag in ag.AccountGroup1)
            {
                lstProfitLoss.AddRange(ProfitLossByGroupName(uag, dtFrom, dtTo,Prefix+ "     ", ref GTotalDr, ref GTotalCr, ref GTotalDrOP, ref GTotalCrOP));
            }

            decimal OPDr = 0 , OPCr = 0 , Dr = 0 , Cr = 0;           

            foreach (var l in ag.Ledgers)
            {
                tb = new BLL.ProfitLoss();
                tb.Ledger = LedgerDAL_BLL(l);

                LedgerBalance(l, dtFrom, dtTo,ref OPDr, ref OPCr, ref Dr, ref Cr);                

                tb.DrAmt = Dr;
                tb.CrAmt = Cr;
                tb.DrAmtOP = OPDr;
                tb.CrAmtOP = OPCr;
                
                if (tb.DrAmt != 0 || tb.CrAmt != 0)
                {
                    tb.Ledger.AccountGroup.GroupCode = Prefix + "     " + tb.Ledger.AccountGroup.GroupCode;                    
                    lstProfitLoss.Add(tb);
                    GTotalDr += tb.DrAmt ?? 0;
                    GTotalCr += tb.CrAmt ?? 0;

                    GTotalDrOP += tb.DrAmtOP ?? 0;
                    GTotalCrOP += tb.CrAmtOP ?? 0;
                }
            }

            if (GTotalDr > GTotalCr)
            {
                GTotalDr = Math.Abs(GTotalDr - GTotalCr);
                GTotalCr = 0;
            }
            else
            {
                GTotalCr = Math.Abs(GTotalDr - GTotalCr);
                GTotalDr = 0;
            }

            if (GTotalDrOP > GTotalCrOP)
            {
                GTotalDrOP = Math.Abs(GTotalDrOP - GTotalCrOP);
                GTotalCrOP = 0;
            }
            else
            {
                GTotalCrOP = Math.Abs(GTotalDrOP - GTotalCrOP);
                GTotalDrOP = 0;
            }
            tb = new BLL.ProfitLoss();
            tb.Ledger = new BLL.Ledger();
            tb.Ledger.AccountName = Prefix + "Total " + ag.GroupName;
            tb.CrAmt = GTotalCr;
            tb.DrAmt = GTotalDr;
            tb.CrAmtOP = GTotalCrOP;
            tb.DrAmtOP = GTotalDrOP;


            lstProfitLoss.Add(tb);

            TotalDr += GTotalDr;
            TotalCr += GTotalCr;
            TotalDrOP += GTotalDrOP;
            TotalCrOP += GTotalCrOP;

            return lstProfitLoss;
        }       

            #endregion
    }
}