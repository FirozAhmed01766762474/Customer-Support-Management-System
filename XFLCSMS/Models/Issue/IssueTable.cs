﻿using XFLCSMS.Models.Affected;
using XFLCSMS.Models.Branch;
using XFLCSMS.Models.Brocarage;
using XFLCSMS.Models.Support;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XFLCSMS.Models.Register;

namespace XFLCSMS.Models.Issue
{
    public class IssueTable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IssueId { get; set; }

        [Required]
        [ForeignKey("Users")]
        public int UserId { get; set; }
        public User Users { get; set; }
        [Required]
        [ForeignKey("Brokerages")]
        public int BrokerageId { get; set; }
        public Brokerage Brokerages { get; set; }

        public DateTime TDate { get; set; }
        public string TNumber { get; set; } = string.Empty;

        public DateTime? AssignOn { get; set; }
        public string? AssignBy { get; set; } = string.Empty;
        public DateTime? ApproveOn { get; set; }
        public string? ApproveBy { get; set; } = string.Empty;

        public DateTime? UpdatedOn { get; set; }

        public string? UpdatedBy { get; set; }= string.Empty;

        [ForeignKey("supportTypes")]
        public int? SupportTypeId { get; set; }
        public SupportType? supportTypes { get; set; }
       
        [ForeignKey("supportCatagorys")]
        public int? SupportCatagoryId { get; set; }
        public SupportCatagory? supportCatagorys { get; set; }
        
        [ForeignKey("SupportSubCatagorys")]
       public int? SupportSubCatagoryId { get; set; }
        public SupportSubCatagory? SupportSubCatagorys { get; set; }
        //[ForeignKey("AffectedSectionId")]
        
        [ForeignKey("affecteds")]
        public int? AffectedSectionId { get; set; }
        public AffectedSection? affecteds { get; set; }
        

        public string Priority { get; set; }=string.Empty;
        public string? ITitle { get; set; } = string.Empty;
        public string? Details { get; set; } 

        public string? Comments { get; set; } 

        public ICollection<Attachment>? attachment { get; set; }

        public string? IStatus { get; set; } = string.Empty;

        public DateTime? ClosedOn { get; set; }
        public string? ClosedBy { get; set; }

      

    }
}
