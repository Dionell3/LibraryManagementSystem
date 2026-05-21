using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystem.Models
{
    public class Reservation
    {
        [Key]
        public int ReservationID { get; set; }

        [Required]
        public int BookID { get; set; }

        [Required]
        public int MemberID { get; set; }

        [Display(Name = "Reservation Date")]
        public DateTime ReservationDate { get; set; } = DateTime.Now;

        [StringLength(20)]
        public string Status { get; set; } = "Reserved";

        [Display(Name = "Expiry Date")]
        [DataType(DataType.Date)]
        public DateTime? ExpiryDate { get; set; }

        [ForeignKey("BookID")]
        public Book? Book { get; set; }

        [ForeignKey("MemberID")]
        public Member? Member { get; set; }
    }
}
