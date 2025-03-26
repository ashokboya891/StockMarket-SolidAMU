using Entities;
using Microsoft.AspNetCore.Identity;

namespace CollegeManagement.Areas.Admin.Models
{
    public class UserRolesViewModel
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public List<string> Roles { get; set; }
    }

    public class ManageUserRolesViewModel
    {
        public List<UserRolesViewModel> Users { get; set; }
        public List<ApplicationRole> Roles { get; set; } // ✅ Change from IdentityRole to ApplicationRole
    }
}
