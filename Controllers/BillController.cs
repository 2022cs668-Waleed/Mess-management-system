using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using _2022_CS_668.Models;
using _2022_CS_668.Repositories.Interfaces;
using _2022_CS_668.ViewModels;

namespace _2022_CS_668.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BillController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public BillController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public IActionResult Generate()
        {
            var viewModel = new MonthlyBillGenerationViewModel
            {
                Month = DateTime.Today.Month,
                Year = DateTime.Today.Year
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateMonthlyBills(int month, int year)
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var students = await _userManager.GetUsersInRoleAsync("Student");
                int billsCreated = 0;
                int billsUpdated = 0;
                decimal totalAmount = 0;

                foreach (var student in students)
                {
                    // Get attendance for the month with selected menu items
                    var attendances = await _unitOfWork.Attendances.GetUserAttendanceAsync(student.Id, month, year);
                    
                    decimal foodAmount = 0;
                    decimal waterTeaAmount = 0;
                    var billDetails = new List<BillDetail>();

                    // Calculate charges based on ACTUAL selected menu items during attendance
                    foreach (var attendance in attendances.Where(a => a.IsPresent))
                    {
                        foreach (var attendanceMenuItem in attendance.AttendanceMenuItems)
                        {
                            var menu = attendanceMenuItem.Menu;
                            if (menu == null)
                            {
                                // Try to load it manually
                                var fullMenu = await _unitOfWork.Menus.GetByIdAsync(attendanceMenuItem.MenuId);
                                if (fullMenu == null) continue;
                                menu = fullMenu;
                            }

                            if (menu.MessGroup == null)
                            {
                                continue;
                            }

                            var itemAmount = attendanceMenuItem.PriceAtSelection * attendanceMenuItem.Quantity;

                            // Categorize by menu group type
                            if (menu.MessGroup.IsMandatory)
                            {
                                waterTeaAmount += itemAmount;
                            }
                            else
                            {
                                foodAmount += itemAmount;
                            }

                            // Create bill detail for each selected item
                            billDetails.Add(new BillDetail
                            {
                                MenuId = attendanceMenuItem.MenuId,
                                Date = attendance.Date,
                                UnitPrice = attendanceMenuItem.PriceAtSelection,
                                Quantity = attendanceMenuItem.Quantity,
                                Amount = itemAmount
                            });
                        }
                    }

                    // Only process if there are actual charges
                    if (foodAmount > 0 || waterTeaAmount > 0)
                    {
                        // Check if bill already exists for this student and month
                        var existingBill = await _unitOfWork.Bills.GetBillAsync(student.Id, month, year);
                        
                        if (existingBill != null)
                        {
                            // UPDATE existing bill with recalculated amounts
                            existingBill.FoodAmount = foodAmount;
                            existingBill.WaterTeaAmount = waterTeaAmount;
                            existingBill.TotalAmount = foodAmount + waterTeaAmount;
                            existingBill.GeneratedBy = currentUser?.Id;
                            existingBill.GeneratedAt = DateTime.UtcNow;
                            
                            // Remove old bill details
                            var oldDetails = existingBill.BillDetails?.ToList() ?? new List<BillDetail>();
                            foreach (var detail in oldDetails)
                            {
                                _unitOfWork.BillDetails.Remove(detail);
                            }
                            
                            // Add new bill details
                            foreach (var detail in billDetails)
                            {
                                detail.BillId = existingBill.Id;
                                await _unitOfWork.BillDetails.AddAsync(detail);
                            }
                            
                            _unitOfWork.Bills.Update(existingBill);
                            billsUpdated++;
                        }
                        else
                        {
                            // CREATE new bill
                            var bill = new Bill
                            {
                                UserId = student.Id,
                                Month = month,
                                Year = year,
                                FoodAmount = foodAmount,
                                WaterTeaAmount = waterTeaAmount,
                                TotalAmount = foodAmount + waterTeaAmount,
                                Status = BillStatus.Generated,
                                GeneratedBy = currentUser?.Id,
                                BillDetails = billDetails
                            };

                            await _unitOfWork.Bills.AddAsync(bill);
                            billsCreated++;
                        }
                        
                        totalAmount += foodAmount + waterTeaAmount;
                    }
                }

                await _unitOfWork.SaveAsync();

                if (billsCreated > 0 || billsUpdated > 0)
                {
                    var message = $"Successfully processed bills for {GetMonthName(month)} {year}. ";
                    if (billsCreated > 0) message += $"Created: {billsCreated}. ";
                    if (billsUpdated > 0) message += $"Updated: {billsUpdated}. ";
                    message += $"Total Amount: Rs {totalAmount:N2}";
                    
                    TempData["SuccessMessage"] = message;
                }
                else
                {
                    TempData["InfoMessage"] = $"No bills to process for {GetMonthName(month)} {year}. No attendance data with menu selections found.";
                }
                
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error generating bills: {ex.Message}";
                return RedirectToAction(nameof(Generate));
            }
        }

        public async Task<IActionResult> Index(int? month, int? year)
        {
            month ??= DateTime.Today.Month;
            year ??= DateTime.Today.Year;

            var bills = (await _unitOfWork.Bills.GetAllAsync())
                .Where(b => b.Month == month && b.Year == year)
                .OrderBy(b => b.User != null ? b.User.FullName : "")
                .ToList();

            ViewBag.Month = month;
            ViewBag.Year = year;
            ViewBag.MonthName = GetMonthName(month.Value);

            return View(bills);
        }

        public async Task<IActionResult> Details(int id)
        {
            var bill = await _unitOfWork.Bills.GetBillWithDetailsAsync(id);
            if (bill == null)
            {
                return NotFound();
            }

            var totalPaid = await _unitOfWork.Payments.GetTotalPaidAmountAsync(id);

            var viewModel = new BillViewModel
            {
                Id = bill.Id,
                UserId = bill.UserId,
                UserName = bill.User?.UserName ?? "",
                FullName = bill.User?.FullName ?? "Unknown",
                Month = bill.Month,
                Year = bill.Year,
                TotalAmount = bill.TotalAmount,
                FoodAmount = bill.FoodAmount,
                WaterTeaAmount = bill.WaterTeaAmount,
                Status = bill.Status,
                GeneratedAt = bill.GeneratedAt,
                Remarks = bill.Remarks,
                BillDetails = bill.BillDetails?.ToList() ?? new List<BillDetail>(),
                Payments = bill.Payments?.ToList() ?? new List<Payment>(),
                TotalPaid = totalPaid,
                Balance = bill.TotalAmount - totalPaid
            };

            return View(viewModel);
        }

        private string GetMonthName(int month)
        {
            return new DateTime(2000, month, 1).ToString("MMMM");
        }
    }
}
