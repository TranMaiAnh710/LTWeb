using BTL.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace BTL.Areas.GiamDoc.Controllers
{
    public class BaoCaoController : Controller
    {
        private readonly DataWebEntities db = new DataWebEntities();
        private readonly AppDbContext dbApp = new AppDbContext();

        public ActionResult Index()
        {
            if (Session["RoleID"] == null) return Redirect(Url.Action("LoginIndex", "Login", new { area = "" }));
            return View();
        }

        public ActionResult NhanVien()
        {
            if (Session["RoleID"] == null) return Redirect(Url.Action("LoginIndex", "Login", new { area = "" }));

            var theoPhongBan = db.NhanViens
                .GroupBy(x => x.PhongBan.TenPB)
                .Select(g => new { TenPB = g.Key, SoLuong = g.Count() })
                .ToList();

            var theoChucVu = db.NhanViens
                .GroupBy(x => x.ChucVu.TenCV)
                .Select(g => new { TenCV = g.Key, SoLuong = g.Count() })
                .ToList();

            ViewBag.TheoPhongBan = theoPhongBan;
            ViewBag.TheoChucVu = theoChucVu;
            ViewBag.Tong = db.NhanViens.Count();
            return View();
        }

        public ActionResult HopDong()
        {
            if (Session["RoleID"] == null) return Redirect(Url.Action("LoginIndex", "Login", new { area = "" }));

            var theoLoai = dbApp.HopDongs
                .GroupBy(h => h.LoaiHopDong)
                .Select(g => new { Loai = g.Key, SoLuong = g.Count() })
                .ToList();

            var theoTT = dbApp.HopDongs
                .GroupBy(h => h.TrangThai)
                .Select(g => new { TT = g.Key, SoLuong = g.Count() })
                .ToList();

            ViewBag.TheoLoai = theoLoai;
            ViewBag.TheoTrangThai = theoTT;
            ViewBag.Tong = dbApp.HopDongs.Count();
            return View();
        }

        public ActionResult ChamCong(int? thang, int? nam)
        {
            if (Session["RoleID"] == null) return Redirect(Url.Action("LoginIndex", "Login", new { area = "" }));

            var t = thang ?? DateTime.Now.Month;
            var n = nam   ?? DateTime.Now.Year;
            ViewBag.Thang = t;
            ViewBag.Nam = n;

            var data = db.ChamCongs
                .Where(c => c.Ngay.Month == t && c.Ngay.Year == n)
                .GroupBy(c => c.NhanVien.HotenNV)
                .Select(g => new { HoTen = g.Key, SoNgay = g.Count() })
                .ToList();

            ViewBag.Data = data;
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) { db?.Dispose(); dbApp?.Dispose(); }
            base.Dispose(disposing);
        }
    }
}
