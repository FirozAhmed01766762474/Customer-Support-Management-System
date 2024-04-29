using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using XFLCSMS.Models.Brocarage;

namespace XFLCSMS.Models.Branch
{
    public class BranchView
    {


        public int BranchId { get; set; }

        [Required(ErrorMessage = "BrokerageHouseName is required")]
        [RegularExpression(@"^[a-z A-Z.]+$", ErrorMessage = "Only letters allowed.")]
        public string BranchName { get; set; } = string.Empty;


        public string BrokerageHouseName { get; set; } = string.Empty;

        public int BrokerageId { get; set;} 

        public List<Brokerage>? brocarage { get; set; }



    }
}
