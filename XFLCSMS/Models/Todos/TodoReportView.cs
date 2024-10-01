using XFLCSMS.Models.Admin;
using XFLCSMS.Models.Brocarage;
using XFLCSMS.Models.Register;

namespace XFLCSMS.Models.Todos
{
    public class TodoReportView
    {
        public ICollection<User>? ListofEmployee { get; set; }
        public ICollection<Brokerage>? brokerages { get; set; }

        public TodoHederInfo? TodoHederInfo { get; set; }
        
        public TodoSearch? search { get; set; }
        public IEnumerable<Todo>? Todos { get; set; }
    }
}
