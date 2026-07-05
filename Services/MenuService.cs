using BTL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BTL.Services
{
    public class MenuService
    {
        DataWebEntities db = new DataWebEntities();

        public List<Menu> GetMenuByRole(int roleId)
        {
            return db.Role_Menu
                .Where(x => x.RoleID == roleId && x.IsVisible)
                .Select(x => x.Menu)
                .OrderBy(x => x.MenuOrder)
                .ToList();
        }
    }
}