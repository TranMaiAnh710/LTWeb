using BTL.Models;
using System.Linq;
using System.Web.Mvc;

namespace BTL.Areas.NhanVien.Controllers
{
    public class ChamCongController : Controller
    {
        private readonly DataWebEntities db = new DataWebEntities();

        public ActionResult Index()
        {
            if (Session["MaNV"] == null) return Redirect(Url.Action("LoginIndex", "Login", new { area = "" }));
            int maNV = (int)Session["MaNV"];
            var data = db.ChamCongs.Where(c => c.MaNV == maNV).OrderByDescending(c => c.Ngay).ToList();
            return View(data);
        }

        protected override void Dispose(bool disposing) { if (disposing) db?.Dispose(); base.Dispose(disposing); }
    }
}
