using BTL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BTL.Areas.TruongPhong.Controllers
{
    public class MenuController : Controller
    {
        DataWebEntities db = new DataWebEntities();

        public ActionResult Index()
        {
            if (Session["RoleID"] == null || (int)Session["RoleID"] != 2)
                return RedirectToAction("LoginIndex", "Login", new { area = "" });
            var vm = new RoleMenuVM
            {
                Roles = db.Roles.ToList(),
                Menus = db.Menus.ToList(),
                RoleMenus = db.Role_Menu.ToList()
            };

            return View(vm);
        }
        [HttpPost]
        public JsonResult ToggleRoleMenu(int roleId, int menuId, bool isVisible)
        {
            var rm = db.Role_Menu
                .FirstOrDefault(x => x.RoleID == roleId && x.MenuID == menuId);

            // chưa có thì tạo mới
            if (rm == null)
            {
                rm = new Role_Menu
                {
                    RoleID = roleId,
                    MenuID = menuId,
                    IsVisible = isVisible,
                    IsDeleted = false
                };

                db.Role_Menu.Add(rm);
            }
            else
            {
                rm.IsVisible = isVisible;
                rm.IsDeleted = !isVisible;
            }

            db.SaveChanges();

            return Json(new
            {
                success = true
            });
        }
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();

            return RedirectToAction("LoginIndex", "Login", new { area = "" });
        }
    }
}