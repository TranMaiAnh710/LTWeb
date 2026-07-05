using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BTL.Models
{
    public class RoleMenuVM
    {
        public List<Role> Roles { get; set; }

        public List<Menu> Menus { get; set; }

        public List<Role_Menu> RoleMenus { get; set; }
    }
}