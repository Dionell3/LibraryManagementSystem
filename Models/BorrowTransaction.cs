using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystem.Models
{
    public class BorrowTransaction
    {
        [Key]
        public int BorrowTransactionId { get; set; }

        [Required]
        public int BookId { get; set; }

        [Required]
        public int MemberId { get; set; }

        [Display(Name = "Borrow Date")]
        public DateTime BorrowDate { get; set; }

        [Display(Name = "Due Date")]
        public DateTime DueDate { get; set; }

        [Display(Name = "Return Date")]
        public DateTime? ReturnDate { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = string.Empty;

        [Display(Name = "Renewals")]
        public int RenewalCount { get; set; } = 0;

        [ForeignKey("BookId")]
        public Book? Book { get; set; }

        [ForeignKey("MemberId")]
        public Member? Member { get; set; }

        public Fine? Fine { get; set; }
    }
}
