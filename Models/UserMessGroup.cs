namespace _2022_CS_668.Models
{
    public class UserMessGroup
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int MessGroupId { get; set; }
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ApplicationUser User { get; set; } = null!;
        public virtual MessGroup MessGroup { get; set; } = null!;
    }
}
