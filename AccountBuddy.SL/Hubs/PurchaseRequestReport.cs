using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        public List<BLL.PurchaseRequestReport> PurchaseRequestReport_List(DateTime dtFrom, DateTime dtTo)
        {
            List<BLL.PurchaseRequestReport> lstPurchaseRequestReport = new List<BLL.PurchaseRequestReport>();
            foreach(var d in DB.PurchaseRequestStatusDetails.Where(x=> x.RequestBy == Caller.StaffId || x.RequestTo == Caller.StaffId).ToList().GroupBy(x=> x.PRId).ToList())
            {
                var dFirst = d.FirstOrDefault();
                var dLast = d.LastOrDefault();
                bool IsNew = dFirst.Id == dLast.Id;
                bool IsHold = dFirst.Id != dLast.Id && dLast.Status == "";
                bool IsReject = dLast.Status == "Reject";
                bool IsApproval = dLast.Status == "Approval";
                bool IsRequestTo = dLast.RequestTo == Caller.StaffId;

                string Status = "";
                foreach(var s in d)
                {
                    Status += string.Format("{1} By {2} on {0:dd/MM/yyyy hh:mm:ss tt}\n", (IsNew || IsHold)?s.RequestAt:s.ResponseAt, (IsNew || IsHold) ? "Hold":(IsApproval?"Approval":"Reject"),s.Staff1.Ledger.LedgerName );
                }
                lstPurchaseRequestReport.Add(new BLL.PurchaseRequestReport() {
                    PurchaseRequestRefNo = dFirst.PurchaseRequest.RefNo,
                    PurchaseRequestId = dLast.PRId.Value,
                    PurchaseRequestStatusDetailsId = dLast.Id,
                    RequestBy = dFirst.Staff.Ledger.LedgerName,
                    RequestTo = dLast.Staff1.Ledger.LedgerName,
                    RequestAt = dFirst.RequestAt,                    
                    ResponseAt = dLast.ResponseAt,
                    SupplierName = dFirst.PurchaseRequest.Ledger.LedgerName,
                    Department = dFirst.Staff.Department.DepartmentName,
                    Amount = dFirst.PurchaseRequest.TotalAmount,
                    Particulars = PurchaseRequestParticulars(dFirst.PurchaseRequest),
                    IsNew = IsNew,
                    IsHold = IsHold,
                    IsApproval= IsApproval,
                    IsReject = IsReject,
                    Remarks = dLast.Remarks,
                    Status = Status,
                    IsRequestTo = IsRequestTo,
                    IsAdmin = Caller.IsAdmin
                });
            }
            return lstPurchaseRequestReport;
        }

        string PurchaseRequestParticulars(DAL.PurchaseRequest pr)
        {
            string rv="";
            foreach(var d in pr.PurchaseRequestDetails)
            {
                rv += string.Format("{2}{0}{1}[RM. {3:0.00} x {4} {5} = RM. {6:0.00}]{0}{0}", "\n", "\t",d.Product.ProductName, d.UnitPrice,d.Quantity,d.UOM.Symbol,(d.UnitPrice* Convert.ToDecimal( d.Quantity)));
            }
            return rv;
        }

    }
}