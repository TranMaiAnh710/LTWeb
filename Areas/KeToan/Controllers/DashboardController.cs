using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BTL.Areas.KeToan.Controllers
{
    public class DashboardController : Controller
    {
        // GET: KeToan/Dashboard
        public ActionResult Index()
        {
            if ((int)Session["RoleID"] != 4)
                return RedirectToAction("LoginIndex", "Login");

            return View();
        }
    }
}