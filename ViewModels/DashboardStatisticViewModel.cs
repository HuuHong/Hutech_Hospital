namespace HUTECH_Hospital.ViewModels
{
    public class DashboardStatisticViewModel
    {
        public int TotalPatients { get; set; }
        public int TotalDoctors { get; set; }
        public int TotalDepartments { get; set; }
        public int TotalAppointments { get; set; }
        public int TodayAppointments { get; set; }
        public int PendingAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public int TotalMedicalRecords { get; set; }
        public int TotalFeedbacks { get; set; }
        public int UnresolvedFeedbacks { get; set; }
    }
}
