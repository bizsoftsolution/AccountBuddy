using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AccountBuddy.SL.Controllers
{
    public class SaleController : Controller
    {
        string SalRefCode = "", SalPrefix = "";
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult Save(int LedgerId, string PayMode,string RefCode, string SaleDetails, bool IsGST, string ChqNo, DateTime? ChqDate, string ChqBankName, decimal DiscountAmount)
        {
            try
            {
                DAL.DBFMCGEntities db = new DAL.DBFMCGEntities();
                dynamic l1 = JsonConvert.DeserializeObject(SaleDetails);
                DAL.Sale sal = new DAL.Sale();
                foreach (dynamic d1 in l1)
                {
                    DAL.SalesDetail sd = new DAL.SalesDetail()
                    {
                        ProductId = d1.ProductId,
                        Quantity = d1.Qty,
                        UnitPrice = d1.Rate,
                        Amount = d1.Amount,
                        UOMId = d1.UOMId, 
                        DiscountAmount=d1.DiscountAmount
                    };
                    sal.SalesDetails.Add(sd);
                }
                sal.LedgerId = LedgerId;
                sal.SalesDate = DateTime.Now;
                sal.ItemAmount = sal.SalesDetails.Sum(x => x.Amount);
                sal.DiscountAmount = DiscountAmount;
                sal.RefCode = RefCode;
                if (IsGST == true)
                {
                    sal.GSTAmount = sal.ItemAmount * 6 / 100;
                    sal.TotalAmount = sal.ItemAmount + sal.GSTAmount-DiscountAmount; ;
                }
                else
                {
                    sal.GSTAmount = 0;
                    sal.TotalAmount = sal.ItemAmount - DiscountAmount; ;
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
                sal.RefNo = Hubs.ABServerHub.Sales_NewRefNoByCompanyId(db.Ledgers.Where(x => x.Id == LedgerId).FirstOrDefault().AccountGroup.CompanyId);
                sal.Narration = PayMode;
                db.Sales.Add(sal);
                db.SaveChanges();
                DAL.Ledger l = db.Ledgers.Where(x => x.Id == LedgerId).FirstOrDefault();
                SalRefCode = db.Sales.Where(x => x.SalesDate.Month == DateTime.Now.Month && x.Ledger.AccountGroup.CompanyId ==l.AccountGroup.CompanyDetail.Id).Max(x => x.RefNo.Substring(x.RefNo.Length - 5, x.RefNo.Length));

                Hubs.ABServerHub.Journal_SaveBySales(sal);
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
                DAL.DBFMCGEntities db = new DAL.DBFMCGEntities();

                var lst1 = from s in db.Sales
                           join sd in db.SalesDetails on s.Id equals sd.SalesId
                           where s.Ledger.AccountGroup.CompanyId == DealerId && s.SalesDate >= DateFrom && s.SalesDate <= DateTo
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
                DAL.DBFMCGEntities db = new DAL.DBFMCGEntities();

                var lst1 = from s in db.Sales
                           where s.Ledger.AccountGroup.CompanyId == DealerId && s.SalesDate >= DateFrom && s.SalesDate <= DateTo
                           select new { s.Ledger.LedgerName, s.TotalAmount };
                var lst2 = lst1.GroupBy(x => x.LedgerName).Select(x => new { LedgerName = x.Key, Amount = x.Sum(y => y.TotalAmount) }).ToList();


                return Json(new { HasError = false, Datas = lst2 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { HasError = true, ErrMsg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Receipt_Save(int LedgerId, decimal Amount, string PayMode, string ChqNo, DateTime? ChqDate, string ChqBankName)
        {
            try
            {
                DAL.DBFMCGEntities db = new DAL.DBFMCGEntities();
                DAL.Journal Jn = new DAL.Journal();
                if (Amount != 0)
                {
                    DAL.JournalDetail jd = new DAL.JournalDetail();
                    var CId = db.Ledgers.Where(x => x.Id == LedgerId).FirstOrDefault().AccountGroup.CompanyId;
                    if(PayMode=="Cheque")
                    {
                        jd = new DAL.JournalDetail()
                        {
                            LedgerId = db.Banks.FirstOrDefault().LedgerId,
                            DrAmt = Amount,
                            Particulars = "Mobile App Receipt"
                        };
                        Jn.JournalDetails.Add(jd);
                    }
                    else
                    {
                        jd = new DAL.JournalDetail()
                        {
                            LedgerId = Hubs.ABServerHub.LedgerIdByKeyAndCompany(BLL.DataKeyValue.CashLedger_Key, CId),
                            DrAmt = Amount,
                            Particulars = "Mobile App Receipt"
                        };
                        Jn.JournalDetails.Add(jd);
                    }                    
                    jd = new DAL.JournalDetail()
                    {
                        LedgerId = LedgerId,
                        CrAmt = Amount,
                        Particulars = "Mobile App Receipt"

                    };
                    Jn.JournalDetails.Add(jd);
                    Jn.JournalDate = DateTime.Now;
                    Jn.EntryNo = Hubs.ABServerHub.Journal_NewRefNoByCompanyId(db.Ledgers.Where(x => x.Id == LedgerId).FirstOrDefault().AccountGroup.CompanyId);
                    db.Journals.Add(Jn);

                    db.SaveChanges();
                }
                return Json(new { Id = Jn.Id, HasError = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Id = 0, HasError = true, ErrMsg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Payment_Save(int LedgerId, decimal Amount, string PayMode)
        {
            try
            {
                DAL.DBFMCGEntities db = new DAL.DBFMCGEntities();
                DAL.Journal Jn = new DAL.Journal();
                DAL.JournalDetail jd = new DAL.JournalDetail();
                if (Amount != 0)
                {
                    var CId = db.Ledgers.Where(x => x.Id == LedgerId).FirstOrDefault().AccountGroup.CompanyId;
                    if (PayMode == "Cheque")
                    {
                        jd = new DAL.JournalDetail()
                        {
                            LedgerId = db.Banks.FirstOrDefault().LedgerId,
                            DrAmt = Amount,
                            Particulars = "Mobile App Receipt"
                        };
                        Jn.JournalDetails.Add(jd);
                    }
                    else
                    {
                        jd = new DAL.JournalDetail()
                        {
                            LedgerId = Hubs.ABServerHub.LedgerIdByKeyAndCompany(BLL.DataKeyValue.CashLedger_Key, CId),
                            DrAmt = Amount,
                            Particulars = "Mobile App Receipt"
                        };
                        Jn.JournalDetails.Add(jd);
                    }
                    jd = new DAL.JournalDetail()
                    {
                        LedgerId = LedgerId,
                        CrAmt = Amount,
                        Particulars = "Mobile App Payment"

                    };
                    Jn.JournalDetails.Add(jd);

                    Jn.JournalDate = DateTime.Now;
                    Jn.EntryNo = Hubs.ABServerHub.Journal_NewRefNoByCompanyId(db.Ledgers.Where(x => x.Id == LedgerId).FirstOrDefault().AccountGroup.CompanyId);
                    db.Journals.Add(Jn);

                    db.SaveChanges();
                }
                return Json(new { Id = Jn.Id, HasError = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Id = 0, HasError = true, ErrMsg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }


}
