using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AccountBuddy.SL.Controllers
{
    public class UOMController : Controller
    {
        DAL.DBFMCGEntities DB = new DAL.DBFMCGEntities();

        // GET: UOM
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult toList(int CompanyId)
        {

            var l1 = DB.UOMs.Where(x => x.CompanyId == CompanyId)
                                      .Select(x => new BLL.UOM()
                                      {
                                          Id = x.Id,
                                          Symbol=x.Symbol,
                                          FormalName=x.FormalName                                          

                                      })
                                      .ToList();

            return Json(l1, JsonRequestBehavior.AllowGet);
        }
    }
}