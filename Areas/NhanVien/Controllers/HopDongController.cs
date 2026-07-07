using BTL.Models;
using System.Linq;
using System.Web.Mvc;

namespace BTL.Areas.NhanVien.Controllers
{
    public class HopDongController : Controller
    {
        private readonly AppDbContext dbApp = new AppDbContext();

        public ActionResult Index()
        {
            if (Session["MaNV"] == null) return Redirect(Url.Action("LoginIndex", "Login", new { area = "" }));
            int maNV = (int)Session["MaNV"];
            var hd = dbApp.HopDongs.Where(x => x.MaNV == maNV).OrderByDescending(x => x.MaHD).ToList();
            return View(hd);
        }

        protected override void Dispose(bool disposing) { if (disposing) dbApp?.Dispose(); base.Dispose(disposing); }
    }
}
