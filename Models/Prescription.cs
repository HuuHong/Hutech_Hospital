using System;
using System.Collections.Generic;

namespace HUTECH_Hospital.Models
{
    public class Prescription
    {
        public int Id { get; set; }

        public int MedicalRecordId { get; set; }
        public MedicalRecord? MedicalRecord { get; set; }

        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<PrescriptionDetail> PrescriptionDetails { get; set; } = new List<PrescriptionDetail>();
    }
}
