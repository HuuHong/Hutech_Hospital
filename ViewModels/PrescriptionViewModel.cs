using System.Collections.Generic;

namespace HUTECH_Hospital.ViewModels
{
    public class PrescriptionViewModel
    {
        public int Id { get; set; }
        public int MedicalRecordId { get; set; }
        public string? Note { get; set; }

        public List<PrescriptionDetailViewModel> PrescriptionDetails { get; set; } = new List<PrescriptionDetailViewModel>();
    }
}
