using System.ComponentModel.DataAnnotations;
using _2022_CS_668.Models;

namespace _2022_CS_668.ViewModels
{
    public class PaymentViewModel
    {
        public int BillId { get; set; }

        [Required]
        [Range(0.01, 100000)]
        public decimal Amount { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Payment Date")]
        public DateTime PaymentDate { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Transaction Reference")]
        public string? TransactionReference { get; set; }

        [StringLength(500)]
        public string? Remarks { get; set; }
    }

    public class UserMessGroupViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public List<int> SelectedGroupIds { get; set; } = new List<int>();
        public List<MessGroup> AvailableGroups { get; set; } = new List<MessGroup>();
    }
}
