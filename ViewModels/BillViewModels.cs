using _2022_CS_668.Models;

namespace _2022_CS_668.ViewModels
{
    public class BillViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal FoodAmount { get; set; }
        public decimal WaterTeaAmount { get; set; }
        public BillStatus Status { get; set; }
        public DateTime GeneratedAt { get; set; }
        public string? Remarks { get; set; }

        public List<BillDetail> BillDetails { get; set; } = new List<BillDetail>();
        public List<Payment> Payments { get; set; } = new List<Payment>();
        public decimal TotalPaid { get; set; }
        public decimal Balance { get; set; }
    }

    public class MonthlyBillGenerationViewModel
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public int TotalUsers { get; set; }
        public int BillsGenerated { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class DailyBillViewModel
    {
        public DateTime Date { get; set; }
        public decimal FoodCharge { get; set; }
        public decimal WaterTeaCharge { get; set; }
        public decimal TotalCharge { get; set; }
        public bool IsPresent { get; set; }
        public bool FoodTaken { get; set; }
    }

    public class UserBillSummaryViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public int Month { get; set; }
        public int Year { get; set; }
        public int PresentDays { get; set; }
        public int FoodDays { get; set; }
        public decimal FoodAmount { get; set; }
        public decimal WaterTeaAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public BillStatus Status { get; set; }
        public List<DailyBillViewModel> DailyBills { get; set; } = new List<DailyBillViewModel>();
    }
}
