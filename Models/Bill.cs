namespace _2022_CS_668.Models
{
    public enum BillStatus
    {
        Pending = 1,
        Generated = 2,
        Approved = 3,
        Paid = 4,
        Disputed = 5
    }

    public class Bill
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal FoodAmount { get; set; }
        public decimal WaterTeaAmount { get; set; }
        public BillStatus Status { get; set; } = BillStatus.Pending;
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public string? GeneratedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? ApprovedBy { get; set; }
        public string? Remarks { get; set; }

        // Navigation properties
        public virtual ApplicationUser User { get; set; } = null!;
        public virtual ICollection<BillDetail> BillDetails { get; set; } = new List<BillDetail>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
