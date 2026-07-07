using BTL.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace BTL.Areas.TruongPhong.Controllers
{
    public class BaoCaoController : Controller
    {
        private readonly DataWebEntities db = new DataWebEntities();
        private readonly AppDbContext dbApp = new AppDbContext();
        private ActionResult Guard() => Session["RoleID"] == null ? Redirect(Url.Action("LoginIndex", "Login", new { area = "" })) : null;

        public ActionResult Index() => Guard() ?? View();

        public ActionResult NhanVien()
        {
            var g = Guard(); if (g != null) return g;
            ViewBag.TheoPhongBan = db.NhanViens.GroupBy(x => x.PhongBan.TenPB).Select(gr => new { TenPB = gr.Key, SoLuong = gr.Count() }).ToList();
            ViewBag.TheoChucVu   = db.NhanViens.GroupBy(x => x.ChucVu.TenCV).Select(gr => new { TenCV = gr.Key, SoLuong = gr.Count() }).ToList();
            ViewBag.Tong = db.NhanViens.Count();
            return View();
        }

        public ActionResult HopDong()
        {
            var g = Guard(); if (g != null) return g;
            ViewBag.TheoLoai       = dbApp.HopDongs.GroupBy(h => h.LoaiHopDong).Select(gr => new { Loai = gr.Key, SoLuong = gr.Count() }).ToList();
            ViewBag.TheoTrangThai  = dbApp.HopDongs.GroupBy(h => h.TrangThai).Select(gr => new { TT = gr.Key, SoLuong = gr.Count() }).ToList();
            ViewBag.Tong = dbApp.HopDongs.Count();
            return View();
        }

        public ActionResult ChamCong(int? thang, int? nam)
        {
            var g = Guard(); if (g != null) return g;
            var t = thang ?? DateTime.Now.Month; var n = nam ?? DateTime.Now.Year;
            ViewBag.Thang = t; ViewBag.Nam = n;
            ViewBag.Data = db.ChamCongs.Where(c => c.Ngay.Month == t && c.Ngay.Year == n)
                .GroupBy(c => c.NhanVien.HotenNV)
                .Select(gr => new { HoTen = gr.Key, SoNgay = gr.Count() }).ToList();
            return View();
        }

        protected override void Dispose(bool disposing) { if (disposing) { db?.Dispose(); dbApp?.Dispose(); } base.Dispose(disposing); }
    }
}
