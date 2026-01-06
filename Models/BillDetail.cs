namespace _2022_CS_668.Models
{
    public class BillDetail
    {
        public int Id { get; set; }
        public int BillId { get; set; }
        public int MenuId { get; set; }
        public DateTime Date { get; set; }
        public int Quantity { get; set; } = 1;
        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }

        // Navigation properties
        public virtual Bill Bill { get; set; } = null!;
        public virtual Menu Menu { get; set; } = null!;
    }
}
