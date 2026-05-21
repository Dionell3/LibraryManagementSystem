using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class Member
    {
        [Key]
        public int MemberID { get; set; }

        [Required(ErrorMessage = "Membership number is required.")]
        [StringLength(20, ErrorMessage = "Membership number cannot exceed 20 characters.")]
        [Display(Name = "Membership Number")]
        public string? MembershipNumber { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Invalid phone number.")]
        [StringLength(20, ErrorMessage = "Phone cannot exceed 20 characters.")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Member since date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Member Since")]
        public DateTime MemberSince { get; set; } = DateTime.Today;

        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public string? Address { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        public string? UserId { get; set; }

        public ICollection<BorrowTransaction> BorrowTransactions { get; set; } = new List<BorrowTransaction>();
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}
