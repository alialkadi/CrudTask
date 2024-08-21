using AutoMapper;
using CrudTask.DAL.Data.Entities;
using CrudTask.PL.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrudTask.PL.Controllers.Admin
{
    public class RoleController : Controller
    {
        private IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private RoleManager<IdentityRole> _roleManger;

        public RoleController(RoleManager<IdentityRole> RoleManger, IMapper mapper, UserManager<AppUser> userManager)
        {
            _mapper = mapper;
            _userManager = userManager;
            _roleManger = RoleManger;
        }

        public async Task<IActionResult> Index(string search)
        {
            var users = Enumerable.Empty<RoleViewModel>();
            if (string.IsNullOrEmpty(search))
            {
                users = await _roleManger.Roles.Select(user => new RoleViewModel()
                {
                    Id = user.Id,
                    Name = user.Name,
                }).ToListAsync();
            }
            else
            {
                users = await _roleManger.Roles.Where(user => user.Name.ToLower().Contains(search.ToLower()))
                    .Select(user => new RoleViewModel()
                    {
                        Id = user.Id,
                        Name = user.Name,
                    }).ToListAsync();
            }
            return View(users);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = new IdentityRole()
                {
                    Name = model.Name,
                };

                var res = await _roleManger.CreateAsync(role);
                if (res.Succeeded)
                {
                    return RedirectToAction("Index");

                }
                else
                {
                    foreach (var err in res.Errors)
                    {
                        ModelState.AddModelError("", err.Description);
                    }
                }

            }
            return View();
        }
        public async Task<IActionResult> Details(string? id, string ViewName = "Details")
        {
            if (id is null)
                return BadRequest();
            var roleFromDB = await _roleManger.FindByIdAsync(id);
            if (roleFromDB == null)
                return NotFound();

            var res = _mapper.Map<RoleViewModel>(roleFromDB);
            return View(ViewName, res);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(string? id)
        {
            return await Details(id, "Edit");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] string id, RoleViewModel model)
        {
            if (id != model.Id)
                return BadRequest();
            if (ModelState.IsValid)
            {

                var roleFromDB = await _roleManger.FindByIdAsync(id);
                if (roleFromDB is null)
                    return NotFound(); // 404

                roleFromDB.Name = model.Name;


                await _roleManger.UpdateAsync(roleFromDB);

                return RedirectToAction(nameof(Index));
            }


            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Delete(string? id)
        {
            return await Details(id, "Delete");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Delete([FromRoute] string id, RoleViewModel model)
        {
            if (id != model.Id)
                return BadRequest();
            if (ModelState.IsValid)
            {
                var roleFromDB = await _roleManger.FindByIdAsync(id);
                if (roleFromDB is null)
                    return NotFound(); // 404


                await _roleManger.DeleteAsync(roleFromDB);

                return RedirectToAction(nameof(Index));
            }


            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> AddOrRemoveUserRole(string roleId)
        {
            var res = await _roleManger.FindByIdAsync(roleId);
            if (res is null) return NotFound();

            ViewData["roleId"] = roleId;
            var usersRole = new List<UersInRoleviewModel>();
            var users = await _userManager.Users.OrderBy(U => U.UserName).ToListAsync();
            foreach (var user in users)
            {
                var userrole = new UersInRoleviewModel()
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                };
                if (await _userManager.IsInRoleAsync(user, res.Name))
                {
                    userrole.isSelected = true;
                }
                else
                {
                    userrole.isSelected = false;
                }
                usersRole.Add(userrole);
            }

            return View(usersRole);
        }
        [HttpPost]
        public async Task<IActionResult> AddOrRemoveUserRole(string roleId, List<UersInRoleviewModel> users)
        {
            var role = await _roleManger.FindByIdAsync(roleId);
            if (role is null) return NotFound();

            if (ModelState.IsValid)
            {
                foreach (var user in users)
                {
                    var appuser = await _userManager.FindByIdAsync(user.UserId);
                    if (appuser is not null)
                    {
                        if (user.isSelected && !await _userManager.IsInRoleAsync(appuser, role.Name))
                        {
                            await _userManager.AddToRoleAsync(appuser, role.Name);
                        }
                        else if (!user.isSelected && await _userManager.IsInRoleAsync(appuser, role.Name))
                        {
                            await _userManager.RemoveFromRoleAsync(appuser, role.Name);
                        }
                    }
                }
                return RedirectToAction(nameof(Edit), new { Id = roleId });
            }
            else
            {
                ModelState.AddModelError("", "Error from user model");
            }

            return View(users);
        }

    }
}
