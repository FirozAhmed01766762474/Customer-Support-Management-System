using System.ComponentModel.DataAnnotations;

namespace XFLCSMS.Models.Todos
{
    public class Updatetodo
    {
      
        [Required]
        public string Todoname { get; set; }
        [Required]
        public string Status { get; set; }
    }
}
