using BTL.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace BTL.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext db = new AppDbContext();
        private readonly DataWebEntities dbEf = new DataWebEntities();

        public ActionResult Index()
        {
            ViewBag.NoiBat = db.TinTucs
                .Where(t => !t.IsDeleted && t.NoiBat)
                .OrderByDescending(t => t.NgayDang)
                .Take(3)
                .ToList();

            ViewBag.TinMoi = db.TinTucs
                .Where(t => !t.IsDeleted)
                .OrderByDescending(t => t.NgayDang)
                .Take(6)
                .ToList();

            ViewBag.SoNhanVien = dbEf.NhanViens.Count();
            ViewBag.SoPhongBan = dbEf.PhongBans.Count();
            ViewBag.SoChucVu   = dbEf.ChucVus.Count();

            return View();
        }

        public ActionResult GioiThieu()
        {
            return View();
        }

        public ActionResult CoCauToChuc()
        {
            ViewBag.PhongBans = dbEf.PhongBans.ToList();
            ViewBag.ChucVus   = dbEf.ChucVus.ToList();
            return View();
        }

        public ActionResult TinTuc(string loai = null)
        {
            var q = db.TinTucs.Where(t => !t.IsDeleted);
            if (!string.IsNullOrEmpty(loai))
                q = q.Where(t => t.LoaiTin == loai);
            ViewBag.Loai = loai;
            return View(q.OrderByDescending(t => t.NgayDang).ToList());
        }

        public ActionResult ChiTiet(int id)
        {
            var tin = db.TinTucs.FirstOrDefault(t => t.MaTin == id && !t.IsDeleted);
            if (tin == null) return HttpNotFound();
            ViewBag.LienQuan = db.TinTucs
                .Where(t => !t.IsDeleted && t.MaTin != id && t.LoaiTin == tin.LoaiTin)
                .OrderByDescending(t => t.NgayDang)
                .Take(4)
                .ToList();
            return View(tin);
        }

        public ActionResult LienHe()
        {
            return View(new LienHe());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LienHe(LienHe model)
        {
            if (!ModelState.IsValid) return View(model);
            model.NgayGui = DateTime.Now;
            model.DaXuLy = false;
            db.LienHes.Add(model);
            db.SaveChanges();
            TempData["Success"] = "Cam on ban da gui lien he. Chung toi se phan hoi som nhat.";
            return RedirectToAction("LienHe");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db?.Dispose();
                dbEf?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
