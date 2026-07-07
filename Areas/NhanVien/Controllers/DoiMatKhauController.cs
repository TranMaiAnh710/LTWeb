using BTL.Models;
using System.Linq;
using System.Web.Mvc;

namespace BTL.Areas.NhanVien.Controllers
{
    public class DoiMatKhauController : Controller
    {
        private readonly DataWebEntities db = new DataWebEntities();

        public ActionResult Index()
        {
            if (Session["UserID"] == null) return Redirect(Url.Action("LoginIndex", "Login", new { area = "" }));
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Index(string oldPassword, string newPassword, string confirmPassword)
        {
            if (Session["UserID"] == null) return Redirect(Url.Action("LoginIndex", "Login", new { area = "" }));
            int uid = (int)Session["UserID"];
            var acc = db.Accounts.FirstOrDefault(a => a.UserID == uid);
            if (acc == null) return HttpNotFound();

            if (acc.PasswordHash != oldPassword) { ViewBag.Error = "Mật khẩu cũ không đúng."; return View(); }
            if (string.IsNullOrEmpty(newPassword) || newPassword.Length < 4) { ViewBag.Error = "Mật khẩu mới phải có ít nhất 4 ký tự."; return View(); }
            if (newPassword != confirmPassword) { ViewBag.Error = "Xác nhận mật khẩu không khớp."; return View(); }

            acc.PasswordHash = newPassword;
            db.SaveChanges();
            ViewBag.Success = "Đổi mật khẩu thành công!";
            return View();
        }

        protected override void Dispose(bool disposing) { if (disposing) db?.Dispose(); base.Dispose(disposing); }
    }
}
