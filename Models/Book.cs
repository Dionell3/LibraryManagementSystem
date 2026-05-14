using Required = System.ComponentModel.DataAnnotations.RequiredAttribute;
﻿using System.ComponentModel.DataAnnotations;
using Microsoft.Build.Framework;
using static Azure.Core.HttpHeader;

namespace LibraryManagementSystem.Models
{
    public class Book
    {
        public int BookID { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Author is required.")]
        [StringLength(100, ErrorMessage = "Author cannot exceed 100 characters.")]
        public string? Author { get; set; }

        [Required(ErrorMessage = "Genre is required.")]
        [StringLength(50, ErrorMessage = "Genre cannot exceed 50 characters.")]
        public string? Genre { get; set; }

        [Required(ErrorMessage = "ISBN is required.")]
        [StringLength(20, ErrorMessage = "ISBN cannot exceed 20 characters.")]
        [RegularExpression(@"^[0-9\-]+$", ErrorMessage = "ISBN can only contain numbers and dashes.")]
        public string? ISBN { get; set; }

        [Display(Name = "Available")]
        public bool IsAvailable { get; set; } = true;

        [StringLength(500)]
        public string? Summary { get; set; }

        [StringLength(255)]
        [Display(Name = "Cover Image")]
        public string? CoverImagePath { get; set; }
    }
}