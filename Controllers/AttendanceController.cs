using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using _2022_CS_668.Models;
using _2022_CS_668.Repositories;
using _2022_CS_668.Repositories.Interfaces;
using _2022_CS_668.ViewModels;

namespace _2022_CS_668.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class AttendanceController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public AttendanceController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public IActionResult Mark()
        {
            var viewModel = new AttendanceMarkViewModel
            {
                Date = DateTime.Today
            };
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetStudentsForDate(DateTime date)
        {
            var students = await _userManager.GetUsersInRoleAsync("Student");
            var attendances = await _unitOfWork.Attendances.GetAttendanceByDateAsync(date);
            
            // Get available menu items for the date - only items with EffectiveDate <= selected date
            var allMenus = await _unitOfWork.Menus.GetActiveMenusAsync();
            var availableMenuItems = allMenus
                .Where(m => m.EffectiveDate.Date <= date.Date) // Only show items effective on or before this date
                .Select(m => new MenuItemViewModel
                {
                    Id = m.Id,
                    ItemName = m.ItemName,
                    Price = m.Price,
                    Category = m.Category.ToString(),
                    MessGroupId = m.MessGroupId,
                    MessGroupName = m.MessGroup?.Name ?? "",
                    IsMandatory = m.MessGroup?.IsMandatory ?? false
                }).ToList();

            var viewModel = students.Select(student =>
            {
                var attendance = attendances.FirstOrDefault(a => a.UserId == student.Id);
                return new AttendanceViewModel
                {
                    Id = attendance?.Id ?? 0,
                    UserId = student.Id,
                    UserName = student.UserName ?? "",
                    FullName = student.FullName,
                    Date = date,
                    IsPresent = attendance?.IsPresent ?? false,
                    Remarks = attendance?.Remarks,
                    SelectedMenuIds = attendance?.AttendanceMenuItems?.Select(ami => ami.MenuId).ToList() ?? new List<int>(),
                    AvailableMenuItems = availableMenuItems
                };
            }).ToList();

            return PartialView("_AttendanceListPartial", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> BulkUpdate([FromBody] List<BulkAttendanceUpdateModel> attendances)
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var context = _unitOfWork as UnitOfWork;

                foreach (var item in attendances)
                {
                    var existing = await _unitOfWork.Attendances.GetAttendanceAsync(item.UserId, item.Date);
                    
                    if (existing != null)
                    {
                        existing.IsPresent = item.IsPresent;
                        existing.MarkedBy = currentUser?.Id;
                        existing.MarkedAt = DateTime.UtcNow;
                        _unitOfWork.Attendances.Update(existing);
                        
                        // Remove old menu items
                        var oldMenuItems = context?.Context.AttendanceMenuItems
                            .Where(ami => ami.AttendanceId == existing.Id)
                            .ToList();
                        if (oldMenuItems != null && oldMenuItems.Any())
                        {
                            context?.Context.AttendanceMenuItems.RemoveRange(oldMenuItems);
                        }
                        
                        // Add new menu items
                        if (item.IsPresent && item.SelectedMenuIds.Any())
                        {
                            foreach (var menuId in item.SelectedMenuIds)
                            {
                                var menu = await _unitOfWork.Menus.GetByIdAsync(menuId);
                                if (menu != null)
                                {
                                    var attendanceMenuItem = new AttendanceMenuItem
                                    {
                                        AttendanceId = existing.Id,
                                        MenuId = menuId,
                                        PriceAtSelection = menu.Price,
                                        Quantity = 1
                                    };
                                    context?.Context.AttendanceMenuItems.Add(attendanceMenuItem);
                                }
                            }
                        }
                    }
                    else
                    {
                        var newAttendance = new Attendance
                        {
                            UserId = item.UserId,
                            Date = item.Date,
                            IsPresent = item.IsPresent,
                            MarkedBy = currentUser?.Id
                        };
                        await _unitOfWork.Attendances.AddAsync(newAttendance);
                        await _unitOfWork.SaveAsync();
                        
                        // Add menu items for new attendance
                        if (item.IsPresent && item.SelectedMenuIds.Any())
                        {
                            foreach (var menuId in item.SelectedMenuIds)
                            {
                                var menu = await _unitOfWork.Menus.GetByIdAsync(menuId);
                                if (menu != null)
                                {
                                    var attendanceMenuItem = new AttendanceMenuItem
                                    {
                                        AttendanceId = newAttendance.Id,
                                        MenuId = menuId,
                                        PriceAtSelection = menu.Price,
                                        Quantity = 1
                                    };
                                    context?.Context.AttendanceMenuItems.Add(attendanceMenuItem);
                                }
                            }
                        }
                    }
                }

                await _unitOfWork.SaveAsync();
                return Json(new { success = true, message = "Attendance updated successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Report(DateTime? fromDate, DateTime? toDate)
        {
            fromDate ??= DateTime.Today.AddDays(-30);
            toDate ??= DateTime.Today;

            ViewBag.FromDate = fromDate.Value.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate.Value.ToString("yyyy-MM-dd");

            return View();
        }
    }
}
