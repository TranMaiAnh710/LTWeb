using BTL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BTL.Areas.NhanVien.Controllers
{
    public class DashboardController : Controller
    {
        // GET: NhanVien/Dashboard
        DataWebEntities db = new DataWebEntities();

        public ActionResult Index()
        {
            int maNV = (int)Session["MaNV"];
            var nv = db.NhanViens.Find(maNV);
            return View(nv);
        }
    }
}