namespace _2022_CS_668.Models
{
    public class AttendanceMenuItem
    {
        public int Id { get; set; }
        public int AttendanceId { get; set; }
        public int MenuId { get; set; }
        public decimal PriceAtSelection { get; set; }
        public int Quantity { get; set; } = 1;
        public DateTime SelectedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual Attendance Attendance { get; set; } = null!;
        public virtual Menu Menu { get; set; } = null!;
    }
}
