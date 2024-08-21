using AutoMapper;
using CrudTask.DAL.Data.Entities;
using CrudTask.PL.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrudTask.PL.Controllers.Admin
{
    public class UserController : Controller
    {
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;

        public UserController(UserManager<AppUser> userManager, IMapper mapper)
        {
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string search)
        {
            var usersQuery = _userManager.Users.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                usersQuery = usersQuery.Where(user => user.Email.ToLower().Contains(search.ToLower()));
            }

            var users = await usersQuery.ToListAsync();

            var userViewModels = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userViewModels.Add(new UserViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Roles = roles
                });
            }

            ViewData["SearchQuery"] = search; // Pass search query to view
            return View(userViewModels);
        }


        public async Task<IActionResult> Details(string id, string viewName = "Details")
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            var userFromDb = await _userManager.FindByIdAsync(id);
            if (userFromDb == null)
                return NotFound();

            var userViewModel = _mapper.Map<UserViewModel>(userFromDb);
            return View(viewName, userViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            return await Details(id, "Edit");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UserViewModel model)
        {
            if (id != model.Id)
                return BadRequest();

            if (ModelState.IsValid)
            {
                var userFromDb = await _userManager.FindByIdAsync(id);
                if (userFromDb == null)
                    return NotFound();

                userFromDb.FirstName = model.FirstName;
                userFromDb.LastName = model.LastName;
                userFromDb.Email = model.Email;

                IdentityResult result = null;

                // Update password if provided
                if (!string.IsNullOrEmpty(model.password))
                {
                    if (model.password == model.Confirmpassword)
                    {
                        var token = await _userManager.GeneratePasswordResetTokenAsync(userFromDb);
                        result = await _userManager.ResetPasswordAsync(userFromDb, token, model.password);

                        if (!result.Succeeded)
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                            return View(model); // Return view if password reset fails
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Password and confirmation password do not match.");
                        return View(model); // Return view if passwords do not match
                    }
                }

                result = await _userManager.UpdateAsync(userFromDb);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }



        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            return await Details(id, "Delete");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id, UserViewModel model)
        {
            if (id != model.Id)
                return BadRequest();

            if (ModelState.IsValid)
            {
                var userFromDb = await _userManager.FindByIdAsync(id);
                if (userFromDb == null)
                    return NotFound();

                var result = await _userManager.DeleteAsync(userFromDb);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }
    }
}
