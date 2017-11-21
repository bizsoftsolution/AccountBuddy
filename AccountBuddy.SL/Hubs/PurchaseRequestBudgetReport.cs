using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        public List<BLL.PurchaseRequestBudgetReport> PurchaseRequestBudgetReport_List()
        {
            var lst = new List<BLL.PurchaseRequestBudgetReport>();
            if (Caller.IsAdmin)
            {
                var l1 = DB.Departments.ToList();
                var l2 = DB.PurchaseRequests.ToList();
                foreach(var d in l1)
                {
                    var BudgetAmt = Convert.ToDecimal(d.Budget);
                    var ApprovedAmt  = l2.Where(x => x.PurchaseRequestStatusDetails.FirstOrDefault().Staff.DepartmentId == d.Id && x.PurchaseRequestStatusDetails.LastOrDefault().Status == "Approval").Sum(x => x.TotalAmount);
                    var BalAmt = BudgetAmt - ApprovedAmt;
                    var ReqAmt = l2.Where(x => x.PurchaseRequestStatusDetails.FirstOrDefault().Staff.DepartmentId == d.Id && x.PurchaseRequestStatusDetails.LastOrDefault().Status == "").Sum(x => x.TotalAmount);
                    var RemAmt = BalAmt - ReqAmt;
                    lst.Add(new BLL.PurchaseRequestBudgetReport() {
                        Department = d.DepartmentName,
                        BudgetAmount = BudgetAmt,
                        ApprovedAmount = ApprovedAmt,
                        BalanceAmount = BalAmt,
                        RequestAmount = ReqAmt,
                        RemainingAmount = RemAmt
                    });
                }
            }
            return lst;
        }        
    }
}