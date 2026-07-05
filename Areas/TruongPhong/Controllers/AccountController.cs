using BTL.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;

namespace BTL.Areas.TruongPhong.Controllers
{
    public class AccountController : Controller
    {
        // GET: TruongPhong/Account
        DataWebEntities db = new DataWebEntities();

        public ActionResult Index(string search, int? roleId)
        {
            if (Session["RoleID"] == null || (int)Session["RoleID"] != 2)
                return RedirectToAction("LoginIndex", "Login", new { area = "" });

            var query = db.Accounts
                .Include(x => x.NhanVien)
                .Include(x => x.NhanVien.PhongBan)
                .Include(x => x.Role);

            // SEARCH
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x =>
                    x.UserName.Contains(search) ||
                    (x.NhanVien != null && x.NhanVien.HotenNV.Contains(search)));
            }

            // FILTER ROLE
            if (roleId.HasValue)
            {
                query = query.Where(x => x.RoleID == roleId);
            }

            ViewBag.Roles = db.Roles.ToList();

            return View(query.ToList());
        }

        // KHÓA / MỞ
        [HttpPost]
        public JsonResult Lock(int id)
        {
            var acc = db.Accounts.Find(id);
            if (acc == null)
                return Json(new { success = false });

            acc.IsDeleted = true;
            db.SaveChanges();

            return Json(new { success = true }); // ⚠️ PHẢI CÓ
        }

        [HttpPost]
        public JsonResult Unlock(int id)
        {
            var acc = db.Accounts.Find(id);
            if (acc == null)
                return Json(new { success = false });

            acc.IsDeleted = false;
            db.SaveChanges();

            return Json(new { success = true });
        }
        [HttpGet]
        public ActionResult CreateAccount()
        {
            ViewBag.Roles = db.Roles.ToList();
            ViewBag.Employees = db.NhanViens
                .Where(x => !db.Accounts.Any(a => a.MaNV == x.MaNV))
                .ToList();

            return View();
        }
        [HttpPost]
        public ActionResult CreateAccount(Account model)
        {
            if (db.Accounts.Any(x => x.UserName == model.UserName))
            {
                TempData["Error"] = "Username đã tồn tại";
                return RedirectToAction("Index");
            }

            model.CreatedAt = DateTime.Now;
            model.IsDeleted = false;

            db.Accounts.Add(model);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();

            return RedirectToAction("LoginIndex", "Login", new { area = "" });
        }
    }
}