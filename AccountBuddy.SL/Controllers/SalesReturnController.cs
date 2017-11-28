using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AccountBuddy.SL.Controllers
{
    public class SalesReturnController : Controller
    {
        // GET: SaleReturn
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Save(int LedgerId, string PayMode,string RefCode, string SaleReturnDetails, bool IsGST,string ChqNo, DateTime? ChqDate, string ChqBankName, decimal DiscountAmount)
        {
            try
            {
                DAL.DBFMCGEntities db = new DAL.DBFMCGEntities();
                Hubs.ABServerHub ab = new Hubs.ABServerHub();

                dynamic l1 = JsonConvert.DeserializeObject(SaleReturnDetails);
                DAL.SalesReturn sal = new DAL.SalesReturn();
                foreach (dynamic d1 in l1)
                {
                    DAL.SalesReturnDetail sd = new DAL.SalesReturnDetail()
                    {
                        ProductId = d1.ProductId,
                        Quantity = d1.Qty,
                        UnitPrice = d1.Rate,
                        Amount = d1.Amount,
                        UOMId = d1.UOMId,
                        DiscountAmount=d1.DiscountAmount, 
                        IsResale=d1.IsResale, 
                        Particulars=d1.Particulars
                    };
                    sal.SalesReturnDetails.Add(sd);
                }
                sal.LedgerId = LedgerId;
                sal.SRDate = DateTime.Now;
                sal.ItemAmount = sal.SalesReturnDetails.Sum(x => x.Amount.Value);
                sal.DiscountAmount = DiscountAmount;
                if (IsGST == true)
                {
                    sal.GSTAmount = sal.ItemAmount * 6 / 100;
                    sal.TotalAmount = sal.ItemAmount + sal.GSTAmount-DiscountAmount;
                }
                else
                {
                    sal.GSTAmount = 0;
                    sal.TotalAmount = sal.ItemAmount-DiscountAmount;
                }
                if (PayMode == "Cash")
                {
                    sal.TransactionTypeId = 1;
                }
                else if (PayMode == "Credit")
                {
                    sal.TransactionTypeId = 2;
                }
                else if (PayMode == "Cheque")
                {
                    sal.TransactionTypeId = 3;
                    sal.BankName = ChqBankName;
                    sal.ChequeDate = ChqDate;
                    sal.ChequeNo = ChqNo;
                }
                else
                {
                    sal.TransactionTypeId = 2;
                }
                sal.Narration = PayMode;
                sal.RefNo =ab.SalesReturn_NewRefNoByCompanyId(db.Ledgers.Where(x => x.Id == LedgerId).FirstOrDefault().AccountGroup.CompanyId);
                sal.RefCode = RefCode;
                db.SalesReturns.Add(sal);
                db.SaveChanges();

               
                ab.Journal_SaveBySalesReturn(sal);
                return Json(new { Id = sal.Id, HasError = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Id = 0, HasError = true, ErrMsg = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

    }
}