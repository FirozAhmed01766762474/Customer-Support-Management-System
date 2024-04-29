﻿
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XFLCSMS.Models.Register
{
    public class UserRegisterRequest
    {
        

        [Required(ErrorMessage = "Required")]
        //[RegularExpression(@"^[a-zA-Z.]+$", ErrorMessage = "Only letters are allowed.")]
        [RegularExpression(@"^[a-zA-Z\s.]+$", ErrorMessage = "Only letters and spaces are allowed.")]

        public String FullName { get; set; } = string.Empty;

        [Required, EmailAddress(ErrorMessage = "Required")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Required")]
        [RegularExpression("^[0-9+-]+$", ErrorMessage = "Invalid Phone Number")]
        public string PhonNumber { get; set; } = string.Empty;
    

        public string Designation { get; set; } = string.Empty;
        [Required]
        public int BrokerageHouseName { get; set; }
        [Required]
        public int Branch { get; set; }

        [Required]
        [StringLength(15, MinimumLength = 1, ErrorMessage = "EmployeeId must be 1 to 15 characters")]
        public string EmployeeId { get; set; } = string.Empty;
        [Required]
        [StringLength(25, MinimumLength = 5, ErrorMessage = "Username must be 5 to 25 characters")]
        public string UserName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{4,}$",
        ErrorMessage = "Password must contain at least one lowercase letter, one uppercase letter, one digit, and one special character.")]
        public string Password { get; set; } = string.Empty;
      

        [Required]
        public bool Terms { get; set; }
    }
}
