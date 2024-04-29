namespace XFLCSMS.Models.Issue
{
    public class EditTicket
    {
        public int IssueId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? AssgnOn { get; set; }
        public string? AssgnBy { get; set; }
        public string? SupportType { get; set; }
        public string? SupportCatagory { get; set; }
        public string? SupportSubCatagory { get; set; }
        public string? AffectedSection { get; set; }

        public string? Priority { get; set; }
        public string? TicketStatus { get; set; }
        public string? TicketDetails { get; set; }

        public string? Command { get; set; }
        public int? attachmentId { get; set; }

    }
}
