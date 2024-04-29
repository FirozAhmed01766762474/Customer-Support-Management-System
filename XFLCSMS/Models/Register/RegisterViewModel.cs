using XFLCSMS.Models.Branch;
using XFLCSMS.Models.Brocarage;

namespace XFLCSMS.Models.Register
{
    public class RegisterViewModel
    {
        public List<Brokerage>? Brokerages { get; set; }
       
        public  UserRegisterRequest userRegisterRequest { get; set; }
    }
}
