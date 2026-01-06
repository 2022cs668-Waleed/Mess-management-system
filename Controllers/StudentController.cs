using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using _2022_CS_668.Models;
using _2022_CS_668.Repositories.Interfaces;
using _2022_CS_668.ViewModels;

namespace _2022_CS_668.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            
            var viewModel = new DashboardViewModel
            {
                TotalUsers = await _unitOfWork.Bills.CountAsync(b => b.UserId == currentUser!.Id),
                PendingBills = await _unitOfWork.Bills.CountAsync(b => b.UserId == currentUser!.Id && b.Status != BillStatus.Paid),
                UnpaidBills = await _unitOfWork.Bills.CountAsync(b => b.UserId == currentUser!.Id && b.Status != BillStatus.Paid)
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> SelectMessGroup()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var userGroups = await _unitOfWork.UserMessGroups.GetUserGroupsAsync(currentUser!.Id);
            var availableGroups = await _unitOfWork.MessGroups.GetActiveGroupsAsync();

            var viewModel = new UserMessGroupViewModel
            {
                UserId = currentUser.Id,
                SelectedGroupIds = userGroups.Select(g => g.MessGroupId).ToList(),
                AvailableGroups = availableGroups.ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SelectMessGroup(UserMessGroupViewModel model)
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                
                // Get existing groups
                var existingGroups = await _unitOfWork.UserMessGroups.GetUserGroupsAsync(currentUser!.Id);

                // Deactivate all existing groups
                foreach (var existing in existingGroups)
                {
                    existing.IsActive = false;
                    _unitOfWork.UserMessGroups.Update(existing);
                }

                // Add selected groups
                foreach (var groupId in model.SelectedGroupIds)
                {
                    var existingGroup = existingGroups.FirstOrDefault(g => g.MessGroupId == groupId);
                    if (existingGroup != null)
                    {
                        existingGroup.IsActive = true;
                        _unitOfWork.UserMessGroups.Update(existingGroup);
                    }
                    else
                    {
                        var newGroup = new UserMessGroup
                        {
                            UserId = currentUser.Id,
                            MessGroupId = groupId,
                            IsActive = true
                        };
                        await _unitOfWork.UserMessGroups.AddAsync(newGroup);
                    }
                }

                await _unitOfWork.SaveAsync();
                TempData["SuccessMessage"] = "Mess groups updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating mess groups: {ex.Message}";
                return RedirectToAction(nameof(SelectMessGroup));
            }
        }

        public async Task<IActionResult> DailyBill(DateTime? date)
        {
            date ??= DateTime.Today;

            var currentUser = await _userManager.GetUserAsync(User);
            
            // Get attendance for the date with selected menu items
            var attendance = await _unitOfWork.Attendances.GetAttendanceAsync(currentUser!.Id, date.Value);
            
            decimal foodCharge = 0;
            decimal waterTeaCharge = 0;

            // Calculate charges based on ACTUAL selected menu items
            if (attendance != null && attendance.IsPresent && attendance.AttendanceMenuItems.Any())
            {
                foreach (var attendanceMenuItem in attendance.AttendanceMenuItems)
                {
                    var itemAmount = attendanceMenuItem.PriceAtSelection * attendanceMenuItem.Quantity;
                    
                    if (attendanceMenuItem.Menu.MessGroup != null && attendanceMenuItem.Menu.MessGroup.IsMandatory)
                    {
                        waterTeaCharge += itemAmount;
                    }
                    else
                    {
                        foodCharge += itemAmount;
                    }
                }
            }

            var viewModel = new DailyBillViewModel
            {
                Date = date.Value,
                FoodCharge = foodCharge,
                WaterTeaCharge = waterTeaCharge,
                TotalCharge = foodCharge + waterTeaCharge,
                IsPresent = attendance?.IsPresent ?? false,
                FoodTaken = attendance?.AttendanceMenuItems.Any(ami => !ami.Menu.MessGroup.IsMandatory) ?? false
            };

            return View(viewModel);
        }

        public async Task<IActionResult> MonthlyBill(int? month, int? year)
        {
            month ??= DateTime.Today.Month;
            year ??= DateTime.Today.Year;

            var currentUser = await _userManager.GetUserAsync(User);
            var bill = await _unitOfWork.Bills.GetBillAsync(currentUser!.Id, month.Value, year.Value);

            if (bill == null)
            {
                ViewBag.Message = "No bill generated for the selected period.";
                return View(null);
            }

            var totalPaid = await _unitOfWork.Payments.GetTotalPaidAmountAsync(bill.Id);

            var viewModel = new BillViewModel
            {
                Id = bill.Id,
                UserId = bill.UserId,
                FullName = bill.User.FullName,
                Month = bill.Month,
                Year = bill.Year,
                TotalAmount = bill.TotalAmount,
                FoodAmount = bill.FoodAmount,
                WaterTeaAmount = bill.WaterTeaAmount,
                Status = bill.Status,
                GeneratedAt = bill.GeneratedAt,
                TotalPaid = totalPaid,
                Balance = bill.TotalAmount - totalPaid
            };

            return View(viewModel);
        }

        public async Task<IActionResult> PaymentStatus()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var bills = await _unitOfWork.Bills.GetUserBillsAsync(currentUser!.Id);

            return View(bills);
        }

        [HttpGet]
        public async Task<IActionResult> ViewMenu(DateTime? date)
        {
            date ??= DateTime.Today;
            
            // Get all active menu items that are effective on or before the selected date
            var allMenus = await _unitOfWork.Menus.GetActiveMenusAsync();
            var effectiveMenus = allMenus.Where(m => m.EffectiveDate.Date <= date.Value.Date).ToList();
            
            // Group by category
            var viewModel = new MenuDisplayViewModel
            {
                SelectedDate = date.Value,
                MandatoryItems = effectiveMenus.Where(m => m.MessGroup != null && m.MessGroup.IsMandatory).ToList(),
                OptionalItems = effectiveMenus.Where(m => m.MessGroup != null && !m.MessGroup.IsMandatory).ToList()
            };

            return View(viewModel);
        }
    }
}
