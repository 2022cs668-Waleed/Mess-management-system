namespace _2022_CS_668.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int BillId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string? TransactionReference { get; set; }
        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
        public string? RecordedBy { get; set; }
        public string? Remarks { get; set; }

        // Navigation properties
        public virtual Bill Bill { get; set; } = null!;
    }
}
