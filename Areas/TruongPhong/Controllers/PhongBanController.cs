using BTL.Models;
using System.Linq;
using System.Web.Mvc;

namespace BTL.Areas.TruongPhong.Controllers
{
    public class PhongBanController : Controller
    {
        private readonly DataWebEntities db = new DataWebEntities();

        private ActionResult Guard()
            => Session["RoleID"] == null ? Redirect(Url.Action("LoginIndex", "Login", new { area = "" })) : null;

        public ActionResult Index() => Guard() ?? View(db.PhongBans.ToList());

        public ActionResult Create() => Guard() ?? View(new PhongBan());

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(PhongBan m)
        {
            if (!ModelState.IsValid) return View(m);
            db.PhongBans.Add(m);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var g = Guard(); if (g != null) return g;
            var m = db.PhongBans.Find(id);
            return m == null ? (ActionResult)HttpNotFound() : View(m);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(PhongBan m)
        {
            if (!ModelState.IsValid) return View(m);
            db.Entry(m).State = System.Data.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var g = Guard(); if (g != null) return g;
            var m = db.PhongBans.Find(id);
            if (m == null) return HttpNotFound();
            if (m.NhanViens.Any())
            {
                TempData["Error"] = "Không thể xóa: phòng ban còn nhân viên.";
                return RedirectToAction("Index");
            }
            db.PhongBans.Remove(m);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing) { if (disposing) db?.Dispose(); base.Dispose(disposing); }
    }
}
