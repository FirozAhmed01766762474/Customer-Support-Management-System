using System.ComponentModel.DataAnnotations;

namespace XFLCSMS.Models.DataTable
{
    public class Customer
    {
        [Key]
        public int Cust_Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Country { get; set; }= string.Empty;
    }
}
