using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class Library
    {
        public int LibraryID { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Library name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        [Display(Name = "Library Name")]
        public string? Name { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Location is required.")]
        [StringLength(200, ErrorMessage = "Location cannot exceed 200 characters.")]
        public string? Location { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Operating hours are required.")]
        [StringLength(100, ErrorMessage = "Operating hours cannot exceed 100 characters.")]
        [Display(Name = "Operating Hours")]
        public string? OperatingHours { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Contact details are required.")]
        [StringLength(150, ErrorMessage = "Contact details cannot exceed 150 characters.")]
        [Display(Name = "Contact Details")]
        public string? ContactDetails { get; set; }
    }
}
