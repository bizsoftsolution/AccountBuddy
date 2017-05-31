using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AccountBuddy.Common;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        #region Balance_Sheet

        public List<BLL.BalanceSheet> Balancesheet_List(DateTime dtFrom, DateTime dtTo)
        {
            List<BLL.BalanceSheet> lstBalanceSheet = new List<BLL.BalanceSheet>();
            var l1 = DB.AccountGroups.Where(x => x.CompanyId==Caller.CompanyId && ( x.GroupName == "Assets" || x.GroupName == "Liabilities")).ToList();
            decimal GTotalDr = 0, GTotalCr = 0, GTotalDrOP = 0, GTotalCrOP = 0;
            foreach (var ag in l1)
            {
                decimal TotalDr = 0, TotalCr = 0, TotalDrOP = 0, TotalCrOP = 0;
                lstBalanceSheet.AddRange(BalanceSheetByGroupName(ag,dtFrom,dtTo,"", ref TotalDr, ref TotalCr, ref TotalDrOP, ref TotalCrOP));

                GTotalDr += TotalDr;
                GTotalCr += TotalCr;
                GTotalDrOP += TotalDrOP;
                GTotalCrOP += TotalCrOP;

            }


            BLL.BalanceSheet tb = new BLL.BalanceSheet();
            tb.LedgerList = new BLL.Ledger();
            tb.LedgerList.AccountName = "Total ";
            tb.CrAmt = GTotalCr;
            tb.DrAmt = GTotalDr;
            tb.CrAmtOP = GTotalCrOP;
            tb.DrAmtOP = GTotalDrOP;
            lstBalanceSheet.Add(tb);



            tb = new BLL.BalanceSheet();
            tb.LedgerList = new BLL.Ledger();
            tb.LedgerList.AccountName = "Balance";
            tb.CrAmt = GTotalCr>GTotalDr?Math.Abs(GTotalDr-GTotalCr):0;
            tb.DrAmt = GTotalCr < GTotalDr ? Math.Abs(GTotalDr - GTotalCr) : 0; ;
            tb.CrAmtOP = GTotalCrOP > GTotalDrOP ? Math.Abs(GTotalDrOP - GTotalCrOP) : 0; 
            tb.DrAmtOP = GTotalCrOP < GTotalDrOP ? Math.Abs(GTotalDrOP - GTotalCrOP) : 0;
            lstBalanceSheet.Add(tb);


            return lstBalanceSheet;
        }

        List<BLL.BalanceSheet> BalanceSheetByGroupName(DAL.AccountGroup ag, DateTime dtFrom,DateTime dtTo,string Prefix,ref decimal TotalDr, ref decimal TotalCr, ref decimal TotalDrOP, ref decimal TotalCrOP)
        {
            List<BLL.BalanceSheet> lstBalanceSheet = new List<BLL.BalanceSheet>();
            decimal total = 0;
            GetLedgerTotal(ag,ref total);
            if (total == 0) return lstBalanceSheet;

            BLL.BalanceSheet tb = new BLL.BalanceSheet();

            tb = new BLL.BalanceSheet();
            tb.LedgerList = new BLL.Ledger();
            tb.LedgerList.AccountName = Prefix + ag.GroupName;
            tb.CrAmt = null;
            tb.DrAmt = null;
            tb.CrAmtOP = null;
            tb.DrAmtOP = null;

            lstBalanceSheet.Add(tb);


            foreach (var uag in ag.AccountGroup1)
            {
                lstBalanceSheet.AddRange(BalanceSheetByGroupName(uag, dtFrom, dtTo,Prefix+ "     ",ref TotalDr, ref TotalCr, ref TotalDrOP, ref TotalCrOP));
            }

            decimal OPDr = 0 , OPCr = 0 , Dr = 0 , Cr = 0;           

            foreach (var l in ag.Ledgers)
            {
                tb = new BLL.BalanceSheet();
                tb.LedgerList = LedgerDAL_BLL(l);

                LedgerBalance(l, dtFrom, dtTo,ref OPDr, ref OPCr, ref Dr, ref Cr);                

                tb.DrAmt = Dr;
                tb.CrAmt = Cr;
                tb.DrAmtOP = OPDr;
                tb.CrAmtOP = OPCr;
                
                if (tb.DrAmt != 0 || tb.CrAmt != 0)
                {
                    tb.LedgerList.AccountGroup.GroupCode = Prefix + "     " + tb.LedgerList.AccountGroup.GroupCode;                    
                    lstBalanceSheet.Add(tb);
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


            tb = new BLL.BalanceSheet();
            tb.LedgerList = new BLL.Ledger();
            tb.LedgerList.AccountName = Prefix + "Total " + ag.GroupName;
            tb.CrAmt = TotalCr;
            tb.DrAmt = TotalDr;
            tb.CrAmtOP = TotalCrOP;
            tb.DrAmtOP = TotalDrOP;


            lstBalanceSheet.Add(tb);

            return lstBalanceSheet;
        }       

            #endregion
    }
}