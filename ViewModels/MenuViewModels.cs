using System.ComponentModel.DataAnnotations;
using _2022_CS_668.Models;

namespace _2022_CS_668.ViewModels
{
    public class MenuViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Mess Group")]
        public int MessGroupId { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Item Name")]
        public string ItemName { get; set; } = string.Empty;

        [Required]
        public MenuCategory Category { get; set; }

        [Required]
        [Range(0.01, 10000)]
        public decimal Price { get; set; }

        [Required]
        [Display(Name = "Effective Date")]
        [DataType(DataType.Date)]
        public DateTime EffectiveDate { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class MenuListViewModel
    {
        public IEnumerable<Menu> Menus { get; set; } = new List<Menu>();
        public IEnumerable<MessGroup> MessGroups { get; set; } = new List<MessGroup>();
    }

    public class MenuDisplayViewModel
    {
        public DateTime SelectedDate { get; set; }
        public List<Menu> MandatoryItems { get; set; } = new List<Menu>();
        public List<Menu> OptionalItems { get; set; } = new List<Menu>();
    }
}
