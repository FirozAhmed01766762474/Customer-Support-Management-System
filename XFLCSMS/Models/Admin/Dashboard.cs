using XFLCSMS.Models.Brocarage;

namespace XFLCSMS.Models.Admin
{
    public class Dashboard
    {
        //Total
        public int? TotalTicket { get; set; }
        public int? TotalClosed { get; set; }
        public int? TotalQueue { get; set; }
        //Today
        public int? TodayTotalTicket { get; set; }
        public int? TodayTotalClosed { get; set; }
        public int? TodayTotalQueue { get; set; }
        //Weekly
        public int? WeeklyTotalTicket { get; set; }
        public int? WeeklyTotalClosed { get; set; }
        public int? WeeklyTotalQueue { get; set; }
        //Monthly
        public int? MonthlyTotalTicket { get; set; }
        public int? MonthlyTotalClosed { get; set; }
        public int? MonthlyTotalQueue { get; set; }
        //Yearly
        public int? YearlyTotalTicket { get; set; }
        public int? YearlyTotalClosed { get; set; }
        public int? YearlyTotalQueue { get; set; }

        //public List<Brokerage>? Brokerages { get; set; }
         public dynamic? ChartData { get; set; }






    }
}
