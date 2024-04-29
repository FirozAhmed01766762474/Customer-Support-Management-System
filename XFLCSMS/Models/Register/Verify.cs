using System.ComponentModel.DataAnnotations;

namespace XFLCSMS.Models.Register
{
    public class Verify
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Email { get; set; }=string.Empty;
        [Required]
        public string Token { get; set; }= string.Empty;
    }
}
