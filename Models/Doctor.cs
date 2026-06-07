using System;

namespace HUTECH_Hospital.Models
{
    public class Doctor
    {
        public int Id { get; set; }
        
        // Foreign Key
        public string ApplicationUserId { get; set; } = null!;
        public ApplicationUser ApplicationUser { get; set; } = null!;
        
        // Cập nhật PHẦN 2: Thêm chuyên khoa
        public int DepartmentId { get; set; }
        public Department? Department { get; set; }
        
        // Extended Properties
        public string? DoctorCode { get; set; }
        public string? FullName { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? AvatarUrl { get; set; }
        
        // Professional Details
        public string? Degree { get; set; }
        public string? Specialization { get; set; }
        public int ExperienceYears { get; set; }
        public string? Description { get; set; }
        
        // Status & Timestamps
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation Properties: 1 Bác sĩ có nhiều Ca Khám
        public ICollection<DoctorSchedule> DoctorSchedules { get; set; } = new List<DoctorSchedule>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
