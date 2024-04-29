namespace XFLCSMS.Models.Issue
{
    public class IssueLoginInfo
    {
        public int UserId { get; set; }
        public string BrocarageHouseName { get; set; }= string.Empty;
        public string BranchName  { get; set; }= string.Empty;

        public string TicketID { get; set; }= string.Empty;
    }
}
