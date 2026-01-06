namespace _2022_CS_668.Models
{
    public class Attendance
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public bool IsPresent { get; set; }
        public DateTime MarkedAt { get; set; } = DateTime.UtcNow;
        public string? MarkedBy { get; set; }
        public string? Remarks { get; set; }

        // Navigation properties
        public virtual ApplicationUser User { get; set; } = null!;
        public virtual ICollection<AttendanceMenuItem> AttendanceMenuItems { get; set; } = new List<AttendanceMenuItem>();
    }
}
