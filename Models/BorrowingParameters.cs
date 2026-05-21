using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class BorrowingParameters
    {
        [Key]
        public int BorrowingParametersID { get; set; }

        [StringLength(100)]
        [Display(Name = "Label")]
        public string? Label { get; set; } = "Default";

        [Required(ErrorMessage = "Loan duration is required.")]
        [Range(1, 365, ErrorMessage = "Loan duration must be between 1 and 365 days.")]
        [Display(Name = "Loan Duration (Days)")]
        public int LoanDurationDays { get; set; } = 14;

        [Required(ErrorMessage = "Renewal limit is required.")]
        [Range(0, 10, ErrorMessage = "Renewal limit must be between 0 and 10.")]
        [Display(Name = "Renewal Limit")]
        public int RenewalLimit { get; set; } = 2;

        [Required(ErrorMessage = "Overdue penalty per day is required.")]
        [Range(0, 100, ErrorMessage = "Penalty must be between $0 and $100.")]
        [DataType(DataType.Currency)]
        [Display(Name = "Overdue Penalty (Per Day, $)")]
        public decimal OverduePenaltyPerDay { get; set; } = 0.50m;

        [Required(ErrorMessage = "Maximum borrowable items is required.")]
        [Range(1, 20, ErrorMessage = "Max borrowable items must be between 1 and 20.")]
        [Display(Name = "Max Borrowable Items")]
        public int MaxBorrowableItems { get; set; } = 5;
    }
}
