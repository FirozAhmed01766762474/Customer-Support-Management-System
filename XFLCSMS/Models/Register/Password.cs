using Org.BouncyCastle.Bcpg;
using System.ComponentModel.DataAnnotations;

namespace XFLCSMS.Models.Register
{
    public class Password
    {
        public int UserId { get; set; }
        public string? CurrentPassword { get; set; }
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{4,}$",
        ErrorMessage = "Password must contain at least one lowercase letter, one uppercase letter, one digit, and one special character")]
        public string? NewPassword { get; set; }
        [Required]
        public string? ConNewPassword { get; set; }



    }
}
