using System.ComponentModel.DataAnnotations;

namespace XFLCSMS.Models.Login
{
    public class UserLoginRequest
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
