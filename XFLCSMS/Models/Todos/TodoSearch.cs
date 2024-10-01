namespace XFLCSMS.Models.Todos
{
    public class TodoSearch
    {
        public string? Status { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? EmployeeName { get; set; }
        public int? BrocarageHouseName { get; set; }
    }
}
