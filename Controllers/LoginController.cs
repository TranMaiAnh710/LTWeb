using BTL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BTL.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        DataWebEntities db = new DataWebEntities();

        public ActionResult LoginIndex()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            var user = db.Accounts.FirstOrDefault(x =>x.UserName == username && x.PasswordHash == password && (x.IsDeleted ?? false) == false);

            if (user != null)
            {
                Session["UserID"] = user.UserID;
                Session["RoleID"] = user.RoleID;
                Session["MaNV"] = user.MaNV;

                switch (user.RoleID)
                {
                    case 2: return RedirectToAction("Index", "Dashboard", new { area = "TruongPhong" });
                    case 4: return RedirectToAction("Index", "Dashboard", new { area = "KeToan" });
                    case 5: return RedirectToAction("Index", "Dashboard", new { area = "NhanSu" });
                    case 6: return RedirectToAction("Index", "Dashboard", new { area = "NhanVien" });
                    case 7: return RedirectToAction("Index", "Dashboard", new { area = "GiamDoc" });
                }
            }

            ViewBag.Error = "Sai tài khoản";
            return View("LoginIndex");
        }

        // GET: Hiển thị form
        public ActionResult ResetPassword()
        {
            return View();
        }

        // POST: Xử lý đổi mật khẩu
        [HttpPost]
        public ActionResult ResetPassword(string username, string oldPassword, string newPassword)
        {
            var user = db.Accounts.FirstOrDefault(x => x.UserName == username && (x.IsDeleted ?? false) == false);

            if (user == null)
            {
                ViewBag.Error = "Email không tồn tại";
                return View();
            }

            if (user.PasswordHash != oldPassword)
            {
                ViewBag.Error = "Mật khẩu cũ không đúng";
                return View();
            }

            user.PasswordHash = newPassword;
            db.SaveChanges();

            ViewBag.Success = "Đổi mật khẩu thành công";
            return View();
        }
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("LoginIndex");
        }
    }
}