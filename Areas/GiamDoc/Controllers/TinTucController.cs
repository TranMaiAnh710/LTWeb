using BTL.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace BTL.Areas.GiamDoc.Controllers
{
    public class TinTucController : Controller
    {
        private readonly AppDbContext dbApp = new AppDbContext();
        private ActionResult Guard() => Session["RoleID"] == null ? Redirect(Url.Action("LoginIndex", "Login", new { area = "" })) : null;

        public ActionResult Index() => Guard() ?? View(dbApp.TinTucs.Where(x => !x.IsDeleted).OrderByDescending(x => x.NgayDang).ToList());

        public ActionResult Create() => Guard() ?? View(new TinTuc { NgayDang = DateTime.Now, LoaiTin = "TinTuc", TacGia = Session["UserName"] as string ?? "Giam doc" });

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(TinTuc m)
        {
            if (!ModelState.IsValid) return View(m);
            dbApp.TinTucs.Add(m); dbApp.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var g = Guard(); if (g != null) return g;
            var m = dbApp.TinTucs.Find(id);
            return m == null ? (ActionResult)HttpNotFound() : View(m);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(TinTuc m)
        {
            if (!ModelState.IsValid) return View(m);
            dbApp.Entry(m).State = System.Data.EntityState.Modified;
            dbApp.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var g = Guard(); if (g != null) return g;
            var m = dbApp.TinTucs.Find(id);
            if (m == null) return HttpNotFound();
            m.IsDeleted = true; dbApp.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing) { if (disposing) dbApp?.Dispose(); base.Dispose(disposing); }
    }
}
