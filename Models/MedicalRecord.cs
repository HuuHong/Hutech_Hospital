using System;
using System.ComponentModel.DataAnnotations;

namespace HUTECH_Hospital.Models
{
    public class MedicalRecord
    {
        public int Id { get; set; }

        public int AppointmentId { get; set; }
        public Appointment? Appointment { get; set; }

        public int PatientId { get; set; }
        public Patient? Patient { get; set; }

        public int DoctorId { get; set; }
        public Doctor? Doctor { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập chẩn đoán")]
        public string Diagnosis { get; set; } = string.Empty;

        public string? Symptoms { get; set; }
        public string? Treatment { get; set; }
        public string? DoctorNote { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime? ReExaminationDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        // Navigation for Prescription
        public Prescription? Prescription { get; set; }
    }
}
