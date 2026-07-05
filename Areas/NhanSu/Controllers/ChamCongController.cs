using BTL.Models;
using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.Data.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BTL.Areas.NhanSu.Controllers
{
    public class ChamCongController : Controller
    {
        // GET: NhanSu/ChamCong
        DataWebEntities db = new DataWebEntities();

        public ActionResult Index(DateTime? ngay)
        {
            // 🔒 Check login
            if (Session["RoleID"] == null)
                return Redirect(Url.Action("LoginIndex", "Login", new { area = "" }));

            int role = (int)Session["RoleID"];
            int? maNV = Session["MaNV"] as int?;

            // 📌 Query có Include (tránh lỗi view)
            var query = db.ChamCongs
                          .Include(x => x.NhanVien)
                          .Include(x => x.NhanVien.PhongBan)
                          .AsQueryable();

            // 📅 Filter theo ngày (nếu có)
            if (ngay.HasValue)
            {
                var d = ngay.Value.Date;

                query = query.Where(x =>
                    x.Ngay.Year == d.Year &&
                    x.Ngay.Month == d.Month &&
                    x.Ngay.Day == d.Day);
            }

            // 👤 Nhân viên chỉ xem của mình
            if (role == 6 && maNV.HasValue)
            {
                query = query.Where(x => x.MaNV == maNV.Value);
            }

            // 🎯 Đưa data ra view
            var data = query.OrderByDescending(x => x.Ngay).ToList();

            return View(data);
        }
    }
}