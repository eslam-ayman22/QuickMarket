using E_Commerce.Migrations;
using E_Commerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UserModel = E_Commerce.Models.UserModel;

namespace E_Commerce.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AddUserController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;

        public AddUserController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserModel user)
        {
            if (ModelState.IsValid)
            {
                var newuser = new ApplicationUser
                {
                    UserName = user.Username,
                    Email = user.Email,
                    Adress = user.Adress
                };
                var result = await userManager.CreateAsync(newuser, user.Password);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(user.Role))
                    {
                        await userManager.AddToRoleAsync(newuser, user.Role);
                    }
                    TempData["add"] = "User Added successfully";
                    return View();
                }
                else
                {
                    foreach (var erorr in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, erorr.Description);
                    }
                }
            }
            return View(user);

        }
    }
}
