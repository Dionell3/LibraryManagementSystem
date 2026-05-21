using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystem.Models
{
    public class Feedback
    {
        [Key]
        public int FeedbackID { get; set; }

        [Required]
        public int BookID { get; set; }

        [Required]
        public int MemberID { get; set; }

        [Required(ErrorMessage = "Rating is required.")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        [StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters.")]
        public string? Comment { get; set; }

        [Display(Name = "Submitted Date")]
        public DateTime SubmittedDate { get; set; } = DateTime.Now;

        [Display(Name = "Approved")]
        public bool IsApproved { get; set; } = false;

        [ForeignKey("BookID")]
        public Book? Book { get; set; }

        [ForeignKey("MemberID")]
        public Member? Member { get; set; }
    }
}
