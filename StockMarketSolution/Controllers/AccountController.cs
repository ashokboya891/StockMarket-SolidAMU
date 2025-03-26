using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StockMarketSolution.DTO;
using StockMarketSolution.Enum;

namespace StockMarketSolution.Controllers
{
    [Route("[controller]")]
    //[AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;


        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            this._signInManager = signInManager;
            this._roleManager = roleManager;
        }
        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }
        [HttpGet("Login")]
        [Authorize("NotAuthorized")]

        public IActionResult Login()
        {
            return View();
        }
        [Authorize("NotAuthorized")]
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(tem => tem.Errors).Select(tem => tem.ErrorMessage);
                return View(loginDTO);
            }
            var result = await _signInManager.PasswordSignInAsync(loginDTO.Email, loginDTO.Password, isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                ApplicationUser? user = await _userManager.FindByEmailAsync(loginDTO.Email);
                if (user != null)
                {
                    if (await _userManager.IsInRoleAsync(user, UserTypeOptions.Admin.ToString()))
                    {
                        return RedirectToAction("Index", "Home", new { area = "Admin" });
                        //return base.RedirectToAction(nameof(global::HomeController.Dashboard), "Admin");

                    }
                    if (await _userManager.IsInRoleAsync(user, UserTypeOptions.Moderator.ToString()))
                    {
                        return RedirectToAction("Index", "Home", new { area = "Moderator" });
                        //return base.RedirectToAction(nameof(global::HomeController.Dashboard), "Admin");

                    }
                }
                // return RedirectToAction("Index", "Students");
                return RedirectToAction(nameof(StocksController.Explore), "Stocks");
            }
            ModelState.AddModelError("Login", "Invalid email or Password");
            return View(loginDTO);
        }

        [HttpGet]
        [Authorize("NotAuthorized")]

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [Authorize("NotAuthorized")]

        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(tem => tem.Errors).Select(tem => tem.ErrorMessage);
                return View(registerDTO);
            }

            ApplicationUser user = new ApplicationUser()
            {
                PersonName = registerDTO.PersonName,
                UserName = registerDTO.Email,

                Token = registerDTO.Token,
                Email = registerDTO.Email,
                PhoneNumber = registerDTO.PhoneNumber,

            };
            IdentityResult result = await _userManager.CreateAsync(user, registerDTO.Password);
            if (result.Succeeded)
            {
                if (registerDTO.UserType == UserTypeOptions.Admin)
                {
                    if (await _userManager.FindByNameAsync(UserTypeOptions.Admin.ToString()) is null)
                    {
                        ApplicationRole role = new ApplicationRole()
                        {
                            Name = UserTypeOptions.Admin.ToString()
                        };
                        await _roleManager.CreateAsync(role);
                        await _userManager.AddToRoleAsync(user, UserTypeOptions.Admin.ToString());
                    }

                }
                else if (registerDTO.UserType == UserTypeOptions.Moderator)
                {
                    if (await _userManager.FindByNameAsync(UserTypeOptions.Moderator.ToString()) is null)
                    {
                        ApplicationRole role = new ApplicationRole()
                        {
                            Name = UserTypeOptions.Moderator.ToString()
                        };
                        await _roleManager.CreateAsync(role);
                        await _userManager.AddToRoleAsync(user, UserTypeOptions.Moderator.ToString());
                    }
                }
                else
                {
                    if (await _userManager.FindByNameAsync(UserTypeOptions.User.ToString()) is null)
                    {
                        ApplicationRole role = new ApplicationRole()
                        {
                            Name = UserTypeOptions.User.ToString()
                        };
                        await _roleManager.CreateAsync(role);
                        await _userManager.AddToRoleAsync(user, UserTypeOptions.User.ToString());
                    }
                }
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Explore", "Stocks");
            }
            else
            {
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("Index", error.Description);
                }
                return View(registerDTO);
            }
            // return RedirectToAction("Login","Account");
        }
        public async Task<IActionResult> IsEmailAddressExisted(string email)
        {
            ApplicationUser user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }
    }
}
