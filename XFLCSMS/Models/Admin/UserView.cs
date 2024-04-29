namespace XFLCSMS.Models.Admin
{
    public class UserView
    {

        public int Id { get; set; }
        public String FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhonNumber { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string BrokerageHouseName { get; set; } = string.Empty;
        public string Branch { get; set; } =string.Empty;
        public string EmployeeId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;

        public bool UCatagory { get; set; }
        public bool UType { get; set; } 
        public bool UStatus { get; set; }
    }
}
