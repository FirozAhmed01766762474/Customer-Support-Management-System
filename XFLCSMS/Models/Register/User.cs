using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using XFLCSMS.Models.Issue;

namespace XFLCSMS.Models.Register
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public String FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhonNumber { get; set; } = string.Empty;
        public string? Designation { get; set; } = string.Empty;
        public int BrokerageHouseName { get; set; }
        public int BrokerageHouseAcronym { get; set; }
        public int Branch { get; set; }
        public string? Department { get; set; }= string.Empty;
        public string EmployeeId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; } = new byte[32];
        public byte[] PasswordSalt { get; set; } = new byte[32];
        public string? VerificationToken { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public string? PasswordResetToken { get; set; }
        public DateTime? ResetTokenExpires { get; set; }

        public bool UCatagory { get; set; } = false;
        public bool UType { get; set; } = false;
        public bool UStatus { get; set; } = false;

        public bool Terms { get; set; }

        public ICollection<IssueTable> issue { get; set; }
    }
}
