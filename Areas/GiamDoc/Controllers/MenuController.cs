using BTL.Models;
using System.Linq;
using System.Web.Mvc;

namespace BTL.Areas.GiamDoc.Controllers
{
    public class MenuController : Controller
    {
        private readonly DataWebEntities db = new DataWebEntities();
        private ActionResult Guard() => Session["RoleID"] == null ? Redirect(Url.Action("LoginIndex", "Login", new { area = "" })) : null;

        public ActionResult Index() => Guard() ?? View(db.Menus.OrderBy(m => m.MenuOrder).ToList());

        public ActionResult Create() => Guard() ?? View(new Menu { MenuOrder = 99 });

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(Menu m)
        {
            if (!ModelState.IsValid) return View(m);
            db.Menus.Add(m); db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var g = Guard(); if (g != null) return g;
            var m = db.Menus.Find(id);
            return m == null ? (ActionResult)HttpNotFound() : View(m);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(Menu m)
        {
            if (!ModelState.IsValid) return View(m);
            db.Entry(m).State = System.Data.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var g = Guard(); if (g != null) return g;
            var m = db.Menus.Find(id);
            if (m == null) return HttpNotFound();
            db.Menus.Remove(m); db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing) { if (disposing) db?.Dispose(); base.Dispose(disposing); }
    }
}
