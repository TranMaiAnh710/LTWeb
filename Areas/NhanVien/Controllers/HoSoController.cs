using BTL.Models;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace BTL.Areas.NhanVien.Controllers
{
    public class HoSoController : Controller
    {
        private readonly DataWebEntities db = new DataWebEntities();

        public ActionResult Index()
        {
            if (Session["MaNV"] == null) return Redirect(Url.Action("LoginIndex", "Login", new { area = "" }));
            int maNV = (int)Session["MaNV"];
            var nv = db.NhanViens.Include(x => x.PhongBan).Include(x => x.ChucVu).FirstOrDefault(x => x.MaNV == maNV);
            if (nv == null) return HttpNotFound();
            return View(nv);
        }

        protected override void Dispose(bool disposing) { if (disposing) db?.Dispose(); base.Dispose(disposing); }
    }
}
