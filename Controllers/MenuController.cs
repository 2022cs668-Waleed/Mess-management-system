using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using _2022_CS_668.Models;
using _2022_CS_668.Repositories.Interfaces;
using _2022_CS_668.ViewModels;

namespace _2022_CS_668.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class MenuController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public MenuController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var menus = await _unitOfWork.Menus.GetActiveMenusAsync();
            var messGroups = await _unitOfWork.MessGroups.GetActiveGroupsAsync();

            var viewModel = new MenuListViewModel
            {
                Menus = menus,
                MessGroups = messGroups
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.MessGroups = await _unitOfWork.MessGroups.GetActiveGroupsAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MenuViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);

                // Convert EffectiveDate to UTC for PostgreSQL compatibility
                var effectiveDateUtc = DateTime.SpecifyKind(model.EffectiveDate.Date, DateTimeKind.Utc);

                var menu = new Menu
                {
                    MessGroupId = model.MessGroupId,
                    ItemName = model.ItemName,
                    Category = model.Category,
                    Price = model.Price,
                    EffectiveDate = effectiveDateUtc,
                    IsActive = model.IsActive,
                    CreatedBy = currentUser?.Id
                };

                await _unitOfWork.Menus.AddAsync(menu);
                await _unitOfWork.SaveAsync();

                TempData["SuccessMessage"] = "Menu item created successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.MessGroups = await _unitOfWork.MessGroups.GetActiveGroupsAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var menu = await _unitOfWork.Menus.GetByIdAsync(id);
            if (menu == null)
            {
                return NotFound();
            }

            var viewModel = new MenuViewModel
            {
                Id = menu.Id,
                MessGroupId = menu.MessGroupId,
                ItemName = menu.ItemName,
                Category = menu.Category,
                Price = menu.Price,
                EffectiveDate = menu.EffectiveDate,
                IsActive = menu.IsActive
            };

            ViewBag.MessGroups = await _unitOfWork.MessGroups.GetActiveGroupsAsync();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MenuViewModel model)
        {
            if (ModelState.IsValid)
            {
                var menu = await _unitOfWork.Menus.GetByIdAsync(model.Id);
                if (menu == null)
                {
                    return NotFound();
                }

                // Convert EffectiveDate to UTC for PostgreSQL compatibility
                var effectiveDateUtc = DateTime.SpecifyKind(model.EffectiveDate.Date, DateTimeKind.Utc);

                menu.MessGroupId = model.MessGroupId;
                menu.ItemName = model.ItemName;
                menu.Category = model.Category;
                menu.Price = model.Price;
                menu.EffectiveDate = effectiveDateUtc;
                menu.IsActive = model.IsActive;

                _unitOfWork.Menus.Update(menu);
                await _unitOfWork.SaveAsync();

                TempData["SuccessMessage"] = "Menu item updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.MessGroups = await _unitOfWork.MessGroups.GetActiveGroupsAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var menu = await _unitOfWork.Menus.GetByIdAsync(id);
            if (menu == null)
            {
                return NotFound();
            }

            menu.IsActive = false;
            _unitOfWork.Menus.Update(menu);
            await _unitOfWork.SaveAsync();

            TempData["SuccessMessage"] = "Menu item deactivated successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
