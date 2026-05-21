using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystem.Models
{
    public class Fine
    {
        [Key]
        public int FineID { get; set; }

        [Required]
        public int BorrowTransactionId { get; set; }

        [Required]
        [Range(0, 10000)]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        [Display(Name = "Issued Date")]
        public DateTime IssuedDate { get; set; } = DateTime.Now;

        [Display(Name = "Paid Date")]
        [DataType(DataType.Date)]
        public DateTime? PaidDate { get; set; }

        [Display(Name = "Paid")]
        public bool IsPaid { get; set; } = false;

        [StringLength(500)]
        public string? Notes { get; set; }

        [ForeignKey("BorrowTransactionId")]
        public BorrowTransaction? BorrowTransaction { get; set; }
    }
}
