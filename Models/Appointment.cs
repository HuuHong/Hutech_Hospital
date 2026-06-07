using System;
using System.ComponentModel.DataAnnotations;

namespace HUTECH_Hospital.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }
        public Patient? Patient { get; set; }

        [Required]
        public int DoctorId { get; set; }
        public Doctor? Doctor { get; set; }

        [Required]
        public int DepartmentId { get; set; }
        public Department? Department { get; set; }

        public int? DoctorScheduleId { get; set; }
        public DoctorSchedule? DoctorSchedule { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public TimeSpan AppointmentTime { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập lý do khám")]
        [StringLength(500)]
        public string Reason { get; set; } = string.Empty;

        [StringLength(50)]
        public string Status { get; set; } = "Pending";

        public string? Note { get; set; }

        public string? CancelReason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public MedicalRecord? MedicalRecord { get; set; }
    }
}
