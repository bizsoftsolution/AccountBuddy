using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AccountBuddy.SL.Controllers
{
    public class StockGroupController : Controller
    {
        DAL.DBFMCGEntities DB = new DAL.DBFMCGEntities();

        // GET: StockGroup
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult toList(int CompanyId)
        {

            var l1 = DB.StockGroups.Where(x => x.CompanyId == CompanyId)
                                      .Select(x => new BLL.StockGroup()
                                      {
                                          Id = x.Id,
                                          StockGroupName = x.StockGroupName,
                                          UnderGroupId = x.UnderGroupId??0
                                      })
                                      .ToList();

            return Json(l1, JsonRequestBehavior.AllowGet);
        }
    }
}