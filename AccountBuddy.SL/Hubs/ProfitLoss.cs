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
            var l1 = DB.AccountGroups.Where(x => x.CompanyId==Caller.CompanyId && ( x.GroupName == "Income" || x.GroupName == "Expenses")).ToList();
            
            foreach (var ag in l1)
            {
                decimal TotalDr = 0, TotalCr = 0, TotalDrOP = 0, TotalCrOP = 0;
                lstProfitLoss.AddRange(ProfitLossByGroupName(ag,dtFrom,dtTo,"", ref TotalDr, ref TotalCr, ref TotalDrOP, ref TotalCrOP));                
            }

            return lstProfitLoss;
        }

        List<BLL.ProfitLoss> ProfitLossByGroupName(DAL.AccountGroup ag, DateTime dtFrom,DateTime dtTo,string Prefix,ref decimal TotalDr, ref decimal TotalCr, ref decimal TotalDrOP, ref decimal TotalCrOP)
        {
            List<BLL.ProfitLoss> lstProfitLoss = new List<BLL.ProfitLoss>();
            if (ag.AccountGroup1.Count() == 0 && ag.Ledgers.Count() == 0) return lstProfitLoss;

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
                lstProfitLoss.AddRange(ProfitLossByGroupName(uag, dtFrom, dtTo,Prefix+ "     ",ref TotalDr, ref TotalCr, ref TotalDrOP, ref TotalCrOP));
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
                    TotalDr += tb.DrAmt??0;
                    TotalCr += tb.CrAmt ?? 0;

                    TotalDrOP += tb.DrAmtOP ?? 0;
                    TotalCrOP += tb.CrAmtOP ?? 0;                    
                }
            }

            if (TotalDr > TotalCr)
            {
                TotalDr = Math.Abs(TotalDr - TotalCr);
                TotalCr = 0;
            }
            else
            {
                TotalCr = Math.Abs(TotalDr - TotalCr);
                TotalDr = 0;
            }

            if (TotalDrOP > TotalCrOP)
            {
                TotalDrOP = Math.Abs(TotalDrOP - TotalCrOP);
                TotalCrOP = 0;
            }
            else
            {
                TotalCrOP = Math.Abs(TotalDrOP - TotalCrOP);
                TotalDrOP = 0;
            }


            tb = new BLL.ProfitLoss();
            tb.Ledger = new BLL.Ledger();
            tb.Ledger.AccountName = Prefix + "Total " + ag.GroupName;
            tb.CrAmt = TotalCr;
            tb.DrAmt = TotalDr;
            tb.CrAmtOP = TotalCrOP;
            tb.DrAmtOP = TotalDrOP;


            lstProfitLoss.Add(tb);

            return lstProfitLoss;
        }       

            #endregion
    }
}