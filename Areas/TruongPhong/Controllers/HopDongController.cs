using BTL.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace BTL.Areas.TruongPhong.Controllers
{
    public class HopDongController : Controller
    {
        private readonly AppDbContext dbApp = new AppDbContext();
        private readonly DataWebEntities db = new DataWebEntities();
        private ActionResult Guard() => Session["RoleID"] == null ? Redirect(Url.Action("LoginIndex", "Login", new { area = "" })) : null;

        public ActionResult Index()
        {
            var g = Guard(); if (g != null) return g;
            var hd = dbApp.HopDongs.OrderByDescending(x => x.MaHD).ToList();
            var nvs = db.NhanViens.ToDictionary(n => n.MaNV, n => n.HotenNV);
            ViewBag.NhanViens = nvs;
            return View(hd);
        }

        public ActionResult Create()
        {
            var g = Guard(); if (g != null) return g;
            ViewBag.NhanViens = new SelectList(db.NhanViens.ToList(), "MaNV", "HotenNV");
            return View(new HopDong { NgayBatDau = DateTime.Today, NgayKy = DateTime.Today, TrangThai = "HieuLuc" });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(HopDong m)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.NhanViens = new SelectList(db.NhanViens.ToList(), "MaNV", "HotenNV", m.MaNV);
                return View(m);
            }
            m.CreatedAt = DateTime.Now;
            dbApp.HopDongs.Add(m); dbApp.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var g = Guard(); if (g != null) return g;
            var m = dbApp.HopDongs.Find(id);
            if (m == null) return HttpNotFound();
            ViewBag.NhanViens = new SelectList(db.NhanViens.ToList(), "MaNV", "HotenNV", m.MaNV);
            return View(m);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(HopDong m)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.NhanViens = new SelectList(db.NhanViens.ToList(), "MaNV", "HotenNV", m.MaNV);
                return View(m);
            }
            dbApp.Entry(m).State = System.Data.EntityState.Modified;
            dbApp.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var g = Guard(); if (g != null) return g;
            var m = dbApp.HopDongs.Find(id);
            if (m == null) return HttpNotFound();
            dbApp.HopDongs.Remove(m); dbApp.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Details(int id)
        {
            var g = Guard(); if (g != null) return g;
            var m = dbApp.HopDongs.Find(id);
            if (m == null) return HttpNotFound();
            ViewBag.NhanVien = db.NhanViens.FirstOrDefault(n => n.MaNV == m.MaNV);
            return View(m);
        }

        protected override void Dispose(bool disposing) { if (disposing) { dbApp?.Dispose(); db?.Dispose(); } base.Dispose(disposing); }
    }
}
