using System;
using System.ComponentModel.DataAnnotations;

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

        public DateTime BorrowDate { get; set; }

        public DateTime DueDate { get; set; }

        public DateTime? ReturnDate { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}