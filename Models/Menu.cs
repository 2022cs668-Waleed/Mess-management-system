namespace _2022_CS_668.Models
{
    public enum MenuCategory
    {
        Food = 1,
        WaterTea = 2
    }

    public class Menu
    {
        public int Id { get; set; }
        public int MessGroupId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public MenuCategory Category { get; set; }
        public decimal Price { get; set; }
        public DateTime EffectiveDate { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }

        // Navigation properties
        public virtual MessGroup MessGroup { get; set; } = null!;
        public virtual ICollection<BillDetail> BillDetails { get; set; } = new List<BillDetail>();
    }
}
