namespace XFLCSMS.Models.Todos
{
    public class TodoHederInfo
    {
        public string? BrokerageHouseName { get; set; }
        public string? EmployeeName { get; set; }
        public int? TotalTodo { get; set; }
        public int? TotalCompleteTodo { get; set; }
        public int? TotalInprogress { get; set; }
        public int? TotalCanseled { get; set; }

        public string? ReportName { get; set; }
    }
}
