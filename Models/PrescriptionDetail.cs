using System.ComponentModel.DataAnnotations;

namespace HUTECH_Hospital.Models
{
    public class PrescriptionDetail
    {
        public int Id { get; set; }

        public int PrescriptionId { get; set; }
        public Prescription? Prescription { get; set; }

        public int MedicineId { get; set; }
        public Medicine? Medicine { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng")]
        public int Quantity { get; set; }

        public string? Dosage { get; set; }
        public string? Usage { get; set; }
        public string? Note { get; set; }
    }
}
