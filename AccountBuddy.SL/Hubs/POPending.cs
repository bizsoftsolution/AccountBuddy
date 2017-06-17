﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        public List<BLL.POPending> POPending_List(DateTime dtFrom, DateTime dtTo)
        {
            List<BLL.POPending> lstPOPending = new List<BLL.POPending>();
            BLL.POPending tb = new BLL.POPending();

            var lstLedger = DB.Ledgers.Where(x => x.AccountGroup.CompanyId == Caller.CompanyId).ToList();
           
            foreach (var l in lstLedger)
            {
                if(l.PurchaseOrders.Where(x => x.PODate >= dtFrom && x.PODate <= dtTo).Count()!=0)
                {
                    var po = l.PurchaseOrders.FirstOrDefault();
                    tb = new BLL.POPending();
                    tb.Ledger = LedgerDAL_BLL(l);

                    tb.EntryNo = po.RefNo;
                    tb.Amount = po.TotalAmount;
                    tb.PODate = po.PODate;
                    tb.Status = po.PurchaseOrderDetails.FirstOrDefault().PurchaseDetails.Count()>0?"Purchased" : "Pending";
                    lstPOPending.Add(tb);
                }
               

               
               
            }



            return lstPOPending ;
        }

    }
}