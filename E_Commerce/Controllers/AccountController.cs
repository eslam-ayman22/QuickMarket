using E_Commerce.Models;
using E_Commerce.Models.View_Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace E_Commerce.Controllers
{
   
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public AccountController(UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(ApplicationUserVm userVm)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new()
                {
                    UserName = userVm.UserName,
                    Email = userVm.Email,
                    Adress = userVm.Adress
                };
              var result =  await userManager.CreateAsync(user , userVm.Password);
                
                    if (result.Succeeded)
                    {
                        
                            await userManager.AddToRoleAsync(user, "Customer");
                        await signInManager.SignInAsync(user, false);
                        return RedirectToAction("Index", "Home");

                    }
                ModelState.AddModelError("Password", "Don't match the constrains");
            }
            return View(userVm);
        }

        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(RoleVm roleVm)
        {
            if (ModelState.IsValid)
            {
                IdentityRole user = new(roleVm.Name);

               await roleManager.CreateAsync(user);
            }
            return View(roleVm);
        }



        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Login(LoginVm loginVm)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(loginVm.Email);

                if (user != null)
                {
                    var result = await userManager.CheckPasswordAsync(user, loginVm.Password);
                    if (result)
                    {
                        await signInManager.SignInAsync(user, loginVm.RememberMe);
                        if (User.IsInRole("Admin"))
                        {
                            return RedirectToAction("Index", "Admin");
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Password", "Inncorect Password");
                    }

                }
                ModelState.AddModelError("Email", "Inncorect Email");
            }
            return View(loginVm);
        }

        public IActionResult Logout()
        {
            signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
