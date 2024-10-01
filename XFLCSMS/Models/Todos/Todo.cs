using System.ComponentModel.DataAnnotations;
using XFLCSMS.Models.Brocarage;
using XFLCSMS.Models.Register;

namespace XFLCSMS.Models.Todos
{
    public class Todo
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Todoname { get; set; }
        public string Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public int UserId { get; set; } // Foreign Key
        public User User { get; set; }

        public int BrokerageId { get; set; } 
    }
}
