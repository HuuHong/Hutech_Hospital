using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace HUTECH_Hospital.ViewModels
{
    public class PrescriptionDetailViewModel
    {
        public int MedicineId { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string? Dosage { get; set; }
        public string? Usage { get; set; }
        public string? Note { get; set; }

        public IEnumerable<SelectListItem>? Medicines { get; set; }
    }
}
