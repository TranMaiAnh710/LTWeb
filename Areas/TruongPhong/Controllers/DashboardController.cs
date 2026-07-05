using BTL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Newtonsoft.Json;

namespace BTL.Areas.TruongPhong.Controllers
{
    public class DashboardController : Controller
    {
        DataWebEntities db = new DataWebEntities();

        // GET: TruongPhong/Dashboard
        public ActionResult Index()
        {
            // ===== CHECK LOGIN =====
            if (Session["RoleID"] == null || (int)Session["RoleID"] != 2)
            {
                return RedirectToAction(
                    "LoginIndex",
                    "Login",
                    new { area = "" }
                );
            }

            // ===== USER LOGIN =====
            int accountID = Convert.ToInt32(Session["AccountID"]);

            var acc = db.Accounts
                        .Include(x => x.NhanVien)
                        .Include(x => x.Role)
                        .FirstOrDefault(x => x.UserID == accountID);

            if (acc != null)
            {
                ViewBag.HotenNV =
                    acc.NhanVien != null
                    ? acc.NhanVien.HotenNV
                    : "";

                ViewBag.TenCV =
                    acc.Role != null
                    ? acc.Role.RoleName
                    : "";
            }

            // ===== TỔNG NHÂN VIÊN =====
            ViewBag.TotalEmployee = db.NhanViens.Count();

            // ===== ĐI LÀM HÔM NAY =====
            DateTime today = DateTime.Today;

            int todayWork = db.ChamCongs.Count(x =>
                x.ThoiGianVao.Year == today.Year &&
                x.ThoiGianVao.Month == today.Month &&
                x.ThoiGianVao.Day == today.Day &&
                x.TrangThai == "Đi làm"
            );

            ViewBag.TodayWork = todayWork;

            // ===== TỔNG QUỸ LƯƠNG =====
            decimal totalSalary = db.PhieuLuongs
                                    .Sum(x => (decimal?)x.TongLuong) ?? 0;

            ViewBag.TotalSalary = totalSalary;

            // ===== PHIẾU CHỜ DUYỆT =====
            int pending = db.PhieuLuongs
                            .Count(x => x.TrangThai == false);

            ViewBag.PendingPayroll = pending;

            // ===== NHÂN VIÊN MỚI =====
            var employees = db.NhanViens
                              .Include(x => x.PhongBan)
                              .OrderByDescending(x => x.MaNV)
                              .Take(5)
                              .ToList();

            ViewBag.Employees = employees;

            // ===== PHÒNG BAN =====
            var departments = db.PhongBans
                                .Select(x => new
                                {
                                    TenPB = x.TenPB,
                                    Total = x.NhanViens.Count()
                                })
                                .ToList();

            ViewBag.DepartmentLabels = JsonConvert.SerializeObject(
                 departments.Select(x => x.TenPB)
             );

            ViewBag.DepartmentData = JsonConvert.SerializeObject(
                departments.Select(x => x.Total)
            );

            // ===== BẢNG LƯƠNG =====
            var payrolls = db.PhieuLuongs
                             .Include(x => x.NhanVien)
                             .OrderByDescending(x => x.MaBL)
                             .Take(5)
                             .ToList();

            return View(payrolls);
        }
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();

            return RedirectToAction("LoginIndex", "Login", new { area = "" });
        }
    }
}