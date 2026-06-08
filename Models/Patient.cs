using System;

namespace HUTECH_Hospital.Models
{
    public class Patient
    {
        public int Id { get; set; }
        
        // Foreign Key
        public string ApplicationUserId { get; set; } = null!;
        public ApplicationUser ApplicationUser { get; set; } = null!;
        
        // Extended Properties
        public string? StudentCode { get; set; }
        public string? FullName { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        public HealthSurvey? HealthSurvey { get; set; }
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
    }
}
