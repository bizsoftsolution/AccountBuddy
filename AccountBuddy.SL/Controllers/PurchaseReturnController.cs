using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace AccountBuddy.SL.Controllers
{
    public class PurchaseReturnController : Controller
    {
        // GET: PurchaseReturn
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult Save(int LedgerId, string PayMode, string PurchaseReturnDetails, bool IsGST, string ChqNo, DateTime? ChqDate, string ChqBankName)
        {
            try
            {
                DAL.DBFMCGEntities db = new DAL.DBFMCGEntities();

                dynamic l1 = JsonConvert.DeserializeObject(PurchaseReturnDetails);
                DAL.PurchaseReturn pur = new DAL.PurchaseReturn();
                foreach (dynamic d1 in l1)
                {
                    DAL.PurchaseReturnDetail sd = new DAL.PurchaseReturnDetail()
                    {
                        ProductId = d1.ProductId,
                        Quantity = d1.Qty,
                        UnitPrice = d1.Rate,
                        Amount = d1.Amount,
                        UOMId = d1.UOMId,
                        DiscountAmount = d1.DiscountAmount,
                        IsResale = d1.IsResale,
                        Particulars = d1.Particulars
                    };
                    pur.PurchaseReturnDetails.Add(sd);
                }
                pur.LedgerId = LedgerId;
                pur.PRDate = DateTime.Now;
                pur.ItemAmount = pur.PurchaseReturnDetails.Sum(x => x.Amount);

                if (IsGST == true)
                {
                    pur.GSTAmount = pur.ItemAmount * 6 / 100;
                    pur.TotalAmount = pur.ItemAmount + pur.GSTAmount;
                }
                else
                {
                    pur.GSTAmount = 0;
                    pur.TotalAmount = pur.ItemAmount;
                }
                if (PayMode == "Cash")
                {
                    pur.TransactionTypeId = 1;
                }
                else if (PayMode == "Credit")
                {
                    pur.TransactionTypeId = 2;
                }
                else if (PayMode == "Cheque")
                {
                    pur.TransactionTypeId = 3;
                    pur.BankName = ChqBankName;
                    pur.ChequeDate = ChqDate;
                    pur.ChequeNo = ChqNo;
                }
                else
                {
                    pur.TransactionTypeId = 2;
                }
                pur.Narration = PayMode;
                pur.RefNo = Hubs.ABServerHub.PurchaseReturn_NewRefNoByCompanyId(db.Ledgers.Where(x => x.Id == LedgerId).FirstOrDefault().AccountGroup.CompanyId);

                db.PurchaseReturns.Add(pur);
                db.SaveChanges();

                Hubs.ABServerHub.Journal_SaveByPurchaseReturn(pur);
                return Json(new { Id = pur.Id, HasError = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Id = 0, HasError = true, ErrMsg = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }
    }
}