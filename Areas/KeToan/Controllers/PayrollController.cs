using BTL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using OfficeOpenXml;
using System.IO;

namespace BTL.Areas.KeToan.Controllers
{
    public class PayrollController : Controller
    {
        DataWebEntities db = new DataWebEntities();

        // =========================
        // DANH SÁCH PHIẾU LƯƠNG
        // =========================
        public ActionResult Index(string search, int? thang, int? nam)
        {
            if (Session["RoleID"] == null)
                return RedirectToAction("LoginIndex", "Login", new { area = "" });

            var data = db.PhieuLuongs
                .Include(p => p.NhanVien)
                .Include(p => p.NhanVien.ChucVu)
                .Include(p => p.NhanVien.PhongBan)
                .AsQueryable();

            // SEARCH NHÂN VIÊN
            if (!string.IsNullOrEmpty(search))
            {
                data = data.Where(x =>
                    x.NhanVien.HotenNV.Contains(search));
            }

            // LỌC THÁNG
            if (thang.HasValue)
            {
                data = data.Where(x => x.Thang == thang.Value);
            }

            // LỌC NĂM
            if (nam.HasValue)
            {
                data = data.Where(x => x.Nam == nam.Value);
            }

            var result = data.ToList();

            // =========================
            // SUMMARY
            // =========================

            ViewBag.TongLuong = result.Sum(x => x.TongLuong);

            ViewBag.DaTra = result.Count(x => x.TrangThai == true);

            ViewBag.ChoTra = result.Count(x => x.TrangThai == false);

            ViewBag.TongKhauTru = result.Sum(x =>
                (x.BHYT ?? 0m) +
                (x.BHXH ?? 0m) +
                (x.Thue ?? 0m) +
                (x.Phat ?? 0m)
            );

            ViewBag.PhongBans = db.PhongBans.ToList();
            ViewBag.ChucVus = db.ChucVus.ToList();

            return View(result);
        }

        // =========================
        // DUYỆT PHIẾU LƯƠNG
        // =========================
        public ActionResult Duyet(int? id)
        {
            if (id == null)
                return RedirectToAction("Index");

            var phieu = db.PhieuLuongs.Find(id);

            if (phieu != null)
            {
                phieu.TrangThai = true;
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // =========================
        // CHI TIẾT PHIẾU LƯƠNG
        // =========================
        public ActionResult Detail(int? id)
        {
            if (id == null)
                return HttpNotFound();

            var phieu = db.PhieuLuongs
                .Include(x => x.NhanVien)
                .Include(x => x.NhanVien.ChucVu)
                .Include(x => x.NhanVien.PhongBan)
                .FirstOrDefault(x => x.MaBL == id);

            if (phieu == null)
                return HttpNotFound();

            return PartialView(phieu);
        }
        // =========================
        // XUẤT EXCEL
        // =========================
        public ActionResult ExportExcel(string search, int? thang, int? nam)
        {
            var data = db.PhieuLuongs
                .Include(p => p.NhanVien)
                .Include(p => p.NhanVien.ChucVu)
                .Include(p => p.NhanVien.PhongBan)
                .AsQueryable();

            // SEARCH
            if (!string.IsNullOrEmpty(search))
            {
                data = data.Where(x =>
                    x.NhanVien.HotenNV.Contains(search));
            }

            // THÁNG
            if (thang.HasValue)
            {
                data = data.Where(x => x.Thang == thang.Value);
            }

            // NĂM
            if (nam.HasValue)
            {
                data = data.Where(x => x.Nam == nam.Value);
            }

            var result = data.ToList();

            ExcelPackage.License.SetNonCommercialPersonal("MaiAnh");

            using (ExcelPackage package = new ExcelPackage())
            {
                var ws = package.Workbook.Worksheets.Add("BangLuong");

                // HEADER
                ws.Cells[1, 1].Value = "Mã BL";
                ws.Cells[1, 2].Value = "Nhân viên";
                ws.Cells[1, 3].Value = "Tháng";
                ws.Cells[1, 4].Value = "Năm";
                ws.Cells[1, 5].Value = "Ngày công";
                ws.Cells[1, 6].Value = "Tăng ca";
                ws.Cells[1, 7].Value = "Lương CB";
                ws.Cells[1, 8].Value = "Thưởng";
                ws.Cells[1, 9].Value = "Phạt";
                ws.Cells[1, 10].Value = "BHYT";
                ws.Cells[1, 11].Value = "BHXH";
                ws.Cells[1, 12].Value = "Thuế";
                ws.Cells[1, 13].Value = "Tổng lương";
                ws.Cells[1, 14].Value = "Trạng thái";

                int row = 2;

                foreach (var item in result)
                {
                    ws.Cells[row, 1].Value = item.MaBL;
                    ws.Cells[row, 2].Value = item.NhanVien?.HotenNV;
                    ws.Cells[row, 3].Value = item.Thang;
                    ws.Cells[row, 4].Value = item.Nam;
                    ws.Cells[row, 5].Value = item.SoNgayCong;
                    ws.Cells[row, 6].Value = item.SoGioTangCa;
                    ws.Cells[row, 7].Value = item.LuongCBan;
                    ws.Cells[row, 8].Value = item.Thuong ?? 0;
                    ws.Cells[row, 9].Value = item.Phat ?? 0;
                    ws.Cells[row, 10].Value = item.BHYT ?? 0;
                    ws.Cells[row, 11].Value = item.BHXH ?? 0;
                    ws.Cells[row, 12].Value = item.Thue ?? 0;
                    ws.Cells[row, 13].Value = item.TongLuong;
                    ws.Cells[row, 14].Value = item.TrangThai == true
                        ? "Đã trả"
                        : "Chờ trả";

                    row++;
                }

                ws.Cells.AutoFitColumns();

                var stream = new MemoryStream(package.GetAsByteArray());

                return File(
                    stream,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "BangLuong.xlsx"
                );
            }
        }
    }
}