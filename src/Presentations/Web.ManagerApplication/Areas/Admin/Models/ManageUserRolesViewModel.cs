using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.ManagerApplication.Areas.Admin.Models
{
    public class ManageUserRolesViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public IList<UserRolesViewModel> UserRoles { get; set; }
    }

    public class UserRolesViewModel
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public bool Selected { get; set; }
    }
}
