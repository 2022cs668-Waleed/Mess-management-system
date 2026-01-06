using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _2022_CS_668.Models;
using _2022_CS_668.Repositories.Interfaces;
using _2022_CS_668.ViewModels;

namespace _2022_CS_668.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(currentUser!);

            if (roles.Contains("SuperAdmin"))
            {
                return RedirectToAction("Index", "SuperAdmin");
            }
            else if (roles.Contains("Admin"))
            {
                return RedirectToAction("Index", "Admin");
            }
            else if (roles.Contains("Teacher"))
            {
                return RedirectToAction("Index", "Teacher");
            }
            else if (roles.Contains("Student"))
            {
                return RedirectToAction("Index", "Student");
            }

            var viewModel = new DashboardViewModel
            {
                TotalUsers = (await _userManager.Users.ToListAsync()).Count,
                ActiveMenuItems = await _unitOfWork.Menus.CountAsync(m => m.IsActive),
                PendingBills = await _unitOfWork.Bills.CountAsync(b => b.Status == BillStatus.Pending),
                UnpaidBills = await _unitOfWork.Bills.CountAsync(b => b.Status != BillStatus.Paid)
            };

            return View(viewModel);
        }
    }
}
