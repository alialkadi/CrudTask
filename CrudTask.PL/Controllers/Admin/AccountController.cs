using CrudTask.DAL.Data.Entities;
using CrudTask.PL.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CrudTask.PL.Controllers.Admin
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(SignInViewModel login)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(login.Email);
                if (user is not null)
                {
                    var flag = await _userManager.CheckPasswordAsync(user, login.Password);

                    if (flag)
                    {
                        var result = await _signInManager.PasswordSignInAsync(user, login.Password, login.RememberMe, false);

                        if (result.Succeeded && await _userManager.IsInRoleAsync(user, "Admin"))
                        {
                            ViewData["MsgSignIN"] = "Signed In Successfully";

                            return RedirectToAction(nameof(HomeController.Index), "Home");
                        }
                        else
                        {
                            ModelState.AddModelError("", "User Not Admin");
                        }

                    }
                }
                ModelState.AddModelError("", "User Not found");
            }
            else
            {
                ModelState.AddModelError("", "Invalid Login Inputs");
            }
            return View(login);
        }
        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            if (ModelState.IsValid)
            {
                var flag = await _userManager.FindByEmailAsync(model.Email);
                if (flag is null)
                {
                    var user = new AppUser
                    {
                        UserName = model.UserName,
                        Email = model.Email,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                    };
                    var result = await _userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        ViewData["MsgSignUp"] = "Signed Up Successfully";
                        return RedirectToAction(nameof(HomeController.Index), "Home");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }

                }
                else
                {
                    ModelState.AddModelError(string.Empty, "USer Email Is Already exists");
                }
            }
            return View(model);
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }
    }
}
