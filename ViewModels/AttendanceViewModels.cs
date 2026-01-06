using System.ComponentModel.DataAnnotations;
using _2022_CS_668.Models;

namespace _2022_CS_668.ViewModels
{
    public class AttendanceViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public bool IsPresent { get; set; }
        public string? Remarks { get; set; }
        public List<int> SelectedMenuIds { get; set; } = new List<int>();
        public List<MenuItemViewModel> AvailableMenuItems { get; set; } = new List<MenuItemViewModel>();
    }

    public class AttendanceMarkViewModel
    {
        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public List<AttendanceViewModel> Attendances { get; set; } = new List<AttendanceViewModel>();
    }

    public class BulkAttendanceUpdateModel
    {
        public string UserId { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public bool IsPresent { get; set; }
        public List<int> SelectedMenuIds { get; set; } = new List<int>();
    }

    public class MenuItemViewModel
    {
        public int Id { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;
        public int MessGroupId { get; set; }
        public string MessGroupName { get; set; } = string.Empty;
        public bool IsMandatory { get; set; }
    }
}
