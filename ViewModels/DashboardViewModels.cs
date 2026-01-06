namespace _2022_CS_668.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalStudents { get; set; }
        public int TotalTeachers { get; set; }
        public int ActiveMenuItems { get; set; }
        public int PendingBills { get; set; }
        public int UnpaidBills { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public int TodayPresent { get; set; }
        public int TodayAbsent { get; set; }
    }

    public class ReportViewModel
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string ReportType { get; set; } = string.Empty;
        public object? Data { get; set; }
    }
}
