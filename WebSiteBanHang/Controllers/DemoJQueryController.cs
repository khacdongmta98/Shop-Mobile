using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebSiteBanHang.Controllers
{
    public class DemoJQueryController : Controller
    {
        // GET: DemoJQuery
        public ActionResult Demo()
        {
            return View();
        }

        [HttpGet]
        public ActionResult TestMethodJquery()
        {
            return View();
        }
    }
}