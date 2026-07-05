using BTL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace BTL.Areas.NhanSu.Controllers
{
    public class EmployeeController : Controller
    {
        // GET: NhanSu/Employee
        DataWebEntities db = new DataWebEntities();

        public ActionResult Index()
        {
            if (Session["RoleID"] == null)
                return Redirect(Url.Action("LoginIndex", "Login", new { area = "" }));

            var data = db.NhanViens
                .Include(x => x.PhongBan)
                .Include(x => x.ChucVu) // 🔥 thêm dòng này
                .ToList();
            ViewBag.PhongBans = db.PhongBans.ToList();
            ViewBag.ChucVus = db.ChucVus.ToList();
            return View(data);
        }
        public ActionResult DetailsPartial(int id)
        {
            var nv = db.NhanViens
                .Include(x => x.PhongBan)
                .Include(x => x.ChucVu)
                .Include(x => x.Accounts)
                .FirstOrDefault(x => x.MaNV == id);

            if (nv == null)
                return HttpNotFound();
            ViewBag.PhongBans = db.PhongBans.ToList();
            ViewBag.ChucVus = db.ChucVus.ToList();
            return PartialView(nv);
        }
    }
}