﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using XFLCSMS.Models.Issue;

namespace XFLCSMS.Models.Support
{
    public class SupportType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SupportTypeId { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "Password must be 4 to 100 characters")]
        [RegularExpression(@"^[a-zA-Z.]+$", ErrorMessage = "Only letters allowed.")]
        public string SType { get; set; }=string.Empty;
        public ICollection<IssueTable> issue { get; set; }

    }
}
