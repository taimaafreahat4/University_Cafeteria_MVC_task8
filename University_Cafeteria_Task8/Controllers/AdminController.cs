
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using University_Cafeteria_Task8.Data;

namespace University_Cafeteria_Task8.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // READ - Display all users
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Users = _userManager.Users.ToList();
            ViewBag.Roles = _roleManager.Roles.ToList();

            return View();
        }

        // UPDATE - Assign Role
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AssignRole(string userId, string roleId)
        {
            var user = _userManager.FindByIdAsync(userId)
                .GetAwaiter().GetResult();

            var role = _roleManager.FindByIdAsync(roleId)
                .GetAwaiter().GetResult();

            if (user == null || role == null)
                return NotFound();

            if (user.Id == _userManager.GetUserId(User))
                return BadRequest("You cannot change your own role.");

            var oldRoles = _userManager.GetRolesAsync(user)
                .GetAwaiter().GetResult();

            if (oldRoles.Contains("SuperAdmin"))
                return BadRequest("Cannot change another SuperAdmin.");

            if (oldRoles.Contains(role.Name!))
                return RedirectToAction("Index");

            var addResult = _userManager
                .AddToRoleAsync(user, role.Name!)
                .GetAwaiter().GetResult();

            if (!addResult.Succeeded)
                return BadRequest("Cannot assign role.");

            var rolesToRemove = oldRoles
                .Where(x => x != role.Name)
                .ToList();

            if (rolesToRemove.Count > 0)
            {
                var removeResult = _userManager
                    .RemoveFromRolesAsync(user, rolesToRemove)
                    .GetAwaiter().GetResult();

                if (!removeResult.Succeeded)
                    return BadRequest("Could not remove old roles.");
            }

            return RedirectToAction("Index");
        }

        // DELETE - Delete User
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteUser(string userId)
        {
            var user = _userManager.FindByIdAsync(userId)
                .GetAwaiter().GetResult();

            if (user == null)
                return NotFound();

            if (user.Id == _userManager.GetUserId(User))
                return BadRequest("You cannot delete yourself.");

            var roles = _userManager.GetRolesAsync(user)
                .GetAwaiter().GetResult();

            if (roles.Contains("SuperAdmin"))
                return BadRequest("Cannot delete SuperAdmin.");

            // Prevent deleting users who have supply requests
            // because SupplyRequests has a UserId foreign key.
            // We'll add the database check in the next step.

            return BadRequest(
                "User deletion is temporarily disabled until the supply request relationship is checked.");
        }
    }
}
