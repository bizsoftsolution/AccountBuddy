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
        public JsonResult ProductReport(int DealerId, DateTime DateFrom, DateTime DateTo)
        {
            try
            {
                DAL.DBFMCGEntities DB = new DAL.DBFMCGEntities();

                var lst1 = from s in DB.SalesReturns
                           join sd in DB.SalesReturnDetails on s.Id equals sd.SRId
                           where s.Ledger.AccountGroup.CompanyId == DealerId && s.SRDate >= DateFrom && s.SRDate <= DateTo
                           select new { sd.Product.ProductName, sd.Amount };
                var lst2 = lst1.GroupBy(x => x.ProductName).Select(x => new { ProductName = x.Key, Amount = x.Sum(y => y.Amount) }).ToList();


                return Json(new { HasError = false, Datas = lst2 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { HasError = true, ErrMsg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult CustomerReport(int DealerId, DateTime DateFrom, DateTime DateTo)
        {
            try
            {
                DAL.DBFMCGEntities DB = new DAL.DBFMCGEntities();

                var lst1 = from s in DB.SalesReturns
                           where s.Ledger.AccountGroup.CompanyId == DealerId && s.SRDate >= DateFrom && s.SRDate <= DateTo
                           select new { s.Ledger.LedgerName, s.TotalAmount };
                var lst2 = lst1.GroupBy(x => x.LedgerName).Select(x => new { LedgerName = x.Key, Amount = x.Sum(y => y.TotalAmount) }).ToList();


                return Json(new { HasError = false, Datas = lst2 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { HasError = true, ErrMsg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}