using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _2022_CS_668.Models;
using _2022_CS_668.Repositories.Interfaces;
using _2022_CS_668.ViewModels;

namespace _2022_CS_668.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var totalBills = await _unitOfWork.Bills.GetAllAsync();

            var viewModel = new DashboardViewModel
            {
                TotalUsers = (await _userManager.Users.ToListAsync()).Count,
                TotalStudents = (await _userManager.GetUsersInRoleAsync("Student")).Count,
                TotalTeachers = 0, // No teachers anymore
                ActiveMenuItems = await _unitOfWork.Menus.CountAsync(m => m.IsActive),
                PendingBills = await _unitOfWork.Bills.CountAsync(b => b.Status == BillStatus.Pending || b.Status == BillStatus.Generated),
                UnpaidBills = await _unitOfWork.Bills.CountAsync(b => b.Status != BillStatus.Paid),
                TotalRevenue = totalBills.Where(b => b.Status == BillStatus.Paid).Sum(b => b.TotalAmount),
                MonthlyRevenue = totalBills.Where(b => b.Status == BillStatus.Paid && b.Month == DateTime.Today.Month && b.Year == DateTime.Today.Year).Sum(b => b.TotalAmount)
            };

            return View(viewModel);
        }

        public async Task<IActionResult> UserManagement()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    PhoneNumber = model.PhoneNumber,
                    IsActive = true,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Assign role
                    if (!string.IsNullOrEmpty(model.Role))
                    {
                        // Ensure role exists
                        if (!await _roleManager.RoleExistsAsync(model.Role))
                        {
                            await _roleManager.CreateAsync(new IdentityRole(model.Role));
                        }
                        await _userManager.AddToRoleAsync(user, model.Role);
                    }

                    TempData["SuccessMessage"] = "User created successfully!";
                    return RedirectToAction(nameof(UserManagement));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction(nameof(UserManagement));
            }

            // Prevent deleting yourself
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.Id == userId)
            {
                TempData["ErrorMessage"] = "You cannot delete your own account.";
                return RedirectToAction(nameof(UserManagement));
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"User '{user.FullName}' deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete user.";
            }

            return RedirectToAction(nameof(UserManagement));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleUserStatus(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction(nameof(UserManagement));
            }

            user.IsActive = !user.IsActive;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"User {(user.IsActive ? "activated" : "deactivated")} successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update user status.";
            }

            return RedirectToAction(nameof(UserManagement));
        }

        public async Task<IActionResult> BillApproval(int? month, int? year)
        {
            month ??= DateTime.Today.Month;
            year ??= DateTime.Today.Year;

            var bills = (await _unitOfWork.Bills.GetAllAsync())
                .Where(b => b.Status == BillStatus.Generated 
                    && b.Month == month 
                    && b.Year == year)
                .OrderBy(b => b.User?.FullName ?? "")
                .ToList();

            ViewBag.Month = month;
            ViewBag.Year = year;
            ViewBag.MonthName = new DateTime(2000, month.Value, 1).ToString("MMMM");

            return View(bills);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveBill(int id)
        {
            var bill = await _unitOfWork.Bills.GetByIdAsync(id);
            if (bill == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            bill.Status = BillStatus.Approved;
            bill.ApprovedBy = currentUser?.Id;
            bill.ApprovedAt = DateTime.UtcNow;

            _unitOfWork.Bills.Update(bill);
            await _unitOfWork.SaveAsync();

            TempData["SuccessMessage"] = "Bill approved successfully!";
            return RedirectToAction(nameof(BillApproval));
        }

        public async Task<IActionResult> PaymentManagement()
        {
            var bills = await _unitOfWork.Bills.GetBillsByStatusAsync(BillStatus.Approved);
            return View(bills);
        }

        [HttpGet]
        public async Task<IActionResult> MarkPayment(int id)
        {
            var bill = await _unitOfWork.Bills.GetBillWithDetailsAsync(id);
            if (bill == null)
            {
                return NotFound();
            }

            var viewModel = new PaymentViewModel
            {
                BillId = id,
                PaymentDate = DateTime.Today
            };

            ViewBag.Bill = bill;
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkPayment(PaymentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var bill = await _unitOfWork.Bills.GetByIdAsync(model.BillId);

                if (bill == null)
                {
                    return NotFound();
                }

                var payment = new Payment
                {
                    BillId = model.BillId,
                    Amount = model.Amount,
                    PaymentDate = model.PaymentDate,
                    PaymentMethod = model.PaymentMethod,
                    TransactionReference = model.TransactionReference,
                    Remarks = model.Remarks,
                    RecordedBy = currentUser?.Id
                };

                await _unitOfWork.Payments.AddAsync(payment);

                // Check if bill is fully paid
                var totalPaid = await _unitOfWork.Payments.GetTotalPaidAmountAsync(model.BillId);
                if (totalPaid + model.Amount >= bill.TotalAmount)
                {
                    bill.Status = BillStatus.Paid;
                    _unitOfWork.Bills.Update(bill);
                }

                await _unitOfWork.SaveAsync();

                TempData["SuccessMessage"] = "Payment recorded successfully!";
                return RedirectToAction(nameof(PaymentManagement));
            }

            ViewBag.Bill = await _unitOfWork.Bills.GetBillWithDetailsAsync(model.BillId);
            return View(model);
        }
    }
}
