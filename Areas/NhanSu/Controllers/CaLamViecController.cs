using BTL.Models;
using System.Linq;
using System.Web.Mvc;

namespace BTL.Areas.NhanSu.Controllers
{
    public class CaLamViecController : Controller
    {
        private readonly AppDbContext dbApp = new AppDbContext();
        private ActionResult Guard() => Session["RoleID"] == null ? Redirect(Url.Action("LoginIndex", "Login", new { area = "" })) : null;

        public ActionResult Index() => Guard() ?? View(dbApp.CaLamViecs.OrderBy(x => x.MaCa).ToList());
        public ActionResult Create() => Guard() ?? View(new CaLamViec { TrangThai = true });

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(CaLamViec m)
        {
            if (!ModelState.IsValid) return View(m);
            dbApp.CaLamViecs.Add(m); dbApp.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var g = Guard(); if (g != null) return g;
            var m = dbApp.CaLamViecs.Find(id);
            return m == null ? (ActionResult)HttpNotFound() : View(m);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(CaLamViec m)
        {
            if (!ModelState.IsValid) return View(m);
            dbApp.Entry(m).State = System.Data.EntityState.Modified;
            dbApp.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing) { if (disposing) dbApp?.Dispose(); base.Dispose(disposing); }
    }
}
