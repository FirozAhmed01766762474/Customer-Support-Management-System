using XFLCSMS.Models.Branch;
using XFLCSMS.Models.Brocarage;

namespace XFLCSMS.Models.Register
{
    public class RegisterViewModel
    {
        public List<Brokerage>? Brokerages { get; set; }

        public List<Branchh>? Branchhs { get; set; }

        public bool? Selected { get; set; }

        public int BrocarageId { get; set; }

        public string? Acronyme { get; set; }
       
        public  UserRegisterRequest userRegisterRequest { get; set; }
    }
}
