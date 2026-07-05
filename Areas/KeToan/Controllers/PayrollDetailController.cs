using BTL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace BTL.Areas.KeToan.Controllers
{
    public class PayrollDetailController : Controller
    {
        DataWebEntities db = new DataWebEntities();

        // GET: KeToan/PayrollDetail
        public ActionResult Index(string search, int? maNV)
        {
            try
            {
                var query = db.NhatKyLuongs
                    .Include(x => x.NhanVien)
                    .AsQueryable();

                // SEARCH
                if (!string.IsNullOrEmpty(search))
                {
                    int maNVSearch;

                    if (int.TryParse(search, out maNVSearch))
                    {
                        query = query.Where(x =>
                            x.NhanVien.HotenNV.Contains(search) ||
                            x.NhanVien.MaNV == maNVSearch);
                    }
                    else
                    {
                        query = query.Where(x =>
                            x.NhanVien.HotenNV.Contains(search));
                    }
                }

                // FILTER
                if (maNV.HasValue)
                {
                    query = query.Where(x => x.MaNV == maNV.Value);
                }

                var dsNhatKy = query
                    .OrderByDescending(x => x.NgayApDung)
                    .ToList();

                ViewBag.NhanVienList = new SelectList(
                    db.NhanViens.ToList(),
                    "MaNV",
                    "HotenNV"
                );

                ViewBag.HotenNV = Session["HotenNV"];
                ViewBag.TenCV = Session["TenCV"];

                return View(dsNhatKy);
            }
            catch (Exception ex)
            {
                return Content(ex.InnerException?.Message ?? ex.Message);
            }
        }
    }
}