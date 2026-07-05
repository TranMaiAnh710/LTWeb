using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BTL.Controllers
{
    public class LogoutController : Controller
    {
        // GET: Logout
        public ActionResult LogOut()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("LoginIndex", "Login");
        }

        public ActionResult LoginIndex()
        {
            return View();
        }
    }
}