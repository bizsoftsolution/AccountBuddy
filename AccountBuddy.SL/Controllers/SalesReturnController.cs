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

        public JsonResult Save(int LedgerId, string PayMode, string SaleReturnDetails, bool IsGST,string ChqNo, DateTime? ChqDate, string ChqBankName)
        {
            try
            {
                DAL.DBFMCGEntities db = new DAL.DBFMCGEntities();

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
                        UOMId = d1.UOMId
                    };
                    sal.SalesReturnDetails.Add(sd);
                }
                sal.LedgerId = LedgerId;
                sal.SRDate = DateTime.Now;
                sal.ItemAmount = sal.SalesReturnDetails.Sum(x => x.Amount.Value);
                
                if (IsGST == true)
                {
                    sal.GSTAmount = sal.ItemAmount * 6 / 100;
                    sal.TotalAmount = sal.ItemAmount + sal.GSTAmount;
                }
                else
                {
                    sal.GSTAmount = 0;
                    sal.TotalAmount = sal.ItemAmount;
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
                sal.RefNo = Hubs.ABServerHub.SalesReturn_NewRefNoByCompanyId(db.Ledgers.Where(x => x.Id == LedgerId).FirstOrDefault().AccountGroup.CompanyId);

                db.SalesReturns.Add(sal);
                db.SaveChanges();

                Hubs.ABServerHub.Journal_SaveBySalesReturn(sal);
                return Json(new { Id = sal.Id, HasError = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Id = 0, HasError = true, ErrMsg = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

    }
}