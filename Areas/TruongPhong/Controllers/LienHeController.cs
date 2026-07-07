using BTL.Models;
using System.Linq;
using System.Web.Mvc;

namespace BTL.Areas.TruongPhong.Controllers
{
    public class LienHeController : Controller
    {
        private readonly AppDbContext dbApp = new AppDbContext();
        private ActionResult Guard() => Session["RoleID"] == null ? Redirect(Url.Action("LoginIndex", "Login", new { area = "" })) : null;

        public ActionResult Index() => Guard() ?? View(dbApp.LienHes.OrderByDescending(x => x.NgayGui).ToList());

        public ActionResult XuLy(int id)
        {
            var g = Guard(); if (g != null) return g;
            var m = dbApp.LienHes.Find(id);
            if (m == null) return HttpNotFound();
            m.DaXuLy = true; dbApp.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var g = Guard(); if (g != null) return g;
            var m = dbApp.LienHes.Find(id);
            if (m == null) return HttpNotFound();
            dbApp.LienHes.Remove(m); dbApp.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing) { if (disposing) dbApp?.Dispose(); base.Dispose(disposing); }
    }
}
