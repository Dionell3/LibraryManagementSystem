using Required = System.ComponentModel.DataAnnotations.RequiredAttribute;
﻿using System.ComponentModel.DataAnnotations;
using Microsoft.Build.Framework;
using static Azure.Core.HttpHeader;

namespace LibraryManagementSystem.Models
{
    public class Librarian
    {
        public int LibrarianID { get; set; }

        [Required(ErrorMessage = "Employee ID is required.")]
        [StringLength(20, ErrorMessage = "Employee ID cannot exceed 20 characters.")]
        [Display(Name = "Employee ID")]
        public string? EmployeeID { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(100)]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        [StringLength(20)]
        [Display(Name = "Phone Number")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Position is required.")]
        [StringLength(50)]
        public string? Position { get; set; }

        [Required(ErrorMessage = "Hire date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Hire Date")]
        public DateTime HireDate { get; set; } = DateTime.Today;

        [Display(Name = "Full Name")]
        public string FullName => $"{FirstName} {LastName}";
    }
}