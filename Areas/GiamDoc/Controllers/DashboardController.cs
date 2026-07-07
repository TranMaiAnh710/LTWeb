using BTL.Models;
using System.Linq;
using System.Web.Mvc;

namespace BTL.Areas.GiamDoc.Controllers
{
    public class DashboardController : Controller
    {
        private readonly DataWebEntities db = new DataWebEntities();
        private readonly AppDbContext dbApp = new AppDbContext();

        public ActionResult Index()
        {
            if (Session["RoleID"] == null)
                return Redirect(Url.Action("LoginIndex", "Login", new { area = "" }));

            ViewBag.SoNhanVien = db.NhanViens.Count();
            ViewBag.SoPhongBan = db.PhongBans.Count();
            ViewBag.SoChucVu   = db.ChucVus.Count();
            ViewBag.SoHopDong  = dbApp.HopDongs.Count(h => h.TrangThai == "HieuLuc");
            ViewBag.SoCa       = dbApp.CaLamViecs.Count();
            ViewBag.SoLienHe   = dbApp.LienHes.Count(l => !l.DaXuLy);

            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) { db?.Dispose(); dbApp?.Dispose(); }
            base.Dispose(disposing);
        }
    }
}
