using CollegeManagement.Areas.Admin.Models;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CollegeManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="Admin")]
    public class HomeController : Controller
    {
        private readonly UserManager<Entities.ApplicationUser>  _user;
        private readonly RoleManager<ApplicationRole> _role;

        public HomeController(UserManager<ApplicationUser> user,RoleManager<ApplicationRole> role)
        {
            this._user = user;
            this._role = role;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetRegisteredUsersWithRoles()
        {
            var users = _user.Users.ToList();
            var roles = _role.Roles.ToList();

            var usersWithRoles = new List<UserRolesViewModel>();

            foreach (var user in users)
            {
                var userRoles = await _user.GetRolesAsync(user);

                usersWithRoles.Add(new UserRolesViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Roles = userRoles.ToList() // Ensure it is a valid List<string>
                });
            }

            var model = new ManageUserRolesViewModel
            {
                Users = usersWithRoles ?? new List<UserRolesViewModel>(), // Ensure non-null list
                Roles = roles ?? new List<ApplicationRole>() // Ensure non-null list
            };

            return View(model); // ✅ Always pass a valid model
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole([FromBody] UserRoleUpdateModel model)
        {
            var user = await _user.FindByIdAsync(model.UserId.ToString());
            if (user == null)
                return Json(new { success = false, message = "User not found" });

            var result = await _user.AddToRoleAsync(user, model.RoleName);
            if (result.Succeeded)
                return Json(new { success = true });

            return Json(new { success = false, message = "Failed to assign role" });
        }

        [HttpPost]
        public async Task<IActionResult> RevokeRole([FromBody] UserRoleUpdateModel model)
        {
            var user = await _user.FindByIdAsync(model.UserId.ToString());
            if (user == null)
                return Json(new { success = false, message = "User not found" });

            var loggedInUser = await _user.GetUserAsync(User); // Get the currently logged-in user
            var isLoggedInUserAdmin = await _user.IsInRoleAsync(loggedInUser, "Admin"); // Check if they are an Admin

            // Prevent removing the "Admin" role from the primary admin account
            if (user.UserName == "Admin@gmail.com" && model.RoleName == "Admin")
            {
                return Json(new { success = false, message = "Admin role cannot be revoked from the primary admin account." });
            }

            // Prevent a logged-in Admin from removing their own Admin role
            if (loggedInUser.Id == user.Id && model.RoleName == "Admin")
            {
                return Json(new { success = false, message = "You cannot remove your own Admin role." });
            }

            var result = await _user.RemoveFromRoleAsync(user, model.RoleName);
            if (result.Succeeded)
                return Json(new { success = true });

            return Json(new { success = false, message = "Failed to revoke role" });
        }


    }


}
