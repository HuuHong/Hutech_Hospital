using System;
using System.Collections.Generic;

namespace HUTECH_Hospital.ViewModels
{
    public class MedicalRecordViewModel
    {
        public int Id { get; set; }
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string Diagnosis { get; set; } = string.Empty;
        public string? Symptoms { get; set; }
        public string? Treatment { get; set; }
        public string? DoctorNote { get; set; }
        public DateTime? ReExaminationDate { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<PrescriptionDetailViewModel> PrescriptionDetails { get; set; } = new List<PrescriptionDetailViewModel>();
    }
}
