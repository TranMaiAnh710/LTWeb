using BTL.Models;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace BTL.Areas.GiamDoc.Controllers
{
    public class TraCuuController : Controller
    {
        private readonly DataWebEntities db = new DataWebEntities();

        public ActionResult NhanVien(string keyword)
        {
            if (Session["RoleID"] == null) return Redirect(Url.Action("LoginIndex", "Login", new { area = "" }));

            var q = db.NhanViens.Include(x => x.PhongBan).Include(x => x.ChucVu).AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
                q = q.Where(x => x.HotenNV.Contains(keyword));
            ViewBag.Keyword = keyword;
            return View(q.ToList());
        }

        public ActionResult PhongBan() =>
            Session["RoleID"] == null
                ? Redirect(Url.Action("LoginIndex", "Login", new { area = "" }))
                : (ActionResult)View(db.PhongBans.ToList());

        public ActionResult ChucVu() =>
            Session["RoleID"] == null
                ? Redirect(Url.Action("LoginIndex", "Login", new { area = "" }))
                : (ActionResult)View(db.ChucVus.ToList());

        protected override void Dispose(bool disposing)
        {
            if (disposing) db?.Dispose();
            base.Dispose(disposing);
        }
    }
}
