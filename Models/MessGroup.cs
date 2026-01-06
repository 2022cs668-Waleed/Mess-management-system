namespace _2022_CS_668.Models
{
    public class MessGroup
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsMandatory { get; set; } // Water & Tea is mandatory
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<UserMessGroup> UserMessGroups { get; set; } = new List<UserMessGroup>();
        public virtual ICollection<Menu> Menus { get; set; } = new List<Menu>();
    }
}
